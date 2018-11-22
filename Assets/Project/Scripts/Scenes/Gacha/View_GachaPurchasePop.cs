using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : ガチャ購入確認ポップアップ.確定で内部的に通信も行う.
/// </summary>
public class View_GachaPurchasePop : PopupViewBase
{   
	/// <summary>
    /// 生成.シングルトン.重複生成時は前回生成したものを強制破棄する.
    /// </summary>
	public static View_GachaPurchasePop Create(GachaClientUseData.ContentsForView.RowData data, Screen_Gacha controller, Action<GachaClientUseData.ContentsForView.RowData, ReceiveGachaPurchaseProduct> didClose)
	{
		if(instance != null){
			instance.Dispose();
		}
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_Popup_GachaPurchase");
        instance = go.GetOrAddComponent<View_GachaPurchasePop>();
		instance.InitInternal(data, didClose);
		instance.m_controller = controller;
		return instance;
	}
	private void InitInternal(GachaClientUseData.ContentsForView.RowData data, Action<GachaClientUseData.ContentsForView.RowData, ReceiveGachaPurchaseProduct> didClose)
	{
		LockInputManager.SharedInstance.IsLock = true;

		m_data = data;
		m_didClose = didClose;

		// ボタン.
        this.GetScript<CustomButton>("Yes/bt_Common").onClick.AddListener(DidTapYes);
        this.GetScript<CustomButton>("No/bt_Common").onClick.AddListener(DidTapNo);
        this.GetScript<CustomButton>("Shop/bt_Common").onClick.AddListener(DidTapShop);
        this.GetScript<CustomButton>("bt_Close").onClick.AddListener(DidTapNo);

        // ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_PopTitle").text = data.Title;
		this.GetScript<TextMeshProUGUI>("txtp_GachaNameText").text = data.ContentsName;
		this.GetScript<TextMeshProUGUI>("txtp_MoneyNameText").text = data.UseCurrencyName;
		this.GetScript<TextMeshProUGUI>("txtp_FreeGacha").gameObject.SetActive(data.CurrencyQuantity <= 0);
		this.GetScript<RectTransform>("PopTextGroup01/PopTextLine01").gameObject.SetActive(data.CurrencyQuantity > 0);
		if(data.CurrencyQuantity > 0){
			this.GetScript<TextMeshProUGUI>("PopTextLine01/txtp_MoneyNameText").text = data.UseCurrencyName;
            this.GetScript<TextMeshProUGUI>("PopTextLine01/txtp_MoneyAmountText").text = data.CurrencyQuantity.ToString();
			this.GetScript<uGUISprite>("PopTextLine01/IconGem").ChangeSprite(data.UseCurrencyIconSptName);
		}      
		this.GetScript<RectTransform>("Stock/Gem").gameObject.SetActive(data.IsGem);
		this.GetScript<RectTransform>("Stock/Other").gameObject.SetActive(!data.IsGem);
		this.GetScript<RectTransform>("After/Gem").gameObject.SetActive(data.IsGem);
		this.GetScript<RectTransform>("After/AfterOther").gameObject.SetActive(!data.IsGem);

		// アイコン.
		this.GetScript<uGUISprite>("StockTitle/IconGem").ChangeSprite(data.UseCurrencyIconSptName);      
		this.GetScript<uGUISprite>("AfterStockTitle/IconGem").ChangeSprite(data.UseCurrencyIconSptName);

        // 使用コストの表示.
		UpdateCostAndHaveCurrency();

        SetBackButton ();

		this.PlayOpenCloseAnimation(true, () => LockInputManager.SharedInstance.IsLock = false);
	}   

