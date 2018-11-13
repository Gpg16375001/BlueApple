/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MissionSettingTable : ScriptableObject
{
    [SerializeField]
    private List<MissionSetting> _dataList;

    public List<MissionSetting> DataList {
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

    private Dictionary<int, MissionSetting> _dataDict = null;
    public MissionSetting this[int key]
    {
        get {
            MissionSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MissionSetting
{
    // ミッションセットID
    [SerializeField]
    public int id;

    [SerializeField]
    private int _type;
    // ミッションセットの種類
    public MissionType type;

    // ミッション情報を取得するための引数1
    [SerializeField]
    public int arg1;

    // ミッション情報を取得するための引数2
    [SerializeField]
    public int arg2;

    // ミッション情報を取得するための引数3
    [SerializeField]
    public int arg3;

    [SerializeField]
    private int _category;
    // ミッションカテゴリー
    public MissionCategory category;

    // 今すぐいく選択時に遷移するシーン名
    [SerializeField]
    public string jump_scene_name;

    [SerializeField]
    private int _reward_item_type;
    // 報酬になるアイテムタイプ
    public ItemType reward_item_type;

    // 報酬となるアイテムのID.カテゴリー指定で事足りるアイテムの場合は使わない.
    [SerializeField]
    public int reward_item_id;

    // 報酬として受け取れる個数
    [SerializeField]
    public int reward_item_count;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        type = MasterDataTable.mission_type.DataList.First (x => x.index == _type);
        category = MasterDataTable.mission_category.DataList.First (x => x.index == _category);
        reward_item_type = MasterDataTable.item_type.DataList.First (x => x.index == _reward_item_type);
        InitExtension();
    }
}
