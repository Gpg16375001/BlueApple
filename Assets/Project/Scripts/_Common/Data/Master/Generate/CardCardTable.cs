/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardCardTable : ScriptableObject
{
    [SerializeField]
    private List<CardCard> _dataList;

    public List<CardCard> DataList {
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
        _dataDict = _dataList.ToDictionary (x => new UniRx.Tuple<int, int>(x.id, x.rarity));
        InitExtension();
    }

    private Dictionary<UniRx.Tuple<int, int>, CardCard> _dataDict = null;
    public CardCard this[int id, int rarity]
    {
        get {
            CardCard ret;
            var key = new UniRx.Tuple<int, int>(id, rarity);
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardCard
{
    // カードID
    [SerializeField]
    public int id;

    [SerializeField]
    private int _character;
    // キャラクター
    public CardCharacter character;

    // カードとしての呼び名。表示名として使用。
    [SerializeField]
    public string nickname;

    // 装飾に使う二つ名
    [SerializeField]
    public string alias;

    [SerializeField]
    private BelongingEnum _country;
    // 所属している国
    public Belonging country;

    // 隊長とかそういった役職名
    [SerializeField]
    public string post;

    // レアリティ
    [SerializeField]
    public int rarity;

    // ロール指定
    [SerializeField]
    public CardRoleEnum role;

    [SerializeField]
    private ElementEnum _element;
    // 属性
    public Element element;

    // 武器ユニークID
    [SerializeField]
    public int initial_weapon_id;

    // テーブルID
    [SerializeField]
    public int level_table_id;

    // 初期HP
    [SerializeField]
    public int initial_hp;

    // HP成長タイプ
    [SerializeField]
    public GrowthTypeEnum hp_growth_type;

    // Lv100時のHP最大値
    [SerializeField]
    public int max_hp;

    // 初期攻撃力
    [SerializeField]
    public int initial_attack;

    // 攻撃成長タイプ
    [SerializeField]
    public GrowthTypeEnum attack_growth_type;

    // Lv100時の攻撃力
    [SerializeField]
    public int max_attack;

    // 初期防御力
    [SerializeField]
    public int initial_defence;

    // 防御力成長タイプ
    [SerializeField]
    public GrowthTypeEnum defence_growth_type;

    // Lv100時の防御力
    [SerializeField]
    public int max_defence;

    // 初期素早さ
    [SerializeField]
    public int initial_agility;

    // 素早さ成長タイプ
    [SerializeField]
    public GrowthTypeEnum agility_growth_type;

    // Lv100時の素早さ
    [SerializeField]
    public int max_agility;

    // 運
    [SerializeField]
    public int luck;

    // フレーバーテキスト
    [SerializeField]
    public string flavor_text;

    // フレーバーテキスト2
    [SerializeField]
    public string flavor_text2;

    [SerializeField]
    private int _release_chapter_flavor2;
    // フレーバーテキスト2の条件となる解放章情報
    public MainQuestChapterInfo release_chapter_flavor2;

    // ボイスファイル指定
    [SerializeField]
    public string voice_sheet_name;

    // 好きなものの名前
    [SerializeField]
    public string like;

    // 嫌いなものの名前
    [SerializeField]
    public string dislike;

    // 趣味
    [SerializeField]
    public string hobby;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        character = MasterDataTable.character.DataList.First (x => x.id == _character);
        country = MasterDataTable.belonging.DataList.First (x => x.Enum == _country);
        element = MasterDataTable.element.DataList.First (x => x.Enum == _element);
        release_chapter_flavor2 = MasterDataTable.quest_main_chapter_info.DataList.First (x => x.id == _release_chapter_flavor2);
        InitExtension();
    }
}
