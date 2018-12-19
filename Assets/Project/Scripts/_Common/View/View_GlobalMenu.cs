using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// View : グローバルメニュー.
/// </summary>
public class View_GlobalMenu : ViewBase
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
    /// Event : メニュー側でボタンロックを掛ける際に追従してロックしたいViewがあればこちらに登録しておく.
    /// シーン切り替え時にリセットされる点に注意.
    /// </summary>
    public static event Action<bool/*bActive*/> DidSetEnableButton;

    /// <summary>
    /// Event : メニューボタンタップ時.
    /// シーン切り替え時にリセットされる点に注意.
    /// </summary>
    public static event Action<bool/*bOpen*/> DidTapMenuEvent;

    /// <summary>
    /// グローバルメニューのボタンが押された時に
    /// </summary>
    public static event Action<Action> DidTapButton;



    public override void Dispose ()
    {
        ScreenChanger.WillChangeScene -= WillChangeScene;   // リブート時は破棄.
        ScreenChanger.DidEndChangeScene -= DidChangeScene;  // ボタン処理リセット.
        DidSetEnableButton = null;
        DidTapMenuEvent = null;
        DidTapButton = null;
        base.Dispose ();
    }
    /// <summary>
    /// 生成メソッド.
    /// </summary>
    public static View_GlobalMenu CreateIfMissing()
    {
        if(instance != null){
            return instance;
        }
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_GlobalMenu");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 1f);
        instance = go.GetOrAddComponent<View_GlobalMenu>();
        instance.InitInternal();
        DontDestroyOnLoad(instance.gameObject);
        return instance;
    }
    // 内部初期化.
    private void InitInternal()
    {
        m_anim = this.GetScript<Animation>("Global");

        ScreenChanger.WillChangeScene += WillChangeScene;   // リブート時は破棄.
        ScreenChanger.DidEndChangeScene += DidChangeScene;  // ボタン処理リセット.

        // ボタン設定
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuOpen", DidTapMenu);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuStory", DidTapStory);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuEvent", DidTapEvent);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuPvP", DidTapPvP);
		this.SetCanvasCustomButtonMsg("bt_GlobalMenuUnitStory", DidTapUnitQuest);      

        this.SetCanvasCustomButtonMsg("bt_GlobalMenuHome", DidTapHome);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuCharacter", DidTapCharacter);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuWeapon", DidTapWeapon);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuOrganization", DidTapPartyEdit);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuGacha", DidTapGacha);
        this.SetCanvasCustomButtonMsg("bt_GlobalMenuShop", DidTapShop);

        this.GetScript<RectTransform> ("bt_GlobalMenuHome").gameObject.SetActive (false);
        SetEventBalloon ();
    }
    // コールバック：シーン切り替え直前
    void WillChangeScene(string sceneName)
    {
        if(sceneName == "boot"){
            this.Dispose();
            return;
        }
        if(!IsVisible){
            return;
        }
        // シーンに応じて出ている状態からなのかしまっている状態からなのか別れる.
        m_bOpen = sceneName != "MyPage";
        this.GetScript<RectTransform> ("bt_GlobalMenuHome").gameObject.SetActive(sceneName != "MyPage");
		this.GetScript<CustomButton>("bt_GlobalMenuHome").interactable = sceneName != "MyPage";
        SetEventBalloon ();
        this.StartCoroutine(this.PlayOpenClose(true));
    }
    // コールバック：シーン切り替え直後
    void DidChangeScene(string sceneName)
    {
        // シーン切り替えごとにイベント類はリセット.
        DidSetEnableButton = null;  
        DidTapMenuEvent = null;
        DidTapButton = null;
    }

    void SetEventBalloon()
    {
        var now = GameTime.SharedInstance.Now;
        var display = MasterDataTable.event_quest != null && MasterDataTable.event_quest.DataList.Any (x => x.start_at <= now && x.end_at >= now);
        GetScript<RectTransform> ("Balloon").gameObject.SetActive (display);
    }

	public static void Setup( GachaClientUseData data=null )
	{
		Action<GachaClientUseData> proc = (_data) => {
			var value = (_data.WeaponContent.DataFree != null) && (_data.WeaponContent.DataFree.IsPurchasable);
			instance.GetScript<RectTransform>("bt_GlobalMenuGacha").transform.Find("Exclamation").gameObject.SetActive( value );

			//ガチャページのキャッシュ
			Screen_Gacha.s_GachaIdList.Clear();
			foreach (var g in _data.CategoryListInCharacter) {
				Screen_Gacha.s_GachaIdList.Add( g.index );
			}
		};

		if( instance != null ) {
			if( data == null ) {
				SendAPI.GachaGetProductList((bSuccess, res) => {
					if (bSuccess && res != null) {
						proc( new GachaClientUseData( res ) );
					}
				});
			}else{
				proc( data );
			}
		}
	}

    #region ButtonDelegate.

    // ボタン：メニューボタン押下.
    void DidTapMenu()
    {
        if(m_anim.isPlaying){
            return;
        }
        if(DidTapMenuEvent != null){
            DidTapMenuEvent(!m_bOpen);
        }
        this.StartCoroutine( this.PlayOpenClose() );
		this.GetScript<CustomButton>("bt_GlobalMenuOpen").SetClickSe(m_bOpen ? SoundClipName.se004 : SoundClipName.se005);
    }
    // ボタン：ストーリー.
    void DidTapStory()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapStoryExec);
            return;
        }
        DidTapStoryExec ();
    }
    void DidTapStoryExec()
    {
        IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToMainQuestSelect();
            IsEnableButtons = true;
        });
    }

    // ボタン：イベント.
    void DidTapEvent()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapEventExec);
            return;
        }
        DidTapEventExec ();
    }
    void DidTapEventExec()
    {
        IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToEvent();
            IsEnableButtons = true;
        });
    }
	// ボタン：PvP.
    void DidTapPvP()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapPvPExec);
            return;
        }
        DidTapPvPExec ();
    }
    void DidTapPvPExec()
    {
		//PopupManager.OpenPopupOK("PvPは未実装です。\nストーリーをお試しください。");
        IsEnableButtons = false;
        if(AwsModule.PartyData.PvPTeam.IsEmpty) {
            // PVPチーム編成に飛ばす
            PopupManager.OpenPopupOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                    IsEnableButtons = true;
                });
            });
        } else {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToPVP();
                IsEnableButtons = true;
            });
        }
    }
	// ボタン：キャラシナリオ
    void DidTapUnitQuest()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapUnitQuestExec);
            return;
        }
        DidTapUnitQuestExec ();
    }
    void DidTapUnitQuestExec()
	{
		IsEnableButtons = false;
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => { 
			ScreenChanger.SharedInstance.GoToUnitQuest();
			IsEnableButtons = true;	
		});
	}

    // ボタン：ホーム画面へ.
    void DidTapHome()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapHomeExec);
            return;
        }
        DidTapHomeExec ();
    }
    void DidTapHomeExec()
    {
        IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToMyPage();
            IsEnableButtons = true;
        });
    }
    // ボタン：キャラ一覧.
    void DidTapCharacter()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapCharacterExec);
            return;
        }
        DidTapCharacterExec ();
    }
    void DidTapCharacterExec()
    {
        IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToUnitList();
            IsEnableButtons = true;
        });
    }
    // ボタン：武器一覧.
    void DidTapWeapon()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapWeaponExec);
            return;
        }
        DidTapWeaponExec ();
    }
    void DidTapWeaponExec()
    {
		IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
			ScreenChanger.SharedInstance.GoToWeapon();
            IsEnableButtons = true;
        });
    }

    // ボタン：パーティ編成.
    void DidTapPartyEdit()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapPartyEditExec);
            return;
        }
        DidTapPartyEditExec ();
    }
    void DidTapPartyEditExec()
    {
        IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOut(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToPartyEditTop();
            IsEnableButtons = true;
        });
    }
    // ボタン：ガチャ.
    void DidTapGacha()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapGachaExec);
            return;
        }
        DidTapGachaExec ();
    }
    void DidTapGachaExec()
    {
		IsEnableButtons = false;
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => { 
			Screen_Gacha.Reset(); //初期化
			ScreenChanger.SharedInstance.GoToGacha();
            IsEnableButtons = true;
		});
    }
    // ボタン：ショップ.
    void DidTapShop()
    {
        if (DidTapButton != null) {
            DidTapButton (DidTapShopExec);
            return;
        }
        DidTapShopExec ();
    }
    void DidTapShopExec()
    {
		IsEnableButtons = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
			ScreenChanger.SharedInstance.GoToShop();
            IsEnableButtons = true;
        });
    }

    #endregion

    // アニメーション開閉処理.
    private IEnumerator PlayOpenClose(bool bImmediate = false)
    {
        LockInputManager.SharedInstance.IsLock = true;

        yield return null;  // ここで一フレ待たないとbImmediate時 & シーン切り替え時にボタン登録がうまくいかないことがある.
        var animName = m_bOpen ? "GlobalMenuClose": "GlobalMenuOpen"; 
        if(bImmediate){
            m_anim[animName].normalizedTime = 1f;
        }
        m_anim.Play(animName);
        do {
            yield return null;
        } while (m_anim.isPlaying);

        m_bOpen = !m_bOpen;

        LockInputManager.SharedInstance.IsLock = false;
    }

    private bool m_bOpen = true;        // 開閉状態.
    private Animation m_anim;

    private static View_GlobalMenu instance = null;
}


/// <summary>
/// enum : menuのタイプ.
/// </summary>
public enum GlobalMenuType
{
    CharaEdit,
    Equip,
    Gacha,
    Shop,
    Friend,
}