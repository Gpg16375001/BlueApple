using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// View : ガチャの詳細説明ポップ.
/// </summary>
public class View_GachaDescription : ViewBase
{

	public static void  Create()
	{
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaDescription");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_GachaDescription>();
		c.InitInternal();
	}
	private void InitInternal()
	{
		this.GetScript<TextMeshProUGUI>("txtp_Notes").text = TextData.GetText("GACHA_RARITY4_NOTE");
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
	}

	// ボタン : 閉じる.
    void DidTapClose()
	{
		this.StartCoroutine(this.CoPlayClose());
	}   
	IEnumerator CoPlayClose()
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		this.Dispose();
    }

	void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}
}
