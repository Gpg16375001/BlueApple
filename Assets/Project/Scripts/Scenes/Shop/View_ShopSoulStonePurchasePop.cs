using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_ShopSoulStonePurchasePop : PopupViewBase
{
    public static View_ShopSoulStonePurchasePop Create(CardBasedMaterial data, int rowMaterialcount, Action<int> didExchange)
    {
        var go = GameObjectEx.LoadAndCreateObject("Shop/View_ShopSoulStonePurchasePop");
        var c = go.GetOrAddComponent<View_ShopSoulStonePurchasePop>();
        c.InitInternal(data, rowMaterialcount, didExchange);
        return c;
    }

    private void InitInternal(CardBasedMaterial data, int rowMaterialCount, Action<int> didExchange)
    {
        m_DidExchange = didExchange;
        m_Data = data;
        m_SelectNum = 1;
        m_NeedNum = data.card_based_row_material_need_number;
        m_HasCard = CardData.CacheGet (data.card_id) != null;
        if (!m_HasCard) {
            m_ExchangeLimitNum = 1;
        } else {
			var capacity = MasterDataTable.CommonDefine["MATERIAL_CAPACITY"].define_value;
			var haveCnt = MaterialData.CacheGet(data.card_based_material_id) != null ? MaterialData.CacheGet(data.card_based_material_id).Count: 0;         
			m_ExchangeLimitNum = Math.Min((rowMaterialCount / m_NeedNum), (capacity - haveCnt));
        }
        var cardBasedMaterial = MaterialData.CacheGet (data.card_based_material_id);
        int cardBasedMaterialCount = 0;
        if (cardBasedMaterial != null) {
            cardBasedMaterialCount = cardBasedMaterial.Count;
        }

        // 
        GetScript<Image> ("Icon").overrideSprite = null;
        IconLoader.LoadMaterial (m_Data.card_based_material_id, LoadedIcon);

        GetScript<Image> ("StockSoulPiece/CurrencyIcon").overrideSprite = null;
        GetScript<Image> ("Total/CurrencyIcon").overrideSprite = null;
        IconLoader.LoadMaterial (m_Data.card_based_row_material_id, LoadedSoulPieceIcon);
        GetScript<TextMeshProUGUI> ("txtp_SoulStoneName").SetText (MasterDataTable.chara_material[data.card_based_material_id].name);

        // 所持数
        GetScript<TextMeshProUGUI> ("txtp_StockSoulPieceNum").SetText (rowMaterialCount);
        GetScript<TextMeshProUGUI> ("txtp_StockNum").SetText (cardBasedMaterialCount);
        
		SetPurchaseInfo();

        SetCanvasCustomButtonMsg ("bt_Plus", DidTapPlus);
        SetCanvasCustomButtonMsg ("bt_Minus", DidTapMinus);

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("Cancel/bt_Common", DidTapClose);
        SetCanvasCustomButtonMsg ("Buy/bt_Common", DidTapBuy);
		this.GetScript<CustomButton>("Buy/bt_Common").interactable = m_NeedNum <= rowMaterialCount && m_SelectNum <= m_ExchangeLimitNum;
		this.GetScript<CustomButton>("bt_Plus").interactable = m_SelectNum < m_ExchangeLimitNum;
		this.GetScript<CustomButton>("bt_Minus").interactable = false;
        SetBackButton ();
        PlayOpenCloseAnimation (true);
    }

    void SetPurchaseInfo()
    {
		GetScript<TextMeshProUGUI> ("txtp_SelectTotalNum").SetText (m_SelectNum);
        GetScript<TextMeshProUGUI> ("txtp_TotalNumLimit").SetText (m_ExchangeLimitNum);
        GetScript<TextMeshProUGUI> ("txtp_TotalSoulPieceNum").SetText (m_SelectNum * m_NeedNum);      
    }

    void LoadedIcon(IconLoadSetting data, Sprite icon)
    {
        GetScript<Image> ("Icon").overrideSprite = icon;
    }

    void LoadedSoulPieceIcon(IconLoadSetting data, Sprite icon)
    {
        GetScript<Image> ("StockSoulPiece/CurrencyIcon").overrideSprite = icon;
        GetScript<Image> ("Total/CurrencyIcon").overrideSprite = icon;
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    void DidTapPlus()
    {
        if (IsClosed) {
            return;
        }
		m_SelectNum = Mathf.Min(Mathf.Max(1, m_ExchangeLimitNum), m_SelectNum + 1);
        this.GetScript<CustomButton>("bt_Minus").interactable = m_SelectNum > 1;
		this.GetScript<CustomButton>("bt_Plus").interactable = m_SelectNum < m_ExchangeLimitNum;
        SetPurchaseInfo ();
    }

    void DidTapMinus()
    {
        if (IsClosed) {
            return;
        }
        m_SelectNum = Mathf.Max(1, m_SelectNum - 1);
		this.GetScript<CustomButton>("bt_Minus").interactable = m_SelectNum > 1;
		this.GetScript<CustomButton>("bt_Plus").interactable = m_SelectNum < m_ExchangeLimitNum;
        SetPurchaseInfo ();
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapBuy()
    {
        if (IsClosed) {
            return;
        }

		if (m_SelectNum > m_ExchangeLimitNum) {
            return;
        }
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        SendAPI.ShopTradeMaterial (m_Data.card_id, m_SelectNum,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }
                if(response.CardData != null && response.CardData.CardId != 0) {
                    response.CardData.CacheSet();
                }
                if(response.MaterialData != null && response.MaterialData.MaterialId != 0) {
                    response.MaterialData.CacheSet();
                }

                var rowMaterial = MaterialData.CacheGet(m_Data.card_based_row_material_id);
                if(rowMaterial != null) {
                    rowMaterial.Count = Mathf.Max(0, rowMaterial.Count - m_SelectNum * m_NeedNum);
                    rowMaterial.CacheSet();
                    if (m_DidExchange != null) {
                        m_DidExchange (rowMaterial.Count);
                    }
                }
                PlayOpenCloseAnimation (false, () => {
                    // カードを手に入れた場合は自己紹介を流す
                    if(response.CardData != null && response.CardData.CardId != 0) {
                        PlayCharacterIntroAdv(response.CardData.CardId, Dispose);
                    } else {
                        PopupManager.OpenPopupOK("交換しました。");
                        Dispose();
                    }
                });
            }
        );
    }

    // 自己紹介
    private void PlayCharacterIntroAdv(int cardId, System.Action didEnd)
    {
        this.StartCoroutine(CoPlayCharacterIntroAdv(cardId, didEnd));
    }
    private IEnumerator CoPlayCharacterIntroAdv(int cardId, System.Action didEnd)
    {
        var bg = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaCharacterIntro");
        bg.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);

        UtageModule.SharedInstance.SetActiveCore(true);
        var bPlayScenario = false;

        bPlayScenario = true;
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips (View_FadePanel.FadeColor.Black, () => {
            UtageModule.SharedInstance.LoadUseChapter (cardId.ToString (), () => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                UtageModule.SharedInstance.StartScenario ("intro", () => {
                    bPlayScenario = false;
                }, true);            
            });
        });
        while(bPlayScenario){
            // スキップを全員分にする.
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
                LockInputManager.SharedInstance.IsLock = false;
                yield break;
            }
            yield return null;
        }

        UtageModule.SharedInstance.SetActiveCore(false);
        GameObject.Destroy(bg);

        didEnd();
    }

    void OnDestory()
    {
        IconLoader.RemoveLoadedEvent(ItemTypeEnum.material, m_Data.card_based_material_id, LoadedIcon);
        IconLoader.RemoveLoadedEvent(ItemTypeEnum.material, m_Data.card_based_row_material_id, LoadedSoulPieceIcon);
    }

    Action<int> m_DidExchange;
    CardBasedMaterial m_Data;
    int m_SelectNum;
    int m_NeedNum;
    int m_ExchangeLimitNum;
    bool m_HasCard = false;
}
