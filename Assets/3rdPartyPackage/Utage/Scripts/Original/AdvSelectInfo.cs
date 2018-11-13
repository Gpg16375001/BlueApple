/**
 *  AdvSelectInfo  
 *
 *  @author Ryo Yoshida <yoshryo@smile-lab.com>
 */
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// struct : 選択肢の選択情報.
/// </summary>
[Serializable]
public struct AdvSelectInfo
{
    public string SheetName;    // シート名.
    public int RowIndex;        // 行番号.
    public int ID;              // 選択肢ID.
}
public static class AdvSelectInfoEx
{
    /// <summary>
    /// 選択肢選択情報の書き込み.
    /// </summary>
    public static void WriteData(this AdvSelectInfo self)
    {
        var baseText = "";
        if (File.Exists(FILE_PATH)) {
            var bytes = File.ReadAllBytes(FILE_PATH);
            baseText = Encoding.UTF8.GetString(bytes)+"\n";
        }
        baseText += self.SheetName+":"+self.RowIndex+":"+self.ID;
        File.WriteAllBytes(FILE_PATH, Encoding.UTF8.GetBytes(baseText));
    }

    /// <summary>
    /// 選択肢情報の読み込み.
    /// </summary>
    public static List<AdvSelectInfo> ReadData()
    {
        var rtn = new List<AdvSelectInfo>();
        if(!File.Exists(FILE_PATH)){
            return rtn;
        }
        var bytes = File.ReadAllBytes(FILE_PATH);
        var text = Encoding.UTF8.GetString(bytes);
        foreach(var line in text.Split('\n')){
            var fields = line.Split(':');
            AdvSelectInfo info;
            info.SheetName = fields[0];
            info.RowIndex = int.Parse(fields[1]);
            info.ID = int.Parse(fields[2]);
            rtn.Add(info);
        }
        return rtn;
    }

    /// <summary>
    /// 削除処理.
    /// </summary>
    public static void DeleteData()
    {
        if (!File.Exists(FILE_PATH)) {
            return;
        }
        File.Delete(FILE_PATH);
    }

	private static string FILE_PATH { get { return SmileLab.GameSystem.PermanentDirectoryPath + "/AdvSelectInfo.txt";  } }
}
    
