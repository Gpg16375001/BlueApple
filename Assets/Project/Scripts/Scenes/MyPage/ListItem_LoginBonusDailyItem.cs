using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.U2D;

using TMPro;

using SmileLab;

public class ListItem_LoginBonusDailyItem : ViewBase
{
    #if false // 2018/11/30 Ogata
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
    #endif

    public void Init(bool isToday, bool doRecived, int? count, LoginbonusDistPackage item)
    {
        if (Exist<RectTransform> ("img_TodayMark")) {
            GetScript<RectTransform> ("img_TodayMark").gameObject.SetActive (isToday);
        }

        if (Exist<RectTransform> ("img_LoginStamp")) {
            GetScript<RectTransform> ("img_LoginStamp").gameObject.SetActive (doRecived);;
        }

        TextMeshProUGUI txtpNum = null;
        if (Exist<TextMeshProUGUI> ("txtp_Num")) {
            txtpNum = GetScript<TextMeshProUGUI> ("txtp_Num");
        }

        TextMeshProUGUI txtpDay = null;
        if (Exist<TextMeshProUGUI> ("txtp_Day")) {
            txtpDay = GetScript<TextMeshProUGUI> ("txtp_Day");
            if (count.HasValue) {
                txtpDay.SetText (count.Value);
            }
            txtpDay.gameObject.SetActive(count.HasValue);
            GetScript<RectTransform>("img_LoginBonusDayTitle").gameObject.SetActive(count.HasValue);
        }

        // >>>> アイテム名 <<<<
        TextMeshProUGUI txtpItemName = null;
        if (Exist<TextMeshProUGUI> ("txtp_ItemName")) {
            txtpItemName = GetScript<TextMeshProUGUI> ("txtp_ItemName");
        }

        // >>>> アイテムアイコンのロード <<<<
        var itemIcon = GetScript<RectTransform> ("ItemIcon").gameObject;
        var loadPackageIcon = item.package_icon_id.HasValue;
        SpriteAtlas iconAtlas = null;
        if (loadPackageIcon) {
            // パッケージ用のアイコンIDが設定されている場合、アトラスをロードする
            DLCManager.AssetBundleFromFileOrDownload (
                DLCManager.DLC_FOLDER.UI,
                "bundle_loginbonus",
                bundleRef => {
                    iconAtlas = bundleRef.assetbundle.LoadAsset<SpriteAtlas> ("loginbonus");
                    itemIcon.GetComponent<Image> ().sprite = iconAtlas.GetSprite (item.package_icon_id.Value.ToString ());
                    itemIcon.GetComponent<uGUISprite> ().enabled = false;
                }
            );
			if(txtpItemName != null) {
	            // アイテム名は隠す
	            txtpItemName.gameObject.SetActive (false);
			}
			if(txtpNum != null) {
	            // 数は「×1」で固定
	            txtpNum.SetText ("×1");
	            txtpNum.gameObject.SetActive (true);
			}
        } else {
            // その他 (パッケージ内で一番最初に設定されているデータを取得)
            var detailList = MasterDataTable.loginbonus_dist_package_detail.Get(item.package_id);
            var detailItem = detailList.FirstOrDefault(x => x.package_index == detailList.Min (y => y.package_index));
            if (detailItem != null) {
                int itemIconId = detailItem.item_id;
                var iconInfo = detailItem.item_type.GetIconInfo (detailItem.item_id);

                if (iconInfo.IsEnableSprite) {
                    GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                    GetScript<uGUISprite> ("ItemIcon").LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
                } else if (iconInfo.IconObject != null) {
                    itemIcon.SetActive (false);
                    var iconRoot = GetScript<RectTransform> ("UnitWeaponRoot");
                    iconRoot.gameObject.SetActive (true);
                    iconInfo.IconObject.transform.SetParent (iconRoot.transform);
                    iconInfo.IconObject.transform.localScale = Vector3.one;
                    iconInfo.IconObject.transform.localPosition = Vector3.zero;
                    iconInfo.IconObject.transform.localRotation = Quaternion.identity;
                }
                if (txtpItemName != null) {
                    // 名前の代入
                    txtpItemName.SetText (detailItem.item_type.GetName (detailItem.item_id));
                    txtpItemName.gameObject.SetActive (true);
                }
                if (txtpNum != null) {
                    // 数の代入
                    txtpNum.SetText (string.Format ("×{0}", detailItem.quantity));
                    txtpNum.gameObject.SetActive (true);
                }
            }
        }
    }
}
