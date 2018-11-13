using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;

public class ListItem_LoginBonusDailyItem : ViewBase
{
    public void Init(bool isToday, bool doRecived, int? count, LoginbonusDistItem item)
    {
        var imgTodayMark = GetScript<Image> ("img_TodayMark");
        var imgLoginStamp = GetScript<Image> ("img_LoginStamp");
        var txtpNum = GetScript<TextMeshProUGUI> ("txtp_Num");
        var txtpDay = GetScript<TextMeshProUGUI> ("txtp_Day");

        imgTodayMark.gameObject.SetActive (isToday);
        imgLoginStamp.gameObject.SetActive (doRecived);

        var iconInfo = item.item_type.GetIconInfo (item.item_id);
        if (iconInfo.IsEnableSprite) {
            GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive(false);
            GetScript<uGUISprite> ("ItemIcon").LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (iconInfo.IconObject != null) {
            GetScript<RectTransform> ("ItemIcon").gameObject.SetActive(false);

            var iconRoot = GetScript<RectTransform> ("UnitWeaponRoot");
            iconRoot.gameObject.SetActive (true);
            iconInfo.IconObject.transform.SetParent (iconRoot.transform);
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        }

        if (count.HasValue) {
            txtpDay.SetText (count.Value);
        } else {
            txtpDay.gameObject.SetActive(false);
            GetScript<RectTransform>("img_LoginBonusDayTitle").gameObject.SetActive(false);
        }
        txtpNum.SetText(string.Format ("×{0}", item.quantity));
    }
}
