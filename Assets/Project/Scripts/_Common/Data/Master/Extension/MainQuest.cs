/// <summary>
/// partial : メインクエスト.
/// </summary>
public partial class MainQuest : IQuestData
{
    /// <summary>
    /// クエストのタイプ
    /// </summary>
    public int QuestType { get { return 1; } }

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
			if(countryInfo == null){
				countryInfo = MasterDataTable.belonging.DataList.Find(b => b.name == this.country);
			}
			return countryInfo;
		} 
	}
	private Belonging countryInfo;

    /// <summary>
    /// from IQuestData interface : 章番号.
    /// </summary>
	public int ChapterNum { get { return this.stage_info.chapter_info.chapter; } }

    /// <summary>
    /// from IQuestData interface : 幕番号.
    /// </summary>
	public int StageNum { get { return this.stage_info.stage; } }

    /// <summary>
    /// from IQuestData interface : シーン番号.
    /// </summary>
	public int QuestNum { get { return this.quest; } }

    /// <summary>
	/// from IQuestData interface : クエスト名として表示する名前.
    /// </summary>
	public string QuestName { get { return this.stage_info.stage_name; } }

    /// <summary>
    /// 消費AP.
    /// </summary>
	public int NeedAP { get { return this.cost_ap; } }
}
