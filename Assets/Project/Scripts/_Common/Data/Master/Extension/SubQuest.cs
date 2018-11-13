/// <summary>
/// partial class : サブクエスト.
/// </summary>
public partial class SubQuest : IQuestData
{
    /// <summary>
    /// クエストのタイプ
    /// </summary>
    public int QuestType { get { return 2; } }

    /// <summary>
    /// from IQuestData interface : クエストID.
    /// </summary>
    public int ID { get { return this.id; } }

    /// <summary>
    /// from IQuestData interface : ステージID.
    /// </summary>
    public int BattleStageID { get { return this.stage_id; } }

	/// <summary>
    /// from IQuestData interface : 国情報.
    /// </summary>
    public Belonging Country
    {
        get {
            if (countryInfo == null) {
				countryInfo = MasterDataTable.belonging.DataList.Find(b => b.Enum == BelongingEnum.Unknown);
            }
            return countryInfo;
        }
    }
    private Belonging countryInfo;

    /// <summary>
    /// from IQuestData interface : 章番号.
    /// </summary>
    public int ChapterNum { get { return 0; } }

    /// <summary>
    /// from IQuestData interface : クエストインデックス.
    /// </summary>
    public int StageNum { get { return this.index; } }

    /// <summary>
    /// from IQuestData interface : シーン番号.
    /// </summary>
    public int QuestNum { get { return this.scene; } }

	/// <summary>
    /// from IQuestData interface : クエスト名として表示する名前.
    /// </summary>
	public string QuestName { get { return "クエスト"+this.scene; } }

    /// <summary>
    /// 必要AP.
    /// </summary>
	public int NeedAP { get { return this.cost_ap; } }
}
