using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;


/// <summary>
/// テキストメッシュプロ用の拡張メソッド
/// </summary>
public static class TextMeshProExtension {
    public static void SetText(this TextMeshPro self, object obj)
    {
        if (self == null) {
            return;
        }
        self.SetText (obj.ToString ());
    }

    public static void SetTextFormat(this TextMeshPro self, string format, params object[] parameters)
    {
        if (self == null) {
            return;
        }
        self.SetText (string.Format(format, parameters));
    }

    public static void SetText(this TextMeshProUGUI self, object obj)
    {
        if (self == null) {
            return;
        }
        self.SetText (obj.ToString ());
    }

    public static void SetTextFormat(this TextMeshProUGUI self, string format, params object[] parameters)
    {
        if (self == null) {
            return;
        }
        self.SetText (string.Format(format, parameters));
    }
}
