using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using Spine.Unity;

using SmileLab;
using BattleLogic;

/// <summary>
/// ListItem : バトルユニットのリストアイテム.敵味方共通.
/// </summary>
public class ListItem_BattleUnit : ViewBase, IActionOrderItem
{
    private ListItem_BattleUnitAnchor m_ParentAnchor;
    /// <summary>
    /// インスタンスされているGameObjectのゲッター.
    /// </summary>
    public GameObject InstanceObject { get { return this.gameObject; } }

    /// <summary>
    /// アンカー位置のTransform.
    /// </summary>
    public Transform Anchor { get { return this.GetScript<Transform>("BattleAnchor"); } }

    public uGUIButtonBehaviour ButtonBehaviour { get { return this.GetScript<uGUIButtonBehaviour>("BattleAnchor"); } }

    /// <summary>
    /// 攻撃ポジションアンカー.
    /// </summary>
    public Transform AttackPosAnchor { get { return this.GetScript<Transform>("AttackPosAnchor"); } }

    /// <summary>
    /// エフェクトアンカー.
    /// </summary>
    public Transform EffectAnchor { get { return this.GetScript<Transform>("EffectAnchor"); } }

    public GameObject InfoGameObject { get { return GetScript<Transform> ("Info").gameObject; } }

    private SpineModelController m_ModelController;
    public SpineModelController ModelController { get { return m_ModelController; } }
    /// <summary>
    /// プレイヤーキャラか？
    /// </summary>
    public bool IsPlayer { get; set; }

    /// <summary>
    /// 現在の線登場のIndex(並び番号).
    /// </summary>
    public int Index { get { return m_index; } }
    private int m_index = 0;

    /// <summary>
    /// 死亡してる？
    /// </summary>
    public bool IsDead { get { return m_bDead; } }
    private bool m_bDead = false;

    public ActionOrderItemType ItemType {
        get {
            return ActionOrderItemType.Unit;
        }
    }

    public int Weight {
        get {
            return m_UnitParameter.Weight;
        }
    }

    public bool IsRemove {
        get {
            return IsDead;
        }
    }

    GameObject TargetGO;
    public bool IsDisplayTarget {
        get {
            return TargetGO.activeSelf;
        }
        set {
            TargetGO.SetActive (value);
        }
    }

    public void SubWeight(int value)
    {
        m_UnitParameter.Weight -= value;

        var prevHasConditions = Parameter.Conditions.HasCondition;
        Parameter.Conditions.Elapsed (Parameter, value);
        // 効果が消えた場合に即時反映させる
        if (prevHasConditions != Parameter.Conditions.HasCondition) {
            UpdateCondition ();
        }
    }

    public void ResetWeight()
    {
        m_UnitParameter.ResetWeight ();
    }

    /// <summary>
    /// 残りHP.
    /// </summary>
    /// <value>The remain hp.</value>
    public int RemainHP { get { return m_UnitParameter.Hp; } }

    /// <summary>
    /// ヘイト値.
    /// </summary>
    public int HateValue { get; set; }

    /// <summary>
    /// 攻撃対象.
    /// </summary>
    public ListItem_BattleUnit Target { get { return m_target; } }
    private ListItem_BattleUnit m_target;

    /// <summary>
    /// 
    /// </summary>
    public ListItem_BattleUnit AllyTarget { get { return m_allyTarget; } }
    private ListItem_BattleUnit m_allyTarget;

    /// <summary>
    /// ロジック計算用のユニットパラメータ
    /// </summary>
    public Parameter Parameter { get { return m_UnitParameter; } }
    private Parameter m_UnitParameter;

    private Sprite m_TimeLineIcon;
    public Sprite TimeLineIcon { 
        get { return m_TimeLineIcon; }
    }

    private GameObject m_StandingImage;
    public GameObject StandingImage { 
        get { return m_StandingImage; }
    }

    private bool m_Pressing;

    /// <summary>
    /// ユニットとしての破棄処理.
    /// </summary>
    public void DestroyUnit()
    {
        m_TimeLineIcon = null;
        m_StandingImage = null;
        this.Dispose();
    }

