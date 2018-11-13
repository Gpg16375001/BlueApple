/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class UserPointRecoveryTimeTable : ScriptableObject
{
    [SerializeField]
    private List<UserPointRecoveryTime> _dataList;

    public List<UserPointRecoveryTime> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.point_type);
        InitExtension();
    }

    private Dictionary<UserPointTypeEnum, UserPointRecoveryTime> _dataDict = null;
    public UserPointRecoveryTime this[UserPointTypeEnum key]
    {
        get {
            UserPointRecoveryTime ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class UserPointRecoveryTime
{
    // ユーザーポイント種別
    [SerializeField]
    public UserPointTypeEnum point_type;

    // 1あたりの回復時間(秒)
    [SerializeField]
    public int recovery_time;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
