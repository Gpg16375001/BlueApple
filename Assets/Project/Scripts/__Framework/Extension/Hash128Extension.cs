using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Hash128Extension
{
    public static uint[] Hash128ToIntArray(this Hash128 self) {
        var hashString = self.ToString ();
        byte[] bytes = new byte[16];
        for (int i = 0; i < 16; ++i) {
            bytes [i] = System.Convert.ToByte (hashString.Substring (i * 2, 2), 16);
        }

        // 文字からの変換のためBigEndianとして扱う
        return new uint[4] {
            System.BitConverter.ToUInt32(bytes, 0),
            System.BitConverter.ToUInt32(bytes, 4),
            System.BitConverter.ToUInt32(bytes, 8),
            System.BitConverter.ToUInt32(bytes, 12)
        };
    }
}
