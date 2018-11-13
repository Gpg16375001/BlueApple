/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CriSoundClipTable : ScriptableObject
{
    [SerializeField]
    private List<CriSoundClip> _dataList;

    public List<CriSoundClip> DataList {
        get {
            return _dataList;
        }
    }

    void OnEnable()
    {
        Init ();
    }

    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    void Init()
    {
        for (int i = 0; i < _dataList.Count; ++i) {
            _dataList [i].Init ();
        }
        _dataDict = _dataList.ToDictionary (x => x.CueName);
        InitExtension();
    }

    private Dictionary<string, CriSoundClip> _dataDict = null;
    public CriSoundClip this[string key]
    {
        get {
            CriSoundClip ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CriSoundClip
{
    // キュー名定義
    [SerializeField]
    public string CueName;

    [SerializeField]
    private string _CueSheetInfo;
    // キューシート指定
    public CriCueSheet CueSheetInfo;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        CueSheetInfo = MasterDataTable.cri_sound_cue.DataList.First (x => x.CueSheetName == _CueSheetInfo);
        InitExtension();
    }
}
