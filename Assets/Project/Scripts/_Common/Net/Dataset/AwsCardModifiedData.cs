using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Amazon.CognitoSync.SyncManager;
using UnityEngine;


namespace SmileLab.Net.API
{ 

    /// <summary>
    /// カード毎の恒久的に保持する必要のあるローカル管理変更データ.ボイスやプロフィールの開放状況など.
    /// </summary>
	public class AwsCardModifiedData : AwsCognitoDatasetBase
    {
		/// <summary>
        /// フレーバーテキスト2の解放をすでに見ているかどうか.
        /// </summary>
		public List<CardModifiedSaveData> List
		{
			get {
				var dat = Get<Serialization<CardModifiedSaveData>>("CardModifiedList");
				if (dat == null) {
					var listEmpty = new List<CardModifiedSaveData>();
					Put("CardModifiedList", new Serialization<CardModifiedSaveData>(listEmpty));
					return listEmpty;
                }
				return dat.ToList();
			}
		}

        /// <summary>
        /// データ更新.
        /// </summary>
		public void UpdateData(CardData card)
		{
			var list = List;
			var target = list.Find(c => c.CardId == card.CardId);
			if(target == null){
				target = new CardModifiedSaveData();
			}
			list.RemoveAll(c => c.CardId == target.CardId);
   
			target.CardId = card.CardId;
			// フレーバー.
			if(!target.IsSeenReleaseFlavor2){
				target.IsNeedSeeReleaseFlavor2 = card.IsReleaseFlavor2;
			}
			// ボイス.
			var dict = target.NeedSeeReleaseVoiceDictionary;
			foreach(var cue in Enum.GetValues(typeof(SoundVoiceCueEnum)) as SoundVoiceCueEnum[]){
				var set = MasterDataTable.card_detail_voice_setting.DataList.Find(s => s.voice_cue_id == cue);
                if (set == null) {
					target.RemoveRegistedVoice((int)cue);
                    continue;
                }
				if(cue == SoundVoiceCueEnum.evolution3){
					var bMobRarity2 = card.MinRarity <= 2 && card.MaxRarity < 6;
					var bDifLessThanR3 = card.MaxRarity - card.MinRarity < 3;
					if(bMobRarity2 || bDifLessThanR3){
						target.RemoveRegistedVoice((int)cue);
						continue;   // 3段階進化しないので進化3段階目のボイスは無視.
					}
				}            
				var bSeen = false;
				if(target.SeenReleaseVoiceDictionary != null){
					target.SeenReleaseVoiceDictionary.TryGetValue((int)cue, out bSeen);               
				}
				if(dict.ContainsKey((int)cue)){
					dict[(int)cue] = !bSeen && set.IsReleaseVoice(card);               
				}else{
					dict.Add((int)cue, !bSeen && set.IsReleaseVoice(card));
				}
			}         

			list.Add(target);
            Put("CardModifiedList", new Serialization<CardModifiedSaveData>(list));
		}

		/// <summary>
        /// ボイス項目だけ確認.
        /// </summary>
		public void ConfirmedVoiceOnly(CardData card)
        {
			Debug.Log("ConfirmedVoiceOnly");
			var list = List;
            var target = list.Find(c => c.CardId == card.CardId);
            if (target == null) {
                target = new CardModifiedSaveData();
			}else{
				list.RemoveAll(c => c.CardId == target.CardId);
			}         
            target.CardId = card.CardId;
			
			var seenDict = target.SeenReleaseVoiceDictionary;
            var needDict = target.NeedSeeReleaseVoiceDictionary;
            if (target.NeedSeeReleaseVoiceDictionary != null) {
                foreach (var cue in Enum.GetValues(typeof(SoundVoiceCueEnum)) as SoundVoiceCueEnum[]) {
                    var set = MasterDataTable.card_detail_voice_setting.DataList.Find(s => s.voice_cue_id == cue);
                    if (set == null) {
                        continue;
                    }
					var bNeedSee = needDict.ContainsKey((int)cue) && needDict[(int)cue];
                    if (bNeedSee) {
                        if (seenDict.ContainsKey((int)cue)) {
                            seenDict[(int)cue] = true;
                        } else {
                            seenDict.Add((int)cue, true);
                        }
                        needDict[(int)cue] = false;
                    }
                }
            }

            list.Add(target);
            Put("CardModifiedList", new Serialization<CardModifiedSaveData>(list));
        }

