using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class PurchaseTestSController : ScreenControllerBase {

    /// <summary>
    /// の生成.
    /// </summary>
    public override void CreateBootScreen()
    {
        // BGM再生テスト.

        var go = GameObjectEx.LoadAndCreateObject("Debug/PurchaseTest/Screen_PurchaseTest", this.gameObject);
        var c = go.GetOrAddComponent<Screen_PurchaseTest>();
        c.Init();
    }
}
