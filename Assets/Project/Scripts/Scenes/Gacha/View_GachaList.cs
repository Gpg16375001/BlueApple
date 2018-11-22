using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using SmileLab;
using SmileLab.Net.API;
using SmileLab.UI;


/// <summary>
/// View : 各種ガチャごとのView.
/// </summary>
public class View_GachaList : ViewBase
{
	/// <summary>ガチャID.</summary>
	public int ID { get { return m_contents.ID; } }

    /// <summary>ガチャタイプ.</summary>
	public Gacha Gacha { get { return m_contents.Gacha; } }
    
    /// <summary>1日限定ガチャは一度引いたらもうその日は引けない.</summary>
	public override bool IsEnableButton
	{
		set {
			base.IsEnableButton = value;
			var bEnableDayGacha = m_contents.DataOncePerDayDiscount != null && m_contents.DataOncePerDayDiscount.IsPurchasable;
			if(this.Exist<CustomButton>("bt_BuyDayGacha")){
				this.GetScript<CustomButton>("bt_BuyDayGacha").interactable = value && bEnableDayGacha;
			}         
		}
	}

    /// <summary>初期化済み？</summary>
	public bool IsInit { get { return m_contents != null; } }

	private Sprite saveSprite = null;

	/// <summary>
	/// 初期化.
	/// </summary>
	public void Init(GachaClientUseData.ContentsForView contents, Action<GachaClientUseData.ContentsForView.RowData, ReceiveGachaPurchaseProduct> didDrawGacha)
	{
		m_contents = contents;
		m_didDrawGacha = didDrawGacha;

		//背景リセット
		if( saveSprite == null )
			saveSprite = this.GetScript<Image>("img_GachaBannerL").sprite;
		else
			this.GetScript<Image>("img_GachaBannerL").sprite = saveSprite;
		// 背景ロード.あればロード.なければ何もしない.
		this.DownloadBG(texture => { 
			if(texture == null){
				return;
			}
			if(this.IsDestroyed){
				return;
			}
			this.GetScript<Image>("img_GachaBannerL").sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		});

		// ボタンとラベル類  初回表示       
		this.UpdateHaveCurrency();
		this.UpdateInfo();

		if(this.Exist<CustomButton>("Details/bt_Sub")){
			this.GetScript<CustomButton>("Details/bt_Sub").onClick.RemoveAllListeners();
			this.SetCanvasCustomButtonMsg("Details/bt_Sub", DidTapDetails);
		}
		if(this.Exist<CustomButton>("Rate/bt_TopLineGray")){
			this.GetScript<CustomButton>("Rate/bt_TopLineGray").onClick.RemoveAllListeners();
			this.SetCanvasCustomButtonMsg("Rate/bt_TopLineGray", DidTapRate);
		}
		if (this.Exist<CustomButton>("bt_GachaCounter")) {
			this.GetScript<CustomButton>("bt_GachaCounter").onClick.RemoveAllListeners();
			this.SetCanvasCustomButtonMsg("bt_GachaCounter", DidTapRarity4Notes);
		}
	}
	private void DownloadBG(Action<Texture2D> didLoad)
    {
		DLCManager.AssetBundleFromFileOrDownload(DLCManager.DLC_FOLDER.GachaBG, "gachabg_"+m_contents.ID, m_contents.ID.ToString(), didLoad, (ex) => {
			Debug.LogWarning(ex.Message);
			didLoad(null);
        });
    }

    /// <summary>
    /// 指定データのガチャを引く.
    /// </summary>
	public void Draw(GachaClientUseData.ContentsForView.RowData data, bool isRetry=false)
	{
		if (data.Type.Enum == GachaTypeEnum.weapon_gacha) {
            var haveCnt = WeaponData.CacheGetAll().Count;
            if (haveCnt + data.DrawCount > AwsModule.UserData.UserData.WeaponBagCapacity) {
                View_GachaArmouryPop.Create(data);
                return;
            }
        }

		Screen_Gacha controller = null;
		if( !isRetry ) {
			controller = GetComponentInParent<Screen_Gacha>();
		}
		View_GachaPurchasePop.Create(data, controller, (row, res) => {
            if (m_didDrawGacha != null) {
				m_didDrawGacha(row, res);
            }         
        });
	}

	#region Button.

	void DidTapOnceFree()
    {
		this.Draw(m_contents.DataFree);
    }

    void DidTapOnceTicket()
	{
		this.Draw(m_contents.DataUseTicket);
	}

