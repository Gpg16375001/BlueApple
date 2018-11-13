using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;
using Live2D.Cubism.Rendering;

/// <summary>
/// View : キャラ進化画面.
/// </summary>
public class View_UnitEvolution : ViewBase
{
	public override bool IsEnableButton
	{
		set {
			Debug.Log("View_UnitEvolution IsEnableButton");
			base.IsEnableButton = value;
		}
	}

	/// <summary>
	/// 生成.
	/// </summary>
	public static View_UnitEvolution Create(CardData card, Action<CardData> didClose)
    {
		var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_UnitEvolution");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_UnitEvolution>();
		c.InitInternal(card, didClose);
        return c;
    }
	private void InitInternal(CardData card, Action<CardData> didClose)
	{
		// ボタン.
        this.SetCanvasCustomButtonMsg("DoEvolution/bt_Common", DidTapEvolved);   
		
		m_card = card;
		m_didClose = didClose;

		// 素材情報表示更新.
		this.UpdateMaterialList();

		// 情報表示.      
		this.GetScript<TextMeshProUGUI>("Coin/txtp_Coin").text = AwsModule.UserData.UserData.GoldCount.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("Cost/txtp_Coin").text = m_defineInfo.evolution_cost.ToString("#,0");
		this.UpdateUnitInfoView();   
	}

	// 素材一覧リスト作成.
	private void UpdateMaterialList()
	{
		m_alredySetIdList.Clear();
		m_defineInfo = MasterDataTable.chara_material_evolution_definition[m_card.Rarity];

		var allList = MasterDataTable.chara_material.DataList;
		var elementList = MasterDataTable.chara_material.GetEvolutionMaterialList(m_card.Card.element.Enum);
        var roleList = MasterDataTable.chara_material.GetEvolutionMaterialList(m_card.Card.role);
		var roleDefine = MasterDataTable.chara_material_evolution_by_role_definition[(int)m_card.Card.role];

		// 最大6箇所に設定する.
		for (var i = 1; i <= 6; ++i){
			var textName = "EvolutionMaterial_"+i;

            // 属性素材.
			if(m_defineInfo.element_based_evolution_material_count_1 > 0){
                var r1ele = elementList.Find(e => e.rarity == 1);
				if(this.SetMaterial(textName, r1ele, m_defineInfo.element_based_evolution_material_count_1)){
                    continue;
                }
            }
			if(m_defineInfo.element_based_evolution_material_count_2 > 0){
				var r2ele = elementList.Find(e => e.rarity == 2);
				if(this.SetMaterial(textName, r2ele, m_defineInfo.element_based_evolution_material_count_2)){
                    continue;
                }
			}
            if(m_defineInfo.element_based_evolution_material_count_3 > 0){
                var r3ele = elementList.Find(e => e.rarity == 3);
				if(this.SetMaterial(textName, r3ele, m_defineInfo.element_based_evolution_material_count_3)) {
                    continue;
                }
            }
            // ロール固有素材.
            if(m_defineInfo.role_based_material_count_1 > 0){
				var r1coun = roleList.Find(r => r.rarity == 1);
                if(this.SetMaterial(textName, r1coun, m_defineInfo.role_based_material_count_1)){
                    continue;
                }
            }
            if(m_defineInfo.role_based_material_count_2 > 0){
				var r2coun = roleList.Find(r => r.rarity == 2);
                if (this.SetMaterial(textName, r2coun, m_defineInfo.role_based_material_count_2)) {
                    continue;
                }
            }
            if(m_defineInfo.role_based_material_count_3 > 0){
				var r3coun = roleList.Find(r => r.rarity == 3);
                if (this.SetMaterial(textName, r3coun, m_defineInfo.role_based_material_count_3)) {
                    continue;
                }
			}
            // モンスター固有素材.
			if (m_defineInfo.enemy_based_material_count_1 > 0) {
				var r1mon = allList.Find(m => m.id == roleDefine.enemy_based_material_1);
				if (this.SetMaterial(textName, r1mon, m_defineInfo.enemy_based_material_count_1)) {
                    continue;
                }
            }
			if (m_defineInfo.enemy_based_material_count_2 > 0) {
				var r2mon = allList.Find(m => m.id == roleDefine.enemy_based_material_2);
				if(this.SetMaterial(textName, r2mon, m_defineInfo.enemy_based_material_count_2)){
                    continue;
                }
            }
			if (m_defineInfo.enemy_based_material_count_3 > 0) {
				var r3mon = allList.Find(m => m.id == roleDefine.enemy_based_material_3);
				if (this.SetMaterial(textName, r3mon, m_defineInfo.enemy_based_material_count_3)) {
                    continue;
                }
            }
            // 虹素材.
			if (m_defineInfo.rainbow_evolution_material_count_1 > 0) {
				var reinbow = allList.Find(c => c.IsCharaEvolutionMaterial && c.element != null && c.element == MasterDataTable.element[ElementEnum.rainbow].Enum);
				if(this.SetMaterial(textName, reinbow, m_defineInfo.rainbow_evolution_material_count_1)){
					continue;
				}
            }
		}
	}
	private bool SetMaterial(string labelRootName, CharaMaterial material, int needCount)
	{
		if(m_alredySetIdList.Contains(material.id)){
			return false;
		}
		m_alredySetIdList.Add(material.id);
		var data = MaterialData.CacheGet(material.id);      
		this.GetScript<RectTransform>(labelRootName).gameObject.GetOrAddComponent<ListItem_UnitEvolutionMaterial>().Init(data, material, needCount);
		return true;
	}   

