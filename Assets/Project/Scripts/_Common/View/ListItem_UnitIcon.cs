using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

/// <summary>
/// ユニットアイコン表示クラス
/// </summary>
public class ListItem_UnitIcon : ViewBase
{
    public enum DispStatusType
    {
        Default,
        Level,
        HP,
        ATK,
        DEF,
        SPD,
        EXP // 経験値
    }

	public void Init(CardData card, DispStatusType statusType = DispStatusType.Default, bool isDispRemove = false, bool isDispOrganizing = false, bool isDispEquip = false, Action<CardData> DidTap = null, Action<CardData> DidLongPress = null)
    {
        if (m_CardData != null) {
			IconLoader.RemoveLoadedEvent (ItemTypeEnum.card, m_CardData.CardId, m_CardData.Rarity, SetIcon);
            // アイコンが切り替わるので古いものの表示を削除
            var iconImg = this.GetScript<Image> ("IconChDummy");
            iconImg.overrideSprite = null;
        }

        m_CardData = card;
        m_DidTap = DidTap;
        m_DidLongPress = DidLongPress;

        var unitStatus = this.GetScript<RectTransform> ("Status");
		var footerRoot = this.GetScript<RectTransform>("FooterInfo");
        var levelRoot = this.GetScript<Transform> ("Lv");
        var starGrid = this.GetScript<Transform> ("StarGrid");
        var elementIcon = GetScript<Image> ("ElementIcon");
		var expGaugeRoot = GetScript<Transform>("UnitExpGauge");
        if (m_CardData == null) {
            unitStatus.gameObject.SetActive (false);
            elementIcon.gameObject.SetActive (false);
            starGrid.gameObject.SetActive (false);
            levelRoot.gameObject.SetActive (false);
			expGaugeRoot.gameObject.SetActive(false);
            return;
        }

		footerRoot.gameObject.SetActive(statusType != DispStatusType.Default);

        var iconBg = this.GetScript<Image> ("ListIconBg");
        iconBg.overrideSprite = IconLoader.LoadListIconBg (m_CardData.Rarity);
        var iconFrame = this.GetScript<Image> ("ListIconFrame");
        iconFrame.overrideSprite = IconLoader.LoadListIconFrame (m_CardData.Rarity);
        IconLoader.LoadCardIcon (m_CardData, SetIcon);

        // level表示
        this.GetScript<TextMeshProUGUI> ("txtp_Lv").SetText(card.Level);
		this.GetScript<TextMeshProUGUI>("txtp_LvMaxNum").SetTextFormat("/{0}", card.MaxLevel);      
		// 経験値表示
		this.GetScript<Image>("img_UnitExpGauge").fillAmount = card.CurrentLevelProgress;

		// レアリティの設定
		int maxRarity = card.MaxRarity;
        int nowRarity = card.Rarity;
		for(int i = 1; i <= 6; ++i){
			var starObj = this.GetScript<Transform>(string.Format("StarGrid/Star{0}", i));
			starObj.gameObject.SetActive(false);
		}
        for(int i = 1; i <= 6; ++i){
            var starObj = this.GetScript<Transform> (string.Format ("StarGrid/Star{0}", i));
			if (maxRarity < i){
				break;
			}
            starObj.gameObject.SetActive (true);
            var starOn = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOn", i));
            var starOff = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOff", i));
            if (nowRarity >= i) {
                starOn.gameObject.SetActive (true);
                starOff.gameObject.SetActive (false);
            } else {
                starOn.gameObject.SetActive (false);
                starOff.gameObject.SetActive (true);
            }
        }
        GetScript<GridLayoutGroup> ("StarGrid").CalculateLayoutInputHorizontal ();

        // ステータス表示切り替え
        switch (statusType) {
        case DispStatusType.Default:
			levelRoot.gameObject.SetActive(false);
			expGaugeRoot.gameObject.SetActive(false);
            unitStatus.gameObject.SetActive (false);
            starGrid.gameObject.SetActive (true);
            break;
		case DispStatusType.Level:
			levelRoot.gameObject.SetActive(true);
            expGaugeRoot.gameObject.SetActive(false);
            unitStatus.gameObject.SetActive(false);
            starGrid.gameObject.SetActive(true);
			break;
        case DispStatusType.HP:
            levelRoot.gameObject.SetActive (false);
            expGaugeRoot.gameObject.SetActive (false);
            starGrid.gameObject.SetActive (false);
            unitStatus.gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconHP").gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconATK").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconDEF").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconSPD").gameObject.SetActive (false);
            GetScript<TextMeshProUGUI>("txtp_UnitStatus").SetText(card.Parameter.MaxHp);
            break;
        case DispStatusType.ATK:
            levelRoot.gameObject.SetActive(false);
			expGaugeRoot.gameObject.SetActive(false);
            starGrid.gameObject.SetActive (false);
            unitStatus.gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconHP").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconATK").gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconDEF").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconSPD").gameObject.SetActive (false);
            GetScript<TextMeshProUGUI>("txtp_UnitStatus").SetText(card.Parameter.Attack);
            break;
        case DispStatusType.DEF:
			levelRoot.gameObject.SetActive(false);
			expGaugeRoot.gameObject.SetActive(false);
            starGrid.gameObject.SetActive (false);
            unitStatus.gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconHP").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconATK").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconDEF").gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconSPD").gameObject.SetActive (false);
            GetScript<TextMeshProUGUI>("txtp_UnitStatus").SetText(card.Parameter.Defense);
            break;
        case DispStatusType.SPD:
			levelRoot.gameObject.SetActive(false);
			expGaugeRoot.gameObject.SetActive(false);
            starGrid.gameObject.SetActive (false);
            unitStatus.gameObject.SetActive (true);
            GetScript<RectTransform> ("img_ListIconHP").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconATK").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconDEF").gameObject.SetActive (false);
            GetScript<RectTransform> ("img_ListIconSPD").gameObject.SetActive (true);
            GetScript<TextMeshProUGUI>("txtp_UnitStatus").SetText(card.Parameter.Agility);
            break;
		case DispStatusType.EXP:
			levelRoot.gameObject.SetActive(true);
			expGaugeRoot.gameObject.SetActive(true);
			unitStatus.gameObject.SetActive(false);
			starGrid.gameObject.SetActive(true);
			break;
        }

        elementIcon.gameObject.SetActive (true);
        elementIcon.overrideSprite = IconLoader.LoadElementIcon (m_CardData.Parameter.Element);

        // 外すボタンの表示
        this.GetScript<Transform>("img_UnitRemove").gameObject.SetActive(isDispRemove);      
        // 編成中表示
		this.GetScript<Transform>("img_UnitOrganizing").gameObject.SetActive(isDispOrganizing && 
		                                                                     !this.GetScript<Transform>("img_UnitRemove").gameObject.activeSelf);
		// 装備中表示
		this.GetScript<Transform>("img_WeaponEquip").gameObject.SetActive(isDispEquip && 
		                                                                  !this.GetScript<Transform>("img_UnitRemove").gameObject.activeSelf && 
		                                                                  !this.GetScript<Transform>("img_UnitOrganizing").gameObject.activeSelf &&
		                                                                  m_CardData.EquippedWeaponBagId != 0);
        // Bonus表示
        if (MasterDataTable.event_quest_BonusUnit != null && MasterDataTable.event_quest != null) {
            var bonusUnit = MasterDataTable.event_quest_BonusUnit.DataList;
            var time = GameTime.SharedInstance.Now;
            this.GetScript<Transform> ("img_EventBonus").gameObject.SetActive (bonusUnit.Count > 0 && bonusUnit.Any (r => r.unit_id == m_CardData.CardId &&
                MasterDataTable.event_quest [r.event_id] != null &&
                MasterDataTable.event_quest [r.event_id].start_at <= time &&
                time < MasterDataTable.event_quest [r.event_id].end_at));
        }

        var iconButton = GetScript<CustomButton> ("IconChDummy");
        if (m_DidTap == null && DidLongPress == null) {
            // コールバックが設定されない場合はボタンとして機能しないようにする。
            iconButton.targetGraphic.raycastTarget = false;
            iconButton.enabled = false;
        } else {
            iconButton.targetGraphic.raycastTarget = true;
            iconButton.enabled = true;
            iconButton.onClick.RemoveListener (DidTapIcon);
            iconButton.onClick.AddListener (DidTapIcon);
            if (DidLongPress == null) {
                iconButton.m_EnableLongPress = false;
            } else {
                iconButton.m_EnableLongPress = true;
                iconButton.onLongPress.RemoveListener (DidLongPressIcon);
                iconButton.onLongPress.AddListener (DidLongPressIcon);
            }
        }
    }

    public void SetIcon(IconLoadSetting data, Sprite icon)
    {
        // 更新状態によっては内部データが変わっている可能性があるので判定が必要
		if(m_CardData.CardId == data.id && m_CardData.Rarity == data.rarity) {
            var iconImg = this.GetScript<Image> ("IconChDummy");
            iconImg.overrideSprite = icon;
        }
    }

    /// 経験値設定.resultExpを設定する場合カウントアップ演出を行う.経験値はレベルあたりのものでなく総合値を指定する.
    public void SetExp(int exp, int resultExp = 0, Action didEffectEnd = null)
	{
		//Debug.Log(exp+" "+resultExp);
		if(resultExp > exp){
			if (m_coRankUp != null) {
                this.StopCoroutine(m_coRankUp);
            }
			m_resultExp = resultExp;
			m_coRankUp = this.StartCoroutine(this.CoPlayCountUpRank(exp, resultExp, didEffectEnd));
		}else{
			var level = MasterDataTable.card_level.GetLevel(m_CardData.Card.level_table_id, exp);
            var currentExp = (float)MasterDataTable.card_level.GetCurrentLevelExp(m_CardData.Card.level_table_id, level, exp);
			var nextExp = (float)(m_CardData.MaxLevel > level ? MasterDataTable.card_level.GetNextLevelExp(m_CardData.Card.level_table_id, level, exp) : 0);
			this.GetScript<TextMeshProUGUI>("txtp_Lv").text = level.ToString();
            this.GetScript<Image>("img_UnitExpGauge").fillAmount = currentExp / nextExp;
            if (didEffectEnd != null) {
                didEffectEnd ();
            }
		}
	}
	private IEnumerator CoPlayCountUpRank(int exp, int resultExp, Action didEffectEnd)
	{
        var animation = GetComponent<Animation> ();
		var level = MasterDataTable.card_level.GetLevel(m_CardData.Card.level_table_id, exp);
		var currentExp = (float)MasterDataTable.card_level.GetCurrentLevelExp(m_CardData.Card.level_table_id, level, exp);
		var nextExp = (float)(m_CardData.MaxLevel > level ? MasterDataTable.card_level.GetNextLevelExp(m_CardData.Card.level_table_id, level, exp) : 0.0f);
        var addVal = Mathf.Ceil((resultExp - exp) / 60.0f);
        do {
            float per = 0.0f;
            if(nextExp > 0.0f) {
                per = currentExp / nextExp;
            }
			this.GetScript<Image>("img_UnitExpGauge").fillAmount = per;
            currentExp += addVal;
            exp = Mathf.Min(resultExp,  exp + (int)addVal);
            // レベルアップ.
            if (currentExp >= nextExp) {
                nextExp = (float)(m_CardData.MaxLevel > level ? MasterDataTable.card_level.GetNextLevelExp(m_CardData.Card.level_table_id, ++level, exp) : 0.0f);
                currentExp = currentExp - nextExp;
                per = 0.0f;
                if(nextExp > 0.0f) {
                    per = currentExp / nextExp;
                }
                this.GetScript<Image>("img_UnitExpGauge").fillAmount = per;
                this.GetScript<TextMeshProUGUI>("txtp_Lv").SetText(level);
                animation.Stop();
                animation.Play();
            }
            yield return null;
        } while (exp < resultExp);

		if(didEffectEnd != null){
			didEffectEnd();
		}
	}
    /// ランクアップアニメーションを強制即時終了させる.
    public void ForceImmediateEndRankUpAnimation(int exp)
	{
		if (m_coRankUp != null) {
            this.StopCoroutine(m_coRankUp);
        }
        var level = MasterDataTable.card_level.GetLevel(m_CardData.Card.level_table_id, exp);
        var currentExp = (float)MasterDataTable.card_level.GetCurrentLevelExp(m_CardData.Card.level_table_id, level, exp);
        var nextExp = (float)(m_CardData.MaxLevel > level ? MasterDataTable.card_level.GetNextLevelExp(m_CardData.Card.level_table_id, level, exp) : 0);
        this.GetScript<TextMeshProUGUI>("txtp_Lv").SetText(level);
        if (nextExp > 0.0f) {
            this.GetScript<Image> ("img_UnitExpGauge").fillAmount = (float)currentExp / (float)nextExp;
        } else {
            this.GetScript<Image> ("img_UnitExpGauge").fillAmount = 0;
        }
	}
	private Coroutine m_coRankUp;
	private int m_resultExp;
    
    private void DidTapIcon()
    {
        if (m_DidTap != null) {
            m_DidTap (m_CardData);
        }
    }

    private void DidLongPressIcon()
    {
        if (m_DidLongPress != null) {
            m_DidLongPress (m_CardData);
        }
    }

    void OnDestroy()
    {
        if (m_CardData != null) {
			IconLoader.RemoveLoadedEvent (ItemTypeEnum.card, m_CardData.CardId, m_CardData.Rarity, SetIcon);
        }
    }

    private CardData m_CardData;
    private Action<CardData> m_DidTap;
    private Action<CardData> m_DidLongPress;
}