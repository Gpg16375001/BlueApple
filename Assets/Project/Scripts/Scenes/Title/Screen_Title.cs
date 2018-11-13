using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

using TMPro;

/// <summary>
/// Screen : タイトル画面.
/// </summary>
public class Screen_Title : ViewBase
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {
        // 親の付け直しをするとAnchorがおかしくなるようなので設定をし直す。
        // 基本的に全て親と同じになるようにする。
        var rectTransfrom = GetComponent<RectTransform>();
        if (rectTransfrom != null) {
            rectTransfrom.anchorMax = Vector2.one;
            rectTransfrom.anchorMin = Vector2.zero;

            rectTransfrom.anchoredPosition = Vector2.zero;
            rectTransfrom.offsetMax = Vector2.zero;
            rectTransfrom.offsetMin = Vector2.zero;
        }

        // テキストの設定
        GetScript<TextMeshProUGUI>("txtp_playerid").SetText(string.Format("プレイヤーID:{0}", AwsModule.UserData.CustomerID));
        GetScript<TextMeshProUGUI>("txtp_appversion").SetText(string.Format("Ver {0}", Application.version));

        // ボタン設定.
		if(AwsModule.ProgressData.TutorialStageNum >= 0){
			this.SetCanvasButtonMsg("BG", DidTapStartForTutorial);
		}else{
			this.SetCanvasButtonMsg("BG", DidTapStart);
		}
        this.SetCanvasCustomButtonMsg("bt_interrogation", DidTapInterrogation);
        this.SetCanvasCustomButtonMsg("bt_cacheclear", DidTapClearCache);
        this.SetCanvasCustomButtonMsg("bt_migrate", DidTapMigrate);
		this.GetScript<CustomButton>("bt_cacheclear").gameObject.SetActive(AwsModule.ProgressData.TutorialStageNum < 0);

        // フェードを開ける.
        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
    }

    #region ButtonDelegate.

    // ボタン：スタート
    void DidTapStart()
    {
        this.IsEnableButton = false;
		if(m_bPushButton){
			return;
		}
		m_bPushButton = true;
		View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black,
            () => {
                SendAPI.HomeLogin(
                    (success, res) => {
                        if(res == null || !success) {
                            Debug.LogError("SendUsersGetUserData Error");
                            return; // エラー.
                        }               
                        AwsModule.UserData.UserData = res.UserData;
                        res.CardDataList.CacheSet();
                        res.FormationDataList.CacheSet();
						res.WeaponDataList.CacheSet();
                        res.MagikiteDataList.CacheSet();
                        res.ConsumerDataList.CacheSet();
                        res.MaterialDataList.CacheSet();

                        if(AwsModule.UserData.Modify()) {
                            AwsModule.UserData.Sync(null);
                        }
                        if(AwsModule.ProgressData.Modify()) {
                            AwsModule.ProgressData.Sync();
                        }

                        if(!UpdateOrAddDownload()) {
                            ScreenChange();
                        }
                    }
                );         
            });
    }

    private bool UpdateOrAddDownload()
    {
        // 更新・追加アセットのダウンロード
        var dlSize = DLCManager.DownloadMinimumContentsFilesSize();
        var dlMaxSize = DLCManager.DownloadAllFilesSize();
        var dlcType = AwsModule.ProgressData.SelectDLCType;
        if(dlSize > 0 || dlMaxSize > 0) {
            bool allFlag = false;
            long size = 0;
            if(dlcType == 2) {
                allFlag = true;
                size = dlMaxSize;
            } else if(dlSize > 0) {
                allFlag = false;
                size = dlSize;
            }

            // 1MB以上は確認をだす
            if(size > 1024 * 1024) {
                // TODO: 確認Popup
                double dlSizeMB = (double)size / 1024D / 1024D;
                PopupManager.OpenPopupSystemOK(string.Format("{0:F2}MBのダウンロードを開始します", dlSizeMB),
                    () => StartDownload(allFlag));
                return true;
            } else if(size > 0) {
                if(allFlag) {
                    DLCManager.DownloadAllFiles (
                        (ret) => {
                            View_FadePanel.SharedInstance.DeativeProgress();
                            ScreenChange();
                        },
                        null,
                        (progress, done, max) => {
                            View_FadePanel.SharedInstance.SetProgress(progress);
                        }
                    );
                } else {
                    DLCManager.DownloadMinimumContentsFiles (
                        (ret) => {
                            View_FadePanel.SharedInstance.DeativeProgress();
                            ScreenChange();
                        },
                        null,
                        (progress, done, max) => {
                            View_FadePanel.SharedInstance.SetProgress(progress);
                        }
                    );
                }
                return true;
            }
        }
        return false;
    }

    void StartDownload(bool all)
    {
        var dlView = View_DL.CreateIfMissing();
        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black,
            () => {
                if (all) {
                    DLCManager.DownloadAllFiles ((ret) => {
                        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                            View_DL.DisposeProc ();
                            ScreenChange();
                        });
                    }, null, dlView.SetProgress);
                } else {
                    DLCManager.DownloadMinimumContentsFiles ((ret) => {
                        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                            View_DL.DisposeProc ();
                            ScreenChange();
                        });
                    }, null, dlView.SetProgress);
                }
            }
        );
    }

    private void ScreenChange()
    {
        if(AwsModule.BattleData.IsProgressing) {
            // クエストの解放状態を確認
            SendAPI.QuestsGetOpenBattle((result, response) => {
                if(!result || response == null) {
                    return;
                }
                if(response.BattleEntryData != null && response.QuestId > 0) {
                    // バトル中の場合は復帰確認を促す
                    if(response.BattleEntryData.Status == 1) {
                        PopupManager.OpenPopupSystemYN("中断データがあります。復帰しますか？",
                            () => {
                                if(response.QuestType == 1) {
        							SendAPI.QuestsGetMainAchievementByCountry((int)MasterDataTable.quest_main[response.QuestId].Country.Enum, (achiveResult, achiveResponse) => {

                                        if(!achiveResult || achiveResponse == null) {
                                            return;
                                        }

    									achiveResponse.QuestAchievementList.CacheSet();
        								AwsModule.BattleData.QuestID = response.QuestId;
                                        AwsModule.BattleData.QuestType = response.QuestType;
                                        AwsModule.BattleData.BattleEntryData = response.BattleEntryData;
                                        ScreenChanger.SharedInstance.GoToBattleReversion();
        							});
                                } else {
                                    AwsModule.BattleData.QuestID = response.QuestId;
                                    AwsModule.BattleData.QuestType = response.QuestType;
                                    AwsModule.BattleData.BattleEntryData = response.BattleEntryData;
                                    ScreenChanger.SharedInstance.GoToBattleReversion();
                                }
                            },
                            () => {
                                SendAPI.QuestsCloseQuest(response.QuestId, false, response.BattleEntryData.EntryId, new int[0], new int[0],
                                    (closeResult, closeResponse) => {
                                        if(!closeResult || closeResponse == null) {
                                            return;
                                        }
										closeResponse.QuestAchievement.CacheSet();
                                        ScreenChanger.SharedInstance.GoToMyPage();
                                        AwsModule.BattleData.CloseBattle();
                                    }
                                );

                            }
                        );
                    } else {
                        ScreenChanger.SharedInstance.GoToMyPage();
                        AwsModule.BattleData.CloseBattle();
                    }
                } else {
                    ScreenChanger.SharedInstance.GoToMyPage();
                }
            });
        } else {
            ScreenChanger.SharedInstance.GoToMyPage();
        }
    }

    // チュートリアルがあればゲームスタートせずこちら.
	void DidTapStartForTutorial()
	{
		this.IsEnableButton = false;      
		if (m_bPushButton) {
            return;
        }
        m_bPushButton = true;
		this.StartCoroutine(WaitResourceLoading());
	}
	IEnumerator WaitResourceLoading()
	{
		var bFadeing = true;
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => bFadeing = false);
		while (bFadeing || !TutorialResourceDownloader.IsDownloadEndForInit) {
            yield return null;
        }      
		ScreenChanger.SharedInstance.GoToTutorial();
	}

    // ボタン：お問い合わせ
    void DidTapInterrogation()
    {
#if UNITY_IOS && !UNITY_EDITOR
        SafariView.Init (gameObject);
        SafariView.LaunchURL (ContactUs.CreateUrl());
#else
        Application.OpenURL(ContactUs.CreateUrl());
#endif
    }

    // ボタン：キャッシュクリア
    void DidTapClearCache()
	{
        PopupManager.OpenPopupSystemYN ("キャッシュクリアを\n実行しますか？",
            () => {
                GameSystem.ClearChache(GameSystem.DownloadDirectoryPath);   // ダウンロード周りのキャッシュは削除する.          
                Caching.ClearCache();
                PopupManager.OpenPopupOK ("キャッシュクリアを実行しました。再起動します。", () => {
                    ScreenChanger.SharedInstance.Reboot();
                });
            }
        );
    }

    // ボタン：データ引き継ぎ
    void DidTapMigrate()
    {
        View_PopupInheriting.Create();
    }

    #endregion

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if(canvas != null){
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private bool m_bPushButton = false;
}
