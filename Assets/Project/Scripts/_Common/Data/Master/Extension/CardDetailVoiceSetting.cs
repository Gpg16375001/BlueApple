using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab.Net.API;


/// <summary>
/// カード詳細ボイス設定拡張.
/// </summary>
public partial class CardDetailVoiceSetting
{
	/// <summary>
    /// 指定したカードがボイス解放しているかどうか.
    /// </summary>
	public bool IsReleaseVoice(CardData card)
    {
        switch (condition) {
            case CardDetailVoiceReleaseEnum.Evolution:
				return card.RarityGrade >= condition_value;
            case CardDetailVoiceReleaseEnum.MaxRarityLevel:
                return card.IsMaxRarity && card.IsMaxLevel;
        }
        return true;
    }

    /// <summary>
	/// TODO : 解放テキスト.ローカライズを考慮してマスターに.
    /// </summary>
	public string UnlockText()
    {
        switch (condition) {
            case CardDetailVoiceReleaseEnum.Evolution:
                return string.Format("進化{0}段階目で解放", condition_value);
            case CardDetailVoiceReleaseEnum.MaxRarityLevel:
                return string.Format("最大進化・最大Lv到達で解放");
        }
        return "";
    }
}
