using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MiniJSON;

using SmileLab;


/// <summary>
/// View : メンテナンス ＆ ストア誘導用のポップアップ.
/// </summary>
public class View_Popup_Maintenance : ViewBase
{
	/// <summary>
    /// メンテナンスView生成.
    /// </summary>
	public static void CreateMaintenance(string jsonText = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("_Common/Popup/View_Popup_Maintenance");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_Popup_Maintenance>();
		c.InitInternalMaintenance(jsonText);
	}
	private void InitInternalMaintenance(string jsonText = null)
	{
		this.GetScript<RectTransform>("Maintenance").gameObject.SetActive(true);
		this.GetScript<RectTransform>("Update").gameObject.SetActive(false);

		this.GetScript<TextMeshProUGUI>("txtp_PopupTitle").text = "メンテナンス中です";

		if (!string.IsNullOrEmpty(jsonText)) {
			Debug.Log("CreateMaintenance from json. \n\n" + jsonText);
			var json = Json.Deserialize(jsonText) as Dictionary<string, object>;
			var text = (string)json["text"];
			var startDate = DateTime.Parse((string)json["start_date"]).ToJapanString("MM月dd日 (ddd) HH:mm");
			var endDate = DateTime.Parse((string)json["end_date"]).ToJapanString("MM月dd日 (ddd) HH:mm");
			this.GetScript<TextMeshProUGUI>("Maintenance/txtp_Notes1").text = text;
			this.GetScript<TextMeshProUGUI>("Maintenance/txtp_Schedule").text = string.Format("[終了予定] {0}", endDate);
		}else{
			// Maintenaceマスターから情報を取得してポップアップに記載. 
            var data = MasterDataTable.maintenance.DataList.Find(d => d.store_type.ToString() == GameSystem.GetPlatformName());
            if (data != null) {
                this.GetScript<TextMeshProUGUI>("Maintenance/txtp_Notes1").text = data.text;
                this.GetScript<TextMeshProUGUI>("Maintenance/txtp_Schedule").text = string.Format("[終了予定] {0}", data.end_date.ToJapanString("MM月dd日 (ddd) HH:mm"));
            }
		}      

		// ボタン.
		this.GetScript<RectTransform>("L").gameObject.SetActive(true);
		this.GetScript<RectTransform>("R").gameObject.SetActive(true);
		this.GetScript<TextMeshProUGUI>("txtp_L").text = "公式サイトへ";
		this.GetScript<TextMeshProUGUI>("txtp_R").text = "タイトル画面へ";
		this.SetCanvasCustomButtonMsg("L/bt_Common", DidTapHP);
		this.SetCanvasCustomButtonMsg("R/bt_Common", DidTapTitle);
	}
    
    /// <summary>
	/// ストア遷移用View生成.
    /// </summary>
    public static void CreateGoToStore()
	{
		var go = GameObjectEx.LoadAndCreateObject("_Common/Popup/View_Popup_Maintenance");
        var c = go.GetOrAddComponent<View_Popup_Maintenance>();
		c.InitInternalGoToStore();
	}
	private void InitInternalGoToStore()
	{
		this.GetScript<RectTransform>("Maintenance").gameObject.SetActive(false);
        this.GetScript<RectTransform>("Update").gameObject.SetActive(true);

		this.GetScript<TextMeshProUGUI>("txtp_PopupTitle").text = "アップデートのお願い";

        // ボタン.
		this.GetScript<RectTransform>("L").gameObject.SetActive(false);
        this.GetScript<RectTransform>("R").gameObject.SetActive(true);
        this.GetScript<TextMeshProUGUI>("txtp_R").text = "ストアへ";
		this.SetCanvasCustomButtonMsg("R/bt_Common", DidTapStore);
	}

	#region ButtonDelegate.

	// ボタン : ホームページへ
    void DidTapHP()
	{
		Application.OpenURL("https://precatus.com/");
	}

	// ボタン : タイトル画面へ
    void DidTapTitle()
	{
		this.StartCoroutine(this.CoPlayOpenClose(() => {
			ScreenChanger.SharedInstance.GoToTitle();
		}));
	}

	// ボタン : ストアに遷移.
    void DidTapStore()
	{
		// TODO : ストアURLが決まったらちゃんと設定する.忘れずに！
		var storeName = GameSystem.GetPlatformName();
		if(storeName == StoreTypeEnum.AppStore.ToString()){
			Application.OpenURL(string.Format("https://itunes.apple.com/jp/app/{0}/id1410091440?mt=8", WWW.EscapeURL("プレカトゥスの天秤")));
		}else if(storeName == StoreTypeEnum.GooglePlay.ToString()){
			Application.OpenURL("https://play.google.com/store/apps/details?id=jp.fg.precatus");
		}else if(storeName == StoreTypeEnum.AUMarket.ToString()){
			Application.OpenURL("https://play.google.com/store/apps/details?id=jp.fg.precatus");
		}

	}

    #endregion.

	IEnumerator CoPlayOpenClose(Action didClose)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		if(didClose != null){
			didClose();
		}
        this.Dispose();
    }

	void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.FadeCamera);
	}
}
