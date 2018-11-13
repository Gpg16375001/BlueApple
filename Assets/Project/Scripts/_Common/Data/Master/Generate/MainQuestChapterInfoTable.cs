/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestChapterInfoTable : ScriptableObject
{
    [SerializeField]
    private List<MainQuestChapterInfo> _dataList;

    public List<MainQuestChapterInfo> DataList {
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

    private Dictionary<int, MainQuestChapterInfo> _dataDict = null;
    public MainQuestChapterInfo this[int key]
    {
        get {
            MainQuestChapterInfo ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MainQuestChapterInfo
{
    // クエスト章ID
    [SerializeField]
    public int id;

    [SerializeField]
    private string _country;
    // 国名
    public Belonging country;

    // 章番号
    [SerializeField]
    public int chapter;

    // 章の名前
    [SerializeField]
    public string chapter_name;

    // 章選択画面で表示する心情テキスト
    [SerializeField]
    public string feeling_text;

    // 章選択画面で表示するキャラクターのカードID。設定しない場合は0。
    [SerializeField]
    public int feeling_unit_id;

    [SerializeField]
    private int _reward_item_type;
    // None
    public ItemType reward_item_type;

    // 報酬アイテムID。使う必要がなければ0。
    [SerializeField]
    public int reward_item_id;

    // 報酬アイテムのもらえる個数
    [SerializeField]
    public int reward_item_count;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        country = MasterDataTable.belonging.DataList.First (x => x.name == _country);
        reward_item_type = MasterDataTable.item_type.DataList.First (x => x.index == _reward_item_type);
        InitExtension();
    }
}
