using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SmileLab;


/// <summary>
/// Screen : バトルフィールド.
/// </summary>
public class Screen_BattleField : ViewBase, IBattleObserver
{
    [SerializeField]
    private GameObject BgRootObject;

    [SerializeField]
    private GameObject[] AreaPlayerObjects;

    [SerializeField]
    private GameObject[] AreaEnemyObjects;

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {
        BattleProgressManager.Shared.RegistObserverAsLast(this);
        BattleProgressManager.Shared.BattleField = this;
        var bgObject = BattleResourceManager.Shared.GetBattleBG ();
        // sortingLayerの設定
        var renderer = bgObject.GetComponent<Renderer>();
        if(renderer != null) {
            renderer.sortingLayerName = "BattleCharacter";
        }
        foreach(var childRenderer in bgObject.GetComponentsInChildren<Renderer>(true))
        {
            childRenderer.sortingLayerName = "BattleCharacter";
        }
        BgRootObject.AddInChild(bgObject);
        this.CreatePlayerArea ();
        this.CreateEnemyArea ();
    }

    private UnitResourceLoader GetUnitResource(BattleLogic.Parameter parameter)
    {
        return BattleResourceManager.Shared.GetResource(parameter);
    }

    private WeaponResourceLoader GetWeaponResource(BattleLogic.WeaponParameter weapon)
    {
        return BattleResourceManager.Shared.GetResource(weapon);
    }

    #region imprements IBattleViewColleague
    /// <summary>バトル進捗状況の変化.</summary>
    public void ChangeProgress(BattleState state, ListItem_BattleUnit unit)
    {
        switch(state){
            case BattleState.DidAlignOrder:
                CallbackDidAlignOrder ();
                break;
            case BattleState.WillAction:
                InvisibleTarget ();
                break;
            case BattleState.DidAction:
                {
                    var attacker = BattleProgressManager.Shared.OrderQueue.Peek() as ListItem_BattleUnit;
                    if (attacker != null) {
                        CallbackEnemyRetarget (attacker.Target);
                    }
                    VisibleTarget ();
                }
                break;
            case BattleState.TurnEnd:
                CallbackTurnEnd ();
                break;
            case BattleState.EndWave:
                CallbackChangeWave();
                break;
        }
    }
    /// <summary>攻撃者の行動順確定.</summary>
    public void DecideAttackerNextOrder(IActionOrderItem attacker, int order, bool isAdd)
    {
        // 何かあれば.
    }

    public void SortActionOrder (ActionOrderSortInfo[] sortInfo)
    {
    }

    /// <summary>敵再ターゲッティング時.</summary>
    public void RetargetEnemy(ListItem_BattleUnit target)
    {
        CallbackEnemyRetarget(target);
    }
    /// <summary>敵再ターゲッティング時.</summary>
    public void RetargetAlly(ListItem_BattleUnit target)
    {
        CallbackAllyRetarget(target);
    }
    /// <summary>スキル詳細表示.</summary>
    public void DrawSkillDetail(BattleLogic.SkillParameter skillParam, bool bVisible)
    {
        // 非表示.
        var unitAnchorList = this.GetScript<Transform>("AreaEnemy").gameObject.GetComponentsInChildren<ListItem_BattleUnitAnchor>().ToList();
        unitAnchorList.AddRange (this.GetScript<Transform> ("AreaPlayer").gameObject.GetComponentsInChildren<ListItem_BattleUnitAnchor> ());
        if(!bVisible){
            foreach (var anchor in unitAnchorList) {
                anchor.IsEffectRange = false;
            }
            return;
        }
        // 表示.
        var attacker = BattleProgressManager.Shared.OrderQueue.Peek() as ListItem_BattleUnit;
        if (attacker == null || !attacker.IsPlayer) {
            return; // プレイヤーのみ.
        }
        var rangeList = BattleLogic.Calculation.GetRangeInPositionList(
            attacker.Parameter, attacker.Target.Parameter,
            attacker.AllyTarget.Parameter, skillParam);
        foreach(var anchor in unitAnchorList){
            var bTarget = rangeList.Any(p => anchor.PositionData == p);
            anchor.IsEffectRange = bTarget;
        }
    }
    /// <summary>ユニット詳細表示.</summary>
    public void DrawUnitDetail(ListItem_BattleUnit invoker, bool bVisible){}
    public void ActionStart(ListItem_BattleUnit invoker, BattleLogic.SkillParameter skillParam){}
    #endregion

