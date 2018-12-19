using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SmileLab;
using BattleLogic;


/// <summary>
/// enum : バトルの状態.
/// </summary>
public enum BattleState
{
    None,
    DidAlignOrder,  // 行動順初期化 or リセット
    NextOrder,      // 次の行動者へ
    SelectAction,   // ユーザー入力待ち
    WillAction,     // 行動直前
    DidAction,      // 行動直後
    TurnStart,      // アクション開始前
    TurnEnd,        // ターン終了
    StartWave,      // WAVE開始時
    EndWave,        // WAVE終了時
    BattleStart,    // 戦闘開始
    GameOver,       // ゲームオーバー時
}

/// <summary>
/// バトルの進捗管理を行うクラス.(シングルトン)
/// </summary>
public class BattleProgressManager
{
    /// <summary>
    /// 現在の状態.
    /// </summary>
    public BattleState CurrentState { get; private set; }

    /// <summary>共通インスタンス.</summary>
    public static BattleProgressManager Shared
    {
        get {
            if(m_instance == null) {
                m_instance = new BattleProgressManager();
            }
            return m_instance;
        }
    }
    private static BattleProgressManager m_instance;

    public Screen_BattleField BattleField {
        get;
        set;
    }

    public Screen_BattleUI BattleUI {
        get;
        set;
    }

    /// <summary>初期化済み？</summary>
	public bool IsInit { get; private set; }

    /// <summary>一時停止.Auto中の場合などに呼ぶことでProcessing終了タイミングに休止を入れることが可能.</summary>
    public bool IsPause { get; set; }

    /// <summary>処理中？</summary>
    public bool IsProcessing { get; private set; }

    /// <summary>行動順リスト.</summary>
    public ActionOrder OrderQueue { get; private set; }


    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {
        // フラグ周りの初期化
        IsProcessing = false;
        IsPause = false;
        if (OrderQueue != null) {
            OrderQueue.Clear ();
        }
        m_missionData = AwsModule.BattleData.MissionProgressData;

		IsInit = true;
    }

    public void Dispose()
    {
        OrderQueue.Clear ();

        CurrentState = BattleState.None;
        IsProcessing = false;
        IsPause = false;


        m_battleSubject.ClearState ();

        m_missionData = null;

        Time.timeScale = 1.0f;

        BattleField = null;
        BattleUI = null;
        m_instance = null;
    }

    public Vector3 GetFieldPosition (bool isPlayer, BattleLogic.PositionData targetPosition, BattleLogic.PositionData[] recivers, SkillPerformanceTargetEnum target)
    {
        if (BattleField == null) {
            return Vector3.zero;
        }
        return BattleField.GetFieldPosition (isPlayer, targetPosition, recivers, target);
    }

    /// <summary>
    /// 末尾に登録.
    /// </summary>
    public void RegistObserverAsLast(IBattleObserver collaegue)
    {
        m_battleSubject.RegistObserverAsLast(collaegue);
    }
    /// <summary>
    /// 最優先登録.
    /// </summary>
    public void RegistObserverAsFirst(IBattleObserver collaegue)
    {
        m_battleSubject.RegistObserverAsFirst(collaegue);
    }


