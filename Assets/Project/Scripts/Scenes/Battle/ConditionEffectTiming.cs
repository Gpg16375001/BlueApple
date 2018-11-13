using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConditionEffectTiming : IActionOrderItem
{
    public BattleLogic.ConditionParameter parent {
        get;
        private set;
    }
    public BattleLogic.Parameter unit {
        get;
        private set;
    }

    public ActionOrderItemType ItemType {
        get {
            return ActionOrderItemType.Condition;
        }
    }

    public int Index {
        get {
            if (parent.HasDamegeLogic) {
                return 0;
            } else {
                return 1;
            }
        }
    }

    public bool IsPlayer {
        get {
            return unit.Position.isPlayer;
        }
    }

    private int _weight;
    public int Weight {
        get {
            return _weight;
        }
    }

    public bool IsRemove {
        get {
            return !parent.IsEnable || unit.Hp <= 0;
        }
    }

    public ConditionEffectTiming(BattleLogic.Parameter unit, BattleLogic.ConditionParameter condition, int? startWeight = null, bool isEnqueue=true)
    {
        parent = condition;
        this.unit = unit;
        if (startWeight.HasValue) {
            _weight = startWeight.Value;
        } else {
            SetWeight ();
        }
        if (isEnqueue) {
            BattleProgressManager.Shared.EnqueueActionOrder (this, true);
        }
    }

    public void SubWeight (int value)
    {
        _weight = Mathf.Max (0, _weight - value);
    }

    public void SetWeight()
    {
        _weight = parent.NextTiming;
    }


    public void ResetWeight ()
    {
        //_weight = parent.NextTiming;
    }

    public override string ToString ()
    {
        return string.Format ("[ConditionEffectTiming: parent={0}, unit={1}, ItemType={2}, Index={3}, IsPlayer={4}, Weight={5}, IsRemove={6}]", parent, unit.Name, ItemType, Index, IsPlayer, Weight, IsRemove);
    }
}
