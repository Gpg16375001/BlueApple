using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;
using System.Linq;

public class View_LoginBonus : ViewBase
{
    public static IEnumerator Create(LoginbonusData loginbonusData, Action<View_LoginBonus> endCallback = null)
    {
        AssetBundleRef abRef = null;
        bool onLoad = false;

        DLCManager.AssetBundleFromFileOrDownload (
            DLCManager.DLC_FOLDER.LoginBonus,
            string.Format ("loginbonus_{0}", loginbonusData.LoginbonusId),
            (_ref) => {
                abRef = _ref;
                onLoad = true;
            },
            (_error) => {
                onLoad = true;
            },
            (_progress) => {
                //ロード進行中
            }
        );
            
        while (!onLoad) {
            yield return null;
        }

        View_LoginBonus viewLoginBonus = null;
        GameObject go = null;

        if (abRef != null) {
            //ロード成功
            var asynObj = abRef.assetbundle.LoadAllAssetsAsync<GameObject> ();

            yield return asynObj;

            var _gameObjects = new Dictionary<string, GameObject> ();
            foreach (var asset in asynObj.allAssets) {
                if (asset != null && asset as GameObject) {
                    _gameObjects.Add (asset.name, asset as GameObject);
                }
            }
            go = GameObject.Instantiate(_gameObjects ["View_EventLoginBonusPop"]);
        } else {
            //ロード失敗 通常通りロードを行う
            go = GameObjectEx.LoadAndCreateObject("Mypage/View_LoginBonusPop");
        }

        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        viewLoginBonus = go.GetOrAddComponent<View_LoginBonus>();
        viewLoginBonus.InitInternal(loginbonusData);

        if (endCallback != null) {
            endCallback (viewLoginBonus);
        }
    }

