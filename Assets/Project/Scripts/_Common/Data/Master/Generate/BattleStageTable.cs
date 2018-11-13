/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleStageTable : ScriptableObject
{
    [SerializeField]
    private List<BattleStage> _dataList;

    public List<BattleStage> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.id);
        InitExtension();
    }

    private Dictionary<int, BattleStage> _dataDict = null;
    public BattleStage this[int key]
    {
        get {
            BattleStage ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleStage
{
    // カードID
    [SerializeField]
    public int id;

    // ステージ名称
    [SerializeField]
    public string name;

    // 再生BGM指定
    [SerializeField]
    public string bgm_clip_name;

    // None
    [SerializeField]
    public int background_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
