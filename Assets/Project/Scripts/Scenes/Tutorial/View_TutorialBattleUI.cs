using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using TMPro;
using BattleLogic;


/// <summary>
/// View : チュートリアルバトルUI.
/// </summary>
public class View_TutorialBattleUI : Screen_BattleUI
{
	/// <summary>ステータス表示更新の度呼ばれるイベント..</summary>
	public event Action DidUpdateViewStatus;
	
	/// <summary>
	/// 初期化.
	/// </summary>
	public void Init(ref ActionOrder order)
    {
        m_ActionOrder = order;
        m_waveEffect = new TutorialBattleWaveEffectManager();      
		m_activeTimeLine = this.GetScript<RectTransform>("GridCountTimeLine").gameObject.GetOrAddComponent<View_ActiveTimeLine>();
        m_activeTimeLine.Init(ref order);
		m_activeTimeLine.Reset();

		this.GetScript<TextMeshProUGUI>("txtp_BattleCount").text = "1 / 1";
		this.GetScript<TextMeshProUGUI>("txtp_TreasureCount").text = "0";      

        var SpCommandButton = GetScript<CustomButton> ("bt_SPCommand");
		SpCommandButton.onClick.AddListener(OnSPCommandClick);

        var AttackCommandButton = GetScript<CustomButton> ("bt_AttackCommand");
        AttackCommandButton.m_EnableLongPress = true;
        AttackCommandButton.onClick.AddListener(OnAttackCommandClick);
    }

    /// <summary>
    /// 指定したボタンだけアクティブにする.
    /// </summary>
    public void SetActiveCustomButtonThisOnly(string btnName)
	{
		foreach (var b in this.gameObject.GetComponentsInChildren<Button>(true)) {
			b.interactable = false;
        }
        foreach (var b in this.gameObject.GetComponentsInChildren<CustomButton>(true)) {
			b.interactable = false;
        }

		if(!string.IsNullOrEmpty(btnName)){
			CustomButton btn;
            if (btnName == "bt_Command") {
                btn = this.GetScript<ListItem_BattleCommand>("ListItem_CommandCharaSkill(Clone)").GetScript<CustomButton>(btnName);
            } else {
                btn = this.GetScript<CustomButton>(btnName);
            }
            btn.enabled = btn.interactable = true;
		}      
	}

    /// <summary>
    /// バトル開始.
    /// </summary>
    public void BattleOpen(Action didStart)
	{
		// システム暗幕開ける(即時).
        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, null, true);
        LockInputManager.SharedInstance.IsLock = true;
        // バトル開始フェードを開ける.
		m_waveEffect.PlayStart(() => {
            // バトル共通UI類を表示.
            this.PlayInOutUI(true, () => {
                LockInputManager.SharedInstance.IsLock = false;
				didStart();
            });
        });
	}

    /// <summary>
    /// 味方ユニットの行動選択.
    /// </summary>
	public void SelectAllyAction(ListItem_BattleUnit selector, Action<SkillParameter> didAction)
	{
		m_didPerAction = didAction;
		this.PlayAnimeUnitUI(UnitUIAnimState.In, selector);
	}

    /// <summary>
    /// 味方ユニット攻撃時の情報表示UIアニメーション更新.
    /// </summary>
    public void UpdateUnitUIAnimation(UnitUIAnimState state, ListItem_BattleUnit selector, SkillParameter action, Action didEnd)
	{
        this.PlayAnimeUnitUI(state, selector, action, didEnd);
	}

    /// <summary>
    /// 攻撃後のタイムライン更新.
    /// </summary>
    public void UpdateTimeLineAfterAttack()
	{
		m_activeTimeLine.DidAttackPreUpdate();
	}
    /// <summary>
    /// 次の攻撃者確定時のタイムライン更新.
    /// </summary>
	public void UpdateTimeLineDecidedNext(IActionOrderItem item, int index, bool isAdd = false)
	{
		// 状態などが追加される場合の処理
        if (isAdd) {
            m_activeTimeLine.Insert(item, index);
            return;
        } else {
            m_activeTimeLine.Replace(item, 0, index);
        }
        m_activeTimeLine.ResetOrder();
	}
    
	/// <summary>敵再ターゲッティング時.</summary>
	public void RetargetEnemyFromAttacker(ListItem_BattleUnit attacker)
    {
		if (attacker == null || attacker.Target == null) {
            return;
        }
		if (attacker.Target.IsPlayer) {
            return; // プレイヤーに対するターゲッティングはここでは対応しない.
        }
    }

    /// <summary>
    /// ファイナルWave再生.
    /// </summary>
    public void PlayFinalWave(Action didAnimationEnd)
	{
		m_waveEffect.PlayFinish(didAnimationEnd);
	}

    private void PlayInOutUI(bool bIn, Action didEnd = null)
    {
        if (bIn) {
            var unit = m_ActionOrder.Peek () as ListItem_BattleUnit;
            if (unit != null && unit.IsPlayer && !AwsModule.BattleData.IsAuto) {
                BattleScheduler.Instance.AddSchedule(PlayUIAndUnitUIIn(unit));
                if (didEnd != null) {
                    BattleScheduler.Instance.AddAction (didEnd);
                }
                return;
            }
        }

        var aName = bIn ? "HudIn" : "HudOut";
        BattleScheduler.Instance.AddSchedule(PlayAnimation("Hud", aName));
        if (didEnd != null) {
            BattleScheduler.Instance.AddAction (didEnd);
        }
    }

	protected override void UpdateViewStatus(ListItem_BattleUnit unit, bool createCommand)
	{
		base.UpdateViewStatus(unit, createCommand);
		if(unit.IsPlayer){
			this.StartCoroutine(this.DelayInvokeDidUpdateStatus()); // スキルコマンド生成待ち
		}      
	}
	IEnumerator DelayInvokeDidUpdateStatus()
	{
		yield return new WaitForSeconds(0.1f);
		if (DidUpdateViewStatus != null) {
            DidUpdateViewStatus();
        }
	}
 
	protected override void OnAttackCommandClick ()
    {
        didTabActionStart (m_Invoker.Parameter.NormalSkill);
    }
	protected override void OnSPCommandClick()
	{ 
		//didTabActionStart(m_Invoker.Parameter.SpecialSkill);

        LockInputManager.SharedInstance.IsLock = true;
        PlayAnimeUnitUI (UnitUIAnimState.TurnEnd, m_Invoker, m_Invoker.Parameter.SpecialSkill);
        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.IsLock = false;
            m_didPerAction(m_Invoker.Parameter.SpecialSkill);
        });
	}

    protected override void didTabActionStart(SkillParameter action)
    {
        if (AwsModule.BattleData.IsAuto) {
            return;
        }
        // 多重入力
        LockInputManager.SharedInstance.IsLock = true;

		View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.None);
        this.PlayAnimeUnitUI(UnitUIAnimState.MoveSide, null, action);
        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.IsLock = false;
            m_didPerAction(action);
        });
    }

    private ActionOrder m_ActionOrder;
    private TutorialBattleWaveEffectManager m_waveEffect;
    private Action<SkillParameter> m_didPerAction;  // 行動選択ごとのコールバック.
}