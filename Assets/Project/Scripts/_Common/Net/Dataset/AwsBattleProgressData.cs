using System;
using System.Linq;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using UnityEngine;
using UniRx;

using SmileLab.Net.API;

/// <summary>
/// AWS SDK "Cognito"を利用した現在のバトル進行データ.
/// </summary>
public class AwsBattleProgressData : AwsCognitoDatasetBase
{
    /// <summary>現在バトル進行中かどうか.</summary>
    public bool IsProgressing 
    { 
        get { return Get<bool>("IsProgressing"); }
        private set { Put("IsProgressing", value); }
    }

    /// <summary>バトル時の速度(恒久保存).</summary>
    public int BattleSpeed
    {
        get { return Get<int>("BattleSpeed"); }
        private set { Put("BattleSpeed", value); }
    }

    /// <summary>オートモード設定してる？</summary>
    public bool IsAuto
    {
        get { return Get<bool>("IsAuto"); }
        private set { Put("IsAuto", value); }
    }

    public bool IsPVP {
        get;
        private set;
    }

    /// <summary> 復帰時に必要なのでクエストIDも </summary>
    public int QuestType;
    /// <summary> 復帰時に必要なのでクエストIDも </summary>
    public int QuestID;

    /// <summary>ステージID.</summary>
    public int StageID;

    public bool IsRetire;

    private BattleStage _stage;
    /// <summary>ステージ情報.</summary>
    public BattleStage Stage {
        get {
            return _stage;
        }
    }

    private BattleStageEnemy[] _stageEnemys;
    /// <summary>ステージ敵情報一覧.</summary>
    public BattleStageEnemy[] StageEnemy {
        get {
            return _stageEnemys;
        }
    }

    private BattleWaveSetting[] _stageWaveSettings;
    /// <summary>ステージウェーブ情報一覧.</summary>
    public BattleWaveSetting[] StageWaveSettings {
        get {
            return _stageWaveSettings;
        }
    }

    /// <summary>最大WAVE数.</summary>
    public int MaxWaveCount { get; private set; }
    /// <summary>現在何WAVEまで進んでいるかの値.</summary>
    public int WaveCount { get; set; }
    /// <summary>現在のアイテムドロップ数.</summary>
	public int ItemDropCount {
		get { 
			return DropedItems.Count;
		}
	}

    /// <summary>バトル終了してる？</summary>
    public bool EndBattle { get { return MaxWaveCount < WaveCount; } }

    public bool LastWave { get { return MaxWaveCount == WaveCount; } }

    /// <summary>バトルエントリーデータ</summary>
    public BattleEntryData BattleEntryData { get; set; }

    /// <summary>PVPエントリーデータ</summary>
    public PvpBattleEntryData PVPBattleEntryData { get; set; }

    /// <summary>味方のリスト.</summary>
    public List<ListItem_BattleUnit> AllyList { get; private set; }
    /// <summary>現在表示中の敵リスト.</summary>
    public List<ListItem_BattleUnit> EnemyList { get; private set; }

    /// <summary>味方の援軍リスト.</summary>
    public List<ListItem_BattleUnit> AllySupportList { get; private set; }
    /// <summary>敵の援軍リスト.</summary>
    public List<ListItem_BattleUnit> EnemySupportList { get; private set; }

    /// <summary>味方の死亡ユニットリスト.</summary>
    public List<ListItem_BattleUnit> AllyDeadList { get; private set; }
    /// <summary>敵の死亡ユニットリスト.</summary>
    public List<ListItem_BattleUnit> EnemyDeadList { get; private set; }

    /// <summary>味方のパラメータリスト.</summary>
    public List<BattleLogic.Parameter> AllyParameterList { get; private set; }
    /// <summary>味方のフィールド上パラメータリスト.</summary>
    public IEnumerable<BattleLogic.Parameter> SallyAllyParameterList {
        get {
            return AllyParameterList.Where (x => x.Hp > 0 && x.PositionIndex <= 9);
        }
    }
    /// <summary>現ウェーブ中敵パラメータリスト.</summary>
    public List<BattleLogic.Parameter> EnemyParameterList { get; private set; }
    /// <summary>現ウェーブ中敵のフィールド上パラメータリスト.</summary>
    public IEnumerable<BattleLogic.Parameter> SallyEnemyParameterList {
        get {
            return EnemyParameterList.Where (x => x.Hp > 0 && x.PositionIndex <= 9);
        }
    }
    /// <summary>取得アイテムリスト.</summary>
    public List<ItemData> DropedItems { get; private set; }

