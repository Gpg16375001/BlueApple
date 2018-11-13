using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_MagiDetailsPop : PopupViewBase {
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_MagiDetailsPop Create(MagikiteData magikite, Action<MagikiteData> didEquip, Action<string, int> didSale, Action<MagikiteData> lockChange)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_MagiDetailsPop");
        var c = go.GetOrAddComponent<View_MagiDetailsPop>();
        c.InitInternal(magikite, didEquip, didSale, lockChange);
        return c;
    }


    private void InitInternal(MagikiteData magikite, Action<MagikiteData> didEquip, Action<string, int> didSale, Action<MagikiteData> lockChange)
    {
        m_Magikite = magikite;
        m_DidEquip = didEquip;
        m_DidSale = didSale;
        m_LockChange = lockChange;
        // マギカイトアイコンの表示
        {
            var magikiteRoot = GetScript<RectTransform> ("MagiIcon").gameObject;
            var go = GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_MagiIcon", magikiteRoot);         
            var c = go.GetOrAddComponent<ListItem_MagiIcon> ();
            c.UpdateItem (magikite);
        }
            
        // ロックアイコン
        GetScript<RectTransform>("bt_LockOn").gameObject.SetActive(magikite.IsLocked);
        GetScript<RectTransform>("bt_LockOff").gameObject.SetActive(!magikite.IsLocked);
        SetCanvasCustomButtonMsg ("bt_LockOn", DidTapLockOn);
        SetCanvasCustomButtonMsg ("bt_LockOff", DidTapLockOff);


        var masterData = magikite.Magikite;

        // 名前
        GetScript<TextMeshProUGUI> ("txtp_Name").SetText (masterData.name);

        // スキルの設定
        if (masterData.skill_id.HasValue && MasterDataTable.skill[masterData.skill_id.Value] != null) {
            GetScript<TextMeshProUGUI> ("txtp_Skii").SetText (MasterDataTable.skill[masterData.skill_id.Value].flavor);
        } else {
            GetScript<TextMeshProUGUI> ("txtp_Skii").SetText ("なし");
        }

        // 装備者のアイコン表示
        if (magikite.IsEquipped && magikite.CardId >= 0) {
            var cardData = CardData.CacheGet (magikite.CardId);
            if (cardData != null) {
                var UnitIconRootObj = GetScript<RectTransform> ("UnitIcon").gameObject;
                var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", UnitIconRootObj);         
                var c = go.GetOrAddComponent<ListItem_UnitIcon> ();
                c.Init (cardData);
            }
        }

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);

        if (m_DidEquip == null) {
            GetScript<RectTransform> ("GoEquip").gameObject.SetActive (false);
        } else {
            GetScript<RectTransform> ("GoEquip").gameObject.SetActive (true);
            SetCanvasCustomButtonMsg ("GoEquip/bt_Sub3", DidTapEquip);
        }
        SetCanvasCustomButtonMsg ("Sale/bt_Common", DidTapSale, !magikite.IsEquipped && !magikite.IsLocked);
        SetBackButton ();
    }

    public void Close()
    {
        PlayOpenCloseAnimation (false, Dispose);
    }

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }
        Close ();
    }

    void DidTapLockOff()
    {
        if (IsClosed) {
            return;
        }
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        // Lockする
        SendAPI.MagikitesLockMagikite (m_Magikite.BagId,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }
                response.MagikiteData.CacheSet();
                m_Magikite = response.MagikiteData;

                ChangeLockState();

                if(m_LockChange != null) {
                    m_LockChange(m_Magikite);
                }
            }
        );
    }

    void DidTapLockOn()
    {
        if (IsClosed) {
            return;
        }
        // Unlockする
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        // Lockする
        SendAPI.MagikitesUnlockMagikite (m_Magikite.BagId,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }
                response.MagikiteData.CacheSet();
                m_Magikite = response.MagikiteData;

                ChangeLockState();

                if(m_LockChange != null) {
                    m_LockChange(m_Magikite);
                }
            }
        );
    }

    void ChangeLockState()
    {
        if (m_Magikite == null) {
            return;
        }
        GetScript<RectTransform>("bt_LockOn").gameObject.SetActive(m_Magikite.IsLocked);
        GetScript<RectTransform>("bt_LockOff").gameObject.SetActive(!m_Magikite.IsLocked);
        GetScript<CustomButton>("Sale/bt_Common").interactable = !m_Magikite.IsEquipped && !m_Magikite.IsLocked;
    }

    void DidTapEquip()
    {
        if (IsClosed) {
            return;
        }
        if (m_DidEquip != null) {
            m_DidEquip (m_Magikite);
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapSale()
    {
        if (IsClosed) {
            return;
        }
        // 確認ポップアップ
        if (m_Magikite.Magikite.rarity >= 3) {
            View_SaleAlertPop.CreateRarityAleart (m_Magikite,
                () => {
                    View_SaleAlertPop.CreateSingleSale (m_Magikite,
                        (price) => {
                            if (m_DidSale != null) {
                                m_DidSale (m_Magikite.Magikite.name, price);
                            }
                            Dispose ();
                        }
                    );
                }
            );
        } else {
            View_SaleAlertPop.CreateSingleSale (m_Magikite,
                (price) => {
                    if (m_DidSale != null) {
                        m_DidSale (m_Magikite.Magikite.name, price);
                    }
                    Dispose ();
                }
            );
        }
    }

    MagikiteData m_Magikite;
    Action<MagikiteData> m_DidEquip;
    Action<string, int> m_DidSale;
    Action<MagikiteData> m_LockChange;
}