    /// <summary>
    /// 初期化. TODO : ユニット構成情報渡す.
    /// </summary>
    public void Init(ListItem_BattleUnitAnchor parent, int index, BattleLogic.Parameter parameter)
    {
        m_Pressing = false;
        m_ParentAnchor = parent;
        m_index = index;

        m_UnitParameter = parameter;

        // カーソルオブジェクトの設定
        if (parameter.Position.isPlayer) {
            TargetGO = GetScript<Transform> ("Target/PlayerCursor").gameObject;
            GetScript<Transform> ("Target/EnemyCursor").gameObject.SetActive (false);
        } else {
            TargetGO = GetScript<Transform> ("Target/EnemyCursor").gameObject;
            GetScript<Transform> ("Target/PlayerCursor").gameObject.SetActive (false);
        }
        TargetGO.SetActive (false);

        // ボタン.
        // TODO: サイズと位置をマスターから設定できるようにする。
        this.GetScript<uGUIButtonBehaviour>("BattleAnchor").AddLongPressHandler(DidLongTapUnit);
        this.GetScript<uGUIButtonBehaviour>("BattleAnchor").AddReleaseHandler(DidReleaseUnitButton);
        if(IsPlayer) {
            this.SetButtonMsg ("BattleAnchor", DidTapAlly);
        } else {
            this.SetButtonMsg ("BattleAnchor", DidTapEnemy);
        }

        // elementIconの設定
        this.GetScript<SpriteRenderer>("ElementIcon").sprite = IconLoader.LoadElementIcon(m_UnitParameter.Element);

        // TODO: AttackPosAnchorは非攻撃時に敵が立つ位置。マスターから設定できるようにする。
        if (!IsPlayer) {
            var pos = AttackPosAnchor.localPosition;
            pos.x *= -1.0f;
            AttackPosAnchor.localPosition = pos;
            EffectAnchor.transform.localScale = new Vector3 (-1.0f, 1.0f, 1.0f);
        }
        UpdateInfoGauge ();
    }

    /// <summary>
    /// ユニットのリソースデータをこのオブジェクトに結びつける
    /// </summary>
    /// <param name="unitResource">Unit resource.</param>
    public void FetchResource(UnitResourceLoader unitResource, WeaponResourceLoader weaponResource, bool isRegistStateChangeStream=true)
    {
        m_TimeLineIcon = unitResource.TimeLineIcon;
        m_StandingImage = null;
        if (unitResource.Live2DModel != null) {
            m_StandingImage = GameObject.Instantiate (unitResource.Live2DModel);
            m_StandingImage.SetActive (false);
        }

        var modelRoot = this.GetScript<Transform> ("Model").gameObject;
        var go = GameObject.Instantiate (unitResource.SpineModel) as GameObject;
        modelRoot.AddInChild (go);
        go.layer = this.gameObject.layer;

        m_ModelController = go.GetOrAddComponent<SpineModelController> ();
        m_ModelController.Init (
            m_UnitParameter.Weapon != null ? new int?(m_UnitParameter.Weapon.weapon.motion_type) : new int?(99),
            unitResource, weaponResource != null ? weaponResource.WeaponAtlas : null,
            IsPlayer
        );

        // ソーティンレイヤーなどの設定
        var renderer = go.GetComponent<Renderer> ();
        if (renderer != null) {
            renderer.sortingLayerName = "BattleCharacter";
            renderer.sortingOrder = 15;
        }

        // ターゲットのcolliderのサイズを変更する。
        var colliderSize = ButtonBehaviour.GetColliderSize ();
        var bounds = m_ModelController.GetBounds();
        colliderSize.x = (bounds.size.x > colliderSize.x)? bounds.size.x : colliderSize.x;
        colliderSize.y = (bounds.size.y > colliderSize.y)? bounds.size.y : colliderSize.y;
        ButtonBehaviour.SetColliderSize (colliderSize);

        // Hpバーなどの表示位置をユニット被らない位置に修正する。
        var infoObj = GetScript<Transform>("Info");
        var infoObjLocalPosition = infoObj.localPosition;
        float pos_y = bounds.size.y;
        if (Parameter.IsBoss) {
            pos_y = pos_y / 2.0f;
        }
        infoObjLocalPosition.y = (bounds.size.y > infoObjLocalPosition.y) ? pos_y : infoObjLocalPosition.y;
        infoObj.localPosition = infoObjLocalPosition;

        // Animatorのステート遷移モジュールを設定.
        if (isRegistStateChangeStream) {
            RegistStateChangeStream ();

            // 復帰時処理用
            if (Parameter.Conditions.HasCondition) {
                UpdateCondition ();
            }
        }
    }

