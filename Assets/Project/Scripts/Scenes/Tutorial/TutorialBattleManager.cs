using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using BattleLogic;
using TMPro;


/// <summary>
/// チュートリアル中のバトル管理クラス.
/// </summary>
public class TutorialBattleManager : ViewBase
{   
    /// <summary>
    /// バトルViewを生成する.
    /// </summary>
	public static void CreateBattleView(int step, Action<TutorialBattleManager> didInit)
	{      
		tutoStep = step;
		actionCount = 0;      
		MakeAllyParameterList();
		MakeEnemyParameterList();
        
		if(instance != null){
			instance.Dispose();
		}
		var root = new GameObject("TutoriaBattleManager");
		instance = root.AddComponent<TutorialBattleManager>();
        root.AddComponent<BattleEffectManager> ();
		BattleEffectManager.DidCreateEffectObject += CallbackDidCreateEffectObj;
		var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleField", root);
        GameObjectEx.LoadAndCreateObject ("Battle/BattleManagers");
		field = go.GetOrAddComponent<View_TutorialBattleField>();
		field.DidTapAllyEvent += CallbackDidTapAlly;
		field.InitAsync(allyParameterList, enemyParameterList, () => {
			var allUnits = new List<IActionOrderItem>(field.AllyList.Select(a => a as IActionOrderItem).ToList());
			allUnits.AddRange(field.EnemyList.Select(a => a as IActionOrderItem).ToList());
			orderQueue = new ActionOrder(allUnits);         
			go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleUI", root);
            go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
            ui = go.GetOrAddComponent<View_TutorialBattleUI>();
			ui.DidUpdateViewStatus += CallbackUpdateViewStatus;
			ui.Init(ref orderQueue);

			// 初期ターゲット.
			var target = field.EnemyList[0];
			RetargetThisEnemy(target);
			field.EnemyList.ForEach(x => x.SetAllyTarget(field.EnemyList[1]));
			field.VisibleTarget(target);
			var allyTarget = field.AllyList[0];
			field.AllyList.ForEach(x => x.SetAllyTarget(allyTarget));

			didInit(instance);
		});
	}
    
    /// <summary>
    /// 開始処理.
    /// </summary>
    public void StartBattle(Action didStart)
	{
		ui.BattleOpen(didStart);
	}

    /// <summary>
    /// 次の行動を開始する.
    /// </summary>
    public void StartNextAction(Action didActionEnd)
	{
		var top = orderQueue.Peek() as ListItem_BattleUnit;
		if(top.IsPlayer){
			ui.SelectAllyAction(top, action => {     
				this.StartCoroutine(this.ActionProc(action, top, didActionEnd));            
			});         
		}else{
			// チュートリアル中敵は通常攻撃しかしない想定.
			var action = top.Parameter.ActionSkillList.FirstOrDefault(x => x.IsNormalAction);
			this.StartCoroutine(this.ActionProc(action, top, didActionEnd));            
		}
	}
	private IEnumerator ActionProc(SkillParameter action, ListItem_BattleUnit actor, Action didEnd)
	{
		View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.None);
		
		// 次の手番を指定した通りに設定する.
        if (tutoStep == 1) {
			actor.Parameter.SetWeight((actionCount+1)*10000000);                           // step1なら2回以上行動しないので一番後ろになる値を設定.
		}else if(tutoStep == 2){
			
		}else{
			Debug.LogError("TutorialBattleManager ActionProc Error!! : Unknown step. step="+tutoStep);
			yield break;
		}

		// Taget選択
		if(tutoStep == 1){
			if(actionCount <= 0){
				actor.SetTarget(field.EnemyList[0]);    // パトリックが、前列の敵に 長銃で通常攻撃をする。
			}else if(actionCount <= 1){
				actor.SetTarget(field.AllyList.Find(a => a.Parameter.Name.Contains("クリスティア")));
			}else if(actionCount <= 2){
				actor.SetAllyTarget(field.AllyList.Find(a => a.Parameter.Name.Contains("クリスティア")));
			}else if(actionCount <= 3){
				actor.SetTarget(field.AllyList.Find(a => a.Parameter.Name.Contains("クリスティア")));
			}else if(actionCount <= 4){
				actor.SetTarget(field.EnemyList.First(e => !e.IsDead));
            }
		}else if(tutoStep == 2){
            
        }

