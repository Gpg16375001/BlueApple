using System;
using System.Linq;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

using SmileLab;
using SmileLab.Net.API;

[Serializable]
public class Party {
    /// <summary>
    /// 編成名
    /// </summary>
    public string Name;
    /// <summary>
    /// チーム番号
    /// </summary>
    public int Number;
    /// <summary>
    /// 陣形データID
    /// </summary>
    public int FormationID;
    /// <summary>
    /// ポジション1のカードデータID
    /// </summary>
    public int Position1_CardID;
    /// <summary>
    /// ポジション2のカードデータID
    /// </summary>
    public int Position2_CardID;
    /// <summary>
    /// ポジション3のカードデータID
    /// </summary>
    public int Position3_CardID;
    /// <summary>
    /// ポジション4のカードデータID
    /// </summary>
    public int Position4_CardID;
    /// <summary>
    /// ポジション5のカードデータID
    /// </summary>
    public int Position5_CardID;

    private string BackUpName;
    private int BackupFormationID;
    private int BackupPosition1_CardID;
    private int BackupPosition2_CardID;
    private int BackupPosition3_CardID;
    private int BackupPosition4_CardID;
    private int BackupPosition5_CardID;

    public const int PartyCardMax = 5;
    public int Count {
        get {
            return PartyCardMax;
        }
    }

    public int TotalCombat {
        get {
            GetCardDataList ();
            return _cardDataList.Where (x => x != null).Sum (x => x.Parameter.Combat);
        }
    }

    private int nowRevision;
    /// <summary>
    /// カード情報のリストを返す
    /// 情報のアクセスの仕方を簡単にするために1 ~ 5で値が取れるようにしている
    /// </summary>
    public CardData this[int index] {
        get {
            if (index < 1 || index > PartyCardMax) {
                return null;
            }

            GetCardDataList ();
            return _cardDataList [index - 1];
        }
        set {
            if (index < 1 || index > PartyCardMax) {
                return;
            }

            GetCardDataList ();
            int pos = index - 1;
            if (_cardDataList [pos] != value || value == null || _cardDataList [pos] == null || _cardDataList [pos].CardId != value.CardId) {
                SetCardID (index, value != null ? value.CardId : 0);
                _cardDataList [pos] = value;
            }
        }
    }

    public bool isPvP {
        get {
            return Number == AwsPartyData.PartyMax;
        }
    }

    public void SetName(string name)
    {
        if (Name != name) {
            Name = name;
            if (string.IsNullOrEmpty (Name)) {
                Name = string.Format ("チーム{0}", Number);
            }
            IsModify = true;
        }
    }

    private CardData[] _cardDataList;
    private CardData[] GetCardDataList()
    {
        var revision = CardData.GetRevision ();
        if (_cardDataList == null || revision != nowRevision) {
            nowRevision = revision;
            _cardDataList = new CardData[PartyCardMax];
            if (Position1_CardID > 0) {
                _cardDataList[0] = CardData.CacheGet (Position1_CardID);
                if (_cardDataList [0] == null) {
                    Position1_CardID = 0;
                }
            } else {
                _cardDataList[0] = null;
            }

            if (Position2_CardID > 0) {
                _cardDataList[1] = CardData.CacheGet (Position2_CardID);
                if (_cardDataList [1] == null) {
                    Position2_CardID = 0;
                }
            } else {
                _cardDataList[1] = null;
            }

            if (Position3_CardID > 0) {
                _cardDataList[2] = CardData.CacheGet (Position3_CardID);
                if (_cardDataList [2] == null) {
                    Position3_CardID = 0;
                }
            } else {
                _cardDataList[2] = null;
            }

            if (Position4_CardID > 0) {
                _cardDataList[3] = CardData.CacheGet (Position4_CardID);
                if (_cardDataList [3] == null) {
                    Position4_CardID = 0;
                }
            } else {
                _cardDataList[3] = null;
            }

            if (Position5_CardID > 0) {
                _cardDataList[4] = CardData.CacheGet (Position5_CardID);
                if (_cardDataList [4] == null) {
                    Position5_CardID = 0;
                }
            } else {
                _cardDataList[4] = null;
            }
        }

        return _cardDataList;
    }

