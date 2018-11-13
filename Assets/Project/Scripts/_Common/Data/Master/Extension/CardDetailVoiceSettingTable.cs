using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab.Net.API;

public partial class CardDetailVoiceSettingTable
{
	public List<CardDetailVoiceSetting> GetEnableList(CardData card)
	{
		// レアリティが2の場合は基本的に3に進化で1段目、4に進化で2段目のボイスを再生、以降はボイス再生なし.
		if(card.MinRarity <= 2){
			if(card.MaxRarity < 6){
				return DataList.Where(d => d.voice_cue_id != SoundVoiceCueEnum.evolution3).ToList();
			}
            // 最高レアリティが星6以上あればボイスの3段階目も使用する.
			return DataList;
		}
		if(card.MaxRarity - card.MinRarity < 3){
			return DataList.Where(d => d.voice_cue_id != SoundVoiceCueEnum.evolution3).ToList();
		}
		return DataList;
	}
}
