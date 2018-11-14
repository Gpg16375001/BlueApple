using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.Net.API;
using SmileLab.UI;


public class BattleDropItemEqualityComparer : IEqualityComparer<BattleDropItem>
{
    //
    // Methods
    //
    public bool Equals (BattleDropItem x, BattleDropItem y)
    {
        if (x == null && y == null) {
            return true;
        }

        if (x == null || y == null) {
            return false;
        }

        return x.reward_type == y.reward_type && x.reward_id == y.reward_id;
    }

    public int GetHashCode (BattleDropItem obj)
    {
        return obj.reward_type.GetHashCode() ^ (int)obj.reward_id;
    }
}
/// <summary>
/// Screen : バトル前フレンド選択画面.
/// </summary>
public class Screen_FriendSelect : ViewBase
{
    public void Init()
    {
        m_friendScrollView = GetScript<ScrollRect>("ScrollAreaPlayerView");

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        // ラベル類
        var questId = AwsModule.ProgressData.CurrentQuest.ID;
        var questType = AwsModule.ProgressData.CurrentQuest.QuestType;
        var stageId = AwsModule.ProgressData.CurrentQuest.BattleStageID;
        var mission = AwsModule.BattleData.MissionProgress;
		this.GetScript<TextMeshProUGUI>("txtp_SceneTitle").text = AwsModule.ProgressData.CurrentQuest.QuestName;
        if (AwsModule.ProgressData.CurrentQuest.QuestType == 1) {
            this.GetScript<TextMeshProUGUI> ("txtp_QuestTitle").SetTextFormat ("クエスト{0}", AwsModule.ProgressData.CurrentQuest.QuestNum);
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_QuestTitle").gameObject.SetActive (false);
        }
        if (mission != null) {
            for (var i = 1; i <= 3; ++i) {
                var label = this.GetScript<TextMeshProUGUI> ("txtp_Mission_" + i.ToString ("d2"));
                var index = i - 1;
                var text = mission.GetMissionTextList ().Count > index ? mission.GetMissionTextList () [index] : "";
                label.text = text;

                // 達成済みミッション設定.
                if (AwsModule.ProgressData.CurrentQuestAchievedMissionIdList != null && AwsModule.ProgressData.CurrentQuestAchievedMissionIdList.Contains(i)) {
                    var bAchived = true;
                    label.color = bAchived ? Color.gray : Color.white;
                    this.GetScript<Image> ("Mission_" + i.ToString ("d2") + "/img_MissionCheckboxOn").gameObject.SetActive (bAchived);
                    this.GetScript<Image> ("Mission_" + i.ToString ("d2") + "/img_MissionCheckboxOff").gameObject.SetActive (mission.MissionList.Count > index && !bAchived);
                } else {
                    label.color = Color.white;
                    this.GetScript<Image> ("Mission_" + i.ToString ("d2") + "/img_MissionCheckboxOn").gameObject.SetActive (false);
                    this.GetScript<Image> ("Mission_" + i.ToString ("d2") + "/img_MissionCheckboxOff").gameObject.SetActive (mission.MissionList.Count > index);
                }
            }
        }

		// タブ作成.
        var categories = MasterDataTable.element.GetCategoryNameList();
        var rootObj = this.GetScript<GridLayoutGroup>("TabMenu").gameObject;
        rootObj.DestroyChildren();
		foreach (var category in categories) {
			if(category == "ALL"){
				continue;   // フレンド選択画面ではALLタブは使用しない.
			}
            var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_HorizontalTextTab", rootObj);
            var c = go.GetOrAddComponent<ListItem_HorizontalElementTab>();
            c.Init(category, CallbackSelectedElement);
        }
        
		// フレンドリストを作成.
		this.RequestUserList(() => SelectElementCategory());

		// 出現敵属性情報
		var grid = this.GetScript<GridLayoutGroup>("StageInfoAttribute/ElementIconGrid");
		if(AwsModule.BattleData != null && AwsModule.BattleData.StageEnemy != null){
            var elements = AwsModule.BattleData.StageEnemy.Select(s => {
                if(s.card != null) {
                    // カードをベースに出す場合の対応
                    return s.card.element.Enum;
                }
                return s.monster.element.Enum;
            }).Distinct().ToList();
            foreach (var sprite in grid.GetComponentsInChildren<uGUISprite>(true)) {
                if (elements.Count <= 0) {
                    sprite.gameObject.SetActive(false);
                    continue;
                }
                var e = elements.FirstOrDefault();
                sprite.gameObject.SetActive(true);
                sprite.ChangeSprite(((int)e).ToString());
                elements.RemoveAt(0);
            }
		}else{
			foreach (var sprite in grid.GetComponentsInChildren<uGUISprite>()) {
                sprite.gameObject.SetActive(false);
            }
		}   

        // ドロップ情報.
		grid = this.GetScript<GridLayoutGroup>("StageInfoDropItem/ItemIconGrid");
		if (AwsModule.BattleData != null && AwsModule.BattleData.StageEnemy != null) {
			var dropItemList = AwsModule.BattleData.StageEnemy
			                            .Where(e => e.drop_table_id != null)
			                            .SelectMany(e => MasterDataTable.battle_drop_item.DataList.FindAll(d => d.drop_table_id == e.drop_table_id.Value))
                                        .Distinct(new BattleDropItemEqualityComparer())
			                            .ToList();
			foreach (var iconRoot in grid.GetComponentsInChildren<Transform>()) {
				if(!iconRoot.name.Contains("ItemRoot")){
					continue;
				}            
				if (dropItemList.Count <= 0) {
                    iconRoot.gameObject.SetActive(false);
                    continue;
                }
				Debug.Log(iconRoot.name);
				iconRoot.gameObject.SetActive(true);
				var v = iconRoot.gameObject.GetOrAddComponent<ViewBase>();            
				var item = dropItemList.FirstOrDefault();
				Debug.Log(item != null ? item.GetName(): "null");
				var iconInfo = ((ItemTypeEnum)item.reward_type).GetIconInfo(item.reward_id, true);
				iconRoot.GetComponentsInChildren<uGUISprite>(true)[0].gameObject.SetActive(iconInfo.IsEnableSprite);    // sprite設定ルート
                if (iconInfo.IsEnableSprite) {
					iconRoot.GetComponentsInChildren<uGUISprite>(true)[0].LoadAtlasFromResources(iconInfo.AtlasName, iconInfo.SpriteName);
                } else if (iconInfo.IconObject != null) {
                    var bUnitOrCard = (ItemTypeEnum)item.reward_type == ItemTypeEnum.weapon || (ItemTypeEnum)item.reward_type == ItemTypeEnum.card;
					v.GetScript<Transform>("UnitWeaponRoot").gameObject.SetActive(bUnitOrCard);
					v.GetScript<Transform>("ItemIcon1").gameObject.SetActive(!bUnitOrCard);
					var parent = v.GetScript<Transform>("UnitWeaponRoot");
					if(!bUnitOrCard){
						parent = v.GetScript<Transform>("ItemIcon1");
					}
					iconInfo.IconObject.transform.SetParent(parent, false);               
                }
				dropItemList.RemoveAt(0);
			}         
		}else{
			foreach (var sprite in grid.GetComponentsInChildren<uGUISprite>()) {
                sprite.gameObject.SetActive(false);
            }
		}

        // ボタン、ドロップダウン.
		var dropdown = GetScript<TMP_Dropdown>("Sort/bt_PullDown");
		dropdown.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(DidTapSort));
		this.SetCanvasCustomButtonMsg("bt_Ascentd", DidTapOrder);
		this.SetCanvasCustomButtonMsg("bt_Descend", DidTapOrder);

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

