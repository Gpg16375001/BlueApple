using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab.Net.API;


/// <summary>
/// View : オプション画面 受け取り履歴.
/// </summary>
public class View_OptionReceiptHistory : View_OptionSingleTypeBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
	public override void Init(OptionBootMenu boot)
	{
		m_prefabListItem = Resources.Load("Option/ListItem_History") as GameObject;
		
		View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.DistributionGetHistory((bSuccess, res) => {
			if(!bSuccess || res == null){
				View_FadePanel.SharedInstance.IsLightLoading = false;
				return;
			}
			this.CreateList(res.DistributionDataList);
			base.Init(boot);
            this.ChangeView(MasterDataTable.option_menu.DataList.Find(d => d.Enum == OptionMenuEnum.History));
			View_FadePanel.SharedInstance.IsLightLoading = false;
		});      
	}

    // 履歴リスト生成
	private void CreateList(DistributionData[] datas)
	{
		m_dats = datas;

		var grid = this.GetScript<InfiniteGridLayoutGroup>("HistoryGrid");
        grid.OnUpdateItemEvent.RemoveAllListeners();
        grid.OnUpdateItemEvent.AddListener(CallbackUpdateListItem);
		grid.Initialize(m_prefabListItem, 5, m_dats.Length, false);
	}
	void CallbackUpdateListItem(int index, GameObject obj)
	{
		var d = m_dats[index];
		var c = obj.GetOrAddComponent<ListItem_Option_ReceiptHistory>();
		c.UpdateData(d);
	}

	private GameObject m_prefabListItem;
	private DistributionData[] m_dats;
}
