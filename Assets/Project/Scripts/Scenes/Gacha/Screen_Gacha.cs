using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;


/// <summary>
/// Screen : ガチャ.
/// </summary>
public class Screen_Gacha : ViewBase
{
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(GachaClientUseData data)
	{
		m_data = data;

		View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
		View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += BackToMyPage;

		this.UpdateViewList();

		this.GetScript<uGUIPageScrollRect>("Scroll").onCenter += CallbackPagingOnCenter;
        
		this.GetScript<CustomButton>("bt_ArrowPage_1").onClick.AddListener(DidTapLeft);
		this.GetScript<CustomButton>("bt_ArrowPage_2").onClick.AddListener(DidTapRight);
		this.SetCanvasCustomButtonMsg("Character/bt_GachaCategory", DidTapCharaGacha);
		this.SetCanvasCustomButtonMsg("Weapon/bt_GachaCategory", DidTapWeaponGacha);      
		{
			var value = (m_data.WeaponContent.DataFree != null) && (m_data.WeaponContent.DataFree.IsPurchasable);
			this.GetScript<RectTransform>("Weapon/txtp_WeaponGacha").transform.Find("Exclamation").gameObject.SetActive( value );
		}

		DidTapCharaGacha();

#if false
		View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
#else
		m_isReady = false;
		StartCoroutine( coWaitInit() );
#endif
	}

	IEnumerator coWaitInit()
	{
		while( !m_isReady )
			yield return null;
		View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
	}

	// ガチャViewリスト作成.
	private void UpdateViewList()
	{
		// シングルView
        var weaponRoot = this.GetScript<RectTransform>("GachaWeapon").gameObject;
		weaponRoot.GetOrAddComponent<View_GachaList>().Init(m_data.WeaponContent, CallbackDidCloseGacha);

		// キャラガチャ内のカテゴリータブ.
        var rootTab = this.GetScript<HorizontalLayoutGroup>("Page").gameObject;
        rootTab.DestroyChildren();
        var idx = 0;
        foreach (var g in m_data.CategoryListInCharacter) {
            var go = GameObjectEx.LoadAndCreateObject("Gacha/ListItem_GachaPage", rootTab);
            var c = go.GetOrAddComponent<ListItem_GachaPage>();
            c.Init(g, CallbackDidTapCategory, idx >= (m_data.CategoryListInCharacter.Count - 1));
            ++idx;
        }

		// キャラガチャリストView.
		var rootList = this.GetScript<InfiniteGridLayoutGroup>("ViewportGacha/Content");
		rootList.OnUpdateItemEvent.AddListener(UpdateInfiniteListItem);
		rootList.gameObject.DestroyChildren();
		var prefab = Resources.Load("Gacha/View_GachaNormal") as GameObject;
		var pageMax = 3;
		rootList.Initialize(prefab, pageMax, m_data.CharacterGachaContents.Count, true);
		this.GetScript<uGUIPageScrollRect>("Scroll").SetInfinit(true, pageMax);

		// 初期選択.
#if false
		this.GetScript<uGUIPageScrollRect>("Scroll").CenterOn(rootList.GetComponentsInChildren<View_GachaList>(true)[0].gameObject);
		//var centerObj = this.GetScript<uGUIPageScrollRect>("Scroll").GetCurrentCenterObject(); // rootList.GetComponentsInChildren<View_GachaList>()[0].gameObject;
		//CallbackPagingOnCenter(centerObj);
#else
		StartCoroutine( coWaitUpdateView( rootList ) );
#endif
	}   

	IEnumerator coWaitUpdateView( InfiniteGridLayoutGroup rootList )
	{
		yield return null;	//InfiniteGridLayoutGroupのアップデートを待つ

		//ページ指定
		var currIdx = 0;
		var views = rootList.GetComponentsInChildren<View_GachaList>(true);
		if( s_PageIndex >= 0 && s_PageIndex < views.Length )
	 		currIdx = s_PageIndex;
		this.GetScript<uGUIPageScrollRect>("Scroll").CenterOn( views[currIdx].gameObject, true );

		//カテゴリ指定
		if( s_SubPageIndex == 0 )
			DidTapCharaGacha();
		else
			DidTapWeaponGacha();

		yield return null;
		m_isReady = true;
	}

	#region ButtonDelegate.

    // キャラガチャ.
    void DidTapCharaGacha()
	{
		this.GetScript<TextMeshProUGUI>("txtp_WeaponGacha_highlight").gameObject.SetActive(false);
		this.GetScript<TextMeshProUGUI>("txtp_WeaponGacha").gameObject.SetActive(true);
		this.GetScript<TextMeshProUGUI>("txtp_CharacterGacha").gameObject.SetActive(false);
		this.GetScript<TextMeshProUGUI>("txtp_CharacterGacha_highlight").gameObject.SetActive(true);
		this.GetScript<CustomButton>("Character/bt_GachaCategory").interactable = false;
		this.GetScript<CustomButton>("Weapon/bt_GachaCategory").interactable = true;
		this.GetScript<CustomButton>("Weapon/bt_GachaCategory").ForceHighlight = false;
		this.GetScript<CustomButton>("Character/bt_GachaCategory").ForceHighlight = true;      
		this.GetScript<RectTransform>("Contents/SingleView").gameObject.SetActive(false);
		this.GetScript<RectTransform>("Contents/TabView").gameObject.SetActive(true);
	}

