﻿/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class WeaponMotionTypeTable : ScriptableObject
{
    [SerializeField]
    private List<WeaponMotionType> _dataList;

    public List<WeaponMotionType> DataList {
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
public partial class WeaponMotionType
{
    // Index
    [SerializeField]
    public int index;

    // モーション種別
    [SerializeField]
    public string name;

    // None
    [SerializeField]
    public AttackRangeEnum attack_range;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}