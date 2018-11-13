using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ListItem : ガチャ確率表示用リストアイテム.
/// </summary>
public class ListItem_GachaRate : ViewBase
{

	// TODO : 一旦カードデータだけで初期化
	public void Init(CardData card, GachaGroup group, GachaItem item, List<GachaGroup> groupList, List<GachaItem> itemList)
	{
		// ユニットアイコン
		var rootObj = this.GetScript<RectTransform>("UnitIcon").gameObject;
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", rootObj);
        go.GetOrAddComponent<ListItem_UnitIcon>().Init(card);

		// 名前.
		this.GetScript<TextMeshProUGUI>("txtp_UnitNameSub").text = card.Card.alias;
		this.GetScript<TextMeshProUGUI>("txtp_UnitName").text = card.Card.nickname;
        
		// レアリティの設定
        int maxRarity = card.MaxRarity;
        int nowRarity = card.Rarity;
        for (int i = 1; i <= 6; ++i) {
            var starObj = this.GetScript<Transform>(string.Format("StarGrid/Star{0}", i));
            if (maxRarity >= i) {
                starObj.gameObject.SetActive(true);
                var starOn = this.GetScript<Transform>(string.Format("Star{0}/RarityStarOn", i));
                var starOff = this.GetScript<Transform>(string.Format("Star{0}/RarityStarOff", i));
                if (nowRarity >= i) {
                    starOn.gameObject.SetActive(true);
                    starOff.gameObject.SetActive(false);
                } else {
                    starOn.gameObject.SetActive(false);
                    starOff.gameObject.SetActive(true);
                }
            } else {
                starObj.gameObject.SetActive(false);
            }
        }

		// TODO : 確率表示.マスターが来たらちゃんと入力する.      
		var proInGroup = (float)group.hit_rate / groupList.Sum(g => (float)g.hit_rate) * 100f;
		var proInAllItem = ((float)item.hit_rate / itemList.Sum(i => (float)i.hit_rate)) * 100f;
		var probability = proInAllItem * proInGroup / 100f;
		if(nowRarity <= 2){
			this.GetScript<TextMeshProUGUI>("Normal/txtp_Rate").text = probability.ToString("F2");
			this.GetScript<TextMeshProUGUI>("R3/txtp_Rate").text = "0";
			this.GetScript<TextMeshProUGUI>("R4/txtp_Rate").text = "0";
		}else if(nowRarity <= 3){
			var proInGroupR3 = (float)group.s10_last_hit_rate / groupList.Sum(g => (float)g.hit_rate) * 100f;
			var probabilityR3 = proInAllItem * proInGroupR3 / 100f;
			this.GetScript<TextMeshProUGUI>("Normal/txtp_Rate").text = probability.ToString("F2");
			this.GetScript<TextMeshProUGUI>("R3/txtp_Rate").text = probabilityR3.ToString("F2");
            this.GetScript<TextMeshProUGUI>("R4/txtp_Rate").text = "0";
		}else if(nowRarity <= 4){
			var proInGroupR3 = (float)group.s10_last_hit_rate / groupList.Sum(g => (float)g.hit_rate) * 100f;
            var probabilityR3 = proInAllItem * proInGroupR3 / 100f;
			this.GetScript<TextMeshProUGUI>("Normal/txtp_Rate").text = probability.ToString("F2");
			this.GetScript<TextMeshProUGUI>("R3/txtp_Rate").text = probabilityR3.ToString("F2");
			this.GetScript<TextMeshProUGUI>("R4/txtp_Rate").text = proInAllItem.ToString("F2");
		}      
	}

}