    public void BattleStart()
    {
        // 復帰時にバトルの情報を確認する。
        if(AwsModule.BattleData.IsGameOver()) {
            //Debug.Log("Game Over!!!!!");
            BattleScheduler.Instance.AddAction (() => {
                
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
                UpdateProgress (BattleState.GameOver);
            });
            return;
        }
        if(AwsModule.BattleData.IsWaveClear()) {
            BattleScheduler.Instance.AddAction (() => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
                AwsModule.BattleData.UpdateWave ();
                UpdateProgress (BattleState.EndWave);
                if(AwsModule.BattleData.EndBattle) {
                    m_battleSubject.ClearState ();
                }
            });
            return;
        }
        if (AwsModule.BattleData.EndBattle) {
            BattleScheduler.Instance.AddAction (() => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
                UpdateProgress (BattleState.EndWave);
                m_battleSubject.ClearState ();
            });
            return;
        }

        InitActionOrder();

        // バトルの開始を投げる
        UpdateProgress (BattleState.BattleStart);

        Time.timeScale = AwsModule.BattleData.BattleSpeed == 1 ? 1.0f : 3.0f;
    }

    public int ChangeBattleSpeed()
    {
        AwsModule.BattleData.ChangeBattleSpeed ();
        Time.timeScale = AwsModule.BattleData.BattleSpeed == 1 ? 1.0f : 3.0f;

        return AwsModule.BattleData.BattleSpeed;
    }

    /// <summary>
    /// 並び順初期化.ユニット生成後実行可能.
    /// </summary>
    public void InitActionOrder()
    {
        var allItems = AwsModule.BattleData.AllyList
                                .Where(x => !x.IsDead)
                                .Concat(AwsModule.BattleData.EnemyList.Where(x => !x.IsDead))
                                .Select(x => (IActionOrderItem)x)
                                .ToList();
        allItems.AddRange(AwsModule.BattleData.AllyList.SelectMany(x => x.Parameter.Conditions.ConditionEffectItemList()).Select(x => (IActionOrderItem)x));
        allItems.AddRange(AwsModule.BattleData.EnemyList.SelectMany(x => x.Parameter.Conditions.ConditionEffectItemList()).Select(x => (IActionOrderItem)x));
        this.OrderQueue = new ActionOrder(allItems);
        this.InitSetTarget();
        UpdateProgress(BattleState.DidAlignOrder);
    }

    public void SortActionOrder()
    {
        m_battleSubject.SortActionOrder(OrderQueue.Sort ());
    }

    /// <summary>
    /// OrderQueueにItemを積むする
    /// </summary>
    /// <param name="item">積むアイテム</param>
    /// <param name="isAdd">追加アイテムか?<c>追加</c></param>
    public void EnqueueActionOrder(IActionOrderItem item, bool isAdd = false)
    {
        // すでにリムーブ対象なら追加しない
        if (item.IsRemove) {
            return;
        }
        var index = OrderQueue.Enqueue (item);
        m_battleSubject.DecideAttackerNextOrder(item, index, isAdd);
    }

    public void RemoveActionItem(IActionOrderItem item)
    {
        OrderQueue.RemoveItem (item);
    }

    public void WaveStart()
    {
        foreach(var unit in AwsModule.BattleData.AllyList.Concat(AwsModule.BattleData.EnemyList)) {
            if (unit.Parameter.InvokePassiveSkillList.Any (x => x != null && x.HasAutoWaveStartLogic)) {
                // 手順毎発動スキルの発動
                StartAutoAction(unit, Calculation.AutoAction(unit.Parameter, false), null);
            }
        }
        BattleScheduler.Instance.AddAction (() => {
            UpdateProgress (BattleState.StartWave);
            NextOrder ();
        });
    }
    public void NextOrder()
    {
        if (IsPause) {
            // ポーズ時はポーズ解除をまつ
            BattleScheduler.Instance.AddWaitUntil (() => !IsPause);
            BattleScheduler.Instance.AddAction (NextOrder);
            return;
        }

        if (CurrentState == BattleState.GameOver || CurrentState == BattleState.EndWave) {
            return;
        }

        UpdateProgress (BattleState.NextOrder);
        var item = OrderQueue.Peek();
        if (item.ItemType == ActionOrderItemType.Unit) {
            // ユニットの行動
            StartUnitAction(item as ListItem_BattleUnit);
        } else if (item.ItemType == ActionOrderItemType.Condition) {
            // 状態の発動
            StartConditionEffect ();
        } else {
            // 判定できない場合は無視
            OrderQueue.Dequeue ();
            NextOrder ();
        }
    }

    private void ExcuteTurnAuto(ListItem_BattleUnit unit)
    {
        // 手順毎発動スキルの発動
        var actionResults = Calculation.AutoAction (unit.Parameter, true);
        // 手順毎発動スキルの表示
        StartAutoAction (unit, actionResults, () => {
            if (CurrentState == BattleState.GameOver || CurrentState == BattleState.EndWave) {
                return;
            }

            if(unit.IsDead) {
                OrderQueue.RemoveDisableItems();               // コールバック側で整合性が取れなくなることがあるのでOrderQueue内のリストは攻撃ごとに削除する.
                this.UpdateTargetDead();
                UpdateProgress(BattleState.DidAction, unit);

                UpdateProgress (BattleState.TurnEnd);
            } else {
                ExecStartUnitAction();
            }
        });
    }
    private void StartUnitAction(ListItem_BattleUnit unit)
    {
        if (unit.Parameter.InvokePassiveSkillList.Any (x => x != null && x.HasAutoTurnLogic)) {
            ExcuteTurnAuto (unit);
            return;
        }

        ExecStartUnitAction ();
    }

    public void ExecStartUnitAction()
    {
        var actor = OrderQueue.Peek() as ListItem_BattleUnit;

        if(actor.Parameter.Conditions.IsNotAction) {    // 状態異常で行動できない場合
            BattleScheduler.Instance.AddAction (() => {
                NotActionForConditon();
            });
        }
        else {
            BattleScheduler.Instance.AddAction (() => {
                var forcedAI = actor.Parameter.InvokePassiveSkillList.Any(x => x != null && x.HasForcedAILogic);
                if (!forcedAI && !AwsModule.BattleData.IsAuto && actor.IsPlayer) {
                    UpdateProgress (BattleState.SelectAction);
                } else {
                    BattleAIDefine overrideAI = null;
                    if(forcedAI) {
                        var skillEffect = actor.Parameter.InvokePassiveSkillList.FirstOrDefault(x => x != null && x.HasForcedAILogic);
                        overrideAI = skillEffect.InvokeEffect.GetValue<BattleAIDefine>(SkillEffectLogicArgEnum.AssignAI);
                    }
                    AutoAction (overrideAI);
                }
            });
        }
    }

    /// <summary>
    /// オート行動.敵行動時呼び出し
    /// </summary>
    public void AutoAction(BattleAIDefine overrideAI = null)
    {
        var actor = OrderQueue.Peek() as ListItem_BattleUnit;
        if (actor == null) {
            NextOrder ();
            return;
        }

        actor.Parameter.BattleAI.DecideAction (actor, ActionStart, overrideAI);
    }

    /// <summary>
    /// 外部からターゲットが変わった際に呼ぶ.
    /// </summary>
    public void RetargetEnemy(ListItem_BattleUnit newTarget)
    {
        this.RetargetThisEnemy(newTarget);
    }

    /// <summary>
    /// 外部から味方側ターゲットが変わった際に呼ぶ.
    /// </summary>
    public void RetargetAlly(ListItem_BattleUnit newTarget)
    {
        this.RetargetThisAlly(newTarget);
    }

    /// <summary>
    /// 外部から味方側ユニット死亡ごとに呼ぶ.
    /// </summary>
    public void DeadAllyUnit()
    {
        if (m_missionData != null) {
            ++m_missionData.CharaDeadCount;
        }
        AwsModule.BattleData.UpdateBattleUnit();
    }

	public void DeadEnemyUnit(ListItem_BattleUnit unit)
	{
        if(unit.Parameter.HasDropItem) {
            // 2Dカメラの位置に合わせるためにポジションの計算を行う。
            var screenPoint = CameraHelper.SharedInstance.BattleCamera.WorldToScreenPoint (unit.Anchor.position);
            var startPos = CameraHelper.SharedInstance.Camera2D.ScreenToWorldPoint (screenPoint);

            BattleDropItemEffect.Create(startPos, BattleUI.TreasurePosition);
            AwsModule.BattleData.DropedItems.AddRange (unit.Parameter.DropItems);
        }
        AwsModule.BattleData.UpdateBattleUnit();
	}

    /// <summary>
    /// スキル詳細表示.
    /// </summary>
    public void DrawSkillDetail(SkillParameter skillParam, bool bVisible)
    {
        m_battleSubject.DrawSkillDetail(skillParam, bVisible);
    }

    /// <summary>
    /// ユニット詳細表示.
    /// </summary>
    public void DrawUnitDetail(ListItem_BattleUnit invoker, bool bVisible)
    {
        m_battleSubject.DrawUnitDetail(invoker, bVisible);
    }

    /// <summary>
    /// 撤退時処理
    /// </summary>
    public void RetireProc(Action didEnd)
    {
        if(IsProcessing){
            return;
        }
        AwsModule.BattleData.IsRetire = true;
        IsPause = false;
        UpdateProgress(BattleState.GameOver);
    }

    /// <summary>
    /// 勝利時モーション
    /// </summary>
    public void WinMotionStart(ListItem_BattleUnit lastUnit, Action endWinMotion)
    {
        BattleScheduler.Instance.AddSchedule(this.WinMotionProc(lastUnit, endWinMotion));
        // 喋り終わるまでまつ
        BattleScheduler.Instance.AddWaitUntil (() => !SoundManager.SharedInstance.IsPlayVoice);
    }
    private IEnumerator WinMotionProc(ListItem_BattleUnit lastUnit, Action endWinMotion)
    {
        List<IEnumerator> winProcs = new List<IEnumerator> ();

        // 最後に行動した人が喋る
        if (lastUnit != null && !string.IsNullOrEmpty (lastUnit.Parameter.VoiceFileName)) {
            SoundManager.SharedInstance.PlayVoice (lastUnit.Parameter.VoiceFileName, SoundVoiceCueEnum.battle_end);
        }

        foreach (var unit in AwsModule.BattleData.AllyList) {
            winProcs.Add(unit.WinProc ());
        }

        if (winProcs.Count > 0) {
            bool isNext = false;
            do {
                isNext = false;
                winProcs.ForEach (x => isNext |= x.MoveNext ());
                yield return null;
            } while(isNext);
        }

        if (endWinMotion != null) {
            endWinMotion ();
        }
    }

    /// <summary>
    /// コンテニュー処理
    /// </summary>
    public void ContinueProc()
    {
        // コンテニューカウントを追加
        if (AwsModule.BattleData.MissionProgressData != null) {
            AwsModule.BattleData.MissionProgressData.ContinueCount++;
        }

        // 味方のHPを回復して状態異常は取り除く
        foreach (var unit in AwsModule.BattleData.AllyParameterList) {
            unit.Resurrection ();
        }

        // サポートが出撃しているからサポートと入れ替わったキャラを元に戻す
        foreach (var unit in AwsModule.BattleData.AllyParameterList) {
            if (unit.IsPositionChanged) {
                var swapUnit = AwsModule.BattleData.AllyParameterList.FirstOrDefault(x => x != unit && x.PositionIndex == unit.OriginalPositionIndex);
                SwapUnitPosition (unit, swapUnit);
            }
        }

        // DeadListからそれぞれのリストに復帰させる
        foreach (var battleUnit in AwsModule.BattleData.AllyDeadList) {
            battleUnit.ResetDead();
            if (battleUnit.Parameter.PositionIndex < 10) {
                AwsModule.BattleData.AllyList.Add (battleUnit);
            } else {
                AwsModule.BattleData.AllySupportList.Add (battleUnit);
            }
        }
        AwsModule.BattleData.AllyDeadList.Clear ();

        // パッシブスキルの更新
        var passiveTargets = AwsModule.BattleData.SallyAllyParameterList.ToArray();
        foreach(var unit in AwsModule.BattleData.SallyAllyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit, passiveTargets);
        }
        foreach(var unit in AwsModule.BattleData.SallyEnemyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit, passiveTargets);
        }

        foreach(var unit in AwsModule.BattleData.SallyAllyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();
        }

        // 味方ユニットの配置をやり直す
        BattleField.ResetPlayerArea();

        // タイムラインの更新
        var allItems = AwsModule.BattleData.AllyList
            .Where(x => !x.IsDead).Select(x => (IActionOrderItem)x).ToList();
        allItems.AddRange(AwsModule.BattleData.AllyList.SelectMany(x => x.Parameter.Conditions.ConditionEffectItemList()).Select(x => (IActionOrderItem)x));

        foreach(var item in allItems) {
            OrderQueue.Enqueue(item);
        }
        BattleUI.ResetActionOrder ();
        InitSetTarget();
        foreach(var item in AwsModule.BattleData.AllyList) {
            item.UpdateCondition ();
        }
        AwsModule.BattleData.UpdateBattleData ();

        CurrentState = BattleState.TurnStart;

        NextOrder ();
    }


    public void UpdateBossInfo(Parameter unit)
    {
        if (BattleUI != null) {
            BattleUI.UpdateBossInfo (unit);
        }
    }

    public void UpdateBossCondition(Parameter unit)
    {
        if (BattleUI != null) {
            BattleUI.UpdateBossCondition (unit);
        }
    }

    /// <summary>
    /// 行動開始.
    /// </summary>
    public void ActionStart(SkillParameter action)
    {
        var actor = OrderQueue.Peek () as ListItem_BattleUnit;
        if (actor == null) {
            NextOrder ();
            return;
        }
        m_battleSubject.ActionStart (actor, action);
        BattleScheduler.Instance.AddSchedule(this.StartProc(action));
    }
    private IEnumerator StartProc(SkillParameter action)
    {
        while (IsPause) {
            if (CurrentState == BattleState.GameOver) {
                yield break;
            }
            yield return null;
        }

        IsProcessing = true;


        var attacker = OrderQueue.Dequeue() as ListItem_BattleUnit;
        UpdateProgress(BattleState.TurnStart, attacker);
        // 有効な自キャラ時にのみ手番更新.
        if (m_missionData != null) {
            if (attacker.IsPlayer) {            
                ++m_missionData.ActionCount;
                //Debug.Log(m_missionData.ActionCount + "手目....");
            }
        }

        if (action.IsSpecial) {
            bool cutinWait = true;
            BattleCutin.StartCutinEffect (attacker.Parameter.ID, attacker.Parameter.Element.Enum, attacker.StandingImage,
                action.Skill.display_name,
                attacker.Parameter.VoiceFileName,
                () => {
                    if(attacker.StandingImage != null) {
                        attacker.StandingImage.transform.SetParent(null);
                        attacker.StandingImage.SetActive(false);
                    }
                    cutinWait = false;
                }
            );
            yield return new WaitUntil(() => !cutinWait);
        } 
        yield return this.ActionProc(action, attacker);  // 攻撃.
        // ミッション：スキル使用チェック.
        if (m_missionData != null) {
            if (attacker.IsPlayer) {
                if (action.IsWeaponSkill) {
                    ++m_missionData.UseWeaponSkillCount;
                } else if (action.IsSpecial) {
                    ++m_missionData.UseSpecialCount;
                } else if (action.IsNormalAction) {
                    ++m_missionData.UseCharaSkillCount;
                }
            }
        }

        BattleScheduler.Instance.AddAction (() => {
            IsProcessing = false;
        });

        // ターン終了Wave切り替え時もGameOver時もある.

        AwsModule.BattleData.TurnCount++;
        UpdateProgress(BattleState.TurnEnd, attacker);
        CheckEndWaveOrGameOver (attacker);

        // GameOver or EndWaveなら続行しない.
        if (CurrentState == BattleState.GameOver) {
            //Debug.Log("GameOver from attack proc.");
            IsPause = false;
            yield break;
        }
        if(CurrentState == BattleState.EndWave){
            //Debug.Log("EndWave from attack proc.");
            yield break;
        }

        if(attacker.IsPlayer) {
            AwsModule.BattleData.UpdateBattleData();
        }
        if (m_missionData != null) {
            AwsModule.BattleData.MissionProgress.Update (m_missionData);
        }
    }
    // 行動処理処理.
    private IEnumerator ActionProc(SkillParameter action, ListItem_BattleUnit actor)
    {
        if(actor.IsDead) {
            yield break;
        }

        var subWeight = actor.Parameter.Weight;                  // 行動したユニットの現在のWeight値を保存
        actor.SubWeight(subWeight);
        OrderQueue.ForEach(x => x.SubWeight(subWeight));   // 行動したユニットのWeightを減算する。

        // 行動処理.攻撃や回復.
        var actionRes = Calculation.Action(actor.Parameter, actor.Target.Parameter, actor.AllyTarget.Parameter, action);

        // Weightで並び替えて次のユニット情報を取得
        EnqueueActionOrder(actor);

        UpdateProgress(BattleState.WillAction, actor);

        yield return actor.ActionProc(actionRes);

        if(actor.Parameter.Conditions.HasCondition) {
            actor.Parameter.ConditionCallingOffAction();
        }
        OrderQueue.RemoveDisableItems();               // コールバック側で整合性が取れなくなることがあるのでOrderQueue内のリストは攻撃ごとに削除する.

        this.UpdateTargetDead();
        UpdateProgress(BattleState.DidAction, actor);

        AwsModule.BattleData.UpdateBattleUnit();    // 進捗データ更新.
        if (m_missionData != null) {
            m_missionData.CurrentMembersHp = AwsModule.BattleData.AllyParameterList.Select (p => p.Hp).ToList ().Sum ();
        }
    }

    private void NotActionForConditon()
    {
        var actor = OrderQueue.Peek () as ListItem_BattleUnit;
        if (actor == null) {
            NextOrder ();
            return;
        }

        BattleScheduler.Instance.AddSchedule(this.CoNotActionForConditon ());

    }
    IEnumerator CoNotActionForConditon()
    {
        while (IsPause) {
            if (CurrentState == BattleState.GameOver) {
                yield break;
            }
            yield return null;
        };

        IsProcessing = true;

        var actor = OrderQueue.Dequeue () as ListItem_BattleUnit;

        UpdateProgress(BattleState.TurnStart, actor);
        var subWeight = actor.Weight;
        OrderQueue.ForEach(x => x.SubWeight(subWeight));   // 発動したエフェクトのWeightを減算する。


        actor.Parameter.SetWeight (actor.Parameter.Conditions.NotActionWait);
        actor.Parameter.Actioned ();

        EnqueueActionOrder (actor);
        UpdateProgress(BattleState.WillAction, actor);

        // 状態異常のなんかの表示
        yield return new WaitForSeconds(1.0f);


        // 行動毎の解除判定
        if(actor.Parameter.Conditions.HasCondition) {
            actor.Parameter.ConditionCallingOffAction();
        }
        OrderQueue.RemoveDisableItems();
        this.UpdateTargetDead();

        AwsModule.BattleData.UpdateBattleUnit();    // 進捗データ更新.
        if (m_missionData != null) {
            m_missionData.CurrentMembersHp = AwsModule.BattleData.AllyParameterList.Select (p => p.Hp).ToList ().Sum ();
        }
        UpdateProgress(BattleState.DidAction, actor);

        BattleScheduler.Instance.AddAction (() => {
            IsProcessing = false;
        });

        // ターン終了Wave切り替え時もGameOver時もある.
        UpdateProgress(BattleState.TurnEnd, actor);
        AwsModule.BattleData.TurnCount++;
        CheckEndWaveOrGameOver (actor);

        // GameOver or EndWaveなら続行しない.
        if (CurrentState == BattleState.GameOver) {
            IsPause = false;
        }
    }

    // 状態効果発動処理
    private void StartConditionEffect()
    {
        var condition = OrderQueue.Peek () as ConditionEffectTiming;
        if (condition == null) {
            NextOrder ();
            return;
        }
        BattleScheduler.Instance.AddSchedule(this.CoStartConditionEffect ());
    }
    IEnumerator CoStartConditionEffect() 
    {
        while (IsPause) {
            if (CurrentState == BattleState.GameOver) {
                yield break;
            }
            yield return null;
        };

        IsProcessing = true;

        UpdateProgress(BattleState.TurnStart);
        var condition = OrderQueue.Dequeue () as ConditionEffectTiming;

        var subWeight = condition.Weight;                  // 発動したエフェクトの現在のWeight値を保存
        OrderQueue.ForEach(x => x.SubWeight(subWeight));   // 発動したエフェクトのWeightを減算する。

        var actionRes = Calculation.ConditionEffect(condition);

        EnqueueActionOrder (condition);

        //
        UpdateProgress(BattleState.WillAction);
        var battleUnit = AwsModule.BattleData.GetBattleUnit (condition.unit.Position);
        yield return battleUnit.ConditionEffectProc(actionRes);

        OrderQueue.RemoveDisableItems();
        this.UpdateTargetDead();

        AwsModule.BattleData.UpdateBattleUnit();    // 進捗データ更新.
        if (m_missionData != null) {
            m_missionData.CurrentMembersHp = AwsModule.BattleData.AllyParameterList.Select (p => p.Hp).ToList ().Sum ();
        }
        UpdateProgress(BattleState.DidAction);

        BattleScheduler.Instance.AddAction (() => {
            IsProcessing = false;
        });

        // ターン終了Wave切り替え時もGameOver時もある.
        UpdateProgress(BattleState.TurnEnd);
        CheckEndWaveOrGameOver ();

        // GameOver or EndWaveなら続行しない.
        if (CurrentState == BattleState.GameOver) {
            IsPause = false;
        }
    }

    private void StartAutoAction(ListItem_BattleUnit unit, List<ActionResult> results, Action NextCallback)
    {
        BattleScheduler.Instance.AddSchedule (this.CoStartAutoAction (unit, results, NextCallback));
    }
    IEnumerator CoStartAutoAction(ListItem_BattleUnit unit, List<ActionResult> results, Action NextCallback) 
    {
        while (IsPause) {
            if (CurrentState == BattleState.GameOver) {
                yield break;
            }
            yield return null;
        };

        IsProcessing = true;

        // 演出
        yield return unit.AutoActionProc(results);

        OrderQueue.RemoveDisableItems();
        this.UpdateTargetDead();

        AwsModule.BattleData.UpdateBattleUnit();    // 進捗データ更新.
        if (m_missionData != null) {
            m_missionData.CurrentMembersHp = AwsModule.BattleData.AllyParameterList.Select (p => p.Hp).ToList ().Sum ();
        }

        BattleScheduler.Instance.AddAction (() => {
            IsProcessing = false;
        });
        if (this.CheckEndWaveOrGameOver (unit)) {
            // GameOver or EndWaveなら続行しない.
            yield break;
        }

        if (NextCallback != null) {
            NextCallback ();
        }
    }

    // 敵味方それぞれのターゲット初期選定.
    private void InitSetTarget()
    {
        // TODO : 何らかの基準でターゲットを絞る.
        var enemyTarget = AwsModule.BattleData.EnemyList.Where(u => !u.IsDead).OrderBy(u => u.Index).First();

        // TODO : 何らかの基準でターゲットを絞る.
        var allyTarget = AwsModule.BattleData.AllyList.Where(u => !u.IsDead).OrderBy(u => u.Index).First();

        // 味方側ターゲット設定
        enemyTarget.HateValue = 1;
        RetargetEnemy (enemyTarget);
        allyTarget.HateValue = 1;
        RetargetAlly (allyTarget);

        // 敵側ターゲット設定
        foreach(var enemy in AwsModule.BattleData.EnemyList) {
            enemy.SetTarget(allyTarget);
            enemy.SetAllyTarget (enemyTarget);
        }
    }

    // 死亡時のターゲット更新処理. 
    private void UpdateTargetDead()
    {
        UpdateTargetEnemyDead ();
        UpdateTargetAllyDead ();
    }

    // 敵ターゲット更新処理
    private void UpdateTargetEnemyDead()
    {
        var enemyList = AwsModule.BattleData.EnemyList;
        if(enemyList == null || enemyList.Count <= 0) {
            return;
        }
        var oldTarget = enemyList.FirstOrDefault(e => (!e.IsDead && e.Parameter.Hp > 0) && e.HateValue >= 1);
        if(oldTarget != null) {
            AwsModule.BattleData.AllyList.ForEach (o => o.SetTarget (oldTarget));
            m_battleSubject.RetargetEnemy(oldTarget);
            return; // ターゲットがまだ生きているので更新しない.
        }
        var newTarget = enemyList.Select(e => { e.HateValue = 0; return e; })
            .ToList()
            .FirstOrDefault(e => !e.IsDead);
        if(newTarget == null) {
            AwsModule.BattleData.AllyList.ForEach (o => o.SetTarget (oldTarget));
            m_battleSubject.RetargetEnemy(oldTarget);
            return; // 死亡チェック.生存者がいない.
        }
        // リターゲット.
        newTarget.HateValue = 1;
        AwsModule.BattleData.AllyList.ForEach (o => o.SetTarget (newTarget));
        m_battleSubject.RetargetEnemy(newTarget);
    }

    // 味方ターゲット更新処理
    private void UpdateTargetAllyDead()
    {
        var allyList = AwsModule.BattleData.AllyList;
        if(allyList == null || allyList.Count <= 0) {
            return;
        }
        var oldTarget = allyList.FirstOrDefault(e => (!e.IsDead && e.Parameter.Hp > 0) && e.HateValue >= 1);
        if(oldTarget != null) {
            AwsModule.BattleData.AllyList.ForEach (o => o.SetAllyTarget (oldTarget));
            m_battleSubject.RetargetEnemy(oldTarget);
            return; // ターゲットがまだ生きているので更新しない.
        }
        var newTarget = allyList.Select(e => { e.HateValue = 0; return e; })
            .ToList()
            .FirstOrDefault(e => !e.IsDead);
        if(newTarget == null) {
            AwsModule.BattleData.AllyList.ForEach (o => o.SetAllyTarget (oldTarget));
            m_battleSubject.RetargetEnemy(oldTarget);
            return; // 死亡チェック.生存者がいない.
        }
        // リターゲット.
        newTarget.HateValue = 1;
        AwsModule.BattleData.AllyList.ForEach (o => o.SetAllyTarget (newTarget));
        m_battleSubject.RetargetAlly(newTarget);
    }

    // 引数のターゲットに強制的に変える.
    private void RetargetThisEnemy(ListItem_BattleUnit target)
    {
        if(target == null || target.InstanceObject == null){
            return;
        }
        if(target.IsPlayer){
            Debug.LogError("[BattleProgressManager] RetargetThisEnemy Error!! : Target is player.");
            return;
        }
        if(target.IsDead){
            Debug.LogError("[BattleProgressManager] RetargetThisEnemy Error!! : Target already dead.");
            return;
        }
        var enemyList = AwsModule.BattleData.EnemyList;
        if(enemyList == null || enemyList.Count <= 0) {
            return;
        }

        //Debug.Log("DidTapenemy : name=" + target.Parameter.Name + " index=" + target.Index);
        var newTarget = enemyList.FirstOrDefault(e => e.Index == target.Index);
        if(newTarget == null) {
            Debug.LogError("DidTapEnemy Error!! : unknown error. " + target.Index + "is not found from list.");
            return;
        }
        // HateValueの更新
        enemyList.ForEach (x => x.HateValue = 0);
        newTarget.HateValue = 1;

        // ターゲットの設定
        AwsModule.BattleData.AllyList.ForEach(x => x.SetTarget(newTarget));
        m_battleSubject.RetargetEnemy(newTarget);
    }

    // 引数の味方ターゲットに強制的に変える.
    private void RetargetThisAlly(ListItem_BattleUnit target)
    {
        if(target == null || target.InstanceObject == null){
            return;
        }
        if(!target.IsPlayer){
            Debug.LogError("[BattleProgressManager] RetargetThisAlly Error!! : Target is enemy.");
            return;
        }
        if(target.IsDead){
            Debug.LogError("[BattleProgressManager] RetargetThisAlly Error!! : Target already dead.");
            return;
        }
        var allyList = AwsModule.BattleData.AllyList;
        if(allyList == null || allyList.Count <= 0) {
            return;
        }

        //Debug.Log("DidTapAlly : name=" + target.Parameter.Name + " index=" + target.Index);
        var newTarget = allyList.FirstOrDefault(e => e.Index == target.Index);
        if(newTarget == null) {
            Debug.LogError("DidTapAlly Error!! : unknown error. " + target.Index + "is not found from list.");
            return;
        }
        // HateValueの更新
        allyList.ForEach (x => x.HateValue = 0);
        newTarget.HateValue = 1;

        allyList.ForEach (x => x.SetAllyTarget (newTarget));
        m_battleSubject.RetargetAlly(newTarget);
    }
        

    // 状態異常などでのWAVE終了かGameOverを検知.
    private bool CheckEndWaveOrGameOver(ListItem_BattleUnit unit = null)
    {
        if(AwsModule.BattleData.IsGameOver()) {
            //Debug.Log("Game Over!!!!!");
            BattleUI.DisableBattleMenu();
            BattleScheduler.Instance.AddAction (() => {
                UpdateProgress (BattleState.GameOver);
            });
            return true;
        }
        if(AwsModule.BattleData.IsWaveClear()) {
            //Debug.Log("End Wave!!!!!");
            if (AwsModule.BattleData.LastWave) {
                BattleUI.DisableBattleMenu ();
            }
            BattleScheduler.Instance.AddAction (() => {
                AwsModule.BattleData.UpdateWave ();
                UpdateProgress (BattleState.EndWave, unit);
                if (AwsModule.BattleData.EndBattle) {
                    BattleUI.DisableBattleMenu ();
                    m_battleSubject.ClearState ();
                }
            });
            return true;
        }
        if (AwsModule.BattleData.IsTurnOver ()) {
            BattleUI.DisableBattleMenu();
            BattleScheduler.Instance.AddAction (() => {
                UpdateProgress (BattleState.GameOver);
            });
            return true;
        }

        return false;
    }


    /// <summary>
    /// サポート
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="support">Support.</param>
    public void SupportProc(ListItem_BattleUnitAnchor anchor)
    {
        if (anchor == null) {
            return;
        }
        var battleData = AwsModule.BattleData;

        // サポートユニットを取得
        ListItem_BattleUnit support = null;
        if (anchor.PositionData.isPlayer) {
            support = battleData.AllySupportList.FirstOrDefault ();
        } else {
            support = battleData.EnemySupportList.FirstOrDefault ();
        }

        if(support != null) {
            // 出撃のためのパラメータの準備
            SupportUnitSally (anchor, support, support.IsPlayer);

            // ロジック上の入れ替え処理
            anchor.SupportProc (support);

            // 走ってくるアニメーション再生
            BattleScheduler.Instance.AddSchedule(CoSupportPerformanceProc(support));
        }
    }
    // サポートの出撃処理
    private void SupportUnitSally(ListItem_BattleUnitAnchor anchor, ListItem_BattleUnit support, bool isPlayer)
    {
        var battleData = AwsModule.BattleData;
        Formation formation = battleData.AllyFormation;
        int formationLevel = battleData.AllyFormationLevel;
        IEnumerable<Parameter> sallyAllyUnits = battleData.SallyAllyParameterList;
        IEnumerable<Parameter> sallyEnemyUnits = battleData.SallyEnemyParameterList;
        List<ListItem_BattleUnit> allySupportList = battleData.AllySupportList;
        List<ListItem_BattleUnit> allyList = battleData.AllyList;
        List<ListItem_BattleUnit> allyDeadList = battleData.AllyDeadList;

        // 敵側用のデータに入れ替え
        if(!isPlayer) {
            allyList = battleData.EnemyList;
            allySupportList = battleData.EnemySupportList;
            formation = battleData.EnemyFormation;
            formationLevel = battleData.EnemyFormationLevel;
            sallyAllyUnits = battleData.SallyEnemyParameterList;
            sallyEnemyUnits = battleData.SallyAllyParameterList;
            allyDeadList = battleData.EnemyDeadList;
        }

        // リストの更新
        allySupportList.Remove (support);
        allyList.Remove (anchor.Unit);
        // 死亡リストに登録
        allyDeadList.Add (anchor.Unit);
        allyList.Add (support);

        // ターゲット情報を引き継ぐ
        support.SetTarget (anchor.Unit.Target);
        support.SetAllyTarget (anchor.Unit.AllyTarget);

        // ポジション情報を引き継ぐ
        SwapUnitPosition (support.Parameter, anchor.Unit.Parameter);

        // 陣形効果の発動
        var skill = formation.GetPositionSkill(anchor.PositionIndex);
        if(skill != null) {
            // 陣形が効果発動するかを判定
            if(formation.SatisfyTheCondition(support.Parameter)) {
                // 陣形スキルをパッシブスキルとして追加
                support.Parameter.AddPassiveSkill(skill, formationLevel);
            }
        }

        // パッシブスキル効果の更新処理
        Calculation.AdditionPassiveSkill (support.Parameter);
        var targetList = new Parameter[1]{ support.Parameter };
        foreach(var unit in sallyAllyUnits) {
            Calculation.AdditionPassiveSkill (unit, targetList);
        }
        foreach(var unit in sallyEnemyUnits) {
            Calculation.AdditionPassiveSkill (unit, targetList);
        }

        // パラメータの計算をし直す
        support.Parameter.RecalcParameterVariation();
        // 速度変化があった時ようにここでリセットしておく
        support.Parameter.ResetWeight();

        AwsModule.BattleData.UpdateBattleData ();

        // タイムラインにデータを追加
        EnqueueActionOrder(support, true);
    }
    // 走って出てくる演出のコルーチン
    private IEnumerator CoSupportPerformanceProc(ListItem_BattleUnit support)
    {
        bool isEnd = false;
        support.ActiveObject();
        // 走ってくるアニメーション再生
        support.SupportPerformanceProc (
            () => isEnd = true
        );
        yield return new WaitUntil (() => isEnd);
    }

    private void SwapUnitPosition(Parameter a, Parameter b)
    {
        int oldIndex = a.PositionIndex;
        var oldPositionIsPlayer = a.Position.isPlayer;
        var oldPositionRow = a.Position.row;
        var oldPositionColumn = a.Position.column;
        var oldPositionUnitSize = a.Position.UnitSizeID;
        // ポジション情報を引き継ぐ
        a.SetPosition (b.PositionIndex, b.Position);
        b.SetPosition (oldIndex, oldPositionIsPlayer, oldPositionRow, oldPositionColumn, oldPositionUnitSize);
    }

    private void UpdateProgress(BattleState state, ListItem_BattleUnit unit = null)
    {
        m_battleSubject.UpdateProgress (state, unit);
        CurrentState = state;
    }

    // 外部からの生成は禁止.Sharedから参照すべし.
    private BattleProgressManager() { }

    private BattleSubject m_battleSubject = new BattleSubject();
    private BattleMissionProgressData m_missionData;
}
