using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;

using TMPro;

using SmileLab;
using SmileLab.Net.API;
using SmileLab.UI;

/// <summary>
/// View : Player情報メニュー.
/// </summary>
public class View_PlayerMenu : ViewBase
{
    /// <summary>
    /// 表示中？
    /// </summary>
    public static bool IsVisible 
    {
        get { return instance != null ? instance.gameObject.activeSelf : false; }
        set { if(instance != null) instance.gameObject.SetActive(value); }
    }

    /// <summary>
    /// Event : メニュー側でボタンロックを掛ける際に追従してロックしたいViewがあればこちらに登録しておく.
    /// シーン切り替え時にリセットされる点に注意.
    /// </summary>
    public static event Action<bool/*bActive*/> DidSetEnableButton;

    /// <summary>
    /// Event : 戻るボタンタップ時.
    /// シーン切り替え時にリセットされる点に注意.
    /// </summary>
    public static event Action DidTapBackButton;

    /// <summary>
	/// Event : プロフィール画面展開直前のイベント.
    /// </summary>
	public static event Action WillOpenProfile;
    /// <summary>
	/// Event : プロフィール画面閉じ後のイベント.ホームキャラクター切り替え時はカードデータ付与.
    /// </summary>
	public static event Action<bool> DidCloseUserProfile;

    /// <summary>
    /// 自身の全ボタンの有効/無効設定.
    /// </summary>
    public static bool IsEnableButtons 
    { 
        set {
            if(DidSetEnableButton != null){
                DidSetEnableButton(value);
            }
            instance.IsEnableButton = value; 
        } 
    }

    /// <summary>
    /// マイページ内でBackボタンを使いたい場合.
    /// </summary>
    public static bool SetActiveBackButton
	{
		set {
			if (instance.m_BackButton != null) {
				instance.m_BackButton.gameObject.SetActive(value);
				instance.m_HomeCharacter.gameObject.SetActive(!value);
				instance.m_HomeSubMenu.gameObject.SetActive(!value);
            }
		}
	}
        
    /// <summary>
    /// 生成メソッド.
    /// </summary>
    public static View_PlayerMenu CreateIfMissing()
    {
        if(instance != null){
            return instance;
        }
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_PlayerMenu");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        instance = go.GetOrAddComponent<View_PlayerMenu>();
        instance.InitInternal();
        DontDestroyOnLoad(instance.gameObject);

        return instance;
    }

	public static void Setup(bool? isDisp=null)
	{
		if( instance != null ) {
			if( isDisp == null ) {
				messageGemFlow = null;
				instance.GetScript<RectTransform>("bt_FirstTimeDiscount").gameObject.SetActive( false );
				View_GemShop.CheckDiscount( ( message ) => {
					messageGemFlow = message;
					if( !string.IsNullOrEmpty(messageGemFlow) ) {
						instance.SetCanvasCustomButtonMsg("bt_FirstTimeDiscount", instance.DidTapGemShop, true, true);
						instance.GetScript<RectTransform>("bt_FirstTimeDiscount").gameObject.SetActive( true );
						instance.GetScript<TextMeshProUGUI>("bt_FirstTimeDiscount/txtp_Discount").text = messageGemFlow;
					}
				} );
			}else{
				instance.GetScript<RectTransform>("bt_FirstTimeDiscount").gameObject.SetActive( (bool)isDisp && !string.IsNullOrEmpty(messageGemFlow) );
			}
		}
	}

	private static string messageGemFlow;
	public static string MessageGemFlow {
		get {
			return (instance == null) ? null : messageGemFlow;
		}
	}

    /// <summary>
    /// Updates the view.
    /// </summary>
    public void UpdateView(UserData userData)
    {
        prevActionPoint = userData.ActionPoint;
        GetScript<TextMeshProUGUI>("txtp_AP").SetText (userData.ActionPoint);

        var exp = userData.Exp;
        int level = MasterDataTable.user_level.GetLevel(exp);

        GetScript<TextMeshProUGUI>("txtp_APTotal").SetText (MasterDataTable.user_level.GetMaxAp (level));
        GetScript<TextMeshProUGUI>("txtp_PlayerLevel").SetText (level);
        GetScript<TextMeshProUGUI>("txtp_PlayerNextLevel").SetText (MasterDataTable.user_level.GetNextLevelExp(exp));
		GetScript<Image>("ExpBar/img_ExpBar").fillAmount = MasterDataTable.user_level.GetCurrentLevelProgress(exp);
        GetScript<TextMeshProUGUI>("txtp_Gem").SetText (userData.GemCount);
        GetScript<TextMeshProUGUI>("txtp_Coin").SetText (userData.GoldCount);
    }