    // 所持通貨などの変動による表示更新.
    private void UpdateCostAndHaveCurrency()
	{
		if (m_data.IsGem) {
            this.GetScript<TextMeshProUGUI>("TollGem/txtp_GemNumberText01").text = AwsModule.UserData.UserData.PaidGemCount.ToString("#,0");
            this.GetScript<TextMeshProUGUI>("FreeGem/txtp_GemNumberText01").text = AwsModule.UserData.UserData.FreeGemCount.ToString("#,0");

            var afterToll = AwsModule.UserData.UserData.PaidGemCount;
            var afterFree = AwsModule.UserData.UserData.FreeGemCount;
			if (m_data.IsToll) {
				afterToll -= m_data.CurrencyQuantity;
            } else {
				afterFree -= m_data.CurrencyQuantity;
                if (afterFree < 0) {
                    afterToll += afterFree;
                    afterFree = 0;
                }
            }
            this.GetScript<TextMeshProUGUI>("AfterTollGem/txtp_GemNumberText01").text = afterToll.ToString("#,0");
            this.GetScript<TextMeshProUGUI>("AfterFreeGem/txtp_GemNumberText01").text = afterFree.ToString("#,0");
        } else {
			this.GetScript<TextMeshProUGUI>("Other/txtp_MoneyAmountText").text = m_data.HaveCurrencyValue.ToString("#,0");
			this.GetScript<TextMeshProUGUI>("AfterOther/txtp_MoneyAmountText").text = (m_data.HaveCurrencyValue - m_data.CurrencyQuantity).ToString("#,0");
        }
        // 足りない場合のラベル.
		this.GetScript<RectTransform>("Warning").gameObject.SetActive(!m_data.CheckEnoughCurrency());
		this.GetScript<RectTransform>("After").gameObject.SetActive(m_data.CheckEnoughCurrency());
		if (!m_data.CheckEnoughCurrency()) {
			this.GetScript<TextMeshProUGUI>("txtp_WarningCurrency").text = m_data.UseCurrencyName;
        }

        // ボタン切り替え.
		if (m_data.CurrencyType == GachaUseCurrencyType.PaidGem || m_data.CurrencyType == GachaUseCurrencyType.FreeGem || m_data.CurrencyType == GachaUseCurrencyType.Money) {
			this.GetScript<RectTransform>("Yes").gameObject.SetActive(m_data.CheckEnoughCurrency());
			this.GetScript<RectTransform>("Shop").gameObject.SetActive(!m_data.CheckEnoughCurrency());
        } else {
            this.GetScript<RectTransform>("Shop").gameObject.SetActive(false);
        }
	}

    protected override void DidBackButton ()
    {
        DidTapNo ();
    }
	#region ButtonDelegate.
 
