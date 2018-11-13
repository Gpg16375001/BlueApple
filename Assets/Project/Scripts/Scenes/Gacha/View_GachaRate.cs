using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : ガチャの確率表示.
/// </summary>
public class View_GachaRate : ViewBase
{
 
    /// <summary>
    /// 生成.同じgroupIdのレートはそのまま使い回す.
    /// </summary>
    public static void Create(int gachaId)
	{
		Debug.Log(gachaId);
		if(instance != null && groupId > 0){
			var groupList = MasterDataTable.gacha_group.DataList.FindAll(d => d.gacha_id == gachaId);
			if(groupId == groupList.First().id){
				instance.gameObject.SetActive(true);
				return;
			}else{
				instance.Dispose();
			}
		}
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaRate");
		var c = go.GetOrAddComponent<View_GachaRate>();
		c.InitInternal(gachaId);
		instance = c;
	}
	private void InitInternal(int gachaId)
	{      
		var groupList = MasterDataTable.gacha_group.DataList.FindAll(d => d.gacha_id == gachaId);
		groupId = groupList.First().id;      
		foreach(var data in groupList){
			var proInGroup = (float)data.hit_rate / groupList.Sum(g => (float)g.hit_rate) * 100f;
			var proInGroupR3 = (float)data.s10_last_hit_rate / groupList.Sum(g => (float)g.hit_rate) * 100f;
			var itemList = MasterDataTable.gacha_item.DataList.FindAll(d => d.group_id == data.id);
            if (itemList.First().item_type.Contains("カード")) {
                var checkCard = MasterDataTable.card[itemList.First().item_id];
                var rootRarity = this.GetScript<Transform>("RateR2/RateList");            
				if(checkCard.rarity <= 2){
					this.GetScript<TextMeshProUGUI>("Normal_R2Total/txtp_Rate").text = proInGroup.ToString("F2");
                    this.GetScript<TextMeshProUGUI>("R3_R2Total/txtp_Rate").text = "0";
                    this.GetScript<TextMeshProUGUI>("R4_R2Total/txtp_Rate").text = "0";
				}
                else if(checkCard.rarity <= 3) {
					rootRarity = this.GetScript<Transform>("RateR3/RateList"); 
					this.GetScript<TextMeshProUGUI>("Normal_R3Total/txtp_Rate").text = proInGroup.ToString("F2");
                    this.GetScript<TextMeshProUGUI>("R3_R3Total/txtp_Rate").text = proInGroupR3.ToString("F2");
                    this.GetScript<TextMeshProUGUI>("R4_R3Total/txtp_Rate").text = "0";              
                } else if (checkCard.rarity <= 4) {
					rootRarity = this.GetScript<Transform>("RateR4/RateList");
					this.GetScript<TextMeshProUGUI>("Normal_R4Total/txtp_Rate").text = proInGroup.ToString("F2");
					this.GetScript<TextMeshProUGUI>("R3_R4Total/txtp_Rate").text = proInGroupR3.ToString("F2");
					this.GetScript<TextMeshProUGUI>("R4_R4Total/txtp_Rate").text = "100";
                }
                foreach (var item in itemList) {
                    var go = GameObjectEx.LoadAndCreateObject("Gacha/ListItem_GachaRate", rootRarity.gameObject);
                    var c = go.GetOrAddComponent<ListItem_GachaRate>();
					c.Init(new CardData(MasterDataTable.card[item.item_id]), data, item, groupList, itemList);
                }
            }
		} 

		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
	}

	// ボタン : 閉じる.
    void DidTapClose()
	{
		this.StartCoroutine(this.CoPlayOpenClose());
	}   
	IEnumerator CoPlayOpenClose()
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		this.gameObject.SetActive(false);
    }

    void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	private static View_GachaRate instance;
	private static int groupId = 0;
}
