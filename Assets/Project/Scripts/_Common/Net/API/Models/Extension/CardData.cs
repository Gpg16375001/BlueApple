using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace SmileLab.Net.API
{
    public partial class CardData
    {
        /// <summary>
        /// カードマスター
        /// </summary>
        private CardCard _card;

        public CardCard Card {
            get {
                _card = _card ?? MasterDataTable.card [CardId, Rarity];
#if DEFINE_DEVELOP
                if (_card == null) {
                    _card = MasterDataTable.card [CardId];
                }
#endif
                return _card;
            }
        }

        private WeaponData _weapon;
        public WeaponData Weapon {
            get {
                if (EquippedWeaponBagId > 0 && (_weapon == null || _weapon.BagId != EquippedWeaponBagId)) {
                    _weapon = WeaponData.CacheGet (EquippedWeaponBagId);
                }
                return _weapon;
            }
        }

        /// <summary>
        /// 現在レベル
        /// </summary>
        private int? level = null;

        public int _Level {
            get {
                if (!level.HasValue) {
                    level = MasterDataTable.card_level.GetLevel (Card.level_table_id, Exp);
                }
                return level.Value;
            }
        }

        /// <summary>
        /// 最大レベル
        /// </summary>
        public int MaxLevel {
            get {
				return MasterDataTable.card_rarity [Rarity].max_level;
            }
        }

        /// <summary>
        /// 最大レベル？.
        /// </summary>
		public bool IsMaxLevel {
			get {
				return Level >= MaxLevel;
			}
		}

        public int NextLevelExp {
            get {
				return !IsMaxLevel ? MasterDataTable.card_level.GetNextLevelExp (Card.level_table_id, Level, Exp): 0;
            }
        }

        public int CurrentLevelExp {
            get {
                return MasterDataTable.card_level.GetCurrentLevelExp (Card.level_table_id, Level, Exp);
            }
        }

        public float CurrentLevelProgress {
            get {
                return (float)CurrentLevelExp / (float)NextLevelExp;
            }
        }

		public int MaxRarity {
			get {
				return MasterDataTable.card.DataList.Where(x => x.id == CardId).Max(x => x.rarity);
			}
		}

		public int MinRarity
        {
            get {
                return MasterDataTable.card.DataList.Where(x => x.id == CardId).Min(x => x.rarity);
            }
        }

        /// 何段階進化しているか.
        public int RarityGrade
		{
			get {
				return Rarity - MinRarity;
			}
		}

		public bool IsMaxRarity {
			get {
				return Rarity >= MaxRarity;
			}
		}

        public System.DateTime CreateAt {
            get {
                return System.DateTime.Parse (CreationDate);
            }
        }

        /// フレーバーテキストその2が解放されているか.
        public bool IsReleaseFlavor2
		{
			get {
				var chapter = Card.release_chapter_flavor2;
				var questList = MasterDataTable.quest_main.GetQuestList(chapter.country, chapter.chapter);
				var quest = MasterDataTable.quest_main[questList.Select(q => q.id).Max()];
				var modData = AwsModule.CardModifiedData.List.Find(m => m.CardId == CardId);
				if(modData != null){
					return modData.IsSeenReleaseFlavor2 || modData.IsNeedSeeReleaseFlavor2 || QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).Any(a => a.QuestId == quest.id);
				}
				return QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).Any(a => a.QuestId == quest.id);
			}
		}

        /// 進化できるか
		public bool IsCanEvolution()
		{
			var defineInfo = MasterDataTable.chara_material_evolution_definition[Rarity];
			if(defineInfo == null){
				return false;
			}
			
			var allList = MasterDataTable.chara_material.DataList;         
			var elementList = MasterDataTable.chara_material.GetEvolutionMaterialList(Card.element.Enum);
            var roleList = MasterDataTable.chara_material.GetEvolutionMaterialList(Card.role);
			var roleDefine = MasterDataTable.chara_material_evolution_by_role_definition[(int)Card.role];

			// 属性素材.
			if (defineInfo.element_based_evolution_material_count_1 > 0) {
                var r1ele = elementList.Find(e => e.rarity == 1);
				var mat = MaterialData.CacheGet(r1ele.id);
				if(mat == null ||  mat.Count < defineInfo.element_based_evolution_material_count_1){
					return false;
				}
            }
			if (defineInfo.element_based_evolution_material_count_2 > 0) {
                var r2ele = elementList.Find(e => e.rarity == 2);
				var mat = MaterialData.CacheGet(r2ele.id);
				if (mat == null || mat.Count < defineInfo.element_based_evolution_material_count_2) {
                    return false;
                }
            }
			if (defineInfo.element_based_evolution_material_count_3 > 0) {
                var r3ele = elementList.Find(e => e.rarity == 3);
				var mat = MaterialData.CacheGet(r3ele.id);
				if (mat == null || mat.Count < defineInfo.element_based_evolution_material_count_3) {
                    return false;
                }
            }
            // 国固有素材.
            if (defineInfo.role_based_material_count_1 > 0) {
                var r1coun = roleList.Find(c => c.rarity == 1);
				var mat = MaterialData.CacheGet(r1coun.id);
				if (mat == null || mat.Count < defineInfo.role_based_material_count_1) {
                    return false;
                }
            }
            if (defineInfo.role_based_material_count_2 > 0) {
                var r2coun = roleList.Find(e => e.rarity == 2);
				var mat = MaterialData.CacheGet(r2coun.id);
				if (mat == null || mat.Count < defineInfo.role_based_material_count_2) {
                    return false;
                }
            }
            if (defineInfo.role_based_material_count_3 > 0) {
                var r3coun = roleList.Find(e => e.rarity == 3);
				var mat = MaterialData.CacheGet(r3coun.id);
				if (mat == null || mat.Count < defineInfo.role_based_material_count_3) {
                    return false;
                }
            }
			// モンスター固有素材.
			if (defineInfo.enemy_based_material_count_1 > 0) {
                var r1mon = allList.Find(m => m.id == roleDefine.enemy_based_material_1);
				var mat = MaterialData.CacheGet(r1mon.id);
				if (mat == null || mat.Count < defineInfo.enemy_based_material_count_1) {
                    return false;
                }
            }
			if (defineInfo.enemy_based_material_count_2 > 0) {
                var r2mon = allList.Find(m => m.id == roleDefine.enemy_based_material_2);
				var mat = MaterialData.CacheGet(r2mon.id);
				if (mat == null || mat.Count < defineInfo.enemy_based_material_count_2) {
                    return false;
                }
            }
			if (defineInfo.enemy_based_material_count_3 > 0) {
                var r3mon = allList.Find(m => m.id == roleDefine.enemy_based_material_3);
				var mat = MaterialData.CacheGet(r3mon.id);
				if (mat == null || mat.Count < defineInfo.enemy_based_material_count_3) {
                    return false;
                }
            }
            // 虹素材.
			if (defineInfo.rainbow_evolution_material_count_1 > 0) {
                var rainbow = allList.Find(c => c.IsCharaEvolutionMaterial && c.element != null && c.element == MasterDataTable.element[ElementEnum.rainbow].Enum);
				var mat = MaterialData.CacheGet(rainbow.id);
				if (mat == null || mat.Count < defineInfo.rainbow_evolution_material_count_1) {
                    return false;
                }
            }
			return IsMaxLevel;
		}      

        [System.NonSerialized]
        private BattleLogic.Parameter _parameter;

        public BattleLogic.Parameter Parameter {
            get {
                if (_parameter == null) {
                    // ポジション情報は仮
                    _parameter = new BattleLogic.Parameter (this, 1, 1, 1, true);
                }
                return _parameter;
            }
        }

        /// <summary>
        /// 引数のWeaponDataを装備できるかを返す
        /// </summary>
        /// <returns><c>true</c> if this instance can equipeed the specified weapon; otherwise, <c>false</c>.</returns>
        /// <param name="weapon">Weapon.</param>
        public bool CanEquipped(WeaponData weapon)
        {
            var settings = MasterDataTable.card_can_equipped_setting [CardId];

            return (!weapon.Weapon.element.HasValue || weapon.Weapon.element.Value == Card.element.Enum) && (settings == null || settings.Any(x => x.weapon_type == weapon.Weapon.type));
        }

        /// <summary>
        /// 引数のWeaponDataの配列から装備できるWeaponDataの配列を返す。
        /// </summary>
        /// <returns><c>true</c> if this instance can equipped the specified weaponList; otherwise, <c>false</c>.</returns>
        /// <param name="weaponList">Weapon list.</param>
        public IEnumerable<WeaponData> CanEquipped(IEnumerable<WeaponData> weaponList)
        {
            foreach (var weapon in weaponList) {
                if (CanEquipped (weapon)) {
                    yield return weapon;
                }
            }
        }
  
        public void SetWeaponData(WeaponData weapon)
        {
            _weapon = weapon;
        }

        public void SetParmater(BattleLogic.Parameter parameter)
        {
            _parameter = parameter;
        }

		/// <summary>
        /// ダミーデータ作成用コンストラクタ.Lv1で作る.
        /// </summary>
		public CardData(CardCard source)
		{
			CardId = source.id;
            Exp = 0;
            Level = 1;
            LimitBreakGrade = 0;
			Rarity = source.rarity;
            EquippedWeaponBagId = 0;
			EquippedMagikiteBagIdList = new long[0];
			ModificationDate = GameTime.SharedInstance.Now.ToString();
            CreationDate = GameTime.SharedInstance.Now.ToString();
		}
        /// <summary>
        /// レベル指定でダミーデータ作成..
        /// </summary>
		public CardData(CardCard source, int lv) : this(source)
		{
			level = lv;
		}
		public CardData(){}
    }
}