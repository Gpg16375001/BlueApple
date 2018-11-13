using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// Screen : ショップ.
/// </summary>
public class Screen_Shop : ViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(ShopProductData[] productDatas)
	{
		m_shopListPrefab = Resources.Load("Shop/ListItem_ShopItem") as GameObject;
        m_soulListPrefab = Resources.Load("Shop/ListItem_SoulStone") as GameObject;
		m_productDatas = productDatas;

		// 司書配置.
        this.RequestLibrarianModel();
		
		// グローバルメニューイベント.
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

		this.UpdateList();

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
	}

    public override void Dispose ()
    {
        m_Live2dVoicePlayer.Stop ();
		base.Dispose();
    }
    // リスト更新.
	private void UpdateList()
	{
		// タブ.
        var categories = MasterDataTable.shop_category.GetItemShopCategories();
        var root = this.GetScript<HorizontalLayoutGroup>("TabGrid");
		root.gameObject.DestroyChildren();
        foreach (var c in categories) {
            var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_HorizontalTextTab", root.gameObject);
            var tab = go.GetOrAddComponent<ListItem_HorizontalTextTab>();
            tab.Init(c.name, DidTapTab);
        }
        // 初回リスト.
        this.DidTapTab(root.GetComponentsInChildren<ListItem_HorizontalTextTab>().First());
	}

	// 司書キャラモデルリクエスト.
    private void RequestLibrarianModel()
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        this.GetScript<RectTransform>("CharacterAnchor").gameObject.DestroyChildren();
        var loader = new UnitResourceLoader(308001011); // 308001011=司書
        loader.LoadFlagReset();
        loader.IsLoadLive2DModel = true;
        loader.IsLoadVoiceFile = true;
        loader.LoadResource(resouce => {
            var go = Instantiate(resouce.Live2DModel) as GameObject;
            go.transform.SetParent(this.GetScript<RectTransform>("CharacterAnchor"));
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            m_Live2dVoicePlayer = go.GetOrAddComponent<Live2dVoicePlayer>();
            m_Live2dVoicePlayer.Play("VOICE_308001011", "shop_001");
            View_FadePanel.SharedInstance.IsLightLoading = false;
        });
    }

	// TODO : ショップリスト作成.
	private void CreateShopList(string category)
	{
        var scrollRect = this.GetScript<ScrollRect> ("ScrollAreaShopItem");
        var storeGrid = this.GetScript<InfiniteGridLayoutGroup> ("ShopItemGrid");
        var soulGrid = this.GetScript<InfiniteGridLayoutGroup> ("IconGrid");


		// TODO : "キャラ被り石と交換"という文言から"ソウルと交換"に変わった.マスター更新が終わったら前者を消す.
		if (category == "ソウルと交換") {
            // かぶり石ショップの作成
            soulGrid.gameObject.SetActive (true);
            storeGrid.gameObject.SetActive (false);

            scrollRect.content = soulGrid.GetComponent<RectTransform> ();
            soulGrid.gameObject.DestroyChildren ();
            m_SoulExchangeList = MasterDataTable.card_based_material.EnableData;
            soulGrid.OnUpdateItemEvent.RemoveAllListeners ();
            soulGrid.OnUpdateItemEvent.AddListener (CallbackUpdateSoulListItem);
            soulGrid.Initialize (m_soulListPrefab, 20, m_SoulExchangeList.Length, false);
			soulGrid.ResetScrollPosition();
        } else {
            m_list = MasterDataTable.shop_list.DataList.FindAll (s => s.shop_category.name == category);
            m_list = m_list.Where (p => Array.Exists (m_productDatas, d => d.ShopProductId == p.id)).ToList ();
            soulGrid.gameObject.SetActive (false);
            storeGrid.gameObject.SetActive (true);

            scrollRect.content = storeGrid.GetComponent<RectTransform> ();
            storeGrid.gameObject.DestroyChildren ();
            storeGrid.OnUpdateItemEvent.RemoveAllListeners ();
            storeGrid.OnUpdateItemEvent.AddListener (CallbackUpdateListItem);
            storeGrid.Initialize (m_shopListPrefab, 5, m_list.Count, false);
			storeGrid.ResetScrollPosition();
        }
	}
	void CallbackUpdateListItem(int index, GameObject go)
	{
		var list = go.GetComponent<ListItem_ShopItem>();
		if(list == null){
			list = go.AddComponent<ListItem_ShopItem>();
			list.Init(data => {
				var i = Array.FindIndex(m_productDatas, d => d.ShopProductId == data.ShopProductId);
				m_productDatas[i] = data;
				this.UpdateList();
			});
		}
		list.SetInfo(m_list[index], Array.Find(m_productDatas, d => d.ShopProductId == m_list[index].id));
	}

    void CallbackUpdateSoulListItem(int index, GameObject go)
    {
        var list = go.GetOrAddComponent<ListItem_SoulStone>();
        list.SetInfo(m_SoulExchangeList[index]);
    }

    // 購入後コールバック.
    void CallbackDidBuyItem()
	{
        //m_Live2dVoicePlayer.Play("VOICE_308001011", "shop_purchase_001");
	}

	#region ButtonDelegate.

	// ボタン: 戻る.
    void DidTapBack()
	{
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
	}

	// ボタン: タブタップ.
	void DidTapTab(ListItem_HorizontalTextTab tab)
	{
		// 情報表示切り替え
		var categoryData = MasterDataTable.shop_category.DataList.Find(s => s.name == tab.CategoryName);
        GetScript<TextMeshProUGUI> ("txtp_ShopNotes").SetText (categoryData.notes);
		// TODO : "キャラ被り石と交換"という文言から"ソウルと交換"に変わった.マスター更新が終わったら前者を消す.
		if (tab.CategoryName == "ソウルと交換") {
            this.GetScript<RectTransform> ("Wallet").gameObject.SetActive (false);
        } else {
            this.GetScript<RectTransform> ("Wallet").gameObject.SetActive (true);
            this.SetShopNotes (categoryData.use_item_type.Enum);
        }
		// タブ選択状態.
        var root = this.GetScript<HorizontalLayoutGroup>("TabGrid");
        root.GetComponentsInChildren<ListItem_HorizontalTextTab>()
            .Select(t => t.IsSelected = t.CategoryName == tab.CategoryName)
            .ToList();
		// ショップリスト.
        this.CreateShopList(tab.CategoryName);
	}
	private void SetShopNotes(ItemTypeEnum type)
    {
        switch (type) {
            case ItemTypeEnum.free_gem:
            case ItemTypeEnum.paid_gem:
				{
					this.GetScript<TextMeshProUGUI>("txtp_Wallet").text = AwsModule.UserData.UserData.GemCount.ToString("#,0");
                    break;
				}
            case ItemTypeEnum.money:
				{
					this.GetScript<TextMeshProUGUI>("txtp_Wallet").text = AwsModule.UserData.UserData.GoldCount.ToString("#,0");
                    break;
                }
            case ItemTypeEnum.gacha_coin:
				{
					this.GetScript<TextMeshProUGUI>("txtp_Wallet").text = "0";
                    break;
                }
            case ItemTypeEnum.pvp_medal:
				{
                    this.GetScript<TextMeshProUGUI>("txtp_Wallet").text = AwsModule.UserData.UserData.PvpMedalCount.ToString("#,0");
                    break;
				}
            default:
                return;

        }
		this.GetScript<uGUISprite>("Wallet/CurrencyIcon").ChangeSprite(type.GetCurrencyIconName());
    }

    #endregion

	void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	private GameObject m_shopListPrefab;
    private GameObject m_soulListPrefab;
	private List<ShopList> m_list;
	private ShopProductData[] m_productDatas;
    private CardBasedMaterial[] m_SoulExchangeList;
    private Live2dVoicePlayer m_Live2dVoicePlayer;
}