		/// <summary>
        /// フレーバーだけ確認.
        /// </summary>
        public void ConfirmedFlavorOnly(CardData card)
        {	         
			Debug.Log("ConfirmedFlavorOnly");
            var list = List;
            var target = list.Find(c => c.CardId == card.CardId);
            if (target == null) {
                target = new CardModifiedSaveData();
            } else {
                list.RemoveAll(c => c.CardId == target.CardId);
            }
			if (!target.IsNeedSeeReleaseFlavor2) {
                return;
            }

            target.CardId = card.CardId;         
            target.IsSeenReleaseFlavor2 = true;
            target.IsNeedSeeReleaseFlavor2 = false;

            list.Add(target);
            Put("CardModifiedList", new Serialization<CardModifiedSaveData>(list));
        }

		public AwsCardModifiedData(CognitoSyncManager mng) : base(mng, "CardModifiedData"){}
  
		protected override void ClearValues(){}

        /// <summary>
        /// カードごとの変更データ管理用セーブデータ.
        /// </summary>
		[Serializable]
		public class CardModifiedSaveData: ISerializationCallbackReceiver
		{
			/// <summary>ボイス解放確認の必要があるかどうか.</summary>
            public bool IsAnyNeedSeeReleaseVoice
            {
                get {
					if(NeedSeeReleaseVoiceDictionary == null){
						return false;
					}
					return NeedSeeReleaseVoiceDictionary.Any(kvp => kvp.Value);
                }
            }
            
			/// <summary>カードID.</summary>
			public int CardId;
			/// <summary>フレーバーテキスト2の解放をすでに見ているかどうか.</summary>
			public bool IsSeenReleaseFlavor2;
			/// <summary>フレーバーテキスト2の解放を見る必要があるかどうか.</summary>
            public bool IsNeedSeeReleaseFlavor2;
            /// <summary>既に聴いているボイスリスト.</summary>
			public Dictionary<int, bool> SeenReleaseVoiceDictionary { get { return seenReleaseVoiceDictionary; } }
			/// <summary>ボイスを聞く必要があるリスト.</summary>
			public Dictionary<int, bool> NeedSeeReleaseVoiceDictionary { get { return needSeeReleaseVoiceDictionary; } }
   
			[SerializeField]
            List<int> seenKeys;
            [SerializeField]
            List<bool> seenValues;
			Dictionary<int, bool> seenReleaseVoiceDictionary;

			[SerializeField]
			List<int> needKeys;
            [SerializeField]
            List<bool> needValues;
			Dictionary<int, bool> needSeeReleaseVoiceDictionary;

			public void OnBeforeSerialize()
			{
				seenKeys = new List<int>(seenReleaseVoiceDictionary.Keys);
				seenValues = new List<bool>(seenReleaseVoiceDictionary.Values);
				needKeys = new List<int>(needSeeReleaseVoiceDictionary.Keys);
				needValues = new List<bool>(needSeeReleaseVoiceDictionary.Values);
			}

			public void OnAfterDeserialize()
			{ 
				var count = Math.Min(seenKeys.Count, seenValues.Count);
				seenReleaseVoiceDictionary = new Dictionary<int, bool>(count);
                for (var i = 0; i < count; ++i) {
					seenReleaseVoiceDictionary.Add(seenKeys[i], seenValues[i]);
                }

				count = Math.Min(needKeys.Count, needValues.Count);
				needSeeReleaseVoiceDictionary = new Dictionary<int, bool>(count);
                for (var i = 0; i < count; ++i) {
					needSeeReleaseVoiceDictionary.Add(needKeys[i], needValues[i]);
                }
			}

            public void RemoveRegistedVoice(int voiceId)
			{
				if (NeedSeeReleaseVoiceDictionary.ContainsKey(voiceId)) {
					NeedSeeReleaseVoiceDictionary.Remove(voiceId);
                }
				if (SeenReleaseVoiceDictionary.ContainsKey(voiceId)) {
					SeenReleaseVoiceDictionary.Remove(voiceId);
                }
			}

			public CardModifiedSaveData()
			{
				seenReleaseVoiceDictionary = new Dictionary<int, bool>();
				needSeeReleaseVoiceDictionary = new Dictionary<int, bool>();            
			}
		}
	}   
}