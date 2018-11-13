using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;
using TMPro;


/// <summary>
/// View : 武器OKのみのポップ.
/// </summary>
public class View_SaleOKPop : PopupViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_SaleOKPop Create(string name, int price, Action didTapOk)
    {
		var go = GameObjectEx.LoadAndCreateObject("Sale/View_SaleOKPop");
        var c = go.GetOrAddComponent<View_SaleOKPop>();
		c.InitInternal(name, price, didTapOk);
        return c;
    }
	private void InitInternal(string name, int price, Action didTapOk)
	{
		m_didTapOk = didTapOk;
        this.GetScript<TextMeshProUGUI>("txtp_SaleSingleItemName").SetText (name);
        this.GetScript<TextMeshProUGUI>("txtp_Price").text = price.ToString("#,0");
		this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapOK);
        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapOK ();
    }

	#region ButtonDelegate.
	// OKボタン
    void DidTapOK()
	{
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            if (m_didTapOk != null) {
                m_didTapOk();
            }
            this.Dispose();
        });
	}

    #endregion   

	private Action m_didTapOk;
}
