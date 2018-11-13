/// <summary>
/// クエストデータ共通インターフェイス.
/// </summary>
public interface IQuestData
{
    /// <summary>
    /// クエストのタイプ
    /// 1: メインクエスト
    /// 2: サブクエスト
    /// 3: ユニットクエスト
    /// 4: 強化クエスト
    /// 5: 進化クエスト
    /// </summary>
    int QuestType { get; }

    /// <summary>
    /// クエストID.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// バトルがある場合のバトルステージID.
    /// </summary>
    int BattleStageID { get; }

    /// <summary>
	/// 国情報.
    /// </summary>
	Belonging Country { get; }

    /// <summary>
    /// 章番号.
    /// </summary>
    int ChapterNum { get; }

    /// <summary>
    /// ステージ or クエストIndex番号.
    /// </summary>
    int StageNum { get; }

    /// <summary>
    /// クエスト番号.
    /// </summary>
    int QuestNum { get;  }

    /// <summary>
    /// クエスト名.
    /// </summary>
	string QuestName { get; }

    /// <summary>
    /// 開始に必要なAP.
    /// </summary>
	int NeedAP { get; }
}
