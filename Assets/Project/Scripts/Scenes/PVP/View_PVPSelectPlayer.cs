using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_PVPSelectPlayer : ViewBase {

    public void Init(PvpUserData userData, PvpTeamData userTeamData, PvpTeamData[] oppnentList, Action openBpRecovery)
    {
        m_OpenBpRecovery = openBpRecovery;
        m_PvpUserData = userData;
        m_ContestId = userData.ContestId;
        m_SelectedTeam = null;
        m_OppnentItemList = new List<ListItem_PVPPlayer> ();
        m_BtBattleStart = GetScript<CustomButton> ("bt_BattleStart");

        // 自パーティーの設定
        var party = AwsModule.PartyData.PvPTeam;

        m_ViewBPGrid = GetScript<RectTransform> ("BP/BPGrid").gameObject.GetOrAddComponent<View_BPGrid>();
        m_ViewBPGrid.Init ();
        UpdatePvpUser (userData);

        SetParty (party, userTeamData);

        SetPVPOpponent(oppnentList);

        SetChangeListGem (m_PvpUserData.ListUpdateGem);


        SetCanvasCustomButtonMsg ("PartyEdit/bt_Common", DidTapPartyEdit);
        SetCanvasCustomButtonMsg ("bt_BattleStart", DidTapBattleStart, false);
        SetCanvasCustomButtonMsg ("ResetPVPList/bt_CommonS02", DidTapChangeList);
        SetCanvasCustomButtonMsg ("BP/bt_Charge", DidTapBPCharge);
    }

    public void LoadBG(Action didLoaded)
    {
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            didLoaded();
        });
    }

    public void UpdatePvpUser(PvpUserData userData)
    {
        m_PvpUserData = userData;
        m_ViewBPGrid.UpdateBP (userData.BattlePoint);
    }

    void SetParty(Party party, PvpTeamData userTeamData)
    {
        // 総合力
        GetScript<TextMeshProUGUI>("txtp_OwnTotalPoint").SetText(userTeamData.TotalOverallIndex);

        // partyIconの設定
        var parytIconRoot = GetScript<RectTransform>("PartyUnitGrid").gameObject;
        for (int partyIndex = 1; partyIndex <= Party.PartyCardMax; ++partyIndex) {
            if (party [partyIndex] == null) {
                continue;
            }
            var iconGo = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", parytIconRoot);
            iconGo.GetOrAddComponent<ListItem_UnitIcon> ().Init (party [partyIndex]);
        }
    }

    void SetChangeListGem(int gem)
    {
        GetScript<TextMeshProUGUI> ("txtp_Gem").SetText (gem);
    }

    void SetPVPOpponent(PvpTeamData[] oppnentList)
    {
        m_SelectedTeam = null;
        m_BtBattleStart.interactable = false;

        m_OppnentItemList.Clear ();
        int oppnentCount = oppnentList.Length;
        for (int opponentIndex = 1; opponentIndex <= 5; ++opponentIndex) {
            var opponentGo = GetScript<RectTransform> (string.Format ("ListItem_PVPPlayer{0}", opponentIndex)).gameObject;
            var opponentItem = opponentGo.GetOrAddComponent<ListItem_PVPPlayer> ();
            PvpTeamData opppnentTeam = null;
            if (opponentIndex - 1 < oppnentCount) {
                opppnentTeam = oppnentList [opponentIndex - 1];
            }
            opponentItem.Init (opppnentTeam, DidTapFrame);
            opponentItem.Selected = false;
            m_OppnentItemList.Add (opponentItem);
        }
    }

    void DidTapPartyEdit()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPVPPartyEdit (true, true));
    }
    void DidTapBattleStart()
    {
        if(m_PvpUserData.BattlePoint <= 0) {
            PopupManager.OpenPopupYN ("BPが足りません。BPを回復しますか？",
                () => {
                    if(m_OpenBpRecovery != null) {
                        m_OpenBpRecovery();
                    }
                }
            );
            return;
        }
        if (m_SelectedTeam != null) {
            LockInputManager.SharedInstance.IsLock = true;
            View_FadePanel.SharedInstance.IsLightLoading = true;
            SendAPI.PvpBeginBattle (m_ContestId, m_SelectedTeam.UserId,
                (result, response) => {
                    if(!result) {
                        LockInputManager.SharedInstance.IsLock = false;
                        View_FadePanel.SharedInstance.IsLightLoading = false;
                        if(response != null && response.ResultCode == (int)ServerResultCodeEnum.PVP_OPPONENT_NOT_AVAILABLE) {
                            // 対戦相手情報を更新する。
                            ErrorOpponentNotAvailable();
                        }
                        return;
                    }

                    if (response.ResultCode == (int)ServerResultCodeEnum.PVP_PARTY_NOT_FOUND) {
                        LockInputManager.SharedInstance.IsLock = false;
                        View_FadePanel.SharedInstance.IsLightLoading = false;
                        // PVPチーム編成に飛ばす
                        PopupManager.OpenPopupSystemOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                                ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                            });
                        });
                        return;
                    }

                    // BPが足りない
                    if (response.ResultCode == (int)ServerResultCodeEnum.LACK_OF_BATTLE_POINT) {
                        LockInputManager.SharedInstance.IsLock = false;
                        View_FadePanel.SharedInstance.IsLightLoading = false;
                        PopupManager.OpenPopupSystemYN ("BPが足りません。BPを回復しますか？",
                            () => {
                                if(m_OpenBpRecovery != null) {
                                    m_OpenBpRecovery();
                                }
                            }
                        );
                        return;
                    }

                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black,
                        () => {
                            LockInputManager.SharedInstance.IsLock = false;
                            View_FadePanel.SharedInstance.IsLightLoading = false;
                            ScreenChanger.SharedInstance.GoToPVPBattle(response.PvpBattleEntryData);
                        }
                    );
                }
            );
        }
    }

    void ErrorOpponentNotAvailable()
    {
        PopupManager.OpenPopupOK (TextData.GetText("PVP_OPPONENT_NOT_AVAILABLE"),
            () => {
                LockInputManager.SharedInstance.IsLock = true;
                View_FadePanel.SharedInstance.IsLightLoading = true;
                SendAPI.PvpGetOpponentList ((result, response) => {
                    LockInputManager.SharedInstance.IsLock = false;
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    if(!result) {
                        return;
                    }
                    AwsModule.UserData.UserData = response.UserData;
                    m_PvpUserData = response.PvpUserData;
                    SetPVPOpponent(response.OpponentPvpTeamDataList);
                    SetChangeListGem (m_PvpUserData.ListUpdateGem);
                });
            }
        );
    }

    void DidTapChangeList()
    {
        if (m_PvpUserData.ListUpdateGem > AwsModule.UserData.UserData.GemCount) {
            // gem足りないポップアップ
            PopupManager.OpenPopupSystemYN ("ジェムが足りません。購入しますか？",
                () => {
                    View_GemShop.OpenGemShop ();
                }
            );
            return;
        }
        PopupManager.OpenPopupSystemYN (
            m_PvpUserData.ListUpdateGem > 0 ? 
                string.Format("ジェムを{0}個使用して\n対戦相手を更新します。\nよろしいですか？", m_PvpUserData.ListUpdateGem) :
                "対戦相手を更新します。\nよろしいですか？",
            () => {
                LockInputManager.SharedInstance.IsLock = true;
                View_FadePanel.SharedInstance.IsLightLoading = true;

                SendAPI.PvpUpdateOpponentList (
                    (result, response) => {
                        LockInputManager.SharedInstance.IsLock = false;
                        View_FadePanel.SharedInstance.IsLightLoading = false;
                        if(!result) {
                            return;
                        }

                        if (response.ResultCode == 1106) {
                            // PVPチーム編成に飛ばす
                            PopupManager.OpenPopupSystemOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                                    ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                                });
                            });
                            return;
                        }

                        AwsModule.UserData.UserData = response.UserData;
                        m_PvpUserData = response.PvpUserData;
                        SetPVPOpponent(response.OpponentPvpTeamDataList);
                        SetChangeListGem (m_PvpUserData.ListUpdateGem);
                    }
                );
            }
        );
    }

    void DidTapFrame(ListItem_PVPPlayer item, PvpTeamData teamData)
    {
        if (!item.Selected) {
            m_SelectedTeam = teamData;
            item.Selected = true;
            m_BtBattleStart.interactable = true;
        } else {
            m_SelectedTeam = null;
            item.Selected = false;
            m_BtBattleStart.interactable = false;
        }
        m_OppnentItemList.ForEach (x => {
            if(x != item) {
                x.Selected = false;
            }
        });
    }

    void DidTapBPCharge()
    {
        if (m_OpenBpRecovery != null) {
            m_OpenBpRecovery ();
        }
    }

    View_BPGrid m_ViewBPGrid;
    PvpUserData m_PvpUserData;
    PvpTeamData m_SelectedTeam;
    int m_ContestId;
    List<ListItem_PVPPlayer> m_OppnentItemList;
    CustomButton m_BtBattleStart;

    Action m_OpenBpRecovery;
}
