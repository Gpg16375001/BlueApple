using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// View : クエストクリア時のアイテム報酬ポップ.
/// </summary>
public class View_QuestRewardItemPop : PopupViewBase
{
    
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_QuestRewardItemPop Create(QuestRewardType rewardType, MainQuest quest, ItemData item, Action didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_QuestRewardItemPop");
        var c = go.GetOrAddComponent<View_QuestRewardItemPop>();
		c.InitInternal(rewardType, quest, item, didClose);
		return c;
	}
	private void InitInternal(QuestRewardType rewardType, MainQuest quest, ItemData item, Action didClose)
	{
        PopOpenAnimeName = "QuestRewardPopOpen";
        PopCloseAnimeName = "QuestRewardPopClose";

		m_didClose = didClose;

        // 報酬アイコン.
		var sprite = this.GetScript<uGUISprite>("RewardItemIcon");
		var iconInfo = ((ItemTypeEnum)item.ItemType).GetIconInfo(item.ItemId);
		this.GetScript<Transform>("RewardItem").gameObject.SetActive(iconInfo.IsEnableSprite);    // sprite設定ルート
		if (iconInfo.IsEnableSprite) {
            sprite.LoadAtlasFromResources(iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (iconInfo.IconObject != null) {
            iconInfo.IconObject.transform.SetParent(this.GetScript<Transform>("UnitWeaponRoot"));
        }
        
		// ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_RewardItemName").text = ((ItemTypeEnum)item.ItemType).GetName(item.ItemId);
		this.GetScript<TextMeshProUGUI>("txtp_RewardNum").text = item.Quantity.ToString();
		this.GetScript<Transform>("Clear").gameObject.SetActive(rewardType == QuestRewardType.Chapter);
		if(rewardType == QuestRewardType.Chapter){
			this.GetScript<TextMeshProUGUI>("txtp_Country").text = quest.Country.name;
			this.GetScript<TextMeshProUGUI>("txtp_ChapterNum").text = quest.ChapterNum.ToString();
			this.GetScript<TextMeshProUGUI>("txtp_ClearTitle").text = quest.stage_info.chapter_info.chapter_name;
			this.GetScript<uGUISprite>("EmblemIcon").ChangeSprite(((int)quest.Country.Enum).ToString());
		}

		// 再生アニメーション選択
		this.GetScript<Transform>("EffQuestClear").gameObject.SetActive(rewardType == QuestRewardType.Quest);
		this.GetScript<Transform>("EffStageClear").gameObject.SetActive(rewardType == QuestRewardType.Stage);
		this.GetScript<Transform>("EffChapterClear").gameObject.SetActive(rewardType == QuestRewardType.Chapter);

		// ボタン.
		this.SetCanvasCustomButtonMsg("BlackCurtain", DidTapClose);   
        SetBackButton ();

        PlayOpenCloseAnimation (true);
	}

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }
	// ボタン : 閉じる.
    void DidTapClose()
	{
        if (IsClosed) {
            return;
        }

		LockInputManager.SharedInstance.IsLock = true;
        PlayOpenCloseAnimation (false, () => {
            if(m_didClose != null){
                m_didClose();
            }
            this.Dispose();         
            LockInputManager.SharedInstance.IsLock = false;
        });
	}

	private Action m_didClose;
}

/// <summary>
/// クエストの報酬タイプ.
/// </summary>
public enum QuestRewardType
{
	Quest,
    Stage,
    Chapter,
}