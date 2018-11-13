using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using SmileLab.Net.API;


/// <summary>
/// ミッション設定.
/// </summary>
public partial class MissionSetting
{
	/// <summary>
    /// ミッション名テキスト取得.
    /// </summary>
	public string GetText()
	{
		switch(type.Enum){
			case MissionTypeEnum.ClearMainQuest: // 指定メインクエストクリア
				{
					var quest = MasterDataTable.quest_main[arg1];
					var stage = MasterDataTable.stage[quest.stage_id];
					return string.Format(type.text_template, stage.name);
				}
			case MissionTypeEnum.ClearAllQuestInStage: // 指定ステージクリア
				{
					var belonging = MasterDataTable.belonging[(BelongingEnum)arg1];
					return string.Format(type.text_template, belonging.name, arg2, arg3);
				}
			case MissionTypeEnum.ClearAllQuestInChapter: // 指定章クリア
				{
                    var belonging = MasterDataTable.belonging[(BelongingEnum)arg1];
                    return string.Format(type.text_template, belonging.name, arg2);
                }
			case MissionTypeEnum.ClearAllQuestInCountry: // 指定国クエスト全クリア
				{
                    var belonging = MasterDataTable.belonging[(BelongingEnum)arg1];
                    return string.Format(type.text_template, belonging.name);
                }
			case MissionTypeEnum.ClearAllQuest: // メインストーリーを全クリア
				break;
			case MissionTypeEnum.ClearAllQuestThisUnit: // キャラストーリーを全クリア
				{
					var character = MasterDataTable.character[arg1];
					return string.Format(type.text_template, character.name);
                }
			case MissionTypeEnum.AchiveUserLv: // ユーザーLv達成
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.JoinPvP: // PvP参加
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.JoinMainQuest: // メインストーリーをプレイ
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.JoinSubQuest: // サブストーリーをプレイ
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.JoinCharaQuest: // キャラストーリーをプレイ
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.OrganizeSupport: // サポートを編成
				break;
			case MissionTypeEnum.OrganizeUnit: // 指定キャラを編成
				{
					var card = MasterDataTable.card[arg1];
					return string.Format(type.text_template, card.nickname);
                }
			case MissionTypeEnum.SettingInheritance: // 引き継ぎ設定
				break;
			case MissionTypeEnum.ChangeMainCard: // ホームキャラクター変更
				{
                    var card = MasterDataTable.card[arg1];
                    return string.Format(type.text_template, card.nickname);
                }
			case MissionTypeEnum.Reinforce: // 武器やキャラを強化
				{
                    var itemType = MasterDataTable.item_type[arg1];
                    return string.Format(type.text_template, itemType.display_name, arg2);
                }            
			case MissionTypeEnum.ReinforceMultiple: // 指定体数分の武器 or キャラを強化
				{
                    var itemType = MasterDataTable.item_type[arg1];
                    var suffix = itemType.Enum == ItemTypeEnum.card ? "体" : "個";
                    return string.Format(type.text_template, arg1+suffix, itemType.display_name);
                }
			case MissionTypeEnum.ReinforceThis: // 指定した武器やキャラを強化
				{
					var card = MasterDataTable.card[arg1];
					if(card != null){
						return string.Format(type.text_template, card.nickname, arg2);
					}
					var weapon = MasterDataTable.weapon[arg1];
					return string.Format(type.text_template, weapon.name, arg2);
                }
			case MissionTypeEnum.EquipArmor: // 武具を装備
				{
                    var itemType = MasterDataTable.item_type[arg1];
                    return string.Format(type.text_template, itemType.display_name);
                }     
			case MissionTypeEnum.EquipMultiple: // 指定体数に武具を装備
				{
                    var itemType = MasterDataTable.item_type[arg1];
                    return string.Format(type.text_template, arg2, itemType.display_name);
                }     
			case MissionTypeEnum.EquipThisArmor: // 指定武具を装備
				{
					var weapon = MasterDataTable.weapon[arg1];
					if (weapon != null) {
						return string.Format(type.text_template, weapon.name);
                    }
					// TODO:マギカイト
					var magikite = MasterDataTable.magikite[arg1];
					return string.Format(type.text_template, magikite.name);
                }     
			case MissionTypeEnum.LimitBreak: // 武器やキャラを限界突破
				{
                    var itemType = MasterDataTable.item_type[arg1];
                    return string.Format(type.text_template, itemType.display_name, arg2);
                }     
			case MissionTypeEnum.LimitBreakMultiple: // 指定体数武器やキャラを限界突破
				{
                    var itemType = MasterDataTable.item_type[arg1];
					var suffix = itemType.Enum == ItemTypeEnum.card ? "体" : "個";
                    return string.Format(type.text_template, arg2+suffix, itemType.display_name);
                }
			case MissionTypeEnum.LimitBreakThis: // 指定した武器やキャラを限界突破
				{
                    var card = MasterDataTable.card[arg1];
                    if(card != null){
                        return string.Format(type.text_template, card.nickname, arg2);
                    }
                    var weapon = MasterDataTable.weapon[arg1];
                    return string.Format(type.text_template, weapon.name, arg2);
                }
			case MissionTypeEnum.EvolutionUnit: // キャラクターを進化
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.EvolutionThisUnit: // 指定キャラクターを進化
				{
    				var card = MasterDataTable.card[arg1];    
                    return string.Format(type.text_template, card.nickname, arg2);
                }
			case MissionTypeEnum.ReleaseTrainingBoardSlot: // 育成ボードのスロット解放
				return string.Format(type.text_template, arg1);            
			case MissionTypeEnum.ReleaseTrainingBoardAllSlot: // 育成ボードのスロット全解放
				break;
			case MissionTypeEnum.ReleaseTrainingBoardSlotThisUnit: // 指定キャラの育成ボードスロット解放
				{
                    var card = MasterDataTable.card[arg1];
                    return string.Format(type.text_template, card.nickname, arg2);
                }
			case MissionTypeEnum.ReleaseTrainingBoardAllSlotThisUnit: // 指定キャラの育成ボードスロット全解放
				{
                    var card = MasterDataTable.card[arg1];
                    return string.Format(type.text_template, card.nickname);
                }
			case MissionTypeEnum.AchiveLogin: // ログイン日数達成
				return string.Format(type.text_template, arg1);
			case MissionTypeEnum.UseSkipTicket: // スキップチケットを使用
				return string.Format(type.text_template, arg1);
		}
		return type.text_template;
	}

