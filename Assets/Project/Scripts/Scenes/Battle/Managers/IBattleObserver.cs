using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;

/// <summary>
/// バトルView共通インターフェイス.Mediatorに操作される.
/// </summary>
public interface IBattleObserver
{
    /// <summary>進行状況変化時に呼び出される.</summary>
    void ChangeProgress(BattleState state, ListItem_BattleUnit unit);
    /// <summary>対象の攻撃者の次回の行動順確定..</summary>
    void DecideAttackerNextOrder(IActionOrderItem attacker, int order, bool isAdd);
    /// <summary>外部要因で行動順が変わった場合に呼ばれる.</summary>
    void SortActionOrder (ActionOrderSortInfo[] sortInfo);
    /// <summary>敵の再ターゲッティング処理.</summary>
    void RetargetEnemy(ListItem_BattleUnit target);
    /// <summary>味方の再ターゲッティング処理.</summary>
    void RetargetAlly(ListItem_BattleUnit target);
    /// <summary>スキル効果の詳細表示.表示時と非表示時に呼ばれる</summary>
    void DrawSkillDetail(BattleLogic.SkillParameter skillParam, bool bVisible);
    /// <summary>ユニット詳細Viewを表示.表示時と非表示時に呼ばれる</summary>
    void DrawUnitDetail(ListItem_BattleUnit invoker, bool bVisible);
    /// <summary> アクション開始時に呼ばれる </summary>
    void ActionStart(ListItem_BattleUnit invoker, BattleLogic.SkillParameter skillParam);
}
