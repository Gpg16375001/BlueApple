/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class LoginbonusDistPackageTable : ScriptableObject
{
    [SerializeField]
    private List<LoginbonusDistPackage> _dataList;

    public List<LoginbonusDistPackage> DataList {
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
public partial class LoginbonusDistPackage
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

    // 配布するパッケージID
    [SerializeField]
    public int package_id;

    [SerializeField]
    private bool package_icon_id_has_value;
    [SerializeField]
    private int package_icon_id_value;
    // 特別なアイコンIDが指定されていればロードするnullの場合はパッケージ内の1つ目
    public int? package_icon_id;

    // カードIDに基づいてLive2Dモデルをロードするデフォルトは司書
    [SerializeField]
    public int disp_card_id;

    // 再生するボイスキューID
    [SerializeField]
    public int voice_cue_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        package_icon_id = null;
        if(package_icon_id_has_value) {
            package_icon_id = package_icon_id_value;
        }
        InitExtension();
    }
}