    public void ResetPlayerArea()
    {
        var unitList = AwsModule.BattleData.AllyList;
        int areaCount = AreaPlayerObjects.Length;
        for (int index = 0; index < areaCount; ++index) {
            int row = index / CNT_COLUMN;
            int column = index % CNT_COLUMN;
            int unitIndex = unitList.FindIndex(x => x.Parameter.Position.Equals(true, row, column));
            //bool unitInArea = unitList.FindIndex(x => x.Parameter.Position.InArea(false, row, column)) >= 0;

            var anchor = AreaPlayerObjects [index].GetComponentInChildren<ListItem_BattleUnitAnchor> ();
            if (unitIndex >= 0) {
                var unit = unitList [unitIndex];
                anchor.SetUnit (unit);
                Vector3 offset = Vector3.zero;
                if (unit.Parameter.Position.UnitSize != null) {
                    offset = GetOffesetUnitPosition (true, anchor.PositionData.row, anchor.PositionData.column, unit.Parameter.Position.UnitSize);
                }
                anchor.SetOffsetPostition (offset);

                unit.SetParentAnchor (anchor);
                unit.ActiveObject ();
            }
        }
    }

    // プレイヤーのフィールドエリア作成.
    private void CreatePlayerArea()
    {
        var parameterList = AwsModule.BattleData.AllyParameterList;
        int areaCount = AreaPlayerObjects.Length;
        for (int index = 0; index < areaCount; ++index) {
            AreaPlayerObjects [index].DestroyChildren();
            var go = GameObjectEx.LoadAndCreateObject("Battle/ListItem_BattleUnitAnchor", AreaPlayerObjects [index]);
            var c = go.GetOrAddComponent<ListItem_BattleUnitAnchor>();

            int row = index / CNT_COLUMN;
            int column = index % CNT_COLUMN;
            int parameterIndex = parameterList.FindIndex(x => x.Position.Equals(true, row, column));
            bool unitInArea = parameterList.FindIndex(x => x.Position.InArea(false, row, column)) >= 0;
            if(parameterIndex >= 0) {
                var param = parameterList [parameterIndex];
                var unitRes = GetUnitResource (param);
                var weaponRes = GetWeaponResource (param.Weapon);
                Vector3 offset = Vector3.zero;
                if (param.Position.UnitSize != null) {
                    offset = GetOffesetUnitPosition (true, row, column, param.Position.UnitSize);
                }
                c.Init(index, row, column, true, unitInArea, param, unitRes, weaponRes);
                c.SetOffsetPostition (offset);
            } else {
                c.Init(index, row, column, true, unitInArea, null, null, null);
            }
        }

        foreach(var supportParameter in parameterList.Where(x => x.Hp > 0 && x.PositionIndex >= 10)) {
            var param = supportParameter;
            var unitRes = GetUnitResource (param);
            var weaponRes = GetWeaponResource (param.Weapon);

            var go = GameObjectEx.LoadAndCreateObject("Battle/Unit", this.gameObject);
            var scale = go.transform.localScale;
            var rotate = go.transform.eulerAngles;
            var c = go.GetOrAddComponent<ListItem_BattleUnit>();
            go.transform.localScale = scale;
            go.transform.rotation = CameraHelper.SharedInstance.BattleCamera.transform.rotation;
            c.IsPlayer = true;
            c.Init(null, param.PositionIndex, param);
            c.FetchResource (unitRes, weaponRes, false);
            go.SetActive (false);

            AwsModule.BattleData.AllySupportList.Add (c);
        }

        // 復帰時に用に死亡しているユニットのリストも復元
        foreach(var deadParameter in parameterList.Where(x => x.Hp <= 0)) {
            var param = deadParameter;
            var unitRes = GetUnitResource (param);
            var weaponRes = GetWeaponResource (param.Weapon);

            var go = GameObjectEx.LoadAndCreateObject("Battle/Unit", this.gameObject);
            var scale = go.transform.localScale;
            var rotate = go.transform.eulerAngles;
            var c = go.GetOrAddComponent<ListItem_BattleUnit>();
            go.transform.localScale = scale;
            go.transform.rotation = CameraHelper.SharedInstance.BattleCamera.transform.rotation;
            c.IsPlayer = true;
            c.Init(null, param.PositionIndex, param);
            c.FetchResource (unitRes, weaponRes, false);
            go.SetActive (false);

            AwsModule.BattleData.AllyDeadList.Add (c);
        }
        AwsModule.BattleData.AllySupportList.OrderBy (x => x.Parameter.PositionIndex);
    }
    // 敵のフィールドエリア作成.
    private void CreateEnemyArea()
    {
        var parameterList = AwsModule.BattleData.EnemyParameterList;
        int areaCount = AreaEnemyObjects.Length;
        for (int index = 0; index < areaCount; ++index) {
            AreaEnemyObjects [index].DestroyChildren();
            var go = GameObjectEx.LoadAndCreateObject("Battle/ListItem_BattleUnitAnchor", AreaEnemyObjects [index]);
            var c = go.GetOrAddComponent<ListItem_BattleUnitAnchor>();

            int row = index / CNT_COLUMN;
            int column = index % CNT_COLUMN;
            int parameterIndex = parameterList.FindIndex(x => x.Position.Equals(false, row, column));
            bool unitInArea = parameterList.FindIndex(x => x.Position.InArea(false, row, column)) >= 0;
            if(parameterIndex >= 0) {
                var param = parameterList [parameterIndex];
                var unitRes = GetUnitResource (param);
                var weaponRes = GetWeaponResource (param.Weapon);
                Vector3 offset = Vector3.zero;
                if (param.Position.UnitSize != null) {
                    offset = GetOffesetUnitPosition (false, row, column, param.Position.UnitSize);
                }
                c.Init(index, row, column, false, unitInArea, param, unitRes, weaponRes);
                c.SetOffsetPostition (offset);
            } else {
                c.Init(index, row, column, false, unitInArea, null, null, null);
            }
        }

        foreach(var supportParameter in parameterList.Where(x => x.PositionIndex >= 10)) {
            var param = supportParameter;
            var unitRes = GetUnitResource (param);
            var weaponRes = GetWeaponResource (param.Weapon);

            var go = GameObjectEx.LoadAndCreateObject("Battle/Unit", this.gameObject);
            var scale = go.transform.localScale;
            var rotate = go.transform.eulerAngles;
            var c = go.GetOrAddComponent<ListItem_BattleUnit>();
            go.transform.localScale = scale;
            go.transform.rotation = CameraHelper.SharedInstance.BattleCamera.transform.rotation;
            c.IsPlayer = false;
            c.Init(null, param.PositionIndex, param);
            c.FetchResource (unitRes, weaponRes, false);
            go.SetActive (false);

            AwsModule.BattleData.EnemySupportList.Add (c);
        }

        // 復帰時に用に死亡しているユニットのリストも復元
        foreach(var deadParameter in parameterList.Where(x => x.Hp <= 0)) {
            var param = deadParameter;
            var unitRes = GetUnitResource (param);
            var weaponRes = GetWeaponResource (param.Weapon);

            var go = GameObjectEx.LoadAndCreateObject("Battle/Unit", this.gameObject);
            var scale = go.transform.localScale;
            var rotate = go.transform.eulerAngles;
            var c = go.GetOrAddComponent<ListItem_BattleUnit>();
            go.transform.localScale = scale;
            go.transform.rotation = CameraHelper.SharedInstance.BattleCamera.transform.rotation;
            c.IsPlayer = true;
            c.Init(null, param.PositionIndex, param);
            c.FetchResource (unitRes, weaponRes, false);
            go.SetActive (false);

            AwsModule.BattleData.EnemyDeadList.Add (c);
        }
        AwsModule.BattleData.EnemySupportList.OrderBy (x => x.Parameter.PositionIndex);
    }