    public float GetMoveTime(string clipName)
    {
        float ret;

        if (m_ModelController.MoveTimes.TryGetValue (clipName, out ret)) {
            return ret;
        }

        return -1.0f;
    }

    public void RegistStateChangeStream()
    {
        if (m_stateModule != null) {
            Destroy (m_stateModule);
        }
        m_stateModule = this.gameObject.AddComponent<BattleUnitMotionModule> ();
        m_stateModule.Init (this, CallbackDead);
        m_stateModule.RegistStateChangeStream ();
    }

    // Hpゲージの更新
    private void UpdateInfoGauge()
    {
        if (Parameter.IsBoss) {
            DisplayInfo (false);
            // ボスゲージの更新をする
            BattleProgressManager.Shared.UpdateBossInfo(Parameter);
            return;
        }
        var HpGauge = this.GetScript<SpriteRenderer> ("HPGauge");
        var HPGaugeWarning = this.GetScript<SpriteRenderer> ("HPGaugeWarning");
        var HPGaugeDying = this.GetScript<SpriteRenderer> ("HPGaugeDying");
        float hpProgress = m_UnitParameter.HpProgress;
        if (hpProgress > 0.5f) {
            HpGauge.gameObject.SetActive (true);
            HPGaugeWarning.gameObject.SetActive (false);
            HPGaugeDying.gameObject.SetActive (false);
            HpGauge.transform.localScale = new Vector3 (hpProgress, 1.0f, 1.0f);
        } else if (hpProgress > 0.2f) {
            HpGauge.gameObject.SetActive (false);
            HPGaugeWarning.gameObject.SetActive (true);
            HPGaugeDying.gameObject.SetActive (false);
            HPGaugeWarning.transform.localScale = new Vector3 (hpProgress, 1.0f, 1.0f);
        } else {
            HpGauge.gameObject.SetActive (false);
            HPGaugeWarning.gameObject.SetActive (false);
            HPGaugeDying.gameObject.SetActive (true);
            HPGaugeDying.transform.localScale = new Vector3 (hpProgress, 1.0f, 1.0f);
        }

        if (m_UnitParameter.SpecialSkill != null) {
            this.GetScript<Transform> ("SPGauge").gameObject.SetActive(true);
            this.GetScript<Transform> ("SPBase").gameObject.SetActive(true);
            var SpGauge = this.GetScript<SpriteRenderer> ("SPGauge");
            SpGauge.transform.localScale = new Vector3 (m_UnitParameter.SpProgress, 1.0f, 1.0f);
        } else {
            this.GetScript<Transform> ("SPGauge").gameObject.SetActive(false);
            this.GetScript<Transform> ("SPBase").gameObject.SetActive(false);
        }
    }

    public void UpdateCondition()
    {
        // 弱りモーション設定
        m_stateModule.SetDying (Parameter.Conditions.HasDebuffCondition);
        if (Parameter.IsBoss) {
            // ボスゲージの更新をする
            BattleProgressManager.Shared.UpdateBossCondition(Parameter);
            return;
        }
        var ConditionIconRoot = this.GetScript<Transform> ("HP/Condition");

        ConditionIconRoot.gameObject.SetActive (Parameter.Conditions.HasCondition);

        if (Parameter.Conditions.HasCondition) {
            int conditionCount = Parameter.Conditions.Count;
            int spriteCount = 0;
            for (spriteCount = 1; spriteCount <= 4 && spriteCount <= conditionCount ; ) {
                var condition = Parameter.Conditions [spriteCount - 1];
                if (!condition.IsEnable) {
                    continue;
                }
                var ConditionIcon = this.GetScript<SpriteRenderer> (string.Format("Condition{0}", spriteCount));
                ConditionIcon.gameObject.SetActive (true);
                ConditionIcon.sprite = IconLoader.LoadConditionIcon (condition.ConditionData);
                ++spriteCount;
            }

            for (; spriteCount <= 4; ++spriteCount) {
                var ConditionIcon = this.GetScript<SpriteRenderer> (string.Format("Condition{0}", spriteCount));
                ConditionIcon.gameObject.SetActive (false);
            }

            // 状態異常が5つ以上かかっている場合は[…]をだす。
            if (conditionCount > 4) {
                var ConditionMore = this.GetScript<Transform> ("ConditionMore");
                ConditionMore.gameObject.SetActive (false);
            }
        }
    }

