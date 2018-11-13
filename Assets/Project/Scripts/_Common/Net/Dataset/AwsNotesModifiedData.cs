using System;
using System.Collections;
using System.Collections.Generic;
using Amazon.CognitoSync.SyncManager;
using UnityEngine;


namespace SmileLab.Net.API
{
	/// <summary>
    /// お知らせの見た見てないデータ.
    /// </summary>
	public class AwsNotesModifiedData : AwsCognitoDatasetBase
    {
		/// <summary>
        /// データリスト.カテゴリー毎.
        /// </summary>
		public List<NotesModifiedSaveData> List
		{
			get {
				var dat = Get<Serialization<NotesModifiedSaveData>>("NotesModifiedList");
                if (dat == null) {
					var listEmpty = new List<NotesModifiedSaveData>();
					Put("NotesModifiedList", new Serialization<NotesModifiedSaveData>(listEmpty));
                    return listEmpty;
                }
                return dat.ToList();
			}
		}

        /// <summary>
        /// 指定データがNewか.
        /// </summary>
		public bool IsNew(CommonNotice data)
		{
			var saveData = List.Find(d => d.CategoryValue == (int)data.category);
			foreach(var kvp in saveData.NeedSeeReleaseDictionary){
				if(kvp.Key == data.id){
					return kvp.Value;
				}
			}
			return false;
		}

        /// <summary>
        /// 指定カテゴリーにnewが存在するか.
        /// </summary>
		public bool IsNew(CommonNoticeCategoryEnum category)
		{
			var saveData = List.Find(d => d.CategoryValue == (int)category);
            foreach (var kvp in saveData.NeedSeeReleaseDictionary) {
				if(kvp.Value){
					return true;
				}
            }
			return false;
		}

		/// <summary>
		/// データ更新.
		/// </summary>
        public void UpdateDataAll()
		{
			foreach(var e in Enum.GetValues(typeof(CommonNoticeCategoryEnum)) as CommonNoticeCategoryEnum[]){
				this.UpdateData(e);
			}
		}
		public void UpdateData(CommonNoticeCategoryEnum category)
		{ 
			var list = List;
			var target = list.Find(d => d.CategoryValue == (int)category);
            if (target == null) {
				target = new NotesModifiedSaveData();
			}else{
				list.RemoveAll(c => c.CategoryValue == target.CategoryValue);
			}         
   
			target.CategoryValue = (int)category;

			var dict = target.NeedSeeReleaseDictionary;
			foreach (var dat in MasterDataTable.notice.GetListThisPlatform(category)) { 
				var bSeen = false;
                if (target.SeenReleaseDictionary != null) {
                    target.SeenReleaseDictionary.TryGetValue(dat.id, out bSeen);
                }            
				if (dict.ContainsKey(dat.id)){
					dict[dat.id] = !bSeen;
                } else {
                    dict.Add(dat.id, !bSeen);
                }
			}

			list.Add(target);
			Put("NotesModifiedList", new Serialization<NotesModifiedSaveData>(list));
		}

		/// <summary>
		/// 確認したら呼ぶ.
		/// </summary>
		public void ConrirmedData(CommonNotice data)
		{ 
			var list = List;
			var target = list.Find(d => d.CategoryValue == (int)data.category);
            if (target == null) {
                target = new NotesModifiedSaveData();
			}else{
				list.RemoveAll(c => c.CategoryValue == target.CategoryValue);
			}         
   
			target.CategoryValue = (int)data.category;

            var seenDict = target.SeenReleaseDictionary;
            var needDict = target.NeedSeeReleaseDictionary; 
			var bNeedSee = target.NeedSeeReleaseDictionary[data.id];
            if (bNeedSee) {
				if (seenDict.ContainsKey(data.id)) {
					seenDict[data.id] = true;
                } else {
					seenDict.Add(data.id, true);
                }
				needDict[data.id] = false;
            } 

			list.Add(target);
            Put("NotesModifiedList", new Serialization<NotesModifiedSaveData>(list));
		}

		public AwsNotesModifiedData(CognitoSyncManager mng) : base(mng, "AwsNotesModifiedData") { }
		protected override void ClearValues() { }
  
		[Serializable]
		public class NotesModifiedSaveData : ISerializationCallbackReceiver
		{
			/// CommonNoticeCategoryEnumのint値.
			public int CategoryValue;

			/// <summary>既にみているリスト.</summary>
			public Dictionary<int, bool> SeenReleaseDictionary { get { return seenReleaseDictionary; } }
            /// <summary>みる必要があるリスト.</summary>
			public Dictionary<int, bool> NeedSeeReleaseDictionary { get { return needSeeReleaseDictionary; } }

			[SerializeField]
            List<int> seenKeys;
            [SerializeField]
            List<bool> seenValues;
			Dictionary<int, bool> seenReleaseDictionary;

            [SerializeField]
            List<int> needKeys;
            [SerializeField]
            List<bool> needValues;
			Dictionary<int, bool> needSeeReleaseDictionary;

			public void OnBeforeSerialize()
            {
                seenKeys = new List<int>(seenReleaseDictionary.Keys);
                seenValues = new List<bool>(seenReleaseDictionary.Values);
                needKeys = new List<int>(needSeeReleaseDictionary.Keys);
                needValues = new List<bool>(needSeeReleaseDictionary.Values);
            }

			public void OnAfterDeserialize()
            {
                var count = Math.Min(seenKeys.Count, seenValues.Count);
                seenReleaseDictionary = new Dictionary<int, bool>(count);
                for (var i = 0; i < count; ++i) {
                    seenReleaseDictionary.Add(seenKeys[i], seenValues[i]);
                }

                count = Math.Min(needKeys.Count, needValues.Count);
                needSeeReleaseDictionary = new Dictionary<int, bool>(count);
                for (var i = 0; i < count; ++i) {
                    needSeeReleaseDictionary.Add(needKeys[i], needValues[i]);
                }
            }

			public NotesModifiedSaveData()
            {
                seenReleaseDictionary = new Dictionary<int, bool>();
                needSeeReleaseDictionary = new Dictionary<int, bool>();
            }
		}
    }

}