    private Vector3 GetOffesetUnitPosition(bool isPlayer, int originalRow, int originalColumn, EnemyUnitSizeDefine unitSize)
    {
        Vector3 offset = Vector3.zero;
        List<Vector3> positionList = new List<Vector3>();
        GameObject[] areaObjects = AreaPlayerObjects;
        if (!isPlayer) {
            areaObjects = AreaEnemyObjects;
        }
        int unisSizeCount = unitSize.SizeList.Length;

        int originalIndex = originalRow * BattleLogic.PositionData.MAX_COLUMN + originalColumn;
        Vector3 originalPosition = areaObjects [originalIndex].transform.position;
        for (int i = 0; i < unisSizeCount; ++i) {
            var sizeDef = unitSize.SizeList [i];
            int index = (originalRow + sizeDef.row) * BattleLogic.PositionData.MAX_COLUMN + (originalColumn + sizeDef.column);
            if (index >= 0 && index < areaObjects.Length) {
                positionList.Add(areaObjects [index].transform.position);
            }
        }
        if (unitSize.ColumnPosition != EnemyUnitSizePositionEnum.Original) {
            if (unitSize.ColumnPosition == EnemyUnitSizePositionEnum.Center) {
                offset.x = positionList.Sum (x => x.x) / (float)positionList.Count - originalPosition.x;
            }
            else if(unitSize.ColumnPosition == EnemyUnitSizePositionEnum.Max) {
                if (!isPlayer) {
                    offset.x = positionList.Min (x => x.x) - originalPosition.x;
                } else {
                    offset.x = positionList.Max (x => x.x) - originalPosition.x;
                }
            }
            else if(unitSize.ColumnPosition == EnemyUnitSizePositionEnum.Min) {
                if (!isPlayer) {
                    offset.x = positionList.Max (x => x.x) - originalPosition.x;
                } else {
                    offset.x = positionList.Min (x => x.x) - originalPosition.x;
                }
            }
        }
        if(unitSize.RowPosition != EnemyUnitSizePositionEnum.Original) {
            if (unitSize.RowPosition == EnemyUnitSizePositionEnum.Center) {
                offset.y = positionList.Sum (x => x.y) / (float)positionList.Count - originalPosition.y;
            }
            else if(unitSize.RowPosition == EnemyUnitSizePositionEnum.Max) {
                offset.y = positionList.Max (x => x.y) - originalPosition.y;
            }
            else if(unitSize.RowPosition == EnemyUnitSizePositionEnum.Min) {
                offset.y = positionList.Min (x => x.y) - originalPosition.y;
            }
        }

        return offset;
    }

