using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// メインクエストで使用する用のドロップダウン.
/// </summary>
public class MainQuestListInDropDown : TMP_Dropdown
{   
	/// <summary>ルートになるボタンの名前.ルートが押されたのか入れ子のリストアイテムが押されたか検知するのに必要.</summary>
    public string RootButtonName;
    /// <summary>ルートをタップした際のイベント.</summary>
	public Action onClickRoot;   


    /// <summary>クエストリスト追加.</summary>
	public void AddOptions(IEnumerable<IQuestData> collection, Action didCreateAndOpenList = null)
	{
		m_didCreateAndOpenList = didCreateAndOpenList;
		m_collection = new List<IQuestData>(collection);
		AddOptions(collection.Select(q => CreateDropDownItem(q)).ToList());      
	}
	OptionData CreateDropDownItem(IQuestData data)
    {
        var rtn = new OptionData();
        rtn.text = "クエスト" + data.QuestNum;
        return rtn;
    }
 
    /// <summary>
    /// ルートをタッチした形式としてリストを閉じる.
    /// </summary>
	public void HideTouchRoot()
	{
		isTouchRoot = true;
		prevVal = -1;
		isShow = false;
		Hide();
	}

	/// ドロップダウンというより入れ子のリストアイテムとして使用するのでToggleの基本機能は無視してonValueChangedを呼ぶ.
	public override void Select()
    {
		base.Select();
  
        // 必ずルートタップを確認してから.
		if(isTouchRoot){
			return;
		}
		// Hide時にToggle.isOnのアイテムはもう一度Selectが呼ばれるのでこうする.
		if (value == prevVal) {
			return;
		}
		// 初期状態でindex=0のアイテムがisOnになっているはずなのでそこに絞って強制的にコールバックを呼ぶ.
		if (value == 0) {
			SoundManager.SharedInstance.PlaySE(SoundClipName.se002);
            onValueChanged.Invoke(0);
        }
  
        prevVal = value;
    }
	// MEMO : Toggle.isOn=trueのアイテムをタップしてもOnPointer***は呼ばれない.
	public override void OnPointerClick(PointerEventData eventData)
	{
		var bTapRoot = !string.IsNullOrEmpty(RootButtonName) && eventData.pointerEnter.gameObject.name.Contains(RootButtonName);
		if (!bTapRoot) {
			isShow = false;
			base.OnPointerClick(eventData);
			return;
		}
		isTouchRoot = true;
        
		isShow = !isShow;
        base.OnPointerClick(eventData);

		if(isShow){
			SoundManager.SharedInstance.PlaySE(SoundClipName.se004);
            StartCoroutine(WaitCreatedDropDownList());
        }else{
			SoundManager.SharedInstance.PlaySE(SoundClipName.se005);
			Hide(); // brockerは使わず手動で閉じる.
        }
		if(onClickRoot != null){
            onClickRoot();
        }      

		prevVal = -1;      
	}

	// MEMO : base.OnPointerClickからここにくる.リスト生成の開始.
	protected override GameObject CreateDropdownList(GameObject template)
    {     
		if(!isTouchRoot && !isShow){
			isShow = true;  // Show()などで強制的に初回から展開されている際はこちらで帳尻を合わせる.
			StartCoroutine(WaitCreatedDropDownList());
		}
        return base.CreateDropdownList(template);
    }   
	protected override void DestroyDropdownList(GameObject dropdownList)
	{
		base.DestroyDropdownList(dropdownList);
		if(coObservationState != null){
			this.StopCoroutine(coObservationState);
		}
	}

