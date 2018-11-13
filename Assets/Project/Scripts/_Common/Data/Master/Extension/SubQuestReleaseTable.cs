using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// partial class : サブクエスト解放条件テーブル.
/// </summary>
public partial class SubQuestReleaseTable
{

    /// <summary>
    /// 指定した選択肢IDで解放するサブクエストがあるかどうか.あればそのサブクエスト情報を返す.
    /// </summary>
    public SubQuest GetReleaseQuest(int selectId, List<int> selectIdList, List<int> subQuestIdList)
    {
        // すでに選択済みの選択肢を指定.
        if(selectIdList.Exists(id => id == selectId)){
            return null;
        }
        // 解放済みのサブクエストインデックスを抽出.
        var releaseIndexList = subQuestIdList.Where(id => MasterDataTable.quest_sub[id] != null)
                                             .Select(id => MasterDataTable.quest_sub[id].index)
                                             .Distinct()
                                             .ToList();
        // サブクエスト解放テーブルから解放済みのデータ以外を抽出.
        var releaseList = DataList;
        if (releaseIndexList != null && releaseIndexList.Count > 0) {
            releaseList = DataList.Where(q => !releaseIndexList.Exists(idx => idx == q.index))
                                  .ToList();
        }
        if(releaseList == null || releaseList.Count <= 0){
            return null;
        }
        foreach(var releaseInfo in releaseList){
            if(releaseInfo.select_1 == selectId && selectIdList.Exists(id => id == releaseInfo.select_2)){
                var list = MasterDataTable.quest_sub.DataList.FindAll(q => q.index == releaseInfo.index);
                list.Sort((x, y) => x.id - y.id);
                return list.First();
            }
            if (releaseInfo.select_2 == selectId && selectIdList.Exists(id => id == releaseInfo.select_1)) {
                var list = MasterDataTable.quest_sub.DataList.FindAll(q => q.index == releaseInfo.index);
                list.Sort((x, y) => x.id - y.id);
                return list.First();
            }
        }
        return null;
    }
}