    #region Callbacks

    // コールバック : 敵のリターゲット.
    void CallbackEnemyRetarget(ListItem_BattleUnit newTarget)
    {
        if(newTarget == null){
            return;
        }
        if(newTarget.IsPlayer) {
            return;
        }
        enemyTarge = newTarget;
        var root = this.GetScript<Transform>("AreaEnemy");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = anchor.ComareUnit(enemyTarge);
        }
    }

    // コールバック : 味方のリターゲット.
    void CallbackAllyRetarget(ListItem_BattleUnit newTarget)
    {
        if(newTarget == null){
            return;
        }
        if(!newTarget.IsPlayer) {
            return;
        }
        allyTarget = newTarget;
        var root = this.GetScript<Transform>("AreaPlayer");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = anchor.ComareUnit(allyTarget);
        }
    }

    // ターゲット表示を消す
    void InvisibleTarget()
    {
        var root = this.GetScript<Transform>("AreaEnemy");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = false;
        }
        root = this.GetScript<Transform>("AreaPlayer");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = false;
        }
    }

    void VisibleTarget()
    {
        var root = this.GetScript<Transform>("AreaEnemy");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = anchor.ComareUnit(enemyTarge);
        }
        root = this.GetScript<Transform>("AreaPlayer");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = anchor.ComareUnit(allyTarget);
        }
    }

    // コールバック : WAVE切り替え.
    void CallbackChangeWave()
    {
        if(AwsModule.BattleData.EndBattle){
            return;
        }
        this.CreateEnemyArea();
        BattleProgressManager.Shared.InitActionOrder();
    }

    // コールバック : ターン終了時
    void CallbackTurnEnd()
    {
        var next = BattleProgressManager.Shared.OrderQueue.Peek () as ListItem_BattleUnit;
        ChangeTurnActive (next);
    }

    // コールバック : 行動順初期化またはリセット時
    void CallbackDidAlignOrder()
    {
        var next = BattleProgressManager.Shared.OrderQueue.Peek () as ListItem_BattleUnit;
        ChangeTurnActive (next);
    }

    // 現在行動中のユニットのエフェクトの表示切り替え
    void ChangeTurnActive(ListItem_BattleUnit unit)
    {
        Transform root;
        root = this.GetScript<Transform>("AreaPlayer");

        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            if (unit == null) {
                anchor.IsEnableTurnActive = false;
            } else {
                anchor.IsEnableTurnActive = anchor.ComareUnit (unit);
            }
        }

        root = this.GetScript<Transform>("AreaEnemy");
        foreach(var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if(anchor.Unit == null) {
                continue;
            }
            if (unit == null) {
                anchor.IsEnableTurnActive = false;
            } else {
                anchor.IsEnableTurnActive = anchor.ComareUnit (unit);
            }
        }
    }
    #endregion

    public Vector3 GetFieldPosition (bool isPlayer, BattleLogic.PositionData targetPosition, BattleLogic.PositionData[] recivers, SkillPerformanceTargetEnum target)
    {
//        bool isPlayer = recivers.Any(x => x.isPlayer);
        var allyList = recivers.Where(x => x.isPlayer == isPlayer);
        var enemyList = recivers.Where(x => x.isPlayer != isPlayer);

        int minAllyRow = 0;
        int minAllyColumn = 0;
        if (allyList.Count () > 0) {
            minAllyRow = allyList.Min (x => x.row);
            minAllyColumn = allyList.Min (x => x.column);
        }
        int minEnemyRow = 0;
        int minEnemyColumn = 0;
        if (enemyList.Count () > 0) {
            minEnemyRow = enemyList.Min (x => x.row);
            minEnemyColumn = enemyList.Min (x => x.column);
        }

        int index = 0;
        GameObject[] allyObjects = AreaPlayerObjects;
        GameObject[] enemyObjects = AreaEnemyObjects;
        if (!isPlayer) {
            enemyObjects = AreaPlayerObjects;
            allyObjects = AreaEnemyObjects;
        }
            
        switch (target) {
        case SkillPerformanceTargetEnum.AllyAll:
            index = 4; // 中心
            return allyObjects [index].transform.position;
        case SkillPerformanceTargetEnum.AllyColumn:
            index = minAllyRow * CNT_COLUMN;
            return allyObjects [index].transform.position;
        case SkillPerformanceTargetEnum.AllyRow:            
            index = CNT_COLUMN + minAllyColumn;
            return allyObjects [index].transform.position;
        case SkillPerformanceTargetEnum.EnemyAll:
            index = 4; // 中心
            return enemyObjects [index].transform.position;
        case SkillPerformanceTargetEnum.EnemyColumn:
            index = minEnemyRow * CNT_COLUMN + 1;
            return enemyObjects [index].transform.position;
        case SkillPerformanceTargetEnum.EnemyRow:
            index = CNT_COLUMN + minEnemyColumn;
            return enemyObjects [index].transform.position;
        case SkillPerformanceTargetEnum.CenterTarget:
            if (targetPosition != null) {
                if (targetPosition.isPlayer == isPlayer) {
                    return allyObjects [targetPosition.row * CNT_COLUMN + targetPosition.column].transform.position;
                } else {
                    return enemyObjects [targetPosition.row * CNT_COLUMN + targetPosition.column].transform.position;
                }
            }
            break;
        }
        return Vector3.zero;
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    private ListItem_BattleUnit enemyTarge;
    private ListItem_BattleUnit allyTarget;

    private const int CNT_COLUMN = 3;
}
