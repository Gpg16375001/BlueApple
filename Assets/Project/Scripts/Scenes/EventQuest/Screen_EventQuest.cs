using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class Screen_EventQuest : ViewBase {
    public void Init(int EventId, EventQuestStageTypeEnum? StageType, int EventPoint, EventQuestAchievement[] questAchievements, EventShopProductData[] productDatas, Dictionary<string, GameObject> assetBundleObjects)
    {
        AssetBundleObjects = assetBundleObjects;
        View_PlayerMenu.DidTapBackButton += DidTapBackButton;

        EventQuest = MasterDataTable.event_quest [EventId];

        var now = GameTime.SharedInstance.Now;
        m_IsEnabelEvent = (now >= EventQuest.start_at && now <= EventQuest.end_at);
        m_IsExchangeOnly = (now > EventQuest.end_at && now <= EventQuest.exchange_time_limit);

        if (!m_IsEnabelEvent && !m_IsExchangeOnly) {
            ScreenChanger.SharedInstance.GoToEvent (() => {
                PopupManager.OpenPopupSystemOK ("開催していないイベントです。");
            });
            return;
        }

        // BGM再生
        if (!string.IsNullOrEmpty (EventQuest.bgm_name)) {
            SoundManager.SharedInstance.PlayBGM (EventQuest.bgm_name, true);
        }

        // BGを取得
        var bgPrefab = GetAssetBundlePrefabs("BG");
        if (bgPrefab) {
            var bgGo = GameObject.Instantiate (bgPrefab) as GameObject;
            gameObject.AddInChild (bgGo);
            m_ViewBG = bgGo.GetOrAddComponent<View_EventBG> ();
            m_ViewBG.Init(EventQuest);
        }


        var viewShop = GameObjectEx.LoadAndCreateObject("EventQuest/View_EventShop", this.gameObject);
        m_ViewShop = viewShop.GetOrAddComponent<View_EventShop> ();
        m_ViewShop.Init (EventId, EventPoint, productDatas, this);
        m_ViewShop.gameObject.SetActive (!m_IsEnabelEvent && m_IsExchangeOnly);
        if (m_IsEnabelEvent) {
            var viewSelect = GameObjectEx.LoadAndCreateObject ("EventQuest/View_EventSelect", this.gameObject);
            m_ViewSelect = viewSelect.GetOrAddComponent<View_EventSelect> ();
            m_ViewSelect.gameObject.SetActive (m_IsEnabelEvent);
            m_ViewSelect.Init (EventId, StageType, this, EventPoint, EventQuest, questAchievements);
            m_CurrentMode = DisplayMode.Select;
        } else {
            m_CurrentMode = DisplayMode.Shop;
        }

        if (m_ViewBG != null) {
            if (m_CurrentMode == DisplayMode.Select) {
                m_ViewBG.InSelect ();
            } else {
                m_ViewBG.InShop ();
            }
        }


        if (m_IsEnabelEvent) {
            var saveData = AwsModule.ProgressData.EventQuestSaveData;
            List<int> readed = null;
            if (saveData != null) {
                readed = AwsModule.ProgressData.EventQuestSaveData.GetReadedScenario (EventId);
            } else {
                readed = new List<int> ();
            }
            var scenarioSettings = MasterDataTable.event_quest_scenario_setting [EventId];
            List<EventQuestScenarioSetting> readSettings = new List<EventQuestScenarioSetting> ();
            foreach (var setting in scenarioSettings) {
                if (readed.Contains (setting.id)) {
                    continue;
                }
                if (setting.release_quest_id.HasValue) {
                    var archive = System.Array.Find (questAchievements, x => x.StageDetailId == setting.release_quest_id.Value);
                    if (archive == null || !archive.IsOpen) {
                        continue;
                    }
                }
                if (setting.Enable ()) {
                    readSettings.Add (setting);
                }
            }

            if (readSettings.Count > 0) {
                LoadAndPlayUtage (readSettings);
                return;
            }
        }
        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);


        #if DEFINE_DEVELOP
        GetScript<RectTransform>("Debug").gameObject.SetActive(true);
        SetCanvasCustomButtonMsg("ResetScenario/bt_Common", () => {
            AwsModule.ProgressData.ResetEventQuestReadScenarioId (EventId);
            PopupManager.OpenPopupOK("シナリオをリセットしました。");
        });
        #endif
    }

    public override void Dispose ()
    {
        if (m_ViewSelect != null) {
            m_ViewSelect.Dispose ();
        }
        if (m_ViewShop != null) {
            m_ViewShop.Dispose ();
        }
        if (m_ViewBG != null) {
            m_ViewBG.Dispose ();
        }
        if (AssetBundleObjects != null) {
            AssetBundleObjects.Clear ();
        }
        base.Dispose ();
    }

    public void OpenShop()
    {
        SetDisplayMode (DisplayMode.Shop);
    }

    public void OpenQuestSelect()
    {
        if (!m_IsEnabelEvent) {
            return;
        }

        SetDisplayMode (DisplayMode.Select);
    }

    public void SetEventPoint(int eventPoint)
    {
        if (m_ViewSelect != null) {
            m_ViewSelect.SetEventPoint (eventPoint);
        }
        if (m_ViewShop != null) {
            m_ViewShop.SetEventPoint (eventPoint);
        }
    }

    private void LoadAndPlayUtage(List<EventQuestScenarioSetting> readSettings)
    {
        // 黒背景のPrefabをロード
        ScenarioBG = GameObjectEx.LoadAndCreateObject("EventQuest/View_EventScenarioBG");
        ScenarioBG.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);

        UtageModule.SharedInstance.SetActiveCore(true);

        NextLoadAndPlayUtage (readSettings, 0);
    }

    private void NextLoadAndPlayUtage(List<EventQuestScenarioSetting> readSettings, int count)
    {
        View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips (View_FadePanel.FadeColor.Black, () => {
            if (readSettings.Count > count) {
                LoadUtageCore (readSettings [count], readSettings, count);
            } else {
                if(ScenarioBG != null) {
                    GameObject.Destroy(ScenarioBG);
                    ScenarioBG = null;
                }
                UtageModule.SharedInstance.SetActiveCore (false);
                UtageModule.SharedInstance.DestroyCore();

                // BGM再生
                if (!string.IsNullOrEmpty (EventQuest.bgm_name)) {
                    SoundManager.SharedInstance.PlayBGM (EventQuest.bgm_name, true);
                }

                View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
            }
        });
    }

    private void LoadUtageCore(EventQuestScenarioSetting setting, List<EventQuestScenarioSetting> readSettings, int count)
    {
        var prefab = GetAssetBundlePrefabs (setting.before_play_prefab);
        UtageModule.SharedInstance.LoadUseChapter (setting.adv_project_name, () => {

            if(prefab != null) {
                var go = GameObject.Instantiate (prefab) as GameObject;
                go.GetOrAddComponent<View_EventScenarioEffect>().StartEffect(() => {
                    LockInputManager.SharedInstance.IsLock = false;
                    UtageModule.SharedInstance.StartScenario (setting.scenario_name, () => {
                        AwsModule.ProgressData.SetEventQuestReadScenarioId (setting.event_quest_id, setting.id);
                        NextLoadAndPlayUtage (readSettings, count + 1);
                    }, true);
                });
            } else {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;

                UtageModule.SharedInstance.StartScenario (setting.scenario_name, () => {
                    AwsModule.ProgressData.SetEventQuestReadScenarioId (setting.event_quest_id, setting.id);

                    NextLoadAndPlayUtage (readSettings, count + 1);
                }, true);

            }
        });
    }

    private GameObject GetAssetBundlePrefabs(string name)
    {
        GameObject prefab = null;
        if (AssetBundleObjects != null && !string.IsNullOrEmpty(name)) {
            AssetBundleObjects.TryGetValue (name, out prefab);
        }
        return prefab;   
    }

    private void SetDisplayMode(DisplayMode mode)
    {
        if (m_CurrentMode == mode) {
            return;
        }

        if (m_CurrentMode == DisplayMode.Shop) {
            m_ViewShop.Hide ();
        }

        if (m_ViewSelect != null) {
            m_ViewSelect.gameObject.SetActive (mode == DisplayMode.Select);
        }
        if (m_ViewShop != null) {
            m_ViewShop.gameObject.SetActive (mode == DisplayMode.Shop);
        }
        if (m_ViewBG != null) {
            if (mode == DisplayMode.Select) {
                m_ViewBG.GotoSelect ();
            } else {
                m_ViewBG.GotoShop ();
            } 
        }
        m_CurrentMode = mode;
    }

    void DidTapBackButton()
    {
        if (m_CurrentMode == DisplayMode.Shop) {
            if (m_ViewShop.BackProc ()) {
                if (m_IsEnabelEvent) {
                    OpenQuestSelect ();
                } else {
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToEvent ();
                    });
                }
            }
            return;
        }

        if (m_ViewSelect.BackProc ()) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToEvent ();
            });
        }
    }

    private enum DisplayMode
    {
        Select = 0,
        Shop,

    }

    DisplayMode m_CurrentMode = DisplayMode.Select;

    bool m_IsEnabelEvent;
    bool m_IsExchangeOnly;
    View_EventSelect m_ViewSelect;
    View_EventShop m_ViewShop;
    View_EventBG m_ViewBG;
    GameObject ScenarioBG;
    EventQuest EventQuest;

    Dictionary<string, GameObject> AssetBundleObjects;

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }
}