    /// <summary>味方の陣形レベル.</summary>
    public int AllyFormationLevel { get; private set; }
    /// <summary>味方の陣形情報.</summary>
    public Formation AllyFormation { get; set; }
    /// <summary>現Wave敵の陣形レベル.</summary>
    public int EnemyFormationLevel { get; private set; }
    /// <summary>現Wave敵の陣形情報.</summary>
    public Formation EnemyFormation { get; set; }

    /// <summary>WAVE終了している？GameOverも含む.</summary>
    public bool IsEndWave { 
        get { 
            return (AllyList.All(a => a.IsDead) && 
                AllySupportList.All(a => a.IsDead)) ||
                (EnemyList.All(e => e.IsDead) &&
                 EnemySupportList.All(e => e.IsDead)); 
        }
    }

    /// <summary>ミッション進捗情報.</summary>
    public BattleMissionProgressManager MissionProgress { get; private set; }
    public BattleMissionProgressData MissionProgressData { get; private set; }

    public SupporterCardData SupportCard { get; set; }

    public int SupportUserId { get; set; }

    public int TurnCount { get; set; }
    private int? LimitTurn;

    public int RestTurnCount {
        get {
            if (LimitTurn.HasValue) {
                return LimitTurn.Value - TurnCount;
            }
            return 0;
        }
    }

    /// <summary>
    /// スタート前初期化.ステージIDを暫定で設定する.BattleEntryDataの設定は確認しない.本番スタート時はStartProcを呼ぶ.
    /// </summary>
    public void SetStage(int stageId)
    {
        if (stageId == this.StageID && MissionProgress != null && _stage != null) {
            return;
        }

        if (AwsModule.ProgressData.CurrentQuest != null) {
            this.QuestType = AwsModule.ProgressData.CurrentQuest.QuestType;
            this.QuestID = AwsModule.ProgressData.CurrentQuest.ID;
        }
        this.StageID = stageId;

        _stage = MasterDataTable.stage.DataList.
            FirstOrDefault (x => x.id == stageId);
        MissionProgress = new BattleMissionProgressManager(this.StageID);

        AllyList = new List<ListItem_BattleUnit>();
        EnemyList = new List<ListItem_BattleUnit>();
        AllySupportList = new List<ListItem_BattleUnit>();
        EnemySupportList = new List<ListItem_BattleUnit>();
        AllyDeadList = new List<ListItem_BattleUnit> ();
        EnemyDeadList = new List<ListItem_BattleUnit> ();
        AllyParameterList = new List<BattleLogic.Parameter>();
        EnemyParameterList = new List<BattleLogic.Parameter>();
        DropedItems = new List<ItemData> ();
    }

	/// <summary>
	/// バトル関連のマスターデータロード.
	/// </summary>
	public void LoadBattleMasterData(Action<bool> didEnd, bool bForce = false)
	{
		Action<bool> didEndEx = bSuccess => {
            if (_stageWaveSettings == null || _stageEnemys == null) {
                return;
            }
            if (didEnd != null) {
                didEnd(bSuccess);
            }
        };
		
		if(bForce){
			_stageEnemys = null;
            _stageWaveSettings = null;
		}else{
			if(_stageEnemys != null && _stageWaveSettings != null){
				didEndEx(true);
				return;
			}
		}  
        
		if(_stageEnemys == null){
			BattleStageEnemyTable.LoadPartitionData(StageID,
                (stageEnemys) => {
                    if (stageEnemys == null) {
                        if (didEnd != null) {
                            didEnd(false);
                        }
                        return;
                    }
                    _stageEnemys = stageEnemys.DataList.ToArray();
                    didEndEx(true);
                }
            );
		}      
  
		if(_stageWaveSettings == null){
			BattleWaveSettingTable.LoadPartitionData(this.StageID,
                (waveSettings) => {
                    if (waveSettings == null) {
                        if (didEnd != null) {
                            didEnd(false);
                        }
                        return;
                    }
                    _stageWaveSettings = waveSettings.DataList.ToArray();
                    didEndEx(true);
                }
            );
		}      
	}

