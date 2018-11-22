using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class EventQuestSController : ScreenControllerBase {

    public int EventId;
    public EventQuestStageTypeEnum? StageType;

    private Dictionary<string, GameObject> assetBundleObjects;
    private EventQuestAchievement[] questAchievements;
    private EventShopProductData[] productDatas;
    private int EventPoint;

    public override void Init (System.Action<bool> didConnectEnd)
    {

        SendAPI.QuestsGetEventQuestAchievement (EventId,
            (success, response) => {
                if(!success) {
                    didConnectEnd(false);
                    return;
                }

                questAchievements = response.EventQuestAchievementList;
                SendAPI.EventGetProductList (EventId,
                    (success2, response2) => {
                        if(!success2) {
                            didConnectEnd(false);
                            return;
                        }

                        productDatas = response2.EventShopProductDataList;
                        EventPoint = response2.EventPoint;

                        // ユーザーデータを更新しておく
                        AwsModule.UserData.UserData = response2.UserData;

                        View_FadePanel.SharedInstance.SetProgress(0);
                        // イベント用データのダウンロード
                        DLCManager.AssetBundleFromFileOrDownload (DLCManager.DLC_FOLDER.EventQuest, string.Format ("eventquest_{0}", EventId),
                            (assetBundleRef) => {
                                // 全部読み込みする。
                                StartCoroutine (LoadAssetBundles (assetBundleRef, didConnectEnd));
                                View_FadePanel.SharedInstance.DeativeProgress();
                            },
                            (ex) => {
                                didConnectEnd (true);
                                View_FadePanel.SharedInstance.DeativeProgress();
                            },
                            (f) => {
                                View_FadePanel.SharedInstance.SetProgress(f);
                            }
                        );
                    }
                );
            }
        );
    }

    private IEnumerator LoadAssetBundles(AssetBundleRef assetBundleRef, System.Action<bool> didConnectEnd)
    {
        var asynObj = assetBundleRef.assetbundle.LoadAllAssetsAsync<GameObject> ();

        yield return asynObj;

        assetBundleObjects = new Dictionary<string, GameObject> ();
        foreach (var asset in asynObj.allAssets) {
            var prefab = asset as GameObject;
            if (prefab != null) {
                assetBundleObjects.Add (prefab.name, prefab);
            }
        }

        assetBundleRef.Unload (false);
        didConnectEnd (true);
    }

    public override void CreateBootScreen ()
    {
        SoundManager.SharedInstance.PlayBGM (SoundClipName.bgm009);
        var go = GameObjectEx.LoadAndCreateObject("EventQuest/Screen_EventQuest", gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var behaviour = go.GetOrAddComponent<Screen_EventQuest> ();
        behaviour.Init (EventId, StageType, EventPoint, questAchievements, productDatas, assetBundleObjects);
    }
}
