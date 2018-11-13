using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class AdditionalFontLoader : ViewBase
{
    [SerializeField]
    private List<TMP_FontAsset> m_FontList = new List<TMP_FontAsset>();

    private Dictionary<TMP_FontAsset, TMP_FontAsset> additionalFonts = new Dictionary<TMP_FontAsset, TMP_FontAsset>();
    /// <summary>
    /// 共通インスタンス
    /// </summary>
    public static AdditionalFontLoader SharedInstance { get; private set; }
    public bool IsReady { get; private set; }

    void Awake()
    {
        IsReady = false;
        if(SharedInstance != null) {
            SharedInstance.Dispose();
        }
        SharedInstance = this;
    }

    public override void Dispose ()
    {
        foreach(var kv in additionalFonts) {
            kv.Key.fallbackFontAssets.Remove(kv.Value);
        }
        additionalFonts.Clear ();
        IsReady = false;
        base.Dispose ();
    }

    public void Init()
    {
        DLCManager.AssetBundleFromFileOrDownload(DLCManager.DLC_FOLDER.Font, "bundle_font",
            (assetBundle) => {
                foreach(var font in m_FontList) {
                    if(assetBundle.assetbundle.Contains(font.name)) {
                        var additionalFont = assetBundle.assetbundle.LoadAsset<TMPro.TMP_FontAsset>(font.name);
                        if(!font.fallbackFontAssets.Contains(additionalFont)) {
                            font.fallbackFontAssets.Add(additionalFont);
                            additionalFonts.Add(font, additionalFont);
                        }
                    }
                }
                assetBundle.Unload(false);
                IsReady = true;
            }
        );

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        #endif
    }

    #if UNITY_EDITOR
    private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
            if (SharedInstance != null) {
                SharedInstance.Dispose ();
            }
        }
    }
    #endif
}
