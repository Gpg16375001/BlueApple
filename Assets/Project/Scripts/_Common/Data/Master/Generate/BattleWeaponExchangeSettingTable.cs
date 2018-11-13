/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleWeaponExchangeSettingTable : ScriptableObject
{
    [SerializeField]
    private List<BattleWeaponExchangeSetting> _dataList;

    public List<BattleWeaponExchangeSetting> DataList {
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
public partial class BattleWeaponExchangeSetting
{
    // None
    [SerializeField]
    public int motion_type;

    // Spineモデルのスロット名
    [SerializeField]
    public string slot_name;

    // スロット内のアタッチメント名
    [SerializeField]
    public string attachment_name;

    // 武器アトラスregion名
    [SerializeField]
    public string atlas_region_name;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
