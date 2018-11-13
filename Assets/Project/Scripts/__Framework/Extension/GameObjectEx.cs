using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


/// <summary>
/// ゲームオブジェクト拡張クラス.
/// </summary>
public static class GameObjectEx
{
	/// <summary>
	/// ResourcesLoadからゲームオブジェクトを生成.
	/// </summary>
	public static GameObject LoadAndCreateObject(string name, GameObject parent = null)
	{
		var o = Resources.Load(name) as GameObject;
        if(o==null) {
            throw new NullReferenceException("プレハブ「" + name + "」がリソースに見つかりませんでした。");
        }
		var go = GameObject.Instantiate(o) as GameObject;
		if(parent != null){
			parent.AddInChild(go);
		}
		return go;
	}

	/// <summary>
	/// 子オブジェクトをすべて取得
	/// </summary>
	public static GameObject[] GetChildren(this GameObject self)
	{
		var rtn = new List<GameObject>();
		foreach(Transform t in self.transform){
			rtn.Add(t.gameObject);
		}
		return rtn.ToArray();
	}
	
	/// <summary>
	/// 子オブジェクトを追加.
	/// </summary>
	public static void AddInChild(this GameObject self, GameObject child)
	{
		var p	= child.transform.localPosition;
		var r	= child.transform.localRotation;
		var s	= child.transform.localScale;

        child.transform.SetParent(self.transform);

		child.transform.localPosition	= p;
		child.transform.localRotation	= r;
		child.transform.localScale	= s;
	}
	
	/// <summary>
	/// 子オブジェクトをすべて破棄.
	/// </summary>
	public static void DestroyChildren(this GameObject self, bool bImmediate = true)
	{
		foreach(Transform t in self.transform){
			if(t != null && t.gameObject != null){
				GameObject.Destroy(t.gameObject);
			}
		}
        if(bImmediate){
            self.transform.DetachChildren();
        }
	}

	/// <summary>
	/// 必要であればAddComponentするGetComponent.
	/// </summary>
	public static T GetOrAddComponent<T>(this GameObject self) where T : Component
	{
        var rtn = self.GetComponent<T>();
        if(rtn == null){
            rtn = self.AddComponent<T>();
        }
        return rtn;
	}

    /// <summary>
    /// uGUICanvasで適宜ルートオブジェクトとして必要なコンポーネントをアタッチor設定する.
    /// matchWidthOrHeightを設定しない限りShrinkで設定する.
    /// ※ OrderInLayer値はプロジェクトごとのレギュレーションがあると思われるので必ずあらかじめ手動で設定しておいて下さい。
    /// ここでは設定しません。
    /// </summary>
    public static void AttachUguiRootComponent(this GameObject self, Camera mainCamera, float matchWidthOrHeight = -1f)
    {
		// 必要最低限の設定のみ.
        var canvas = self.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = mainCamera;
              
        // iPad対応.
		var scaler = self.GetOrAddComponent<CanvasScaler>();
		if(SystemInfo.deviceModel.Contains("iPad")){
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			scaler.matchWidthOrHeight = 0;
		}

		self.GetOrAddComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// 自分自身を含む全ての子オブジェクトのレイヤーを設定する.
    /// </summary>
	public static void SetLayerRecursively(this GameObject self, int layer)   
	{
		self.layer = layer;
		foreach(Transform t in self.transform){
			SetLayerRecursively(t.gameObject, layer);
		}
	}
}
