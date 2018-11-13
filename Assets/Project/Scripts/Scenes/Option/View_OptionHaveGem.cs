using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// View : オプション画面 ジェム所持数.
/// </summary>
public class View_OptionHaveGem : View_OptionSingleTypeBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
	public override void Init(OptionBootMenu boot)
	{
		base.Init(boot);
		this.ChangeView(MasterDataTable.option_menu.DataList.Find(d => d.Enum == OptionMenuEnum.HaveGem));
		this.GetScript<TextMeshProUGUI>("txtp_TotalNum").text = AwsModule.UserData.UserData.GemCount.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_FreeGemNum").text = AwsModule.UserData.UserData.FreeGemCount.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_PayGemNum").text = AwsModule.UserData.UserData.PaidGemCount.ToString("#,0");
	}
}
