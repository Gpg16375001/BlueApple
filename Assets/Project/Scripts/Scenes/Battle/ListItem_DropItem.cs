using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


using SmileLab;

public class ListItem_DropItem : ViewBase 
{
    public void Init(BattleDropItem dropItem)
    {
        var iconInfo = dropItem.reward_type.GetIconInfo (dropItem.reward_id,
            dropItem.reward_type != ItemTypeEnum.card && dropItem.reward_type != ItemTypeEnum.weapon);

        if (iconInfo.IsEnableSprite) {
            GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);

            var uGuiSprite = GetScript<uGUISprite> ("ItemAnchor");
            uGuiSprite.gameObject.SetActive (true);
            uGuiSprite.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (iconInfo.IconObject != null) {
            Transform root;
            if (dropItem.reward_type != ItemTypeEnum.card && dropItem.reward_type != ItemTypeEnum.weapon) {
                GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                var image = GetScript<Image> ("ItemAnchor");
                image.enabled = false;
                image.gameObject.SetActive (true);
                root = image.transform;
            } else {
                GetScript<Image> ("ItemAnchor").gameObject.SetActive(false);
                root = GetScript<RectTransform> ("UnitWeaponRoot");
                root.gameObject.SetActive (true);
            }

            iconInfo.IconObject.transform.SetParent (root);
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        }
        GetScript<TextMeshProUGUI> ("txtp_DropItemName").SetText (dropItem.reward_type.GetName (dropItem.reward_id));
        GetScript<TextMeshProUGUI> ("txtp_DropNum").SetText (dropItem.quantity.ToString());
    }
}
