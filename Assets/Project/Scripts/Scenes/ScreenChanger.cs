using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SmileLab.Net.API;

namespace SmileLab
{
    public partial class ScreenChanger
    {
        #region 各種シーン移動処理.

        /// <summary>
        /// 冒頭チュートリアルに移動.
        /// </summary>
		public void GoToTutorial(Action didProcEnd = null)
		{
			View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = false;
			var ctrl = ScreenControllerBase.Create<TutorialSController>();
            ScreenChanger.SharedInstance.Exec("Tutorial", ctrl, didProcEnd);
		}

        /// <summary>
        /// タイトルに移動
        /// </summary>
        public void GoToTitle(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = false;
            var ctrl = ScreenControllerBase.Create<TitleSController>();
            ScreenChanger.SharedInstance.Exec("Title", ctrl, didProcEnd);
        }

        /// <summary>
        /// マイページに移動
        /// </summary>
        public void GoToMyPage(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<MyPageSController>();
            ScreenChanger.SharedInstance.Exec("MyPage", ctrl, didProcEnd);
        }

        /// <summary>
        /// バトルシーンへ移動.
        /// </summary>
        public void GoToBattle(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = false;
            var ctrl = ScreenControllerBase.Create<BattleSController>();
            ctrl.IsReversion = false;
            ScreenChanger.SharedInstance.Exec("Battle", ctrl, didProcEnd);
        }

        /// <summary>
        /// バトル復帰.
        /// </summary>
        public void GoToBattleReversion(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = false;
            var ctrl = ScreenControllerBase.Create<BattleSController>();
            ctrl.IsReversion = true;
            ScreenChanger.SharedInstance.Exec("Battle", ctrl, didProcEnd);
        }

        /// <summary>
        /// シナリオシーンに移動
        /// </summary>
        public void GoToScenario(bool bLatestQuestClear = false, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = false;
            var ctrl = ScreenControllerBase.Create<ScenarioSController>();
			ctrl.IsLatestQuestClear = bLatestQuestClear;
            ScreenChanger.SharedInstance.Exec("Scenario", ctrl, didProcEnd);
        }

        /// <summary>
        /// メインクエスト選択に移動
        /// </summary>
        public void GoToMainQuestSelect(MainQuestBootEnum boot = MainQuestBootEnum.Country, bool bBoot = true, bool bCheckNewRoot = false, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<MainQuestSController>();
            ctrl.BootMode = boot;
			ctrl.IsBoot = bBoot;
			ctrl.IsCheckNewRoot = bCheckNewRoot;
            ScreenChanger.SharedInstance.Exec("MainQuest", ctrl, didProcEnd);
        }

        /// <summary>
        /// フレンド選択画面に移動
        /// </summary>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToFriendSelect(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<FriendSelectSController>();
            ScreenChanger.SharedInstance.Exec("FriendSelect", ctrl, didProcEnd);
        }

