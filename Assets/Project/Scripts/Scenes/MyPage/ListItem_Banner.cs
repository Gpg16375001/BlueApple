using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
public class ListItem_Banner : ViewBase {

    private bool m_IsInit = false;
	public void InitItem(System.Action didTapBanner = null)
    {
		m_didTapBanner = didTapBanner;
        GetComponent<CustomButton> ().onClick.AddListener (TabBanner);
        m_IsInit = true;
    }
	public void UpdateItem(BannerSetting bannerData, Sprite bannerImage, System.Action didTapBanner = null)
    {
        if (!m_IsInit) {
			InitItem (didTapBanner);
        }
        m_BannerData = bannerData;
        gameObject.GetComponent<Image>().overrideSprite = bannerImage;
    }

    void TabBanner()
    {
        if (!m_BannerData.Enable ()) {
            PopupManager.OpenPopupOK ("有効でありません");
            return;
        }
        m_BannerData.transition.Transition (m_BannerData.transition_detail);
		if(m_didTapBanner != null){
			m_didTapBanner();
		}
    }

	private System.Action m_didTapBanner;
    BannerSetting m_BannerData;
}
