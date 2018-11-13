/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GemStoreSettingTable : ScriptableObject
{
    [SerializeField]
    private List<GemStoreSetting> _dataList;

    public List<GemStoreSetting> DataList {
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
public partial class GemStoreSetting
{
    // ストア商品ID
    [SerializeField]
    public string store_product_id;

    // 購入可能プラットフォーム
    [SerializeField]
    public PlatformEnum platform;

    // 購入可能ストアタイプ
    [SerializeField]
    public StoreTypeEnum store_type;

    // アプリ商品ID
    [SerializeField]
    public int app_product_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