    /// <summary>
    /// バトル開始時に呼ぶ.中断している場合はWave単位で続きからの情報を取得する.
    /// </summary>
    public void StartProc(System.Action<bool> didEnd)
    {
        IsPVP = false;
        IsRetire = false;
        if (BattleEntryData != null) {
            this.SetStage(BattleEntryData.StageId);
        } else {
#if DEVELOPMENT_BUILD
            this.SetStage(101010101);
#else
            return;
#endif
        }

        BattleLogic.Calculation.Init (Guid.NewGuid().ToString());

        IsProgressing = true;
        WaveCount = 1;
        TurnCount = 0;
        LimitTurn = null;

		// 必要情報の取得
		LoadBattleMasterData(bSucccess => {
            if(!bSucccess){
                didEnd(false);
                return;
            }
            InitParamter(didEnd);
        }, true);   
    }   
    // バトル開始時のパラメータ初期化処理
    private void InitParamter(System.Action<bool> didEnd)
    {
        // 必要なデータが揃うまでまつ
        if (_stageWaveSettings == null || _stageEnemys == null) {
            return;
        }
        MaxWaveCount = StageWaveSettings.Count() > 0 ? 
            StageWaveSettings.Max(x => x.wave_count) : 0;
        // パラメータを作成.
        MakePlayerParameterList();
        MakeEnemyParameterList(WaveCount);

        foreach(var unit in SallyAllyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }
        foreach(var unit in SallyEnemyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }

        foreach(var unit in SallyAllyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();
        }


        foreach(var unit in SallyEnemyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();
        }

        var party = AwsModule.PartyData.CurrentTeam;
        var idList = new List<int>();
        idList.Add(party.Position1_CardID);
        idList.Add(party.Position2_CardID);
        idList.Add(party.Position3_CardID);
        idList.Add(party.Position4_CardID);
        idList.Add(party.Position5_CardID);
        if (SupportCard != null) {
            idList.Add (SupportCard.CardId);
        }
        var allHp = AwsModule.BattleData.AllyParameterList.Select(p => p.Hp).ToList().Sum();
        MissionProgressData = new BattleMissionProgressData (idList.ToArray (), allHp);

        this.SyncBattleData();

        if(didEnd != null) {
            didEnd(true);
        }
    }

