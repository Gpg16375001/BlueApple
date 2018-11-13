using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

using com.adjust.sdk;

/// <summary>
/// Screen : パーティ編成.
/// </summary>
public class Screen_PartyEdit : ViewBase
{
    enum EditMode {
        PartyEdit,
        MemberEdit,
        Sorting
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(bool bBattleInit = false, bool isPvP = false, bool isPvPOpponentSelect = false, SupporterCardData supportCard = null, string prevScene = null)
    {
        m_PrevSceneName = prevScene;
        m_IsPvP = isPvP;
        m_IsBattleInit = bBattleInit;
        m_IsPvPOpponentSelect = isPvPOpponentSelect;
        m_EditMode = EditMode.PartyEdit;

        // サポート情報の保存と復元
        if (bBattleInit && !isPvP) {

            if (supportCard != null) {
                AwsModule.BattleData.SupportCard = supportCard;
            } else {
                supportCard = AwsModule.BattleData.SupportCard;
            }         
        }

        // TODO : とりあえずグローバルメニュー類の設定.
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_GlobalMenu.DidTapButton += GlobalMenuDidTapButton;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackFromPlayerMenu;

        // ボタン登録.
        this.SetCanvasCustomButtonMsg("bt_BattleStart", DidTapSortie);
        this.SetCanvasCustomButtonMsg("bt_Decide", DidTapDecide);
        this.SetCanvasCustomButtonMsg("bt_PartySave", DidTapPartySave);

        // バトルかどうかで切り替える各種設定.
        ChangeSaveButton();

        this.SetCanvasCustomButtonMsg("PartyDetails/bt_Common", DidTapTeamInfo);
        this.SetCanvasCustomButtonMsg("PartyUnitChange/bt_Common", DidTapChangeMember);
        this.SetCanvasCustomButtonMsg("PartyClear/bt_Common", DidTapTeamClear);
        this.SetCanvasCustomButtonMsg("bt_FormationChange", DidTapFormation);
        this.GetScript<InputField>("PartyName/bt_InputArea").onValueChanged.AddListener(DidChangeTeamName);
        this.SetCanvasCustomButtonMsg("PartyChange/bt_CommonS01", DidTapChangeTeam);
        if (m_IsPvP) {
            // Pvpの時はチーム変更できない
            GetScript<RectTransform> ("PartyChange").gameObject.SetActive (false);
        } else {
            GetScript<RectTransform> ("PartyChange").gameObject.SetActive (true);
        }

        // チーム情報の設定
        var party = EditParty;
        // TODO: デバッグ用
        if (!party.IsModify && party.IsEmpty && !m_IsPvP && AwsModule.PartyData.CurrentTeamIndex == 0) {
            party.InitForDevelop ();
        }
        SetParty (party, supportCard);

		// 初回起動
		if (bBattleInit && !isPvP) {
			if (AwsModule.ProgressData.IsFirstBoot) {
				TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.Sortie, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
			}
		}

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    void SetParty(Party party, SupporterCardData supportCard = null)
    {
        // チーム名設定
        this.GetScript<InputField> ("PartyName/bt_InputArea").text = party.Name;

        // フォーメーション設定
        SetPartyFormation(party);

        // ユニット情報の設定
        InitPartyUnit(party);

        // ヘルパー情報の設定
        SetHelper(party, supportCard);

        m_PartyEditUnitItems.ForEach (x => x.EnableLongPress = true);
        m_PartyEditUnitItemHelper.EnableLongPress = true;
    }

    void SetPartyFormation(Party party)
    {
        if (party.FormationData == null) {
            this.GetScript<TextMeshProUGUI> ("txtp_FormationName").SetText ("未設定");
            return;
        }
        this.GetScript<TextMeshProUGUI>("txtp_FormationName").SetText(party.FormationData.Formation.name);
        var formationIconRoot = this.GetScript<Transform> ("Formation/FormationIconRoot");
        var foramtionIcon = GameObjectEx.LoadAndCreateObject ("PartyEdit/FormationIcon", formationIconRoot.gameObject);
        var foramtionIconScript = foramtionIcon.GetComponent<FormationIcon> ();
        foramtionIconScript.Init (party.FormationData.Formation);
    }

    void InitPartyUnit(Party party)
    {
        for (int position = 1; position <= Party.PartyCardMax; ++position) {
            var partyEditUnit = this.GetScript<ListItem_PartyEditUnit> (string.Format("ListItem_PartyEditUnit{0}", position));
            m_PartyEditUnitItems.Add (partyEditUnit);
            m_PartyEditUnitItems[position - 1].Init (party[position], party.FormationData, position, DidTapPartyEditUnit, DidLongPressPartyEditUnit);
        }
    }

    void UpdatePartyUnit(Party party)
    {
        for (int position = 1; position <= Party.PartyCardMax; ++position) {
            m_PartyEditUnitItems[position - 1].UpdateUnit (party[position], party.FormationData, position);
        }
    }

    void SetHelper(Party party, SupporterCardData helper)
    {
        var partyEditHelperUnit = this.GetScript<ListItem_PartyEditSupporterUnit> ("ListItem_PartyEditUnitHelper");
        m_PartyEditUnitItemHelper = partyEditHelperUnit;
        if (m_IsBattleInit && !m_IsPvP) {
            m_HelperCard = helper;
            m_PartyEditUnitItemHelper.Init (helper, party.FormationData, HelperPosition, null, DidLongPressPartyEditSupporterUnit);
        } else {
            m_HelperCard = null;
            m_PartyEditUnitItemHelper.gameObject.SetActive (false);
        }
    }

    void SaveParty(System.Action didSave)
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        AwsModule.PartyData.Sync ((bSuccess, sender, eArgs) => {
            View_FadePanel.SharedInstance.IsLightLoading = false;
            if(!bSuccess) {
                PopupManager.OpenPopupSystemOK("編成の保存に失敗しました。通信環境のいい場所でもう一度保存を試してみてください。"); 
            } else {
                if(didSave != null) {
                    didSave();
                }
            }
            LockInputManager.SharedInstance.IsLock = false;
        });
    }

