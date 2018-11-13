using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

/// <summary>
/// pvp報酬リストアイテム
/// </summary>
public class ListItem_PVPAchievement : ViewBase
{
    public void Init(PvpLeague leagueInfo)
    {
        GetScript<TextMeshProUGUI>("txtp_AchievementPoint").SetText(leagueInfo.winning_point);
        GetScript<TextMeshProUGUI>("txtp_Gem").SetText(leagueInfo.quantity);

		// icon情報の取得
		ItemTypeEnum itemType = (ItemTypeEnum)leagueInfo.item_type;
		var iconInfo = itemType.GetIconInfo (leagueInfo.item_id.Value, leagueInfo.item_type.Value != (int)ItemTypeEnum.card && leagueInfo.item_type.Value != (int)ItemTypeEnum.weapon);
		GameObject iconGem = GetScript<RectTransform> ("IconGem").gameObject;
		if (iconInfo.IsEnableSprite) {
			//uGUISpriteを追加する
			var _uGUISprite = iconGem.AddComponent<uGUISprite> ();
			_uGUISprite.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
		} else if(iconInfo.IconObject != null) {
			//元いたイメージを無効に
			iconGem.GetComponent<UnityEngine.UI.Image> ().enabled = false;
			//元々のオブジェクトサイズが違うのでスケールを算出
			Vector3 size = Vector3.one;
			size.x = iconGem.GetComponent<RectTransform> ().sizeDelta.x / iconInfo.IconObject.GetComponent<RectTransform>().sizeDelta.x;
			size.y = iconGem.GetComponent<RectTransform> ().sizeDelta.y / iconInfo.IconObject.GetComponent<RectTransform>().sizeDelta.y;
			//縦横の値が同じになるように大きい方に調整
			if (size.x > size.y) {
				size.y = size.x;
			} else if (size.y > size.x) {
				size.y = size.x;
			}
			//オブジェクトの移動とパラメータの設定
			iconInfo.IconObject.transform.SetParent (iconGem.transform);
			iconInfo.IconObject.transform.localScale = size;
			iconInfo.IconObject.transform.localPosition = Vector3.zero;
			iconInfo.IconObject.transform.localRotation = Quaternion.identity;
		}
    }
}
