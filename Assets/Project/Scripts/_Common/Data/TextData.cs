using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テキストデータ取得用関数
/// </summary>
public static class TextData {
    static CommonTextTable table;

    /// <summary>
    /// キーワードから文言情報を取得する
    /// </summary>
    /// <returns>文言情報</returns>
    /// <param name="keyword">キーワード</param>
    public static string GetText(string keyword)
    {
        if (table == null) {
            table = MasterDataTable.text;
        }

        var text = table [keyword];
        if (text == null) {
            
            return string.Empty;
        }
        return text.Text;
    }

    /// <summary>
    /// キャッシュしているマスターのテーブル情報をクリアする。
    /// </summary>
    public static void ClearCache()
    {
        table = null;
    }
}
