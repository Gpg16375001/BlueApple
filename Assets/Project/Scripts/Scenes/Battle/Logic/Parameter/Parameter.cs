using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

using SmileLab.Net.API;

namespace BattleLogic {
    /// <summary>
    /// バトルで使用するユニット情報管理クラス
    /// </summary>
    [Serializable]
    public class Parameter : ISaveParameter<Parameter.SaveParameter> {
        private bool _isCard;
        /// <summary> カードデータであるか？ </summary>
        public bool IsCard {
            get {
                return _isCard;
            }
        }

        public int ID {
            get {
                if (IsCard) {
                    return CardMasterData.id;
                } else {
                    if (CardMasterData != null) {
                        return CardMasterData.id;
                    }
                    return MonsterData.id;
                }
            }
        }

        public int ResourceID {
            get {
                if (IsCard) {
                    return CardMasterData.id;
                } else {
                    if (CardMasterData != null) {
                        return CardMasterData.id;
                    }
                    if (MonsterData.resource_id.HasValue) {
                        return MonsterData.resource_id.Value;
                    }
                    return MonsterData.id;
                }
            }
        }

        public bool ResourceIsCard {
            get {
                if (IsCard) {
                    return true;
                } else {
                    if (CardMasterData != null) {
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary> ユニット名 </summary>
        public string Name {
            get {
                if (CharacterData != null) {
                    return CardMasterData.nickname;
                } else {
                    return MonsterData.name;
                }
            }
        }

        public string VoiceFileName {
            get {
                if (CardMasterData != null) {
                    return CardMasterData.voice_sheet_name;
                } else {
                    return MonsterData.voice_sheet_name;
                }
            }
        }

        /// <summary> ユニットの属性 </summary>
        public Element Element {
            get {
                if (CharacterData != null) {
                    return CardMasterData.element;
                } else {
                    return MonsterData.element;
                }
            }
        }

        public Belonging belonging {
            get {
                if (CharacterData != null) {
                    return CharacterData.belonging;
                } else {
                    return MonsterData.belonging;
                }
            }
        }

        public Family family {
            get {
                if (CharacterData != null) {
                    return CharacterData.family;
                } else {
                    return MonsterData.family;
                }
            }
        }

        public Gender gender {
            get {
                if (CharacterData != null) {
                    return CharacterData.gender;
                } else {
                    return MonsterData.gender;
                }
            }
        }

        public int MotionType {
            get {
                if (Weapon != null) {
                    return Weapon.weapon.motion_type;
                }
                return 0;
            }
        }

        public bool IsBoss {
            get {
                if (EnemyData != null) {
                    return EnemyData.is_boss;
                }
                return false;
            }
        }


        private int _originalPositionIndex;
        /// <summary>
        /// 初期のポジション番号
        /// 10以上は控えとして処理され番号が若い順にユニットが死んだ場所に出て行く。
        /// </summary>
        public int OriginalPositionIndex {
            get {
                return _originalPositionIndex;
            }
        }

        private int _positionIndex;
        /// <summary>
        /// ポジション番号
        /// 10以上は控えとして処理され番号が若い順にユニットが死んだ場所に出て行く。
        /// </summary>
        public int PositionIndex {
            get {
                return _positionIndex;
            }
        }

        public bool IsPositionChanged
        {
            get {
                return OriginalPositionIndex != PositionIndex;
            }
        }

        public BattleAIDefine AIDefine {
            get {
                if (EnemyData != null) {
                    return MasterDataTable.battle_ai_define[EnemyData.ai_setting];
                }
                return null;
            }
        }

        private PositionData _position;
        /// <summary> 配置情報 </summary>
        public PositionData Position {
            get {
                return _position;
            } 
        }

        private int _level;
        /// <summary> レベル </summary>
        public int Level {
            get {
                return _level;
            }
        }

        /// <summary> 攻撃回数 </summary>
        public int AttackCount {
            get {
                // TODO: 仕様が明確ではないが複数回攻撃は存在するとの事なのでここから攻撃回数を取得するようにしておく
                // とりあえず1を返す
                return 1;
            }
        }

        private int _Hp;
        /// <summary> 現在HP </summary>
        public int Hp {
            get {
                return _Hp;
            }
            set {
                int prevHp = _Hp;
                _Hp = Mathf.Clamp(value, 0, MaxHp);

                // Hp関連のロジックがあれば
                if (prevHp != _Hp && _invokePassiveSkillList != null && _invokePassiveSkillList.Any (x => x != null && x.HasHpLogic)) {
                    isDirtyPussive = true;
                }
            }
        }

        public float HpProgress {
            get {
                return (float)Hp / (float)MaxHp;
            }
        }
            
        private int _Sp;
        /// <summary> 現在SP </summary>
        public int Sp {
            get {
                return _Sp;
            }
            private set {
                _Sp = Mathf.Clamp(value, 0, MaxSp);
            }
        }

        public float SpProgress {
            get {
                return (float)Sp / (float)MaxSp;
            }
        }

        public bool IsSpMax {
            get {
                return _Sp >= MaxSp;
            }
        }
            
        private int _TotalTurnCount;
        public int TotalTurnCount {
            get {
                return _TotalTurnCount;
            }
        }
            
        private int _WaveTurnCount;
        public int WaveTurnCount {
            get {
                return _WaveTurnCount;
            }
        }

        private int _DamageCount;
        public int DamageCount {
            get {
                return _DamageCount;
            }
        }

        private int _DamageGivenCount;
        public int DamageGivenCount {
            get {
                return _DamageGivenCount;
            }
        }

        /// <summary> 最大HPベース </summary>
        private int BaseMaxHp;

        /// <summary> 攻撃力ベース </summary>
        private int BaseAttack;

        /// <summary> 防御力ベース </summary>
        private int BaseDefense;

        /// <summary> 速さベース </summary>
        private int BaseAgility;

        /// <summary> 運ベース </summary>
        private int BaseLuck;

        /// <summary> 命中ベース </summary>
        public int BaseHit {
            get {
                return (int)(Math.Pow (Agility / 300.0f, 0.5f) * 100.0f);
            }
        }

        /// <summary> 回避ベース </summary>
        public int BaseEvasion {
            get {
                return (int)(Math.Pow (Agility / 300.0f, 0.5f) * 100.0f);
            }
        }

        /// <summary> クリティカルベース </summary>
        public int BaseCritical {
            get {
                return (int)(Math.Pow (Luck, 0.4f));
            }
        }

        private bool isDirtyPussive = false;

        private int _MaxHp;
        /// <summary> 最大HP </summary>
        public int MaxHp { 
            get {
                return _MaxHp;
            }
        }
            
        public int _MaxSp = 100;
        /// <summary> 最大SP </summary>
        public int MaxSp {
            get {
                return _MaxSp;
            }
        }
            
        private int _Attack;
        /// <summary> 攻撃力 </summary>
        public int Attack { 
            get {
                return _Attack;
            }
        }
            
        private int _Defense;
        /// <summary> 防御力 </summary>
        public int Defense { 
            get { 
                return _Defense;
            }
        }
            
        private int _Agility;
        /// <summary> 速さ </summary>
        public int Agility { 
            get {
                return _Agility;
            }
        }
            
        private int _Luck;
        /// <summary> 運 </summary>
        public int Luck { 
            get { 
                return _Luck;
            }
        }

        public int Combat {
            get {
                return Mathf.FloorToInt(MaxHp + (Attack + Defense) * 1.2f + Agility * 20 + Luck * 30);
            }
        }

        private int _Hit;
        /// <summary> 命中 </summary>
        public int Hit {
            get {
                return _Hit;
            }
        }

        private int _Evasion;
        /// <summary> 回避 </summary>
        public int Evasion {
            get {
                return _Evasion;
            }
        }

        private int _Critical;
        public int Critical {
            get {
                return _Critical;
            }
        }

        /// <summary> 戦闘時の実際のパラメータ </summary>

        private int _RealAttack;
        /// <summary> 戦闘時攻撃力 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealAttack { 
            get {
                return _RealAttack;
            }
        }

        /// <summary> 戦闘時SP攻撃力 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealSpAttack {
            get {
                return CalcSituationPussiveParameter (RealAttack, SkillTargetParameterEnum.SPAttack);
            }
        }

        private int _RealDefense;
        /// <summary> 戦闘時防御力 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealDefense { 
            get { 
                return _RealDefense;
            }
        }

        private int _RealAgility;
        /// <summary> 戦闘時速さ 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealAgility { 
            get {
                return _RealAgility;
            }
        }

