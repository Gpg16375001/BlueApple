using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ListItem : 受け取り履歴のリストアイテム.
/// </summary>
public class ListItem_Option_ReceiptHistory : ViewBase
{

    /// <summary>
    /// 情報更新.
    /// </summary>
	public void UpdateData(DistributionData data)
	{
		var itemType = (ItemTypeEnum)data.ItemType;
		this.GetScript<TextMeshProUGUI>("txtp_PresentName").text = itemType.GetName(data.ItemId);
        this.GetScript<TextMeshProUGUI>("txtp_Num").text = itemType == ItemTypeEnum.paid_gem || itemType == ItemTypeEnum.free_gem || itemType == ItemTypeEnum.money || itemType == ItemTypeEnum.event_point ? data.Quantity.ToString("#,0"): data.Quantity.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_PresentDetails").text = data.Description;
		this.GetScript<TextMeshProUGUI>("txtp_Date").text = string.Format("受け取り日時 {0}", DateTime.Parse(data.CreationDate, null, DateTimeStyles.RoundtripKind).ToString("yyyy/MM/dd HH:mm"));

	}
}