    // 内部初期化.
    private void InitInternal(LoginbonusData loginbonusData, Action didTapClose = null)
    {
        m_Close = true;
        m_Open = false;

        // ボタン設定. アセットバンドルからロードした場合はボタンがついていないのでAddする
        var _close = GetScript<Transform>("bt_Close").gameObject;
        if (_close.GetComponent<SmileLab.UI.CustomButton> () == null) {
            var button = _close.AddComponent<SmileLab.UI.CustomButton> ();
            button.transition = Selectable.Transition.None;
        }
        var _bg = GetScript<Transform> ("Bg").gameObject;
        if (_bg.GetComponent<SmileLab.UI.CustomButton> () == null) {
            var button =_bg.AddComponent<SmileLab.UI.CustomButton> ();
            button.transition = Selectable.Transition.None;
        }

        this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
        this.SetCanvasCustomButtonMsg("Bg", DidTapClose);

        foreach (var days in loginbonusData.CurrentRoundDayCountList) {
            var loginbonusItem = MasterDataTable.loginbonus_dist_package.Get (loginbonusData.LoginbonusId, days);
            bool isToday = false;
            if (days == loginbonusData.LastReceivedDayCount) {
                //本日
                if (this.Exist<RectTransform> ("TodayItemAnchor")) {
                    var toDayObj = GameObjectEx.LoadAndCreateObject ("Mypage/ListItem_LoginBonusTodayBonus", GetScript<RectTransform> ("TodayItemAnchor").gameObject);
                    var toDayItem = toDayObj.AddComponent<ListItem_LoginBonusDailyItem> ();
                    toDayItem.Init (false, false, null, loginbonusItem);
                }
                isToday = true;

                dispLive2d = loginbonusItem.disp_card_id;
                playVoiceCue = loginbonusItem.voice_cue_id;
            }

            if (this.Exist<RectTransform> (string.Format ("DailyItemAnchor{0}", days))) {
                var dayObj = GameObjectEx.LoadAndCreateObject ("Mypage/ListItem_LoginBonusDailyItem", GetScript<RectTransform> (string.Format ("DailyItemAnchor{0}", days)).gameObject);
                var dayItem = dayObj.GetComponent<ListItem_LoginBonusDailyItem> ();
                dayItem.Init (
                    isToday,
                    loginbonusData.LastReceivedDayCount > days,
                    days,
                    loginbonusItem
                );
            }
        }

        // Live2Dの読み込み
        if (this.Exist<RectTransform> ("CharacterAnchor")) {
            this.GetScript<RectTransform> ("CharacterAnchor").gameObject.DestroyChildren ();
            loader = new UnitResourceLoader (dispLive2d); // デフォルト=司書
            loader.LoadFlagReset ();
            loader.IsLoadLive2DModel = true;
            loader.IsLoadVoiceFile = true;

            loader.LoadResource (
                (resource) => {
                    var go = Instantiate (resource.Live2DModel) as GameObject;
                    var canvas = this.GetScript<Canvas> ("CharacterAnchor");
                    go.transform.SetParent (canvas.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    var cubismRender = go.GetComponentsInChildren<Live2D.Cubism.Rendering.CubismRenderController> () [0];
                    cubismRender.SortingLayerId = canvas.sortingLayerID;
                    cubismRender.SortingOrder = canvas.sortingOrder;

                    playVoice = () => {
                        m_Live2dVoicePlayer = go.GetOrAddComponent<Live2dVoicePlayer> ();
                        if (dispLive2d == 308001011) {
                            // 司書だった場合    
                            m_Live2dVoicePlayer.Play ("VOICE_308001011", string.Format ("greeting_00{0}", UnityEngine.Random.Range (1, 4)));
                        } else {
                            // 特定のキャラだった場合の処理
                            try
                            {
                                // 数字のみの場合Falseになる
                                if(((SoundVoiceCueEnum)playVoiceCue).ToString ().Except("0123456789").Any()) {
                                    var master = MasterDataTable.card[dispLive2d];
                                    m_Live2dVoicePlayer.Play(master.voice_sheet_name, (SoundVoiceCueEnum)playVoiceCue);
                                } else {
                                    Debug.LogWarning("設定にないIDが指定されています");
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning(e.Message);
                            }
                        }
                    };
                }
            );
        }

        if (Exist<TextMeshProUGUI> ("txtp_LoginBonusDuration")) {
            // テキストをDateTime型にパースして出力
            var endDate = DateTime.Parse (loginbonusData.EndDate);
            GetScript<TextMeshProUGUI> ("txtp_LoginBonusDuration").SetText (endDate.ToString("開催期間：M月d日 HH:mm まで"));
            GetScript<TextMeshProUGUI> ("txtp_LoginBonusDuration").font = Resources.Load<TMP_FontAsset> (dateFont);
        }
    }

    /// <summary>
    /// ポップアップの開くアニメーションを実行
    /// Enableになったタイミングだと暗転時だったりするので明示的に再生できるように
    /// </summary>
    public void Open(Action didTapClose = null)
    {
        if (m_Open) {
            return;
        }

        m_Open = true;
        m_didTapClose = didTapClose;
        PlayOpenCloseAnimation (true, () => {
            m_Close = false;
        });
    }

    // ボタン : 閉じる.
    void DidTapClose()
    {
        if (m_Close) {
            return;
        }

        m_Close = true;
        PlayOpenCloseAnimation (false, () => {
            if (m_didTapClose != null) {
                m_didTapClose ();
                m_Open = false;
            }
            this.Dispose ();
        });
    }

    // 開閉アニメーション処理.
    private void PlayOpenCloseAnimation(bool bOpen, System.Action didEnd = null)
    {
        this.StartCoroutine(CoPlayOpenClose(bOpen, didEnd));
    }
    IEnumerator CoPlayOpenClose(bool bOpen, System.Action didEnd)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play(bOpen ? "LoginBonusOpen" : "LoginBonusClose");
        do {
            yield return null;

            if(bOpen && View_FadePanel.SharedInstance.CurrentState != View_FadePanel.FadeState.FadeIn) {
                View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black, playVoice);
            }
            else if(!bOpen && View_FadePanel.SharedInstance.CurrentState != View_FadePanel.FadeState.FadeOut){
                View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black);
            }

        } while(anim.isPlaying);
            
        if(didEnd != null){
            didEnd();
        }
    }

    public override void Dispose ()
    {
        if (m_Live2dVoicePlayer != null) {
            m_Live2dVoicePlayer.Stop ();
            m_Live2dVoicePlayer.gameObject.SetActive (false);
        }
        loader.Dispose ();
        base.Dispose ();
    }

    public bool IsLoaded()
    {
        return loader.IsLoaded;
    }

    void Awake()
    {
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }

    bool m_Close = false;
    bool m_Open = false;
    private Action m_didTapClose;
    private Live2dVoicePlayer m_Live2dVoicePlayer;
    UnitResourceLoader loader;
    bool bgLoaded;
    int dispLive2d = 0;
    int playVoiceCue = 0;
    Action playVoice = null;
    const string dateFont = "Font/FOT-MATISSEPRON-DB SDF";
}