/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class LoginbonusDistItemTable : ScriptableObject
{
    [SerializeField]
    private List<LoginbonusDistItem> _dataList;

    public List<LoginbonusDistItem> DataList {
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
public partial class LoginbonusDistItem
{
    // ログインボーナスID
    [SerializeField]
    public int loginbonus_id;

    // ログイン日数
    [SerializeField]
    public int day_count;

    // 表示名
    [SerializeField]
    public string display_title;

    // アイテムタイプ
    [SerializeField]
    public ItemTypeEnum item_type;

    // アイテムID
    [SerializeField]
    public int item_id;

    // 個数
    [SerializeField]
    public int quantity;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
