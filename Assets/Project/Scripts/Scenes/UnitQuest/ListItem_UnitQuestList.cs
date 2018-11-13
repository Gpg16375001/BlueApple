using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// ListItem : 各キャラシナリオのルートリスト.
/// </summary>
public class ListItem_UnitQuestList : ViewBase
{   
    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(UnitQuest info, Action<UnitQuest> didTap)
	{
		m_info = info;
		m_didTap = didTap;

		var card = MasterDataTable.card[m_info.card_id];

		// ラベルとアイコン
		this.GetScript<TextMeshProUGUI>("txtp_UnitName").text = card.nickname;
		IconLoader.LoadCardIcon(m_info.card_id, (set, spt) => this.GetScript<Image>("IconChDummy").sprite = spt);   // アイコンはスプライトのみ表示.

        // 持ってない場合も表示するが鍵マーク表示とのこと.
		var exists = CardData.CacheGet(m_info.card_id) != null;
		this.GetScript<Image>("img_LockIcon").gameObject.SetActive(!exists);
		this.GetScript<Image>("img_UnitQuestIconDisable").gameObject.SetActive(!exists);
		this.GetScript<CustomButton>("bt_SmallList").interactable = exists;
		if(exists){
			// TODO : 解放数に応じてListItem_UnitQuestScenarioIconを生成.クリア数に応じて黄色に表示.

            // ボタン.
			this.SetCanvasCustomButtonMsg("bt_SmallList", DidTap);
		}
	}

    // ボタン：タップ
    void DidTap()
	{
		if(m_didTap != null){
			m_didTap(m_info);
		}
	}

	private UnitQuest m_info;
	private Action<UnitQuest> m_didTap;
}
