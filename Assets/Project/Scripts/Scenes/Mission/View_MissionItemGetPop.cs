using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// View : ミッションの受け取りポップ.
/// </summary>
public class View_MissionItemGetPop : PopupViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_MissionItemGetPop Create(List<MissionSetting> missions, Action didTapOK)
	{
		var go = GameObjectEx.LoadAndCreateObject("Mission/View_MissionItemGetPop");
        var c = go.GetOrAddComponent<View_MissionItemGetPop>();
		c.InitInternal(missions, didTapOK);
		return c;
	}
	private void InitInternal(List<MissionSetting> missions, Action didTapOK)
	{
		m_didTapOK = didTapOK;

		var text = "";
		foreach(var m in missions){
			text += "\n"+m.reward_item_type.Enum.GetNameAndQuantity(m.reward_item_id, m.reward_item_count);
		}
		this.GetScript<TextMeshProUGUI>("txtp_ItemList").text = text;

		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("OK/bt_Common", DidTapOK);
		this.SetCanvasCustomButtonMsg("Cancel/bt_Common", DidTapClose);
        SetBackButton ();
	}   

    protected override void DidBackButton ()
    {
        DidTapClose();
    }
	#region ButtonDelegate.

	// ボタン: 閉じる.
	void DidTapClose()
	{
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
	}   

	// ボタン: OK
    void DidTapOK()
	{
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            if(m_didTapOK != null) {
                m_didTapOK();
            }
            Dispose();
        });
	}

    #endregion

	private Action m_didTapOK;
}
