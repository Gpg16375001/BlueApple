/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardCanEquippedSettingTable : ScriptableObject
{
    [SerializeField]
    private List<CardCanEquippedSetting> _dataList;

    public List<CardCanEquippedSetting> DataList {
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
        _dataDict_card_id_array = _dataList.GroupBy (x => x.card_id).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<int, CardCanEquippedSetting[]> _dataDict_card_id_array = null;
    public CardCanEquippedSetting[] this[int key]
    {
        get {
            CardCanEquippedSetting[] ret;
            _dataDict_card_id_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardCanEquippedSetting
{
    // カードID
    [SerializeField]
    public int card_id;

    // None
    [SerializeField]
    public int weapon_type;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
