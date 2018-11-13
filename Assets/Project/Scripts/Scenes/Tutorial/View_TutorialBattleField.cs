using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using SmileLab;
using BattleLogic;


/// <summary>
/// View : チュートリアルバトルフィールド.
/// </summary>
public class View_TutorialBattleField : ViewBase
{
	/// <summary>
    /// 味方リスト.
    /// </summary>
	public List<ListItem_BattleUnit> AllyList 
	{ 
		get {
			var playerRoot = this.GetScript<Transform>("AreaPlayer");
			return playerRoot.GetComponentsInChildren<ListItem_BattleUnit>().ToList();
		}
	}
    /// <summary>
    /// 敵リスト.
    /// </summary>
	public List<ListItem_BattleUnit> EnemyList 
	{ 
		get {
			var enemyRoot = this.GetScript<Transform>("AreaEnemy");
			return enemyRoot.GetComponentsInChildren<ListItem_BattleUnit>().ToList();
		}
	}

    /// <summary>
    /// 味方キャラクタータップ時のイベント.ユニット名を指定する.
	/// </summary>
	public event Action<string> DidTapAllyEvent;


    /// <summary>
    /// キャッシュも解放しておく.
    /// </summary>
	public override void Dispose()
	{
        BattleResourceManager.Shared.Dispose ();
		base.Dispose();
	}

	/// <summary>
	/// 非同期な初期化.
	/// </summary>
	public void InitAsync(List<Parameter> allyList, List<Parameter> enemyList, Action didInit)
	{      
		m_allyParamList = new List<Parameter>(allyList);
		m_enemyParamList = new List<Parameter>(enemyList);

        // エリアルートからアンカーポジションを取得.
		var playerRoot = this.GetScript<Transform>("AreaPlayer");
		foreach(Transform t in playerRoot){
			var num = -1;
			if(!int.TryParse(t.name, out num)){
				continue;
			}
			if (num > 9) {
                continue;
            }
			m_areaPlayerObjects.Add(t.gameObject);
		}
		var enemyRoot = this.GetScript<Transform>("AreaEnemy");
		foreach (Transform t in enemyRoot) {
            var num = -1;
            if (!int.TryParse(t.name, out num)) {
                continue;
            }
			if(num > 9){
				continue;
			}
			m_areaEnemyObjects.Add(t.gameObject);
        }

		// 背景.
        BattleResourceManager.Shared.TutorialBattleLoadResource (m_allyParamList, m_enemyParamList, 1,
            () => {
                var bgGO = BattleResourceManager.Shared.GetBattleBG();

                // sortingLayerの設定
                var renderer = bgGO.GetComponent<Renderer>();
                if(renderer != null) {
                    renderer.sortingLayerName = "BattleCharacter";
                }
                foreach(var childRenderer in bgGO.GetComponentsInChildren<Renderer>(true))
                {
                    childRenderer.sortingLayerName = "BattleCharacter";
                }
                this.GetScript<Transform>("BgRoot").gameObject.AddInChild(bgGO);
                CreateBattleArea();
                didInit();
            }
        );

		InvisibleTarget();
	}   

    /// <summary>
    /// 敵再ターゲッティング処理.
    /// </summary>
	public void EnemyRetarget(ListItem_BattleUnit newTarget)
	{
		if (newTarget == null) {
            return;
        }
        if (newTarget.IsPlayer) {
            return;
        }
        var root = this.GetScript<Transform>("AreaEnemy");
        foreach (var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if (anchor.Unit == null) {
                continue;
            }
			anchor.IsEnableTarget = anchor.ComareUnit(newTarget);
        }
	}

    /// <summary>
	/// ターゲット表示を消す.
    /// </summary>
    public void InvisibleTarget()
    {
        var root = this.GetScript<Transform>("AreaEnemy");
        foreach (var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if (anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = false;
        }
        root = this.GetScript<Transform>("AreaPlayer");
        foreach (var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if (anchor.Unit == null) {
                continue;
            }
            anchor.IsEnableTarget = false;
        }
    }
    /// <summary>
    /// ターゲットを表示する.
    /// </summary>
	public void VisibleTarget(ListItem_BattleUnit enemy)
	{
		var root = this.GetScript<Transform>("AreaEnemy");
        foreach (var anchor in root.GetComponentsInChildren<ListItem_BattleUnitAnchor>()) {
            if (anchor.Unit == null) {
                continue;
            }
			anchor.IsEnableTarget = anchor.ComareUnit(enemy);
        }
		// TODO : 味方のターゲッティングは今のところ必要なさそう.
	}
    // 3 x 3 のマス作成.
    private void CreateBattleArea()
	{
		// 味方.
		int areaCount = m_areaPlayerObjects.Count;
		for (int index = 0; index < areaCount; ++index) {
			var anchor = m_areaPlayerObjects.Find(o => int.Parse(o.name) == (index+1));
			anchor.DestroyChildren();
			var go = GameObjectEx.LoadAndCreateObject("Battle/ListItem_BattleUnitAnchor", anchor);
			var c = go.GetOrAddComponent<ListItem_BattleUnitAnchor>();
			int row = index / CNT_COLUMN;
            int column = index % CNT_COLUMN;
			int parameterIndex = m_allyParamList.FindIndex(x => x.Position.Equals(true, row, column));
            bool unitInArea = m_allyParamList.FindIndex(x => x.Position.InArea(false, row, column)) >= 0;
			if (parameterIndex >= 0) { 
				var param = m_allyParamList[parameterIndex];
                var unitRes = BattleResourceManager.Shared.GetResource(param);
                var weaponRes = BattleResourceManager.Shared.GetResource(param.Weapon);
                c.Init(index, row, column, true, unitInArea, param, unitRes, weaponRes);
				c.SetButtonMsg("BattleAnchor", () => DidTapAllyTutorial(param.Name));
			}else{
                c.Init(index, row, column, true, unitInArea, null, null, null);
			}         
		}
        // 敵.
		areaCount = m_areaEnemyObjects.Count;
        for (int index = 0; index < areaCount; ++index) {
			var anchor = m_areaEnemyObjects.Find(o => int.Parse(o.name) == (index + 1));
			anchor.DestroyChildren();
			var go = GameObjectEx.LoadAndCreateObject("Battle/ListItem_BattleUnitAnchor", anchor);
            var c = go.GetOrAddComponent<ListItem_BattleUnitAnchor>();         
            int row = index / CNT_COLUMN;
            int column = index % CNT_COLUMN;
			int parameterIndex = m_enemyParamList.FindIndex(x => x.Position.Equals(false, row, column));
            bool unitInArea = m_enemyParamList.FindIndex(x => x.Position.InArea(false, row, column)) >= 0;
            if (parameterIndex >= 0) {
				var param = m_enemyParamList[parameterIndex];
                var unitRes = BattleResourceManager.Shared.GetResource(param);
                var weaponRes = BattleResourceManager.Shared.GetResource(param.Weapon);
                c.Init(index, row, column, false, unitInArea, param, unitRes, weaponRes);
            } else {
                c.Init(index, row, column, false, unitInArea, null, null, null);
            }
        }
	}

    // 味方タップ.
    void DidTapAllyTutorial(string unitName)
	{
        if (DidTapAllyEvent != null) {
			DidTapAllyEvent(unitName);
        }
	}

	private List<Parameter> m_allyParamList;
	private List<Parameter> m_enemyParamList;
	private List<GameObject> m_areaPlayerObjects = new List<GameObject>();
	private List<GameObject> m_areaEnemyObjects = new List<GameObject>();

	const int CNT_COLUMN = 3;
}