		// 必要に応じてスキル結果を加工.      
		SkillEffectResultBase skillRes = null; 
        if (tutoStep == 1) {
            if (actionCount <= 0) {
				// パトリックが、前列の敵に 長銃で通常攻撃をする。 →半分くらいダメージ与える
				skillRes = CreateDamageResult((actor.Target.Parameter.Hp / 2));
            } else if (actionCount <= 1) {
				// 敵の最前列のキャラがクリスティアに 攻撃をしてくる。 クリスティアはHPの1/5くらいのダメージをくらう
				skillRes = CreateDamageResult((actor.Target.Parameter.Hp / 5));
            } else if (actionCount <= 2) {
				// アルンのキャラスキル発動 →クリスティアに1000回復
				skillRes = CreateHealResult(1000);            
			} else if (actionCount <= 3) {
				// 敵の最前列のキャラがクリスティアに 攻撃をしてくる。 クリスティアはHPの1/5くらいのダメージをくらう
				skillRes = CreateDamageResult((actor.Target.Parameter.Hp / 5));            
            } else if (actionCount <= 4) {
                // クリスティアの必殺技技
				skillRes = CreateDamageResult(field.EnemyList.Select(e => e.Parameter.Hp).Max(), true);
            }
        } else if (tutoStep == 2) {

        }

        orderQueue.Dequeue();
		var actionRes = TutorialBattleCalculation.SimpleAction(allyParameterList, enemyParameterList, actor.Parameter, actor.Target.Parameter, actor.AllyTarget.Parameter, action, skillRes);

		var index = orderQueue.Enqueue(actor);
		ui.UpdateTimeLineDecidedNext(actor, index);
        
		field.InvisibleTarget();                        // 攻撃前にターゲット表示を消す.

        // カットインの表示
        if (action.IsSpecial) {
            bool cutinWait = true;
            BattleCutin.StartCutinEffect (actor.Parameter.ID, actor.Parameter.Element.Enum, actor.StandingImage,
                action.Skill.display_name,
                actor.Parameter.VoiceFileName,
                () => {
                    actor.StandingImage.transform.SetParent(null);
                    actor.StandingImage.SetActive(false);
                    cutinWait = false;
                }
            );
            yield return new WaitUntil(() => !cutinWait);
        } 

		yield return actor.ActionProc(actionRes);    // カメラタイプは使ってない.
        
		orderQueue.RemoveDisableItems();                // コールバック側で整合性が取れなくなることがあるのでOrderQueue内のリストは攻撃ごとに削除する.      
        // 敵の死亡時のリターゲットは手動操作のため不要.

		// 終了判定.
        if (tutoStep == 1) {
            // 4手番で終了.
            if (actionCount >= 4) {
                WinMotionStart(didEnd);
                yield break;
            }
        } else if (tutoStep == 2) {

        }

		// 行動順更新.
        ui.UpdateTimeLineAfterAttack();
  
		if(actor.IsPlayer){
            ui.UpdateUnitUIAnimation(Screen_BattleUI.UnitUIAnimState.TurnEnd, null, action, () => {});
		}  

        // 事後処理.次の行動に向けたもの.ここでやっちゃう.
		if (tutoStep == 1) {
            if (actionCount <= 1) {
				// アルンのキャラスキルを使えるようにしておく.
				var invoker = field.AllyList.Find(a => a.Parameter.Name.Contains("アルン"));
				var skill = invoker.Parameter.ActionSkillList.First(s => !s.IsSpecial && !s.IsNormalAction);
				skill.Charge(skill.Skill.charge_time);            
            } else if (actionCount <= 2) {
				// クリスティアの必殺技を使えるようにしておく.
                var invoker = field.AllyList.Find(a => a.Parameter.Name.Contains("クリスティア"));
				invoker.Parameter.ChargeSp(invoker.Parameter.MaxSp, true);
			} else if (actionCount <= 3) {
				
            }
        } else if (tutoStep == 2) {
            
        }