        /// <summary>
        /// 課金テスト画面に移動.
        /// </summary>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToPurchaseTest(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PurchaseTestSController>();
            ScreenChanger.SharedInstance.Exec("PurchaseTest", ctrl, didProcEnd);
        }

        /// <summary>
        /// プレゼント一覧シーンに移動
        /// </summary>
        public void GoToPresent(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PresentBoxSController>();
            ScreenChanger.SharedInstance.Exec("PresentBox", ctrl, didProcEnd);
        }

        /// <summary>
        /// お知らせ一覧シーンに移動
        /// </summary>
        public void GoToNotice(Action didProcEnd = null)
        {
            PopupManager.OpenPopupOK("お知らせ画面は未実装です。");
        }

        /// <summary>
        /// ミッション一覧シーンに移動
        /// </summary>
        public void GoToMission(Action didProcEnd = null)
        {
			View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
			var ctrl = ScreenControllerBase.Create<MissionSController>();
            ScreenChanger.SharedInstance.Exec("Mission", ctrl, didProcEnd);
        }

        /// <summary>
        /// キャラクター変更シーンに移動
        /// </summary>
        public void GoToCharacterChange(Action didProcEnd = null)
        {
            PopupManager.OpenPopupOK("キャラクター変更画面は未実装です。");
        }

        /// <summary>
        /// チーム編成シーンに移動.
        /// </summary>
        public void GoToPartyEdit(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PartyEditSController>();
            ScreenChanger.SharedInstance.Exec("PartyEdit", ctrl, didProcEnd);
        }

        /// <summary>
        /// 出撃前編成シーンに移動.
        /// </summary>
        public void GoToQuestPreparation(SupporterCardData supportCard = null, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PartyEditSController>();         
            ctrl.IsBattleInit = true;
			ctrl.SupportCard = supportCard;
            ScreenChanger.SharedInstance.Exec("QuestPreparation", ctrl, didProcEnd);
        }

        /// <summary>
        /// チーム編成シーンに移動.
        /// </summary>
        public void GoToPVPPartyEdit(bool isBattleInit = false, bool isPvpOpponentSelect = false, string prevScene = null, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PartyEditSController>();
            ctrl.IsPvP = true;
            ctrl.IsPvPOpponentSelect = isPvpOpponentSelect;
            ctrl.PrevSceneName = prevScene;
            ctrl.IsBattleInit = isBattleInit;
            ScreenChanger.SharedInstance.Exec("PartyEdit", ctrl, didProcEnd);
        }

        /// <summary>
        /// 設定シーンに移動
        /// </summary>
        public void GoToOption(OptionBootMenu boot, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<OptionSController>();
            ctrl.BootMenu = boot;
            ScreenChanger.SharedInstance.Exec("Option", ctrl, didProcEnd);
        }

        /// <summary>
        /// 陣形編成シーンに移動.
        /// </summary>
        public void GoToFormationSelect(bool isBattleInit, bool isPvp, Action backScene, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = !isBattleInit;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<FormationSelectSController>();
            ctrl.IsPvp = isPvp;
            ctrl.BackScene = backScene;
            ScreenChanger.SharedInstance.Exec("FormationSelect", ctrl, didProcEnd);
        }

        /// <summary>
        /// ユニット詳細シーンに移動.
        /// </summary>
        public void GoToUnitDetails(CardData card, Action backSceneCallback, bool globalMenu = true, bool supportCard = false, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = globalMenu;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<UnitDetailsSController>();
            ctrl.DisplayCard = card;
            ctrl.DisplaySupporterCard = null;
            ctrl.IsSupportCard = supportCard;
            ctrl.BackSceneCallback = backSceneCallback;
            ctrl.DispGlobalMenu = globalMenu;
            ScreenChanger.SharedInstance.Exec("UnitDetails", ctrl, didProcEnd);
        }

        public void GoToUnitDetails(SupporterCardData card, Action backSceneCallback, bool globalMenu = true, bool supportCard = false, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = globalMenu;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<UnitDetailsSController>();
            ctrl.DisplayCard = null;
            ctrl.DisplaySupporterCard = card;
            ctrl.IsSupportCard = supportCard;
            ctrl.BackSceneCallback = backSceneCallback;
            ScreenChanger.SharedInstance.Exec("UnitDetails", ctrl, didProcEnd);
        }

        /// <summary>
        /// パーティー詳細シーンに移動.
        /// </summary>
        public void GoToPartyDetails(bool isBattleInit, bool isPvP, Action backScene, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = !isBattleInit;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PartyDetailsSController>();
            ctrl.BackScene = backScene;
            ctrl.IsBattleInit = isBattleInit;
            ctrl.IsPvP = isPvP;
            ScreenChanger.SharedInstance.Exec("PartyDetails", ctrl, didProcEnd);
        }

        /// <summary>
        /// パーティー変更シーンに移動.
        /// </summary>
        public void GoToPartyChange(bool isBattleInit, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = !isBattleInit;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PartyChangeSController>();
            ctrl.IsBattleInit = isBattleInit;
            ScreenChanger.SharedInstance.Exec("PartyChange", ctrl, didProcEnd);
        }

        /// <summary>
        /// ユニット選択シーンに移動.
        /// </summary>
        public void GoToSelectUnit(bool isBattleInit, bool isDispOrganizing, bool isPvP, bool isDispRemove, int removeTargetCardID, int focusTargetCardID, ElementEnum? dispOnlyElement, bool globalMenu, Action<CardData> DidSelect, Action DidBack, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = !isBattleInit && globalMenu;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<SelectUnitSController>();
            ctrl.IsBattleInit = isBattleInit;
            ctrl.IsGlobalMenu = globalMenu;
            ctrl.IsPvP = isPvP;
            ctrl.IsDispOrganizing = isDispOrganizing;
            ctrl.IsDispRemove = isDispRemove;
            ctrl.RemoveTargetCardID = removeTargetCardID;
            ctrl.DidSelect = DidSelect;
            ctrl.DidBack = DidBack;
            ctrl.FocusTargetCardID = focusTargetCardID;
            ctrl.DispOnlyElement = dispOnlyElement;
            ScreenChanger.SharedInstance.Exec("PartyEditUnit", ctrl, didProcEnd);
        }

        /// <summary>
        /// ガチャ画面に移動.
        /// </summary>
		public void GoToGacha(Action didProcEnd = null)
		{
			View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
			var ctrl = ScreenControllerBase.Create<GachaSController>();
			ScreenChanger.SharedInstance.Exec("Gacha", ctrl, didProcEnd);
		}

		/// <summary>
        /// 武器画面に移動.
        /// </summary>
        public void GoToWeapon(CardData equipedCard = null, bool dispGlobalMenu = true, Action didBack = null, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = dispGlobalMenu;
            View_PlayerMenu.IsVisible = true;
			var ctrl = ScreenControllerBase.Create<WeaponSContoller>();
			ctrl.BackSceneProc = didBack;
			ctrl.EquipedCard = equipedCard;
            ScreenChanger.SharedInstance.Exec("Weapon", ctrl, didProcEnd);
        }

        /// <summary>
        /// ユニット一覧に移動.
        /// </summary>
        /// <param name="forcus">Forcus.</param>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToUnitList(CardData forcus = null, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<UnitListSController>();
            ScreenChanger.SharedInstance.Exec("UnitList", ctrl, didProcEnd);
        }

        /// <summary>
        /// Gos to PVP.
        /// </summary>
        /// <param name="isPlayerSelect">If set to <c>true</c> is player select.</param>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToPVP(bool isPlayerSelect=false, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
            var ctrl = ScreenControllerBase.Create<PVPSController>();
            ctrl.IsPlayerSelect = isPlayerSelect;
            ScreenChanger.SharedInstance.Exec("PVP", ctrl, didProcEnd);
        }

        /// <summary>
        /// PVPバトルシーンへ移動.
        /// </summary>
        public void GoToPVPBattle(PvpBattleEntryData entry, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = false;
            View_PlayerMenu.IsVisible = false;
            var ctrl = ScreenControllerBase.Create<PVPBattleSController>();
            ctrl.enrtyData = entry;
            ScreenChanger.SharedInstance.Exec("Battle", ctrl, didProcEnd);
        }

		/// <summary>
        /// ショップ画面に移動.
        /// </summary>
        public void GoToShop(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
			var ctrl = ScreenControllerBase.Create<ShopSController>();
            ScreenChanger.SharedInstance.Exec("Shop", ctrl, didProcEnd);
        }

        /// <summary>
        /// キャラシナリオに移動.
        /// </summary>
		public void GoToUnitQuest(Action didProcEnd = null)
		{
			View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;
			var ctrl = ScreenControllerBase.Create<UnitQuestSController>();
            ScreenChanger.SharedInstance.Exec("UnitQuest", ctrl, didProcEnd);
		}

        /// <summary>
        /// サポート編成画面へ
        /// </summary>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToPlayerSupportEdit(bool globalMenu = false, string prevSceneName = null, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = globalMenu;
            View_PlayerMenu.IsVisible = true;

            var ctrl = ScreenControllerBase.Create<PlayerSupportEditSController>();

            ctrl.DispGlobalMenu = globalMenu;
            ctrl.PrevSceneName = prevSceneName;
            ScreenChanger.SharedInstance.Exec("PlayerSupportEdit", ctrl, didProcEnd);
        }

        /// <summary>
        /// イベントTOPへの遷移
        /// </summary>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToEvent(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;

            var ctrl = ScreenControllerBase.Create<EventSController>();
            ScreenChanger.SharedInstance.Exec("Event", ctrl, didProcEnd);
        }

        /// <summary>
        /// 曜日クエストへの遷移
        /// </summary>
        /// <param name="type">1: 強化　2: 進化</param>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToDailyQuest(int type, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;

            var ctrl = ScreenControllerBase.Create<DailyQuestSController>();
            ctrl.Type = type;
            ScreenChanger.SharedInstance.Exec("DailyQuest", ctrl, didProcEnd);
        }

        /// <summary>
        /// 編成TOPへの遷移
        /// </summary>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToPartyEditTop(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;

            var ctrl = ScreenControllerBase.Create<PartyEditTopSController>();
            ScreenChanger.SharedInstance.Exec("PartyEditTop", ctrl, didProcEnd);
        }

        /// <summary>
        /// Gos to event quest.
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToEventQuest(int eventId, EventQuestStageTypeEnum? stageType = null, Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;

            var ctrl = ScreenControllerBase.Create<EventQuestSController>();
            ctrl.EventId = eventId;
            ctrl.StageType = stageType;
            ScreenChanger.SharedInstance.Exec("EventQuest", ctrl, didProcEnd);
        }

        /// <summary>
        /// アイテム一覧画面へ
        /// </summary>
        /// <param name="didProcEnd">Did proc end.</param>
        public void GoToItemList(Action didProcEnd = null)
        {
            View_GlobalMenu.IsVisible = true;
            View_PlayerMenu.IsVisible = true;

            var ctrl = ScreenControllerBase.Create<ItemListSController> ();
            ScreenChanger.SharedInstance.Exec ("ItemList", ctrl, didProcEnd);
        }

        #endregion
    }
}