    /// <summary>
    /// バトルの復帰処理
    /// </summary>
    /// <param name="didEnd">処理終了時コールバック.</param>
    public void Reversion(System.Action<bool> didEnd)
    {
        IsRetire = false;
        IsPVP = false;
        // バトル中の進捗データの復帰処理
        var stageID = Get<int> ("StageID");
        var questType = Get<int> ("QuestType");
        var questID = Get<int> ("QuestID");

        BattleLogic.Calculation.Init (Get<UnityEngine.Random.State> ("RandomState"));

        if (BattleEntryData.StageId != stageID || questID != QuestID || questType != QuestType) {
            // 初期化して始める
            StartProc (didEnd);
            return;
        }
        StageID = BattleEntryData.StageId;
        WaveCount = Get<int> ("WaveCount");

        _stage = MasterDataTable.stage.DataList.
            FirstOrDefault (x => x.id == StageID);
        MissionProgress = new BattleMissionProgressManager(this.StageID);

        AllyList = new List<ListItem_BattleUnit>();
        EnemyList = new List<ListItem_BattleUnit>();
        AllySupportList = new List<ListItem_BattleUnit>();
        EnemySupportList = new List<ListItem_BattleUnit>();
        AllyDeadList = new List<ListItem_BattleUnit> ();
        EnemyDeadList = new List<ListItem_BattleUnit> ();

        // 必要情報の取得
        BattleStageEnemyTable.LoadPartitionData (StageID,
            (stageEnemys) => {
                if (stageEnemys == null) {
                    if (didEnd != null) {
                        didEnd (false);
                    }
                    return;
                }
                _stageEnemys = stageEnemys.DataList.ToArray();

                if (_stageWaveSettings != null && _stageEnemys != null) {
                    ReversionParameter();
                    didEnd(true);
                }
            }
        );
        BattleWaveSettingTable.LoadPartitionData (this.StageID,
            (waveSettings) => {
                if(waveSettings == null) {
                    if(didEnd != null) {
                        didEnd(false);
                    }
                    return;
                }
                _stageWaveSettings = waveSettings.DataList.ToArray();

                // 最大Weva数の設定
                MaxWaveCount = StageWaveSettings.Count() > 0 ? 
                    StageWaveSettings.Max(x => x.wave_count) : 0;
                
                if (_stageWaveSettings != null && _stageEnemys != null) {
                    ReversionParameter();
                    didEnd(true);
                }
            }
        );

        // 選択肢情報の復帰
        var CurrentScenarioSelectIdListString = Get<string> ("CurrentScenarioSelectIdList");
        if (!string.IsNullOrEmpty (CurrentScenarioSelectIdListString)) {
            AwsModule.ProgressData.CurrentScenarioSelectIdList = CurrentScenarioSelectIdListString.Split (',').Select (x => int.Parse (x)).ToList ();
        } else {
            AwsModule.ProgressData.CurrentScenarioSelectIdList = new List<int> ();
        }
        // ミッションの進捗情報の復帰
        MissionProgressData = Get<BattleMissionProgressData> ("BattleMissionProgressData");
    }
    private void ReversionParameter()
    {
        saveParameter = Get<SaveData> ("ParameterData");

        var saveDataArray = saveParameter.ally;
        int listCount = saveDataArray.Length;
        AllyParameterList = new List<BattleLogic.Parameter> ();
        for(int i = 0; i < listCount; ++i) {
            var unit = new BattleLogic.Parameter ();
            unit.Load(saveDataArray[i]);
            unit.Reversion (unit, saveDataArray[i]);
            AllyParameterList.Add(unit);
        }
        AllyFormationLevel = Get<int> ("AllyFormationLevel");
        AllyFormation = MasterDataTable.formation[Get<int> ("AllyFormationID")];

        // 陣形が発動するかを判定
        foreach(var unit in AwsModule.BattleData.AllyParameterList) {
            var skill = this.AllyFormation.GetPositionSkill(unit.PositionIndex);
            if(skill != null) {
                // 陣形が効果発動するかを判定
                if(this.AllyFormation.SatisfyTheCondition(unit)) {
                    // 陣形スキルをパッシブスキルとして追加
                    unit.AddPassiveSkill(skill, this.AllyFormationLevel);
                }
            }
        }

        saveDataArray = saveParameter.enemy;
        listCount = saveDataArray.Length;
        EnemyParameterList = new List<BattleLogic.Parameter> ();
        for(int i = 0; i < listCount; ++i) {
            var unit = new BattleLogic.Parameter ();
            unit.Load(saveDataArray[i]);
            unit.Reversion (unit, saveDataArray[i]);
            EnemyParameterList.Add(unit);
        }
        EnemyFormationLevel = Get<int> ("EnemyFormationLevel");
        EnemyFormation = MasterDataTable.formation[Get<int> ("EnemyFormationID")];

        // 陣形が発動するかを判定
        foreach(var unit in AwsModule.BattleData.EnemyParameterList) {
            var skill = this.EnemyFormation.GetPositionSkill(unit.PositionIndex);
            if(skill != null) {
                // 陣形が効果発動するかを判定
                if(this.EnemyFormation.SatisfyTheCondition(unit)) {
                    // 陣形スキルをパッシブスキルとして追加
                    unit.AddPassiveSkill(skill, this.AllyFormationLevel);
                }
            }
        }


        foreach(var unit in SallyAllyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }
        foreach(var unit in SallyEnemyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }

        foreach(var unit in AllyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
        }
        foreach(var unit in EnemyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
        }

        var party = AwsModule.PartyData.CurrentTeam;
        var idList = new List<int>();
        idList.Add(party.Position1_CardID);
        idList.Add(party.Position2_CardID);
        idList.Add(party.Position3_CardID);
        idList.Add(party.Position4_CardID);
        idList.Add(party.Position5_CardID);
        var supportUnit = AllyParameterList.FirstOrDefault (x => x.OriginalPositionIndex == 10);
        if (supportUnit != null) {
            idList.Add (supportUnit.ID);
        }
        if (MissionProgressData != null) {
            MissionProgressData.Reversion (idList.ToArray ());
        }

        DropedItems = saveParameter.DropedItems;
    }