	protected override DropdownItem CreateItem(DropdownItem itemTemplate)
	{
		var item = base.CreateItem(itemTemplate);
		this.StartCoroutine(this.WaitCreateItem(item));      
		return item;
	}
	IEnumerator WaitCreateItem(DropdownItem item)
	{
		var viewBase = item.gameObject.GetOrAddComponent<ViewBase>();
		yield return null;  // CreateItemから1フレ待たないとデータが生成されない.

        // リストアイテム名以外の初期化.
		var info = m_collection.Find(c => c.QuestNum.ToString() == viewBase.GetScript<TextMeshProUGUI>("txtp_Quest").text.Replace("クエスト",""));
		viewBase.GetScript<TextMeshProUGUI>("txtp_APNum").text = info.NeedAP.ToString();
		viewBase.GetScript<TextMeshProUGUI>("txtp_OpenStageNum").text = info.StageNum.ToString();
		viewBase.GetScript<TextMeshProUGUI>("txtp_QuestNum").text = info.QuestNum.ToString();
		// ミッション.
		var bCompleteMission = true;
		var mission = MasterDataTable.battle_mission_setting.DataList.Find(s => s.stage_id == info.BattleStageID);
		viewBase.GetScript<Transform>("MissionGrid").gameObject.SetActive(mission != null);
		if(mission != null){
			var releaseMissions = QuestAchievement.CacheGet(info.ID).AchievedMissionIdList;
            var missionList = new List<int>();
            missionList.Add(mission.condition_1 ?? -1);
            missionList.Add(mission.condition_2 ?? -1);
            missionList.Add(mission.condition_3 ?? -1);
			if (releaseMissions == null || releaseMissions.Length <= 0) {
                bCompleteMission = false;      // 一つも設定されていない場合はコンプリートにはならないとのこと.
            }
			for (var i = 1; i <= missionList.Count; ++i) {
				// 設定無し.
				if(missionList[i-1] < 0){
					viewBase.GetScript<uGUISprite>("img_MissionClearIcon_" + i).gameObject.SetActive(false);
					continue;
				}
                // クリアしてないので空表示.
				if (releaseMissions == null || releaseMissions.Length <= 0) {
					viewBase.GetScript<uGUISprite>("img_MissionClearIcon_" + i).ChangeSprite("img_MissionClearIconOff");
                    continue;
                }
                // クリア済みでも達成してなければ空表示.
				var sptName = releaseMissions.Contains(i) ? "img_MissionClearIconOn" : "img_MissionClearIconOff";            
                viewBase.GetScript<uGUISprite>("img_MissionClearIcon_" + i).ChangeSprite(sptName);

				if (bCompleteMission && (missionList[i - 1] <= 0 || !releaseMissions.Contains(i))) {
                    bCompleteMission = false;
                }
            }
		}else{
			bCompleteMission = false;
		}
        // 初回報酬.
		var mainQuest = MasterDataTable.quest_main[info.ID];
		var bAchived = QuestAchievement.CacheGet(info.ID) != null && QuestAchievement.CacheGet(info.ID).IsAchieved;
		var iconInfo = mainQuest.reward_item_type.Enum.GetIconInfo(mainQuest.reward_item_id);
		var sprite = viewBase.GetScript<uGUISprite>("ItemIcon");
		viewBase.GetScript<Image>("img_RewardGet").gameObject.SetActive(bAchived);
		viewBase.GetScript<Transform>("Item").gameObject.SetActive(iconInfo.IsEnableSprite);    // sprite設定ルート
        if (iconInfo.IsEnableSprite) {
            sprite.LoadAtlasFromResources(iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (iconInfo.IconObject != null) {
			iconInfo.IconObject.transform.SetParent(viewBase.GetScript<Transform>("UnitWeaponRoot"));
        }      
        // 強制ロックかどうか.
		viewBase.GetComponentsInChildren<Toggle>(true)[0].interactable = !mainQuest.is_force_lock;
		viewBase.GetScript<TextMeshProUGUI>("txtp_Quest").gameObject.SetActive(!mainQuest.is_force_lock);
		viewBase.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockQuestNote").gameObject.SetActive(mainQuest.is_force_lock);
		viewBase.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockQuestNote").enabled = viewBase.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockQuestNote").interactable = mainQuest.is_force_lock;
		viewBase.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockQuestNote").onClick.RemoveAllListeners();
		if(mainQuest.is_force_lock){
			viewBase.SetCanvasCustomButtonMsg("bt_ForceLockQuestNote", () => PopupManager.OpenPopupOK("このクエストはまだ解放されておりません。\nアップデートされるまで\nお待ちください。"));
		}
        // バッジ.
        var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(info.Country);

		if(achiveQuests.Exists(a => a.QuestId == info.ID)){
			if (bCompleteMission) {
				viewBase.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(false);
                viewBase.GetScript<Image>("img_StoryIconComplete").gameObject.SetActive(true);
				viewBase.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(false);
				viewBase.GetScript<uGUISprite>("bt_Quest").ChangeSprite("bt_QuestComplete");
            } else {
				viewBase.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(true);
				viewBase.GetScript<Image>("img_StoryIconComplete").gameObject.SetActive(false);
				viewBase.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(false);
				viewBase.GetScript<uGUISprite>("bt_Quest").ChangeSprite("bt_Quest");
            }
		}else{
			viewBase.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(false);
            viewBase.GetScript<Image>("img_StoryIconComplete").gameObject.SetActive(false);
			viewBase.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(!mainQuest.is_force_lock && latest.id == info.ID);
			if(!mainQuest.is_force_lock){
				viewBase.GetScript<uGUISprite>("bt_Quest").ChangeSprite("bt_Quest");
			}         
		}  
	}

	// CreateDropDownListでは正確にDropDownListの生成を検知できないのでこうする.
	IEnumerator WaitCreatedDropDownList()
    {
        var canvas = GetComponentInChildren<Canvas>();
        while (canvas == null) {
            yield return null;
            canvas = GetComponentInChildren<Canvas>();
        }
        // スクロールエリアを遵守.
        canvas.overrideSorting = false;
		coObservationState = this.StartCoroutine(this.CoObservationState());

		if (m_didCreateAndOpenList != null) {
            m_didCreateAndOpenList();
        }
    }
    // リスト展開中のステート監視.
	IEnumerator CoObservationState()
	{
		while(true){
			yield return new WaitForEndOfFrame();
			isTouchRoot = false;
		}
	}

	// スクロールが効かなくなるのでblockerは生成しない.
	protected override GameObject CreateBlocker(Canvas rootCanvas)
	{
		return null;
	}

	private List<IQuestData> m_collection;
	private Action m_didCreateAndOpenList;
	private Coroutine coObservationState;
	private bool isShow = false;
	private bool isTouchRoot = false;
	private int prevVal = -1;   // デフォルトだたとルートと先頭のアイテムは両方ともvalue=0で判定しているのでルートを-1と定義する.
}
