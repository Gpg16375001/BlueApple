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

        var presentBoxBadge = GetScript<RectTransform> ("BadgePresent");
        bool hasPresent = AwsModule.UserData.UserData.ReceivablePresentCount > 0;
        presentBoxBadge.gameObject.SetActive (hasPresent);
        if (hasPresent) {
            GetScript<TextMeshProUGUI> ("BadgePresent/txtp_Num").SetText(AwsModule.UserData.UserData.ReceivablePresentCount.ToString());
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

        View_LoginBonus loginbonusView = null;
        if (loginbonus != null && loginbonus.Length > 0) {
            // ログインボーナスの表示
            int nowCount = 0;
            loginbonusView = DispLoginbonus (loginbonus, nowCount);
        } else {
            LoginBonusAfterProc();
        }

		// お知らせ.
		if (AwsModule.ProgressData.CheckViewNotice()) {
			m_viewNotes = View_MyPageNotes.Create(false);
			m_viewNotes.gameObject.SetActive(false);
		}

        m_charaBasePos = this.GetScript<Transform>("CharacterAnchor").position;
        GetScript<uGUIPageScrollRect> ("ScrollBanner").gameObject.SetActive (false);
        bannerDatas = MasterDataTable.banner_setting.EnableData;
        bannerImages = new Dictionary<string, Sprite>();
        DLCManager.StartBannerDownload (bannerDatas.Select (x => x.image_path).ToArray (), DownloadBanner);

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            if (loginbonusView != null && !loginbonusView.IsLoaded ()) {
                StartCoroutine (WaitLoginbonusLoad (loginbonusView));
            } else {
                View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black,
                    () => {
                        if (loginbonusView != null) {
                            loginbonusView.Open ();
                        } else {
                            // 初回起動.
                            if (AwsModule.ProgressData.IsFirstBoot) {
                                var module = TutorialFirstBootModule.CreateIfMissing (TutorialFirstBootModule.ViewMode.StorySelect, this, subMenuRootObj.GetOrAddComponent<ViewBase> (), View_GlobalMenu.CreateIfMissing (), View_PlayerMenu.CreateIfMissing ());
                                module.LoadAndStartFirstScenario ();
                            } else {
                                // お知らせ.
                                if (m_viewNotes != null) {
                                    m_viewNotes.gameObject.SetActive (true);
                                }
                            }
                        }
                    });
            }
        });
    }

    IEnumerator WaitLoginbonusLoad(View_LoginBonus loginbonusView)
    {
        yield return new WaitUntil (() => loginbonusView.IsLoaded ());

        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black,
            () => {
                loginbonusView.Open ();
            }
        );
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

    BannerSetting[] bannerDatas;
    private Dictionary<string, Sprite> bannerImages;

    private void DownloadBanner(string imageName, Sprite sprite)
    {
        bannerImages [imageName] = sprite;
        if (bannerImages.Count >= bannerDatas.Length) {
            BannerCreate ();
        }
        // お知らせでも使う.
		if(m_viewNotes != null){
			m_viewNotes.UpdateBanner(imageName, sprite);
		}
    }

    private void SetBanner(int number, GameObject obj)
    {
        Sprite spt = null;
        bannerImages.TryGetValue (bannerDatas [number].image_path, out spt);
        obj.GetOrAddComponent<ListItem_Banner> ().UpdateItem (bannerDatas [number], spt);
    }

    private View_LoginBonus DispLoginbonus(LoginbonusData[] loginbonus, int count)
    {
        return View_LoginBonus.Create(loginbonus[count++],
            () => {
                if(loginbonus.Length > count) {
                    DispLoginbonus(loginbonus, count);
                } else {
                    loginbonus = null;

    				if(AwsModule.ProgressData.IsFirstBoot){
						var module = TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.StorySelect, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
						module.LoadAndStartFirstScenario();
    				}else{
					    // お知らせ.
					    if (m_viewNotes != null) {
                            m_viewNotes.gameObject.SetActive(true);
    					}
    				}
                    LoginBonusAfterProc(!AwsModule.ProgressData.IsFirstBoot);
                }
            }
        );
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
                    // 課金コールバックを追加しておく
                    PurchaseManager.SharedInstance.SucceedEvent += OnSucceed;
                    PurchaseManager.SharedInstance.ErrorEvent += OnError;

                    LockInputManager.SharedInstance.IsLock = true;
                    View_FadePanel.SharedInstance.IsLightLoading = true;
                    // 検証の終わっていないアイテムが存在するので検証を行う。
                    PurchaseManager.SharedInstance.ValidateForNonValdateTransaction (
                        () => {
                            // 課金コールバックを削除しておく
                            PurchaseManager.SharedInstance.SucceedEvent -= OnSucceed;
                            PurchaseManager.SharedInstance.ErrorEvent -= OnError;
                        }
                    );
                }
            );
        }
    }

    private void OnSucceed(SkuItem item)
    {
        View_FadePanel.SharedInstance.IsLightLoading = false;
        LockInputManager.SharedInstance.IsLock = false;
    }

    private void OnError(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item)
    {
        View_FadePanel.SharedInstance.IsLightLoading = false;
        LockInputManager.SharedInstance.IsLock = false;
    }
 
    private void WillOpenUserProfile()
	{
        isOpenUserProfile = true;
		subMenuRootObj.SetActive(false);
	}
	private void DidCloseUserProfile(bool bClose, CardData card)
	{
		if(card != null){
			this.RequestMainModel();
		}
        isOpenUserProfile = false;
		subMenuRootObj.SetActive(bClose);    
	}
    
    // ホームメインキャラモデルリクエスト.
    private void RequestMainModel()
    {
		this.StopCoroutine("CoRepeatPlayVoice");
		subMenuRootObj.SetActive(true);      
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
			this.StartCoroutine("CoRepeatPlayVoice");
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
        m_viewNotes = View_MyPageNotes.Create(true, () => {
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
		this.StartCoroutine("CoRepeatPlayVoice");
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
	private Live2dVoicePlayer PlayStartRandomVoice()
	{
		var baseRarity = MasterDataTable.card.DataList.Where(c => c.id == AwsModule.UserData.MainCard.CardId)
                                                      .Select(c => c.rarity)
                                                      .Min();
        var sheet = AwsModule.UserData.MainCard.Card.voice_sheet_name;
        if(!string.IsNullOrEmpty(sheet)){
            var cue = this.GetSoundCue(baseRarity);         
            var rootObj = this.GetScript<RectTransform>("CharacterAnchor").gameObject;
            var obj = rootObj.GetChildren()[0];
            var player = obj.GetOrAddComponent<Live2dVoicePlayer>();
            player.Play(sheet, cue);
			return player;
        }
		return null;
	}
	private SoundVoiceCueEnum GetSoundCue(int rarity)
	{
		var now = GameTime.SharedInstance.Now;
		var list = new List<SoundVoiceCueEnum>();
        
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
    
	IEnumerator CoRepeatPlayVoice()
	{
		if(AwsModule.ProgressData.IsFirstBoot){
			yield break;
		}
		while(true){
			m_voicePlayer = this.PlayStartRandomVoice();
			do {
				yield return null;
			} while (m_voicePlayer.IsPlayingVoice);
			var interval = UnityEngine.Random.Range(INTERVAL_PLAY_VOICE_MIN, INTERVAL_PLAY_VOICE_MAX);
			yield return new WaitForSeconds(interval);         
		}
	}

    bool IsOpenViews()
    {
        return m_viewNotes != null || m_optionTopPop != null || m_mirrativTop != null || isOpenUserProfile;
    }

	private const float INTERVAL_PLAY_VOICE_MIN = 10f;
	private const float INTERVAL_PLAY_VOICE_MAX = 20f;
	private Live2dVoicePlayer m_voicePlayer;

    void Awake()
    {
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }
 
	private static GameObject subMenuRootObj;
    private Vector3 m_charaBasePos;
    private bool isOpenUserProfile;
	private View_MyPageNotes m_viewNotes;
    private View_OptionTopPop m_optionTopPop;
    private View_MirrativTop m_mirrativTop;
}