		didEnd();
		++actionCount;
	}   

    /// <summary>
    /// 破棄.
    /// </summary>
	public override void Dispose()
	{
		if(ui != null && !ui.IsDestroyed){
			ui.Dispose();
		}
		if(field != null && !field.IsDestroyed){
			field.Dispose();
		}
		if(m_resultView != null){
			m_resultView.Dispose();
		}
		BattleEffectManager.DidCreateEffectObject -= CallbackDidCreateEffectObj;
		base.Dispose();      
	}
 
    // 勝利時モーション
    private void WinMotionStart(Action endWinMotion)
    {
		this.StartCoroutine(this.WinMotionProc(endWinMotion));
    }
    private IEnumerator WinMotionProc(Action endWinMotion)
    {
        List<IEnumerator> winProcs = new List<IEnumerator>();

        foreach (var unit in AwsModule.BattleData.AllyList) {
            winProcs.Add(unit.WinProc());
        }

        if (winProcs.Count > 0) {
            bool isNext = false;
            do {
                isNext = false;
                winProcs.ForEach(x => isNext |= x.MoveNext());
                yield return null;
            } while (isNext);
        }
		m_resultView = View_BattleResult.CreateForTutorial(field.AllyList.Select(a => MasterDataTable.card[a.Parameter.ID]).ToList(), endWinMotion);
    }

	// 引数のターゲットに強制的に変える.
    private static void RetargetThisEnemy(ListItem_BattleUnit target)
    {
        if (ui == null || field == null) {
            return;
        }
        if (target == null || target.InstanceObject == null) {
            return;
        }
        if (target.IsPlayer) {
            Debug.LogError("[TutorialBattleManager] RetargetThisEnemy Error!! : Target is player.");
            return;
        }
        if (target.IsDead) {
            Debug.LogError("[TutorialBattleManager] RetargetThisEnemy Error!! : Target already dead.");
            return;
        }
        var enemyList = field.EnemyList;
        if (enemyList == null || enemyList.Count <= 0) {
            return;
        }

        //Debug.Log("DidTapenemy : name=" + target.Parameter.Name + " index=" + target.Index);
        var newTarget = enemyList.FirstOrDefault(e => e.Index == target.Index);
        if (newTarget == null) {
            Debug.LogError("[TutorialBattleManager] RetargetThisEnemy Error!! : unknown error. " + target.Index + "is not found from list.");
            return;
        }
        // HateValueの更新
        enemyList.ForEach(x => x.HateValue = 0);
        newTarget.HateValue = 1;

        // ターゲットの設定
        field.AllyList.ForEach(x => x.SetTarget(newTarget));
        ui.RetargetEnemyFromAttacker(orderQueue.Peek() as ListItem_BattleUnit);
    }

	// 味方パラメータリストの作成.
	private static void MakeAllyParameterList()
	{
		allyParameterList.Clear();

		// リーンハルトとクリスティアが出てくる.
		var cristeaCard = MasterDataTable.card[104003011];    // 104003011=クリスティア
		var arunCard = MasterDataTable.card[104011011];       // 104011011=アルン
		var patricCard = MasterDataTable.card[104009011];     // 104009011=パトリック      
		var cristea = new Parameter(cristeaCard, 70, 2, 0, 1, true);
		var arun = new Parameter(arunCard, 30, 4, 1, 2, true);
		var patric = new Parameter(patricCard, 30, 5, 1, 0, true);
		if (tutoStep == 1) {
            patric.SetWeightDirect(0);   // パトリック先攻.
            cristea.SetWeightDirect(7000);  // 最後クリスティア.
            arun.SetWeightDirect(1000);  // アルン 3番手で回復
		} else if (tutoStep == 2) {
			
		}     
		allyParameterList.Add(cristea);
		allyParameterList.Add(arun);      
		allyParameterList.Add(patric);
	}
    // 敵パラメータリストの作成.
    private static void MakeEnemyParameterList()
	{
		enemyParameterList.Clear();

		var monster = MasterDataTable.monster[804101000];   // 804101000=ヴェルム兵士火
		var enemy = MasterDataTable.enemy_parameter[10001];
		var eList = new List<Parameter>();
        // ステップごとに変更.
		if(tutoStep == 1){
			var e = new Parameter(enemy, monster, 2, 10, 1, 2, 0, 1, 110110001);
            e.SetWeightDirect(100); // 2番手
			eList.Add(e);
			e = new Parameter(enemy, monster, 2, 10, 1, 4, 1, 2, 110110001);
            e.SetWeightDirect(5000); // 4番手
			eList.Add(e);
			e = new Parameter(enemy, monster, 2, 10, 1, 5, 1, 0, 110110001);
            e.SetWeightDirect(10000); // 5番手
            eList.Add(e);
		}else if(tutoStep == 2){
			
		}
		enemyParameterList.AddRange(eList);
	}
 
	// 追加用のダメージ結果情報を生成.
	private DamageSkillEffectResult CreateDamageResult(int damage, bool bCritical = false)
	{
		var info = new DamageInfo();
		info.heal = 0;
		info.damage = damage;
		info.affinityEnum = ElementAffinityEnum.normal;
		info.isCritical = bCritical;
		info.isHit = true;
		info.addWeight = 0;      
		var addDamage = new List<DamageInfo>();
		addDamage.Add(info);
		return new DamageSkillEffectResult(true, SkillEffectLogicEnum.damage, addDamage, null);
	}   
    // 回復情報生成.
	private HealSkillEffectResult CreateHealResult(int heal)
	{
		return new HealSkillEffectResult(true, SkillEffectLogicEnum.hp_heal, heal);
	}

	// コールバック:ui側のステータス表示更新のたび
    static void CallbackUpdateViewStatus()
	{
		if (tutoStep == 1) {
			if(actionCount <= 0){
				ui.SetActiveCustomButtonThisOnly("bt_AttackCommand");
		    } else if (actionCount <= 1) {
                // 敵のターン
			} else if (actionCount <= 2) {
				// クリスティアタップ待ち.
				ui.SetActiveCustomButtonThisOnly("");
			} else if (actionCount <= 3) {
				// 敵のターン
			} else if (actionCount <= 4) { 
				ui.SetActiveCustomButtonThisOnly("bt_SPCommand");
			}
        }
	}
    // コールバック：味方キャラクタータップ時.
	static void CallbackDidTapAlly(string unitName)
	{
		if(tutoStep == 1){
			if(actionCount != 2){
				return;
			}
			if(!unitName.Contains("クリスティア")){
				return;
			}
			View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.CharacterSkill);
			ui.SetActiveCustomButtonThisOnly("bt_Command");
		}
	}

	// コールバック : エフェクトオブジェクト生成時.
	static void CallbackDidCreateEffectObj(GameObject effectObj)
	{
		Debug.Log("CallbackDidCreateEffectObj : tutoStep=" + tutoStep + " actionCount=" + actionCount);
		if(tutoStep == 1){
			if(actionCount <= 0){
            } else if (actionCount <= 1) {
            } else if (actionCount <= 2) {
            } else if (actionCount <= 3) {
            } else if (actionCount <= 4) {
				// クリスティアの必殺技のターン.
				effectObj.transform.position = field.GetScript<Transform>("AreaEnemy/5").position;
			}
		}
	}

	private View_BattleResult m_resultView;
	private static int tutoStep;
	private static View_TutorialBattleUI ui;
	private static View_TutorialBattleField field;
	private static List<Parameter> allyParameterList = new List<Parameter>();    // 味方のパラメータリスト.
	private static List<Parameter> enemyParameterList = new List<Parameter>();   // 敵のパラメータリスト.
	private static ActionOrder orderQueue;   

	private static int actionCount = 0; // 行動回数.

	private static TutorialBattleManager instance;
}