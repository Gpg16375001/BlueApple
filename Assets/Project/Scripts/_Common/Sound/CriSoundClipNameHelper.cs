using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SmileLab;

public static class CriSoundClipNameHelper
{
    /// <summary>
    /// CRIサウンド情報取得.
    /// </summary>
    public static CriSoundClip ToSoundClipInfo (this SoundClipName clip)
    {
        if (MasterDataTable.cri_sound_clip != null) {
            return MasterDataTable.cri_sound_clip [clip.ToString ()];
        }
        return null;
    }

    public static CriCueSheet GetCueSheetInfo (string cueSheetName)
    {
        return MasterDataTable.cri_sound_cue [cueSheetName];
    }
}


public partial class CriCueSheet
{
    public string AcbFilePath {
        get {
            if (isStreamingAssets) {
#if !UNITY_ANDROID || UNITY_EDITOR
                return Application.streamingAssetsPath + "/Sound/" + ACBFileName + ".acb";
#else
                return "Sound/" + ACBFileName + ".acb";
#endif
            }
            return DLCManager.GetDownloadPath(DLCManager.DLC_FOLDER.Sound,
                string.Format("{0}.acb", ACBFileName));
        }
    }

    public string AcbS3Path {
        get {
            if (isStreamingAssets) {
                return string.Empty;
            }
            return DLCManager.GetS3Path(DLCManager.DLC_FOLDER.Sound,
                string.Format("{0}.acb", ACBFileName));
        }
    }

    public string AwbFilePath {
        get {
            if (string.IsNullOrEmpty (AWBFileName)) {
                return string.Empty;
            }
            if (isStreamingAssets) {
#if !UNITY_ANDROID || UNITY_EDITOR
                return Application.streamingAssetsPath + "/Sound/" + AWBFileName + ".awb";
#else
                return "Sound/" + AWBFileName + ".awb";
#endif
            }
            return DLCManager.GetDownloadPath(DLCManager.DLC_FOLDER.Sound,
                string.Format("{0}.awb", AWBFileName));
        }
    }

    public string AwbS3Path {
        get {
            if (isStreamingAssets) {
                return string.Empty;
            }
            return DLCManager.GetS3Path(DLCManager.DLC_FOLDER.Sound,
                string.Format("{0}.awb", AWBFileName));
        }
    }
}