using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab.Net.API;


public partial class UnitQuestTable
{   
    /// <summary>全キャラクターの最初のクエスト.</summary>
	public List<UnitQuest> DistinctFirstList 
	{ 
		get {
			var rtn = new List<UnitQuest>();
			var cids = DataList.Select(q => q.card_id).Distinct();
			foreach(var cid in cids){
				var firstId = DataList.FindAll(q => q.card_id == cid).Select(q => q.id).Min();
				rtn.Add(DataList.Find(q => q.id == firstId));
			}
			return rtn; 
		} 
	}

    /// <summary>
    /// このカードのクエストリストを取得.
    /// </summary>
    public List<UnitQuest> GetListThisCard(int card_id)
	{
		return DataList.Where(d => d.card_id == card_id).ToList();
	}
}
