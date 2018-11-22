using SmileLab.Net.API;

/// <summary>
/// partial : キャラクエスト.
/// </summary>
public partial class UnitQuest : IQuestData
{
    /// <summary>
    /// クエストのタイプ
    /// </summary>
    public int QuestType { get { return 3; } }

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
			if (m_cardData == null) {
                m_cardData = new CardData(MasterDataTable.card[card_id]);
            }
            if (countryInfo == null) {
				countryInfo = MasterDataTable.belonging.DataList.Find(b => b.Enum == m_cardData.Parameter.belonging.Enum);
            }
			return countryInfo ?? MasterDataTable.belonging[BelongingEnum.Unknown];
        }
    }
    private Belonging countryInfo;
	private CardData m_cardData;   

    /// <summary>
    /// from IQuestData interface : 章番号.
    /// </summary>
    public int ChapterNum { get { return 0; } }

    /// <summary>
    /// from IQuestData interface : 幕番号.
    /// </summary>
    public int StageNum { get { return this.stage; } }

    /// <summary>
    /// from IQuestData interface : シーン番号.
    /// </summary>
    public int QuestNum { get { return this.quest; } }

	/// <summary>
    /// from IQuestData interface : クエスト名として表示する名前.
    /// </summary>
	public string QuestName { get { return "クエスト"+this.quest; } }

    /// <summary>
    /// 必要AP.
    /// </summary>
	public int NeedAP { get { return this.cost_ap; } }

    /// <summary>
    /// 初回クリア報酬があるか？
    /// </summary>
    public bool HasClearReward { get { return false; } }

    /// <summary>
    /// 初回クリア報酬タイプ
    /// </summary>
    public ItemTypeEnum? ClearRewardType { get { return null; } }

    /// <summary>
    /// 初回クリア報酬ID
    /// </summary>
    public int ClearRewardId { get { return 0; } }

    /// <summary>
    /// 初回クリア報酬数
    /// </summary>
    public int ClearRewardQuantity { get { return 0; }  }

    /// <summary>
    /// 強制ロックか？
    /// </summary>
    public bool ForceLock { get { return false; } }

    public int[] ReleaseMissions { get { return new int[0]; } }
}