    private void SetCardID(int index, int cardDataId)
    {
        switch (index) {
        case 1:
            Position1_CardID = cardDataId;
            break;
        case 2:
            Position2_CardID = cardDataId;
            break;
        case 3:
            Position3_CardID = cardDataId;
            break;
        case 4:
            Position4_CardID = cardDataId;
            break;
        case 5:
            Position5_CardID = cardDataId;
            break;
        }
        IsModify = true;
    }

    /// <summary>
    /// 陣形情報を返す
    /// </summary>
    private FormationData _formation;
    public FormationData FormationData {
        get {
            if (_formation == null) {
                _formation = FormationData.CacheGet(FormationID);
            }
            return _formation;
        }
        set {
            if (_formation == null || value.FormationId != FormationID) {
                _formation = value;
                FormationID = value.FormationId;
                IsModify = true;
            }
        }
    }

    public Party(int number)
    {
        Number = number;
        Name = string.Format ("チーム{0}", number);
        FormationID = 4;
    }

    // 開発用の初期化関数
    // あとで本番用の関数ができるはず
    public void InitForDevelop()
    {
        if (!IsEmpty) {
            return;
        }
        if (GetCardDataList ().All (x => x == null)) {
            int index = 1;
            foreach (var cardData in CardData.CacheGetAll()) {
                this [index++] = cardData;
                if (index > PartyCardMax) {
                    break;
                }
            }
        }

        if (FormationID <= 0) {
            FormationData = FormationData.CacheGetAll ().First (); 
        }

        // 保存してもらわないと困るから落とさない
        IsModify = true;
    }

    /// <summary>
    /// 初期化.
	/// 持ってるカードをレアリティ順に上詰して初期設定しておくとのこと.
    /// </summary>
    public void Init()
	{
        if (!IsEmpty) {
            return;
        }
		_cardDataList = new CardData[PartyCardMax];
		
        int index = 1;
		var cardList = CardData.CacheGetAll().ToList();
		cardList.Sort((x, y) => y.Rarity - x.Rarity);
		foreach (var cardData in cardList) {
            this[index++] = cardData;
            if (index > PartyCardMax) {
                break;
            }
        }

        if (FormationID <= 0) {
            // 初期化は基本陣形で
            var formationData = FormationData.CacheGetAll().FirstOrDefault(x => x.FormationId == 4);
            if (formationData != null) {
                FormationData = formationData;
            } else {
                FormationData = FormationData.CacheGetAll ().First ();
            }
        }
	}

    public int GetPosition(CardData card)
    {
        int index = Array.FindIndex (GetCardDataList (), x => x != null && x.CardId == card.CardId);
        return index >= 0 ? index + 1 : index;
    }

    public bool IsEmpty {
        get {
            return (Position1_CardID <= 0 &&
                Position2_CardID <= 0 &&
                Position3_CardID <= 0 &&
                Position4_CardID <= 0 &&
                Position5_CardID <= 0) ||
                FormationID <= 0;
        }
    }

    public bool IsModify {
        get;
        private set;
    }

    public void Reset()
    {
        _cardDataList = null;
        _formation = null;

        FormationID = BackupFormationID;
        Position1_CardID = BackupPosition1_CardID;
        Position2_CardID = BackupPosition2_CardID;
        Position3_CardID = BackupPosition3_CardID;
        Position4_CardID = BackupPosition4_CardID;
        Position5_CardID = BackupPosition5_CardID;
        Name = BackUpName;
        IsModify = false;
    }

    public void Commit()
    {
        IsModify = false;
        BackupFormationID = FormationID;
        BackupPosition1_CardID = Position1_CardID;
        BackupPosition2_CardID = Position2_CardID;
        BackupPosition3_CardID = Position3_CardID;
        BackupPosition4_CardID = Position4_CardID;
        BackupPosition5_CardID = Position5_CardID;
        BackUpName = Name;
    }
}

