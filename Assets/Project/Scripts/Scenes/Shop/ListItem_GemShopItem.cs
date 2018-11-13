using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_GemShopItem : ViewBase {

    public void Init(SkuItem item, GemProductData[] productDataList, bool isLimitation)
    {
        // TODO: サーバーからのデータも加味して修正
        var storeSetting = MasterDataTable.gem_store_setting.DataList.FirstOrDefault(x => x.store_product_id == item.productID);
        var gemData = MasterDataTable.gem_product [storeSetting.app_product_id];

        var severData = productDataList.FirstOrDefault (x => x.GemProductId == storeSetting.app_product_id);

        var bonusData = MasterDataTable.bonus_gem [severData.BonusId];
        // データがない場合は自信を非アクティブにして終了
        if (gemData == null) {
            gameObject.SetActive (false);
            return;
        }
        m_Item = item;
        m_IsLimitation = isLimitation;

        // iconの設定
        gemData.LoadIcon (spt => GetScript<Image> ("ItemIcon/WhitePanel").overrideSprite = spt);

        int totalGem = gemData.gem_count + (bonusData != null ? bonusData.gem_count : 0);
        GetScript<TextMeshProUGUI> ("txtp_Gem").SetText (totalGem);

        GetScript<TextMeshProUGUI> ("Toll/txtp_GemNum").SetText (gemData.gem_count);
        if (bonusData == null) {
            GetScript<TextMeshProUGUI> ("txtp_BonusText").gameObject.SetActive(false);
        } else {
            GetScript<TextMeshProUGUI> ("txtp_BonusText").gameObject.SetActive(true);
            GetScript<TextMeshProUGUI> ("txtp_BonusText").SetText (bonusData.catch_copy);
            GetScript<TextMeshProUGUI> ("Free/txtp_GemNum").SetText (bonusData.gem_count);
        }
        GetScript<TextMeshProUGUI> ("txtp_Price").SetText (item.formattedPrice);

        var buyButton = GetScript<CustomButton> ("Buy/bt_Common02");
        buyButton.interactable = severData.IsPurchasable;
        buyButton.onClick.AddListener(DidTapBuy);

        var limitation = MasterDataTable.purchase_limitation.DataList.FirstOrDefault (x => x.index == severData.PurchaseLimitation);
        if (limitation != null && limitation.Enum != PurchaseLimitationEnum.unlimited) {
            GetScript<RectTransform> ("Limit").gameObject.SetActive (true);
            if (limitation.Enum != PurchaseLimitationEnum.timelimit) {
                GetScript<RectTransform> ("Remaining").gameObject.SetActive (true);
                GetScript<TextMeshProUGUI> ("txtp_LimitNum").SetText (severData.MaxPurchaseQuantity);
            } else {
                GetScript<RectTransform> ("Remaining").gameObject.SetActive (false);
            }

            var txtpTimeLimit = GetScript<TextMeshProUGUI> ("txtp_TimeLimit");
            if (!string.IsNullOrEmpty (severData.EndDate)) {
                System.DateTime endDate;
                if (System.DateTime.TryParse (severData.EndDate, out endDate)) {
                    txtpTimeLimit.gameObject.SetActive (true);

                    txtpTimeLimit.SetText (endDate.ToString ("MM/dd hh:mm"));
                } else {
                    txtpTimeLimit.gameObject.SetActive (false);
                }
            } else {
                txtpTimeLimit.gameObject.SetActive (false);
            }
        } else {
            GetScript<RectTransform> ("Limit").gameObject.SetActive (false);
        }
    }

    void DidTapBuy()
    {
        if (m_IsLimitation) {
            PopupManager.OpenPopupOK ("今月の購入上限に達しました。\n購入は月が替わるまで\nお待ちください。");
            return;
        }

		var limit = MasterDataTable.CommonDefine["GEM_CAPACITY"].define_value;
		var gemSetting = MasterDataTable.gem_store_setting.DataList.Find(s => s.store_product_id == m_Item.productID);
		var totalValue = AwsModule.UserData.UserData.PaidGemCount + MasterDataTable.gem_product[gemSetting.app_product_id].gem_count;
		Debug.Log("limit=" + limit + " / totalValue=" + totalValue);
		if(totalValue > limit){
			PopupManager.OpenPopupOK(string.Format("{0}が所持上限を超過します。\n購入できません。", MasterDataTable.item_type.DataList.Find(i => i.Enum == ItemTypeEnum.paid_gem).display_name));
            return;
		}

        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;
        PurchaseManager.SharedInstance.BuyItem (m_Item);
    }

    private bool m_IsLimitation;
    private SkuItem m_Item;
}