    // コールバック : 死亡処理.
    private void CallbackDead()
    {
        // TODO: パッシブスキル効果の削除処理
        Calculation.RemovePassiveSkill(Parameter);

        // 控えを確認していたら出す
        BattleProgressManager.Shared.SupportProc(m_ParentAnchor);

        foreach(var unit in AwsModule.BattleData.SallyAllyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
        }
        foreach(var unit in AwsModule.BattleData.SallyEnemyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
        }

        m_bDead = true;
        if (IsPlayer) {
            BattleProgressManager.Shared.DeadAllyUnit ();
        } else {
            BattleProgressManager.Shared.DeadEnemyUnit (this);
        }

        if (m_Pressing) {
            m_Pressing = false;
            if (BattleProgressManager.Shared.IsInit) {
                BattleProgressManager.Shared.DrawUnitDetail (this, false);
            }
        }
    }

    public void ResetDead()
    {
        m_bDead = false;
    }

    /// <summary>
    /// ターゲット設定.
    /// </summary>
    public void SetTarget(ListItem_BattleUnit target)
    {
        if (target == null) {
            return;
        }

        m_target = target;
        // 相性は攻撃対象が決まらないと表示できないのでここで切り替える
        if(IsPlayer){
            var affinity = Calculation.GetElementAffinityEnum(Parameter, m_target.Parameter);
            this.GetScript<SpriteRenderer>("StrongSymbol").gameObject.SetActive(affinity == ElementAffinityEnum.advantage);
            this.GetScript<SpriteRenderer>("WeakSymbol").gameObject.SetActive(affinity == ElementAffinityEnum.disadvantage);
        }
    }

    /// <summary>
    /// 味方側ターゲット設定
    /// </summary>
    /// <param name="target">Target.</param>
    public void SetAllyTarget(ListItem_BattleUnit target)
    {
        if (target == null) {
            return;
        }

        m_allyTarget = target;
    }

    /// <summary>
    /// 攻撃処理.
    /// </summary>
    public IEnumerator ActionProc(ActionResult actionRes)
    {
        if(this.IsDead) {
            yield break;
        }
        if(this.Target== null) {
            Debug.LogError("[ListItem_BattleUnitAlly] AttackProc Error!! : Target not found.");
            yield break;
        }

        m_actionResult = actionRes;

        // 効果範囲BattleUnitを取得
        var targetPositions = new HashSet<PositionData> (
            m_actionResult.skillResult.GetReceivers().Select(x => x.Position)
        );
        var excludes = new List<ListItem_BattleUnit> ();
        excludes.Add (this);
        excludes.AddRange(targetPositions.Select (x => AwsModule.BattleData.GetBattleUnit(x)));

        // HPゲージを消す
        DisplayInfo(false);
        // 自信と効果範囲BattleUnit以外をGrayに変更
        ChangeGrayUnitColor (excludes);

        // ステート側に行動開始を伝える.
        m_stateModule.StartActionProcess (m_actionResult, CallbackEnterAction);
        yield return new WaitUntil (() => (!m_stateModule.IsAction && !m_IsEnterAction) || IsDead);

        m_stateModule.AttackCameraReset ();

        // カウンターダメージ演出
        if (actionRes.skillResult.GetResults ().Cast<DamageSkillEffectResult> ().Any (x => x.DamageInfos.Any (info => info.counterDamage > 0))) {
            m_stateModule.CounterDamageProcess (actionRes);
            yield return new WaitUntil (() => !m_stateModule.IsDamaging);
        }

        // 行動者は何かしら変化があるはずなので更新
        UpdateInfoGauge ();
        UpdateCondition ();

        // HPゲージを出す
        DisplayInfo(true);


        // 全てWhileに変更
        ChangeWhiteUnitColor ();
    }

