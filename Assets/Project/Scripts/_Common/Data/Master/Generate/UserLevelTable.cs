/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class UserLevelTable : ScriptableObject
{
    [SerializeField]
    private List<UserLevel> _dataList;

    public List<UserLevel> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.level);
        InitExtension();
    }

    private Dictionary<int, UserLevel> _dataDict = null;
    public UserLevel this[int key]
    {
        get {
            UserLevel ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class UserLevel
{
    // ユーザーレベル
    [SerializeField]
    public int level;

    // レベルになるのに必要な経験値
    [SerializeField]
    public int required_experience;

    // AP最大値
    [SerializeField]
    public int ap_max;

    // フォロー最大値
    [SerializeField]
    public int follow_max;

    // フォロワー最大値
    [SerializeField]
    public int follower_max;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
