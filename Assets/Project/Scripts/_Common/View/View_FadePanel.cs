using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SmileLab;
using TMPro;

/// <summary>
/// 汎用フェード用パネル.
/// </summary>
public class View_FadePanel : ViewBase
{

	/// <summary>共有インスタンス.</summary>
	public static View_FadePanel SharedInstance { get; private set; }

    /// <summary>現在のフェード状況.</summary>
    public FadeState CurrentState { get; private set; }

    /// <summary>ページング間などの軽いローディング演出.</summary>
    public bool IsLightLoading
    {
        get {
            return this.GetScript<RectTransform>("LightLoadingEffect").gameObject.activeSelf;
        }
        set {
            this.GetScript<RectTransform>("LightLoadingEffect").gameObject.SetActive(value);
        }
    }

    [SerializeField]
    private GameObject loadingAnimItem; // ローティングアニメーションアイテム.あれば指定して使用.
	[SerializeField]
	private GameObject tipsObject;
    [SerializeField]
    private GameObject loadingGaugeObj;
    [SerializeField]
    private Image loadingGauge;


	/// <summary>
	/// 透明な状態から色付きに.
	/// </summary>
	public void FadeOut(FadeColor color, Action didEnd = null, bool bImmediate = false)
	{
        if(this.CurrentState == FadeState.FadeOut){
            if(didEnd != null){
                didEnd();
            }
            return;
        }

        this.CurrentState = FadeState.FadeOut;
		var spt = color == FadeColor.Black ? m_sptBlack: m_sptWhite;
		spt.gameObject.SetActive(true);
			
		if(bImmediate){
            m_sptBlack.color = Color.white;
			m_sptWhite.color = Color.white;
			if(didEnd != null){
				didEnd();
			}
			return;
		}

        if(spt.color.a >= 1f){
            if(loadingAnimItem != null) {
                loadingAnimItem.SetActive(false);
            }
            spt.color = new Color(1f, 1f, 1f, 0);
		}
		CoroutineAgent.Execute( this.FadeProc(spt, true, didEnd) ); // 立て続けに呼ばれる状況などを加味してコルーチンを追加していき順番に処理させたい為CroutineAgentを使用する.
	}
    /// <summary>
    /// ローディングアニメーションを再生しつつ、透明な状態から色付きに.
    /// </summary>
    public void FadeOutWithLoadingAnim(FadeColor color, Action didEnd = null, bool bImmediate = false)
    {      
        if(bImmediate) {
            if(loadingAnimItem != null){
                loadingAnimItem.SetActive(true);
            }
			if (tipsObject != null) {
                tipsObject.SetActive(true);
                SetTips ();
            }
            if (loadingGaugeObj != null) {
                loadingGaugeObj.SetActive (false);
            }
        }
        Action didEndEx = () => {
            if(loadingAnimItem != null) {
                loadingAnimItem.SetActive(true);
            }
			if (tipsObject != null) {
                tipsObject.SetActive(true);
                SetTips ();
            }
            if (loadingGaugeObj != null) {
                loadingGaugeObj.SetActive (false);
            }
            if(didEnd != null){
                didEnd();
            }
        };
        this.FadeOut(color, didEndEx, bImmediate);
    }
    /// <summary>
	/// アニメーションしつつTipsの表示を行わない.透明な状態から色付きに.
    /// </summary>
	public void FadeOutAnimAndWithoutTips(FadeColor color, Action didEnd = null, bool bImmediate = false)
	{
		if (bImmediate) {
			if (loadingAnimItem != null) {
                loadingAnimItem.SetActive(true);
            }
			if (tipsObject != null) {
				tipsObject.SetActive(false);
            }
            if (loadingGaugeObj != null) {
                loadingGaugeObj.SetActive (false);
            }
        }
		Action didEndEx = () => {
			if (loadingAnimItem != null) {
                loadingAnimItem.SetActive(true);
            }
            if (tipsObject != null) {
				tipsObject.SetActive(false);
            }
            if (loadingGaugeObj != null) {
                loadingGaugeObj.SetActive (false);
            }
            if (didEnd != null) {
                didEnd();
            }         
        };
		this.FadeOut(color, didEndEx, bImmediate);
	}

