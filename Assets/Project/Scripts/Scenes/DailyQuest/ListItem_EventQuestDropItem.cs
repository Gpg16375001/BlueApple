using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using SmileLab;

/// <summary>
/// 曜日クエストドロップアイテム用アイコン
/// </summary>
public class ListItem_EventQuestDropItem : ViewBase {
    public void Init(DailyQuestRewardInfo info)
    {
        var iconInfo = info.reward_type.GetIconInfo (info.reward_id,
            info.reward_type != ItemTypeEnum.card && info.reward_type != ItemTypeEnum.weapon);

        if (iconInfo.IsEnableSprite) {
            GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
            var uGuiSprite = GetScript<uGUISprite> ("ItemIcon");
            uGuiSprite.gameObject.SetActive (true);
            uGuiSprite.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (iconInfo.IconObject != null) {
            Transform root;
            if (info.reward_type != ItemTypeEnum.card && info.reward_type != ItemTypeEnum.weapon) {
                GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                var image = GetScript<Image> ("ItemIcon");
                image.enabled = false;
                image.gameObject.SetActive (true);
                root = image.transform;
            } else {
                GetScript<Image> ("ItemIcon").gameObject.SetActive(false);
                root = GetScript<RectTransform> ("UnitWeaponRoot");
                root.gameObject.SetActive (true);
            }

            iconInfo.IconObject.transform.SetParent (root);
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        }
        GetScript<TextMeshProUGUI> ("txtp_ItemName").SetText (info.reward_type.GetName (info.reward_id));
    }
}
