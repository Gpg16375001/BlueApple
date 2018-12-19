using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_PartyEditAutoSelect : PopupViewBase
{
	public static View_PartyEditAutoSelect Create(Screen_PartyEdit parent, Action didClose = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("PartyEdit/View_PartyEditAutoSelectPop");
		var c = go.GetOrAddComponent<View_PartyEditAutoSelect>();
		c.InitInternal(parent, didClose);
		return c;
	}
	private void InitInternal(Screen_PartyEdit parent, Action didClose)
	{
		m_parent = parent;

		this.SetCanvasCustomButtonMsg("Cancel/bt_Common", DidTapClose);
		this.SetCanvasCustomButtonMsg("OK/bt_Common", DidTapOk);

		//初期トグル
		m_toggleGrp = GetScript<ToggleGroup>("WeaponGrid");
		var toggles = m_toggleGrp.transform.GetComponentsInChildren<Toggle>();
		if( toggles != null && toggles.Length > 0 ) {
			if( s_saveIdx < 0 || s_saveIdx >= toggles.Length ) {
				s_saveIdx = 0;
			}
			toggles[s_saveIdx].isOn = true;
			toggles[s_saveIdx].Select();
		}
	}

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

	// ボタン : 閉じる.
	void DidTapClose()
	{
		this.PlayOpenCloseAnimation(false, () => {
            base.Dispose();
        });
    }

	void DidTapOk()
	{
		var elements = new ElementEnum[] { 0, ElementEnum.fire, ElementEnum.water, ElementEnum.wind, ElementEnum.soil, ElementEnum.light, ElementEnum.dark };

		if( m_toggleGrp.AnyTogglesOn() ) {
			s_saveIdx = -1;
			var toggles = m_toggleGrp.transform.GetComponentsInChildren<Toggle>();
			var i = Array.FindIndex( toggles, t => t == m_toggleGrp.ActiveToggles().First() );
			if( i >= 0 && i < elements.Length ) {
				m_parent.TeamClear();
				s_saveIdx = i;
			}

			this.PlayOpenCloseAnimation(false, () => {
				base.Dispose();
				if( s_saveIdx >= 0 ) {
					setAutoParty( elements[s_saveIdx] );
				}
			});
		}
    }

	void setAutoParty( ElementEnum elementType=(ElementEnum)0 )
	{
		var cardDatasList = new List<List<CardData>>();
		if( elementType == 0 ) {
			cardDatasList.Add( CardData.CacheGetAll() );
		}else{
			cardDatasList.Add( CardData.CacheGetAll().FindAll( c => c.Card.element.Enum == elementType ) );
			var weakList = MasterDataTable.element_affinity_setting.GetWeakElements( elementType );
			if( (weakList != null) && (weakList.Count > 0) ) {
				cardDatasList.Add( CardData.CacheGetAll().FindAll( c => (c.Card.element.Enum != elementType) && c.Card.element.Enum != weakList[0] ) );
				cardDatasList.Add( CardData.CacheGetAll().FindAll( c => c.Card.element.Enum == weakList[0] ) );
			}else{
				cardDatasList.Add( CardData.CacheGetAll().FindAll( c => c.Card.element.Enum != elementType ) );
			}
		}
		int i = 1;
		cardDatasList.ForEach( cardDatas => {
			foreach( var cardData in cardDatas.OrderByDescending( x => x.Parameter.Combat ) ) {
				if( i > m_parent.EditParty.Count ) break;
				m_parent.EditParty[i++] = cardData;
			}
		} );

		m_parent.UpdatePartyUnit( m_parent.EditParty );
		// ボタンなどの再設定をする
		m_parent.ChangePartyEditMode();
	}

	public static Dictionary<ElementEnum,List<CardData>> GetElementTypeDict()
	{
		var elements = new List<ElementEnum>() { ElementEnum.fire, ElementEnum.water, ElementEnum.wind, ElementEnum.soil, ElementEnum.light, ElementEnum.dark };
		return elements.ToDictionary( element => element, element => CardData.CacheGetAll().FindAll( c => c.Card.element.Enum == element ) );
	}

	public static void Reset()
	{
		s_saveIdx = 0;
	}

	ToggleGroup m_toggleGrp;
	Screen_PartyEdit m_parent;
	static int s_saveIdx = 0;
}