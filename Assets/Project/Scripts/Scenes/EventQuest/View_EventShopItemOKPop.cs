using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using TMPro;


/// <summary>
/// View : ショップアイテム購入後ポップ.
/// </summary>
public class View_EventShopItemOKPop : PopupViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_EventShopItemOKPop Create(EventQuestExchangeSetting setting, int num, Action didClose)
    {
        var go = GameObjectEx.LoadAndCreateObject("EventQuest/View_EventShopItemOKPop");
        var c = go.GetOrAddComponent<View_EventShopItemOKPop>();
        c.InitInternal(setting, num, didClose);
        return c;
    }
    private void InitInternal(EventQuestExchangeSetting setting, int num, Action didClose)
    {
        m_didClose = didClose;

        // アイコン設定
        SetIcon (setting);
        this.GetScript<TextMeshProUGUI>("txtp_ItemIconNum").SetText(setting.quantity * num);

        // ボタン.
        this.SetCanvasCustomButtonMsg("OK/bt_Common", DidTapOK);
        SetBackButton ();
    }

    void SetIcon(EventQuestExchangeSetting setting)
    {
        bool weaponOrUnit = setting.item_type == ItemTypeEnum.card || setting.item_type == ItemTypeEnum.weapon;
        var iconInfo = setting.item_type.GetIconInfo (setting.item_id, true);

        var otherIconRoot = GetScript<RectTransform> ("OtherIconRoot");
        var unitWeaponIconRoot = GetScript<RectTransform> ("UnitWeaponRoot");
        var ItemIcon = GetScript<uGUISprite> ("ItemIcon");

        otherIconRoot.gameObject.DestroyChildren ();
        unitWeaponIconRoot.gameObject.DestroyChildren ();

        if (iconInfo.IsEnableSprite) {
            // Spriteの画像のみ
            ItemIcon.gameObject.SetActive (true);
            otherIconRoot.gameObject.SetActive (false);
            unitWeaponIconRoot.gameObject.SetActive (false);

            ItemIcon.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (weaponOrUnit) {
            // ユニットや武器のアイコン
            ItemIcon.gameObject.SetActive (false);
            otherIconRoot.gameObject.SetActive (false);
            unitWeaponIconRoot.gameObject.SetActive (true);

            iconInfo.IconObject.transform.SetParent (unitWeaponIconRoot);
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        } else {
            // それ以外のアイコン
            ItemIcon.gameObject.SetActive (false);
            otherIconRoot.gameObject.SetActive (true);
            unitWeaponIconRoot.gameObject.SetActive (false);

            iconInfo.IconObject.transform.SetParent (otherIconRoot);
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void Close()
    {
        DidTapOK ();
    }

    protected override void DidBackButton ()
    {
        DidTapOK ();
    }

    // OKボタン
    void DidTapOK()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            if (m_didClose != null) {
                m_didClose();
            }
            this.Dispose();
        });
    }

    private Action m_didClose;
}
