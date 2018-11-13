using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

using BattleLogic;

using UnityEngine.EventSystems;

/// <summary>
/// Screen : バトルのUI部分.
/// </summary>
public class Screen_BattleUI : ViewBase, IBattleObserver
{

    public virtual Vector3 TreasurePosition
    {
        get {
            return GetScript<Transform> ("Treasure/img_TreasureBox").position;
        }
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public virtual void Init()
    {
        BattleProgressManager.Shared.RegistObserverAsFirst(this);
        BattleProgressManager.Shared.BattleUI = this;
        // メニューオブジェクト生成.
        m_menu = View_BattleMenu.Create();
        // TODO : エフェクト用のオブジェクトを作成.似たような感じのものが複数出来上がってるのでまとめられないかどうか.
        m_battleStartEffect = View_BattleEffect.Create("Battle/BattleStart");
        m_battleStartEffect.IsVisible = false;
        m_battleWaveCountEffect = View_BattleEffect.Create("Battle/BattleWave_countup");
        m_battleWaveCountEffect.IsVisible = false;
        m_battleWaveFinishEffect = View_BattleEffect.Create("Battle/BattleWave_finish");
        m_battleWaveFinishEffect.IsVisible = false;

        // ボタン設定
        this.SetCanvasCustomButtonMsg("bt_BattleMenu", DidTapMenu);
        this.SetCanvasCustomButtonMsg("bt_Speed", DidTapChangeSpeed);
        this.SetCanvasCustomButtonMsg("bt_Auto", DidTapAuto);

        var SpCommandButton = GetScript<CustomButton> ("bt_SPCommand");
        SpCommandButton.m_EnableLongPress = true;
        SpCommandButton.onClick.AddListener(OnSPCommandClick);
        SpCommandButton.onLongPress.AddListener(OnSPCommandLongPress);
        SpCommandButton.onRelease.AddListener(OnSPCommandRelease);

        var AttackCommandButton = GetScript<CustomButton> ("bt_AttackCommand");
        AttackCommandButton.m_EnableLongPress = true;
        AttackCommandButton.onClick.AddListener(OnAttackCommandClick);
        AttackCommandButton.onLongPress.AddListener(OnAttackCommandLongPress);
        AttackCommandButton.onRelease.AddListener(OnAttackCommandRelease);

        m_activeTimeLine = this.GetScript<RectTransform>("GridCountTimeLine").gameObject.GetOrAddComponent<View_ActiveTimeLine> ();
		m_activeTimeLine.Init();
    }

    /// <summary>
    /// フッターを非表示にする。
    /// </summary>
    public void DeactiveFooter()
    {
        GetScript<RectTransform> ("BattleFooter").gameObject.SetActive (false);
    }

    /// <summary>
    /// バトル開幕.
    /// </summary>
    protected virtual void CallbackBattleStart()
    {
        LockInputManager.SharedInstance.IsLock = true;

        if(AwsModule.BattleData.IsBattleStart()) {
            // バトル開始フェードを開ける.
            m_battleStartEffect.Play(
                () => {
                    // システム暗幕開ける(即時).
                    View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, null, true);
                    // ボイス再生を一緒にする。
                    // とりあえず、先頭にいる人のボイスを鳴らしてみる
                    var unit = AwsModule.BattleData.AllyParameterList.OrderBy(x => x.PositionIndex).First();
                    SoundManager.SharedInstance.PlayVoice(unit.VoiceFileName, SoundVoiceCueEnum.battle_start);
                }
            );
        } else {
            // wave毎のBGMの切り替えを行う
            var waveSetting = AwsModule.BattleData.StageWaveSettings.FirstOrDefault(x => x.wave_count == AwsModule.BattleData.WaveCount);
            if (waveSetting != null && !string.IsNullOrEmpty (waveSetting.wave_bgm)) {
                SoundManager.SharedInstance.PlayBGM (waveSetting.wave_bgm, true);
            } else {
                SoundManager.SharedInstance.PlayBGM (AwsModule.BattleData.Stage.bgm_clip_name, true);
            }

            if(AwsModule.BattleData.WaveCount < AwsModule.BattleData.MaxWaveCount) {
                m_battleWaveCountEffect.gameObject.GetOrAddComponent<View_BattleWaveCountUp> ().SetWaveCount (AwsModule.BattleData.WaveCount);
                m_battleWaveCountEffect.Play(
                    () => {
                        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, null);
                    }
                );
            } else {
                m_battleWaveFinishEffect.Play(
                    () => {
                        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, null);
                    }
                );
            }
        }
        PlayInOutUI (true);

        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.IsLock = false;
            if(AwsModule.BattleData.IsBattleStart()) {
                this.PlayStartScenario(BattleProgressManager.Shared.WaveStart); // シナリオがあれば再生.
            } else if(AwsModule.BattleData.IsWaveStart()){
                BattleProgressManager.Shared.WaveStart();
            } else {
                BattleProgressManager.Shared.NextOrder();
            }
            SetAutoAnimation(AwsModule.BattleData.IsAuto);
            SetSpeedAnimation(AwsModule.BattleData.BattleSpeed);
        });
    }   
	// シナリオがあれば再生.
    private void PlayStartScenario(Action didEnd)
    {
        var scenario = ScenarioProvider.GetScenario();
        if (string.IsNullOrEmpty(scenario)) {
            didEnd();
            return;
        }
		var tScale = Time.timeScale;
		Time.timeScale = 1;
		UtageModule.SharedInstance.SetActiveCore(true);
		UtageModule.SharedInstance.StartScenario(scenario, () => {
			didEnd();
			UtageModule.SharedInstance.SetActiveCore(false);
			Time.timeScale = tScale;
		});
    }

    public void PlayGetItemEffect()
    {
        GetScript<Animation> ("Treasure").Play();
    }

    bool IsBossIn = false;
    public virtual void UpdateBossInfo(Parameter unit)
    {
        var HpGauge = this.GetScript<Image> ("BossHPGauge");
        var HPGaugeWarning = this.GetScript<Image> ("BossHPGaugeWarning");
        var HPGaugeDying = this.GetScript<Image> ("BossHPGaugeDying");
        float hpProgress = unit.HpProgress;
        if (hpProgress > 0.5f) {
            HpGauge.gameObject.SetActive (true);
            HPGaugeWarning.gameObject.SetActive (false);
            HPGaugeDying.gameObject.SetActive (false);
            HpGauge.rectTransform.localScale = new Vector3 (hpProgress, 1.0f, 1.0f);
        } else if (hpProgress > 0.2f) {
            HpGauge.gameObject.SetActive (false);
            HPGaugeWarning.gameObject.SetActive (true);
            HPGaugeDying.gameObject.SetActive (false);
            HPGaugeWarning.rectTransform.localScale = new Vector3 (hpProgress, 1.0f, 1.0f);
        } else {
            HpGauge.gameObject.SetActive (false);
            HPGaugeWarning.gameObject.SetActive (false);
            HPGaugeDying.gameObject.SetActive (true);
            HPGaugeDying.rectTransform.localScale = new Vector3 (hpProgress, 1.0f, 1.0f);
        }
    }

    public virtual void UpdateBossCondition(Parameter unit)
    {
        var ConditionIconRoot = this.GetScript<Transform> ("BossHP/Condition");

        // 弱りモーション設定
        ConditionIconRoot.gameObject.SetActive (unit.Conditions.HasCondition);

        if (unit.Conditions.HasCondition) {
            int conditionCount = unit.Conditions.Count;
            int spriteCount = 0;
            for (spriteCount = 1; spriteCount <= 4 && spriteCount <= conditionCount ; ) {
                var condition = unit.Conditions [spriteCount - 1];
                if (!condition.IsEnable) {
                    continue;
                }
                var ConditionIcon = this.GetScript<Image> (string.Format("Condition{0}", spriteCount));
                ConditionIcon.gameObject.SetActive (true);
                ConditionIcon.overrideSprite = IconLoader.LoadConditionIcon (condition.ConditionData);
                ++spriteCount;
            }

            for (; spriteCount <= 4; ++spriteCount) {
                var ConditionIcon = this.GetScript<Image> (string.Format("Condition{0}", spriteCount));
                ConditionIcon.gameObject.SetActive (false);
            }

            // 状態異常が5つ以上かかっている場合は[…]をだす。
            if (conditionCount > 4) {
                var ConditionMore = this.GetScript<Transform> ("ConditionMore");
                ConditionMore.gameObject.SetActive (false);
            }
        }
    }

    public virtual void DisplayBossInfo(bool enable)
    {
        if (enable == IsBossIn) {
            return;
        }
        var bossAnimation = GetScript<Animation> ("Boss");
        if (IsBossIn) {
            bossAnimation.Play ("BossOut");
            IsBossIn = false;
        } else {
            bossAnimation.gameObject.SetActive (true);
            bossAnimation.Play ("BossIn");
            IsBossIn = true;
        }
    }

    #region imprements IBattleViewColleague.
    /// <summary>進捗変化.</summary>
    public void ChangeProgress(BattleState state, ListItem_BattleUnit unit)
    {
        switch(state){
            case BattleState.BattleStart:
                CallbackBattleStart ();
                break;
            case BattleState.DidAlignOrder:
                CallbackDidInitBattle();
                break;
            case BattleState.SelectAction:
                CallbackSelectAction ();
                break;
            case BattleState.DidAction:
                CallbackDidPerAction();
                break;
            case BattleState.TurnEnd:
                CallbackTurnEnd();
                break;
            case BattleState.EndWave:
                CallbackChangeWave(unit);
                break;
            case BattleState.GameOver:
                // メニューを効かないようにする
                if (!AwsModule.BattleData.IsRetire && !AwsModule.BattleData.IsPVP) {
                    CallbackAskContinue ();
                } else {
                    CallbackGameOver ();
                }
                break;
        }
    }
    /// <summary>攻撃者の次の行動順確定時.</summary>
    public void DecideAttackerNextOrder(IActionOrderItem attacker, int order, bool isAdd)
    {
        CallbackDecideAttackerOrder(attacker, order, isAdd);
    }

    public void SortActionOrder (ActionOrderSortInfo[] sortInfo)
    {
        CallbackSortActionOrder (sortInfo);
    }

    /// <summary>敵再ターゲッティング時.</summary>
    public void RetargetEnemy(ListItem_BattleUnit newTarget)
    {
        if(newTarget == null) {
            return;
        }
        if(newTarget.IsPlayer) {
            return; // プレイヤーに対するターゲッティングはここでは対応しない.
        }

        // 行動順ターゲット表示.
        foreach(var o in m_listOrder) {
            o.IsTarget = o.ComareItem(newTarget);
        }
        // ステータスUI上の属性相性.
        var attacker = BattleProgressManager.Shared.OrderQueue.Peek() as ListItem_BattleUnit;
        if (attacker == null) {
            return;
        }
    }
    /// <summary>味方再ターゲッティング時.</summary>
    public void RetargetAlly(ListItem_BattleUnit newTarget)
    {
    }
    View_BattleCommandNotes m_ViewBattleCommandNotes;
    /// <summary>スキル効果表示.</summary>
    public void DrawSkillDetail(SkillParameter skillParam, bool bVisible)
    {
        if (m_ViewBattleCommandNotes == null) {
            m_ViewBattleCommandNotes = View_BattleCommandNotes.Create (GetScript<RectTransform>("CommandNotesAnchor"));
        }
        if (bVisible) {
            m_ViewBattleCommandNotes.Open (skillParam);
        } else if(m_ViewBattleCommandNotes.IsOpen) {
            m_ViewBattleCommandNotes.Close ();
        }

        m_activeTimeLine.DrawSkillDetail (skillParam, bVisible);
    }

    View_BattleUnitNotes m_ViewBattleUnitNotes;
    /// <summary>ユニット詳細表示.</summary>
    public void DrawUnitDetail(ListItem_BattleUnit invoker, bool bVisible)
    {
        if (m_ViewBattleUnitNotes == null) {
            m_ViewBattleUnitNotes = View_BattleUnitNotes.Create ();
        }

        if (bVisible) {
            var rootName = invoker.IsPlayer ? "PlayerNotesAnchor" : "EnemyNotesAnchor";
            m_ViewBattleUnitNotes.Open (this.GetScript<RectTransform>(rootName).gameObject, invoker.Parameter);
        } else if (m_ViewBattleUnitNotes.IsOpen) {
            m_ViewBattleUnitNotes.Close ();
        }
    }
    public void ActionStart(ListItem_BattleUnit invoker, BattleLogic.SkillParameter skillParam)
    {
        if(AwsModule.BattleData.IsAuto && invoker.IsPlayer) {
            if (skillParam.IsSpecial) {
                this.PlayAnimeUnitUI (UnitUIAnimState.TurnEnd, invoker, skillParam);
            } else {
                this.PlayAnimeUnitUI (UnitUIAnimState.MoveSide, invoker, skillParam);
            }
        }
    }
    #endregion  

    // UI開閉アニメーションの再生.
    protected void PlayInOutUI(bool bIn)
    {
        var aName = bIn ? "HudIn" : "HudOut";
        BattleScheduler.Instance.AddSchedule(this.PlayAnimation("Hud", aName));
        var boss = AwsModule.BattleData.EnemyParameterList.FirstOrDefault (x => x.IsBoss);
        if (boss != null) {
            GetScript<Image> ("BossElementIcon").overrideSprite = IconLoader.LoadElementIcon (boss.Element);
            UpdateBossInfo (boss);
            UpdateBossCondition (boss);
            BattleScheduler.Instance.AddAction (
                () => {
                    DisplayBossInfo (bIn);
                }
            );
        }
    }

    protected IEnumerator PlayUIAndUnitUIIn(ListItem_BattleUnit unit)
    {
        var animHub = this.GetScript<Animation>("Hud");
        var animUnit = this.GetScript<Animation>("Unit");

        animHub.gameObject.SetActive (true);
        animUnit.gameObject.SetActive (true);

        unitUiAnimeState = UnitUIAnimState.In;

        if(unit != null) {
            UpdateViewStatus(unit, true);
        }

        animHub.Play ("HudIn");
        animUnit.Play ("UnitIn");

        yield return new WaitUntil(() => !animHub.isPlaying && !animUnit.isPlaying);

    }

    public enum UnitUIAnimState
    {
        None,

        In,
        MoveSide,
        TurnEnd
    }

    private enum UnitUIAnimSkillType
    {
        Normal,
        Chara,
        Weapon,
        Special,
    }

    private UnitUIAnimState unitUiAnimeState = UnitUIAnimState.None;
    private UnitUIAnimSkillType unitUiAnimSkill = UnitUIAnimSkillType.Normal;

    protected void PlayAnimeUnitUI(UnitUIAnimState state, ListItem_BattleUnit unit = null, SkillParameter action = null, Action didEnd = null)
    {
        if (unitUiAnimeState == state) {
            return;
        }

        Debug.Log ("PlayAnimeUnitUI: state = " + state);
        if (action != null) {
            if (action.IsSpecial) {
                unitUiAnimSkill = UnitUIAnimSkillType.Special;
            } else if (action.IsNormalAction) {
                unitUiAnimSkill = UnitUIAnimSkillType.Normal;
            } else if (action.IsWeaponSkill) {
                unitUiAnimSkill = UnitUIAnimSkillType.Weapon;
            } else {
                unitUiAnimSkill = UnitUIAnimSkillType.Chara;
            }
        }

        string aName = null;
        var prevState = unitUiAnimeState;
        unitUiAnimeState = state;
        Action startAction = null;
        switch (unitUiAnimeState) {
        case UnitUIAnimState.In:
            startAction = () => {
                if(unit != null) {
                    UpdateViewStatus(unit, true);
                }
            };
            aName = "UnitIn";
            break;
        case UnitUIAnimState.MoveSide:
            // コマンドボタンをはける
            GetScript<TextMeshProUGUI> ("txtp_NormalSkillName").SetText (action.Skill.display_name);
            if (prevState == UnitUIAnimState.In) {
                startAction = () => {
                    HideCommand(action);
                };
                if (unitUiAnimSkill == UnitUIAnimSkillType.Normal) {
                    aName = "UnitTurnInAttack";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Chara) {
                    aName = "UnitTurnInCharaSkill";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Weapon) {
                    aName = "UnitTurnInWeaponSkill";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Special) {
                    aName = "UnitOut";
                }
            } else {
                startAction = () => {
                    if(unit != null) {
                        UpdateViewStatus(unit, false);
                    }
                    HideCommand(null);
                };
                if (unitUiAnimSkill == UnitUIAnimSkillType.Normal) {
                    aName = "UnitTurnInAttackAuto";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Chara) {
                    aName = "UnitTurnInCharaSkillAuto";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Weapon) {
                    aName = "UnitTurnInWeaponSkillAuto";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Special) {
                    aName = "UnitOut";
                }
            }
            break;
        case UnitUIAnimState.TurnEnd:
            if (prevState == UnitUIAnimState.MoveSide) {
                if (unitUiAnimSkill == UnitUIAnimSkillType.Normal) {
                    aName = "UnitTurnOutAttack";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Chara) {
                    aName = "UnitTurnOutCharaSkill";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Weapon) {
                    aName = "UnitTurnOutWeaponSkill";
                } else if(unitUiAnimSkill == UnitUIAnimSkillType.Special) {
                    aName = "UnitOut";
                }
            } else if(prevState != UnitUIAnimState.None){
                // コマンドボタンをはける
                startAction = () => {
                    HideCommand(null);
                };
                aName = "UnitOut";
            }
            break;
        default:
            break;
        }
        BattleScheduler.Instance.AddSchedule(this.PlayAnimation("Unit", aName, startAction));

        if (didEnd != null) {
            BattleScheduler.Instance.AddAction (didEnd);
        }
    }
    protected IEnumerator PlayAnimation(string rootName, string animName, Action startAction=null, Action endAction=null)
    {
        if (string.IsNullOrEmpty (animName)) {
            yield break;
        }
        var anim = this.GetScript<Animation>(rootName);
        // SetActive & PlayAutomaticallyで基本的にLegacyアニメーションは運用している.
        if(!anim.gameObject.activeSelf) {
            //anim.clip = anim.GetClip(animName);
            anim.gameObject.SetActive(true);
        }

        if (startAction != null) {
            startAction ();
        }

        anim.Play(animName);

        yield return new WaitUntil(() => !anim.isPlaying);

        if (endAction != null) {
            endAction ();
        }
    }

    // バトル設定周りのView更新.
    protected virtual void UpdateViewSettings()
    {
        var info = AwsModule.BattleData;

        // バトル終了時は更新しない
        if (info.EndBattle) {
            return;
        }
        this.GetScript<TextMeshProUGUI>("txtp_TreasureCount").SetText(info.ItemDropCount);
        this.GetScript<TextMeshProUGUI>("txtp_BattleCount").SetTextFormat("{0}/{1}", info.WaveCount, info.MaxWaveCount);
    }

    // キャラクターステータスView更新
	protected virtual void UpdateViewStatus(ListItem_BattleUnit unit, bool createCommand)
    {
        m_Invoker = unit;

        // TODO : 頻繁に呼ばれるこのUpdateのたびに削除と生成を繰り返すのは効率的ではないので別の方法を考える.
        var gridObj = this.GetScript<GridLayoutGroup>("GridBattleCommand").gameObject;
        gridObj.DestroyChildren();
        m_BattleCommandObjects.Clear ();
        if (createCommand) {
            int skillCount = unit.Parameter.ActionSkillList.Length;
            for (int i = 0; i < skillCount; ++i) {
                var skill = unit.Parameter.ActionSkillList [i];

                string prefabName = string.Empty;
                if (skill.IsNormalAction) {
                    continue;
                } else if (skill.IsWeaponSkill) {
                    prefabName = "Battle/ListItem_CommandWeaponSkill";
                } else {
                    prefabName = "Battle/ListItem_CommandCharaSkill";
                }

                var go = GameObjectEx.LoadAndCreateObject (prefabName, gridObj);
                var c = go.GetOrAddComponent<ListItem_BattleCommand> ();
                c.Init (skill, didTabActionStart);
                m_BattleCommandObjects.Add (c);
            }

            // 通常攻撃表示
            FrameInAttackCommand ();

            // SPコマンド表示
            FrameInSPCommand (unit.Parameter.SpecialSkill, unit.Parameter.SpProgress);

            m_IsHideCommand = false;
        }
  
        var imageRoot = GetScript<RectTransform> ("CharacterAnchor");
        // TODO: UIの表示順の問題などでコメントアウトしておく
        if (nowStandingImage != unit.StandingImage) {
            if (nowStandingImage != null) {
                nowStandingImage.transform.SetParent (null);
                nowStandingImage.SetActive (false);
                nowStandingImage = null;
            }

            if (unit.StandingImage != null) {
                nowStandingImage = unit.StandingImage;
                nowStandingImage.transform.localScale = Vector3.one;
                nowStandingImage.SetActive (true);
                nowStandingImage.transform.SetParent (imageRoot);
                nowStandingImage.transform.localRotation = Quaternion.identity;
                nowStandingImage.transform.localScale = Vector3.one;
                nowStandingImage.transform.localPosition = Vector3.zero;
                MasterDataTable.card_live2d_anchor_setting.FixPostionFaceCenter (unit.Parameter.ID, nowStandingImage, 0.001f);
                nowStandingImage.GetComponent<Live2D.Cubism.Rendering.CubismRenderController> ().SortingLayer = "Default";
                nowStandingImage.GetComponent<Live2D.Cubism.Rendering.CubismRenderController> ().SortingOrder = 1;
                nowStandingImage.SetLayerRecursively (nowStandingImage.transform.parent.gameObject.layer);
            }
        } else if (nowStandingImage.transform.parent != imageRoot) {
            // 親が違っていた場合は設定をし直す。
            nowStandingImage.transform.SetParent (null);
            nowStandingImage.transform.localScale = Vector3.one;
            nowStandingImage.SetActive (true);
            nowStandingImage.transform.SetParent (imageRoot);
            nowStandingImage.transform.localRotation = Quaternion.identity;
            nowStandingImage.transform.localScale = Vector3.one;
            nowStandingImage.transform.localPosition = Vector3.zero;
            MasterDataTable.card_live2d_anchor_setting.FixPostionFaceCenter (unit.Parameter.ID, nowStandingImage, 0.001f);
            nowStandingImage.GetComponent<Live2D.Cubism.Rendering.CubismRenderController> ().SortingLayer = "Default";
            nowStandingImage.GetComponent<Live2D.Cubism.Rendering.CubismRenderController> ().SortingOrder = 1;
            nowStandingImage.SetLayerRecursively (nowStandingImage.transform.parent.gameObject.layer);
        }

        // SPFullアニメーションの表示
        if (unit.Parameter.SpecialSkill == null) {
            GetScript<RectTransform> ("TurnUnitAnchor/eff_SPFull").gameObject.SetActive (false);
        } else {
            if (unit.Parameter.IsSpMax) {
                GetScript<RectTransform> ("TurnUnitAnchor/eff_SPFull").gameObject.SetActive (true);
            } else {
                GetScript<RectTransform> ("TurnUnitAnchor/eff_SPFull").gameObject.SetActive (false);
            }
        }
    }

    private bool m_IsHideCommand;
    private void HideCommand(SkillParameter action)
    {
        if (m_IsHideCommand) {
            return;
        }

        m_IsHideCommand = true;
        if (action == null) {
            // 全部フレームアウト
            FrameOutSPCommand(false);
            FrameOutAttackCommand (false);
            foreach (var command in m_BattleCommandObjects) {
                if (command.isActiveAndEnabled) {
                    command.FrameOut (action);
                }
            }
        } else {
            // 使ったのはUsed
            FrameOutSPCommand(action.IsSpecial);
            FrameOutAttackCommand (action.IsNormalAction);
            foreach (var command in m_BattleCommandObjects) {
                if (command.isActiveAndEnabled) {
                    command.FrameOut (action);
                }
            }
        }
    }

    protected ListItem_BattleUnit m_Invoker;
    List<ListItem_BattleCommand> m_BattleCommandObjects = new List<ListItem_BattleCommand> ();

    protected void FrameInAttackCommand()
    {
        var animation = GetScript<Animation> ("CommandAttack");
        animation.clip = animation.GetClip ("AttackLoop");
        animation.Play ("AttackLoop");
    }

    protected void FrameOutAttackCommand(bool isUsed)
    {
        var animation = GetScript<Animation> ("CommandAttack");
        animation.clip = null;
        if (isUsed) {
            animation.Play ("AttackUse");
        } else {
            animation.Play ("AttackExit");
        }
    }

    protected virtual void OnAttackCommandClick()
    {
        if (AwsModule.BattleData.IsAuto) {
            return;
        }
        if (m_Invoker.Parameter.NormalSkill != null) {
            LockInputManager.SharedInstance.IsLock = true;
            PlayAnimeUnitUI (UnitUIAnimState.MoveSide, m_Invoker, m_Invoker.Parameter.NormalSkill);
            BattleScheduler.Instance.AddAction (() => {
                LockInputManager.SharedInstance.IsLock = false;
                BattleProgressManager.Shared.ActionStart (m_Invoker.Parameter.NormalSkill);
            });
        }
    }

    protected virtual void OnAttackCommandLongPress()
    {
        if (BattleProgressManager.Shared.IsInit) {
            BattleProgressManager.Shared.DrawSkillDetail (m_Invoker.Parameter.NormalSkill, true);
        }
    }

    protected virtual void OnAttackCommandRelease()
    {
        if (BattleProgressManager.Shared.IsInit) {
            BattleProgressManager.Shared.DrawSkillDetail (m_Invoker.Parameter.NormalSkill, false);
        }
    }

    protected void FrameInSPCommand(SkillParameter spSkill, float spProgress)
    {
        if (spSkill == null) {
            GetScript<RectTransform>("CommandSP").gameObject.SetActive(false);
        } else {
            GetScript<RectTransform>("CommandSP").gameObject.SetActive(true);
            GetScript<TextMeshProUGUI> ("SPName/txtp_CommandName").SetText(spSkill.Skill.display_name);
            GetScript<Image> ("SPGauge/img_CommandGauge").fillAmount = spProgress;
            var animation = GetScript<Animation> ("CommandSP");
            if (spProgress >= 1.0f) {
                isSpOnLoop = true;
                animation.clip = animation.GetClip ("SPOnLoop");
                animation.Play ("SPOnLoop");
            } else {
                isSpOnLoop = false;
                animation.clip = animation.GetClip ("SPOffLoop");
                animation.Play ("SPOffLoop");
            }
        }
    }

    private bool isSpOnLoop = false;
    protected void FrameOutSPCommand(bool isUsed)
    {
        var animation = GetScript<Animation> ("CommandSP");
        if (animation.isActiveAndEnabled) {
            animation.clip = null;
            if (isUsed) {
                animation.Play ("SPOnUse");
            } else {
                if (!isSpOnLoop) {
                    animation.Play ("SPOffExit");
                } else {
                    animation.Play ("SPOnExit");
                }
            }
        }
    }

    protected virtual void OnSPCommandClick()
    {
        if (AwsModule.BattleData.IsAuto) {
            return;
        }
        if (m_Invoker.Parameter.IsSpMax && m_Invoker.Parameter.SpecialSkill != null) {
            LockInputManager.SharedInstance.IsLock = true;
            PlayAnimeUnitUI (UnitUIAnimState.TurnEnd, m_Invoker, m_Invoker.Parameter.SpecialSkill);
            BattleScheduler.Instance.AddAction (() => {
                LockInputManager.SharedInstance.IsLock = false;
                BattleProgressManager.Shared.ActionStart (m_Invoker.Parameter.SpecialSkill);
            });
        }
    }

    protected virtual void OnSPCommandLongPress()
    {
        BattleProgressManager.Shared.DrawSkillDetail(m_Invoker.Parameter.SpecialSkill, true);
    }

    protected virtual void OnSPCommandRelease()
    {
        BattleProgressManager.Shared.DrawSkillDetail(m_Invoker.Parameter.SpecialSkill, false);
    }


    // 攻撃開始. 
    protected virtual void didTabActionStart(SkillParameter action)
    {
        if (AwsModule.BattleData.IsAuto) {
            return;
        }
        // 多重入力
        LockInputManager.SharedInstance.IsLock = true;

        this.PlayAnimeUnitUI(UnitUIAnimState.MoveSide, null, action);
        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.IsLock = false;
            BattleProgressManager.Shared.ActionStart(action);
        });
    }

    // 行動順リストの作成.
    private void CreateActiveTimeList()
    {
        //Debug.Log("Call CreateActiveTimeList");

        m_activeTimeLine.Reset ();

        // 一番最初に行動予定の味方ユニットを取得
        var firstUnit = BattleProgressManager.Shared.OrderQueue.First(
            x => x.ItemType == ActionOrderItemType.Unit && x.IsPlayer
        ) as ListItem_BattleUnit;
        // ターゲッティング初期表示
        this.RetargetEnemy(firstUnit.Target);

        #if false
        // TODO : 以下デバッグ用出力.落ち着いたら消す.
        var txt = ">>>>>>> CreateOrderList";
        foreach(var o in m_listOrder) {
            txt += "\nname=" + o.UnitName + "/index=" + o.transform.GetSiblingIndex();
        }
        Debug.Log(txt);
        txt = ">>>>>>> CreateOrderQueue";
        for(var i = BattleProgressManager.Shared.OrderQueue.Count - 1; i >= 0; --i) {
            txt += "\nname=" + BattleProgressManager.Shared.OrderQueue[i].Parameter.Name + "/index=" + i;
        }
        Debug.Log(txt);
        // -----------------------------------
        #endif
    }

    #region Callbacks.
    //コールバック : バトル初期化ごと.
    protected virtual void CallbackDidInitBattle()
    {
        this.CreateActiveTimeList();
        this.UpdateViewSettings();
    }

    // コールバック : 攻撃者の行動順確定ごと
    protected virtual void CallbackDecideAttackerOrder(IActionOrderItem item, int index, bool isAdd)
    {
        // 状態などが追加される場合の処理
        if (isAdd) {
            m_activeTimeLine.Insert (item, index);
            return;
        } else {
            m_activeTimeLine.Replace (item, 0, index);
        }
        m_activeTimeLine.ResetOrder ();
    }

    protected virtual void CallbackSortActionOrder(ActionOrderSortInfo[] sortInfo)
    {
        int outCount = View_ActiveTimeLine.GetOutCount(sortInfo);
        foreach (var info in sortInfo) {
            if(info.oldIndex != info.newIndex) {
                m_activeTimeLine.Replace (info.item, info.oldIndex, info.newIndex, outCount);
                if (View_ActiveTimeLine.IsTimeLineOut (info)) {
                    outCount--;
                }
            }
        }
    }

    public void ResetActionOrder ()
    {
        m_activeTimeLine.Reset ();
    }

    // コールバック : ユーザーアクション選択
    protected virtual void CallbackSelectAction()
    {
        this.PlayAnimeUnitUI (UnitUIAnimState.In, BattleProgressManager.Shared.OrderQueue.Peek () as ListItem_BattleUnit);
    }

    // コールバック : 攻撃ごと.
    protected virtual void CallbackDidPerAction()
    {
        //Debug.Log("View_BattleCanvas : CallbackDidPerAttack");
#if false
        // TODO : 以下デバッグ用出力.落ち着いたら消す.
        var txt = ">>>>>>> PrevOrderList";
        foreach(var o in m_listOrder) {
            txt += "\nname=" + o.UnitName + "/index=" + o.transform.GetSiblingIndex();
        }
        Debug.Log(txt);
#endif
        // ---------------------------------

        m_activeTimeLine.DidAttackPreUpdate ();

#if false
        // TODO : 以下デバッグ用出力.落ち着いたら消す.
        txt = ">>>>>>> OrderList";
        foreach(var o in m_listOrder) {
            txt += "\nname=" + o.UnitName + "/index=" + o.transform.GetSiblingIndex();
        }
        Debug.Log(txt);
        txt = ">>>>>>> OrderQueue";
        for(var i = BattleProgressManager.Shared.OrderQueue.Count - 1; i >= 0; --i) {
            txt += "\nname=" + BattleProgressManager.Shared.OrderQueue[i].Parameter.Name + "/index=" + i;
        }
        Debug.Log(txt);
#endif
    }

    // コールバック : ターン終了.ユニット行動ごとに呼ばれる.
    protected virtual void CallbackTurnEnd()
    {
        // Waveが終了していたら次のOrderの処理を呼ばない
        bool callNextOrder = !AwsModule.BattleData.IsEndWave;

        this.PlayAnimeUnitUI (UnitUIAnimState.TurnEnd, null);
        BattleScheduler.Instance.AddAction (() => {
            // プレイヤーなら次のプレイヤーのステータスUIを開く.
            this.UpdateViewSettings ();
            if (callNextOrder) {
                BattleProgressManager.Shared.NextOrder ();
            }
        });
    }
        
    // コールバック : WAVE切り替え.
    protected virtual void CallbackChangeWave(ListItem_BattleUnit unit)
    {
        // バトル終了.
        if(AwsModule.BattleData.EndBattle) {
            // フッターを非表示にする
            BattleScheduler.Instance.AddAction (() => {
                DeactiveFooter ();
            });

            SoundManager.SharedInstance.PlayBGM (SoundClipName.Jingle001);
			ScenarioProvider.CurrentScenarioState = ScenarioProgressState.OutBattle;
            BattleProgressManager.Shared.WinMotionStart(unit, () => {
                this.GetScript<RectTransform>("GridCountTimeLine").gameObject.SetActive(false);

                // 通信.
                if(AwsModule.BattleData.BattleEntryData != null) {
                    LockInputManager.SharedInstance.IsLock = true;
                    SendAPI.QuestsCloseQuest(
                        AwsModule.ProgressData.CurrentQuest.ID, 
                        true,
                        AwsModule.BattleData.BattleEntryData.EntryId,
                        AwsModule.ProgressData.CurrentScenarioSelectIdList.ToArray(),
                        AwsModule.BattleData.MissionProgress.GetAchivedMissionIndexList().ToArray(),
                        (success, res) => {
                            if (res == null) {
                                Debug.LogError("SendBattlesOpenBattleStage Error");
                                return; // エラー.
                            }                     

							// クエストデータ更新.
                            var questAchivement = QuestAchievement.CacheGet(AwsModule.ProgressData.CurrentQuest.ID);
                            var bLatestQuestClear =  false;
                            if(questAchivement != null) {
                                bLatestQuestClear = !QuestAchievement.CacheGet(AwsModule.ProgressData.CurrentQuest.ID).IsAchieved;
                            }
							res.QuestAchievement.CacheSet();
                            
                            // 各種クエスト達成報酬設定.あれば.
							if(res.QuestRewardItemList != null && res.QuestRewardItemList.Length > 0){
								// クエスト.
								AwsModule.ProgressData.RewardItemForClearMainQuestQuest = res.QuestRewardItemList.Length > 0 ? res.QuestRewardItemList[0]: null; // 0=クエスト報酬.
								// ステージ.
								AwsModule.ProgressData.RewardItemForClearMainQuestStage = res.QuestRewardItemList.Length > 1 ? res.QuestRewardItemList[1]: null; // 1=ステージ報酬.
                                // 章.
								AwsModule.ProgressData.RewardItemForClearMainQuestChapter = res.QuestRewardItemList.Length > 2 ? res.QuestRewardItemList[2]: null; // 2=章報酬.
							}

                            // ステージ数更新.
                            if (AwsModule.ProgressData.CurrentQuest != null) {
                                AwsModule.ProgressData.UpdateReleaseQuest(AwsModule.ProgressData.CurrentQuest);
                            }

                            // クエストでの取得物の更新
                            if(res.MagikiteDataList != null) {
                                res.MagikiteDataList.CacheSet();
                            }
                            if(res.MaterialDataList != null) {
                                res.MaterialDataList.CacheSet();
                            }
                            if(res.WeaponDataList != null) {
                                res.WeaponDataList.CacheSet();
                            }
                            if(res.ConsumerDataList != null) {
                                res.ConsumerDataList.CacheSet();
                            }

                            // あればシナリオ再生
							this.PlayStartScenario(() => {
								View_BattleResult.CreateForQuestResult(res, bLatestQuestClear);

								// ユーザーデータ更新.
                                AwsModule.UserData.UserData = res.UserData;
                                res.MemberCardDataList.CacheSet();
							});

							LockInputManager.SharedInstance.IsLock = false;
                        }
                    );
                } else {
					View_BattleResult.CreateForQuestResult(null, false);
                }

            });
            return;
        }

        // puaseがかかっていたらまつ
        BattleScheduler.Instance.AddWaitUntil(() => !BattleProgressManager.Shared.IsPause);
        // 演出.
        if(AwsModule.BattleData.WaveCount < AwsModule.BattleData.MaxWaveCount) {
            m_battleWaveCountEffect.gameObject.GetOrAddComponent<View_BattleWaveCountUp> ().SetWaveCount (AwsModule.BattleData.WaveCount);
            m_battleWaveCountEffect.Play();
        } else {
            m_battleWaveFinishEffect.Play();
        }
        BattleScheduler.Instance.AddAction (BattleProgressManager.Shared.WaveStart);
    }

    protected virtual void CallbackAskContinue ()
    {
        // 
        View_BattleContinuePop.Create(
            () => {
                // 復活処理
                ContinueProc();
            },
            () => {
                CallbackGameOver();
            }
        );
    }

    protected void ContinueProc()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        // API
        SendAPI.QuestsContinueQuest (AwsModule.BattleData.QuestID, AwsModule.BattleData.BattleEntryData.EntryId,
            (result, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!result) {
                    if(response != null && response.ResultCode == (int)ServerResultCodeEnum.LACK_OF_GEM) {
                        PopupManager.OpenPopupSystemYN("ジェムを購入しますか？", () => {
                            // ジェム購入処理を開く
                        }, CallbackAskContinue);
                    } else {
                        // 汎用エラー時処理
                        PopupManager.OpenPopupSystemOK("通信エラーが発生しました。", CallbackAskContinue);
                    }
                    return;
                }
                if(response.ResultCode == 0) {
                    // 大元のデータを更新
                    AwsModule.UserData.UserData = response.UserData;

                    // コンテニュー処置
                    BattleProgressManager.Shared.ContinueProc();

                    // Menuを聞くようにする。
                    GetScript<CustomButton> ("bt_BattleMenu").interactable = true;
                }
            }
        );
    }

    // ゲームオーバー時処理
    protected virtual void CallbackGameOver()
    {
        var gameOverEffect = View_BattleEffect.Create("Battle/BattleGameOver");
        // 再生開始まで表示を隠しておく
        gameOverEffect.IsVisible = false;
        // 再生終了時に表示を消すか
        gameOverEffect.IsEndOfPlaybackHide = false;

        // すでに溜まっているキューを削除する。
        BattleScheduler.Instance.Clear ();

        // フッターを非表示にする
        BattleScheduler.Instance.AddAction (() => {
            DeactiveFooter ();
        });
        SoundManager.SharedInstance.PlayBGM (SoundClipName.Jingle002);
        gameOverEffect.Play ();
        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.ForceUnlockInput();
            if(AwsModule.BattleData.BattleEntryData != null) {
                SendAPI.QuestsCloseQuest(
                    AwsModule.ProgressData.CurrentQuest.ID,
                    false,
                    AwsModule.BattleData.BattleEntryData.EntryId,
                    new int[0], 
                    new int[0],
                    (success, res) => {
                        if (res == null) {
                            Debug.LogError("SendBattlesCloseBattleStage Error");
                            return; // エラー.
                        }
						res.QuestAchievement.CacheSet();
                        AwsModule.UserData.UserData = res.UserData;
                        AwsModule.BattleData.BattleEntryData = null;
                        AwsModule.BattleData.StageID = 0;
                        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                            AwsModule.BattleData.GameOverProc();
                            ScreenChanger.SharedInstance.GoToMyPage();
                        });
                    }
                );
            } else {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    AwsModule.BattleData.GameOverProc();
                    ScreenChanger.SharedInstance.GoToMyPage();
                });
            }
        });
    }


    public virtual void DisableBattleMenu()
    {
        GetScript<CustomButton> ("bt_BattleMenu").interactable = false;
        if (m_MenuOpenCoroutin != null) {
            View_FadePanel.SharedInstance.IsLightLoading = false;
            LockInputManager.SharedInstance.IsLock = false;
            BattleProgressManager.Shared.IsPause = false;
            StopCoroutine (m_MenuOpenCoroutin);
        }
        if (m_menu.IsVisible) {
            m_menu.Close ();
        }
    }
    Coroutine m_MenuOpenCoroutin = null;

    #endregion

    #region ButtonDelegate
    // ボタン：メニュー.
    protected void DidTapMenu()
    {
        m_MenuOpenCoroutin = this.StartCoroutine(OpenMenuWithWaitBattleProcess());
    }
    IEnumerator OpenMenuWithWaitBattleProcess()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        BattleProgressManager.Shared.IsPause = true;
        // バトル処理待機.
        while(BattleProgressManager.Shared.IsProcessing){
            yield return null;
        }
        // WAVE切り替え中だった場合これの演出も待つ.
        var eff = AwsModule.BattleData.WaveCount < AwsModule.BattleData.MaxWaveCount ? m_battleWaveCountEffect : m_battleWaveFinishEffect;
        while(eff.isPlaying){
            yield return null;
        }

        // 戦闘が終了していたら抜ける.
        if(AwsModule.BattleData.EndBattle){
            View_FadePanel.SharedInstance.IsLightLoading = false;
            LockInputManager.SharedInstance.IsLock = false;
            BattleProgressManager.Shared.IsPause = false;
            m_MenuOpenCoroutin = null;
            yield break;
        }

        // メニュー展開
        m_menu.Open(false, () => BattleProgressManager.Shared.IsPause = false);
        View_FadePanel.SharedInstance.IsLightLoading = false;
        LockInputManager.SharedInstance.IsLock = false;
        m_MenuOpenCoroutin = null;
    }

    // ボタン：スピード変更.
    protected void DidTapChangeSpeed()
    {
        SetSpeedAnimation(BattleProgressManager.Shared.ChangeBattleSpeed ());
    }

    protected void SetSpeedAnimation(int speed)
    {
        var battleSpeedAnimation = GetScript<Animation> ("BattleSpeed");

        if (speed == 1) {
            // Normal
            battleSpeedAnimation.Play("BtSpeedOffLoop");
        } else {
            // Fast
            battleSpeedAnimation.Play("BtSpeedOnLoop");
        }
    }

    // ボタン：オートモード切り替え.
    protected void DidTapAuto()
    {
        SetAutoAnimation (AwsModule.BattleData.ChangeAutoMode());

        if(!BattleProgressManager.Shared.IsProcessing && BattleProgressManager.Shared.CurrentState == BattleState.SelectAction && AwsModule.BattleData.IsAuto ){
            BattleScheduler.Instance.AddAction (BattleProgressManager.Shared.ExecStartUnitAction);
        }
    }

    protected void SetAutoAnimation(bool auto)
    {
        var autoBattleAnimetion = GetScript<Animation> ("AutoBattle");
        // 一旦セミオートはむし
        if (!auto) {
            // Auto Off
            autoBattleAnimetion.Play("BtAutoOffLoop");
        } else {
            // Auto On
            autoBattleAnimetion.Play("BtAutoOnAutoLoop");
        }
    }

    #endregion


    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    void OnApplicationPause(bool pause)
    {
        //Debug.Log ("OnApplicationPause " + pause);
        if (pause) {
            AwsModule.BattleData.PutBattleData ();
        }
    }

    #if UNITY_EDITOR
    void OnEnable()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    void OnDisable()
    {
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange change)
    {
        if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
            AwsModule.BattleData.PutBattleData ();
        }
    }
    #endif

    protected View_BattleMenu m_menu;
    protected List<ListItem_ActiveTimeIcon> m_listOrder = new List<ListItem_ActiveTimeIcon>();

    protected View_BattleEffect m_battleStartEffect;
    protected View_BattleEffect m_battleWaveCountEffect;
    protected View_BattleEffect m_battleWaveFinishEffect;

    protected View_ActiveTimeLine m_activeTimeLine;

    protected GameObject nowStandingImage;
}
