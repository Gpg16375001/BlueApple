using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class UnitDetailsSController : ScreenControllerBase
{
    public CardData DisplayCard;
    public SupporterCardData DisplaySupporterCard;
    public bool IsSupportCard;
    public bool DispGlobalMenu;
    public System.Action BackSceneCallback;

    private UnitResourceLoader m_CardResource;
    private WeaponResourceLoader m_WeaponResource;

    public override void Init (System.Action<bool> didConnectEnd)
    {
        UnitResourceLoader loader = null;
        WeaponResourceLoader weaponLoader = null;
        if (DisplayCard != null) {
            loader = new UnitResourceLoader (DisplayCard);
            weaponLoader = new WeaponResourceLoader (
                DisplayCard.Weapon != null ?
                DisplayCard.Weapon.Weapon.id :
                DisplayCard.Card.initial_weapon_id
            );
        } else {
            loader = new UnitResourceLoader (DisplaySupporterCard.ConvertCardData());
            weaponLoader = new WeaponResourceLoader (
                DisplaySupporterCard.EquippedWeaponData != null && DisplaySupporterCard.EquippedWeaponData.BagId > 0 ?
                DisplaySupporterCard.EquippedWeaponData.Weapon.id :
                DisplaySupporterCard.Card.initial_weapon_id
            );
        }
        loader.IsLoadCardBg = true;
        loader.IsLoadPortrait = true;
        loader.IsLoadAnimationClip = true;
        loader.IsLoadLive2DModel = true;
        loader.IsLoadSpineModel = true;
        loader.IsLoadTimeLine = false;
        loader.IsLoadOriginalImage = true;

        loader.LoadResource ((resource) => {
            m_CardResource = resource;
            weaponLoader.LoadResource(
                (weaponResource) => {
                    m_WeaponResource = weaponResource;

                    if(DisplayCard != null) {
    					// カードごとの変更データも都度確認しておく
    					AwsModule.CardModifiedData.UpdateData(DisplayCard);
    					AwsModule.CardModifiedData.Sync((success, sender, e) => didConnectEnd(true));
                    } else {
                        didConnectEnd(true);
                    }
                },
                this
            );
        }, this);
    }

    public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/Screen_UnitDetails", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        m_screen = go.GetOrAddComponent<Screen_UnitDetails>();
        if (DisplayCard != null) {
            m_screen.Init (DisplayCard, DispGlobalMenu, IsSupportCard, BackSceneCallback, m_CardResource, m_WeaponResource);
        } else {
            m_screen.Init (DisplaySupporterCard, DispGlobalMenu, IsSupportCard, BackSceneCallback, m_CardResource, m_WeaponResource);
        }
    }

    public override void Dispose ()
    {
        if (m_CardResource != null) {
            m_CardResource.Dispose ();
        }
        if (m_WeaponResource != null) {
            m_WeaponResource.Dispose ();
        }
		if (m_screen != null) {
            m_screen.Dispose();
        }
        base.Dispose ();
    }

	private Screen_UnitDetails m_screen;
}
