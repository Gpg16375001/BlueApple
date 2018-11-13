using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using TMPro;


/// <summary>
/// View : 武器上限達成時の警告ポップ.
/// </summary>
public class View_GachaArmouryPop : PopupViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_GachaArmouryPop Create(GachaClientUseData.ContentsForView.RowData data)
	{
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_Popup_GachaArmoury");
        var c = go.GetOrAddComponent<View_GachaArmouryPop>();
		c.InitInternal(data);
		return c;
	}
	private void InitInternal(GachaClientUseData.ContentsForView.RowData data)
	{
		this.GetScript<TextMeshProUGUI>("txtp_PopTitle").text = data.ContentsName;
		this.GetScript<CustomButton>("Ok/bt_Common").onClick.AddListener(DidTapOk);
		this.GetScript<CustomButton>("GoWeaponList/bt_Common").onClick.AddListener(DidTapWeaponList);      
		this.GetScript<CustomButton>("bt_Close").onClick.AddListener(DidTapOk);

        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapOk ();
    }

    // Ok.
    void DidTapOk()
	{
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, Dispose);
    }

    // 武器一覧へ
    void DidTapWeaponList()
	{
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToWeapon();
            });
            Dispose();
        });
	}
}
