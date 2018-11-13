using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using SmileLab;

using BattleLogic;

/// <summary>
/// バトルユニットのモーション操作クラス.
/// </summary>
public class BattleUnitMotionModule : ViewBase
{
    /// <summary>カメラのズームタイプ.</summary>
    public CameraType CameraMode { get; set; }
    // enum : カメラタイプ.
    public enum CameraType
    {
        Zoom,
        Switch,
        None
    }

    /// <summary>
    /// 攻撃中？
    /// </summary>
    public bool IsAction { get; private set; }

    /// <summary>
    /// ダメージ中？
    /// </summary>
    public bool IsDamaging {
        get {
            return m_DamageEffectPlayingCount > 0 || m_DamageMotionPlaying;
        }
    }


    public static bool PlayPerformance(ActionResult result, string key, SkillEffectLogicEnum[] logics, SkillPerformanceTargetEnum target,
        ListItem_BattleUnit unit = null, UnityEngine.GameObject parent = null,
        System.Action<UnityEngine.GameObject> didCreate = null, System.Action<UnityEngine.GameObject> didDesrtoy = null)
    {
        if (result.action != null) {
            var skill = result.action.Skill;
            int motionType = result.invoker != null ? result.invoker.MotionType : -1;
            if (skill.HasPerformanceKey (key, motionType, target)) {
                skill.PlayPerformance (key, motionType, logics, target, unit, parent, didCreate, didDesrtoy);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(ListItem_BattleUnit myInfo, Action didDeadProc = null)
    {
        m_myInfo = myInfo;
        m_didDead = didDeadProc;

        var animList = this.gameObject.GetComponentsInChildren<Animator>();
        if(animList == null || animList.Length <= 0) {
            Debug.LogError("[BattleUnitStateChangeReceiver] Awake Error!! : Animator is null or empty or not active all.");
        }
        // Animatorが複数ついていることは無い想定.
        m_animator = animList[0];
        m_stateObserver = m_animator.GetBehaviour<StateMachineObservables>();
        if(m_stateObserver == null){
            Debug.LogError("[BattleUnitStateChangeReceiver] Init Error!! : "+m_animator.gameObject.name+"にStateMachineObservablesがアタッチされていない.");
        }
        m_animator.gameObject.GetOrAddComponent<BattleUnitClipEvents> ().Init(this);
    }

    public void PlayIdel()
    {
        m_animator.Play ("Idel");
    }
    /// <summary>
    /// 攻撃処理開始.
    /// </summary>
    public void StartActionProcess(ActionResult actionResult, Action didAction = null)
    {
        this.IsAction = true;
        m_didAction = didAction;

        m_basePos = this.gameObject.transform.position;

        CameraMode = SelectCameraMode (actionResult);

        var targetBattleUnits = actionResult.skillResult.GetReceivers().
            Select (x => AwsModule.BattleData.GetBattleUnit (x.Position)).
            Where (x => x != null);
        // カメラの焦点位置の計算
        var targetPositions = targetBattleUnits.Select(x => x.Anchor.position);
        float pos_count = targetPositions.Count ();
        float pos_x = targetPositions.Select (pos => pos.x).Sum () / pos_count;
        float pos_y = targetPositions.Select (pos => pos.y).Sum () / pos_count;
        float pos_z = targetPositions.Select (pos => pos.z).Sum () / pos_count;
        m_cameraTargetPosition = new Vector3(pos_x, pos_y, pos_z);

        m_attackMovePosistion = Vector3.zero;
        var attackMovePositions = targetBattleUnits.Where(x => x.IsPlayer != m_myInfo.IsPlayer).
            Select(x => x.AttackPosAnchor.position);
        if(attackMovePositions.Count() > 0) {
            pos_count = targetPositions.Count ();
            // xは陣営に一番近いもの
            pos_x = !m_myInfo.IsPlayer ? attackMovePositions.Select(pos => pos.x).Min () : attackMovePositions.Select(pos => pos.x).Max ();
            // yは中心
            pos_y = attackMovePositions.Select(pos => pos.y).Sum() / pos_count;
            // zは一番小さいの
            pos_z = attackMovePositions.Select(pos => pos.z).Min ();
            m_attackMovePosistion = new Vector3(pos_x, pos_y, pos_z);
        }

        // 発動時スキル演出再生
        m_actionResult = actionResult;
        var weaponMotion = MasterDataTable.weapon_motion_type.DataList.FirstOrDefault (x => actionResult.invoker.Weapon != null && x.index == actionResult.invoker.Weapon.weapon.motion_type);

        this.StartCoroutine(ActionRoutine(actionResult.action.Skill.play_animator_trigger,
            actionResult.action.Skill.attack_pattern_index,
            actionResult.action.Skill.attack_range == AttackRangeEnum.LongRange ||
            (
                actionResult.action.Skill.attack_range != AttackRangeEnum.CrossRange &&
                weaponMotion != null && 
                weaponMotion.attack_range == AttackRangeEnum.LongRange
            ), m_myInfo.Parameter.BattleAI.TargetReversal // ターゲット逆転が起こっている場合は移動処理を飛ばす
        ));
    }
    private CameraType SelectCameraMode(ActionResult actionResult)
    {
        // 必殺で全体範囲の場合はカメラ演出はなし
        if(actionResult.action.IsSpecial && actionResult.action.Skill.SkillEffects.Any(x => x.range.IsAll)) {
            return CameraType.None;
        }
        // 通常攻撃なら
        if (actionResult.action.IsNormalAction) {
            if (m_myInfo.Parameter.Weapon == null) {
                return CameraType.Zoom;
            } else {
                var weaponMotion = MasterDataTable.weapon_motion_type.DataList.FirstOrDefault (x => x.index == m_myInfo.Parameter.Weapon.weapon.motion_type);
                // 近距離ならZoom
                if (weaponMotion != null && weaponMotion.attack_range == AttackRangeEnum.CrossRange) {
                    return CameraType.Zoom;
                }
                // 遠距離ならSwitching
                if (weaponMotion != null && weaponMotion.attack_range == AttackRangeEnum.LongRange) {
                    return CameraType.Switch;
                }
            }
        }
        // 範囲行動ならZoom
        if (actionResult.action.Skill.SkillEffects.Any (x => x.range.IsRange)) {
            return CameraType.Zoom;
        }
        // 単体行動ならSwitching
        return CameraType.Switch;
    }
    private IEnumerator ActionRoutine(string actionTrigger, int attackPattern, bool longRange, bool nonMoveAttack)
    {
        // スイッチングカメラならフォーカス.
        if(this.CameraMode == CameraType.Switch) {
            CameraHelper.SharedInstance.ForcusTarget(m_myInfo.Anchor.position);
            CameraHelper.SharedInstance.SetOrthographicSize(15.0f);
            // TODO : 目分量でとりあえず待ち.この時間がスイッチングカメラの際はフォーカス時間になる.
            yield return new WaitForSeconds(0.5f);
        }

        m_animator.SetInteger ("AttackPattern", attackPattern);
        m_animator.SetBool("LongRangeAttack", longRange);
        m_animator.SetBool("NonMoveAttack", nonMoveAttack);
        if (actionTrigger == "Skill") {
            // 発動エフェクト発生
            PlayPerformance (m_actionResult, "Invoke", null, SkillPerformanceTargetEnum.Invoker, m_myInfo, m_myInfo.EffectAnchor.gameObject);
        }

        // 進行不能対策
        if (string.IsNullOrEmpty (actionTrigger)) {
            actionTrigger = "Attack";
        }

        m_animator.SetTrigger(actionTrigger);
    }

    ActionResult m_actionResult;
    public void PlayAttackEffect()
    {
        if (m_actionResult == null)
            return;
        PlayPerformance (m_actionResult, "Damage", null, SkillPerformanceTargetEnum.Invoker, m_myInfo, m_myInfo.EffectAnchor.gameObject);
    }

    public void PlayAttackSe()
    {
        if (m_actionResult == null)
            return;

        if (m_myInfo.Parameter.Weapon == null) {
            return;
        }

        SoundClipName? clipName = null;
        switch (m_myInfo.Parameter.Weapon.weapon.motion_type) {
        case 1: // 
            clipName = SoundClipName.se106;
            break;
        case 2: //
            clipName = SoundClipName.se107;
            break;
        case 3: //
            clipName = SoundClipName.se108;
            break;
        case 4: //
            clipName = SoundClipName.se109;
            break;
        case 5: //
            clipName = SoundClipName.se110;
            break;
        case 6: //
            clipName = SoundClipName.se111;
            break;
        case 7: //
            clipName = SoundClipName.se112;
            break;
        case 8: //
            clipName = SoundClipName.se113;
            break;
        case 9: //
            clipName = SoundClipName.se114;
            break;
        case 10: //
            clipName = SoundClipName.se115;
            break;
        case 11: //
            clipName = SoundClipName.se116;
            break;
        case 12: //
            clipName = SoundClipName.se106;
            break;
        default:
            break;
        }

        if (clipName.HasValue) {
            SoundManager.SharedInstance.PlaySE (clipName.Value);
        }
    }

    public void StartConditionEffectProcess(ActionResult actionResult, Action didAction = null)
    {
        this.IsAction = true;

        // 状態異常系のカメラはSwitch固定
        CameraMode = CameraType.Switch;

        m_basePos = this.gameObject.transform.position;

        var targetBattleUnits = actionResult.skillResult.GetReceivers().
            Select (x => AwsModule.BattleData.GetBattleUnit (x.Position)).
            Where (x => x != null);
        // カメラの焦点位置の計算
        var targetPositions = targetBattleUnits.Select(x => x.Anchor.position);
        float pos_count = targetPositions.Count ();
        float pos_x = targetPositions.Select (pos => pos.x).Sum () / pos_count;
        float pos_y = targetPositions.Select (pos => pos.y).Sum () / pos_count;
        float pos_z = targetPositions.Select (pos => pos.z).Sum () / pos_count;
        m_cameraTargetPosition = new Vector3(pos_x, pos_y, pos_z);

        m_attackMovePosistion = Vector3.zero;
        var attackMovePositions = targetBattleUnits.Where(x => x.IsPlayer != m_myInfo.IsPlayer).
            Select(x => x.AttackPosAnchor.position);
        if(attackMovePositions.Count() > 0) {
            pos_count = targetPositions.Count ();
            // xは陣営に一番近いもの
            pos_x = m_myInfo.IsPlayer ? attackMovePositions.Select(pos => pos.x).Min () : attackMovePositions.Select(pos => pos.x).Max ();
            // yは中心
            pos_y = attackMovePositions.Select(pos => pos.y).Sum() / pos_count;
            // zは一番小さいの
            pos_z = attackMovePositions.Select(pos => pos.z).Min ();
            m_attackMovePosistion = new Vector3(pos_x, pos_y, pos_z);
        }
        if(this.CameraMode == CameraType.Switch) {
            CameraHelper.SharedInstance.ForcusTarget(m_myInfo.Anchor.position);
            CameraHelper.SharedInstance.SetOrthographicSize(15.0f);
        }
        AttackCameraTargetSet ();

        didAction ();
        this.IsAction = false;
    }

    public void StartAutoActionProcess(ActionResult actionResult, Action didAction = null)
    {
        this.IsAction = true;

        // 状態異常系のカメラはSwitch固定
        CameraMode = CameraType.None;
        m_basePos = this.gameObject.transform.position;

        var targetBattleUnits = actionResult.skillResult.GetReceivers().
            Select (x => AwsModule.BattleData.GetBattleUnit (x.Position)).
            Where (x => x != null);
        // カメラの焦点位置の計算
        var targetPositions = targetBattleUnits.Select(x => x.Anchor.position);
        float pos_count = targetPositions.Count ();
        float pos_x = targetPositions.Select (pos => pos.x).Sum () / pos_count;
        float pos_y = targetPositions.Select (pos => pos.y).Sum () / pos_count;
        float pos_z = targetPositions.Select (pos => pos.z).Sum () / pos_count;
        m_cameraTargetPosition = new Vector3(pos_x, pos_y, pos_z);

        m_attackMovePosistion = Vector3.zero;
        var attackMovePositions = targetBattleUnits.Where(x => x.IsPlayer != m_myInfo.IsPlayer).
            Select(x => x.AttackPosAnchor.position);
        if(attackMovePositions.Count() > 0) {
            pos_count = targetPositions.Count ();
            // xは陣営に一番近いもの
            pos_x = m_myInfo.IsPlayer ? attackMovePositions.Select(pos => pos.x).Min () : attackMovePositions.Select(pos => pos.x).Max ();
            // yは中心
            pos_y = attackMovePositions.Select(pos => pos.y).Sum() / pos_count;
            // zは一番小さいの
            pos_z = attackMovePositions.Select(pos => pos.z).Min ();
            m_attackMovePosistion = new Vector3(pos_x, pos_y, pos_z);
        }

        didAction ();
        this.IsAction = false;
    }


    public void CounterDamageProcess(ActionResult actionResult)
    {
        m_damageInfos = actionResult.skillResult.GetResults().Cast<DamageSkillEffectResult> ().SelectMany (x => x.DamageInfos).ToArray();
        m_isHit = true;
        m_damageResult = actionResult;
        m_DamageMotionPlaying = true;
        m_DamageEffectPlayingCount = 0;
        m_counterDamage = true;

        if (m_myInfo.Parameter.Hp <= 0) {
            // 死亡演出.
            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.dead);
            m_animator.SetTrigger ("Destroy");
        } else {
            // ダメージ演出.
            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.damage);
            m_animator.SetTrigger ("Damage");
        }
    }

    /// <summary>
    /// ダメージ処理.
    /// </summary>
    public void DamageProcess(ActionResult actionResult, IEnumerable<SkillEffectResultBase> skillEffectResults)
    {
        m_damageInfos = skillEffectResults.Cast<DamageSkillEffectResult> ().SelectMany (x => x.DamageInfos).ToArray();
        m_damageEffectLogics = skillEffectResults.Select (x => x.LogicEnum).Distinct().ToArray();
        m_isHit = m_damageInfos.IsHit();
        m_damageResult = actionResult;
        m_counterDamage = false;

        if (!m_isHit) {
            m_DamageMotionPlaying = false;

            // Miss表示待ち用
            m_DamageEffectPlayingCount = 1;

            // ミスの表示
            BattleEffectManager.Instance.CreateDamageLabel(0, m_isHit, false, false, false, 0, m_myInfo.gameObject, m_myInfo.InfoGameObject.transform.position,
                () => {
                    m_DamageEffectPlayingCount--;
                });
        } else {
            m_DamageMotionPlaying = true;
            m_DamageEffectPlayingCount = 0;

            if (m_myInfo.Parameter.Hp <= 0) {
                // 死亡演出.
                if (!m_myInfo.Parameter.IsBoss) {
                    SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.dead);
                }
                m_animator.SetTrigger ("Destroy");
            } else {
                // ダメージ演出.
                SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.damage);
                m_animator.SetTrigger ("Damage");
            }
        }
    }
        
    public void PlayDamageEffect()
    {
        if (m_damageResult == null)
            return;

        if (m_isHit) {
            if (m_counterDamage) {
                PlayPerformance (m_damageResult, "CounterDamage", m_damageEffectLogics, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject,
                    (go) => {
                        m_DamageEffectPlayingCount++;
                    },
                    (go) => {
                        m_DamageEffectPlayingCount--;
                    }
                );
            } else {
                PlayPerformance (m_damageResult, "Damage", m_damageEffectLogics, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject,
                    (go) => {
                        m_DamageEffectPlayingCount++;
                    },
                    (go) => {
                        m_DamageEffectPlayingCount--;
                    }
                );
                PlayPerformance (m_damageResult, "Condition", m_damageEffectLogics, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject,
                    (go) => {
                        m_DamageEffectPlayingCount++;
                    },
                    (go) => {
                        m_DamageEffectPlayingCount--;
                    }
                );
            }
        }
    }

    public void PlayDamageNumber()
    {
        if (m_damageInfos == null) {
            return;
        }

        // カウンターダメージ表示
        if (m_counterDamage) {
            int damageDispCount = 0;
            for (int i = 0; i < m_damageInfos.Length; ++i) {
                var info = m_damageInfos [i];
                if (info.counterDamage > 0) {
                    BattleEffectManager.Instance.CreateDamageLabel (info.counterDamage, true, false, false,
                        false, damageDispCount++, m_myInfo.gameObject, m_myInfo.InfoGameObject.transform.position);
                }
            }
        } else {
            int damageDispCount = 0;
            for (int i = 0; i < m_damageInfos.Length; ++i) {
                var info = m_damageInfos [i];
                BattleEffectManager.Instance.CreateDamageLabel (info.damage, info.isHit, info.isCritical, info.affinityEnum == ElementAffinityEnum.advantage,
                    info.affinityEnum == ElementAffinityEnum.disadvantage, damageDispCount++, m_myInfo.gameObject, m_myInfo.InfoGameObject.transform.position);

                if (info.reverseDamage > 0) {
                    BattleEffectManager.Instance.CreateHealLabel (info.reverseDamage,
                        damageDispCount++, m_myInfo.gameObject, m_myInfo.InfoGameObject.transform.position);
                }
            }
        }
    }

    public void HealProcess(BattleLogic.ActionResult actinResult, SkillEffectLogicEnum[] logicEnums)
    {
        var actionSkill = actinResult.action.Skill;
        if (actinResult.invoker != m_myInfo.Parameter) {
            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.receive_buff);
        }
        PlayPerformance (actinResult, "Heal", logicEnums, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject);
        PlayPerformance (actinResult, "Condition", logicEnums, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject);
    }

    public void ConditionProcess(BattleLogic.ActionResult actinResult, SkillEffectLogicEnum[] logicEnums)
    {
        var actionSkill = actinResult.action.Skill;
        PlayPerformance (actinResult, "Condition", logicEnums, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject);
    }

    public void PlayHealEffect()
    {
        if (m_damageResult == null)
            return;
        var actionSkill = m_damageResult.action.Skill;
        PlayPerformance (m_damageResult, "Heal", m_damageEffectLogics, SkillPerformanceTargetEnum.Receiver, m_myInfo, m_myInfo.EffectAnchor.gameObject);
    }

    public void PlayMuzzleFlash()
    {
        if (m_actionResult == null)
            return;
        PlayPerformance (m_actionResult, "MuzzleFlash", null, SkillPerformanceTargetEnum.Invoker, m_myInfo, m_myInfo.EffectAnchor.gameObject,
            (go) => {
                var animation = go.GetComponent<Animation>();
                if(animation != null && m_actionResult.invoker != null) {
                    string animationName = string.Format("{0:D2}", m_actionResult.invoker.Element.index);
                    if(animation.GetClip(animationName) != null) {
                        animation.Play(animationName);
                    } else {
                        animation.Play();
                    }
                }
            }
        );
    }

    public void PlayAttackSt(int count)
    {
        if (m_actionResult == null)
            return;
        string playKey = string.Format ("AttackSt{0}", count);
        PlayPerformance (m_actionResult, playKey, null, SkillPerformanceTargetEnum.Invoker, m_myInfo, m_myInfo.EffectAnchor.gameObject,
            (go) => {
                var animation = go.GetComponent<Animation>();
                if(animation != null && m_actionResult.invoker != null) {
                    string animationName = string.Format("{0:D2}", m_actionResult.invoker.Element.index);
                    if(!animation.Play(animationName)) {
                        animation.Play();
                    }
                }
            }
        );
    }

    public void WinProcess()
    {
        IsAction = true;
        m_animator.SetTrigger("Win");
    }

    public void SetDying(bool isDying)
    {
        m_animator.SetBool ("Dying", isDying);
    }

    public void SupportPerformanceProc(Action endCallback)
    {
        m_animator.SetTrigger ("Run");
        m_SupportPreformanceEndCallback = endCallback;

        var fromPos = this.gameObject.transform.position;
        if (m_myInfo.IsPlayer) {
            fromPos.x += 40.0f;
        } else {
            fromPos.x += -40.0f;
        }
        var moveHash = new Hashtable();
        moveHash.Add ("position", fromPos);
        moveHash.Add("time", 1.0f);
        moveHash.Add("easetype", iTween.EaseType.linear);
        moveHash.Add ("oncomplete", "SupportPreformanceEnd");
        moveHash.Add ("oncompletetarget", this.gameObject);
        iTween.MoveFrom(this.gameObject, moveHash);
    }
    private Action m_SupportPreformanceEndCallback;
    private void SupportPreformanceEnd()
    {
        m_animator.SetTrigger ("RunStop");
        if (m_SupportPreformanceEndCallback != null) {
            m_SupportPreformanceEndCallback ();
        }
    }

    /// <summary>
    /// ステート遷移の監視を開始.
    /// </summary>
    public void RegistStateChangeStream()
    {
        // 既に購読中のストリームがあれば無視する.
        if(m_bReadingStream){
            return;
        }
        m_bReadingStream = true;

        // TODO : 購読開始.とりあえず開始と終了.
        m_stateObserver.OnStateEnterObservable
                       .TakeUntilDisable(this)     // 自身が破棄された場合停止.
                       .Subscribe(CallbackStateEnter);

        m_stateObserver.OnStateExitObservable
                       .TakeUntilDisable(this)
                       .Subscribe(CallbackStateExit);

    }

    void OnDisable()
    {
        m_bReadingStream = false;
    }
    private bool m_bReadingStream = false;

    // コールバック : ステート遷移(Enter).
    void CallbackStateEnter(AnimatorStateInfo stateInfo)
    {
        // 移動モーション
        if (stateInfo.IsTag ("Move")) {
            StateEnterMove ();
        }
        else if (stateInfo.IsTag ("Attack")) {
            StateEnterAttack ();
        }
        else if (stateInfo.IsTag ("LongRangeAttack")) {
            StateEnterLongRangeAttack ();
        }
        else if (stateInfo.IsTag ("Skill")) { 
            StateEnterSkill ();
        }
        // 攻撃戻りモーション
        else if (stateInfo.IsTag ("Back")) {
        }
        // 死亡モーション
        else if (stateInfo.IsTag ("Destroy")) {
            StateEnterDead ();
        }
        // 待機モーション
        else if (stateInfo.IsTag ("Idle")) {
        }
    }

    // コールバック : ステート遷移(Exit).
    void CallbackStateExit(AnimatorStateInfo stateInfo)
    {
        // 攻撃戻りモーション
        if (stateInfo.IsTag ("Back")) { 
            StateExitBack ();
        }
        else if (stateInfo.IsTag ("Attack")) { 
            StateExitAction ();
        }
        else if(stateInfo.IsTag ("LongRangeAttack")) {
            StateExitLongRangeAttack();
        }
        else if(stateInfo.IsTag("Skill")) { 
            StateExitSkill();
        }
        // ダメージモーション
        else if(stateInfo.IsTag("Damage")) {
            StateExitDamage();
        }
        else if(stateInfo.IsTag("Win")) {
            IsAction = false;
        }
    }

    #region Subscribe enter receivers.

    // ステート入り : 移動モーション.
    void StateEnterMove()
    {
        //Debug.Log("StateEnterMove");

        AttackCameraTargetSet ();
    }

    void StateEnterAttackStart ()
    {
    }

    // ステート入り : 攻撃モーション.
    void StateEnterAttack()
    {
        //Debug.Log("StateEnterAttack");
        if (!m_actionResult.action.IsSpecial) {
            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.attack);
        }
    }

    void StateEnterLongRangeAttack ()
    {
        if (!m_actionResult.action.IsSpecial) {
            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.attack);
        }
    }

    // ステート入り : 攻撃戻りモーション.
    void StateEnterAttackEnd()
    {
        //Debug.Log("StateEnterBackFromTarget");
        AttackCameraMyselfSet ();
    }

    // ステート入り : 射撃モーション.
    void StateEnterShot()
    {
        if (!m_actionResult.action.IsSpecial) {
            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.attack);
        }
    }

    // ステート入り : スキルモーション.
    void StateEnterSkill()
    {
        if (!m_actionResult.action.IsSpecial) {
            if (m_actionResult.skillResult.HasEffectResult (SkillEffectLogicEnum.hp_heal) && m_actionResult.skillResult.GetReceivers().Any(x => x != m_myInfo.Parameter)) {
                SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.use_heal);
            } else {
                SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.use_skill);
            }
        }
    }

    // ステート入り : 死亡モーション.
    void StateEnterDead()
    {
        if (m_myInfo.Parameter.IsBoss) {
            this.StartCoroutine (BossDeadRoutine ());
        } else {
            this.StartCoroutine (DeadRoutine ());
        }
    }
    private IEnumerator BossDeadRoutine()
    {
        // ボス死亡時はタッチブロックする
        LockInputManager.SharedInstance.IsLock = true;

        float waitLoopCount = Mathf.Floor(m_animator.GetCurrentAnimatorStateInfo (0).normalizedTime) + 1.0f;
        yield return new WaitUntil (() => m_animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= waitLoopCount);

        // ダメージエフェクト再生待ち
        yield return new WaitUntil (() => BattleEffectManager.Instance.IsNextAllWithTag("Damage"));

        // Hpバーが出てるとかっこわるいので消す
        m_myInfo.DisplayInfo (false);

        var destoryEffect = GameObjectEx.LoadAndCreateObject ("Battle/BattleEffect/eff_DestroyBoss", m_myInfo.EffectAnchor.gameObject);
        var destoryEffectAnime = destoryEffect.GetComponent<Animation> ();

        if (!string.IsNullOrEmpty(m_myInfo.Parameter.VoiceFileName)) {
            //BGMを止める
            SoundManager.SharedInstance.StopBGM();

            SoundManager.SharedInstance.PlayVoice (m_myInfo.Parameter.VoiceFileName, SoundVoiceCueEnum.dead);
        }
        destoryEffectAnime.Play ("eff_DestroyL0101_in");
        yield return new WaitUntil (() => !destoryEffectAnime.isPlaying);

        if (!string.IsNullOrEmpty (m_myInfo.Parameter.VoiceFileName)) {
            if (SoundManager.SharedInstance.IsPlayVoice) {
                destoryEffectAnime.Play ("eff_DestroyL0101_loop");
                yield return new WaitUntil (() => !SoundManager.SharedInstance.IsPlayVoice);
            }
        }

        // 死亡エフェクトと共に透過して消す。
        iTween.ValueTo (gameObject, new Hashtable () {
            {"onupdate", "UpdateUnitAlpha"}, {"onupdatetarget", gameObject}, {"from", 1.0f}, {"to", 0.0f}, {"time", 3.0f}
        });
        destoryEffectAnime.Play ("eff_DestroyL0101_out");
        // 死亡時エフェクト再生待ち
        yield return new WaitUntil (() => !destoryEffectAnime.isPlaying);

        if(m_didDead != null) {
            m_didDead();
        }
        // 死亡時演出終了タイミングでとりあえず消す.
        m_myInfo.InstanceObject.SetActive(false);
        LockInputManager.SharedInstance.IsLock = false;

        m_DamageMotionPlaying = false;
    }

    private IEnumerator DeadRoutine()
    {
        float waitLoopCount = Mathf.Floor(m_animator.GetCurrentAnimatorStateInfo (0).normalizedTime) + 1.0f;
        yield return new WaitUntil (() => m_animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= waitLoopCount);

        // ダメージエフェクト再生待ち
        yield return new WaitUntil (() => BattleEffectManager.Instance.IsNextAllWithTag("Damage"));

        // Hpバーが出てるとかっこわるいので消す
        m_myInfo.DisplayInfo (false);

        bool destoryEffectPlayEnd = false;
        // 死亡エフェクトと共に透過して消す。
        iTween.ValueTo (gameObject, new Hashtable () {
            {"onupdate", "UpdateUnitAlpha"}, {"onupdatetarget", gameObject}, {"from", 1.0f}, {"to", 0.0f}, {"time", 0.5f}
        });

        // 死亡時エフェクトを発生する。
        BattleEffectManager.Instance.CreateEffectItem("Destory", "eff_Destroy", 0.0f, m_myInfo.EffectAnchor.gameObject, 
            (go) => {
                // ボイスがない場合はSEを再生
                if(string.IsNullOrEmpty(m_myInfo.Parameter.VoiceFileName)) {
                    SoundManager.SharedInstance.PlaySE(SoundClipName.se028);
                }
            },
            (go) => {
                destoryEffectPlayEnd = true;
            }
        );

        // 死亡時エフェクト再生待ち
        yield return new WaitUntil (() => destoryEffectPlayEnd);

        if(m_didDead != null) {
            m_didDead();
        }
        // 死亡時演出終了タイミングでとりあえず消す.
        m_myInfo.InstanceObject.SetActive(false);

        m_DamageMotionPlaying = false;
    }
    private void UpdateUnitAlpha(float value)
    {
        m_myInfo.SetColor (new Color(1.0f, 1.0f, 1.0f, value));
    }

    #endregion

    #region Subscribe exit receivers.

    // ステート出 : 攻撃戻りモーション.
    void StateExitBack()
    {
        //Debug.Log("StateExitBackFromTarget");
        this.IsAction = false;
    }

    // ステート出 : 攻撃モーション.
    void StateExitAction()
    {
        //Debug.Log("StateExitAttack");
        AttackCameraReset ();
    }

    void StateExitLongRangeAttack()
    {
        //Debug.Log("StateExitAttack");
        this.IsAction = false;
    }

    // ステート出 : 攻撃モーション.
    void StateExitAttackEnd()
    {
        //Debug.Log("StateExitAttack");
        this.IsAction = false;
        AttackCameraReset ();
    }

    // ステート出 : 射撃モーション.
    void StateExitShot()
    {
        //Debug.Log("StateExitShot");
        AttackCameraTargetSet ();
        if(m_didAction != null){
            m_didAction();
        }
        StartCoroutine (WaitCameraReset());
    }

    // ステート出 : スキルモーション.
    void StateExitSkill()
    {
        //Debug.Log("StateExitSkill");
        AttackCameraTargetSet ();
        if(m_didAction != null){
            m_didAction();
        }
        StartCoroutine (WaitCameraReset());
    }

    IEnumerator WaitCameraReset()
    {
        // ダメージ演出待ち
        yield return new WaitForSeconds (1.0f);

        // カメラのリセット
        AttackCameraReset ();

        // カメラのリセット待ち
        yield return new WaitForSeconds (0.5f);

        this.IsAction = false;
    }

    // ステート出 : ダメージモーション.
    void StateExitDamage()
    {
        m_DamageMotionPlaying = false;
    }

    #endregion

    /// <summary>
    /// 移動時イベント処理
    /// </summary>
    public void StepSt()
    {
        Debug.Log ("StepSt");

        var stateInfo = m_animator.GetCurrentAnimatorStateInfo (0);

        var moveHash = new Hashtable();
        if (stateInfo.IsTag ("Move") || stateInfo.IsName("Base Layer.AttackStart")) {
            var time = m_myInfo.GetMoveTime ("21_attack_jump_st");
            if(time < 0) {
                time = stateInfo.length * (1.0f - stateInfo.normalizedTime);
            }
            moveHash.Add ("position", m_attackMovePosistion);
            moveHash.Add("time", time);
            moveHash.Add("easetype", iTween.EaseType.easeOutQuad);
            iTween.MoveTo(this.gameObject, moveHash);
        } else if (stateInfo.IsTag ("Back") || stateInfo.IsName("Base Layer.AttackEnd")){
            var time = m_myInfo.GetMoveTime ("23_attack_jump_st");
            if(time < 0) {
                time = stateInfo.length * (1.0f - stateInfo.normalizedTime);
            }
            moveHash.Add ("position", m_basePos);
            moveHash.Add("time", time);
            moveHash.Add("easetype", iTween.EaseType.easeOutQuad);
            iTween.MoveTo(this.gameObject, moveHash);
        }
    }

    /// <summary>
    /// 移動終了時イベント処理
    /// </summary>
    public void StepEn()
    {
        //Debug.Log ("StepEn");
    }

    public void AttackCameraTargetSet()
    {
        if(this.CameraMode == CameraType.Switch){
            CameraHelper.SharedInstance.ForcusTarget(m_cameraTargetPosition);
        }
        else if(this.CameraMode == CameraType.Zoom) {
            CameraHelper.SharedInstance.ResetCameraPostion();
            var cameraPos = CameraHelper.SharedInstance.BattleCamera.transform.position;
            var targetPos = m_cameraTargetPosition;
            var dirVec = targetPos - cameraPos;
            var pos = cameraPos + dirVec.normalized * dirVec.magnitude / 2.0f;
            CameraHelper.SharedInstance.ForcusTargetForZoom(pos, 19, 0.5f);
        }
    }

    public void AttackCameraMyselfSet()
    {
        // ユニットの表示を元に戻す
        if(this.CameraMode == CameraType.Switch) {
            CameraHelper.SharedInstance.ForcusTarget(m_basePos);
        }
    }

    public void AttackCameraReset()
    {
        if (this.CameraMode == CameraType.Switch) {
            CameraHelper.SharedInstance.ResetOrthographicSize ();
            CameraHelper.SharedInstance.ResetCameraPostion ();
        } else if(this.CameraMode == CameraType.Zoom) {
            CameraHelper.SharedInstance.ResetForcusForZoom(0.5f);
        }
    }

    public void CallAction()
    {
        if(m_didAction != null){
            m_didAction();
        }
    }


    private Animator m_animator;
    private Action m_didAction;
    private Action m_didDead;
    private ListItem_BattleUnit m_myInfo;
    private StateMachineObservables m_stateObserver;

    // 攻撃時アニメーション制御用ワーク
    private Vector3 m_basePos;
    private Vector3 m_cameraTargetPosition;
    private Vector3 m_attackMovePosistion;


    // ダメージ表示用ワーク
    private bool m_DamageMotionPlaying = false;
    private int m_DamageEffectPlayingCount = 0;
    private bool m_isHit;
    private bool m_counterDamage;
    private ActionResult m_damageResult;
    private DamageInfo[] m_damageInfos;
    private SkillEffectLogicEnum[] m_damageEffectLogics;
}
