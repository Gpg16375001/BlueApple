using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// uGUIのキャンバス関連ヘルパー.
/// </summary>
public static class CanvasHelper
{

    /// <summary>
    /// キャンバス管理でないオブジェクトのリサイズ処理.自身のルートに存在しているcanvasに対して合わせる.
    /// </summary>
    public static GameObject ResizeOtherObject(GameObject target)
    {
        var canvas = SearchCanvas(target);
        if(canvas == null){
            Debug.LogError("[CanvasHelper] ResizeOtherObject Error!! : Canvas not found.");
            return null;
        }
        var rectT = canvas.GetComponent<RectTransform>();

        // スケール.
        var scale = target.transform.localScale;
        target.transform.localScale = new Vector3(scale.x/rectT.localScale.x, scale.y/rectT.localScale.y, scale.z/rectT.localScale.z);
        // 座標.
        var dist = target.transform.localPosition;
        var x = !Mathf.Approximately(dist.x, 0) ? dist.x/rectT.localScale.x: 0;
        var y = !Mathf.Approximately(dist.y, 0) ? dist.y/rectT.localScale.y: 0;
        var z = !Mathf.Approximately(dist.z, 0) ? dist.z/rectT.localScale.z: 0;
        target.transform.localPosition = new Vector3(x, y, z); 

        return target;
    }
    private static Canvas SearchCanvas(GameObject target)
    {
        var canvas = target.transform.root.GetComponent<Canvas>();
        if(canvas == null) {
            canvas = target.transform.parent.GetComponent<Canvas>();
            if(canvas == null) {
                canvas = target.GetComponent<Canvas>();
            }
        }
        return canvas;
    }
}
