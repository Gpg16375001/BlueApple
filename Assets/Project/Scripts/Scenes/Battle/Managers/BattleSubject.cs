using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

/// <summary>
/// バトル画面のメディエーター.
/// </summary>
public class BattleSubject
{
    /// <summary>
    /// 末尾に登録.
    /// </summary>
    public void RegistObserverAsLast(IBattleObserver collaegue)
    {
        observerList.Add(collaegue);
    }
    /// <summary>
    /// 最優先登録.
    /// </summary>
    public void RegistObserverAsFirst(IBattleObserver collaegue)
    {
        observerList.Insert(0, collaegue);
    }

    /// <summary>
    /// バトルの状態更新.
    /// </summary>
    public void UpdateProgress(BattleState state, ListItem_BattleUnit unit = null)
    {
        foreach(var c in observerList){
            c.ChangeProgress(state, unit);
        }
    }

    /// <summary>
    /// 対象の攻撃者の次回の行動順確定.
    /// </summary>
    public void DecideAttackerNextOrder(IActionOrderItem attacker, int order, bool isAdd)
    {
        foreach(var c in observerList) {
            c.DecideAttackerNextOrder(attacker, order, isAdd);
        }
    }

    public void SortActionOrder(ActionOrderSortInfo[] sortInfo)
    {
        foreach(var c in observerList) {
            c.SortActionOrder(sortInfo);
        }
    }

    /// <summary>
    /// 敵の再ターゲッティング.
    /// </summary>
    public void RetargetEnemy(ListItem_BattleUnit target)
    {
        foreach(var c in observerList) {
            c.RetargetEnemy(target);
        }
    }

    /// <summary>
    /// 味方の再ターゲッティング.
    /// </summary>
    public void RetargetAlly(ListItem_BattleUnit target)
    {
        foreach(var c in observerList) {
            c.RetargetAlly(target);
        }
    }

    /// <summary>
    /// スキル詳細表示.
    /// </summary>
    public void DrawSkillDetail(BattleLogic.SkillParameter skillParam, bool bVisible)
    {
        foreach(var c in observerList){
            c.DrawSkillDetail(skillParam, bVisible);
        }
    }

    /// <summary>
    /// ユニット詳細表示.
    /// </summary>
    public void DrawUnitDetail(ListItem_BattleUnit invoker, bool bVisible)
    {
        foreach(var c in observerList){
            c.DrawUnitDetail(invoker, bVisible);
        }
    }

    public void ActionStart(ListItem_BattleUnit invoker, BattleLogic.SkillParameter skillParam)
    {
        foreach(var c in observerList){
            c.ActionStart(invoker, skillParam);
        }
    }

    /// <summary>
    /// リセット.
    /// </summary>
    public void ClearState()
    {
        observerList.Clear();
    }
        
    private List<IBattleObserver> observerList = new List<IBattleObserver>();
}