	// カテゴリータブとして属性選択を行う.
	private void SelectElementCategory(string categoryName = "火")
	{
		m_currentCategoryName = categoryName;

		var info = MasterDataTable.element.GetInfoFromCategoryName(categoryName);
        m_filterList = m_userCacheList.Where(u => u.SupporterCardList.Any(c => c.Card != null && c.Card.element.Enum == info.Enum)).ToList();
		
		// リスト更新.
		this.SortOrder(m_currentSort, m_bOrderDescend);
		// タブ選択状態.
		var rootObj = this.GetScript<GridLayoutGroup>("TabMenu").gameObject;
		foreach (var tab in rootObj.GetComponentsInChildren<ListItem_HorizontalElementTab>()) {
			tab.IsSelected = tab.CategoryName == categoryName;
		}      
	}

    // ソートと並び替え処理.
	private void SortOrder(SortMode sort, bool bDescend)
	{
		switch(sort){
			case SortMode.Login:
				m_filterList.Sort((x, y) => {
					var xLogin = DateTime.Parse(x.LastLoginDate, null, DateTimeStyles.RoundtripKind);      
					var yLogin = DateTime.Parse(y.LastLoginDate, null, DateTimeStyles.RoundtripKind);
					return bDescend ? Mathf.FloorToInt((float)(xLogin - yLogin).TotalSeconds) : Mathf.FloorToInt((float)(yLogin - xLogin).TotalSeconds);
				});
				break;
			case SortMode.Level:
				m_filterList.Sort((x, y) => {
                var xCard = x.SupporterCardList.FirstOrDefault(c => c.Card != null && c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
                var yCard = y.SupporterCardList.FirstOrDefault(c => c.Card != null && c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
					return bDescend ? yCard.Level - xCard.Level : xCard.Level - yCard.Level;
				});
				break;
			case SortMode.HP:
                m_filterList.Sort((x, y) => {
                var xCard = x.SupporterCardList.FirstOrDefault(c => c.Card != null && c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
                var yCard = y.SupporterCardList.FirstOrDefault(c => c.Card != null && c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
					return bDescend ? yCard.Parameter.Hp - xCard.Parameter.Hp : xCard.Parameter.Hp - yCard.Parameter.Hp;
                });
                break;
			case SortMode.ATK:
				m_filterList.Sort((x, y) => {
                var xCard = x.SupporterCardList.FirstOrDefault(c => c.Card != null &&  c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
                var yCard = y.SupporterCardList.FirstOrDefault(c => c.Card != null &&  c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
					return bDescend ? yCard.Parameter.Attack - xCard.Parameter.Attack : xCard.Parameter.Attack - yCard.Parameter.Attack;
                });
                break;
			case SortMode.DEF:
				m_filterList.Sort((x, y) => {
                var xCard = x.SupporterCardList.FirstOrDefault(c => c.Card != null &&  c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
                var yCard = y.SupporterCardList.FirstOrDefault(c => c.Card != null &&  c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
					return bDescend ? yCard.Parameter.Defense - xCard.Parameter.Defense : xCard.Parameter.Defense - yCard.Parameter.Defense;
                });
				break;
			case SortMode.SPD:
				m_filterList.Sort((x, y) => {
                var xCard = x.SupporterCardList.FirstOrDefault(c => c.Card != null &&  c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
                var yCard = y.SupporterCardList.FirstOrDefault(c => c.Card != null &&  c.Card.element.Enum == MasterDataTable.element.GetInfoFromCategoryName(m_currentCategoryName).Enum);
					return bDescend ? yCard.Parameter.Agility - xCard.Parameter.Agility : xCard.Parameter.Agility - yCard.Parameter.Agility;
                });
                break;
		}
		this.UpdateSupportList(m_currentCategoryName);
	}
    // ソートのみ
	private void Sort(SortMode sort)
	{
		this.SortOrder(sort, m_bOrderDescend);
		m_currentSort = sort;
	}
    // 並び替えのみ
	private void Order(bool bDescend)
	{
		this.SortOrder(m_currentSort, bDescend);
		m_bOrderDescend = bDescend;
	}   

	// サポートリスト更新作成.
    void UpdateSupportList(string categoryName = "火")
    {
		// リスト作成.
        m_friendScrollView.content.gameObject.DestroyChildren();
        var info = MasterDataTable.element.GetInfoFromCategoryName(categoryName);
		foreach (var user in m_filterList) {
            var card = user.SupporterCardList.FirstOrDefault(c => c.Card != null && c.Card.element.Enum == info.Enum);
            if (card != null) {
                var item = GameObjectEx.LoadAndCreateObject ("FriendSelect/ListItem_Friend");
                item.transform.SetParent (m_friendScrollView.content, false);
                item.GetOrAddComponent<ListItem_Friend> ().Init (user, card);
            }
        }

        // 初回起動
        if (AwsModule.ProgressData.IsFirstBoot) {
            var module = TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.Blank, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
            module.CreateFriendSelect(m_friendScrollView.content.GetComponentsInChildren<ListItem_Friend>(true).FirstOrDefault(i => i.transform.GetSiblingIndex() <= 0));
        }
    }

    // 通信リクエスト : ユーザーリスト.一回だけ行なってあとはキャッシュしたやつを使い回す.
    private void RequestUserList(Action didLoad)
    {
		View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.UsersGetBattleSupporterList((bSuccess, res) => {
            if (!bSuccess || res == null) {
                Debug.LogError("[Screen_FriendSelect] CreateSupportList Error!! : res=" + res);
                return;
            }
            m_userCacheList = new List<UserData>(res.UserDataList);
			m_filterList = new List<UserData>(res.UserDataList);
            if (didLoad != null) {
                didLoad();
            }
			View_FadePanel.SharedInstance.IsLightLoading = false;
        });
    }

    void OnDestroy()
    {
        View_PlayerMenu.DidTapBackButton -= DidTapBack;
    }

    #region ButtonDelegate.

	// ボタン : 戻る
	void DidTapBack()
    {
        if (!gameObject.activeSelf) {
            return;
        }
        var questType = AwsModule.ProgressData.CurrentQuest.QuestType;

        if (questType == 4) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToDailyQuest (1);
            });
        } else if (questType == 5) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToDailyQuest (2);
            });
        } else {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToMainQuestSelect (MainQuestBootEnum.Stage);
            });
        }
    }

	// ボタン : 並び替え.
	void DidTapOrder()
	{
		this.Order(!m_bOrderDescend);
		this.GetScript<CustomButton>("bt_Ascentd").gameObject.SetActive(!m_bOrderDescend);
		this.GetScript<CustomButton>("bt_Descend").gameObject.SetActive(m_bOrderDescend);      
	}
    
	// ボタン : ソート
    void DidTapSort(int sortVal)
	{
		this.Sort((SortMode)sortVal);
	}

	// コールバック : カテゴリータブとして属性選択を行う.
    void CallbackSelectedElement(ListItem_HorizontalElementTab selected)
    {
        this.SelectElementCategory(selected.CategoryName);
    }
    #endregion

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private SortMode m_currentSort = SortMode.Login;
    private ScrollRect m_friendScrollView;
	private List<UserData> m_userCacheList = new List<UserData>();
	private List<UserData> m_filterList = new List<UserData>();
	private string m_currentCategoryName = "火";
	private bool m_bOrderDescend = false;


    /// フィルター状態.
    private enum SortMode
	{
		Login,
        Level,
        HP,
        ATK,
        DEF,
        SPD,
	}
}
