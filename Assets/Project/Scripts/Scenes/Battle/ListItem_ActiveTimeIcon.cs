using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

using TMPro;

using SmileLab;


/// <summary>
/// ListItem : 行動順１つあたりのユニットアイコン.
/// </summary>
public class ListItem_ActiveTimeIcon : ViewBase
{
    /// <summary>
    /// パラメータ上のユニットID.
    /// </summary>
    public int UnitID { get { return m_unit != null ? m_unit.Parameter.ID : -1; } }

    /// <summary>
    /// TODO : Debug用.落ち着いたら消す.
    /// </summary>
    public string UnitName { get { return m_unit != null ? m_unit.Parameter.Name : ""; } }

    public Transform LineTarget { get { return GetScript<Transform> ("LineTarget"); } }
    /// <summary>
    /// 死んでる？
    /// </summary>
    public bool IsRemove { get { return m_ActionOrderItem.IsRemove; } }

    public IActionOrderItem Item { get { return m_ActionOrderItem; } }

    private bool _IsOrder;
    /// <summary>
    /// 自分の順番？
    /// </summary>
    public bool IsOrder
    {
        get {
            return _IsOrder;
        }
        set {
            if (_IsOrder != value) {
                if (value) {
                    var anim = this.GetComponent<Animation>();
                    anim.Play("Active");
                }
                _IsOrder = value;
            }
        }
    }

    /// <summary>
    /// ターゲットになってる？.
    /// </summary>
    public bool IsTarget
    {
        get {
            return this.GetScript<Animation>("Target").gameObject.activeSelf;
        }
        set {
            this.GetScript<Animation>("Target").gameObject.SetActive(value);
        }
    }

    void OnEnable()
    {
        if (_IsOrder) {
            var anim = this.GetComponent<Animation>();
            anim.Play("Active");
        }
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(IActionOrderItem item)
    {
        m_ActionOrderItem = item;
        m_conditionTiming = null;
        m_unit = null;
        if (item.ItemType == ActionOrderItemType.Unit) {
            m_unit = m_ActionOrderItem as ListItem_BattleUnit;
            // TODO : スプライト差し替え.とりあえず敵味方のみで判断.
            this.GetScript<Image> ("img_FrameCountTimePlayer").gameObject.SetActive (m_unit.IsPlayer);
            this.GetScript<Image> ("img_FrameCountTimeEnemy").gameObject.SetActive (!m_unit.IsPlayer);

            RequestIcon ();
            SetPositionNum ();

            var conditionIcon = this.GetScript<Image> ("Condition");
            conditionIcon.gameObject.SetActive (false);
        } else if (item.ItemType == ActionOrderItemType.Condition) {
            m_conditionTiming = m_ActionOrderItem as ConditionEffectTiming;
            m_unit = AwsModule.BattleData.GetBattleUnit (m_conditionTiming.unit.Position);

            this.GetScript<Image> ("img_FrameCountTimePlayer").gameObject.SetActive (m_unit.IsPlayer);
            this.GetScript<Image> ("img_FrameCountTimeEnemy").gameObject.SetActive (!m_unit.IsPlayer);

            RequestIcon ();
            SetPositionNum ();

            // 状態を設定する。
            var conditionIcon = this.GetScript<Image> ("Condition");
            conditionIcon.gameObject.SetActive (true);
            conditionIcon.sprite = IconLoader.LoadConditionIcon(m_conditionTiming.parent.ConditionData);
        }

        // ボタン.
        this.SetCanvasCustomButtonMsg ("img_FrameCountTimeEnemy", DidTapIcon);
        this.SetCanvasCustomButtonMsg ("img_FrameCountTimeBoss", DidTapIcon);
        //this.SetCanvasCustomButtonMsg ("img_FrameCountTimePlayer", DidTapIcon);
        //アルファの閾値(1だと不透明なピクセルのみ反応する)
        this.GetScript<Image> ("img_FrameCountTimeEnemy").alphaHitTestMinimumThreshold = 1.0f;
        this.GetScript<Image> ("img_FrameCountTimeBoss").alphaHitTestMinimumThreshold = 1.0f;
        //this.GetScript<Image> ("img_FrameCountTimePlayer").alphaHitTestMinimumThreshold = 1.0f;
    }

    // 通信リクエスト : アイコン差し替え.
    private void RequestIcon()
    {
        if(m_unit == null || !m_unit.IsPlayer){
            return;
        }

        if (m_unit.TimeLineIcon != null) {
            this.GetScript<Image>("img_FrameCountTimePlayer").sprite = m_unit.TimeLineIcon;
            return;
        }
    }

    static readonly char[,] PositionChar = new char[,] { {'A', 'B', 'C'}, {'D', 'E', 'F'}, {'G', 'H', 'I'}};
    private void SetPositionNum()
    {
        if(m_unit == null || m_unit.IsPlayer){
            return;
        }
        this.GetScript<TextMeshProUGUI> ("txtp_EnemyNum").SetText (PositionChar[m_unit.Parameter.Position.row, m_unit.Parameter.Position.column]);
    }

    /// <summary>
    /// 引数のユニットと同一かどうか.
    /// </summary>
    public bool ComareItem(IActionOrderItem item)
    {
        if (item.ItemType == m_ActionOrderItem.ItemType) {
            if (item.ItemType == ActionOrderItemType.Unit) {
                var unit = item as ListItem_BattleUnit;
                return m_unit != null && m_unit.IsPlayer == unit.IsPlayer && m_unit.Index == unit.Index && m_unit.Parameter.ID == unit.Parameter.ID;
            } else if (item.ItemType == ActionOrderItemType.Condition) {
                var conditionTiming = item as ConditionEffectTiming;
                return m_conditionTiming != null && m_conditionTiming.parent == conditionTiming.parent && m_conditionTiming.unit == conditionTiming.unit;
            }
        }
        return false;
    }

    /// <summary>リスト外に排出される際のアニメ.</summary>
    public void PlayOutAnimation(Action didEnd = null)
    {
        this.StartCoroutine(PlayAnimation("Out", didEnd));
    }
    /// <summary>リスト内の末尾より追加される際のアニメ.</summary>
    public void PlayInAnimation(Action didEnd = null)
    {
        this.StartCoroutine(PlayAnimation("In", didEnd));
    }
    /// <summary>リスト内に挿入される際のアニメ.</summary>
    public void PlayInsertAnimation(Action didEnd = null)
    {
        this.StartCoroutine(PlayAnimation("Interrupt", didEnd));
    }

    // アニメーション再生.
    private IEnumerator PlayAnimation(string animName, Action didEnd)
    {
        var anim = this.GetComponent<Animation>();
        anim.Play(animName);
        yield return null;
        while(anim.isPlaying){
            yield return null;
        }
        if(didEnd != null){
            didEnd();
        }
    }

#region ButtonDelegate

    // ボタン : アイコンタップ.該当ユニットにターゲッティングを行う.
    void DidTapIcon()
    {
        Debug.Log("TapActiveTimeIcon");
        if(m_ActionOrderItem.ItemType != ActionOrderItemType.Unit || m_unit == null || m_unit.IsPlayer){
            return; // TODO : プレイヤーキャラに対するターゲッティングはとりあえず無し.
        }
        BattleProgressManager.Shared.RetargetEnemy(m_unit);
    }

#endregion

    private IActionOrderItem m_ActionOrderItem;
    private ListItem_BattleUnit m_unit;
    private ConditionEffectTiming m_conditionTiming;
}