        private int _RealLuck;
        /// <summary> 戦闘時運 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealLuck { 
            get { 
                return _RealLuck;
            }
        }

        /// <summary> 戦闘時命中ベース 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealBaseHit {
            get {
                return (int)(Math.Pow (RealAgility / 300.0f, 0.5f) * 100.0f);
            }
        }

        /// <summary> 戦闘時回避ベース 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealBaseEvasion {
            get {
                return (int)(Math.Pow (RealAgility / 300.0f, 0.5f) * 100.0f);
            }
        }

        /// <summary> 戦闘時クリティカルベース 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealBaseCritical {
            get {
                return (int)(Math.Pow (RealLuck, 0.4f));
            }
        }
            
        private int _RealHit;
        /// <summary> 戦闘時命中 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealHit {
            get {
                return _RealHit;
            }
        }
            
        private int _RealEvasion;
        /// <summary> 戦闘時回避 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealEvasion {
            get {
                return _RealEvasion;
            }
        }

        private int _RealCritical;
        /// <summary> 戦闘時クリティカル率 計算中しか正しい値が入らないので計算処理部以外での参照禁止 </summary>
        public int RealCritical {
            get {
                return _RealCritical;
            }
        }


        private int _Weight;
        /// <summary> 次行動順までの重さ </summary>
        public int Weight { 
            get {
                return _Weight;
            } 
            set {
                _Weight = Mathf.Max (value, 0);
            }
        }

        /// <summary> このカードがもつアクションスキル情報 </summary>
        private SkillParameter[] _unitActionSkillList;
        public SkillParameter[] UnitActionSkillList {
            get{
                return _unitActionSkillList;
            }
        }

        /// <summary> 武器なども含めた全てのアクションスキル情報 </summary>
        private SkillParameter[] _actionSkillList;
        public SkillParameter[] ActionSkillList {
            get{
                return _actionSkillList;
            }
        }

        /// <summary> このカードがもつパッシブスキル情報 </summary>
        private SkillParameter[] _unitPassiveSkillList;
        public SkillParameter[] UnitPassiveSkillList {
            get{
                return _unitPassiveSkillList;
            }
        }
        /// <summary> パッシブスキル情報 </summary>
        private SkillParameter[] _passiveSkillList;
        public SkillParameter[] PassiveSkillList { 
            get {
                return _passiveSkillList;
            }
        }

        private PassiveSkillParameter[] _invokePassiveSkillList;
        public PassiveSkillParameter[] InvokePassiveSkillList { 
            get {
                return _invokePassiveSkillList;
            }
        }
        /// <summary> スペシャルスキル情報 </summary>
        private SkillParameter _specialSkill;
        public SkillParameter SpecialSkill {
            get{
                return _specialSkill;
            }
        }

        private SkillParameter _normalSkill;
        public SkillParameter NormalSkill {
            get{
                return _normalSkill;
            }
        }

        /// <summary> 装備武器パラメータ </summary>
        private WeaponParameter _weapon;
        public WeaponParameter Weapon {
            get{
                return _weapon;
            }
        }

        /// <summary> マギカイトパラメータ </summary>
        private MagikiteParameter[] _magikite;
        public MagikiteParameter[] Magikite {
            get {
                return _magikite;
            }
        }

        private ConditionParameterList _conditions;
        public ConditionParameterList Conditions {
            get {
                return _conditions;
            }
        }

        private ItemData[] _dropItems;
        public ItemData[] DropItems {
            get {
                return _dropItems;
            }
        }

        private BattleAI _battleAI;
        public BattleAI BattleAI {
            get {
                if (_battleAI == null) {
                    _battleAI = new BattleAI (this);
                }
                return _battleAI;
            }
        }

        public bool HasDropItem {
            get {
                return _dropItems != null && _dropItems.Length > 0;
            }
        }

        /// <summary> ガード状態 </summary>
        public bool IsGuard { get; private set; }

        /// <summary> ガード行動値 </summary>
        public int GuardAddValue { get; private set; }

        /// <summary> ガード行動倍率 </summary>
        public int GuardRatioValue { get; private set; }

        // カード情報をキャッシュ
        private CardCard CardMasterData;
        // キャラクター情報をキャッシュ
        private CardCharacter CharacterData;
        // 敵情報をキャッシュ
        private BattleStageEnemy EnemyData;
        // モンスター情報をキャッシュ
        private EnemyMonster MonsterData;

        // TODO: マスターで設定できるようにすべかかな？
        const int DefaultWeight = 4000;

        private int _growthBoardAcLv = 0;
        private int _growthBoardSpLv = 0;
        private int _growthBoardPsLv = 0;

        public Parameter()
        {
        }

        /// <summary>
        /// サーバーから受け取ったデータでParameterの作成を行う。
        /// </summary>
        /// <param name="cardData">Card data.</param>
        /// <param name="positionIndex">Position index.</param>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        /// <param name="isPlayer">If set to <c>true</c> is player.</param>
        public Parameter(CardData cardData, int positionIndex, int row, int column, bool isPlayer, bool isPvp = false)
        {
            var weapon = cardData.Weapon;
            InitPlayer (cardData.Card, cardData.Level, 
                weapon != null ? weapon.WeaponId : cardData.Card.initial_weapon_id, 
                weapon != null ? weapon.Level : 1, 
                weapon != null ? weapon.LimitBreakGrade : 0, 
                cardData.EquippedMagikiteBagIdList.Select(x => MagikiteData.CacheGet(x)).ToArray(),
                cardData.BoardDataList,
                positionIndex, row, column, isPlayer, isPvp);
        }

        public Parameter(PvpCardData pvpCardData, int positionIndex, int row, int column, bool isPlayer)
        {
            var weapon = pvpCardData.EquippedWeaponData;
            bool eqpuippedWeapon = weapon != null && weapon.Weapon != null;
            InitPlayer (pvpCardData.Card, pvpCardData.Level, 
                eqpuippedWeapon ? weapon.WeaponId : pvpCardData.Card.initial_weapon_id, 
                eqpuippedWeapon ? weapon.Level : 1, 
                eqpuippedWeapon ? weapon.LimitBreakGrade : 0, 
                pvpCardData.EquippedMagikiteDataList,
                pvpCardData.BoardDataList,
                positionIndex, row, column, isPlayer, true);
        }

        public Parameter(SupporterCardData supportCardData, int positionIndex, int row, int column, bool isPlayer)
        {
            var weapon = supportCardData.EquippedWeaponData;
            bool eqpuippedWeapon = weapon != null && weapon.Weapon != null;
            InitPlayer (supportCardData.Card, supportCardData.Level, 
                eqpuippedWeapon ? weapon.WeaponId : supportCardData.Card.initial_weapon_id, 
                eqpuippedWeapon ? weapon.Level : 1, 
                eqpuippedWeapon ? weapon.LimitBreakGrade : 0, 
                supportCardData.EquippedMagikiteDataList,
                supportCardData.BoardDataList,
                positionIndex, row, column, isPlayer, false);
        }

        /// <summary>
        /// プレイヤー側ユニット情報の作成
        /// </summary>
        /// <param name="cardData">カード情報</param>
        /// <param name="level">レベル</param>
        public Parameter(CardCard cardData, int level, int positionIndex, int row, int column, bool isPlayer, bool isPvp = false)
        {
            InitPlayer (cardData, level, cardData.initial_weapon_id, 1, 0, null, null, positionIndex, row, column, isPlayer, isPvp);
        }

        private void InitPlayer(CardCard cardData, int level, int weaponID, int weaponLevel, int weaponLimitBreakGrade, MagikiteData[] magikiteDataList, BoardData[] boardDataList, int positionIndex, int row, int column, bool isPlayer, bool isPvp)
        {
            isDirtyPussive = true;

            _isCard = true;
            CardMasterData = cardData;
            CharacterData = cardData.character;
            EnemyData = null;
            MonsterData = null;

            // 武器パラメータの初期化
            _weapon = new WeaponParameter(weaponID, weaponLevel, weaponLimitBreakGrade);

            // 育成ボード
            var activeSlots = GetGrowthBoardSlot(boardDataList);
            var growthBoardHp = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.hp);
            var growthBoardAtk = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.attack);
            var growthBoardDef = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.defence);
            var growthBoardAgi = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.agility);
            _growthBoardAcLv = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.action_skill_level);
            _growthBoardSpLv = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.special_skill_level);
            _growthBoardPsLv = CalcGrowthBoardSlot (activeSlots, MaterialGrowthBoardParameterTypeEnum.passive_skill_level);

            // マギカイトパラメータの初期化
            if (magikiteDataList != null && magikiteDataList.Length > 0) {
                List<MagikiteParameter> magikiteParames = new List<MagikiteParameter> ();
                foreach (var magikiteData in magikiteDataList) {
                    if (magikiteData != null) {
                        if (magikiteData.MagikiteId > 0) {
                            magikiteParames.Add (new MagikiteParameter (magikiteData));
                        }
                    }
                }
                _magikite = magikiteParames.ToArray ();
            } else {
                _magikite = new MagikiteParameter[0];
            }

            _level = level;
            var ratiryData = MasterDataTable.card_rarity [cardData.rarity];
            int maxLevel = ratiryData.max_level;
            int coefficient = maxLevel / 20; //ratiryData.coefficient;
            // ベースパラメータの計算
            BaseMaxHp = 99999999;// CalcParameter (cardData.initial_hp, cardData.max_hp, _level, maxLevel, coefficient, cardData.hp_growth_type) + Weapon.Hp + _magikite.Select(x => x.Hp).Sum() + growthBoardHp;
            BaseAttack = 99999999;//CalcParameter (cardData.initial_attack, cardData.max_attack, _level, maxLevel, coefficient, cardData.attack_growth_type) + Weapon.Attack + _magikite.Select(x => x.Attack).Sum() + growthBoardAtk;
            BaseDefense = 99999999;//CalcParameter (cardData.initial_defence, cardData.max_defence, _level, maxLevel, coefficient, cardData.defence_growth_type) + Weapon.Defense + _magikite.Select(x => x.Defense).Sum() + growthBoardDef;
            BaseAgility = CalcParameter (cardData.initial_agility, cardData.max_agility, _level, maxLevel, coefficient, cardData.agility_growth_type) + Weapon.Agility + _magikite.Select(x => x.Agility).Sum() + growthBoardAgi;
            BaseLuck = 99999999;//cardData.luck;

            // TODO: あとで実際の取得情報から生成するように修正する
            var skillList = MasterDataTable.card_acquire_skill.DataList.
                Where (
                    x => x.card_id == cardData.id &&
                    x.acquire_level <= level &&
                    MasterDataTable.skill [x.skill_id] != null
                ).
                Select (
                    x => {
                        int skillLevel = 999999;//1;
                        var skill = MasterDataTable.skill [x.skill_id];
                        if(skill.skill_type == SkillTypeEnum.special) {
                            skillLevel += _growthBoardSpLv;
                        } else if(skill.skill_type == SkillTypeEnum.passive) {
                            skillLevel += _growthBoardPsLv;
                        } else if(skill.skill_type == SkillTypeEnum.action && !skill.is_normal_action) {
                            skillLevel += _growthBoardAcLv;
                        }
                        return new SkillParameter (skillLevel, skill);
                    }
                );
            _unitActionSkillList = skillList.Where (x => x.IsAction).ToArray();
            _actionSkillList = _unitActionSkillList.
                Concat(Weapon.ActionSkillList).
                ToArray();
            _unitPassiveSkillList = skillList.Where (x => x.IsPassive).ToArray();
            _passiveSkillList = _unitPassiveSkillList.
                Concat(Weapon.PassiveSkillList).
                Concat(_magikite.SelectMany(x => x.PassiveSkillList)).
                ToArray();
            _specialSkill = skillList.FirstOrDefault (x => x.IsSpecial);
            _normalSkill = _actionSkillList.FirstOrDefault (x => x.IsNormalAction);

            _invokePassiveSkillList = new PassiveSkillParameter[0];
            _conditions = new ConditionParameterList ();

            RecalcParameterVariation ();
            Hp = MaxHp;
            _MaxSp = 0;
            Sp = 0;

            _dropItems = null;

            _position = new PositionData ();
            _position.isPlayer = isPlayer;
            _position.row = row;
            _position.column = column;
            _position.UnitSizeID = 0;
            _positionIndex = positionIndex;
            _originalPositionIndex = positionIndex;
            _TotalTurnCount = 0;
            _WaveTurnCount = 0;


            if (isPvp) {
                // Pvpの時はBaseのDEF0.7倍
                BaseDefense = Mathf.FloorToInt((float)BaseDefense * 0.5f);
            }
            ResetWeight ();
        }

        /// <summary>
        /// 敵側ユニット情報の作成
        /// </summary>
        /// <param name="enemyData">敵情報</param>
        public Parameter(BattleStageEnemy enemyData, int positionIndex, int row, int column, ItemData[] dropItem)
        {
            isDirtyPussive = true;

            _isCard = false;
            EnemyData = enemyData;
            if (enemyData.card != null) {
                MonsterData = null;
                CardMasterData = EnemyData.card;
                CharacterData = CardMasterData.character;
            } else {
                MonsterData = enemyData.monster;
                CardMasterData = null;
                CharacterData = null;
            }

            // 武器パラメータの初期化
            if (enemyData.equip_weapon.HasValue) {
                _weapon = new WeaponParameter (enemyData.equip_weapon.Value, 1, 1);
            }

            var table = enemyData.parameter_table;
            BaseMaxHp = CalcParameter (table.hp, enemyData.parameter_correction) + ((Weapon != null) ? Weapon.Hp : 0);
            BaseAttack = CalcParameter (table.attack, enemyData.parameter_correction) + ((Weapon != null) ? Weapon.Attack : 0);
            BaseDefense = CalcParameter (table.defence, enemyData.parameter_correction) + ((Weapon != null) ? Weapon.Defense : 0);
            BaseAgility = CalcParameter (table.agility, enemyData.parameter_correction) + ((Weapon != null) ? Weapon.Agility : 0);
            BaseLuck = enemyData.luck;

            var skillList = MasterDataTable.enemy_skill_setting.DataList.
                Where(x => x.group_id == enemyData.skill_group_id).
                Select(x => x.CreatePlayerSkill());
            _unitActionSkillList = _actionSkillList = skillList.Where(x => x.IsAction).ToArray();
            _passiveSkillList = skillList.Where(x => x.IsPassive).ToArray();
            _invokePassiveSkillList = new PassiveSkillParameter[0];
            _specialSkill = skillList.FirstOrDefault (x => x.IsSpecial);
            _normalSkill = _actionSkillList.FirstOrDefault (x => x.IsNormalAction);

            _conditions = new ConditionParameterList ();

            RecalcParameterVariation ();
            Hp = MaxHp;
            _MaxSp = 100;
            Sp = 0;

            _dropItems = dropItem;

            _position = new PositionData ();
            _position.isPlayer = false;
            _position.row = row;
            _position.column = column;
            _position.UnitSizeID = MonsterData != null && MonsterData.size.HasValue ? MonsterData.size.Value : 0;
            _positionIndex = positionIndex;
            _originalPositionIndex = positionIndex;

            _TotalTurnCount = 0;
            _WaveTurnCount = 0;

            ResetWeight ();
        }
		// ダミー生成用.
        public Parameter(BattleEnemyParameter enemyParameter, EnemyMonster monster, int parameterCollection, int luck, int skillGroupId, int positionIndex, int row, int column, int weaponId = -1)
        {
            isDirtyPussive = true;

            _isCard = false;
            CardMasterData = null;
            CharacterData = null;
            EnemyData = null;
            MonsterData = monster;

            // 武器パラメータの初期化
            if (weaponId > 0) {
                _weapon = new WeaponParameter(weaponId, 1, 1);
            }
            BaseMaxHp = CalcParameter(enemyParameter.hp, parameterCollection) + ((Weapon != null) ? Weapon.Hp : 0);
            BaseAttack = CalcParameter(enemyParameter.attack, parameterCollection) + ((Weapon != null) ? Weapon.Attack : 0);
            BaseDefense = CalcParameter(enemyParameter.defence, parameterCollection) + ((Weapon != null) ? Weapon.Defense : 0);
            BaseAgility = CalcParameter(enemyParameter.agility, parameterCollection) + ((Weapon != null) ? Weapon.Agility : 0);
            BaseLuck = luck;

            var skillList = MasterDataTable.enemy_skill_setting.DataList.
                Where(x => x.group_id == skillGroupId).
                Select(x => x.CreatePlayerSkill());
            _unitActionSkillList = _actionSkillList = skillList.Where(x => x.IsAction).ToArray();
            _passiveSkillList = skillList.Where(x => x.IsPassive).ToArray();
            _invokePassiveSkillList = new PassiveSkillParameter[0];
            _specialSkill = skillList.FirstOrDefault(x => x.IsSpecial);
            _normalSkill = _actionSkillList.FirstOrDefault(x => x.IsNormalAction);

            _conditions = new ConditionParameterList();

            RecalcParameterVariation();
            Hp = MaxHp;
            _MaxSp = 100;
            Sp = 0;

            _dropItems = null;

            _position = new PositionData();
            _position.isPlayer = false;
            _position.row = row;
            _position.column = column;
            _position.UnitSizeID = MonsterData != null && MonsterData.size.HasValue ? MonsterData.size.Value : 0;
            _positionIndex = positionIndex;
            _originalPositionIndex = positionIndex;

            _TotalTurnCount = 0;
            _WaveTurnCount = 0;

            ResetWeight();
        }
            
        /// <summary>
        /// Weightを初期化する
        /// </summary>
        public void ResetWeight()
        {
            Weight = DefaultWeight / Agility;
        }

        /// <summary>
        /// 次の行動までのWeightを設定する
        /// </summary>
        /// <param name="skillWeight">重さ</param>
        public void SetWeight(int skillWeight)
        {
            Weight = CalcWeight(skillWeight);
        }

        /// <summary>
        /// 次の行動までのWeightに加算する
        /// </summary>
        /// <param name="weight">重さ</param>
        public void AddWeight(int weight)
        {
            Weight += CalcWeight(weight) ;
        }

        /// <summary>
        /// 次の行動までのWeightを計算する
        /// </summary>
        /// <param name="skillWeight">重さ</param>
        public int CalcWeight(int weight)
        {
            return weight / Agility;
        }

        public void SetWeightDirect(int weight)
        {
            Weight = weight;
        }

        /// <summary>
        /// 攻撃された回数をカウント
        /// </summary>
        /// <param name="count">Count.</param>
        public void AddDamageCount(int count = 1)
        {
            _DamageCount += count;
        }

        /// <summary>
        /// 攻撃した回数をカウント
        /// </summary>
        /// <param name="count">Count.</param>
        public void AddDamageGivenCount(int count = 1)
        {
            _DamageGivenCount += count;
        }

        // 行動終了時変更処理
        public void Actioned()
        {
            // 状態の更新を行う
            Conditions.Actioned (this);
            Conditions.RemoveDisableConditions (this);
        }

        public void ConditionElapsed(int time)
        {
            // 状態の更新を行う
            Conditions.Elapsed (this, time);
            Conditions.RemoveDisableConditions (this);
        }

        public void ConditionCallingOffAction()
        {
            Conditions.CallingOffAction (this);
        }

        public void ConditionCallingOffDamage()
        {
            Conditions.CallingOffDamage (this);
        }

        /// <summary>
        /// ガード状態へ遷移させる。
        /// </summary>
        /// <param name="addValue">防御行動値</param>
        /// <param name="racioValue">防御行動倍率</param>
        public void Guard(int addValue, int ratioValue)
        {
            IsGuard = true;
            GuardAddValue = addValue;
            GuardRatioValue = ratioValue;
        }

        /// <summary>
        /// ガード状態を解除する。
        /// </summary>
        public void ReleaseGuard()
        {
            IsGuard = false;
            GuardAddValue = 0;
            GuardRatioValue = 0;
        }
            
        /// <summary>
        /// パッシブスキルを追加する。
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <param name="level">level.</param>
        public void AddPassiveSkill(Skill skill, int level)
        {
            var skillParam = new SkillParameter (level, skill);
            foreach (var skillEffect in skill.SkillEffects) {
                AddPassiveSkill (null, skillParam, skillEffect.skill_effect);
            }
        }

        /// <summary>
        /// パッシブスキルを追加する。
        /// RecalcParameterVariationを必ず呼び出す。
        /// </summary>
        /// <param name="skillParameter">スキル情報</param>
        public void AddPassiveSkill(ConditionParameter condition, SkillParameter skillParameter, SkillEffect invokeEffect)
        {
            if (skillParameter.Skill.skill_type != SkillTypeEnum.passive) {
                return;
            }
                
            int length = _invokePassiveSkillList.Length;
            int count = _invokePassiveSkillList.Count (x => x != null);
            // 配列長が足りない場合は配列をResizeする。
            if (length <= count) {
                Array.Resize (ref _invokePassiveSkillList, length > 0 ? length * 2 : 4);
            }
            int emptyIndex = Array.FindIndex(_invokePassiveSkillList, x => x == null);
            _invokePassiveSkillList[emptyIndex] = new PassiveSkillParameter(this, condition, skillParameter, invokeEffect);

            isDirtyPussive = true;
        }

        /// <summary>
        /// パッシブスキルを削除する。
        /// RecalcParameterVariationを必ず呼び出す。
        /// </summary>
        /// <param name="skillParameter">スキル情報</param>
        public void RemovePassiveSkill(SkillParameter skillParameter)
        {
            int index = Array.FindIndex(_invokePassiveSkillList, x => x != null && x.ParentSkill == skillParameter);
            if (index >= 0) {
                _invokePassiveSkillList[index] = null;
                isDirtyPussive = true;
            }
        }

        public void ResetPassiveSkill()
        {
            var count = _invokePassiveSkillList.Length;
            for (int i = 0; i < count; ++i) {
                _invokePassiveSkillList [i] = null;
            }
        }


        /// <summary>
        /// 完全防御が有効化判断する。
        /// </summary>
        /// <returns><c>true</c>, if perfect guard was enabled, <c>false</c> otherwise.</returns>
        /// <param name="isEvaluation">If set to <c>true</c> is evaluation.</param>
        public bool EnablePerfectGuard(bool isEvaluation=false)
        {
            var index = Array.FindIndex (_invokePassiveSkillList, x => x != null && x.HasPerfectGuardLogic);
            if (index >= 0) {
                var passiveSkill = _invokePassiveSkillList [index];
                if (passiveSkill.EnableCondition()) {
                    if (!isEvaluation) {
                        passiveSkill.AddExecuteCount ();
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 絶対回避が有効か判断する。
        /// </summary>
        /// <returns><c>true</c>, if perfect guard was enabled, <c>false</c> otherwise.</returns>
        /// <param name="isEvaluation">If set to <c>true</c> is evaluation.</param>
        public bool EnableAbsoluteAvoidance(bool isEvaluation=false)
        {
            var index = Array.FindIndex (_invokePassiveSkillList, x => x != null && x.HasAbsoluteAvoidanceLogic);
            if (index >= 0) {
                var passiveSkill = _invokePassiveSkillList [index];
                if (passiveSkill.EnableCondition()) {
                    if (!isEvaluation) {
                        passiveSkill.AddExecuteCount ();
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 完全防御が有効化判断する。
        /// </summary>
        /// <returns><c>true</c>, if perfect guard was enabled, <c>false</c> otherwise.</returns>
        /// <param name="isEvaluation">If set to <c>true</c> is evaluation.</param>
        public bool EnableGuts(int damage, bool isEvaluation=false)
        {
            if (Hp > 1 && damage >= Hp) {
                var index = Array.FindIndex (_invokePassiveSkillList, x => x != null && x.HasGutsLogic);
                if (index >= 0) {
                    var passiveSkill = _invokePassiveSkillList [index];
                    if (passiveSkill.EnableCondition ()) {
                        if (!isEvaluation) {
                            passiveSkill.AddExecuteCount ();
                        }
                        return true;
                    }
                }
            }
            return false;
        }


        public bool EnableCounter(int damage)
        {
            var skills = _invokePassiveSkillList.Where(x => x != null && x.HasCounterLogic);
            foreach (var skill in skills) {
                if (!skill.EnableCondition ()) {
                    continue;
                }
                bool deadInvoke = false;
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.DeadInvoke)) {
                    deadInvoke = skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.DeadInvoke) != 0;
                }
                if (!deadInvoke && Hp <= damage) {
                    continue;
                }
                return true;
            }
            return false;
        }

        public bool EnableReverseDamage(int damage)
        {
            var skills = _invokePassiveSkillList.Where(x => x != null && x.HasCounterLogic);
            foreach (var skill in skills) {
                if (!skill.EnableCondition ()) {
                    continue;
                }
                bool deadInvoke = false;
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.DeadInvoke)) {
                    deadInvoke = skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.DeadInvoke) != 0;
                }
                if (!deadInvoke && Hp <= damage) {
                    continue;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 状態の追加
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void AddCondition(Condition condition)
        {
            _conditions.AddCondition (this, condition);

            RecalcParameterVariation ();
        }

        /// <summary>
        /// 状態の削除
        /// </summary>
        public Condition[] RemoveCondition(int? conditionID, int? conditionGroup, ConditionTypeEnum? type)
        {
            var ret = _conditions.RemoveConditions (this, conditionID, conditionGroup, type);

            RecalcParameterVariation ();
            return ret;
        }

        /// <summary>
        /// 該当する状態があるか
        /// </summary>
        public bool HasCondition(int? conditionID, int? conditionGroup, ConditionTypeEnum? type)
        {
            return _conditions.ContainsConditions (this, conditionID, conditionGroup, type);
        }

        public void Resurrection()
        {
            // Hpを最大にする。
            Hp = MaxHp;
            // 状態異常とデバフは取り除く
            RemoveCondition (null, null, ConditionTypeEnum.AbnormalState);
            RemoveCondition (null, null, ConditionTypeEnum.Debuff);

            ResetPassiveSkill ();

            _conditions.AddWithInSkill (this);
        }


        Dictionary<SkillTargetParameterEnum, ChangeParameterElement> ParameterVariation;
        /// <summary>
        /// パッシブスキルの影響を受けたパラメータの計算を行う。
        /// パッシブスキルの更新がある場合は必ず呼び出すこと
        /// </summary>
        public void RecalcParameterVariation ()
        {
            // フラグが立っていないなら何もしない
            if (!isDirtyPussive) {
                return;
            }

            ParameterVariation = Calculation.CalcParameterVariation (this);

            _MaxHp = CalcPussiveParameter(BaseMaxHp, SkillTargetParameterEnum.Hp);
            _Attack = CalcPussiveParameter(BaseAttack, SkillTargetParameterEnum.Attack);
            _Defense = CalcPussiveParameter(BaseDefense, SkillTargetParameterEnum.Defense);
            _Agility = Mathf.Max(CalcPussiveParameter(BaseAgility, SkillTargetParameterEnum.Agility), 1);
            _Luck = CalcPussiveParameter(BaseLuck, SkillTargetParameterEnum.Luck);

            _Hit = CalcPussiveParameter(BaseHit, SkillTargetParameterEnum.Hit);
            _Evasion = CalcPussiveParameter(BaseEvasion, SkillTargetParameterEnum.Evasion);
            _Critical = CalcPussiveParameter(BaseCritical, SkillTargetParameterEnum.Critical);

            isDirtyPussive = false;
        }

        private int CalcPussiveParameter(int baseValue, SkillTargetParameterEnum target)
        {
            ChangeParameterElement variation = null;
            ParameterVariation.TryGetValue (target, out variation);
            return baseValue + variation;
        }

        Dictionary<SkillTargetParameterEnum, ChangeParameterElement> SituationParameterVariation;
        /// <summary>
        /// パッシブスキルの影響を受けたパラメータの計算を行う。
        /// </summary>
        public void CalcSituationParameterVariation (bool isAttacker, SkillParameter action, Parameter target)
        {
            if (isDirtyPussive || ParameterVariation == null) {
                RecalcParameterVariation();
            }

            SituationParameterVariation = Calculation.CalcSituationParameterVariation (this, isAttacker, action, target);

            if (SituationParameterVariation.Count <= 0) {
                _RealAttack = _Attack;
                _RealDefense = _Defense;
                _RealAgility = _Agility;
                _RealLuck = _Luck;

                _RealHit = _Hit;
                _RealEvasion = _Evasion;
                _RealCritical = _Critical;

                return;
            }

            _RealAttack = CalcSituationPussiveParameter(BaseAttack, SkillTargetParameterEnum.Attack);
            _RealDefense = CalcSituationPussiveParameter(BaseDefense, SkillTargetParameterEnum.Defense);
            _RealAgility = Mathf.Max(CalcSituationPussiveParameter(BaseAgility, SkillTargetParameterEnum.Agility), 1);
            _RealLuck = CalcSituationPussiveParameter(BaseLuck, SkillTargetParameterEnum.Luck);

            _RealHit = CalcSituationPussiveParameter(RealBaseHit, SkillTargetParameterEnum.Hit);
            _RealEvasion = CalcSituationPussiveParameter(RealBaseEvasion, SkillTargetParameterEnum.Evasion);
            _RealCritical = CalcSituationPussiveParameter(RealBaseCritical, SkillTargetParameterEnum.Critical);
        }

        private int CalcSituationPussiveParameter(int baseValue, SkillTargetParameterEnum target, bool isNagative=false)
        {
            ChangeParameterElement variation = null;
            ChangeParameterElement situationVariation = null;
            if (ParameterVariation != null) {
                ParameterVariation.TryGetValue (target, out variation);
            }
            if (SituationParameterVariation != null) {
                SituationParameterVariation.TryGetValue (target, out situationVariation);
            }

            if (isNagative) {
                return baseValue - (variation + situationVariation);
            }
            return baseValue + (variation + situationVariation);
        }

        private ChangeParameterElement GetSituationPussiveChangeParameterElement(SkillTargetParameterEnum target, bool isNagative=false)
        {
            ChangeParameterElement variation = null;
            ChangeParameterElement situationVariation = null;
            if (ParameterVariation != null) {
                ParameterVariation.TryGetValue (target, out variation);
            }
            if (SituationParameterVariation != null) {
                SituationParameterVariation.TryGetValue (target, out situationVariation);
            }
            return variation + situationVariation;
        }

        public List<MaterialGrowthBoardSlot> GetGrowthBoardSlot(BoardData[] boardDataList)
        {
            if (!IsCard || boardDataList == null || boardDataList.Length <= 0) {
                return null;
            }

            var setting = MasterDataTable.card_growth_board_setting [CardMasterData.id];
            if (setting == null) {
                return null;
            }

            int pattern_id = setting.growth_board_pattern_id;

            List<MaterialGrowthBoardSlot> ret = new List<MaterialGrowthBoardSlot> ();
            foreach (var boardData in boardDataList) {
                if (!boardData.IsAvailable || boardData.UnlockedSlotList.Length <= 0) {
                    continue;
                }

                foreach(var slotIndex in boardData.UnlockedSlotList) {
                    var slotData = MasterDataTable.material_growth_board_slot [pattern_id, boardData.Index, slotIndex];
                    if (slotData != null) {
                        ret.Add (slotData);
                    }
                }
            }

            return ret;
        }

        public static int CalcGrowthBoardSlot(IEnumerable<MaterialGrowthBoardSlot> slotList, MaterialGrowthBoardParameterTypeEnum type)
        {
            if (slotList == null || slotList.Count() <= 0) {
                return 0;
            }
            return slotList.Where (x => x.parameter_type == type).Sum (x => x.parameter_value);
        }

        public void ResetSp()
        {
            Sp = 0;
        }

        public void ChargeSp(int value, bool ignorePussive)
        {
            if (ignorePussive) {
                Sp += value;
            } else {
                Sp += CalcSituationPussiveParameter (value, SkillTargetParameterEnum.SPCharge);
            }
        }

        public int CalcChargeSp(int value, bool ignorePussive)
        {
            if (ignorePussive) {
                return value;
            } 
            return CalcSituationPussiveParameter (value, SkillTargetParameterEnum.SPCharge);
        }

        public int AddDamage(int damage)
        {
            return CalcSituationPussiveParameter (damage, SkillTargetParameterEnum.AddDamage, false);
        }

        public int DecreaseDamage(int damage)
        {
            return CalcSituationPussiveParameter (damage, SkillTargetParameterEnum.DecreaseDamage, true);
        }

        public int HealValue(int heal, bool ignorePussive)
        {
            if (ignorePussive) {
                return heal;
            }
            return CalcSituationPussiveParameter (heal, SkillTargetParameterEnum.HealValue);
        }

        public int CriticalDamageRate(int rate)
        {
            return CalcSituationPussiveParameter (rate, SkillTargetParameterEnum.CriticalDamageRate);
        }

        public int GetHate()
        {
            return CalcSituationPussiveParameter (0, SkillTargetParameterEnum.Hate);
        }

        public ChangeParameterElement GetElementRateChangeParameter()
        {
            return GetSituationPussiveChangeParameterElement (SkillTargetParameterEnum.ElementDamageRate);
        }

        public int StateGrantResistance(int probability, Condition condition)
        {
            return Calculation.CalcStateGrantResistance(this, probability, condition);
        }

        public ChangeParameterElement StateEffectChange(Condition condition)
        {
            return Calculation.CalcStateEffectChange(this, condition);
        }

        public void SetPosition(int index, PositionData position)
        {
            _positionIndex = index;
            _position.isPlayer = position.isPlayer;
            _position.row = position.row;
            _position.column = position.column;
            _position.UnitSizeID = position.UnitSizeID;
        }

        public void SetPosition(int index, bool isPlayer, int row, int column, int enemyUnitSizeID)
        {
            _positionIndex = index;
            _position.isPlayer = isPlayer;
            _position.row = row;
            _position.column = column;
            _position.UnitSizeID = enemyUnitSizeID;
        }


        public void AddTurnCount()
        {
            _TotalTurnCount++;
            _WaveTurnCount++;
        }

        public void WaveChange()
        {
            _WaveTurnCount = 0;

            _conditions.WaveChange (this);

            RecalcParameterVariation ();
        }

        #region static parameter calc methods
        /// <summary>
        /// プレイヤー側パラメータ計算
        /// </summary>
        /// <returns>パラメータ値</returns>
        /// <param name="init_value">初期値</param>
        /// <param name="max_value">最大値</param>
        /// <param name="level">現在レベル</param>
        /// <param name="growthType">成長タイプ</param>
        static private int CalcParameter(int init_value, int max_value, int level, int maxLevel, int coefficient, GrowthTypeEnum growthType)
        {
            switch (growthType) {
            case GrowthTypeEnum.normal:
                return Normal ((double)init_value, (double)max_value, (double)level, (double)maxLevel);
            case GrowthTypeEnum.early:
                return Early ((double)init_value, (double)max_value, (double)level, (double)maxLevel, (double)coefficient);
            case GrowthTypeEnum.late:
                return Late ((double)init_value, (double)max_value, (double)level, (double)maxLevel, (double)coefficient);
            }

            return init_value;
        }

        /// <summary>
        /// 敵側パラメータ計算
        /// </summary>
        /// <returns>パラメータ値</returns>
        /// <param name="value">基礎値</param>
        /// <param name="parameter_correction">補正値(±百分率)</param>
        static private int CalcParameter(int value, int parameter_correction)
        {
            var random = UnityEngine.Random.Range ((float)parameter_correction * -1f, (float)parameter_correction);
            double t = (double)random / 100D;
            return (int)((double)value + (double)value * t);
        }

        /// <summary>
        /// クランプ関数
        /// </summary>
        /// <param name="t">値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        static private double Clamp(double t, double min, double max)
        {
            return Math.Max (min, Math.Min (max, t));
        }

        /// <summary>
        /// 0~1クランプ関数
        /// </summary>
        /// <param name="t">値</param>
        static private double Clamp01(double t)
        {
            return Clamp(t, 0D, 1D);
        }

        /// <summary>
        /// 普通成長パラメータ計算
        /// </summary>
        /// <param name="initValue">初期値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="now">現在レベル</param>
        /// <param name="max">最大レベル</param>
        static private int Normal(double initValue, double maxValue, double now, double max)
        {
            var ret = initValue + (maxValue - initValue) * Clamp01 (now / max);
            return (int)(Math.Floor(ret));
        }

        /// <summary>
        /// 早熟成長タイプパラメータ計算
        /// </summary>
        /// <param name="initValue">初期値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="now">現在レベル</param>
        /// <param name="max">最大レベル</param>
        static private int Early(double initValue, double maxValue, double now, double max, double coefficient)
        {
            // INT(P1*((11-T)/10)+(((P2-P1)/LMax)*(((1000-((P2-P1)/T))^(1/6))*T))*(L^(1/2.05)))
            double sub = ( maxValue - initValue );
            var ret = initValue * ((11D - coefficient) / 10D) +
                (
                    (sub / max) * 
                    (Math.Pow(1000D - (sub / coefficient), 1D / 6D) * coefficient) *
                    Math.Pow(now, (1D / 2.05D))
                );
            return (int)(Math.Floor(ret));
        }

        /// <summary>
        /// 晩成成長タイプパラメータ計算
        /// </summary>
        /// <param name="initValue">初期値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="now">現在レベル</param>
        /// <param name="max">最大レベル</param>
        static private int Late(double initValue, double maxValue, double now, double max, double coefficient)
        {
            // =INT(初期値+((レベル^(33/16))/((最大値-初期値)/(初期値*1.5))))
            double sub = ( maxValue - initValue );
            var ret = (
                initValue + 
                (
                    Math.Pow(now, (1.14D + (2D - Math.Pow(coefficient, (1D/11D))))) /
                    (sub / (initValue * ((coefficient / 3.7D))))
                )
            );
            return (int)(Math.Floor(ret));
        }
        #endregion

        [Serializable]
        public class SaveParameter
        {
            public bool IsCard;
            public int ID;
            public int Rarity;
            public int Level;
            public int OriginalPositionIndex;
            public int PositionIndex;
            public PositionData Position;
            public int BaseMaxHp;
            public int BaseAttack;
            public int BaseDefense;
            public int BaseAgility;
            public int BaseLuck;
            public int GrowthBoardAcLv;
            public int GrowthBoardSpLv;
            public int GrowthBoardPsLv;
            public int Hp;
            public int Sp;
            public int TotalTurnCount;
            public int WaveTurnCount;
            public int Weight;
            public SkillParameter.SaveParameter[] UnitActionSkillList;
            public bool EquipWeapon;
            public WeaponParameter.SaveParameter Weapon;
            public bool EquipMagikite;
            public MagikiteParameter.SaveParameter[] Magikite;
            public ConditionParameterList.SaveParameter Conditions;
            public bool HasDropItems;
            public ItemData[] DropItems;
            public bool IsGuard;
            public int GuardAddValue;
            public int GuardRatioValue;
            public int DamageCount;
            public int DamageGivenCount;
        }

        SaveParameter _saveData = new SaveParameter ();

        // 最新データの更新
        public bool IsModify()
        {
            return _saveData.Hp != Hp ||
                _saveData.Sp != Sp ||
                _saveData.Weight != Weight ||
                Conditions.IsModify () ||
                _saveData.PositionIndex != PositionIndex ||
                (Weapon != null && Weapon.IsModify()) ||
                _saveData.DamageGivenCount != DamageGivenCount || 
                _saveData.DamageCount != DamageCount ||
                UnitActionSkillList.Any(x => x.IsModify ());
        }


        public Parameter.SaveParameter CreateSaveData()
        {
            var saveData = _saveData;

            saveData.IsCard = IsCard;
            saveData.ID = ID;

            if (IsCard) {
                saveData.Rarity = CardMasterData.rarity;
            } else {
                saveData.ID = EnemyData.id;
            }
            saveData.Level = Level;
            saveData.OriginalPositionIndex = OriginalPositionIndex;
            saveData.PositionIndex = PositionIndex;
            saveData.Position = Position;
            // 敵側のパラメータが乱数使ってるので保存するようにしておく
            saveData.BaseMaxHp = BaseMaxHp;
            saveData.BaseAttack = BaseAttack;
            saveData.BaseDefense = BaseDefense;
            saveData.BaseAgility = BaseAgility;
            saveData.BaseLuck = BaseLuck;
            saveData.GrowthBoardAcLv = _growthBoardAcLv;
            saveData.GrowthBoardSpLv = _growthBoardSpLv;
            saveData.GrowthBoardPsLv = _growthBoardPsLv;
            saveData.Hp = Hp;
            saveData.Sp = Sp;
            saveData.TotalTurnCount = TotalTurnCount;
            saveData.WaveTurnCount = WaveTurnCount;
            saveData.Weight = Weight;
            saveData.UnitActionSkillList = new SkillParameter.SaveParameter[_unitActionSkillList.Length];
            for (int i = 0; i < _unitActionSkillList.Length; ++i) {
                saveData.UnitActionSkillList [i] = _unitActionSkillList [i].CreateSaveData ();
            }
            saveData.EquipWeapon = false;
            if (_weapon != null) {
                saveData.EquipWeapon = true;
                saveData.Weapon = _weapon.CreateSaveData ();
            }
            saveData.EquipMagikite = false;
            if (Magikite != null) {
                saveData.EquipMagikite = true;
                saveData.Magikite = new MagikiteParameter.SaveParameter[Magikite.Length];
                for (int i = 0; i < Magikite.Length; ++i) {
                    saveData.Magikite [i] = Magikite [i].CreateSaveData ();
                }
            }
            saveData.Conditions = Conditions.CreateSaveData();
            saveData.HasDropItems = false;
            if (DropItems != null) {
                saveData.HasDropItems = true;
                saveData.DropItems = DropItems;
            }
            saveData.IsGuard = IsGuard;
            saveData.GuardAddValue = GuardAddValue;
            saveData.GuardRatioValue = GuardRatioValue;
            saveData.DamageCount = DamageCount;
            saveData.DamageGivenCount = DamageGivenCount;
            return saveData;
        }

        public Parameter.SaveParameter UpdateSaveData()
        {
            var saveData = _saveData;

            saveData.Hp = Hp;
            saveData.Sp = Sp;
            saveData.TotalTurnCount = TotalTurnCount;
            saveData.WaveTurnCount = WaveTurnCount;
            saveData.Weight = Weight;
            saveData.PositionIndex = PositionIndex;
            saveData.Position = Position;
            saveData.DamageCount = DamageCount;
            saveData.DamageGivenCount = DamageGivenCount;
            if (Weapon != null && Weapon.IsModify ()) {
                Weapon.UpdateSaveData ();
            }
            if (Conditions.IsModify()) {
                Conditions.UpdateSaveData ();
            }
            int count = UnitActionSkillList.Length;
            for (int i = 0; i < count; ++i) {
                if (UnitActionSkillList [i].IsModify ()) {
                    UnitActionSkillList[i].UpdateSaveData ();
                }
            }
            return _saveData;
        }

        public void Load(Parameter.SaveParameter saveData)
        {
            _isCard = saveData.IsCard;
            _level = saveData.Level;
            if (_isCard) {
                CardMasterData = MasterDataTable.card [saveData.ID, saveData.Rarity];
                CharacterData = CardMasterData.character;
                EnemyData = null;
                MonsterData = null;
            } else {
                EnemyData = AwsModule.BattleData.GetStageEnemy (saveData.ID);
                if (EnemyData.card != null) {
                    MonsterData = null;
                    CardMasterData = EnemyData.card;
                    CharacterData = CardMasterData.character;
                } else {
                    MonsterData = EnemyData.monster;
                    CardMasterData = null;
                    CharacterData = null;
                }

            }

            _originalPositionIndex = saveData.OriginalPositionIndex;
            _positionIndex = saveData.PositionIndex;
            _position = saveData.Position;
            BaseMaxHp = saveData.BaseMaxHp;
            BaseAttack = saveData.BaseAttack;
            BaseDefense = saveData.BaseDefense;
            BaseAgility = saveData.BaseAgility;
            BaseLuck = saveData.BaseLuck;
            _Hp = saveData.Hp;
            _Sp = saveData.Sp;
            _MaxSp = 100;
            _TotalTurnCount = saveData.TotalTurnCount;
            _WaveTurnCount = saveData.WaveTurnCount;
            _Weight = saveData.Weight;
            _unitActionSkillList = new SkillParameter[saveData.UnitActionSkillList.Length];
            for (int i = 0; i < _unitActionSkillList.Length; ++i) {
                _unitActionSkillList [i] = new SkillParameter ();
                _unitActionSkillList [i].Load(saveData.UnitActionSkillList [i]);
            }
            _weapon = null;
            if (saveData.EquipWeapon) {
                _weapon = new WeaponParameter ();
                _weapon.Load (saveData.Weapon);
            }
            _magikite = null;
            if (saveData.EquipMagikite) {
                _magikite = new MagikiteParameter[saveData.Magikite.Length];
                for (int i = 0; i < _magikite.Length; ++i) {
                    _magikite [i] = new MagikiteParameter ();
                    _magikite [i].Load (saveData.Magikite [i]);
                }
            }
            _dropItems = null;
            if (saveData.HasDropItems) {
                _dropItems = saveData.DropItems;
            }
            _conditions = new ConditionParameterList();
            _conditions.Load (saveData.Conditions);

            IsGuard = saveData.IsGuard;
            GuardAddValue = saveData.GuardAddValue;
            GuardRatioValue = saveData.GuardRatioValue;
            _growthBoardAcLv = saveData.GrowthBoardAcLv;
            _growthBoardSpLv = saveData.GrowthBoardSpLv;
            _growthBoardPsLv = saveData.GrowthBoardPsLv;

            _DamageCount = saveData.DamageCount;
            _DamageGivenCount = saveData.DamageGivenCount;

            // スキル情報を復元
            if (_isCard) {
                var skillList = MasterDataTable.card_acquire_skill.DataList.
                    Where (
                        x => x.card_id == CardMasterData.id &&
                        x.acquire_level <= _level &&
                        MasterDataTable.skill [x.skill_id] != null
                    ).
                    Select (
                        x => {
                            int skillLevel = 1;
                            var skill = MasterDataTable.skill [x.skill_id];
                            if(skill.skill_type == SkillTypeEnum.special) {
                                skillLevel += _growthBoardSpLv;
                            } else if(skill.skill_type == SkillTypeEnum.passive) {
                                skillLevel += _growthBoardPsLv;
                            } else if(skill.skill_type == SkillTypeEnum.action && !skill.is_normal_action) {
                                skillLevel += _growthBoardAcLv;
                            }
                            return new SkillParameter (skillLevel, skill);
                        }
                    );
                _actionSkillList = _unitActionSkillList.
                    Concat(Weapon.ActionSkillList).
                    ToArray();
                _passiveSkillList = skillList.Where(x => x.IsPassive).
                    Concat(Weapon.PassiveSkillList).
                    Concat(_magikite.SelectMany(x => x.PassiveSkillList)).
                    ToArray();
                _specialSkill = skillList.FirstOrDefault (x => x.IsSpecial);
                _normalSkill = _actionSkillList.FirstOrDefault (x => x.IsNormalAction);
            } else {
                var skillList = MasterDataTable.enemy_skill_setting.DataList
                    .Where (x => x.group_id == EnemyData.skill_group_id)
                    .Select (x => x.CreatePlayerSkill ());
                _actionSkillList = _unitActionSkillList;
                _passiveSkillList = skillList.Where (x => x.IsPassive).ToArray ();
                _specialSkill = skillList.FirstOrDefault (x => x.IsSpecial);
                _normalSkill = _actionSkillList.FirstOrDefault (x => x.IsNormalAction);
            }
            _invokePassiveSkillList = new PassiveSkillParameter[0];

            _saveData = saveData;
        }

        public void Reversion(Parameter unit, Parameter.SaveParameter saveData)
        {
            isDirtyPussive = true;
            _conditions.Reversion (unit, saveData.Conditions);
        }
    }
}