    // 武器ガチャ
    void DidTapWeaponGacha()
	{      
		this.GetScript<TextMeshProUGUI>("txtp_CharacterGacha_highlight").gameObject.SetActive(false);
		this.GetScript<TextMeshProUGUI>("txtp_CharacterGacha").gameObject.SetActive(true);
        this.GetScript<TextMeshProUGUI>("txtp_WeaponGacha").gameObject.SetActive(false);
        this.GetScript<TextMeshProUGUI>("txtp_WeaponGacha_highlight").gameObject.SetActive(true);
		this.GetScript<CustomButton>("Weapon/bt_GachaCategory").interactable = false;
        this.GetScript<CustomButton>("Character/bt_GachaCategory").interactable = true;      
		this.GetScript<CustomButton>("Character/bt_GachaCategory").ForceHighlight = false;
		this.GetScript<CustomButton>("Weapon/bt_GachaCategory").ForceHighlight = true;
		this.GetScript<RectTransform>("Contents/TabView").gameObject.SetActive(false);
		this.GetScript<RectTransform>("Contents/SingleView").gameObject.SetActive(true);      
	}

    // 左スクロール
    void DidTapLeft()
	{
		this.GetScript<uGUIPageScrollRect>("Scroll").Paging(-1);
	}
	// 右スクロール
	void DidTapRight()
    {
		this.GetScript<uGUIPageScrollRect>("Scroll").Paging(1);
    }

    #endregion

	#region Callbacks.

	// コールバック：ページスクロールセンタリング時.
	void CallbackPagingOnCenter(GameObject centerObj)
	{
		var rootTab = this.GetScript<HorizontalLayoutGroup>("Page").gameObject;
		m_currentView = centerObj.GetComponent<View_GachaList>();
		rootTab.GetComponentsInChildren<ListItem_GachaPage>(true).Select(lst => lst.ForceHighlight = lst.Gacha.index == m_currentView.Gacha.index).ToList();
	}

	// コールバック : 無限スクロール常にリストアイテムを生成した際のコールバック.
	//index = m_data.CharacterGachaContents[]のインデックス
	void UpdateInfiniteListItem(int index, GameObject createObj)
	{
		//Debug.Log("Screen_Gacha:UpdateInfiniteListItem() index="+index+", createObj="+createObj); //createObj=更新view
		var oldGacha = createObj.GetComponent<View_GachaList>() ? createObj.GetComponent<View_GachaList>().Gacha : null;
		var c = createObj.GetOrAddComponent<View_GachaList>();      
		c.Init(m_data.CharacterGachaContents[index], CallbackDidCloseGacha);

		if( oldGacha == null ) {
			var item = this.GetComponentsInChildren<ListItem_GachaPage>(true).FirstOrDefault(lst => lst.Gacha.index == m_data.CharacterGachaContents[index].Gacha.index);
			if(item != null){
				if(item.LinkViewObj == null) {
					item.LinkViewObj = createObj;
				}
			}
		}else{
			var item = Array.Find( this.GetComponentsInChildren<ListItem_GachaPage>(true), i => i.Gacha == c.Gacha );
			if( item != null ) {
				item.LinkViewObj = createObj;
			}
			if( oldGacha != c.Gacha ) {
				item = Array.Find( this.GetComponentsInChildren<ListItem_GachaPage>(true), i => i.Gacha == oldGacha );
				if( item != null ) {
#if true
					var scroll = this.GetScript<uGUIPageScrollRect>("Scroll");
					var views = scroll.GetComponentsInChildren<View_GachaList>( true );
					var view = Array.Find( views, v => v.Gacha == item.Gacha );
					item.LinkViewObj = (view != null) ? view.gameObject : null;
#else
					item.LinkViewObj = null;
#endif
				}
			}
		}
		return;

	}

