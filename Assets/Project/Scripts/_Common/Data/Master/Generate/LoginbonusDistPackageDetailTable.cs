/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class LoginbonusDistPackageDetailTable : ScriptableObject
{
    [SerializeField]
    private List<LoginbonusDistPackageDetail> _dataList;

    public List<LoginbonusDistPackageDetail> DataList {
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
public partial class LoginbonusDistPackageDetail
{
    // 配布パッケージID
    [SerializeField]
    public int dist_package_id;

    // パッケージ内のインデックス
    [SerializeField]
    public int package_index;

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