	void DidTapOnce()
	{
		this.Draw(m_contents.Data);
	}

    void DidTapFreeTen()
	{
		this.Draw(m_contents.DataFree10th);
	}

	void DidTapTen()
    {
		this.Draw(m_contents.Data10th);      
    }
    
	void DidTapDiscount()
    {
		this.Draw(m_contents.DataOncePerDayDiscount);
    }

	// ボタン : ジェム購入
    void DidTapBuyGem()
    {
		View_GemShop.OpenGemShop();      
    }
    
	// ボタン : 詳細.
    void DidTapDetails()
	{
		// TODO : 詳細説明はガチャ商品ごとに設定できるがボタンが一つしかないためいずれか有効なものを設定する.
		var dataList = new GachaClientUseData.ContentsForView.RowData[] { m_contents.Data, m_contents.Data10th, m_contents.DataOncePerDayDiscount, m_contents.DataFree, m_contents.DataFree10th, m_contents.DataUseTicket };
		var data = dataList.First(d => d != null);
		View_WebView.Open(data.DescriptionURL);
	}

	// ボタン : 提供割合.
    void DidTapRate()
    {
		// TODO : 詳細説明はガチャ商品ごとに設定できるがボタンが一つしかないためいずれか有効なものを設定する.
        var dataList = new GachaClientUseData.ContentsForView.RowData[] { m_contents.Data, m_contents.Data10th, m_contents.DataOncePerDayDiscount, m_contents.DataFree, m_contents.DataFree10th, m_contents.DataUseTicket };
        var data = dataList.First(d => d != null);
		View_GachaRate.Create(data.ID);
    }
    
	// ボタン : 天井説明.
    void DidTapRarity4Notes()
	{
		View_GachaDescription.Create();
	}

    #endregion   

    // 所持通貨数の表示更新.
    private void UpdateHaveCurrency()
	{
		if(m_contents.Type.Enum == GachaTypeEnum.weapon_gacha){
			this.GetScript<TextMeshProUGUI>("txtp_NumCoin").text = AwsModule.UserData.UserData.GoldCount.ToString();
		}else{
			this.GetScript<TextMeshProUGUI>("txtp_NumGem").text = AwsModule.UserData.UserData.PaidGemCount.ToString();
            this.GetScript<TextMeshProUGUI>("txtp_NumFreeGem").text = AwsModule.UserData.UserData.FreeGemCount.ToString();
		}      
	}