	// コールバック : ガチャカテゴリを押した.
	void CallbackDidTapCategory(GameObject targetViewObj, ListItem_GachaPage item)
	{
		var scroll = this.GetScript<uGUIPageScrollRect>("Scroll");
		if( (scroll == null) || scroll.IsBusy )
			return;
		if( targetViewObj == null ) {
			//強制ページ送り
			scroll.Paging( -1 );
			//Debug.Log("DispTransformIndex="+scroll.DispTransformIndex);
			var views = scroll.GetComponentsInChildren<View_GachaList>( true );
			var view = Array.Find( views, v => v.gameObject.name == scroll.DispTransformIndex.ToString() );
			targetViewObj = (view != null) ? view.gameObject : null;
			//Debug.Log(" > targetViewObj="+targetViewObj);
		}
		if (targetViewObj == m_currentView.gameObject){
			return;
		}

		//itemから再取得
		int idx = m_data.CharacterGachaContents.FindIndex( cfv => cfv.Gacha == item.Gacha );
		//Debug.Log(" > idx="+idx);
		if( idx < 0 )
			return;
		// InfiniteGridLayoutGroup側の都合でView_GachaListの更新がされないことがあるため強制呼び出し.
		UpdateInfiniteListItem(idx, targetViewObj);

		scroll.CenterOn(targetViewObj);
		m_currentView = targetViewObj.GetComponent<View_GachaList>();      
		this.gameObject.GetComponentsInChildren<ListItem_GachaPage>(true).Select(lst => lst.ForceHighlight = lst.LinkViewObj == targetViewObj).ToList();
	}
   
	// コールバック : ガチャページを閉じた際.
	private void CallbackDidCloseGacha(GachaClientUseData.ContentsForView.RowData row, ReceiveGachaPurchaseProduct response)   
    {
		if(row == null || response == null){
			return; // 単純に閉じた.
		}
		View_GlobalMenu.IsVisible = false;
		View_PlayerMenu.IsVisible = false;
		this.GetScript<RectTransform>("Contents").gameObject.SetActive(false);

		Action didClose = () => {
			View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
				ScreenChanger.SharedInstance.GoToGacha();
			});
        };
        Action oneMoreProc = () => {
			if( m_currentView == null ) {
				//武器ガチャ引いて戻った時、m_currentViewが初期化されたままになるので再検索
				var views = GetComponentsInChildren<View_GachaList>( true );
				m_currentView = Array.Find( views, v => v.ID == row.ID );
			}
           m_currentView.Draw(row , true);
        };

		// ガチャを引いた.
		m_data.UpdateInfo(response);
		this.UpdateViewList();

		// キャラ喋る演出.
		if(row.Type.Enum == GachaTypeEnum.character_gacha){
			var list = new List<int>();
			list.AddRange(response.AcquiredGachaItemDataList.Where(a => a.IsNew).Select(a => a.ItemId).ToList());
			if(response.RarestCardGachaItemData != null && response.RarestCardGachaItemData.IsNew){
				list.Add(response.RarestCardGachaItemData.ItemId);            
			}
			list = list.Distinct().ToList();
			Debug.Log("自己紹介予定人数:" + list.Count + "人");
			this.PlayCharacterIntroAdv(list, () => {});
		}

		View_GachaResult.Create(row, response, didClose, oneMoreProc);
    }

	#endregion   

    // 自己紹介
    private void PlayCharacterIntroAdv(List<int> cardList, Action didEnd)
	{
		if(cardList == null || cardList.Count <= 0){
			didEnd();
			return;
		}      
		this.StartCoroutine(CoPlayCharacterIntroAdv(cardList, didEnd));
	}
	private IEnumerator CoPlayCharacterIntroAdv(List<int> cardIdList, Action didEnd)
	{
		var bg = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaCharacterIntro");
        bg.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);

		UtageModule.SharedInstance.SetActiveCore(true);
		var bPlayScenario = false;
		foreach(var cardId in cardIdList){
			bPlayScenario = true;
			View_FadePanel.SharedInstance.IsLightLoading = true;
			UtageModule.SharedInstance.LoadUseChapter(cardId.ToString(), () => {
				View_FadePanel.SharedInstance.IsLightLoading = false;
				UtageModule.SharedInstance.StartIntro("intro", MasterDataTable.card[cardId].character.cv, () => {
					bPlayScenario = false;
				}, true);            
			});

			// スキップを全員分にする.
			while(bPlayScenario){
                if (UtageModule.SharedInstance.IsSkip) {
					Utage.SoundManager.GetInstance().StopVoice();               
                    View_FadePanel.SharedInstance.FadeOut(View_FadePanel.FadeColor.Black, () => {
						UtageModule.SharedInstance.IsSkip = false;
						UtageModule.SharedInstance.ClearCache();
                        UtageModule.SharedInstance.SetActiveCore(false);
                        GameObject.Destroy(bg);
                        didEnd();
						View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
                    });               
                    yield break;
                }
				yield return null;
			}
		}
		UtageModule.SharedInstance.SetActiveCore(false);
		GameObject.Destroy(bg);

		didEnd();
	}

	// マイページに戻る.
	void BackToMyPage()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
    }

    void Awake()
	{
		var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
	}

	private GachaClientUseData m_data;
	private View_GachaList m_currentView;
	private bool m_isReady;

	public static int s_PageIndex;
	public static int s_SubPageIndex;
	public static void Reset()
	{
		s_PageIndex = 0;
		s_SubPageIndex = 0;
	}
}
