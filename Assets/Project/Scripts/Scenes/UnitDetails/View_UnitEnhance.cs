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


/// <summary>
/// View : キャラ強化画面.
/// </summary>
public class View_UnitEnhance : ViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_UnitEnhance Create(CardData card, Action<CardData> didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_UnitEnhance");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_UnitEnhance>();
		c.InitInternal(card, didClose);
		return c;
	}
	private void InitInternal(CardData card, Action<CardData> didClose)
	{
		// ボタン
        this.SetCanvasCustomButtonMsg("Enhance/bt_Common", DidTapReinforce);
        this.SetCanvasCustomButtonMsg("Deselect/bt_TopLineGray", DidTapSelectAllClear);
        for (var i = 0; i < 10; ++i) {
            var btnName = string.Format("bt_SelectMaterialBase{0}", i + 1);
            var btIdx = i + 0;    // 一時変数を作成して設定しないとiは一つしかないので最終値の10で固定される.
            this.SetCanvasCustomButtonMsg(btnName, () => DidTapSelectClear(btIdx));
        }
		
		m_card = card;
		m_didClose = didClose;
  
		// カードアイコン.
		var rootObj = this.GetScript<RectTransform>("UnitIcon").gameObject;      
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", rootObj);
		go.GetOrAddComponent<ListItem_UnitIcon>().Init(card);

		// リスト
		this.CreateMaterialList();
              
		// 初回情報表示
        this.UpdateReinforceInfo();      
	}
    
    // 素材一覧リスト作成.
    private void CreateMaterialList()
	{
		var list = MasterDataTable.chara_material.DataList.Where(m => m.IsCharaEnhanceMaterial).ToList();
		foreach(var material in list){
			// レアリティごとの素材設定.
            var rootName = "EnhanceMaterialSet_";
			var element = MasterDataTable.element.DataList.Find(e => e.Enum == material.element);
			if (material.rarity >= 3) {
                rootName += (element.index.ToString() + "LL");
			} else if (material.rarity >= 2) {
                rootName += (element.index.ToString() + "L");
            } else {
                rootName += element.index.ToString();
            }
            var c = this.GetScript<RectTransform>(rootName).gameObject.GetOrAddComponent<ListItem_UnitEnhanceMaterial>();
			var data = MaterialData.CacheGet(material.id);
            c.Init(material, data, CallbackDidTapMaterial);         
		}

		// 属性一致の欄にはボーナス経験値パネルを表示.
		var elemntIndexList = MasterDataTable.element.DataList.Where(e => e.Enum != ElementEnum.rainbow && e.Enum != ElementEnum.naught)
		                                                      .Select(e => e.index)
		                                                      .ToList();
		foreach(var index in elemntIndexList){
			this.GetScript<Image>(index+"/img_MaterialBonus").gameObject.SetActive(m_card.Card.element.index == index);
		}
	}
	// コールバック : 素材リストをタップした.
	bool CallbackDidTapMaterial(MaterialData material)
	{
		// 選択上限は10個.
		if(m_materialList.Count >= 10){
			return false;
		}
		m_materialList.Add(material);
		this.UpdateReinforceInfo();
		this.UpdateSelectedMaterialList();
		return true;
	}

	// 強化情報表示の更新.
    private void UpdateReinforceInfo()
    {
		// --- ラベル類 ---
        var bExistChooseMaterial = m_materialList.Count > 0;      
		var enhancePoint = m_materialList.Sum(m => m.GetEnhancePoint(m_card.Card.element));
        this.GetScript<TextMeshProUGUI>("txtp_UnitLv").text = m_card.Level.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_LimitLv").text = m_card.MaxLevel.ToString();      
        this.GetScript<Image>("img_ChangeArrow").gameObject.SetActive(bExistChooseMaterial);
        this.GetScript<TextMeshProUGUI>("txtp_AfterUnitLv").gameObject.SetActive(bExistChooseMaterial);
        this.GetScript<TextMeshProUGUI>("txtp_UnitName").text = m_card.Card.nickname;
		this.GetScript<TextMeshProUGUI>("txtp_GetEnhance").text = enhancePoint.ToString();
        this.GetScript<Image>("img_IconUpArrow").gameObject.SetActive(bExistChooseMaterial);
		this.GetScript<TextMeshProUGUI>("txtp_Coin").text = m_materialList.Sum(m => m.CharaMaterialDefineInfo.cost).ToString();
        this.GetScript<CustomButton>("Enhance/bt_Common").interactable = bExistChooseMaterial;
        this.GetScript<TextMeshProUGUI>("txtp_SelectNum").text = m_materialList.Count.ToString();
		// 強化に応じたレベルアップ情報.
		var exp = m_card.Exp+enhancePoint;
		var lv = Math.Min(MasterDataTable.card_level.GetLevel(m_card.Card.level_table_id, exp), m_card.MaxLevel);
		var currentLvExp = lv < m_card.MaxLevel ? MasterDataTable.card_level.GetCurrentLevelExp(m_card.Card.level_table_id, lv, exp): 0;
		var nextLvExp = lv < m_card.MaxLevel ? MasterDataTable.card_level.GetNextLevelExp(m_card.Card.level_table_id, lv, exp): 0;
		this.GetScript<TextMeshProUGUI>("txtp_AfterUnitLv").text = lv.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").gameObject.SetActive(lv < m_card.MaxLevel); // これが経験値表示のルートになってる.
		this.GetScript<Image>("img_LvMax").gameObject.SetActive(lv >= m_card.MaxLevel);
		if(lv < m_card.MaxLevel){
			this.GetScript<TextMeshProUGUI>("txtp_LvPoint").text = currentLvExp.ToString();
            this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").text = nextLvExp.ToString();
		}
		this.GetScript<Image>("EXP_WhitePanel_1").fillAmount = lv < m_card.MaxLevel ? (float)currentLvExp / (float)nextLvExp: 1f;
		// 最大レベルで素材選択できないように.
		this.GetComponentsInChildren<ListItem_UnitEnhanceMaterial>()
		    .Select(l => l.IsEnable = lv < m_card.MaxLevel && l.RemainCount > 0)
		    .ToList();      
    }   
    // 選択中のマテリアルリスト更新.
    private void UpdateSelectedMaterialList()
	{
		for (var i = 0; i < 10; ++i){
			var iconRootName = string.Format("bt_SelectMaterialBase{0}/EnhanceMaterialIcon", i+1);
			this.GetScript<Image>(iconRootName).gameObject.SetActive(m_materialList.Count > i);
			if (m_materialList.Count <= i) {
                continue;
            }
			IconLoader.LoadMaterial(m_materialList[i].CharaMaterialInfo.id, (data, icon) => {
				if(m_materialList[i].CharaMaterialInfo.id == data.id){
					this.GetScript<Image>(iconRootName).sprite = icon;               
				}
			});
		}
	}

	#region ButtonDelegate.

	// ボタン : 強化する.
    void DidTapReinforce()
	{
		if (m_materialList == null || m_materialList.Count <= 0) {
            return;
        }
		var cost = m_materialList.Sum(m => m.CharaMaterialDefineInfo.cost);
		if(AwsModule.UserData.UserData.GoldCount < cost){
			PopupManager.OpenPopupOK("強化に必要なクレドが足りません。");
			return;
		}

		LockInputManager.SharedInstance.IsLock = true;
		View_FadePanel.SharedInstance.IsLightLoading = true;
  
        // 通信.
		SendAPI.CardsReinforceCard(m_card.CardId, m_materialList.Select(m => m.MaterialId).ToArray(), (bSuccess, res) => {
			if(!bSuccess || res == null){
				LockInputManager.SharedInstance.IsLock = false;
				return;
			}
			// 成功演出.
            View_EnhanceCaption.CreateUnit(res.ReinforcementDegreeId, () => {
                // カウントアップ演出.            
                m_routineCountUp = this.StartCoroutine(PlayCountUpAnimation(res.CardData));
            });
			res.CardData.CacheSet();         
			View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
			AwsModule.UserData.UserData = res.UserData;
			View_FadePanel.SharedInstance.IsLightLoading = false;
		});

        // 表示の差分更新.
		m_materialList.Clear();
		this.UpdateSelectedMaterialList();
        this.GetComponentsInChildren<ListItem_UnitEnhanceMaterial>()
            .Select(l => { l.Apply(); return true; })
            .ToList();
	}
    // カウントアップ演出開始
	private IEnumerator PlayCountUpAnimation(CardData targetData)
	{      
		// 演出終了後の共通情報更新処理定義.
        Action didUpdateInfo = () => {
			if (m_card.Level < targetData.Level) {
				m_viewEnhanceResult = View_UnitEnhanceResult.Create(m_card, targetData, m_didClose);
				this.GetScript<RectTransform>("UnitIcon").GetComponentInChildren<ListItem_UnitIcon>().Init(targetData);
			}else{
				LockInputManager.SharedInstance.ForceUnlockInput();
				if(m_didClose != null){
					m_didClose(targetData);
				}
			}
            m_card = targetData;
            this.UpdateReinforceInfo();         
        };

        // スキップ待機.
		this.StartCoroutine(WaitSkipTap(() => {
			if(m_routineCountUp != null){
				this.StopCoroutine(m_routineCountUp);
			}
			didUpdateInfo();         
		}));

		var level = m_card.Level;
		var exp = m_card.Exp;
        var nextExp = (float)m_card.NextLevelExp;
        var currentExp = (float)m_card.CurrentLevelExp;
		var addVal = Mathf.Ceil(nextExp * Time.unscaledDeltaTime);
		do {
            var per = currentExp / nextExp;         
			this.GetScript<Image>("EXP_WhitePanel_1").fillAmount = per;
			currentExp += addVal;
			exp += (int)addVal;
			this.GetScript<TextMeshProUGUI>("txtp_LvPoint").text = ((int)currentExp).ToString();
            // レベルアップ.
            if (currentExp >= nextExp) {
				nextExp = MasterDataTable.card_level.GetNextLevelExp(m_card.Card.level_table_id, (++level), exp);            
                currentExp = 0;
				addVal = Mathf.Ceil(nextExp * Time.unscaledDeltaTime);
				this.GetScript<TextMeshProUGUI>("txtp_UnitLv").text = level.ToString();
				this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").text = nextExp.ToString();
				this.GetScript<Animation>("Lv").Stop();
				this.GetScript<Animation>("Lv").Play();
            }
            yield return null;
		} while (exp < targetData.Exp);

		didUpdateInfo();
	}
	private IEnumerator WaitSkipTap(Action didSkip)
	{
		do{
			if(Input.GetMouseButtonDown(0)){
				didSkip();
				break;
			}
			yield return null;
		}while (true);
        // リザルトがあれば閉じる対応も行う.
		if(m_viewEnhanceResult != null){
			yield return null;
			while (true){
				if(!LockInputManager.SharedInstance.IsLock && Input.GetMouseButtonDown(0)) {    // Live2Dロード中に破棄しないようにLockInputManagerもみておく。
					m_viewEnhanceResult.Dispose();
                    break;
                }
                yield return null;
			}
			// 消滅待機.         
			do {
				yield return null;
			}while (!m_viewEnhanceResult.IsDestroyed);
			m_viewEnhanceResult = null;
		}
	}

	// ボタン : 選択全解除.
    void DidTapSelectAllClear()
	{
		if(m_materialList == null || m_materialList.Count <= 0){
			return;
		}

		m_materialList.Clear();
		this.UpdateReinforceInfo();
		this.UpdateSelectedMaterialList();
		this.GetComponentsInChildren<ListItem_UnitEnhanceMaterial>()
		    .Select(l => { l.ResetSelect(); return true; })
		    .ToList();
	}

	// ボタン : 単体素材解除.
	void DidTapSelectClear(int index)
	{
		if(m_materialList.Count <= index){
			return; // 空欄.
		}
		var mat = m_materialList[index];
		this.GetComponentsInChildren<ListItem_UnitEnhanceMaterial>()
		    .FirstOrDefault(m => m.ID == mat.MaterialId)
		    .ReturnOneSelect();
		m_materialList.RemoveAt(index);
		this.UpdateReinforceInfo();
		this.UpdateSelectedMaterialList(); 
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
	private List<MaterialData> m_materialList = new List<MaterialData>();

	private Coroutine m_routineCountUp;
	private View_UnitEnhanceResult m_viewEnhanceResult;
}