    public IEnumerator ConditionEffectProc(BattleLogic.ActionResult actionRes)
    {
        m_actionResult = actionRes;

        var excludes = new List<ListItem_BattleUnit> ();
        excludes.Add (this);
        // 自信と効果範囲BattleUnit以外をGrayに変更
        ChangeGrayUnitColor (excludes);

        // 自分に寄せる
        m_stateModule.StartConditionEffectProcess (m_actionResult, CallbackEnterAction);
        yield return null;

        yield return new WaitUntil (() => IsDead || (!m_stateModule.IsAction && !m_IsEnterAction));
        m_stateModule.AttackCameraReset ();

        // 全てWhileに変更
        ChangeWhiteUnitColor ();
    }

    /// <summary>
    /// 攻撃処理.
    /// </summary>
    public IEnumerator AutoActionProc(List<ActionResult> actionRess)
    {
        if(this.IsDead) {
            yield break;
        }

        foreach (var actionRes in actionRess) {
            m_actionResult = actionRes;

            // 効果範囲BattleUnitを取得
            var targetPositions = new HashSet<PositionData> (
                                  m_actionResult.skillResult.GetReceivers ().Select (x => x.Position)
                              );
            var excludes = new List<ListItem_BattleUnit> ();
            excludes.Add (this);
            excludes.AddRange (targetPositions.Select (x => AwsModule.BattleData.GetBattleUnit (x)));

            // HPゲージを消す
            var hpBar = GetScript<Transform> ("Info/HP");
            hpBar.gameObject.SetActive (false);
            // 自信と効果範囲BattleUnit以外をGrayに変更
            ChangeGrayUnitColor (excludes);

            // ステート側に行動開始を伝える.
            m_stateModule.StartAutoActionProcess(m_actionResult, CallbackEnterAction);
            yield return null;

            yield return new WaitUntil (() => (!m_stateModule.IsAction && !m_IsEnterAction) || IsDead);

            // HPゲージを出す
            hpBar.gameObject.SetActive (true);
            // 全てWhileに変更
            ChangeWhiteUnitColor ();
        }
    }

    public void SupportPerformanceProc(Action endCallback)
    {
        m_stateModule.SupportPerformanceProc(
            () => {
                if(endCallback != null) {
                    endCallback();
                }
            }
        );
    }

    #region Set Material Color
    /// <summary>
    /// カラー設定.
    /// </summary>
    public void SetColor(Color color)
    {
        if (skeletonRenderer == null) {
            skeletonRenderer = GetComponentInChildren<Spine.Unity.SkeletonRenderer> (true);
        }

        if (skeletonRenderer != null) {
            skeletonRenderer.skeleton.SetColor (color);
        }
    }

    public void DisplayInfo(bool isActive) {
        if (Parameter.IsBoss && isActive) {
            return;
        }
        var infoObj = GetScript<Transform>("Info");
        infoObj.gameObject.SetActive(isActive);
    }

    private Spine.Unity.SkeletonRenderer skeletonRenderer;
    #endregion

    // TODO : ListItem単位でやらずにView_BattleFieldやMainあたりから操作したい.
    private static void ChangeUnitColor(IEnumerable<ListItem_BattleUnit> excludes, Color color)
    {
        // 攻撃者と対象者以外の色を落とす
        int maxCount = AwsModule.BattleData.EnemyList.Count;
        for (int i = 0; i < maxCount; ++i) {
            var battleUnit = AwsModule.BattleData.EnemyList [i];
            if (excludes == null || !excludes.Contains(battleUnit)) {
                battleUnit.SetColor (color);
            }
        }
        maxCount = AwsModule.BattleData.AllyList.Count;
        for (int i = 0; i < maxCount; ++i) {
            var battleUnit = AwsModule.BattleData.AllyList [i];
            if (excludes == null || !excludes.Contains(battleUnit)) {
                battleUnit.SetColor (color);
            }
        }
    }

    private static void ChangeGrayUnitColor(IEnumerable<ListItem_BattleUnit> excludes)
    {
        ChangeUnitColor (excludes, Color.gray);
    }

    private static void ChangeWhiteUnitColor()
    {
        ChangeUnitColor (null, Color.white);
    }

    #region internal AttackProcess.

    // コールバック : 攻撃プロセス
    void CallbackEnterAction()
    {
        // すでにアクション中なら
        if (m_IsEnterAction)
            return;
        
        m_IsEnterAction = true;
        StartCoroutine (EnterAction());
    }