    /// <summary>
    /// PvP開始処理
    /// </summary>
    /// <param name="entryData">Entry data.</param>
    public void PvpStartProc(PvpBattleEntryData entryData)
    {
        IsRetire = false;
        IsPVP = true;
        PVPBattleEntryData = entryData;
        LimitTurn = 100;

        BattleLogic.Calculation.Init (Guid.NewGuid().ToString());

        // あとでサーバーからとか引っ張ってくる必要性があるかも
        SetStage (200000001);

        // PVPの時は1waveのみ
        WaveCount = 1;
        MaxWaveCount = 1;
        TurnCount = 0;

        // パラメータを作成.
        MakePvpParameterList(entryData.PvpTeamData, true);
        MakePvpParameterList(entryData.OpponentPvpTeamData, false);

        foreach(var unit in SallyAllyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }
        foreach(var unit in SallyEnemyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }

        foreach(var unit in SallyAllyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();
        }
        foreach(var unit in SallyEnemyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();
        }
    }

    /// <summary>
    /// Wave更新.
    /// </summary>
    public void UpdateWave()
    {
        this.WaveCount++;
        if(EndBattle){
            return;
        }

        // 敵リセット.
        foreach(var enemy in EnemyList){
            enemy.DestroyUnit();
        }
        EnemyList.Clear();
        foreach(var enemy in EnemyDeadList){
            enemy.DestroyUnit();
        }
        EnemyDeadList.Clear();

        MakeEnemyParameterList(WaveCount);

        // TODO: パッシブスキルの更新処理
        foreach(var unit in SallyAllyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit, SallyEnemyParameterList.ToArray());
        }
        foreach(var unit in SallyEnemyParameterList) {
            BattleLogic.Calculation.AdditionPassiveSkill(unit);
        }

        foreach(var unit in SallyAllyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();

            unit.WaveChange ();
        }

        foreach(var unit in SallyEnemyParameterList) {
            // パラメータの計算をし直す
            unit.RecalcParameterVariation();
            // 速度変化があった時ようにここでリセットしておく
            unit.ResetWeight();

            unit.WaveChange ();
        }

