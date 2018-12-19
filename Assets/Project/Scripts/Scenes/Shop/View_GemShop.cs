using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_GemShop : PopupViewBase {

	public static event System.Action DidPurchasedGem;

    public static void OpenGemShop()
    {
        if (!AwsModule.UserData.IsExistBirthInfo) {
            View_GemShopAgePop.Create ();
        } else {
            Create ();
        }
    }

    private static View_GemShop instance;
    public static View_GemShop Create()
    {
        AwsModule.LocalData.IsOpenedGemshop = true;
		View_PlayerMenu.Setup();	//ジェム誘導更新

        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Shop/View_GemShop");
        instance = go.GetOrAddComponent<View_GemShop>();
        instance.InitInternal();
        return instance;
    }

	public static void CheckDiscount( System.Action<string> proc )
	{
		if( MasterDataTable.gem_recommended != null ) {
			var recommended = MasterDataTable.gem_recommended.GetRecommended();
			if( recommended != null ) {
				switch( recommended.type ) {
					case GemRecommendedEnum.Discount:
						if( AwsModule.LocalData.IsOpenedGemshop )
							return;

						if( checkPaymentsGetProductList == false ) {
							checkPaymentsGetProductList = true;
							SendAPI.PaymentsGetProductList( (result, response) => {
								if( result && (response != null) ) {
									GemProductData[] productDataList = response.GemProductDataList;
									var skuItems = PurchaseManager.SharedInstance.SkuItems;
									if( skuItems != null ) {
										skuItems = skuItems.OrderBy (x => x.price).ToList();
										List<BonusGem> list = new List<BonusGem>();
										skuItems.ForEach( item => {
											var storeSetting = MasterDataTable.gem_store_setting.DataList.FirstOrDefault(x => x.store_product_id == item.productID);
											var severData = productDataList.FirstOrDefault (x => x.GemProductId == storeSetting.app_product_id);
											var bonusData = MasterDataTable.bonus_gem [severData.BonusId];
											if( !string.IsNullOrEmpty( bonusData.catch_copy ) )
												list.Add( bonusData );
										} );
										if( list.Count == skuItems.Count ) {
											proc( recommended.message );
											saveDisplayDiscount = true;
										}else{
											AwsModule.LocalData.IsOpenedGemshop = true;	//過去に開いているとみなす
										}
									}
								}
							} );
						}else{
							if( saveDisplayDiscount ) {
								proc( recommended.message );
							}
						}
						break;
					case GemRecommendedEnum.Free:
						proc( recommended.message );
						break;
				}
			}
		}
	}

    public override void Dispose ()
    {
		DidPurchasedGem = null;
        PurchaseManager.SharedInstance.SucceedEvent -= OnSucceed;
        PurchaseManager.SharedInstance.ErrorEvent -= OnError;
        base.Dispose ();
    }

    private void OnSucceed(SkuItem item)
    {
        View_FadePanel.SharedInstance.IsLightLoading = false;
        LockInputManager.SharedInstance.IsLock = false;
		if(DidPurchasedGem != null){
			DidPurchasedGem();
		}
        UpdateInfo ();
        CreateItem ();
    }

    private void OnError(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item)
    {
        View_FadePanel.SharedInstance.IsLightLoading = false;
        LockInputManager.SharedInstance.IsLock = false;
    }

    private void InitInternal()
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.gameObject.SetActive (false);
        // APIを投げる？
        SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("LawCommerce/bt_CommonS", DidTapLawCommerce);
        SetCanvasCustomButtonMsg ("LawFund/bt_CommonS", DidTapLawFund);

        UpdateInfo ();

        int age = -1;
        age = AwsModule.UserData.GetAge();
        if (age < 0) {
            View_GemShopAgePop.Create ();
            Dispose ();
            return;
        }

        monthlyLimitation = null;
        if (MasterDataTable.gem_monthry_limitation != null) {
            monthlyLimitation = MasterDataTable.gem_monthry_limitation.DataList.Where (x => x.age >= age).OrderBy (x => x.age).FirstOrDefault ();
        }

        PurchaseManager.SharedInstance.SucceedEvent += OnSucceed;
        PurchaseManager.SharedInstance.ErrorEvent += OnError;
        SetBackButton ();

        // リスト内容の更新がありうるので作り直す
        CreateItem ();
    }

    private void UpdateInfo()
    {
        GetScript<TextMeshProUGUI> ("Total/txtp_Wallet").SetTextFormat("{0:#,0}", AwsModule.UserData.UserData.GemCount);
        GetScript<TextMeshProUGUI> ("Gem/txtp_Wallet").SetTextFormat("{0:#,0}", AwsModule.UserData.UserData.PaidGemCount);
        GetScript<TextMeshProUGUI> ("FreeGem/txtp_Wallet").SetTextFormat("{0:#,0}", AwsModule.UserData.UserData.FreeGemCount);
    }


    private void CreateItem()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.PaymentsGetProductList (
            (result, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!result) {
                    PopupManager.OpenPopupSystemOK("商品リストの取得に失敗しました。時間を置いて再度お試しください。");
                    Dispose();
                    return;
                }
                CreateItem (monthlyLimitation, response.MonthlyPayment, response.GemProductDataList);
            }
        );
    }

    private void CreateItem(GemMonthryLimitation monthlyLimitation, int monthlyPayment, GemProductData[] productDataList)
    {
        var ScrollAreaGemShopItem = GetScript<ScrollRect> ("ScrollAreaGemShopItem");
        ScrollAreaGemShopItem.content.gameObject.DestroyChildren ();

        var skuItems = PurchaseManager.SharedInstance.SkuItems;

		// アイテムリストが正常に受け取れておらず、1時間以上端末時間と乖離していた場合は不正とみなし注意文言を表示の上閉じる.
		if(skuItems == null || skuItems.Count <= 0){
			View_StoreAleart.Create(Dispose);
            return;
		}

        // 金額によって並び替えをする。
        skuItems = skuItems.OrderBy (x => x.price).ToList();

        int itemCount = skuItems.Count;
        for (int i = 0; i < itemCount; ++i) {
            var item = skuItems [i];
            var go = GameObjectEx.LoadAndCreateObject ("Shop/ListItem_GemShopItem", ScrollAreaGemShopItem.content.gameObject);

            bool isLimitation = false;
            if (monthlyLimitation != null) {
                isLimitation = (double)monthlyLimitation.limitation < (double)monthlyPayment + item.price;
            }
            go.GetOrAddComponent<ListItem_GemShopItem> ().Init (item, productDataList, isLimitation);
        }
            
        PlayOpenCloseAnimation (true);
    }


    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false,
            () => {
                Dispose();
            }
        );
    }

	// 特定商取引法に関しての表示
    void DidTapLawCommerce()
    {
        if (IsClosed) {
            return;
        }

		var menu = MasterDataTable.option_menu.DataList.Find(d => d.Enum == OptionMenuEnum.SpecifiedCommercialTransactions);
        var info = MasterDataTable.help_notice.DataList.Find(d => d.subject == menu.Enum);
        PopupManager.OpenPopupOkWithScroll(info.text, null, menu.name);
    }

	// 資金決済法に関しての表示
    void DidTapLawFund()
    {
        if (IsClosed) {
            return;
        }

		var menu = MasterDataTable.option_menu.DataList.Find(d => d.Enum == OptionMenuEnum.PaymentService);
        var info = MasterDataTable.help_notice.DataList.Find(d => d.subject == menu.Enum);
        PopupManager.OpenPopupOkWithScroll(info.text, null, menu.name);
    }

	static bool saveDisplayDiscount;
	static bool checkPaymentsGetProductList;
	public static void Reset()
	{
		saveDisplayDiscount = false;
		checkPaymentsGetProductList = false;
	}

    GemMonthryLimitation monthlyLimitation;
}