    bool m_IsEnterAction = false;
    private IEnumerator EnterAction()
    {
        //yield return new WaitForSeconds (0.15f);

        int allEffectCount = 0;
        int playEndAllEffectCount = 0;
        List<IEnumerator> exeCoroutines = new List<IEnumerator> ();
        var skillResultRecivers = m_actionResult.skillResult.GetReceivers ().Select(x => x.Position).ToArray ();
        if (m_actionResult.skillResult.HasEffectResult(SkillEffectLogicEnum.damage, SkillEffectLogicEnum.special_damage)) {
            SkillEffectLogicEnum[] logicEnums = new SkillEffectLogicEnum[2] {
                SkillEffectLogicEnum.damage,
                SkillEffectLogicEnum.special_damage
            };
            // 全体ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Damage", logicEnums, SkillPerformanceTargetEnum.EnemyAll, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    // 中心位置を取得
                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_target.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.EnemyAll);
                    if(isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // 行指定ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Damage", logicEnums, SkillPerformanceTargetEnum.EnemyRow, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    // 横位置を取得
                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_target.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.EnemyRow);
                    if(isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // 列指定ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Damage", logicEnums, SkillPerformanceTargetEnum.EnemyColumn, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_target.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.EnemyColumn);
                    if(isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // 列指定ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Damage", logicEnums, SkillPerformanceTargetEnum.CenterTarget, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_target.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.CenterTarget);
                    if(isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // ダメージ情報を渡してターゲットに対して攻撃, 演出待ち.
            foreach (var result in m_actionResult.skillResult.GetResults (SkillEffectLogicEnum.damage, SkillEffectLogicEnum.special_damage)) {
                ListItem_BattleUnit target = AwsModule.BattleData.GetBattleUnit (result.Key.Position);
                if (target != null) {
                    exeCoroutines.Add (target.DamageProc (m_actionResult, result.Value));
                }
            }
                
            var effectResults = m_actionResult.skillResult.GetEffectResults (SkillEffectLogicEnum.damage, SkillEffectLogicEnum.special_damage);
            var damageEffectResults = effectResults.Cast<DamageSkillEffectResult> ();
            if (damageEffectResults.Count () > 0) {
                // 吸収時の回復表示
                var healValue = damageEffectResults.Where(x => x.DamageInfos != null && x.DamageInfos.Length > 0).Sum(x => x.DamageInfos.Sum(info => info.heal));
                if (healValue > 0) {
                    exeCoroutines.Add (AbsorptionProc (m_actionResult, healValue));
                }
                // 入れ替えを行う
                if(damageEffectResults.Where(x => x.DamageInfos != null && x.DamageInfos.Length > 0).Any(x => x.DamageInfos.Any(info => info.addWeight > 0))) {
                    BattleProgressManager.Shared.SortActionOrder ();
                }
            }



        }
        if(m_actionResult.skillResult.HasEffectResult (SkillEffectLogicEnum.hp_heal, SkillEffectLogicEnum.sp_charge)) {
            SkillEffectLogicEnum[] logicEnums = new SkillEffectLogicEnum[2] {
                SkillEffectLogicEnum.hp_heal,
                SkillEffectLogicEnum.sp_charge
            };
            // 回復全体エフェクト再生
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Heal", logicEnums, SkillPerformanceTargetEnum.AllyAll, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_allyTarget.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.AllyAll);
                    if(!isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // 行指定ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Heal", logicEnums, SkillPerformanceTargetEnum.AllyRow, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_allyTarget.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.AllyRow);
                    if(!isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // 列指定ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Heal", logicEnums, SkillPerformanceTargetEnum.AllyColumn, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_allyTarget.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.AllyColumn);
                    if(!isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            // 列指定ダメージエフェクト
            BattleUnitMotionModule.PlayPerformance(m_actionResult, "Heal", logicEnums, SkillPerformanceTargetEnum.CenterTarget, null, null,
                (go) => {
                    if (go == null) {
                        return;
                    }
                    allEffectCount++;

                    bool isPlayerSide = IsPlayer;
                    if(Parameter.BattleAI.TargetReversal) {
                        isPlayerSide = !isPlayerSide;
                    }
                    go.transform.localPosition = BattleProgressManager.Shared.GetFieldPosition(isPlayerSide, m_allyTarget.Parameter.Position, skillResultRecivers, SkillPerformanceTargetEnum.CenterTarget);
                    if(!isPlayerSide) {
                        go.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                },
                (go) => {
                    playEndAllEffectCount++;
                }
            );

            var isUpdateCondition = false;
            if (m_actionResult.skillResult.HasEffectResult (SkillEffectLogicEnum.condition_remove, SkillEffectLogicEnum.condition_granting)) {
                isUpdateCondition = true;
            }
            // ダメージ情報を渡してターゲットに対して攻撃, 演出待ち.
            foreach (var results in m_actionResult.skillResult.GetResults(SkillEffectLogicEnum.hp_heal, SkillEffectLogicEnum.sp_charge)) {
                ListItem_BattleUnit target = AwsModule.BattleData.GetBattleUnit (results.Key.Position);
                if (target != null) {
                    exeCoroutines.Add (target.HealProc (m_actionResult, results.Value));
                    if (isUpdateCondition) {
                        target.UpdateCondition ();
                    }
                }
            }
        }

        if(m_actionResult.skillResult.HasEffectResult (SkillEffectLogicEnum.condition_granting)) {
            foreach (var results in m_actionResult.skillResult.GetResults(SkillEffectLogicEnum.condition_granting)) {
                ListItem_BattleUnit target = AwsModule.BattleData.GetBattleUnit (results.Key.Position);
                if (target != null) {
                    exeCoroutines.Add (target.ConditionProc (m_actionResult, results.Value));
                    if (results.Value.Any (x => x.IsSuccess)) {
                        target.UpdateCondition ();
                    }
                }
            }
        }

        // 状態removeだけ
        if (!m_actionResult.skillResult.HasEffectResult (SkillEffectLogicEnum.hp_heal, SkillEffectLogicEnum.sp_charge) &&
            m_actionResult.skillResult.HasEffectResult (SkillEffectLogicEnum.condition_remove)) {
            foreach (var results in m_actionResult.skillResult.GetResults(SkillEffectLogicEnum.condition_remove)) {
                ListItem_BattleUnit target = AwsModule.BattleData.GetBattleUnit (results.Key.Position);
                if (target != null) {
                    exeCoroutines.Add (target.ConditionProc (m_actionResult, results.Value));
                    if (results.Value.Any (x => x.IsSuccess)) {
                        target.UpdateCondition ();
                    }
                }
            }
        }

        if (exeCoroutines.Count > 0) {
            yield return ParallelCorotines.DoCoroutines (this, exeCoroutines);
        }

        while (playEndAllEffectCount < allEffectCount) {
            yield return null;
        }
        m_IsEnterAction = false;
    }

    /// <summary>
    /// ダメージ処理.
    /// </summary>
    public IEnumerator DamageProc(ActionResult actionResult, IEnumerable<SkillEffectResultBase> skillEffectResults)
    {
        if(this.IsDead) {
            yield break;
        }
            
        UpdateInfoGauge ();
        UpdateCondition ();

        // モーション
        m_stateModule.DamageProcess (actionResult, skillEffectResults);

        yield return new WaitUntil (() => !m_stateModule.IsDamaging);
    }

    /// <summary>
    /// 吸収処理.
    /// </summary>
    public IEnumerator AbsorptionProc(ActionResult actionResult, int healValue)
    {
        if(this.IsDead) {
            yield break;
        }

        UpdateInfoGauge ();
        UpdateCondition ();

        BattleEffectManager.Instance.CreateHealLabel (healValue, 0,  this.gameObject, InfoGameObject.transform.position);

        yield return new WaitUntil (() => BattleEffectManager.Instance.IsNextAllWithTag("Heal"));
    }

    /// <summary>
    /// 回復処理.
    /// </summary>
    public IEnumerator HealProc(BattleLogic.ActionResult actionResult, IEnumerable<SkillEffectResultBase> skillEffectResults)
    {
        if(this.IsDead) {
            yield break;
        }

        var healValues = skillEffectResults.Cast<HealSkillEffectResult> ().Select (x => x.HealValue);
        foreach (var healValue in healValues) {
            BattleEffectManager.Instance.CreateHealLabel (healValue, 0, this.gameObject, InfoGameObject.transform.position);
        }

        UpdateInfoGauge ();
        UpdateCondition ();
        // モーション
        m_stateModule.HealProcess(actionResult, skillEffectResults.Select(x => x.LogicEnum).Distinct().ToArray());

        yield return new WaitUntil (() => BattleEffectManager.Instance.IsNextAllWithTag("Heal") && BattleEffectManager.Instance.IsNextAllWithTag("Condition"));
    }

    /// <summary>
    /// 状態付与処理.
    /// </summary>
    public IEnumerator ConditionProc(BattleLogic.ActionResult actionResult, IEnumerable<SkillEffectResultBase> skillEffectResults)
    {
        if(this.IsDead) {
            yield break;
        }

        UpdateInfoGauge ();
        UpdateCondition ();
        // モーション
        m_stateModule.ConditionProcess(actionResult, skillEffectResults.Select(x => x.LogicEnum).Distinct().ToArray());

        yield return new WaitUntil (() => BattleEffectManager.Instance.IsNextAllWithTag("Condition"));
    }

    public IEnumerator WinProc ()
    {
        if(this.IsDead) {
            yield break;
        }
        m_stateModule.WinProcess ();
        yield return new WaitUntil (() => !m_stateModule.IsAction);
    }
    #endregion

    #region ButtoneDelegate.

    // ボタン: 敵タップ.ターゲット切り替え.
    void DidTapEnemy()
    {
        if(this.IsPlayer){
            return;
        }
        if(this.IsDead){
            return;
        }
		if(BattleProgressManager.Shared != null){
			BattleProgressManager.Shared.RetargetEnemy(this);
		}
    }

    void DidTapAlly()
    {
        if(!this.IsPlayer){
            return;
        }
        if(this.IsDead){
            return;         
        }
		if (BattleProgressManager.Shared.IsInit) {
			BattleProgressManager.Shared.RetargetAlly(this);
		}
    }

    // ボタン: ユニット長押し.
    void DidLongTapUnit(PointerEventData eventData)
    {
        m_Pressing = true;
		if (BattleProgressManager.Shared.IsInit) {
			BattleProgressManager.Shared.DrawUnitDetail(this, true);
		}
    }

    // ボタン: ユニットボタン押下解除.
    void DidReleaseUnitButton(PointerEventData eventData)
    {
        m_Pressing = false;
		if (BattleProgressManager.Shared.IsInit) {
			BattleProgressManager.Shared.DrawUnitDetail(this, false);
		}
    }

    #endregion

    private BattleUnitMotionModule m_stateModule;
    private BattleLogic.ActionResult m_actionResult;

    #region animation clip events.

    void TrailEffectOn(string name)
    {
        if(trailRenderers == null) {
            trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
        }
        int max = trailRenderers.Length;
        for(int i = 0; i < max; i++) {
            if(string.IsNullOrEmpty(name) || name == trailRenderers[i].name) {
                trailRenderers[i].enabled = true;
            }
        }
    }

    void TrailEffectOff(string name)
    {
        if(trailRenderers == null) {
            return;
        }
        int max = trailRenderers.Length;
        for(int i = 0; i < max; i++) {
            if(string.IsNullOrEmpty(name) || name == trailRenderers[i].name) {
                trailRenderers[i].enabled = false;
            }
        }
    }

    private TrailRenderer[] trailRenderers = null;

    void PlaySE(string se)
    {
        SoundManager.SharedInstance.PlaySE((SoundClipName)Enum.Parse(typeof(SoundClipName), se));
    }
    #endregion

    public override string ToString ()
    {
        return string.Format ("[ListItem_BattleUnit: Name={0}, IsPlayer={1}, Index={2}, ItemType={3}]", Parameter.Name, IsPlayer, Index, ItemType);
    }


    public void SetParentAnchor(ListItem_BattleUnitAnchor anchor)
    {
        m_ParentAnchor = anchor;
    }

    public void ActiveObject()
    {
        InstanceObject.SetActive (true);
        RegistStateChangeStream ();
        m_stateModule.PlayIdel ();
        SetColor (Color.white);

        DisplayInfo (true);
        UpdateInfoGauge ();
    }
}