    // 表示情報更新.
    private void UpdateInfo()
	{
		this.ClearButtonLisners();
        
        // 無料単発.       
		if(m_contents.DataFree != null && m_contents.DataFree.IsPurchasable){
			this.GetScript<TextMeshProUGUI>("txtp_MoneyOneGacha").gameObject.SetActive(false);
			this.GetScript<uGUISprite>("txtp_MoneyOneGacha/IconCurrency").ChangeSprite(m_contents.DataFree.UseCurrencyIconSptName);
			this.GetScript<CustomButton>("BuyOneGacha/bt_BuyGacha").onClick.AddListener(DidTapOnceFree);         
		}
		// チケット使用単発.
		else if(m_contents.DataUseTicket != null && m_contents.DataUseTicket.HaveCurrencyValue > 0) {
			this.GetScript<TextMeshProUGUI>("txtp_MoneyOneGacha").gameObject.SetActive(true);
			this.GetScript<uGUISprite>("txtp_MoneyOneGacha/IconCurrency").ChangeSprite(m_contents.DataUseTicket.UseCurrencyIconSptName);
			this.GetScript<TextMeshProUGUI>("txtp_MoneyOneGacha").text = m_contents.DataUseTicket.CurrencyQuantity.ToString();
			this.GetScript<CustomButton>("BuyOneGacha/bt_BuyGacha").onClick.AddListener(DidTapOnceTicket);
        }
        // 通常の単回.
		else if(m_contents.Data != null){
			this.GetScript<TextMeshProUGUI>("txtp_MoneyOneGacha").gameObject.SetActive(true);         
			this.GetScript<uGUISprite>("txtp_MoneyOneGacha/IconCurrency").ChangeSprite(m_contents.Data.UseCurrencyIconSptName);
			this.GetScript<TextMeshProUGUI>("txtp_MoneyOneGacha").text = m_contents.Data.CurrencyQuantity.ToString();      
			this.GetScript<CustomButton>("BuyOneGacha/bt_BuyGacha").onClick.AddListener(DidTapOnce);
		}
        // 無料10連は10連ボタンの場所を利用.無料から引かせる.
        if(m_contents.DataFree10th != null && m_contents.DataFree10th.IsPurchasable){
            this.GetScript<TextMeshProUGUI>("txtp_Money10Gacha").text = m_contents.DataFree10th.CurrencyQuantity.ToString();         
			this.GetScript<uGUISprite>("txtp_Money10Gacha/IconCurrency").ChangeSprite(m_contents.DataFree10th.UseCurrencyIconSptName);
            this.GetScript<CustomButton>("Buy10Gacha/bt_BuyGacha").onClick.AddListener(DidTapFreeTen);
        }else if(m_contents.Data10th != null){
            this.GetScript<TextMeshProUGUI>("txtp_Money10Gacha").text = m_contents.Data10th.CurrencyQuantity.ToString();
			this.GetScript<uGUISprite>("txtp_Money10Gacha/IconCurrency").ChangeSprite(m_contents.Data10th.UseCurrencyIconSptName);
            this.GetScript<CustomButton>("Buy10Gacha/bt_BuyGacha").onClick.AddListener(DidTapTen);
        }
        if(m_contents.DataOncePerDayDiscount != null){
            this.GetScript<TextMeshProUGUI>("txtp_MoneyDayGacha").text = m_contents.DataOncePerDayDiscount.CurrencyQuantity.ToString();
			this.GetScript<uGUISprite>("txtp_Money10Gacha/IconCurrency").ChangeSprite(m_contents.DataOncePerDayDiscount.UseCurrencyIconSptName);
            this.GetScript<CustomButton>("bt_BuyDayGacha").onClick.AddListener(DidTapDiscount);
			this.GetScript<CustomButton>("bt_BuyDayGacha").interactable = m_contents.DataOncePerDayDiscount.IsPurchasable;
        }

		if(m_contents.Type.Enum == GachaTypeEnum.character_gacha){
			this.GetScript<TextMeshProUGUI>("txtp_GachaCountNum").SetTextFormat("{0}/{1}", m_contents.MissCount, m_contents.MissTriggerCount);
		}

		// 設定がない場合は非表示.
		if (this.Exist<RectTransform>("BuyOneGacha/txtp_FreeGacha")) {
			var value = m_contents.DataFree != null && m_contents.DataFree.IsPurchasable;
            this.GetScript<RectTransform>("BuyOneGacha/txtp_FreeGacha").gameObject.SetActive( value );
            this.GetScript<RectTransform>("BuyOneGacha/Exclamation").gameObject.SetActive( value );
        }
		this.GetScript<RectTransform>("OneGacha").gameObject.SetActive(m_contents.Data != null);
        this.GetScript<RectTransform>("Buy10Gacha").gameObject.SetActive(m_contents.Data10th != null);      
		if(m_contents.Type.Enum != GachaTypeEnum.weapon_gacha){
			this.GetScript<RectTransform>("DayGacha").gameObject.SetActive(m_contents.DataOncePerDayDiscount != null);
		}
        
        if(m_contents.Type.Enum == GachaTypeEnum.character_gacha){
            this.GetScript<CustomButton>("bt_Buy").onClick.AddListener(DidTapBuyGem);
        }      
	}
	private void ClearButtonLisners()
	{
		if (this.Exist<CustomButton>("BuyOneGacha/bt_BuyGacha")) {
            this.GetScript<CustomButton>("BuyOneGacha/bt_BuyGacha").onClick.RemoveAllListeners();
        }
        if (this.Exist<CustomButton>("Buy10Gacha/bt_BuyGacha")) {
            this.GetScript<CustomButton>("Buy10Gacha/bt_BuyGacha").onClick.RemoveAllListeners();
        }
        if (this.Exist<CustomButton>("Buy10Gacha/bt_BuyGacha")) {
            this.GetScript<CustomButton>("Buy10Gacha/bt_BuyGacha").onClick.RemoveAllListeners();
        }
        if (this.Exist<CustomButton>("bt_BuyDayGacha")) {
            this.GetScript<CustomButton>("bt_BuyDayGacha").onClick.RemoveAllListeners();
        }
        if (this.Exist<CustomButton>("bt_Buy")) {
            this.GetScript<CustomButton>("bt_Buy").onClick.RemoveAllListeners();
        }
	}

	private GachaClientUseData.ContentsForView m_contents;
	private Action<GachaClientUseData.ContentsForView.RowData, ReceiveGachaPurchaseProduct> m_didDrawGacha;
}
