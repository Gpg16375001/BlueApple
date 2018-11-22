using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// MiniView : バトルリザルトの経験値表示ページ.
/// </summary>
public class MiniView_BattleExpResult : ViewBase, IBattleResultPage
{   
	/// <summary>
    /// リザルト内のページインデックス.
    /// </summary>
	public int Index { get { return m_index; } }
	private int m_index = 0;

    /// <summary>
    /// 演出中？
    /// </summary>
	public bool IsEffecting { get { return m_dictUnits.Count > m_cntEffList || m_bEffecting; } }
	private bool m_bEffecting = false;


    public ResultTitle GetResultTitle()
    {
        return ResultTitle.Clear;
    }

    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(int index, ReceiveQuestsCloseQuest questResult)
	{
        m_IsOpen = false;
        m_ForceImmediate = false;
		m_index = index;
		m_result = questResult;

		// ユーザー経験値.
		m_currentTotalExp = AwsModule.UserData.UserData.Exp;
		GetScript<Image>("img_PlayerExpGauge").fillAmount = MasterDataTable.user_level.GetCurrentLevelProgress(m_currentTotalExp);
        GetScript<TextMeshProUGUI>("txtp_PlayerExp").SetText(m_result.GainUserExp);
        GetScript<TextMeshProUGUI>("txtp_PlayerRankNum").SetText(AwsModule.UserData.UserData.Level);

		// ユニット経験値.
		var rootObj = GetScript<HorizontalLayoutGroup>("PartyUnitGrid").gameObject;
        GetScript<TextMeshProUGUI>("txtp_UnitExp").SetText(m_result.GainUnitExp);
        for (var i = 1; i <= Party.PartyCardMax; ++i) {
            var card = AwsModule.PartyData.CurrentTeam[i];
            if(card == null) {
                continue;
            }
            var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", rootObj);         
			var c = go.GetOrAddComponent<ListItem_UnitIcon>();
            c.Init(card, ListItem_UnitIcon.DispStatusType.EXP);
            m_dictUnits.Add(i, new UniRx.Tuple<int, ListItem_UnitIcon>(card.Exp, c));
        }

        // 獲得クレド
        GetScript<TextMeshProUGUI>("txtp_GetCoinNum").SetText(m_result.GainGold);
	}   
    
    /// <summary>
    /// 開く.
    /// </summary>
	public void Open()
	{
		m_bEffecting = true;
		if(!this.gameObject.activeSelf){
			this.gameObject.SetActive(true);
		}
		StartCoroutine(PlayOpenClose(true, () => {
            m_IsOpen = true;
            m_coResutAnime = StartCoroutine(WaitPlayExpAnime());
        }));
	}

    private IEnumerator WaitPlayExpAnime()
    {
        
        yield return new WaitForSeconds (1.0f);

        PlayUnitExpAnim ();

        yield return new WaitUntil (() => (m_dictUnits.Count <= m_cntEffList));

        StopExpSe();

        yield return new WaitForSeconds (0.5f);

        yield return EffectProgressUserGetExp (m_result.UserData.Exp);
    }

    private void PlayUnitExpAnim()
    {
        for (var i = 1; i <= Party.PartyCardMax; ++i) {
            var card = AwsModule.PartyData.CurrentTeam[i];
            if(card == null) {
                continue;
            }
            var result = m_result.MemberCardDataList.FirstOrDefault(x => x.CardId == card.CardId);
            if(result == null) {
                continue;
            }

            UniRx.Tuple<int, ListItem_UnitIcon> item;
            if(!m_dictUnits.TryGetValue(i, out item)) {
                continue;
            }

            if (result.Exp > item.Item1) {
                PlayExpSe ();
            }
            item.Item2.SetExp(item.Item1, result.Exp, () => {
                ++m_cntEffList;
                if(m_dictUnits.Count <= m_cntEffList) {
                    StopExpSe();
                }
            });
        }
    }

