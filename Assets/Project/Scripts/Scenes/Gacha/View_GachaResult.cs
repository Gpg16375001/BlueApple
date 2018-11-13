using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : ガチャリザルト画面.
/// </summary>
public class View_GachaResult : ViewBase
{

	/// <summary>
	/// 生成.シングルトン.重複生成時は前回生成したものを強制破棄する.
	/// </summary>
	public static View_GachaResult Create(GachaClientUseData.ContentsForView.RowData data, ReceiveGachaPurchaseProduct response, Action didClose, Action oneMoreProc)
	{
		if (instance != null) {
			instance.Dispose();
		}
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaResult");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        instance = go.GetOrAddComponent<View_GachaResult>();
		instance.InitInternal(data, response, didClose, oneMoreProc);
		return instance;
	}
	private void InitInternal(GachaClientUseData.ContentsForView.RowData data, ReceiveGachaPurchaseProduct response, Action didClose, Action oneMoreProc)
	{
		m_res = response;
		m_didClose = didClose;
		m_oneMoreProc = oneMoreProc;

		var list = new List<AcquiredGachaItemData>(m_res.AcquiredGachaItemDataList);
		if(m_res.RarestCardGachaItemData != null){
			list.Add(m_res.RarestCardGachaItemData);
		}
		this.CreateResultItemList(list.ToArray());

		this.GetScript<TextMeshProUGUI>("OneMore/txtp_GachaTitle").SetTextFormat("{0}回ガチャ", list.Count <= 2 ? "1": "10");

		// ボタン.
		this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapClose);
		this.GetScript<RectTransform>("OneMore").gameObject.SetActive(data.IsPurchasable);
		if (data.IsPurchasable) {
			this.GetScript<CustomButton>("OneMore/bt_Common").onClick.AddListener(DidTapOneMore);
		}
	}

	/// <summary>
	/// 生成.チュートリアル.
	/// </summary>
    public static View_GachaResult Create(ReceiveGachaExecuteTutorialGacha response, Action didClose)
	{
		if (instance != null) {
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaResult");
        instance = go.GetOrAddComponent<View_GachaResult>();
        instance.InitInternal(response, didClose);
        return instance;      
	}
    private void InitInternal(ReceiveGachaExecuteTutorialGacha response, Action didClose)
	{ 
		m_didClose = didClose;

        this.CreateResultItemList(response.AcquiredGachaItemDataList);

        // ボタン.
		this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapClose);
        this.GetScript<RectTransform>("OneMore").gameObject.SetActive(false);
	}

	public override void Dispose()
	{
		foreach(var listItem in this.GetComponentsInChildren<ListItem_UnitIcon>(true)){
			listItem.Dispose();
		}
		// 繰り返していくと重くなるとのことなのでこのタイミングで使わなくなったResourcesを解放しておく.
		IconLoader.Dispose();      
        Resources.UnloadUnusedAssets();
		IconLoader.Init();
		base.Dispose();
	}

	#region ButtonDelegate.

	// ボタン : もう一回引く
	void DidTapOneMore()
	{ 
		if(m_oneMoreProc != null){
			m_oneMoreProc();
		}      
	}

	// ボタン : 閉じる.
    void DidTapClose()
	{
		if(m_didClose != null){
			m_didClose();
		}
		Dispose();
	}

    #endregion

	// リスト生成.
	private void CreateResultItemList(AcquiredGachaItemData[] itemList)
	{      
		var parent = itemList.Length <= 2 ? "One" : "Ten";
		this.GetScript<RectTransform>(parent).gameObject.SetActive(true);
		for (var i = 0; i < itemList.Length; ++i){
			var anchor = this.GetScript<RectTransform>(string.Format("{0}/{1}", parent, (i+1).ToString("d2")));
			var item = itemList[i];

            // 本体.
			var obj = CreateResultObject(item);
			anchor.gameObject.SetActive(true);
			obj.transform.SetParent(anchor, false);

			// 演出.
			ListItem_GachaResultEffect.Mode mode = ListItem_GachaResultEffect.Mode.None;
			if(item.ItemType == (int)ItemTypeEnum.card){
				var card = MasterDataTable.card[item.ItemId];
				if(card.rarity >= 4){
					mode |= ListItem_GachaResultEffect.Mode.R4;
				}
			}else if(item.ItemType == (int)ItemTypeEnum.weapon){
				var weapon = MasterDataTable.weapon[item.ItemId];
				if (weapon.rarity.rarity >= 4) {
                    mode |= ListItem_GachaResultEffect.Mode.R4;
                }
			}
			if(item.IsNew){
				mode |= ListItem_GachaResultEffect.Mode.New;
			}else{
				if(item.ItemType == (int)ItemTypeEnum.card){
					mode |= ListItem_GachaResultEffect.Mode.Soul;
				}            
			}
			if(mode != ListItem_GachaResultEffect.Mode.None){
				var prefabName = item.ItemType == (int)ItemTypeEnum.weapon ? "eff_GachaResultWeaponIcon" : "eff_GachaResultIcon";
				var eff = GameObjectEx.LoadAndCreateObject("Gacha/"+prefabName, anchor.gameObject);
				eff.gameObject.GetOrAddComponent<ListItem_GachaResultEffect>().Init(mode);
			}
		}
	}

    // 種類によって作るべきオブジェクトが異なる.
	private GameObject CreateResultObject(AcquiredGachaItemData item)
	{
		switch((ItemTypeEnum)item.ItemType){
			case ItemTypeEnum.card:
				{
					var obj = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon");
					var c = obj.GetOrAddComponent<ListItem_UnitIcon>();
					c.Init(new CardData(MasterDataTable.card[item.ItemId]));
                    return obj;
				}
			case ItemTypeEnum.weapon:
				{
                    var obj = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon");
					var c = obj.GetOrAddComponent<ListItem_WeaponIcon>();
					c.Init(new WeaponData(MasterDataTable.weapon[item.ItemId]), ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);
                    return obj;
                }
		}
		Debug.LogError("CreateResultObject Error!! : Unknown itemType. type="+(ItemTypeEnum)item.ItemType);
		return null;
	}
 
	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private ReceiveGachaPurchaseProduct m_res;
	private Action m_didClose;
	private Action m_oneMoreProc;

	static View_GachaResult instance;
}