    // 内部初期化.
    private void InitInternal()
    {
        // TODO : 実値が決まって来たら色々とここで実装.
        m_BackButton = GetScript<CustomButton>("bt_Back");
        m_BackButton.gameObject.SetActive(false);
        m_OtherBg = GetScript<RectTransform> ("Other").gameObject;
        m_OtherBg.SetActive (false);
        m_PageTitleImages = GetScript<Transform> ("PageTitle").GetComponentsInChildren<Image>(true);
        m_HomeBg = GetScript<RectTransform> ("img_PlayerMenuBase").gameObject;
        m_HomeBg.SetActive (true);
		m_HomeCharacter = GetScript<RectTransform>("HomeCharacter");
		m_HomeSubMenu = GetScript<RectTransform>("LeftButton");
		SetCanvasCustomButtonMsg("bt_Back", DidTapBack);
		this.SetCanvasCustomButtonMsg("bt_HomeCharacter", DidTapCharacter);
		this.SetCanvasCustomButtonMsg("Coin/img_PlayerHeaderFrame", DidTapCoinShop);
        this.SetCanvasCustomButtonMsg("Gem/img_PlayerHeaderFrame", DidTapGemShop);
        this.SetCanvasCustomButtonMsg ("AP/img_PlayerHeaderFrame", DidTapAPHeal);      

		m_HomeSubMenu.gameObject.SetActive( true );
		this.SetCanvasCustomButtonMsg("LeftButton/bt_CharaChange", DidTapCharacterChange);
		this.SetCanvasCustomButtonMsg("LeftButton/bt_Community", DidTapCommunity);
        this.SetCanvasCustomButtonMsg("LeftButton/bt_ItemList", DidItemList);

        UpdateView (AwsModule.UserData.UserData);

        ScreenChanger.WillChangeScene += WillChangeScene;  // リブート時は破棄.
        ScreenChanger.DidEndChangeScene += DidChangeScene;      

        AwsModule.UserData.UpdateUserData += UpdateView;

        AndroidBackButton.SetEventInThisScene (DidBackButton, false);
    }

    public override void Dispose ()
    {
        AwsModule.UserData.UpdateUserData -= UpdateView;
        ScreenChanger.WillChangeScene -= WillChangeScene;  // リブート時は破棄.
        ScreenChanger.DidEndChangeScene -= DidChangeScene;    
        DidSetEnableButton = null;
        DidTapBackButton = null;
        WillOpenProfile = null;
        DidCloseUserProfile = null;
        base.Dispose ();
    }

    void WillChangeScene(string sceneName)
    {
        if(sceneName == "boot") {  
            this.Dispose();
            return;
        }

        if (m_BackButton != null) {
            bool isMypage = sceneName == "MyPage";
            m_BackButton.gameObject.SetActive (!isMypage);
            m_OtherBg.SetActive (!isMypage);
            if (!isMypage) {
                var pageTitle = string.Format ("img_PageTitle{0}", sceneName);
                System.Array.ForEach (m_PageTitleImages, (x) => x.gameObject.SetActive(x.name == pageTitle));
            }
            m_HomeCharacter.gameObject.SetActive(isMypage);
            m_HomeBg.gameObject.SetActive(isMypage);
			m_HomeSubMenu.gameObject.SetActive(isMypage);
        }
    }
    // コールバック：シーン切り替え直後
    void DidChangeScene(string sceneName)
    {
        // シーン切り替えごとにイベント類はリセット.
        DidSetEnableButton = null;
        DidTapBackButton = null;
		WillOpenProfile = null;
        DidCloseUserProfile = null;

		//ジェム購入促しの表示
		if( sceneName == "MyPage" || sceneName == "Shop" ) {
			Setup( true );
		}else{
			Setup( false );
		}

        AndroidBackButton.SetEventInThisScene (DidBackButton, false);
    }

    void DidBackButton()
    {
        if (m_BackButton != null && m_BackButton.isActiveAndEnabled && m_BackButton.interactable) {
            DidTapBack ();
        }
    }
    // ボタン : 戻るボタン押下.
    void DidTapBack()
    {
        if (DidTapBackButton != null) {
            DidTapBackButton ();
        }
    }
	// ボタン : キャラクターアイコンタップ.マイページのみ.
    void DidTapCharacter()
	{
        OpenPlayerInfoPop ();
	}
    public void OpenPlayerInfoPop()
    {
        if(m_infoPop != null){
            return;
        }
        if(WillOpenProfile != null){
            WillOpenProfile();
        }
        m_infoPop = View_PlayerInfoPop.Create((bEnd) => {
            if(DidCloseUserProfile != null) {
                DidCloseUserProfile(bEnd);
            }
        });
    }

	void DidTapCharacterChange()
	{
		View_PlayerInfoPop.CharacterChange( null, () => ScreenChanger.SharedInstance.GoToMyPage() );
	}

	void DidTapCommunity()
	{
        if(m_communityPop != null){
            return;
        }
        m_communityPop = View_CommunityPop.Create();
	}
        
    void DidItemList()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (
            View_FadePanel.FadeColor.Black, 
            () => {
                ScreenChanger.SharedInstance.GoToItemList ();
            }
        );
    }

	private View_PlayerInfoPop m_infoPop;
	private View_CommunityPop m_communityPop;

    // ボタン：クレドショップ
    void DidTapCoinShop()
	{
		View_CoinShop.Create();
	}

    public void DidTapGemShop()
    {
        // 年齢確認Popup
        View_GemShop.OpenGemShop();
    }

    void DidTapAPHeal()
    {
        View_APRecovery.Create ();
    }

    void Update()
    {
        int nowAP = AwsModule.UserData.ActionPoint;
        if (prevActionPoint != nowAP) {
            GetScript<TextMeshProUGUI>("txtp_AP").SetText (nowAP);
            prevActionPoint = nowAP;
        }
    }
 
	private CustomButton m_BackButton;
	private RectTransform m_HomeCharacter;
	private RectTransform m_HomeSubMenu;
    private GameObject m_OtherBg;
    private GameObject m_HomeBg;
    private Image[] m_PageTitleImages;
    int prevActionPoint = 0;
    private static View_PlayerMenu instance = null;
    public static View_PlayerMenu Instance { get { return instance; } }
}
