using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// View : キャラクエストのステージ選択.
/// </summary>
public class View_UnitQuest_StageSelect : ViewBase
{
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(UnitQuest quest, List<int> achiveIdList)
	{
		m_quest = quest;
		m_achiveIdList = achiveIdList;

		this.CreateList();
	}

    private void CreateList()
	{
		var root = this.GetScript<ScrollRect>("StageScrollView").content.gameObject;
        root.DestroyChildren();
		var list = MasterDataTable.quest_unit.DataList.FindAll(q => q.card_id == m_quest.card_id);
		foreach(var q in list){
			if (root.transform.childCount > 0) {
				var bReleaseQuest = m_achiveIdList.Contains(q.id);
                if (!bReleaseQuest) {
                    return;
                }
            }
			// TODO : 最新クエストも追加.
			var go = GameObjectEx.LoadAndCreateObject("MainQuest/ListItem_UnitQuest_Stage", root);
			var c = go.GetOrAddComponent<ListItem_UnitQuest_Stage>();
			//c.Init(q);
		}
	}

	private UnitQuest m_quest;
	private List<int> m_achiveIdList;
}