public class AwsPartyData : AwsCognitoDatasetBase
{
    private Party[] PartyArray;

    public const int PartyMax = 10;
    public const int PvPTeamIndex = 9;
    public Party this[int index] {
        get {
            return PartyArray[index];
        }
    }

    public Party CurrentTeam {
        get {
            return PartyArray[CurrentTeamIndex];
        }
    }

    public Party PvPTeam {
        get {
            return PartyArray[PvPTeamIndex];
        }
    }

    public bool IsModify {
        get {
            return PartyArray.Any (x => x.IsModify) || CurrentTeamIndex != _BackupCurrentTeamIndex;
        }
    }
        
    public int CurrentTeamIndex;
    private int _BackupCurrentTeamIndex;

    public AwsPartyData(CognitoSyncManager mng) : base(mng, "PlayerPartyData")
    {
        LoadPartyData ();
        _BackupCurrentTeamIndex = CurrentTeamIndex;
    }

    protected override void ClearValues ()
    {
        CurrentTeamIndex = 0;
    }

    public override void Sync (DidSyncDelegate didCallback)
    {
        // 変更がない場合はSyncしない。
        if (PartyArray.Any (x => x.IsModify) || CurrentTeamIndex != _BackupCurrentTeamIndex) {
            for (int i = 0; i < PartyMax; ++i) {
                Put (string.Format ("Party{0}", i), PartyArray [i]);
            }
            Put("CurrentTeamIndex", CurrentTeamIndex);
            base.Sync (
                (bSuccess, sender, eArgs) => {
                    if(bSuccess) {
                        for (int i = 0; i < PartyMax; ++i) {
                            PartyArray [i].Commit();
                        }
                        _BackupCurrentTeamIndex = CurrentTeamIndex;
                    }
                    didCallback(bSuccess, sender, eArgs);
                }
            );
        } else {
            if (didCallback != null) {
                didCallback (true, null, null);
            }
        }
    }

    public void  SyncAndForceCommit (DidSyncDelegate didCallback)
    {
        // 変更がない場合はSyncしない。
        if (PartyArray.Any (x => x.IsModify) || CurrentTeamIndex != _BackupCurrentTeamIndex) {
            for (int i = 0; i < PartyMax; ++i) {
                Put (string.Format ("Party{0}", i), PartyArray [i]);
            }
            Put("CurrentTeamIndex", CurrentTeamIndex);
            base.Sync (
                (bSuccess, sender, eArgs) => {
                    for (int i = 0; i < PartyMax; ++i) {
                        PartyArray [i].Commit();
                    }
                    _BackupCurrentTeamIndex = CurrentTeamIndex;
                    didCallback(bSuccess, sender, eArgs);
                }
            );
        } else {
            if (didCallback != null) {
                didCallback (true, null, null);
            }
        }
    }

    public void OriginalSync (DidSyncDelegate didCallback)
    {
        base.Sync (didCallback);
    }

    private void LoadPartyData()
    {
        PartyArray = new Party[PartyMax];
        for (int i = 0; i < PartyMax; ++i) {
            string key = string.Format ("Party{0}", i);
            if (ExistKey (key)) {
                PartyArray [i] = Get<Party> (key);
                if (PartyArray [i].FormationID <= 0) {
                    PartyArray [i].FormationID = 4;
                }
            } else {
                PartyArray [i] = new Party (i + 1);
            }
            PartyArray [i].Commit ();
        }
        CurrentTeamIndex = Get<int> ("CurrentTeamIndex");
    }


    public void Reset()
    {
        // ローカルの内容が書き換わっている可能性もあるのでPutもしておく
        CurrentTeamIndex = _BackupCurrentTeamIndex;
        Put("CurrentTeamIndex", CurrentTeamIndex);
        for(int i = 0; i < PartyMax; ++i) {
            PartyArray[i].Reset ();
            Put (string.Format ("Party{0}", i), PartyArray [i]);
        }
    }
}
