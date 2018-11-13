using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// 2つのOn/Offトグル式ボタン管理クラス(uGUI).
/// </summary>
public class TwoToggleButtonGroup : ViewBase
{
    // Onボタン
    [SerializeField]
    private CustomButton onButton;

    // Offボタン
    [SerializeField]
	private CustomButton offButton;

    /// <summary>ボタン押下時のイベント.Initごとにリセットされる.</summary>
    public event Action<bool/*bOn*/> DidTapButtonEvent;


    /// <summary>
    /// 初期化.同じ箇所で表示する切り替え式のトグルの場合はtrueを指定する.
    /// </summary>
    public void Init(bool bOn, bool bPairToggle = false)
    {
        DidTapButtonEvent = null;

		m_bPairType = bPairToggle;

        // Onの設定.
		onButton.onClick.RemoveAllListeners();

		if(m_bPairType){
			onButton.gameObject.SetActive(bOn);
			onButton.onClick.AddListener(DidTapOff);
		}else{
            if (m_sptOnNormal == null || m_sptOnHighlight == null) {
                m_sptOnNormal = onButton.image.sprite;
                m_sptOnHighlight = onButton.spriteState.highlightedSprite;
            }
            onButton.image.sprite = bOn ? m_sptOnHighlight : m_sptOnNormal;
			onButton.onClick.AddListener(DidTapOn);
		}      
        // Offの設定.
		offButton.onClick.RemoveAllListeners();
        
		if(m_bPairType){
			offButton.gameObject.SetActive(!bOn);
			offButton.onClick.AddListener(DidTapOn);
		}else{
            if (m_sptOffNormal == null || m_sptOffHighlight == null) {
                m_sptOffNormal = offButton.image.sprite;
                m_sptOffHighlight = offButton.spriteState.highlightedSprite;
            }
            offButton.image.sprite = !bOn ? m_sptOffHighlight : m_sptOffNormal;
			offButton.onClick.AddListener(DidTapOff);
		}
    }

    #region ButtonDelegate.

    // ボタン : Onボタンタップ.
    void DidTapOn()
    {
        // ボタン表示切り替え.
		if(m_bPairType){
			if(onButton.gameObject.activeSelf){
				return;
			}
			onButton.gameObject.SetActive(true);
			offButton.gameObject.SetActive(false);
		}else{
			onButton.image.sprite = m_sptOnHighlight;
            offButton.image.sprite = m_sptOffNormal;
		}
        if (DidTapButtonEvent != null){
            DidTapButtonEvent(true);
        }
    }
    // ボタン : Offボタンタップ.
    void DidTapOff()
    {
        // ボタン表示切り替え.
		if (m_bPairType) {
			if (offButton.gameObject.activeSelf) {
                return;
            }
            onButton.gameObject.SetActive(false);
			offButton.gameObject.SetActive(true);
        } else {
			onButton.image.sprite = m_sptOnNormal;
            offButton.image.sprite = m_sptOffHighlight;
        }
        if (DidTapButtonEvent != null){
            DidTapButtonEvent(false);
        }
    }

	#endregion.

	private bool m_bPairType;
    private Sprite m_sptOnNormal;
    private Sprite m_sptOnHighlight;
    private Sprite m_sptOffNormal;
    private Sprite m_sptOffHighlight;
}