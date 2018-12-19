using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// チュートリアル用のリソースダウンローダー.
/// </summary>
public class TutorialResourceDownloader
{
	public static bool IsLoadEnd { get { return IsDownloadEndForInit && IsDownloadEndUtage; } }


	public static bool IsDownloadEndForInit { get; private set; }

	public static bool IsDownloadEndUtage { get; private set; }

	public static List<KeyValuePair<int, WeakReference>> GachaObjectList { get; private set; }
	public static ReceiveGachaExecuteTutorialGacha GachaResponse { get; private set; }
	public static bool IsDownloadEndForGacha { get; private set; }

    /// <summary>
    /// ダウンロード処理開始.
    /// </summary>
    public static void StartDownload()
	{
        if (IsStartLoad) {
            return;
        }
        IsStartLoad = true;

		IsDownloadEndForInit = false;
		Debug.Log("StartDownload");

		// 初回必要最低限のアセットのダウンロード.
        List<string> downloadS3Paths = new List<string>();
        downloadS3Paths.Add(DLCManager.GetS3Path(DLCManager.DLC_FOLDER.Icon, "unit_icon"));

        // ど頭にDLを悟られるとiOSで必要最低限のアセットがバイナリに含まれていないとしてリジェクトを食らうのであえてプログレスを表示しない.
        DLCManager.DownloadFiles(downloadS3Paths,
            (ret) => {
				IsDownloadEndForInit = true;
			    Debug.Log("TutorialResourceDownloader IsDownloadEndForInit="+IsDownloadEndForInit);
            },
            null
        );

		// 宴シナリオもロード
		LoadUtageChapters();
	}

	// 通信リクエスト : チュートリアルガチャ.
	public static void RequestGacha()
    {
		IsDownloadEndForGacha = false;
		Debug.Log("RequestGacha");
  
        SendAPI.GachaExecuteTutorialGacha((bSuccess, res) => {
			if(res == null){
				return; // TODO : どうしようもないエラーなので何もせずに処理を終える.
			}
            if (!bSuccess) {
				PopupManager.OpenPopupSystemOK("通信エラーが発生しました。\n再起動します。", () => ScreenChanger.SharedInstance.Reboot());
                return;
			}
			GachaResponse = res;
			CoroutineAgent.Execute(LoadLive2dProc(res.AcquiredGachaItemDataList.Select(i => MasterDataTable.card[i.ItemId]).ToList()));
        });
    }
	// ガチャ排出のLive2Dモデルロード.
    private static IEnumerator LoadLive2dProc(List<CardCard> cardList)
    {      
		if(rootObj != null){
			DisposeLive2dModels();
		}
		yield return null;
		rootObj = new GameObject("Live2dRoot");
		GameObject.DontDestroyOnLoad(rootObj);
		GachaObjectList = new List<KeyValuePair<int, WeakReference>>();
        foreach (var card in cardList) {
            var loader = new UnitResourceLoader(card.id);
            loader.LoadFlagReset();
            loader.IsLoadLive2DModel = true;
            loader.IsLoadVoiceFile = false;
            loader.LoadResource(resouce => {
				var go = GameObject.Instantiate(loader.Live2DModel) as GameObject;
				go.SetActive(false);
				rootObj.AddInChild(go);
				GachaObjectList.Add(new KeyValuePair<int, WeakReference>(card.id, new WeakReference(go)));
			});
        }

		while (GachaObjectList.Count < cardList.Count) {
            yield return null;
        }

		IsDownloadEndForGacha = true;
    }
	private static GameObject rootObj;

    public static void DisposeLive2dModels()
	{
		if(rootObj == null){
			return;
		}
		rootObj.DestroyChildren();
		GameObject.Destroy(rootObj);
	}

    /// <summary>
    /// 宴シナリオロード.
    /// </summary>
    public static void LoadUtageChapters()
	{      
		IsDownloadEndUtage = false;
		var chapters = AwsModule.ProgressData.TutorialStageNum <= 3 ? ACT_LIST_FIRST : ACT_LIST_LATER;
		// 初回起動でDLを悟られないようにプログレス表示なし.
        View_FadePanel.SharedInstance.DeativeProgress();      
        UtageModule.SharedInstance.LoadUseChapter("Tutorial", () => {
            Debug.Log("[TutorialSController] Utage Load Success.");
			IsDownloadEndUtage = true;         
		}, false, chapters);
		UtageModule.SharedInstance.SetActiveCore(false);
		UtageModule.SharedInstance.SetCoreDontDestroy();
	}

	private static readonly string[] ACT_LIST_FIRST = { "tuto_1", "tuto_2", "tuto_3" };
	private static readonly string[] ACT_LIST_LATER = { "tuto_1", "tuto_4", "tuto_5", "tuto_5_1", "tuto_5_2", "tuto_5_3", "tuto_5_4", "tuto_6" };   // tuto1は設定ファイルが入っているの必須.
    private static bool IsStartLoad = false;
}
