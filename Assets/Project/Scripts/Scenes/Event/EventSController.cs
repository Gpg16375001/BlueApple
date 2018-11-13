using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

using SmileLab;

public class EventSController : ScreenControllerBase {

    private SpriteAtlas eventQuestTop;
    private SpriteAtlas eventQuestBanner;
    public override void Init (System.Action<bool> didConnectEnd)
    {
        // eventquest用の画像データを取得
        DLCManager.AssetBundleFromFileOrDownload (DLCManager.DLC_FOLDER.UI, "bundle_event", 
            (assetBundleRef) => {
                StartCoroutine(LoadAtlas(assetBundleRef, didConnectEnd));
            },
            (ex) => {
                // TODO: api発行してクエスト情報を取得する
                didConnectEnd(true);
            }
        );
    }

    private IEnumerator LoadAtlas(AssetBundleRef assetBundleRef, System.Action<bool> didConnectEnd)
    {
        var loadAsync = assetBundleRef.assetbundle.LoadAllAssetsAsync<SpriteAtlas>();

        yield return loadAsync;

        eventQuestBanner = loadAsync.allAssets.FirstOrDefault(x => x.name == "EventBanner") as SpriteAtlas;
        eventQuestTop = loadAsync.allAssets.FirstOrDefault(x => x.name == "EventTop") as SpriteAtlas;

        assetBundleRef.Unload(false);

        // TODO: api発行してクエスト情報を取得する
        didConnectEnd(true);
    }

    public override void CreateBootScreen ()
    {
        SoundManager.SharedInstance.PlayBGM (SoundClipName.bgm009);
        var go = GameObjectEx.LoadAndCreateObject("Event/Screen_Event", gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var behaviour = go.GetOrAddComponent<Screen_Event> ();
        behaviour.Init (eventQuestTop, eventQuestBanner);
    }
}
