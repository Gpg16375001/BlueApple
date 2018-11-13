using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_PointRecoveryItem : ViewBase {
    public void Init(UserPointTypeEnum userPointType, ConsumerItem item, int count, Action<object> didHeal)
    {
        m_Item = item;
        m_IsGem = false;
        m_PointType = userPointType;
        m_DidHeal = didHeal;

        GetScript<RectTransform> ("IconNameLayout").gameObject.SetActive (false);
        GetScript<RectTransform> ("WalletAPItem").gameObject.SetActive (true);
        GetScript<RectTransform> ("txtp_ItemName").gameObject.SetActive(true);

        GetScript<TextMeshProUGUI> ("txtp_ItemName").SetText(item.name);
        GetScript<TextMeshProUGUI> ("txtp_Wallet").SetText(count);

        ConsumerHealingEffect healingEffect = MasterDataTable.consumer_healing_effect [item.index];
        if(healingEffect == null || healingEffect.increment <= 0) {
            GetScript<TextMeshProUGUI> ("txtp_ItemNotes").SetTextFormat ("1個使用\n{0}全回復", userPointType.ToString());
        } else {
            GetScript<TextMeshProUGUI> ("txtp_ItemNotes").SetTextFormat ("1個使用\n{0}{1}回復", userPointType.ToString(), healingEffect.increment);
        }

        GetScript<Image> ("WhitePanel").overrideSprite = null;
        IconLoader.LoadConsumer(item.index, LoadedIconSprite);
        SetCanvasCustomButtonMsg ("Use/bt_CommonS02", DidTapHeal);
    }

    void LoadedIconSprite(IconLoadSetting data, Sprite icon)
    {
        if (m_Item.index == data.id && data.type == ItemTypeEnum.consumer) {
            GetScript<Image> ("WhitePanel").overrideSprite = icon;
        }
    }

    void OnDestory()
    {
        if (m_Item != null) {
            IconLoader.RemoveLoadedEvent(ItemTypeEnum.consumer, m_Item.index, LoadedIconSprite);
        }
    }

    public void Init(UserPointTypeEnum userPointType, int gemCount, Action<object> didHeal)
    {
        m_Item = null;
        m_IsGem = true;
        m_PointType = userPointType;
        m_DidHeal = didHeal;

        GetScript<RectTransform> ("txtp_ItemName").gameObject.SetActive(false);
        GetScript<RectTransform> ("WalletAPItem").gameObject.SetActive (false);
        GetScript<RectTransform> ("IconNameLayout").gameObject.SetActive (true);

        GetScript<TextMeshProUGUI> ("IconNameLayout/txtp_Num").SetText(gemCount);
        GetScript<TextMeshProUGUI> ("txtp_ItemNotes").SetTextFormat ("{0}個使用\n{1}全回復", gemCount, userPointType.ToString());

        var currencyIcon = Resources.Load ("Atlases/CurrencyIcon") as SpriteAtlas;
        GetScript<Image> ("WhitePanel").overrideSprite = currencyIcon.GetSprite("IconGem");
        SetCanvasCustomButtonMsg ("Use/bt_CommonS02", DidTapHeal);
    }


    void DidTapHeal()
    {
        if (m_PointType == UserPointTypeEnum.AP) {
            if (m_IsGem) {
                // TODO: ジェムで回復を繋ぐ
                HealAPGem();
            } else {
                HealAPItem ();
            }
        } else if (m_PointType == UserPointTypeEnum.BP) {
            if (m_IsGem) {
                // TODO: ジェムで回復を繋ぐ
                HealBPGem();
            } else {
                HealBPItem ();
            }
        }
    }

    void HealAPGem()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        SendAPI.UsersHealActionPointWithGem (
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }
                    
                AwsModule.UserData.UserData = response.UserData;

                if (m_DidHeal != null) {
                    m_DidHeal (response.UserData);
                }
            }
        );
    }

    void HealAPItem()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        SendAPI.UsersHealActionPoint (m_Item.index, 1,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }

                AwsModule.UserData.UserData = response.UserData;

                var consumerData = ConsumerData.CacheGet (m_Item.index);
                consumerData.Count -= 1;
                consumerData.CacheSet();

                if (m_DidHeal != null) {
                    m_DidHeal (response.UserData);
                }
            }
        );
    }

    void HealBPGem()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        SendAPI.UsersHealBattlePointWithGem (
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }

                AwsModule.UserData.UserData = response.UserData;
                if (m_DidHeal != null) {
                    m_DidHeal (response.PvpUserData);
                }
            }
        );
    }

    void HealBPItem()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        SendAPI.UsersHealBattlePoint (m_Item.index, 1,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }

                var consumerData = ConsumerData.CacheGet (m_Item.index);
                consumerData.Count -= 1;
                consumerData.CacheSet();

                AwsModule.UserData.UserData = response.UserData;
                if (m_DidHeal != null) {
                    m_DidHeal (response.PvpUserData);
                }
            }
        );
    }


    UserPointTypeEnum m_PointType;
    ConsumerItem m_Item;
    bool m_IsGem;
    Action<object> m_DidHeal;
}