    /// <summary>
    /// クリアに必要な数.
    /// </summary>
    public int NeedCount()
	{
		switch (type.Enum) {
            case MissionTypeEnum.ClearMainQuest: // 指定メインクエストクリア
				return 1;
            case MissionTypeEnum.ClearAllQuestInStage: // 指定ステージクリア
				return 1;
            case MissionTypeEnum.ClearAllQuestInChapter: // 指定章クリア
				return 1;
            case MissionTypeEnum.ClearAllQuestInCountry: // 指定国クエスト全クリア
				return 1;
            case MissionTypeEnum.ClearAllQuest: // メインストーリーを全クリア
				return 1;
            case MissionTypeEnum.ClearAllQuestThisUnit: // キャラストーリーを全クリア
				return 1;
            case MissionTypeEnum.AchiveUserLv: // ユーザーLv達成
				return arg1;
            case MissionTypeEnum.JoinPvP: // PvP参加
				return arg1;
            case MissionTypeEnum.JoinMainQuest: // メインストーリーをプレイ
				return arg1;
            case MissionTypeEnum.JoinSubQuest: // サブストーリーをプレイ
				return arg1;
            case MissionTypeEnum.JoinCharaQuest: // キャラストーリーをプレイ
				return arg1;
            case MissionTypeEnum.OrganizeSupport: // サポートを編成
				return 1;
            case MissionTypeEnum.OrganizeUnit: // 指定キャラを編成
				return 1;
            case MissionTypeEnum.SettingInheritance: // 引き継ぎ設定
				return 1;
            case MissionTypeEnum.ChangeMainCard: // ホームキャラクター変更
				return 1;
            case MissionTypeEnum.Reinforce: // 武器やキャラを強化
				return arg2;
            case MissionTypeEnum.ReinforceMultiple: // 指定体数分の武器 or キャラを強化
				return arg2;
            case MissionTypeEnum.ReinforceThis: // 指定した武器やキャラを強化
				return arg2;
            case MissionTypeEnum.EquipArmor: // 武具を装備
				return 1;
            case MissionTypeEnum.EquipMultiple: // 指定体数に武具を装備
				return arg2;
            case MissionTypeEnum.EquipThisArmor: // 指定武具を装備
				return 1;
            case MissionTypeEnum.LimitBreak: // 武器やキャラを限界突破
				return arg2;
            case MissionTypeEnum.LimitBreakMultiple: // 指定体数武器やキャラを限界突破
				return arg2;
            case MissionTypeEnum.LimitBreakThis: // 指定した武器やキャラを限界突破
				return arg2;
            case MissionTypeEnum.EvolutionUnit: // キャラクターを進化
				return arg1;
            case MissionTypeEnum.EvolutionThisUnit: // 指定キャラクターを進化
				return arg2;
            case MissionTypeEnum.ReleaseTrainingBoardSlot: // 育成ボードのスロット解放
				return arg1;
            case MissionTypeEnum.ReleaseTrainingBoardAllSlot: // 育成ボードのスロット全解放
				return 1;
            case MissionTypeEnum.ReleaseTrainingBoardSlotThisUnit: // 指定キャラの育成ボードスロット解放
				return arg2;
            case MissionTypeEnum.ReleaseTrainingBoardAllSlotThisUnit: // 指定キャラの育成ボードスロット全解放
				return 1;
            case MissionTypeEnum.AchiveLogin: // ログイン日数達成
				return arg1;
			case MissionTypeEnum.UseSkipTicket: // スキップチケットを使用
				return arg1;
        }
		Debug.LogError("[MissionSetting] Error!! : UnknownType : "+type.Enum);
        return -1;
	}   
}