	private IEnumerator EffectProgressUserGetExp(int resultExp)
    {
        var maxLevel = MasterDataTable.user_level.DataList.Max (x => x.level);
        var startLevel = MasterDataTable.user_level.GetLevel(m_currentTotalExp);
        var level = startLevel;
		var exp = m_currentTotalExp;
		var currentExp = MasterDataTable.user_level.GetCurrentLevelExp(m_currentTotalExp);
        var nextExp = MasterDataTable.user_level.GetNextLevelRequiredExp(m_currentTotalExp);
        var addVal = Mathf.Max(1, Mathf.CeilToInt((resultExp - exp) / 60.0f));
        GetScript<Image>("img_PlayerExpGauge").fillAmount = MasterDataTable.user_level.GetCurrentLevelProgress(exp);

        if (maxLevel <= level) {
            m_bEffecting = false;
            yield break;
        }

        if (exp < resultExp) {
            PlayExpSe();
        }

        do {
            yield return null;

            currentExp += addVal;
            exp = Mathf.Min(exp + addVal, resultExp);
            GetScript<Image>("img_PlayerExpGauge").fillAmount = MasterDataTable.user_level.GetCurrentLevelProgress(exp);
            // レベルアップ.
            if (currentExp >= nextExp) {
                nextExp = MasterDataTable.user_level.GetNextLevelRequiredExp(exp);
                currentExp = MasterDataTable.user_level.GetCurrentLevelExp(exp);
                GetScript<TextMeshProUGUI>("txtp_PlayerRankNum").SetText(++level);
            }
        } while (exp < resultExp);

        StopExpSe ();

		// レベルアップ時はポップアップ表示.
        var nextLevel = MasterDataTable.user_level.GetLevel(resultExp);
        if (startLevel < nextLevel) {
            View_BattleRankUpPop.Create(startLevel, nextLevel, () => m_bEffecting = false);
		}else{
			m_bEffecting = false;
		}
    }

    void PlayExpSe()
    {
        if (m_PlaySeChanel < 0) {
            m_PlaySeChanel = SoundManager.SharedInstance.PlaySE (SoundClipName.se027, true);
        }
    }

    void StopExpSe()
    {
        if (m_PlaySeChanel >= 0) {
            SoundManager.SharedInstance.StopSE (m_PlaySeChanel);
        }
        m_PlaySeChanel = -1;
    }

    /// <summary>
    /// 閉じる.
    /// </summary>
    public void Close(Action didEnd)
	{
        StartCoroutine(PlayOpenClose(false, () => {
            m_IsOpen = false;
            didEnd();
        }));
	}

	private IEnumerator PlayOpenClose(bool bOpen, Action didEnd = null)
	{
		var anim = GetScript<Animation>("AnimParts");
		anim.Play(bOpen ? "CommonPopOpen" : "CommonPopClose");
		do {
			yield return null;
		} while (anim.isPlaying);
		if(didEnd != null){
			didEnd();
		}
	}

    /// <summary>
    /// アニメーションを強制的に即時終了させる.
    /// </summary>
    public void ForceImmediateEndAnimation()
	{
        if (m_ForceImmediate || !m_IsOpen) {
            return;
        }
        m_ForceImmediate = true;
        for (var i = 1; i <= Party.PartyCardMax; ++i) {
            var card = AwsModule.PartyData.CurrentTeam [i];
            if (card == null) {
                continue;
            }
            var result = m_result.MemberCardDataList.FirstOrDefault(x => x.CardId == card.CardId);
            if(result == null) {
                continue;
            }

            UniRx.Tuple<int, ListItem_UnitIcon> item;
            if(!m_dictUnits.TryGetValue(i, out item)) {
                continue;
            }

            item.Item2.ForceImmediateEndRankUpAnimation (result.Exp);
        }

		if (m_coResutAnime != null) {
            StopCoroutine(m_coResutAnime);
        }

        // 無理やり止めるときはフラグを落とす。
        m_cntEffList = m_dictUnits.Count;
        StopExpSe ();

        var startLevel = MasterDataTable.user_level.GetLevel(m_currentTotalExp);
        int afterLevel = MasterDataTable.user_level.GetLevel (m_result.UserData.Exp);
        GetScript<Image> ("img_PlayerExpGauge").fillAmount = MasterDataTable.user_level.GetCurrentLevelProgress (m_result.UserData.Exp);
        GetScript<TextMeshProUGUI>("txtp_PlayerRankNum").SetText(afterLevel);
        if (startLevel < afterLevel) {
            View_BattleRankUpPop.Create(startLevel, afterLevel, () => m_bEffecting = false);
        }else{
            m_bEffecting = false;
        }

	}

    private bool m_IsOpen;
    private bool m_ForceImmediate;

	private int m_currentTotalExp; 
	private int m_cntEffList = 0;
	private ReceiveQuestsCloseQuest m_result;
	private Dictionary<int/*index*/, UniRx.Tuple<int/*exp*/,ListItem_UnitIcon>> m_dictUnits = new Dictionary<int, UniRx.Tuple<int/*exp*/, ListItem_UnitIcon>>();
	private Coroutine m_coResutAnime;
    private int m_PlaySeChanel = -1;
}
