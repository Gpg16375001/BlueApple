/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillPerformanceSettingTable : ScriptableObject
{
    [SerializeField]
    private List<SkillPerformanceSetting> _dataList;

    public List<SkillPerformanceSetting> DataList {
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
        _dataDict_performance_pattern_array = _dataList.GroupBy (x => x.performance_pattern).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<int, SkillPerformanceSetting[]> _dataDict_performance_pattern_array = null;
    public SkillPerformanceSetting[] this[int key]
    {
        get {
            SkillPerformanceSetting[] ret;
            _dataDict_performance_pattern_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SkillPerformanceSetting
{
    // 演出パターン
    [SerializeField]
    public int performance_pattern;

    [SerializeField]
    private bool motion_type_has_value;
    [SerializeField]
    private int motion_type_value;
    // None
    public int? motion_type;

    // 再生キー
    [SerializeField]
    public string plaing_key;

    // None
    [SerializeField]
    public SkillPerformanceTargetEnum performance_target;

    // 演出Prefab名
    [SerializeField]
    public string prefab_name;

    // 発生ボーン
    [SerializeField]
    public string bone_name;

    // 追従するか
    [SerializeField]
    public bool is_follower;

    // FollowZPosition
    [SerializeField]
    public bool is_follower_z;

    // FollowBoneRotation
    [SerializeField]
    public bool is_follower_rotation;

    // FollowSkeletonFlip
    [SerializeField]
    public bool is_follower_flip;

    // FollowLocalScale
    [SerializeField]
    public bool is_follower_local_scale;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        motion_type = null;
        if(motion_type_has_value) {
            motion_type = motion_type_value;
        }
        InitExtension();
    }
}
