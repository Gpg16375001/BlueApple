/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardAcquireSkillTable : ScriptableObject
{
    [SerializeField]
    private List<CardAcquireSkill> _dataList;

    public List<CardAcquireSkill> DataList {
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

    private Dictionary<int, CardAcquireSkill[]> _dataDict_card_id_array = null;
    public CardAcquireSkill[] this[int key]
    {
        get {
            CardAcquireSkill[] ret;
            _dataDict_card_id_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardAcquireSkill
{
    // カードID
    [SerializeField]
    public int card_id;

    // スキル識別ID
    [SerializeField]
    public int skill_id;

    // 習得レベル
    [SerializeField]
    public int acquire_level;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
