using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// BattleMissionChecker : 編成されているカードチェック
/// </summary>
public class BattleMissionCardChecker : IBattleMissionChecker
{
    /// <summary>
    /// 開始以前に前回プレイ時などで既に達成済みか.
    /// </summary>
    public bool IsAlreadyAchived { get { return m_bAlreadyAchived; } }
    private bool m_bAlreadyAchived;

    /// <summary>
    /// 達成しているかどうか.
    /// </summary>
    public bool IsAchived { get { return IsAlreadyAchived || m_bAchived; } }
    private bool m_bAchived = true;

    /// <summary>
    /// 説明文.
    /// </summary>
    public string ExplanatoryText { get { return m_explanatoryText; } }
    private string m_explanatoryText;


    /// <summary>
    /// 進捗更新.
    /// </summary>
    public void UpdateProgress(BattleMissionProgressData data)
    {
        if(data.UseCardIdList == null || data.UseCardIdList.Length <= 0){
            Debug.LogError("BattleMissionCardChecker UpdateProgress Error!! : card list is null or empty.");
            return;
        }
        if(IsAchived){
            return; // すでに達成していたらチェックの必要なし.
        }

        // カード or キャラID or 属性IDとしてチェック.
        if(m_type == BattleMissionEnum.SpecificCard){
            m_bAchived = Array.Exists(data.UseCardIdList, id => id == m_variant);
        }
        else if(m_type == BattleMissionEnum.SpecificCharacter){
            m_bAchived = data.UseCardIdList
                             .Where(x => x != 0)
                             .Select(i => MasterDataTable.card[i].character)
                             .ToList()
                             .Exists(c => c.id == m_variant);
        } 
        else if(m_type == BattleMissionEnum.SpecificCardElement){
            // 全員指定属性じゃないとダメ.
            m_bAchived = !data.UseCardIdList
                              .Where(x => x != 0)
                              .Select(i => MasterDataTable.card[i])
                              .ToList()
                              .Exists(c => c.element.Enum != (ElementEnum)m_variant);
        }
    }

    /// <summary>初期化.カードID or キャラクタID or 属性ID値.</summary>
    public BattleMissionCardChecker(BattleMissionConditionDefine defineInfo, int variant, bool bAlreadyAchived = false)
    {
        
        m_type = MasterDataTable.battle_mission.DataList.Find(m => m.name == defineInfo.name).Enum;
        m_variant = variant;
        m_bAchived = false;
        m_bAlreadyAchived = bAlreadyAchived;

        // カード or キャラID or 属性ID.
        if (m_type == BattleMissionEnum.SpecificCard) {
            var card = MasterDataTable.card[variant];
            m_explanatoryText = string.Format(defineInfo.text_detail, card.nickname);
        } else if (m_type == BattleMissionEnum.SpecificCharacter) {
            var chara = MasterDataTable.character[variant];
            m_explanatoryText = string.Format(defineInfo.text_detail, chara.name);
        } else if (m_type == BattleMissionEnum.SpecificCardElement) {
            var element = MasterDataTable.element[(ElementEnum)variant];
            m_explanatoryText = string.Format(defineInfo.text_detail, element.name);
        }
    }

    private BattleMissionEnum m_type;
    private int m_variant;
}
