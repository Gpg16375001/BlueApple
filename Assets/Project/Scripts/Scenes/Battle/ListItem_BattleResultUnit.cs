using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// ListItem : バトルユニットごとのリザルト.
/// </summary>
public class ListItem_BattleResultUnit : ViewBase
{

    /// <summary>
    /// 初期化.indexは1スタートのデッキ参照用indexを渡すこと.
    /// </summary>
    public void Init(int index, CardData result)
    {
        m_cardData = AwsModule.PartyData.CurrentTeam[index];
		m_result = result;
  
		var per = (float)m_cardData.CurrentLevelExp / (float)m_cardData.NextLevelExp;
        this.GetScript<Image>("img_UnitExpGauge").fillAmount = per;
        this.GetScript<Image>("ElementIcon").sprite = IconLoader.LoadElementIcon(m_cardData.Parameter.Element);
		this.GetScript<TextMeshProUGUI>("txtp_UnitStatus").text = "Lv."+m_cardData.Level.ToString();    

        IconLoader.LoadCardIcon(m_cardData, SetIcon);
    }
	public void InitDummy(CardCard card)
	{
		m_cardData = new CardData(card);      
        this.GetScript<Image>("img_UnitExpGauge").fillAmount = 0;
        this.GetScript<Image>("ElementIcon").sprite = IconLoader.LoadElementIcon(m_cardData.Parameter.Element);
        this.GetScript<TextMeshProUGUI>("txtp_UnitStatus").text = "Lv." + m_cardData.Level.ToString();
		IconLoader.LoadCardIcon(m_cardData, SetIcon);
	}

    // アイコン.
    void SetIcon(IconLoadSetting data, Sprite icon)
    {
        // 更新状態によっては内部データが変わっている可能性があるので判定が必要
        if (m_cardData.CardId == data.id) {
            var iconImg = this.GetScript<Image>("IconChCDummy");
            iconImg.sprite = icon;
        }
    }
    
    private IEnumerator EffectProgressGetExp(int resultExp)
    {
		var level = m_cardData.Level;
		var exp = m_cardData.Exp;
		var nextExp = (float)m_cardData.NextLevelExp;
		var currentExp = (float)m_cardData.CurrentLevelExp;
        var addVal = Mathf.Ceil(nextExp * Time.unscaledDeltaTime);
        do {
            var per = currentExp / nextExp;
            this.GetScript<Image>("EXP_WhitePanel_1").fillAmount = per;
            currentExp += addVal;
            exp += (int)addVal;
            this.GetScript<TextMeshProUGUI>("txtp_LvPoint").text = ((int)currentExp).ToString();
            // レベルアップ.
            if (currentExp >= nextExp) {
				nextExp = MasterDataTable.card_level.GetNextLevelExp(m_cardData.Card.level_table_id, (++level), exp);
                currentExp = 0;
                addVal = Mathf.Ceil(nextExp * Time.unscaledDeltaTime);
                this.GetScript<TextMeshProUGUI>("txtp_UnitLv").text = level.ToString();
                this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").text = nextExp.ToString();
            }
            yield return null;
		} while (exp < resultExp);
    }

	void OnEnable()
	{
		if(m_result != null){
			this.StartCoroutine(this.EffectProgressGetExp(m_result.Exp));
		}
	}

	private CardData m_result;
	private CardData m_cardData;
}
