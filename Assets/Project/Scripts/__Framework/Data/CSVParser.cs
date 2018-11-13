using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace SmileLab {
/// <summary>
/// CSVテキストのパースを提供します。
/// </summary>
public static class CSVParser
{
    /// <summary>
    /// CSVテキストを指定した型の配列にパースします。
    /// </summary>
    public static T[] Execute<T>(string csvText)
    {
        var masterList = new List<T>();
        var masterType = typeof(T);
        using(var sr = new StringReader(csvText)) {
            var columnNames = sr.ReadLine().Split(',');
            while(sr.Peek() != -1) {
                var lineText = sr.ReadLine().Replace("\\\"", "[DOUBLEQUAUTO]");
                if(string.IsNullOrEmpty(lineText)) continue;
                var lineMaster = Activator.CreateInstance<T>();
                foreach(var columnName in columnNames) {
                    var columnProperty = masterType.GetProperty(columnName);
                    if(columnProperty == null) continue;
                    var columnTypeName = columnProperty.PropertyType.Name;
                    var fieldText = string.Empty;
                    switch(columnTypeName) {
                        case "String":
                            var doubleQuautoIndex = lineText.IndexOf('"', 1);
                            fieldText = lineText.Substring(1, doubleQuautoIndex - 1).Replace("[DOUBLEQUAUTO]", "\"").Replace("\\n", "\n");
                            lineText = lineText.Substring(doubleQuautoIndex + 1, lineText.Length - doubleQuautoIndex - 1).TrimStart(',');
                            break;

                        default:
                            var commaIndex = lineText.IndexOf(',', 1);
                            if(commaIndex >= 0) {
                                fieldText = lineText.Substring(0, commaIndex);
                                lineText = lineText.Substring(commaIndex + 1, lineText.Length - commaIndex - 1);
                            } else {
                                fieldText = lineText;
                                lineText = string.Empty;
                            }
                            break;
                    }
                    var fieldValue = null as object;
                    switch(columnTypeName) {
                        // 型パースを追加する場合はここ
                        case "String":
                            fieldValue = fieldText;
                            break;

                        case "Int32":
                            fieldValue = int.Parse(fieldText);
                            break;

                        case "Int32[]":
                            fieldValue = fieldText.Split(',').Select(s => int.Parse(s)).ToArray();
                            break;

                        default:
                            throw new NotSupportedException("CSVパースクラスは「" + columnTypeName + "」型のフィールドには対応していません。実装を追加するか他の型での代用を検討してください。");
                    }
                    columnProperty.SetValue(lineMaster, fieldValue, null);
                }
                masterList.Add(lineMaster);
            }
        }
        return masterList.ToArray();
    }
}
}