	// ボタン : 購入.
    void DidTapYes()
	{
        if (IsClosed) {
            return;
        }

		//ページ記憶
		if( m_controller != null ) {
			Screen_Gacha.s_PageIndex = m_controller.GetScript<uGUIPageScrollRect>("Scroll").DispTransformIndex;

			if( (m_controller.GetScript<RectTransform>("Contents/TabView").gameObject.activeSelf == true) &&
				(m_controller.GetScript<RectTransform>("Contents/SingleView").gameObject.activeSelf == false) )
			{
				Screen_Gacha.s_SubPageIndex = 0;
			}else if( (m_controller.GetScript<RectTransform>("Contents/TabView").gameObject.activeSelf == false) &&
				(m_controller.GetScript<RectTransform>("Contents/SingleView").gameObject.activeSelf == true) )
			{
				Screen_Gacha.s_SubPageIndex = 1;
			}
		}

		// 時間チェック.
		Debug.Log(m_data.ContentsName+" "+m_data.DrawLimitaionType.ToString()+" : start="+m_data.StartDate+" ~ end="+m_data.EndDate);
		if(!GameTime.SharedInstance.IsWithinPeriod(m_data.StartDate, m_data.EndDate)){
			PopupManager.OpenPopupOK("このガチャは販売期間外です。\nガチャ情報を更新します。", () => {
				View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToGacha());
			});
			return;
		}

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black);
        View_FadePanel.SharedInstance.SetProgress (0);
        ++m_dlMaxCnt;
        // 通信も行う.
        SendAPI.GachaPurchaseProduct (m_data.ProductID, (bSuccess, res) => {
            if (!bSuccess || res == null) {
                Debug.LogError ("[View_GachaPurchasePop] DidTapYes Error!! : Request error.");
                return;
            }
            if (res.ConsumerData != null) {
                res.ConsumerData.CacheSet ();    // ガチャチケット使用の更新用.
            }
            AwsModule.UserData.UserData = res.UserData;
            View_PlayerMenu.CreateIfMissing ().UpdateView (res.UserData);
            View_GlobalMenu.IsVisible = View_PlayerMenu.IsVisible = false;         
            this.UpdateCache (res);
            m_data.UpdateDrawCount ();

            ++m_dlCurrentCnt;         

            // 演出をそのままこのViewから展開する.あらかじめ演出表示用のLive2Dモデルをロードしておく.
            SoundManager.SharedInstance.PlayBGM (SoundClipName.bgm011, true);
            switch (m_data.Type.Enum) {
            case GachaTypeEnum.character_gacha:
                CreateCardGacha (res);
                break;
            case GachaTypeEnum.weapon_gacha:
                CreateWeaponGacha (res);
				View_GlobalMenu.Setup();
                break;
            default:
                LockInputManager.SharedInstance.IsLock = false;
                Dispose();
                break;
            }
        });
	}   
	private void CreateCardGacha(ReceiveGachaPurchaseProduct res)
	{
		this.LoadModels(res, objList => {
            this.Dispose();
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
            View_GachaMovie.Create(res, objList, () => {
                View_GlobalMenu.IsVisible = View_PlayerMenu.IsVisible = true;
                if (m_didClose != null) {
                    m_didClose(m_data, res);
                }
                Resources.UnloadUnusedAssets();
                SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm010, true);
            });
        });
	}
	private void CreateWeaponGacha(ReceiveGachaPurchaseProduct res)
	{
		this.LoadWeaponSprite(res, sptList => {
            this.Dispose();
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
            View_GachaMovie.Create(res, sptList, () => {
                View_GlobalMenu.IsVisible = View_PlayerMenu.IsVisible = true;
                if (m_didClose != null) {
					m_didClose(m_data, res);
                }
                Resources.UnloadUnusedAssets();
                SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm010, true);
            });
        });
	}

	// ボタン : キャンセル.
    void DidTapNo()
	{
        if (IsClosed) {
            return;
        }

        this.PlayOpenCloseAnimation(false, () => {
            if(m_didClose != null) {
                m_didClose(null, null);
            }
            Dispose();
        });
	}

	// ボタン : ショップへ.
    void DidTapShop()
	{
        if (IsClosed) {
            return;
        }

        this.PlayOpenCloseAnimation(false, () => {
            switch(m_data.CurrencyType){
            case GachaUseCurrencyType.FreeGem:
            case GachaUseCurrencyType.PaidGem:
                View_GemShop.DidPurchasedGem += UpdateCostAndHaveCurrency;
                View_GemShop.OpenGemShop();
                break;
            case GachaUseCurrencyType.Money:
                View_CoinShop.DidBuy += () => { };
                View_CoinShop.Create();
                break;
            }
            Dispose();
        });
	}

    #endregion

    // キャッシュ更新.
	private void UpdateCache(ReceiveGachaPurchaseProduct res)
	{
		var itemList = res.AcquiredGachaItemDataList;
		var service = res.RarestCardGachaItemData;
		if (itemList != null & itemList.Length > 0) {
            itemList.Where(i => i.CardData != null).Select(i => i.CardData).CacheSet();
            itemList.Where(i => i.WeaponData != null).Select(i => i.WeaponData).CacheSet();
			itemList.Where(i => i.MagikiteData != null).Select(i => i.MagikiteData).CacheSet();
            
            // 素材=かぶり石の場合、このガチャで出た数だけが帰ってくるので加算処理が必要.
			foreach(var i in itemList){
				if(i.IsNew){
					continue;
				}
				var cache = MaterialData.CacheGet(i.ConvertedItemId);
				if(cache == null){
					var newItem = new MaterialData(i.ConvertedItemId, i.ConvertedQuantity);
					newItem.CacheSet();
					continue;
				}
				cache.Count += i.ConvertedQuantity;
				cache.CacheSet();
			}

        }
		if(service != null){
			switch ((ItemTypeEnum)service.ItemType) {
                case ItemTypeEnum.card:
					if (service.IsNew) {
						service.CardData.CacheSet();
					} else{
						(new MaterialData(service.ConvertedItemId, service.ConvertedQuantity)).CacheSet();
					}               
                    break;
				case ItemTypeEnum.weapon:
					service.WeaponData.CacheSet();
					break;
				case ItemTypeEnum.magikite:
					service.MagikiteData.CacheSet();
					break;
            }
		}
	}
   
    // 排出されたモデルを適宜ロード.
	private void LoadModels(ReceiveGachaPurchaseProduct res, Action<List<KeyValuePair<int, GameObject>>> didLoad)
	{
		var itemList = res.AcquiredGachaItemDataList;
		if(itemList == null || itemList.Length <= 0){
			return;
		}

		var service = res.RarestCardGachaItemData;
		var cardList = new List<CardCard>();
		var list = itemList.Where(i => (ItemTypeEnum)i.ItemType == ItemTypeEnum.card).Select(i => MasterDataTable.card[i.ItemId]).ToList();
		cardList.AddRange(list);      
		if(service != null && (ItemTypeEnum)service.ItemType == ItemTypeEnum.card){         
			cardList.Add(MasterDataTable.card[service.ItemId]);
		}
		if(cardList.Count > 0){
			m_dlMaxCnt += cardList.Count;
			View_FadePanel.SharedInstance.SetProgress(m_dlCurrentCnt / m_dlMaxCnt);
			this.StartCoroutine(this.LoadLive2dProc(cardList, didLoad));         
		}else{
			View_FadePanel.SharedInstance.SetProgress(1f);
			didLoad(null);
		}
	}
    // Live2Dモデルロード.
	private IEnumerator LoadLive2dProc(List<CardCard> cardList, Action<List<KeyValuePair<int, GameObject>>> didLoad)
	{
		var objList = new List<KeyValuePair<int, GameObject>>();
		foreach (var card in cardList) {
			var loader = new UnitResourceLoader(card.id);
            loader.LoadResource(resouce => {
                var go = Instantiate(resouce.Live2DModel) as GameObject;
				go.SetActive(false);
				objList.Add(new KeyValuePair<int, GameObject>(card.id, go));
				++m_dlCurrentCnt;
				View_FadePanel.SharedInstance.SetProgress(m_dlCurrentCnt / m_dlMaxCnt);
            });
        }
		while(objList.Count < cardList.Count){
			yield return null;
		}
		didLoad(objList);
	}

    // 武器絵のロード.
	private void LoadWeaponSprite(ReceiveGachaPurchaseProduct res, Action<List<KeyValuePair<int, Sprite>>> didLoad)
	{
		var itemList = res.AcquiredGachaItemDataList;
        if (itemList == null || itemList.Length <= 0) {
            return;
        }

		var service = res.RarestCardGachaItemData;
		var wpnList = new List<Weapon>();
		var list = itemList.Where(i => (ItemTypeEnum)i.ItemType == ItemTypeEnum.weapon).Select(i => MasterDataTable.weapon[i.ItemId]).ToList();
		wpnList.AddRange(list);
		if (service != null && (ItemTypeEnum)service.ItemType == ItemTypeEnum.weapon) {
			wpnList.Add(MasterDataTable.weapon[service.ItemId]);
        }      
		if(wpnList.Count > 0){
			this.StartCoroutine(this.LoadWeaponSprite(wpnList, didLoad));
        }else{
            didLoad(null);
        }
	}
	private IEnumerator LoadWeaponSprite(List<Weapon> wpnList, Action<List<KeyValuePair<int, Sprite>>> didLoad)
    {
		var objList = new List<KeyValuePair<int, Sprite>>();
		foreach (var wpn in wpnList) {
			var loader = new WeaponResourceLoader(wpn.id);
            loader.LoadResource(resouce => objList.Add(new KeyValuePair<int, Sprite>(wpn.id, resouce.PortraitImage)));
        }
        while (objList.Count < wpnList.Count) {
            yield return null;
        }
        didLoad(objList);
    }

	private GachaClientUseData.ContentsForView.RowData m_data;
	private Action<GachaClientUseData.ContentsForView.RowData, ReceiveGachaPurchaseProduct> m_didClose;

	private float m_dlMaxCnt;
	private float m_dlCurrentCnt;
	private Screen_Gacha m_controller;

	static View_GachaPurchasePop instance;
}
