using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using SmileLab;


/// <summary>
/// バトルユニット配置用アンカーScript.
/// </summary>
public class ListItem_BattleUnitAnchor : ViewBase
{
    /// <summary>
    /// ポジション番号
    /// </summary>
    /// <value>The index of the position.</value>
    public int PositionIndex { get; private set; }

    /// <summary>
    /// 配置されているユニット.
    /// </summary>
    public ListItem_BattleUnit Unit { get; private set; }

    /// <summary>マスのポシション情報.</summary>
    public BattleLogic.PositionData PositionData { get; private set; }

    /// <summary>
    /// ターゲッティング？
    /// </summary>
    public bool IsEnableTarget 
    {
        get {
            return Unit != null && Unit.IsDisplayTarget;
        }
        set {
            if (Unit != null) {
                Unit.IsDisplayTarget = value;
            }
        }
    }

    /// <summary>
    /// スキル効果範囲内？
    /// </summary>
    public bool IsEffectRange
    {
        get {
            return this.GetScript<Transform>("TargetRange").gameObject.activeSelf;
        }
        set {
            this.GetScript<Transform>("TargetRange").gameObject.SetActive(value);
        }
    }

    public bool IsEnableTurnActive
    {
        get {
            return this.GetScript<Transform>("Turn").gameObject.activeSelf;
        }
        set {
            this.GetScript<Transform>("Turn").gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(int index, int row, int column, bool bPlayer, bool unitInArea, BattleLogic.Parameter parameter, UnitResourceLoader unitResource, WeaponResourceLoader weaponResrouce)
    {
        PositionIndex = index;
        this.PositionData = new BattleLogic.PositionData();
        this.PositionData.row = row;
        this.PositionData.column = column;
        this.PositionData.isPlayer = bPlayer;

        if(parameter != null) {
            // フロアーのロード
            GameObjectEx.LoadAndCreateObject("Battle/FloorNormal", this.gameObject);

            // 復帰時処理用　Hpが0の場合は死んでるのでスキップする
            if (parameter.Hp <= 0) {
                return;
            }

            var go = GameObjectEx.LoadAndCreateObject("Battle/Unit", this.gameObject);
            var scale = go.transform.localScale;
            var rotate = go.transform.eulerAngles;
            var c = go.GetOrAddComponent<ListItem_BattleUnit>();
            go.transform.localScale = scale;
            go.transform.rotation = CameraHelper.SharedInstance.BattleCamera.transform.rotation;
            c.IsPlayer = bPlayer;
            c.Init(this, index, parameter);
            c.FetchResource (unitResource, weaponResrouce);
            if(bPlayer) {
                AwsModule.BattleData.AllyList.Add(c);
            } else {
                AwsModule.BattleData.EnemyList.Add(c);
            }
            this.Unit = c;
        } else if (unitInArea) {
            // 大型ユニットのサイズ内である
            GameObjectEx.LoadAndCreateObject("Battle/FloorNormal", this.gameObject);
        } else {
            GameObjectEx.LoadAndCreateObject("Battle/FloorEmpty", this.gameObject);
        }
    }

    // ここで追加出現の処理される
    public void SupportProc(ListItem_BattleUnit support)
    {
        if (support == null) {
            return;
        }

        support.SetParentAnchor (this);
        SetUnit (support);

        bool oldEnableTarget = this.IsEnableTarget;
        // 自分自身がターゲットの場合
        if (oldEnableTarget) {
            if (Unit.IsPlayer) {
                BattleProgressManager.Shared.RetargetAlly (this.Unit);
            } else {
                BattleProgressManager.Shared.RetargetEnemy (this.Unit);
            }
        }
    }

    public void SetOffsetPostition(Vector3 offset)
    {
        if (Unit != null) {
            Unit.transform.localPosition = offset;
        }
    }

    /// <summary>
    /// 引数のユニットと同一かどうか.
    /// </summary>
    public bool ComareUnit(ListItem_BattleUnit unit)
    {
        if(Unit == null) {
            return false;
        }
        return Unit.IsPlayer == unit.IsPlayer && Unit.Index == unit.Index && Unit.Parameter.ID == unit.Parameter.ID;
    }

    public void SetUnit(ListItem_BattleUnit unit)
    {
        if (ComareUnit (unit) && unit.transform.parent == this.transform) {
            return;
        }

        var scale = unit.InstanceObject.transform.localScale;
        unit.InstanceObject.transform.SetParent (transform);

        unit.InstanceObject.transform.localPosition = Vector3.zero;
        unit.InstanceObject.transform.localScale = scale;
        unit.InstanceObject.transform.rotation = CameraHelper.SharedInstance.BattleCamera.transform.rotation;
        Unit = unit;
    }
}