        this.SyncUpdateBattleData();
    }

    [Serializable]
    public class SaveData
    {
        public List<ItemData> DropedItems;
        public BattleLogic.Parameter.SaveParameter[] ally;
        public BattleLogic.Parameter.SaveParameter[] enemy;
    }

    SaveData saveParameter = new SaveData();
    private void SyncBattleData()
    {
        if (IsPVP) {
            return;
        }
        RandomState = BattleLogic.Calculation.GetRandomState ();
        Put ("RandomState", RandomState);
        Put ("StageID", StageID);
        Put ("QuestType", QuestType);
        Put ("QuestID", QuestID);
        Put ("WaveCount", WaveCount);

        saveParameter.ally = AllyParameterList.Select (x => x.CreateSaveData ()).ToArray ();
        saveParameter.enemy = EnemyParameterList.Select (x => x.CreateSaveData ()).ToArray ();
        saveParameter.DropedItems = DropedItems;
        Put ("ParameterData", saveParameter);

        Put("AllyFormationLevel", AllyFormationLevel);
        Put ("AllyFormationID", AllyFormation.id);
        Put("EnemyFormationLevel", EnemyFormationLevel);
        Put ("EnemyFormationID", EnemyFormation.id);

        Put ("CurrentScenarioSelectIdList", string.Join(",", AwsModule.ProgressData.CurrentScenarioSelectIdList.Select(x => x.ToString()).ToArray()));
        Put ("BattleMissionProgressData", MissionProgressData);

        Put ("TurnCount", TurnCount);
        if (LimitTurn.HasValue) {
            Put ("LimitTurn", LimitTurn.Value);
        } else {
            Remove ("LimitTurn");
        }

        Sync ();
    }

    private void SyncUpdateBattleData()
    {
        if (IsPVP) {
            return;
        }
        RandomState = BattleLogic.Calculation.GetRandomState ();
        Put ("RandomState", RandomState);
        Put ("WaveCount", WaveCount);

        foreach (var unit in SallyAllyParameterList) {
            unit.UpdateSaveData ();
        }
        saveParameter.enemy = EnemyParameterList.Select (x => x.CreateSaveData ()).ToArray ();
        Put ("ParameterData", saveParameter);

        Put("EnemyFormationLevel", EnemyFormationLevel);
        Put ("EnemyFormationID", EnemyFormation.id);
        Put ("BattleMissionProgressData", MissionProgressData);

        Sync ();
    }

    UnityEngine.Random.State RandomState;
    public void UpdateBattleData()
    {
        if (IsPVP) {
            return;
        }

        RandomState = BattleLogic.Calculation.GetRandomState ();
        foreach (var unit in AllyParameterList) {
            if (unit.IsModify ()) {
                Debug.Log (unit.Name);
                unit.UpdateSaveData ();
            }
        }
        foreach (var unit in EnemyParameterList) {
            if (unit.IsModify ()) {
                Debug.Log (unit.Name);
                unit.UpdateSaveData ();
            }
        }
    }

    public void PutBattleData()
    {
        if (IsPVP || !IsProgressing) {
            return;
        }

        Put ("RandomState", RandomState);
        Put ("ParameterData", saveParameter);
        Put ("TurnCount", TurnCount);
        Put ("BattleMissionProgressData", MissionProgressData);
    }

    /// <summary>
    /// ゲームオーバー.
    /// </summary>
    public void GameOverProc()
    {
        IsRetire = false;
        IsProgressing = false;
        WaveCount = 0;
		if(DropedItems != null){
			DropedItems.Clear();
		}

        AllyFormation = null;
        if (AllyList != null) {
            foreach (var ally in AllyList) {
                ally.DestroyUnit();
            }
            AllyList.Clear();
        }

        if (AllySupportList != null) {
            foreach (var ally in AllySupportList) {
                ally.DestroyUnit();
            }
            AllySupportList.Clear();
        }

        if (AllyDeadList != null) {
            foreach (var ally in AllyDeadList) {
                ally.DestroyUnit();
            }
            AllyDeadList.Clear();
        }

        if (AllyParameterList != null) {
            AllyParameterList.Clear ();
        }

        EnemyFormation = null;
        if (EnemyList != null) {
            foreach (var enemy in EnemyList) {
                enemy.DestroyUnit();
            }
            EnemyList.Clear();
        }

        if (EnemySupportList != null) {
            foreach (var ally in EnemySupportList) {
                ally.DestroyUnit();
            }
            EnemySupportList.Clear();
        }

        if (EnemyDeadList != null) {
            foreach (var enemy in EnemyDeadList) {
                enemy.DestroyUnit();
            }
            EnemyDeadList.Clear();
        }

        if (EnemyParameterList != null) {
            EnemyParameterList.Clear ();
        }

        BattleEntryData = null;
        StageID = 0;
    }

    /// <summary>
    /// バトルスピード切り替え.
    /// </summary>
    public void ChangeBattleSpeed()
    {
        this.BattleSpeed = this.BattleSpeed == 1 ? 2: 1;
    }

    /// <summary>
    /// オートモード切り替え.
    /// </summary>
    public bool ChangeAutoMode()
    {
        this.IsAuto = !this.IsAuto;
        return this.IsAuto;
    }

    /// <summary>
    /// バトルユニットリストの更新.
    /// </summary>
    public void UpdateBattleUnit()
    {
        foreach(var ally in AllyList) {
            if(ally.IsDead) {
                ally.gameObject.SetActive (false);
                ally.transform.SetParent (null);
                AllyDeadList.Add(ally);
            }
        }
        AllyList.RemoveAll(a => a == null || a.IsDead);

        foreach(var enemy in EnemyList){
            if(enemy.IsDead){
                enemy.gameObject.SetActive (false);
                enemy.transform.SetParent (null);
                EnemyDeadList.Add(enemy);
            }
        }
        EnemyList.RemoveAll(e => e == null || e.IsDead);
    }

    public int GetNumberOfFront(ListItem_BattleUnit unit)
    {
        var unitList = AllyList;
        if (!unit.IsPlayer) {
            unitList = EnemyList;
        }

        // 全ての値を取得し重複を削除する。
        var rows = unitList.Select (x => x.Parameter.Position.row).Distinct().OrderBy(x => x);
        var rowCount = rows.Count ();

        int targetRow = unit.Parameter.Position.row;
        for (int i = 0; i < rowCount; ++i) {
            if (targetRow == rows.ElementAt (i)) {
                return i;
            }
        }
        return 0;
    }

    public bool IsWaveClear()
    {
        return !(
            EnemyList.Exists (e => e != null && e.InstanceObject != null && !e.IsDead) ||
            EnemySupportList.Exists (e => e != null && e.InstanceObject != null && !e.IsDead)
        );
    }

    public bool IsGameOver()
    {
        return !(
            AllyList.Exists (a => a != null && a.InstanceObject != null && !a.IsDead) || 
            AllySupportList.Exists (a => a != null && a.InstanceObject != null && !a.IsDead)
        );
    }

    public bool IsTurnOver()
    {
        return (LimitTurn.HasValue && LimitTurn.Value <= TurnCount);
    }

    public void CloseBattle()
    {
        IsProgressing = false;
        Sync ();
    }

    /// <summary>
    /// 復帰時にバトルスタート直後かを確認するため
    /// </summary>
    /// <returns><c>true</c> if this instance is wave start; otherwise, <c>false</c>.</returns>
    public bool IsBattleStart()
    {
        return AllyParameterList.Sum (x => x.TotalTurnCount) + EnemyParameterList.Sum (x => x.TotalTurnCount) <= 0;
    }

    /// <summary>
    /// 復帰時にWaveスタート直後かを確認するため
    /// </summary>
    /// <returns><c>true</c> if this instance is wave start; otherwise, <c>false</c>.</returns>
    public bool IsWaveStart()
    {
        return AllyParameterList.Sum (x => x.WaveTurnCount) + EnemyParameterList.Sum (x => x.WaveTurnCount) <= 0;
    }

