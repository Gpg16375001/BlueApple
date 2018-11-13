using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class GemProduct {

    /// <summary>
    /// 商品アイコンロード.
    /// </summary>
    public void LoadIcon(Action<Sprite> didLoad)
    {
        DLCManager.AssetBundleFromFileOrDownload<SpriteAtlas>(DLCManager.DLC_FOLDER.UI,
            "bundle_gemproduct",
            "gemproduct",
            asset => {
                var spt = asset.GetSprite(id.ToString());
                didLoad(spt);
            },
            ex => {
                // エラーでても無視する。
            }
        );
    }   
}
