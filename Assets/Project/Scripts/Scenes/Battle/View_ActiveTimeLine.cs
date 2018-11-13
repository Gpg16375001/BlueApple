using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_ActiveTimeLine : MonoBehaviour
{
    private const int MAX_ICON_COUNT = 6;
    private GridLayoutGroup m_timeLineGrid;
    private GameObject m_timeLineIconPrefab;
    private ListItem_ActiveTimeIcon m_tempIcon;
    private List<ListItem_ActiveTimeIcon> m_iconList = new List<ListItem_ActiveTimeIcon>();
    
    public void Init(ref ActionOrder order)
    {
		m_order = order;
		this.Init();
    }
	public void Init()
	{
		m_timeLineGrid = GetComponent<GridLayoutGroup>();
        m_timeLineIconPrefab = Resources.Load("Battle/ListItem_CountTimeLineIcon") as GameObject;
	}

    private ListItem_ActiveTimeIcon CreateIcon(IActionOrderItem item)
    {
        var go = GameObject.Instantiate(m_timeLineIconPrefab);
        m_timeLineGrid.gameObject.AddInChild (go);
        var c = go.GetOrAddComponent<ListItem_ActiveTimeIcon>();
        c.Init (item);
        return c;
    }

    private void SortSiblingIndex()
    {
        var actionOrder = m_order != null ? m_order : BattleProgressManager.Shared.OrderQueue;
        m_iconList.Sort ((x, y) => actionOrder.GetOrder (y.Item) - actionOrder.GetOrder (x.Item));
        for (int i = 0; i < m_iconList.Count; ++i) {
            m_iconList [i].transform.SetSiblingIndex (i);
        }
    }

    public void Reset()
    {
        m_timeLineGrid.gameObject.DestroyChildren();
        m_iconList.Clear ();
		var order = m_order != null ? m_order : BattleProgressManager.Shared.OrderQueue;
		var lastIndex = order.Count - 1;
        var loopCount = Mathf.Min (MAX_ICON_COUNT - 1, lastIndex);
        for(var i = loopCount; i >= 0; --i) {
			var item = order[i];
            m_iconList.Add(CreateIcon(item));
        }
        // 一番後ろが最初に行動するアイテム。
        m_iconList.Last().IsOrder = true;
    }

    public void ResetOrder ()
    {
        m_iconList.ForEach (x => x.IsOrder = false);
    }

    public void Insert(IActionOrderItem item, int index)
    {
        if (index >= MAX_ICON_COUNT) {
            return;
        }

        var lastIndex = m_iconList.Count - 1;
        if (index == 0) {
            // 一番前に追加の場合は一番前にいるアイコンのOrderを落とす。
            var topIcon = m_iconList.Last ();
            if (topIcon != null) {
                topIcon.IsOrder = false;
            }
        }
        if (m_iconList.Count >= MAX_ICON_COUNT) {
            // 一番行動の遅いユニットを削除
            var timeIcon = m_iconList.First ();
            if (timeIcon != null) {
                timeIcon.PlayOutAnimation (() => {
                    m_iconList.Remove (timeIcon);
                    timeIcon.Dispose ();

                    var newTimeIcon = CreateIcon (item);
                    newTimeIcon.transform.SetSiblingIndex (lastIndex - index);
                    newTimeIcon.PlayInsertAnimation ();
                    m_iconList.Add (newTimeIcon);

                    SortSiblingIndex ();
                });
            }
        } else {
            // 最大値に足りない場合は追加
            var newTimeIcon = CreateIcon (item);
            newTimeIcon.transform.SetSiblingIndex (lastIndex - index);
            newTimeIcon.PlayInsertAnimation ();
            m_iconList.Add (newTimeIcon);

            SortSiblingIndex ();
        }
    }

    public void Replace(IActionOrderItem item, int oldIndex, int newIndex, int outCount=0)
    {
        if (oldIndex >= MAX_ICON_COUNT && newIndex >= MAX_ICON_COUNT) {
            return;
        }
        if (oldIndex >= MAX_ICON_COUNT && newIndex < MAX_ICON_COUNT) {
            Insert (item, newIndex);
            return;
        }

        // オブジェクトを取得
        var itemIndex = m_iconList.FindIndex (x => x.ComareItem (item));

        // オブジェクトがなく新規で追加される場合はinsertする。
        if (itemIndex < 0) {
            if (newIndex < MAX_ICON_COUNT) {
                Insert (item, newIndex);
            }
            return;
        }

		var order = m_order != null ? m_order : BattleProgressManager.Shared.OrderQueue;
        var lastIndex = m_iconList.Count - 1;
        Debug.Log ("Replace item:" + item.ToString() + " old:" + oldIndex + " new:" + newIndex + " itemIndex:" + itemIndex);
        var timeIcon = m_iconList[itemIndex];
        timeIcon.PlayOutAnimation (() => {
            if(newIndex >= MAX_ICON_COUNT) {
                m_iconList.Remove(timeIcon);
                timeIcon.Dispose();
                var nextItem = order[MAX_ICON_COUNT - 1 - outCount];
                var nextTimeIcon = CreateIcon(nextItem);
                nextTimeIcon.transform.SetAsFirstSibling();
                m_iconList.Add(nextTimeIcon);
                nextTimeIcon.PlayInAnimation();
            } else {
                timeIcon.transform.SetSiblingIndex(lastIndex - newIndex);
                if (newIndex == m_iconList.Count - 1) {
                    timeIcon.PlayInAnimation ();
                } else {
                    timeIcon.PlayInsertAnimation ();
                }
            }
            SortSiblingIndex ();
        });
    }

    public void DidAttackPreUpdate()
    {
        // 削除対象がいた場合.
        if (m_iconList.Any (x => x.IsRemove)) {
            var removeItems = m_iconList.Where (x => x.IsRemove).ToArray ();
            foreach (var o in removeItems) {
                o.Dispose ();
                m_iconList.Remove (o);
            }
            // 追加できるなら追加.
			var order = m_order != null ? m_order : BattleProgressManager.Shared.OrderQueue;
			int maxIconCount = Mathf.Min (MAX_ICON_COUNT, order.Count);
            while (m_iconList.Count < maxIconCount) {
				var c = CreateIcon (order[m_iconList.Count]);
                c.transform.SetAsFirstSibling ();
                c.PlayInAnimation ();
                m_iconList.Add (c);
            }
        }
        SortSiblingIndex ();
        m_iconList.Last().IsOrder = true;
    }

    // スキル使用時のActiveTimeLine表示用関数
    public void DrawSkillDetail(BattleLogic.SkillParameter skillParam, bool bVisible)
    {
        if (!bVisible) {
            LineRendererHelper.DestroyLine();
            if(m_tempIcon != null){
                m_tempIcon.Dispose();
            }
            return;
        }
		var order = m_order != null ? m_order : BattleProgressManager.Shared.OrderQueue;
		var attacker = order.Peek() as ListItem_BattleUnit;
        Transform targetPosition = null;
		var PredictionIndex = order.Prediction (attacker,
            attacker.Parameter.CalcWeight(skillParam.Skill.weight));
        if(PredictionIndex < 6){
            m_tempIcon = CreateIcon (attacker);
            m_tempIcon.transform.SetSiblingIndex(m_iconList.Count - PredictionIndex - 1);
            m_tempIcon.IsOrder = true;
            targetPosition = m_tempIcon.LineTarget;
        }
        var start = m_iconList.Find(o => o.ComareItem(attacker));
        LineRendererHelper.DrawActiveTimeLineNext(start.LineTarget, targetPosition, this.GetComponentInParent<Canvas>());
    }

    public static bool IsTimeLineOut(ActionOrderSortInfo sortInfo)
    {
        return sortInfo.newIndex >= MAX_ICON_COUNT && sortInfo.oldIndex < MAX_ICON_COUNT;
    }
    public static int GetOutCount(ActionOrderSortInfo[] sortInfo)
    {
        return sortInfo.Count (x => IsTimeLineOut(x));
    }

	private ActionOrder m_order;
}