    public void SetProgress(float progress)
    {
        if (loadingGaugeObj != null) {
            if (!loadingGaugeObj.activeSelf) {
                loadingGaugeObj.SetActive (true);
            }
            if (loadingGauge != null && loadingGauge.isActiveAndEnabled) {
                loadingGauge.rectTransform.localScale = new Vector3 (progress, 1.0f, 1.0f);
            }
        }
    }

    public void DeativeProgress()
    {
        if (loadingGaugeObj != null && loadingGaugeObj.activeSelf) {
            loadingGaugeObj.SetActive (false);
        }
    }

	/// <summary>
	/// 色付きから透明に.
	/// </summary>
	public void FadeIn(FadeColor color, Action didEnd = null, bool bImmediate = false)
	{
        if(this.CurrentState == FadeState.FadeIn){
            if (didEnd != null) {
                didEnd();
            }
            return;
        }

        this.CurrentState = FadeState.FadeIn;
		var spt = color == FadeColor.Black ? m_sptBlack: m_sptWhite;
		spt.gameObject.SetActive(true);

        // 問答無用でローディングアニメーションを切る.
        if(loadingAnimItem != null) {
            loadingAnimItem.SetActive(false);
        }

		if(bImmediate){
            m_sptBlack.color = new Color(1f, 1f, 1f, 0);
            m_sptWhite.color = new Color(1f, 1f, 1f, 0);
			if(didEnd != null){
				didEnd();
			}
			m_sptBlack.gameObject.SetActive(false);
			m_sptWhite.gameObject.SetActive(false);
			return;
		}

        if(spt.color.a <= 0f){
            spt.color = Color.white;
		}
		CoroutineAgent.Execute( this.FadeProc(spt, false, didEnd) );
	}

	// フェード処理.
    private IEnumerator FadeProc(Image sprite, bool bFadeOut, Action didEnd)
	{
        LockInputManager.SharedInstance.IsLock = true;
        var alpha = sprite.color.a;
		var limitVal = bFadeOut ?  1f : 0f;
		var val = bFadeOut ? 5f: -5f;        
		var bLimit = false;
		while(!bLimit){
			alpha += val * Time.deltaTime;
			bLimit = bFadeOut ? alpha >= limitVal : alpha < limitVal;
			if(bLimit){
				alpha = limitVal;
			}
			// 透過色は同期.
            m_sptBlack.color = new Color(1f, 1f, 1f, alpha);
			m_sptWhite.color = new Color(1f, 1f, 1f, alpha);
			yield return null;
		}
		if(didEnd != null){
			didEnd();
		}
		// 透明な状態なら表示しない.
		if(!bFadeOut){
			m_sptBlack.gameObject.SetActive(false);
			m_sptWhite.gameObject.SetActive(false);
		}
        LockInputManager.SharedInstance.IsLock = false;
	}

    private void SetTips()
    {
        if (MasterDataTable.tips_setting != null) {
            var tips = MasterDataTable.tips_setting.GetRandom ();
            GetScript<TextMeshProUGUI> ("txtp_Tips").SetText (tips.text);
        }
    }

	void Awake()
	{
        if(SharedInstance != null) {
            SharedInstance.Dispose();
        }

        SharedInstance = this;

        m_sptBlack = this.GetScript<Image>("spt_black");
        m_sptWhite = this.GetScript<Image>("spt_white");

        // 透過状態からスタート.
        m_sptBlack.color = new Color(1f, 1f, 1f, 0);
		m_sptWhite.color = new Color(1f, 1f, 1f, 0);
		m_sptBlack.gameObject.SetActive(false);
		m_sptWhite.gameObject.SetActive(false);
        if(loadingAnimItem != null) {
            loadingAnimItem.SetActive(false);
        }
		if(tipsObject != null){
			tipsObject.SetActive(true);
		}
        if (loadingGaugeObj != null) {
            loadingGaugeObj.SetActive (false);
        }

        IsLightLoading = false;
        CurrentState = FadeState.None;
	}

	/// <summary>enum : フェード色.</summary>
	public enum FadeColor
	{
		Black,
		White,
	}
    /// <summary>enum : フェード状況.</summary>
    public enum FadeState
    {
        None,
        FadeIn,
        FadeOut,
    }

    private Image m_sptBlack;
    private Image m_sptWhite;
}