	// ユニット情報表示.
    private void UpdateUnitInfoView()
    {
		// Live2D
		View_FadePanel.SharedInstance.IsLightLoading = true;
		this.GetScript<Transform>("CharacterAnchor").DetachChildren();
		var loader = new UnitResourceLoader(m_card);
		loader.IsLoadLive2DModel = true;
		loader.LoadResource(resource => {
			var live2d = Instantiate(resource.Live2DModel) as GameObject;
            live2d.transform.SetParent(this.GetScript<Transform>("CharacterAnchor"));
            live2d.transform.localScale = Vector3.one;
            live2d.transform.localPosition = Vector3.zero;
            var cubismRender = live2d.GetComponentsInChildren<CubismRenderController>()[0];
            if (cubismRender != null) {
                var rootCanvas = this.GetScript<Canvas>("CharacterAnchor");
                cubismRender.gameObject.SetLayerRecursively(rootCanvas.gameObject.layer);
                cubismRender.SortingLayer = rootCanvas.sortingLayerName;
                cubismRender.SortingOrder = rootCanvas.sortingOrder;
            }
            View_FadePanel.SharedInstance.IsLightLoading = false;
		});
		// カードアイコン.
        var rootObj = this.GetScript<RectTransform>("UnitIcon").gameObject;
		rootObj.DestroyChildren();
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", rootObj);
		go.GetOrAddComponent<ListItem_UnitIcon>().Init(m_card);
        // ラベル類.
        this.GetScript<TextMeshProUGUI>("txtp_UnitName").text = m_card.Card.nickname;
        this.GetScript<TextMeshProUGUI>("txtp_UnitLv").text = m_card.Level.ToString();
        // 強化状況に応じたレベルアップ情報.
        this.GetScript<Image>("EXP_WhitePanel_1").fillAmount = !m_card.IsMaxLevel ? m_card.CurrentLevelProgress : 1f;      
        this.GetScript<Image>("img_LvMax").gameObject.SetActive(m_card.IsMaxLevel);
		this.GetScript<RectTransform>("LvCaution").gameObject.SetActive(!m_card.IsMaxLevel);
		// これが経験値表示のルートになってる.
		this.GetScript<RectTransform>("ExpInfo").gameObject.SetActive(!m_card.IsMaxLevel);
        if (!m_card.IsMaxLevel) {
            this.GetScript<TextMeshProUGUI>("txtp_LvPoint").text = m_card.CurrentLevelExp.ToString();
            this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").text = m_card.NextLevelExp.ToString();
        }
        // 進化できるかどうかでボタン設定.
		var haveGold = AwsModule.UserData.UserData.GoldCount;
		this.GetScript<CustomButton>("DoEvolution/bt_Common").interactable = haveGold >= m_defineInfo.evolution_cost && m_card.IsCanEvolution();
    }

	#region ButtonDelegate.

	// ボタン : 進化.
    void DidTapEvolved()
	{
		var haveGold = AwsModule.UserData.UserData.GoldCount;
		if(haveGold < m_defineInfo.evolution_cost || !m_card.IsCanEvolution()){
			return;
		}

		PopupManager.OpenPopupYN("進化します。\nよろしいですか？", () => {
			View_FadePanel.SharedInstance.IsLightLoading = true;
            // 通信.
            SendAPI.CardsEvolveCard(m_card.CardId, (bSuccess, res) => {
                if (!bSuccess && res == null) {
                    return;
                }
                var prevCard = m_card;

                res.CardData.CacheSet();
                m_card = res.CardData;
                // 演出表示.
                View_UnitEvolutionMovie.Create(prevCard, res.CardData, () => {
                    if (m_didClose != null) {
                        m_didClose(m_card);
                    }
                });
                View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
                AwsModule.UserData.UserData = res.UserData;
				this.gameObject.GetComponentsInChildren<ListItem_UnitEvolutionMaterial>().Select(item => { item.ApplyEvolution(); return item; }).ToList();
                View_FadePanel.SharedInstance.IsLightLoading = false;
            });
		});      
	}

    #endregion

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }
 
	private CardData m_card;
	private Action<CardData> m_didClose;
	private CharaMaterialEvolutionDefinition m_defineInfo;
	private List<int> m_alredySetIdList = new List<int>();
}
