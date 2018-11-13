/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleUnitModelSettingTable : ScriptableObject
{
    [SerializeField]
    private List<BattleUnitModelSetting> _dataList;

    public List<BattleUnitModelSetting> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.motion_type);
        InitExtension();
    }

    private Dictionary<int, BattleUnitModelSetting> _dataDict = null;
    public BattleUnitModelSetting this[int key]
    {
        get {
            BattleUnitModelSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleUnitModelSetting
{
    // None
    [SerializeField]
    public int motion_type;

    // スキン名
    [SerializeField]
    public string skin_name;

    // アニメーションコントローラー名指定
    [SerializeField]
    public string animator_controller_name;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
