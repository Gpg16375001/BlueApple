using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI背景を自動でロードして設定する
/// このコンポーネントが付いているオオブジェクトはビルド時にspriteを剥がされBackGroundImageNameにsprite名が登録される。
/// </summary>
[RequireComponent(typeof(Image))]
[DisallowMultipleComponent()]
public class ScreenBackground : MonoBehaviour
{
    [SerializeField]
    private string BackGroundImageName;

    private Image BackGroundImage;

    private bool isLoaded = false;

    void Awake()
    {
        BackGroundImage = GetComponent<Image> ();
    }

    void Start()
    {
        Load ();
    }

    public void Load()
    {
        if (string.IsNullOrEmpty (BackGroundImageName)) {
            isLoaded = true;
            if (m_LoadEnd != null) {
                m_LoadEnd ();
            }
            return;
        }

        if (!Application.isEditor && Application.isPlaying) {
            if (this.gameObject.name == "BG" && m_NowBgSprite != null && m_NowBgSprite.name == BackGroundImageName) {
                // 名前がBGの時に二枚ほどキャッシュとっておきある場合はそちらから使用する。
                if (BackGroundImage != null) {
                    BackGroundImage.sprite = m_NowBgSprite;
                }
                isLoaded = true;
                if (m_LoadEnd != null) {
                    m_LoadEnd ();
                }
                return;
            }
            // 背景画像の呼び出し。
            DLCManager.AssetBundleFromFileOrDownload<Sprite> (DLCManager.DLC_FOLDER.ScreenBG, "screenbg", BackGroundImageName,
                (spt) => {
                    if(gameObject != null) {
                        if (this.gameObject.name == "BG") {
                            m_NowBgSprite = spt;
                        }
                        if(BackGroundImage != null) {
                            BackGroundImage.sprite = spt;
                        }
                    }
                    isLoaded = true;
                    if (m_LoadEnd != null) {
                        m_LoadEnd ();
                    }
                },
                (ex) => {
                    // 無視
                    isLoaded = true;
                    if (m_LoadEnd != null) {
                        m_LoadEnd ();
                    }
                }
            );
        } else {
            isLoaded = true;
            if (m_LoadEnd != null) {
                m_LoadEnd ();
            }
        }
    }

    public void CallbackLoaded(System.Action loadEnd)
    {
        m_LoadEnd = null;
        if (isLoaded) {
            loadEnd ();
            return;
        }

        m_LoadEnd = loadEnd;
    }

    private System.Action m_LoadEnd;
    static Sprite m_NowBgSprite;

#if UNITY_EDITOR
    public void BuildProcess()
    {
        if (BackGroundImage == null) {
            BackGroundImage = GetComponent<Image> ();
        }
        if (BackGroundImage != null && BackGroundImage.sprite != null) {
            BackGroundImageName = BackGroundImage.sprite.name;
            BackGroundImage.sprite = null;
        }
    }
#endif
}
