using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

using BattleLogic;

public class ActionOrderSortInfo
{
    public IActionOrderItem item;
    public int oldIndex;
    public int newIndex;
}

[Serializable]
public class ActionOrder : IEnumerable<IActionOrderItem>, ICollection, IEnumerable
{
    List<IActionOrderItem> order;

    public IActionOrderItem this[int key] {
        get {
            return order[key];
        }
    }

    public int Count {
        get {
            return order.Count;
        }
    }

    public ActionOrder(List<IActionOrderItem> list)
    {
        order = new List<IActionOrderItem> ();
        order.AddRange (list);

        order.Sort (Comparison);
    }

    /// <summary>
    /// リストの登録し直しとWeightのReset
    /// </summary>
    /// <param name="list">ユニットのリスト</param>
    public void Reset(List<IActionOrderItem> list)
    {
        order.Clear ();
        order.AddRange (list);

        // Weightの初期化をする
        order.ForEach (x => x.ResetWeight ());
        order.Sort (Comparison);
    }

    /// <summary>
    /// 指定ユニットをリストに追加し並べ直しを行う。
    /// 追加したユニットが何番目に追加されたかを返す
    /// </summary>
    /// <param name="unit">追加するユニット情報</param>
    public int Enqueue(IActionOrderItem element)
    {
        order.Add (element);
        order.Sort (Comparison);
        return order.FindIndex (x => element.Equals(x));
    }

    /// <summary>
    /// 死亡ユニットの削除
    /// </summary>
    public void RemoveDisableItems()
    {
        // removeの条件を満たした場合にremoveする。
        order.RemoveAll(x => x.IsRemove);
    }

    public void RemoveItem(IActionOrderItem item)
    {
        order.Remove (item);
    }

    /// <summary>
    /// 先頭のユニットを取得してリストから削除
    /// </summary>
    public IActionOrderItem Dequeue()
    {
        IActionOrderItem ret = order [0];
        order.RemoveAt (0);
        return ret;
    }

    /// <summary>
    /// 先頭のユニットを取得
    /// </summary>
    public IActionOrderItem Peek()
    {
        return order [0];
    }

    /// <summary>
    /// リストをクリア
    /// </summary>
    public void Clear()
    {
        order.Clear();
    }

    /// <summary>
    /// リストに含まれるかの判定
    /// </summary>
    /// <param name="element">要素</param>
    public bool Contains(IActionOrderItem element)
    {
        return order.Contains (element);
    }

    /// <summary>
    /// リスト内全体に処理をするとき
    /// </summary>
    /// <param name="action">Action.</param>
    public void ForEach(Action<IActionOrderItem> action)
    {
        order.ForEach(x => action (x));
    }

    /// <summary>
    /// 行動後オーダーの位置予測
    /// </summary>
    /// <param name="action">行動</param>
    public int Prediction(IActionOrderItem invoker, int actionWeight)
    {
        int nextWeight = invoker.Weight + actionWeight;
        for (int i = 0; i < order.Count; ++i) {
            if (order [i].Weight > nextWeight) {
                return i - 1;
            }
        }
        return order.Count - 1;
    }

    /// <summary>
    /// 行動順番を取得
    /// </summary>
    /// <returns>The order.</returns>
    /// <param name="item">Item.</param>
    public int GetOrder(IActionOrderItem item)
    {
        return order.FindIndex (x => x == item);
    }

    /// <summary>
    /// 行動順をソートし直す
    /// </summary>
    public ActionOrderSortInfo[] Sort()
    {
        int count = order.Count;
        ActionOrderSortInfo[] ret = new ActionOrderSortInfo[count];

        for(int i = 0; i < count; ++i) {
            ret [i] = new ActionOrderSortInfo ();
            ret [i].item = order [i];
            ret [i].oldIndex = i;
        }
        order.Sort (Comparison);

        for(int i = 0; i < count; ++i) {
            ret [i].newIndex = GetOrder(ret [i].item);
        }
        return ret;
    }

    bool ICollection.IsSynchronized {
        get {
            return false;
        }
    }

    object ICollection.SyncRoot {
        get {
            return this;
        }
    }

    void ICollection.CopyTo (Array array, int idx)
    {
        for(int i = 0; i < order.Count; ++i, ++idx) {
            array.SetValue (order [i], idx);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return order.GetEnumerator ();
    }

    IEnumerator<IActionOrderItem> IEnumerable<IActionOrderItem>.GetEnumerator()
    {
        return order.GetEnumerator ();
    }

    // 並び替え判定
    private int Comparison(IActionOrderItem a, IActionOrderItem b)
    {
        if (a.Weight == b.Weight) {
            if (a.ItemType == b.ItemType) {
                if (a.ItemType == ActionOrderItemType.Unit) {
                    if (a.IsPlayer == b.IsPlayer) {
                        return a.Index - b.Index;
                    } else {
                        if (a.IsPlayer) {
                            return -1;
                        } else {
                            return -1;
                        }
                    }
                } else if(a.ItemType == ActionOrderItemType.Condition) {
                    if (a.IsPlayer == b.IsPlayer) {
                        return a.Index - b.Index;
                    } else {
                        if (a.IsPlayer) {
                            return -1;
                        } else {
                            return -1;
                        }
                    }
                }
            } else {
                return (int)a.ItemType - (int)b.ItemType;
            }
        }
        return a.Weight - b.Weight;
    }
}
