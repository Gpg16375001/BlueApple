/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardDetailVoiceSettingTable : ScriptableObject
{
    [SerializeField]
    private List<CardDetailVoiceSetting> _dataList;

    public List<CardDetailVoiceSetting> DataList {
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

        InitExtension();
    }


}

[Serializable]
public partial class CardDetailVoiceSetting
{
    // None
    [SerializeField]
    public SoundVoiceCueEnum voice_cue_id;

    // 表示名
    [SerializeField]
    public string display_name;

    // None
    [SerializeField]
    public CardDetailVoiceCategoryEnum category;

    // None
    [SerializeField]
    public CardDetailVoiceReleaseEnum condition;

    // 条件値
    [SerializeField]
    public int condition_value;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