    #region ButtonDelegate.
    // コールバック : プレイヤーメニューからの戻るボタン押下時.
    void DidTapBackFromPlayerMenu()
    {
        if(m_EditMode != EditMode.PartyEdit){
            ChangePartyEditMode();
            return;
        }

        if (EditParty.IsEmpty) {
            PopupManager.OpenPopupYN("ユニットが一体も設定されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    BackScreen ();
                },
                () => {
                }
            );
            return;
        }

        // 変更が保存されていない場合
        if (AwsModule.PartyData.IsModify) {
            PopupManager.OpenPopupYN("変更が保存されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    BackScreen ();
                },
                () => {
                }
            );
            return;
        }

        BackScreen ();
    }

    void BackScreen()
    {
        if (m_IsPvP) {
            if (EditParty.IsEmpty) {
                if (m_PrevSceneName == "PartyEditTop") {
                    View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToPartyEditTop ();
                    });
                } else {
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToMyPage ();
                    });
                }

            } else {
                if (m_PrevSceneName == "PartyEditTop") {
                    View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToPartyEditTop ();
                    });
                } else if (m_PrevSceneName == "PVP") {
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToPVP (m_IsPvPOpponentSelect);
                    });
                } else {
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToMyPage ();
                    });
                }
            }
            return;
        } else if(m_IsBattleInit) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToFriendSelect());
        } else {
            View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPartyEditTop ());
        }
    }


    void GlobalMenuDidTapButton(System.Action exec)
    {
        if (EditParty.IsEmpty) {
            PopupManager.OpenPopupYN("ユニットが一体も設定されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    exec ();
                },
                () => {
                }
            );
            return;
        }

        // 変更が保存されていない場合
        if (AwsModule.PartyData.IsModify) {
            PopupManager.OpenPopupYN("変更が保存されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    exec ();
                },
                () => {
                }
            );
            return;
        }

        exec ();
    }

    // ボタン : 出撃.
    void DidTapSortie()
    {      
		// バトル開始.
		if (AwsModule.ProgressData.CurrentQuest == null) {
            return;
        }

		// 初回起動ここで終わり.
        if (AwsModule.ProgressData.IsFirstBoot) {
			TutorialFirstBootModule.DestroyInstance();
			AwsModule.ProgressData.IsFirstBoot = false;
            // IsFirstBootの進捗を保存する。
            AwsModule.ProgressData.Sync ();

            // チュートリアル突破イベントの通知
            AdjustEvent adjustEvent = new AdjustEvent("vv5boh");
            adjustEvent.addCallbackParameter("uid", AwsModule.UserData.UserID.ToString());
            Adjust.trackEvent(adjustEvent);
        }

        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;

        // スキルによるドロップ率・経験値・クレド増加割合分を計算する。
        int OverrideExpPercentage = 0;
        int OverrideGoldPercentage = 0;
        int OverrideItemDropPercentage = 0;
        if (m_HelperCard != null) {
            if (m_HelperCard.Parameter.PassiveSkillList.Any (x => x.Skill.HasItemDropUpLogic ())) {
                OverrideItemDropPercentage += m_HelperCard.Parameter.PassiveSkillList.Where (x => x.Skill.HasItemDropUpLogic ()).Sum (x => x.GetItemDropUp ());
            }
            if (m_HelperCard.Parameter.PassiveSkillList.Any (x => x.Skill.HasExpUpLogic ())) {
                OverrideItemDropPercentage += m_HelperCard.Parameter.PassiveSkillList.Where (x => x.Skill.HasExpUpLogic ()).Sum (x => x.GetExpUp ());
            }
            if (m_HelperCard.Parameter.PassiveSkillList.Any (x => x.Skill.HasMoneyUpLogic ())) {
                OverrideItemDropPercentage += m_HelperCard.Parameter.PassiveSkillList.Where (x => x.Skill.HasMoneyUpLogic ()).Sum (x => x.GetMoneyUp ());
            }
        }
        for (int i = 1; i <= Party.PartyCardMax; ++i) {
            var cardData = EditParty [i];
            if (cardData == null) {
                continue;
            }

            if (cardData.Parameter.PassiveSkillList.Any (x => x.Skill.HasItemDropUpLogic ())) {
                OverrideItemDropPercentage += cardData.Parameter.PassiveSkillList.Where (x => x.Skill.HasItemDropUpLogic ()).Sum (x => x.GetItemDropUp ());
            }
            if (cardData.Parameter.PassiveSkillList.Any (x => x.Skill.HasExpUpLogic ())) {
                OverrideItemDropPercentage += cardData.Parameter.PassiveSkillList.Where (x => x.Skill.HasExpUpLogic ()).Sum (x => x.GetExpUp ());
            }
            if (cardData.Parameter.PassiveSkillList.Any (x => x.Skill.HasMoneyUpLogic ())) {
                OverrideItemDropPercentage += cardData.Parameter.PassiveSkillList.Where (x => x.Skill.HasMoneyUpLogic ()).Sum (x => x.GetMoneyUp ());
            }
        }

        SendAPI.QuestsOpenQuest(
            AwsModule.ProgressData.CurrentQuest.ID,
            AwsModule.ProgressData.CurrentQuest.BattleStageID,
            new int[5] {
                EditParty.Position1_CardID,
                EditParty.Position2_CardID,
                EditParty.Position3_CardID,
                EditParty.Position4_CardID,
                EditParty.Position5_CardID
            },
            AwsModule.BattleData.SupportUserId,
            AwsModule.BattleData.SupportCard != null ? AwsModule.BattleData.SupportCard.CardId : 0,
            OverrideExpPercentage, OverrideGoldPercentage, OverrideItemDropPercentage,
            (success, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!success) {
                    Debug.LogError("SendBattlesOpenBattleStage Error");
                    return; // エラー.
                }
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    AwsModule.BattleData.BattleEntryData = res.BattleEntryData;
                    ScreenChanger.SharedInstance.GoToBattle();
                });
            }
        );
    }

    void DidTapDecide()
    {
        if (EditParty.IsEmpty) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToMyPage ();
            });
        } else {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToPVP (m_IsPvPOpponentSelect);
            });
        }
    }

    void DidTapPartySave()
    {
        var party = EditParty;
        if (EditParty.IsEmpty) {
            if (EditParty.FormationData == null) {
                // 陣形設定されてない
                PopupManager.OpenPopupOK("陣形が設定されていません。", DidTapFormation);
            } else {
                // ユニットが編成されていない
                PopupManager.OpenPopupOK("ユニットが設定されていません。");
            }
            return;
        }
        SaveParty (() => {
            ChangePartyEditMode();
        });
    }
        
    void DidChangeTeamName(string value)
    {
        var party = EditParty;

        party.SetName(value);

        // 変更があった時用のボタン切り替え処理
        ChangeSaveButton();
    }

    // ボタン : チーム変更
    void DidTapChangeTeam()
    {
        if (EditParty.IsEmpty && EditParty.IsModify) {
            PopupManager.OpenPopupSystemYN (
                "無効なチーム編成になっています。リセットをした上でチーム選択へ遷移しますか？",
                () => {
                    EditParty.Reset();
                    View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToPartyChange(m_IsBattleInit);
                    });
                }
            );

            return;
        }

        View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToPartyChange(m_IsBattleInit);
        });

    }

    // ボタン : 陣形
    void DidTapFormation()
    {
        View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black,
            () => ScreenChanger.SharedInstance.GoToFormationSelect(m_IsBattleInit, m_IsPvP, NextSceneBackCallback()));
    }

    // ボタン : チームクリア
    void DidTapTeamClear()
    {
        IsEnableButton = false;
        PopupManager.OpenPopupYN ("現在のチームを解散します。\n再度メンバーを登録してください。",
            () => {
                var party = EditParty;
                for (int index = 1; index <= Party.PartyCardMax; ++index) {
                    party[index] = null;
                }
                UpdatePartyUnit(party);

                IsEnableButton = true;

                // ボタンなどの再設定をする
                ChangePartyEditMode();
            },
            () => {
                IsEnableButton = true;
            }
        );
    }

    // ボタン : メンバー変更
    void DidTapChangeMember()
    {
        if (m_EditMode == EditMode.Sorting) {
            ChangePartyEditMode ();
        } else {
            ChangeSortingMode ();
            m_SelectPosition = -1;
        }
    }

    // ボタン : チーム情報
    void DidTapTeamInfo()
    {
        View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black,
            () => ScreenChanger.SharedInstance.GoToPartyDetails (m_IsBattleInit, m_IsPvP, NextSceneBackCallback()));
    }

    Party EditParty {
        get {
            if (m_IsPvP) {
                return AwsModule.PartyData.PvPTeam;
            }
            return AwsModule.PartyData.CurrentTeam;
        }
    }

    void ChangeSortingMode()
    {
        // 色々ボタン効かなくする
        this.GetScript<CustomButton> ("bt_BattleStart").interactable = false;
        this.GetScript<CustomButton> ("bt_Decide").interactable = false;
        this.GetScript<CustomButton> ("PartyDetails/bt_Common").interactable = false;
        this.GetScript<CustomButton> ("PartyUnitChange/bt_Common").interactable = true;
        this.GetScript<CustomButton> ("PartyClear/bt_Common").interactable = false;
        this.GetScript<CustomButton> ("bt_FormationChange").interactable = false;
        this.GetScript<InputField> ("PartyName/bt_InputArea").interactable = false;
        this.GetScript<CustomButton> ("PartyChange/bt_CommonS01").interactable = false;

        m_PartyEditUnitItems.ForEach(x => {
            x.EnableLongPress = false;
        });
        m_PartyEditUnitItemHelper.EnableLongPress = false;

        if (m_IsBattleInit && !m_IsPvP) {
            // サポートユニットは操作できないようにする。
            m_PartyEditUnitItemHelper.IsEnableButton = false;
        }
        m_EditMode = EditMode.Sorting;
    }

    void ChangePartyEditMode()
    {
        // ボタンを効くようにする
        ChangeSaveButton();

        this.GetScript<CustomButton> ("PartyDetails/bt_Common").interactable = true;
        this.GetScript<CustomButton> ("PartyUnitChange/bt_Common").interactable = true;
        this.GetScript<CustomButton> ("PartyClear/bt_Common").interactable = true;
        this.GetScript<CustomButton> ("bt_FormationChange").interactable = true;
        this.GetScript<InputField> ("PartyName/bt_InputArea").interactable = true;
        this.GetScript<CustomButton> ("PartyChange/bt_CommonS01").interactable = true;

        // 強制選択状態を全て解除する。
        m_PartyEditUnitItems.ForEach(x => {
            x.ForceHighlight = false;
            x.EnableLongPress = true;
        });
        m_PartyEditUnitItemHelper.EnableLongPress = false;
        if (m_IsBattleInit && !m_IsPvP) {
            // サポートユニットは操作できるようにする。
            m_PartyEditUnitItemHelper.IsEnableButton = true;
        }
        this.GetScript<CustomButton> ("PartyUnitChange/bt_Common").ForceHighlight = false;

        // 選択情報をクリア
        m_SelectPosition = 0;

        m_EditMode = EditMode.PartyEdit;
    }

    void ChangeSaveButton()
    {
        // ボタンを効くようにする
        var btBattleStart = this.GetScript<CustomButton> ("bt_BattleStart");
        var btDecide = this.GetScript<CustomButton> ("bt_Decide");
        var btPartySave = this.GetScript<CustomButton> ("bt_PartySave");

        var party = EditParty;
        btBattleStart.gameObject.SetActive(m_IsBattleInit && !AwsModule.PartyData.IsModify);
        btBattleStart.interactable = !party.IsEmpty;
        btDecide.gameObject.SetActive(m_IsBattleInit && m_IsPvP && !AwsModule.PartyData.IsModify);
        btDecide.interactable = !party.IsEmpty;
        btPartySave.gameObject.SetActive(!m_IsBattleInit || AwsModule.PartyData.IsModify);
        btPartySave.interactable = !(!party.IsEmpty && !AwsModule.PartyData.IsModify);
    }

    bool DidTapPartyEditUnit (int position, int cardID)
    {
        var party = EditParty;
        bool forceSelected = false;
        // フレンドユニットは特殊処理
        if (position == 6) {
            return false;
        }

        if (m_EditMode == EditMode.Sorting) {
            // 入れ替えをする
            if (m_SelectPosition < 0) {
                m_SelectPosition = position;
                forceSelected = true;
            }
            if (position != m_SelectPosition) {
                var cardData = party [position];
                party [position] = party[m_SelectPosition];
                party [m_SelectPosition] = cardData;
                UpdatePartyUnit(party);
                m_SelectPosition = -1;

                // ハイライトをfalseに変更
                m_PartyEditUnitItems.ForEach (x => {
                    x.ForceHighlight = false;
                });
                m_PartyEditUnitItemHelper.ForceHighlight = false;

                // ボタンを効くようにする
                ChangeSaveButton();
            }
        } else {
            OpenViewChangeUnit (position, party);
        }
        return forceSelected;
    }

    void DidLongPressPartyEditUnit(CardData card, bool supportCard)
    {
        if (card == null) {
            return;
        }
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToUnitDetails (card, NextSceneBackCallback(), false, supportCard));
    }

    void DidLongPressPartyEditSupporterUnit(SupporterCardData card, bool supportCard)
    {
        if (card == null) {
            return;
        }
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToUnitDetails (card, NextSceneBackCallback(), false, supportCard));
    }

    void OpenViewChangeUnit(int position, Party party)
    {
        m_SelectPosition = position;

        View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => 
            ScreenChanger.SharedInstance.GoToSelectUnit (
                m_IsBattleInit,
                true,
                m_IsPvP,
                true,
                party[position] != null ? party[position].CardId : 0,
                0, null, false,
                (card) => {
                    // 選択されたカードを設定
                    var nowPos = party.GetPosition(card);
                    if(nowPos < 0) {
                        party[position] = card;
                    } else if(nowPos == position) {
                        // 同じ位置に同じカードの指定がきた場合は外す処理
                        party [position] = null;
                    } else {
                        var cardData = party [position];
                        party [position] = card;
                        party[nowPos] = cardData;
                    }
                },
                NextSceneBackCallback()
            )
        );
    }


    System.Action NextSceneBackCallback()
    {
        System.Action backCallback = new System.Action (() => {
            View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPartyEdit ());
        });
        if (m_IsPvP) {
            backCallback = new System.Action (() => {
                View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPVPPartyEdit (m_IsBattleInit, m_IsPvPOpponentSelect, m_PrevSceneName));
            });
        } else if (m_IsBattleInit) {
            backCallback = new System.Action (() => {
                View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToQuestPreparation (m_HelperCard));
            });
        }

        return backCallback;
    }

    #endregion

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    private string m_prevSceneName;
    private EditMode m_EditMode;

    private List<ListItem_PartyEditUnit> m_PartyEditUnitItems = new List<ListItem_PartyEditUnit> ();
    private ListItem_PartyEditSupporterUnit m_PartyEditUnitItemHelper;
    private int m_SelectPosition;

    private bool m_IsBattleInit;
    private bool m_IsPvP;
    private bool m_IsPvPOpponentSelect;
    private SupporterCardData m_HelperCard;

    private const int HelperListIndex = 5;
    private const int HelperPosition = 6;

    private string m_PrevSceneName;
}
