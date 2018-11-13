/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestStageInfoTable : ScriptableObject
{
    [SerializeField]
    private List<MainQuestStageInfo> _dataList;

    public List<MainQuestStageInfo> DataList {
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

    private Dictionary<int, MainQuestStageInfo> _dataDict = null;
    public MainQuestStageInfo this[int key]
    {
        get {
            MainQuestStageInfo ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MainQuestStageInfo
{
    // クエストステージID
    [SerializeField]
    public int id;

    [SerializeField]
    private int _chapter_info;
    // メインクエスト章情報
    public MainQuestChapterInfo chapter_info;

    // ステージ名
    [SerializeField]
    public string stage_name;

    // ステージ番号
    [SerializeField]
    public int stage;

    // 章選択時デフォルトフォーカス基準となるステージかどうか.
    [SerializeField]
    public bool is_default_stage;

    // ステージ選択時デフォルトフォーカス基準となるクエスト番号.
    [SerializeField]
    public int default_quest;

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
        chapter_info = MasterDataTable.quest_main_chapter_info.DataList.First (x => x.id == _chapter_info);
        reward_item_type = MasterDataTable.item_type.DataList.First (x => x.index == _reward_item_type);
        InitExtension();
    }
}
