using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;

/// <summary>
/// ListItem : 単一あたりのバトルコマンド.
/// </summary>
public class ListItem_BattleCommand : ViewBase
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(BattleLogic.SkillParameter action, Action<BattleLogic.SkillParameter> didTapCommand/*TODO : バトルコマンド情報*/)
    {
        var animation = GetComponent<Animation> ();
        // TODO : とりあえずデータがないので通常攻撃として設定.
        m_didTapCommand = didTapCommand;
        m_action = action;

        // TODO : ラベルやTexture設定.
        GetScript<TextMeshProUGUI>("txtp_CommandName").SetText(m_action.Skill.display_name);

        EnableAction = action.Enabled;

        // ボタン設定
        var battleCommond = this.GetScript<CustomButton>("bt_Command");
        battleCommond.m_EnableLongPress = true;
        if (EnableAction) {
            battleCommond.onClick.AddListener (DidTapCommand);
        }
        battleCommond.onLongPress.AddListener(DidLongTap);
        battleCommond.onRelease.AddListener(DidButtonRelease);

        // ボタン押下状態の設定
        GetScript<RectTransform> ("WaitTurn").gameObject.SetActive (!EnableAction);
        GetScript<RectTransform> ("Gauge").gameObject.SetActive (!EnableAction);
        if (!EnableAction) {
            GetScript<TextMeshProUGUI> ("WaitTurn/txtp_Num").SetText (action.RemainChargeTime);
            GetScript<Image> ("img_CommandGauge").fillAmount = action.ChargeProgress;

            animation.clip = animation.GetClip ("SkillOffLoop");
            animation.Play ("SkillOffLoop");
        } else {
            animation.clip = animation.GetClip ("SkillOnLoop");
            animation.Play ("SkillOnLoop");
        }


    }

    public void FrameOut(BattleLogic.SkillParameter skill)
    {
        if (skill == m_action) {
            Used ();
        } else {
            FrameOut ();
        }
    }

    private void FrameOut()
    {
        var animation = GetComponent<Animation> ();
        if (EnableAction) {
            animation.Play ("SkillOnExit");
        } else {
            animation.Play ("SkillOffExit");
        }
    }

    private void Used()
    {
        var animation = GetComponent<Animation> ();
        animation.Play ("SkillOnUse");
    }

    #region ButtonDelegate.

    // EventTriggerボタン：自身をタップ.
    void DidTapCommand()
    {
        if (!EnableAction) {
            return;
        }
        if(m_didTapCommand != null){
            m_didTapCommand(m_action);
        }
    }

    // EventTriggerボタン : ロングタップ.
    void DidLongTap()
    {
		if(BattleProgressManager.Shared.IsInit){
			BattleProgressManager.Shared.DrawSkillDetail(m_action, true);
		}
    }

    // EventTriggerボタン : ボタン押下解除検知.
    void DidButtonRelease()
    {
		if (BattleProgressManager.Shared.IsInit) {
			BattleProgressManager.Shared.DrawSkillDetail(m_action, false);
		}
    }

    #endregion

    private Action<BattleLogic.SkillParameter> m_didTapCommand; // TODO : バトルコマンド情報を引数に取るようにする.
    private BattleLogic.SkillParameter m_action;
    private bool EnableAction;
}
