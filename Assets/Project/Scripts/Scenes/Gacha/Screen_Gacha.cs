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
		this.GetScript<CustomButton>("bt_ArrowPage_1").gameObject.SetActive(true);
		this.GetScript<CustomButton>("bt_ArrowPage_2").gameObject.SetActive(false);
		this.SetCanvasCustomButtonMsg("Character/bt_GachaCategory", DidTapCharaGacha);
		this.SetCanvasCustomButtonMsg("Weapon/bt_GachaCategory", DidTapWeaponGacha);      
		DidTapCharaGacha();

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
		rootList.Initialize(prefab, m_data.CharacterGachaContents.Count, m_data.CharacterGachaContents.Count, true);
		this.GetScript<uGUIPageScrollRect>("Scroll").SetInfinit(true, m_data.CharacterGachaContents.Count);

		// 初期選択.
		this.GetScript<uGUIPageScrollRect>("Scroll").CenterOn(rootList.GetComponentsInChildren<View_GachaList>(true)[0].gameObject);
		//var centerObj = this.GetScript<uGUIPageScrollRect>("Scroll").GetCurrentCenterObject(); // rootList.GetComponentsInChildren<View_GachaList>()[0].gameObject;
		//CallbackPagingOnCenter(centerObj);
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
		if (m_currentPage >= m_data.CharacterGachaContents.Count-1) {
            return;
        }
		if(!m_bEnableButton){
			return;
		}
		m_bEnableButton = false;
		++m_currentPage;      
		this.StartCoroutine(this.WaitInputInterval());      
		this.GetScript<uGUIPageScrollRect>("Scroll").Paging(-1);

		this.GetScript<CustomButton>("bt_ArrowPage_1").gameObject.SetActive(m_currentPage < m_data.CharacterGachaContents.Count-1);
		this.GetScript<CustomButton>("bt_ArrowPage_2").gameObject.SetActive(true);
	}
	// 右スクロール
	void DidTapRight()
    {
		if (m_currentPage <= 0) {
            return;
        }      
		if (!m_bEnableButton) {
            return;
        }
        m_bEnableButton = false;
		--m_currentPage;
        this.StartCoroutine(this.WaitInputInterval());
		this.GetScript<uGUIPageScrollRect>("Scroll").Paging(1);

		this.GetScript<CustomButton>("bt_ArrowPage_1").gameObject.SetActive(true);
		this.GetScript<CustomButton>("bt_ArrowPage_2").gameObject.SetActive(m_currentPage > 0);
    }
	private int m_currentPage;

	IEnumerator WaitInputInterval()
	{
		yield return new WaitForSeconds(0.2f);
		m_bEnableButton = true;

	}
	private bool m_bEnableButton = true;

    #endregion

	#region Callbacks.

	// コールバック：ページスクロールセンタリング時.
	void CallbackPagingOnCenter(GameObject centerObj)
	{
		Debug.Log("CallbackPagingOnCenter " + m_data.CharacterGachaContents[int.Parse(centerObj.name)].Data.ContentsName + " objName=" + centerObj.name);
		m_currentPage = int.Parse(centerObj.name);
		this.GetScript<CustomButton>("bt_ArrowPage_1").gameObject.SetActive(m_currentPage < m_data.CharacterGachaContents.Count-1);
        this.GetScript<CustomButton>("bt_ArrowPage_2").gameObject.SetActive(m_currentPage > 0);

		var c = centerObj.GetOrAddComponent<View_GachaList>();
        c.Init(m_data.CharacterGachaContents[int.Parse(centerObj.name)], CallbackDidCloseGacha);

        var rootTab = this.GetScript<HorizontalLayoutGroup>("Page").gameObject;
        m_currentView = centerObj.GetComponent<View_GachaList>();
        Debug.Log("CallbackPagingOnCenter : " + centerObj.name + " " + m_currentView.Gacha.name + " index=" + m_currentView.Gacha.index);
        rootTab.GetComponentsInChildren<ListItem_GachaPage>(true).Select(lst => lst.ForceHighlight = lst.Gacha.index == m_currentView.Gacha.index).ToList();
	}

	// コールバック : 無限スクロール常にリストアイテムを生成した際のコールバック.
	void UpdateInfiniteListItem(int index, GameObject createObj)
	{
		var c = createObj.GetOrAddComponent<View_GachaList>();
		Debug.Log("Init " + m_data.CharacterGachaContents[index].Data.ContentsName + " objName=" + createObj.name);
        c.Init(m_data.CharacterGachaContents[index], CallbackDidCloseGacha);      

		// ページ側のリンクオブジェクトも更新.
		var item = this.GetComponentsInChildren<ListItem_GachaPage>(true).FirstOrDefault(lst => lst.Gacha.index == m_data.CharacterGachaContents[index].Gacha.index);
		if(item != null){
			item.LinkViewObj = createObj;
            Debug.Log("UpdateInfiniteListItem UpdatePage : " + item.Gacha.name + " index=" + item.Gacha.index);
		}      
	}   

	// コールバック : ガチャカテゴリを押した.
	void CallbackDidTapCategory(GameObject targetViewObj)
	{
		if (targetViewObj == m_currentView.gameObject){
			return;
		}

		// InfiniteGridLayoutGroup側の都合でView_GachaListの更新がされないことがあるため強制呼び出し.
		UpdateInfiniteListItem(int.Parse(targetViewObj.name), targetViewObj);

		this.GetScript<uGUIPageScrollRect>("Scroll").CenterOn(targetViewObj);
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
            m_currentView.Draw(row);
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
}
