using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Rendering;


/// <summary>
/// Screen : マイページ.
/// </summary>
public class Screen_MyPage : ViewBase
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(LoginbonusData[] loginbonus)
    {
		SetCanvasCustomButtonMsg ("bt_SubMenuPresent", DidTapPresent);
		SetCanvasCustomButtonMsg ("bt_SubMenuNews", DidTapNotice);
		SetCanvasCustomButtonMsg ("bt_SubMenuMission", DidTapMission);
		SetCanvasCustomButtonMsg ("bt_SubMenuSetting", DidTapSettings);
		SetCanvasCustomButtonMsg ("bt_MainUnit", DidTapMainUnit);
		SetCanvasCustomButtonMsg ("Mirrativ/bt_Sub", DidTapMirrative);
		GetScript<RectTransform>("Mirrativ").gameObject.SetActive( false );	//コミュメニューに統合した

        var presentBoxBadge = GetScript<RectTransform> ("BadgePresent");
        bool hasPresent = AwsModule.UserData.UserData.ReceivablePresentCount > 0;
        presentBoxBadge.gameObject.SetActive (hasPresent);
        if (hasPresent) {
            GetScript<TextMeshProUGUI> ("BadgePresent/txtp_Num").SetText(AwsModule.UserData.UserData.ReceivablePresentCount.ToString());
        }

		if (AwsModule.ProgressData.TutorialStageNum == -1) {
			View_GlobalMenu.Setup();	//ガチャ誘導
			View_PlayerMenu.Setup();	//ジェム誘導
		}

		var missionBadge = GetScript<RectTransform>("BadgeMission");
		bool hasMission = AwsModule.UserData.UserData.ReceivableMissionCount > 0;
		missionBadge.gameObject.SetActive(hasMission);
		if(hasMission) {
			GetScript<TextMeshProUGUI>("BadgeMission/txtp_Num").SetText(AwsModule.UserData.UserData.ReceivableMissionCount);
		}

        var newsBadge = GetScript<RectTransform> ("BadgeNews");
        newsBadge.gameObject.SetActive (false);
        
        // ホームのサブメニューはグローバルメニューより上に表示したいとのこと.
        subMenuRootObj = this.GetScript<RectTransform>("MyPageSubMenu").gameObject;

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_GlobalMenu.DidTapMenuEvent += DidTapGlobalMenuOpenClose;
		View_PlayerMenu.WillOpenProfile += WillOpenUserProfile;
		View_PlayerMenu.DidCloseUserProfile += DidCloseUserProfile;
        isOpenUserProfile = false;

        m_charaBasePos = this.GetScript<Transform>("CharacterAnchor").position;
        GetScript<uGUIPageScrollRect> ("ScrollBanner").gameObject.SetActive (false);
        bannerDatas = MasterDataTable.banner_setting.EnableData;
        bannerImages = new Dictionary<string, Sprite>();
        downloadCount = 0;
        DLCManager.StartBannerDownload (bannerDatas.Select (x => x.image_path).ToArray (), DownloadBanner);
   
        // ログインボーナス系の完了後の処理
        Action endCallback = () => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black,
                () => {
                    // 初回起動.
                    if (AwsModule.ProgressData.IsFirstBoot) {
                        var module = TutorialFirstBootModule.CreateIfMissing (TutorialFirstBootModule.ViewMode.StorySelect, this, subMenuRootObj.GetOrAddComponent<ViewBase> (), View_GlobalMenu.CreateIfMissing (), View_PlayerMenu.CreateIfMissing ());
                        module.LoadAndStartFirstScenario ();
                    } else {
                        // お知らせ.
                        if (AwsModule.ProgressData.CheckViewNotice()) {
                            m_viewNotes = View_MyPageNotes.Create(bannerImages);
                            m_viewNotes.gameObject.SetActive(false);
                        }
                        StartCoroutine( displayNotes() );
                        LoginBonusAfterProc();
                    }
                }
            );
        };

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            StartCoroutine (DispLoginBonus (loginbonus, endCallback));
        });
    }

    IEnumerator DispLoginBonus(LoginbonusData[] loginbonus, Action endCallback)
    {
        if (loginbonus != null && loginbonus.Length > 0) {
            int count = 0;
            while (loginbonus.Length > count) {
                View_LoginBonus viewLoginBonus = null;
                // ログインボーナスの作成
                StartCoroutine(
                    View_LoginBonus.Create(loginbonus[count],
                        (view) => {
                            viewLoginBonus = view;
                        }
                    )
                );
                // ABロード等GameObjectのロードが完了するまで待機
                while (viewLoginBonus == null) {
                    yield return null;
                }
                // 全部のロードが完了するまで待機
                while (!viewLoginBonus.IsLoaded ()) {
                    yield return null;
                }
                // ログインボーナスをオープンする
                bool isOpen = true;
                viewLoginBonus.Open(
                    () => {
                        count++;
                        isOpen = false;
                    }
                );
                // ボードを開いている間待機
                while (isOpen) {
                    yield return null;
                }
            }
        }

        if (endCallback != null) {
            endCallback ();
        }    
    }

    IEnumerator displayNotes()
	{
		IsReady = false;

		if (m_viewNotes != null) {
			m_viewNotes.gameObject.SetActive (true);

			//閉じられるまで待つ
			while( true ) {
				if( m_viewNotes == null || !m_viewNotes.gameObject.activeSelf ) break;
				yield return null;
			}
		}

		if (PurchaseManager.SharedInstance.ExistNonValidateTransaction ()) {
			IsReady = true;
			yield break;
		}

		if( ScreenChanger.SharedInstance.PrevSceneName == "Title" ) {
			var categories = new CommonNoticeCategoryEnum[] { CommonNoticeCategoryEnum.Note, CommonNoticeCategoryEnum.Update, CommonNoticeCategoryEnum.Bug };
			int count = 0;
			List<CommonNotice> infoList = new List<CommonNotice>();
			Array.ForEach( categories, category => {
				var list = MasterDataTable.notice.GetListThisPlatform(category);
				list.RemoveAll( cn => !cn.IsPopupEnable );
				list.RemoveAll( cn => {
					if( (cn.popup_option == CommonNoticePopupEnum.OneTime) && AwsModule.NotesModifiedData.IsNew(cn) )
						return false;
					else if( (cn.popup_option == CommonNoticePopupEnum.EveryTime) )
						return false;
					return true;
				} );
				infoList.AddRange( list );
				count++;
			} );
			while( categories.Length != count ) {
				yield return null;
			}
			infoList = infoList.OrderBy( cn => cn.category ).OrderBy( cn => cn.priority ).ToList();
			count = 0;
			foreach( var info in infoList ) {
				if( count >= MasterDataTable.CommonDefine.GetValue( "POPUP_NOTICE_MAX", 99 ) ) break;
				var view = ListItem_MyPageNotes.DisplayItem( info );
				while( view != null ) {
					yield return null;
				}
				count++;
			}
		}

		IsReady = true;
	}

    private void BannerCreate ()
    {
        var scrollBanner = GetScript<uGUIPageScrollRect> ("ScrollBanner");
        scrollBanner.gameObject.SetActive (true);
        var infiniteGrid = scrollBanner.content.gameObject.GetComponent<InfiniteGridLayoutGroup> ();

        var ListItemBanner = Resources.Load ("MyPage/ListItem_Banner") as GameObject;

        infiniteGrid.OnUpdateItemEvent.AddListener(SetBanner);
        infiniteGrid.Initialize (ListItemBanner, 3, bannerDatas.Length, true);

        var bannerGridLed = GetScript<RectTransform> ("BannerLedGrid").gameObject;
        for (int i = 0; i < bannerDatas.Length; ++i) {
            GameObjectEx.LoadAndCreateObject ("MyPage/ListItem_BannerLED", bannerGridLed);
        }
        scrollBanner.SetInfinit(true, bannerDatas.Length);
        scrollBanner.Init ();

        scrollBanner.RotationInterval = 3.0f;
    }

    int downloadCount = 0;
    BannerSetting[] bannerDatas;
    private Dictionary<string, Sprite> bannerImages;

    private void DownloadBanner(string imageName, Sprite sprite)
    {
		bannerImages.Add( imageName, sprite );
        downloadCount++;
        if (downloadCount >= bannerDatas.Length) {
            BannerCreate ();
        }
    }

    private void SetBanner(int number, GameObject obj)
    {
        Sprite spt = null;
        bannerImages.TryGetValue (bannerDatas [number].image_path, out spt);
        obj.GetOrAddComponent<ListItem_Banner> ().UpdateItem (bannerDatas [number], spt);
    }

    private void LoginBonusAfterProc(bool checkPruchased = true)
    {
        // チュートリアル中はやらない
        if(AwsModule.ProgressData.TutorialStageNum < 0){
            this.RequestMainModel();
        }
        if (checkPruchased) {
            //ログインボーナスの表示が終わったら課金処理の終わっていないのものがないか確認してみる。
            ChechPruchasedItem ();
        }
    }

    public override void Dispose ()
    {
        // バナーのDLCは止める
        DLCManager.StopBannerDownload ();

		// ボイス停止.
		this.StopRandomVoice();

        base.Dispose ();
    }
    private void ChechPruchasedItem()
    {
        if (PurchaseManager.SharedInstance.ExistNonValidateTransaction ()) {
            PopupManager.OpenPopupSystemOK ("課金の処理が終了していないアイテムが見つかりました。課金処理を続行します。",
                () => {
                    LockInputManager.SharedInstance.IsLock = true;
                    View_FadePanel.SharedInstance.IsLightLoading = true;
                    // 検証の終わっていないアイテムが存在するので検証を行う。
                    PurchaseManager.SharedInstance.ValidateForNonValdateTransaction (
                        () => {
                            View_FadePanel.SharedInstance.IsLightLoading = false;
                            LockInputManager.SharedInstance.IsLock = false;
                        }
                    );
                }
            );
        }
    }
 
    private void WillOpenUserProfile()
	{
        isOpenUserProfile = true;
		subMenuRootObj.SetActive(false);
	}
	private void DidCloseUserProfile(bool bClose)
	{
        isOpenUserProfile = false;
		subMenuRootObj.SetActive(bClose);    
	}
    
    // ホームメインキャラモデルリクエスト.
    private void RequestMainModel()
    {
		this.StopCoroutine("CoRepeatPlayVoice");
        subMenuRootObj.SetActive(!isOpenUserProfile);      
        View_FadePanel.SharedInstance.IsLightLoading = true;
		this.GetScript<Transform>("CharacterAnchor").gameObject.DestroyChildren();
		var loader = new UnitResourceLoader(AwsModule.UserData.MainCard);
        loader.LoadResource(resouce => { 
            var go = Instantiate(resouce.Live2DModel) as GameObject;
			go.transform.SetParent(this.GetScript<Transform>("CharacterAnchor"));
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			var cubismRender = go.GetComponentsInChildren<CubismRenderController>()[0];
            if (cubismRender != null) {
				var rootCanvas = this.GetScript<Canvas>("CharacterAnchor");
                cubismRender.gameObject.SetLayerRecursively(rootCanvas.gameObject.layer);
                cubismRender.SortingLayer = rootCanvas.sortingLayerName;
                cubismRender.SortingOrder = rootCanvas.sortingOrder;
            }
            View_FadePanel.SharedInstance.IsLightLoading = false;         
			this.StartCoroutine("CoRepeatPlayVoice", true);
        });
    }

    #region ButtonDelegate.

    // プレゼント画面への遷移
    void DidTapPresent()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, 
            () => {
                ScreenChanger.SharedInstance.GoToPresent ();
            });
    }

    // お知らせ画面への遷移
    void DidTapNotice()
    {
        if (IsOpenViews ()) {
            return;
        }

        View_PlayerMenu.IsEnableButtons = false;
        this.IsEnableButton = false;
        m_viewNotes = View_MyPageNotes.Create(bannerImages, () => {
            View_PlayerMenu.IsEnableButtons = true;
            this.IsEnableButton = true;
        });
    }

    // ミッション画面への遷移
    void DidTapMission()
    {
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMission());
    }

    // 設定画面への遷移
    void DidTapSettings()
    {
        if (IsOpenViews ()) {
            return;
        }
        View_PlayerMenu.IsEnableButtons = false;
        this.IsEnableButton = false;
        m_optionTopPop = View_OptionTopPop.Create(() => {
            View_PlayerMenu.IsEnableButtons = true;
            this.IsEnableButton = true;
        });
    }

    // ボタン : グローバルメニューの開閉ボタン.
    void DidTapGlobalMenuOpenClose(bool bOpen)
    {
        // TODO : 表示キャラクターを真ん中寄りに.
		var target = this.GetScript<RectTransform>("CharacterAnchor").gameObject;
		var target2 = this.GetScript<RectTransform>("bt_MainUnit").gameObject;
        if(bOpen){
            iTween.MoveTo(target, m_charaBasePos, 0.2f);
			iTween.MoveTo(target2, m_charaBasePos, 0.2f);
        }else{
            var moveHash = new Hashtable();
            moveHash.Add("x", 0f);
            moveHash.Add("islocal", true);
            moveHash.Add("time", 0.3f);
            iTween.MoveTo(target, moveHash);
			iTween.MoveTo(target2, moveHash);
        }
    }

	// ボタン : メインキャラクタータップ.
    void DidTapMainUnit()
	{
		if(m_voicePlayer != null && m_voicePlayer.IsPlayingVoice){
			return;
		}
		this.StopRandomVoice(); 
		this.StopCoroutine("CoRepeatPlayVoice");
		this.StartCoroutine("CoRepeatPlayVoice", false);
	}

	// ボタン : ミラティブ展開.
    void DidTapMirrative()
	{
        if (IsOpenViews ()) {
            return;
        }
        View_PlayerMenu.IsEnableButtons = false;
        this.IsEnableButton = false;
        m_mirrativTop = View_MirrativTop.Create(() => {
            View_PlayerMenu.IsEnableButtons = true;
            this.IsEnableButton = true;
        });
	}

    // ランダムボイス再生.
	private Live2dVoicePlayer PlayStartRandomVoice( bool first=false )
	{
		var baseRarity = MasterDataTable.card.DataList.Where(c => c.id == AwsModule.UserData.MainCard.CardId)
                                                      .Select(c => c.rarity)
                                                      .Min();
        var sheet = AwsModule.UserData.MainCard.Card.voice_sheet_name;
        if(!string.IsNullOrEmpty(sheet)){
            var cue = this.GetSoundCue(baseRarity, first);         
            var rootObj = this.GetScript<RectTransform>("CharacterAnchor").gameObject;
            var obj = rootObj.GetChildren()[0];
            var player = obj.GetOrAddComponent<Live2dVoicePlayer>();
            player.Play(sheet, cue);
			return player;
        }
		return null;
	}
	private SoundVoiceCueEnum GetSoundCue(int rarity, bool first=false)
	{
		var now = GameTime.SharedInstance.Now;
		var list = new List<SoundVoiceCueEnum>();

		if( first ) {
			list.AddRange( GetSeasonVoice( now ) );
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

        // APとBP最大.
		if(AwsModule.UserData.ActionPointTimeToFull <= 0){
			list.Add(SoundVoiceCueEnum.ap_max);
		}
		if(AwsModule.UserData.BattlePointTimeToFull <= 0){
			list.Add(SoundVoiceCueEnum.bp_max);
		}
		if(!m_bListenedRetentionVoice){
			m_bListenedRetentionVoice = true;
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

        // 高レア
		if(rarity >= 3){
			// デフォルトセット再生確認.
			list.Add(SoundVoiceCueEnum.normal1);
			list.Add(SoundVoiceCueEnum.normal2);
			list.Add(SoundVoiceCueEnum.normal3);
			list.Add(SoundVoiceCueEnum.normal4);
			list.Add(SoundVoiceCueEnum.normal5);
			list.Add(SoundVoiceCueEnum.normal6);
            list.Add(SoundVoiceCueEnum.normal7);
			// 深夜
			if (now.Hour >= 23 || (now.Hour >= 0 && now.Hour <= 4)){
				list.Add(SoundVoiceCueEnum.midnight);
			}
			// 朝
			if(now.Hour >= 5 && now.Hour <= 8){
				list.Add(SoundVoiceCueEnum.morning);
			}
            // 午前
			if(now.Hour >= 9 && now.Hour <= 11){
				list.Add(SoundVoiceCueEnum.am);
			}
            // 昼
            if (now.Hour >= 12 && now.Hour < 13) {
				list.Add(SoundVoiceCueEnum.noon);
            }
            // 午後.
			if(now.Hour >= 13 && now.Hour <= 15){
				list.Add(SoundVoiceCueEnum.pm);
			}
            // 夕方
			if(now.Hour >= 16 && now.Hour < 19){
				list.Add(SoundVoiceCueEnum.evening);
			}
			// 夜
			if(now.Hour > 19 && now.Hour <= 22){
				list.Add(SoundVoiceCueEnum.night);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		// デフォルトセット再生確認.      
		list.Add(SoundVoiceCueEnum.normal1);
		list.Add(SoundVoiceCueEnum.normal2);
		list.Add(SoundVoiceCueEnum.normal3);
		// 低レアは午前、午後、夕方のボイスを持っているとのこと
		// 午前
        if (now.Hour >= 9 && now.Hour <= 11) {
            list.Add(SoundVoiceCueEnum.am);
        }
		// 午後
        if (now.Hour >= 12 && now.Hour < 13) {
            list.Add(SoundVoiceCueEnum.pm);
        }
		// 夕方
        if (now.Hour >= 16 && now.Hour < 19) {
            list.Add(SoundVoiceCueEnum.evening);
        }      
		return list[UnityEngine.Random.Range(0, list.Count)];
	}
	private bool m_bListenedRetentionVoice = false;

	List<SoundVoiceCueEnum> GetSeasonVoice( DateTime now )
	{
		var dict = new Dictionary<CommonSeasonEnum,SoundVoiceCueEnum>() {
			{ CommonSeasonEnum.NewYear, SoundVoiceCueEnum.new_year },
			{ CommonSeasonEnum.Valentine, SoundVoiceCueEnum.valentine },
			{ CommonSeasonEnum.Halloween, SoundVoiceCueEnum.halloween },
			{ CommonSeasonEnum.Christmas, SoundVoiceCueEnum.christmas },
			{ CommonSeasonEnum.EndOfYear, SoundVoiceCueEnum.end_of_year },
		};

		var list = new List<SoundVoiceCueEnum>();
		if( MasterDataTable.season != null ) {
			var type = MasterDataTable.season.GetSeason( now );
			if( dict.ContainsKey( type ) ) {
				list.Add( dict[type] );
			}
		}
		return list;
	}

    private void StopRandomVoice()
	{
		var rootObj = this.GetScript<RectTransform>("CharacterAnchor").gameObject;
		if(rootObj.transform.childCount <= 0){
			return;
		}
        var obj = rootObj.GetChildren()[0];
        var player = obj.GetOrAddComponent<Live2dVoicePlayer>();
		player.Stop();
	}

	#endregion
    
	IEnumerator CoRepeatPlayVoice(bool first)
	{
		if(AwsModule.ProgressData.IsFirstBoot){
			yield break;
		}
		while(true){
			m_voicePlayer = this.PlayStartRandomVoice( first );
			first = false;
			do {
				yield return null;
			} while (m_voicePlayer.IsPlayingVoice);
			var interval = UnityEngine.Random.Range(INTERVAL_PLAY_VOICE_MIN, INTERVAL_PLAY_VOICE_MAX);
			yield return new WaitForSeconds(interval);         
		}
	}

	public void OpenMirrativ()
	{
		DidTapMirrative();
	}

    bool IsOpenViews()
    {
        return (m_viewNotes != null || m_optionTopPop != null || m_mirrativTop != null || isOpenUserProfile) || !IsReady;
    }

	private const float INTERVAL_PLAY_VOICE_MIN = 10f;
	private const float INTERVAL_PLAY_VOICE_MAX = 20f;
	private Live2dVoicePlayer m_voicePlayer;

    void Awake()
    {
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
		instance = this;
    }
 
	void OnDestory()
	{
		instance = null;
	}

	private static GameObject subMenuRootObj;
    private static Screen_MyPage instance = null;
	public static Screen_MyPage Instance { get { return instance; } }
    private Vector3 m_charaBasePos;
    private bool isOpenUserProfile;
	private View_MyPageNotes m_viewNotes;
    private View_OptionTopPop m_optionTopPop;
    private View_MirrativTop m_mirrativTop;
    public bool IsReady { get; private set; }
}