#region Create Parameter List
    // 敵パラメータリスト作成.
    private void MakeEnemyParameterList(int waveNumber)
    {
        this.EnemyParameterList.Clear();

        var waveSetting = StageWaveSettings.FirstOrDefault (x => x.wave_count == waveNumber);
        Formation formation = waveSetting.formation;
        this.EnemyFormationLevel = waveSetting.formation_level;
        this.EnemyFormation = formation;

        // TODO: サーバーから取得するデータに置き換える。
        if (BattleEntryData == null) {
            var enemyList = this.StageEnemy.Where (x => x.wave_num == waveNumber);
            // 出撃番号は1~9である
            for (int pos = 1; pos <= 9; ++pos) {
                var enemy = enemyList.FirstOrDefault (x => x.number == pos);
                if (enemy != null) {
                    this.EnemyParameterList.Add (new BattleLogic.Parameter (
                        enemy,
                        enemy.number,
                        formation.GetPostionRow (enemy.number),
                        formation.GetPostionColumn (enemy.number),
                        null
                    ));
                }
            }
        } else {
            var count = BattleEntryData.StageEnemyList.Length;
            for (int i = 0; i < count; ++i) {
                var enemyEntry = BattleEntryData.StageEnemyList[i];
                var enemy = GetStageEnemy (enemyEntry.EnemyId);
                if (enemy == null) {
                    Debug.LogErrorFormat ("StageID = {0} not found Enemy ID = {1} ", BattleEntryData.StageId, enemyEntry.EnemyId);
                    continue;
                }
                if (enemy.wave_num == waveNumber) {
                    this.EnemyParameterList.Add (new BattleLogic.Parameter (
                        enemy,
                        enemy.number,
                        (enemy.number > 9) ? -1 : formation.GetPostionRow (enemy.number),
                        (enemy.number > 9) ? -1 : formation.GetPostionColumn (enemy.number),
                        enemyEntry.DropItemList
                    ));
                }
            }
        }

        // 陣形が発動するかを判定
        foreach(var unit in AwsModule.BattleData.EnemyParameterList) {
            var skill = formation.GetPositionSkill(unit.PositionIndex);
            if(skill != null) {
                // 陣形が効果発動するかを判定
                if(formation.SatisfyTheCondition(unit)) {
                    // 陣形スキルをパッシブスキルとして追加
                    unit.AddPassiveSkill(skill, this.EnemyFormationLevel);
                }
            }
        }
    }
    // 味方側ユニットパラメータリスト作成.
    private void MakePlayerParameterList()
    {
        // TODO: 実際は編成情報から生成する。
        var party = AwsModule.PartyData.CurrentTeam;
        this.AllyParameterList.Clear();
        this.AllyFormationLevel = party.FormationData.FormationLevel;
        this.AllyFormation = party.FormationData.Formation;

        for(int i = 0; i < Party.PartyCardMax; ++i) {
            int position = i + 1;
            if (party [position] != null) {
                this.AllyParameterList.Add (new BattleLogic.Parameter (
                    party [position],
                    position,
                    this.AllyFormation.GetPostionRow (position),
                    this.AllyFormation.GetPostionColumn (position),
                    true
                ));
            }
        }
        if (BattleEntryData != null && BattleEntryData.SupporterCardData != null) {
            this.AllyParameterList.Add (new BattleLogic.Parameter (
                BattleEntryData.SupporterCardData,
                10,
                -1,
                -1,
                true
            ));
        }

        // 陣形が発動するかを判定
        foreach(var unit in AwsModule.BattleData.AllyParameterList) {
            var skill = this.AllyFormation.GetPositionSkill(unit.PositionIndex);
            if(skill != null) {
                // 陣形が効果発動するかを判定
                if(this.AllyFormation.SatisfyTheCondition(unit)) {
                    // 陣形スキルをパッシブスキルとして追加
                    unit.AddPassiveSkill(skill, this.AllyFormationLevel);
                }
            }
        }
    }
    // PVP用ユニットパラメータリスト作成.
    private void MakePvpParameterList(PvpTeamData teamData, bool isPlayer)
    {
        List<BattleLogic.Parameter> parameterList = null;
        int formationLevel = 0;
        Formation formation = null;
        if (isPlayer) {
            parameterList = AllyParameterList;
            formationLevel = this.AllyFormationLevel = teamData.FormationLevel;
            formation = this.AllyFormation = teamData.Formation;
        } else {
            parameterList = EnemyParameterList;
            formationLevel = this.EnemyFormationLevel = teamData.FormationLevel;
            formation = this.EnemyFormation = teamData.Formation;
        }
        parameterList.Clear ();

        for(int i = 0; i < Party.PartyCardMax; ++i) {
            int position = i + 1;
            if (teamData.MemberPvpCardDataList [i] != null && teamData.MemberPvpCardDataList [i].CardId > 0) {
                parameterList.Add (new BattleLogic.Parameter (
                    teamData.MemberPvpCardDataList [i],
                    position,
                    formation.GetPostionRow (position),
                    formation.GetPostionColumn (position),
                    isPlayer
                ));
            }
        }

        // 陣形が発動するかを判定
        foreach(var unit in parameterList) {
            var skill = formation.GetPositionSkill(unit.PositionIndex);
            if(skill != null) {
                // 陣形が効果発動するかを判定
                if(formation.SatisfyTheCondition(unit)) {
                    // 陣形スキルをパッシブスキルとして追加
                    unit.AddPassiveSkill(skill, formationLevel);
                }
            }
        }
    }
#endregion

    // 敵情報の取得
    public BattleStageEnemy GetStageEnemy(int id)
    {
        return StageEnemy.FirstOrDefault (x => x.id == id);
    }

    // BattleLogic.PositionDataからListItem_BattleUnitを取得する処理
    public ListItem_BattleUnit GetBattleUnit(BattleLogic.PositionData position)
    {
        if (position.isPlayer) {
            return AllyList.FirstOrDefault (x => x.Parameter.Position == position);
        }
        return EnemyList.FirstOrDefault (x => x.Parameter.Position == position);
    }

    // コンストラクタ.
    public AwsBattleProgressData(CognitoSyncManager mng) : base(mng, "BattleProgressData")
    {
    }

    // 全値のリセット.
    protected override void ClearValues()
    {
        BattleSpeed = 1;
        GameOverProc();
    }
}
