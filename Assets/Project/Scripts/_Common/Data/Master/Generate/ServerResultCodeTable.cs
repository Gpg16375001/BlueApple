/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ServerResultCodeTable : ScriptableObject
{
    [SerializeField]
    private List<ServerResultCode> _dataList;

    public List<ServerResultCode> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.Enum);
        InitExtension();
    }

    private Dictionary<ServerResultCodeEnum, ServerResultCode> _dataDict = null;
    public ServerResultCode this[ServerResultCodeEnum key]
    {
        get {
            ServerResultCode ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ServerResultCode
{
    // ID
    [SerializeField]
    public int index;

    // None
    [SerializeField]
    public ServerResultCodeEnum Enum;

    // None
    [SerializeField]
    public ServerResultCodeProcEnum proc;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
