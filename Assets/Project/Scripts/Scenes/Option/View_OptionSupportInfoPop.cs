using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// View : サポート確認Pop.
/// </summary>
public class View_OptionSupportInfoPop : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
	public static void Create(UserData user, Action didClose = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionSupportInfoPop");
		var c = go.GetOrAddComponent<View_OptionSupportInfoPop>();
		c.InitInternal(user, didClose);
	}
	private void InitInternal(UserData user, Action didClose)
	{
		m_didClose = didClose;
		// ユニット.
		var elements = Enum.GetValues(typeof(ElementEnum)) as ElementEnum[];
		elements = elements.Where(e => e != ElementEnum.naught && e != ElementEnum.rainbow).ToArray();
		foreach(var element in elements){
			var num = (int)element;
            var card = user.SupporterCardList.FirstOrDefault(c => c.Card != null && c.Card.element.Enum == element);
			if(card == null){
				continue;
			}
			var anchor = this.GetScript<RectTransform>(num+"/UnitIcon");
			var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", anchor.gameObject);
            go.GetOrAddComponent<ListItem_UnitIcon>().Init(card.ConvertCardData(), ListItem_UnitIcon.DispStatusType.Level);
		}

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }
    
    // 閉じるボタン
    void DidTapClose()
	{
        PlayOpenCloseAnimation (false, () => {
            if(m_didClose != null) {
                m_didClose();
            }
            Dispose();
        });
	}

	private Action m_didClose;
}
