using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace SmileLab.Net.API
{
	public partial class WeaponData
	{
		/// <summary>
		/// 武器マスター
		/// </summary>
		private Weapon _weapon;

		public Weapon Weapon
		{
			get {
				_weapon = _weapon ?? MasterDataTable.weapon[WeaponId];
				return _weapon;
			}
		}

		/// <summary>
		/// 現在レベル
		/// </summary>
		private int? level = null;

		public int _Level
		{
			get {
				if (!level.HasValue) {
					level = MasterDataTable.weapon_level.GetLevel(Weapon.level_table_id, Exp);
				}
				return level.Value;
			}
		}

		/// <summary>
		/// 武器のレアリティ.
		/// </summary>
		public int _Rarity
		{
			get {
				return Weapon.rarity.rarity;
			}
		}

		/// <summary>
		/// 売却時の価格.
		/// </summary>
		public int Price
		{
			get {
				return MasterDataTable.weapon_rarity[Rarity].price;
			}
		}

        /// <summary>
        /// 強化時のコスト.
        /// </summary>
        public int CostUsedMaterial
		{
			get {
				return MasterDataTable.weapon_rarity[Rarity].cost;
			}
		}

        /// <summary>
        /// 素材として得られる経験値.
        /// </summary>
        public int ExpGainedMaterial
		{
			get {
				var info = MasterDataTable.weapon_rarity[Rarity];
				var rtn = info.base_exp;
				if(this.Exp > 0){
					rtn += Mathf.FloorToInt((float)this.Exp / 10f);
				}
				return rtn;
			}
		}

		/// <summary>
		/// 現在の限界突破での最大レベル
		/// </summary>
		public int CurrentLimitBreakMaxLevel
		{
			get {
				return CurrentRarityMaxLevel + (LimitBreakGrade * 10);
			}
		}

		/// <summary>
		/// 現在のレアリティでの最大レベル
		/// </summary>
		public int CurrentRarityMaxLevel
		{
			get {
				return Weapon.rarity.initial_max_level;
			}
		}

		public int NextLevelExp
		{
			get {
				return MasterDataTable.weapon_level.GetNextLevelExp(Weapon.level_table_id, Level, Exp);
			}
		}

		public int CurrentLevelExp
		{
			get {
				return MasterDataTable.weapon_level.GetCurrentLevelExp(Weapon.level_table_id, Level, Exp);
			}
		}

		public float CurrentLevelProgress
		{
			get {
				return (float)CurrentLevelExp / (float)NextLevelExp;
			}
		}

		/// <summary>CreationDateをDateTime型にパースしたもの.</summary>
		public DateTime CreationDateTime
		{
			get {
				return DateTime.Parse(CreationDate, null, DateTimeStyles.RoundtripKind);
			}
		}
		/// <summary>ModificationDateをDateTime型にパースしたもの.</summary>
		public DateTime ModificationDateTime
		{
			get {
				return DateTime.Parse(ModificationDate, null, DateTimeStyles.RoundtripKind);
			}
		}

		/// <summary>スキル持ってるか.</summary>
		public bool IsHaveSkill
		{
			get {
				var haveAction = Parameter.ActionSkillList != null && Parameter.ActionSkillList.Length > 0;
				var havePassive = Parameter.PassiveSkillList != null && Parameter.PassiveSkillList.Length > 0;
				return haveAction || havePassive;
			}
		}

		[System.NonSerialized]
		private BattleLogic.WeaponParameter _parameter;

		public BattleLogic.WeaponParameter Parameter
		{
			get {
				if (_parameter == null) {
					_parameter = new BattleLogic.WeaponParameter(WeaponId, Level, LimitBreakGrade);
				}
				return _parameter;
			}
		}

		/// <summary>
		/// 引数のCardDataを装備できるかを返す
		/// </summary>
		public bool CanEquipped(CardData card)
		{
			var settings = MasterDataTable.card_can_equipped_setting[card.CardId];
			return (!Weapon.element.HasValue || Weapon.element.Value == card.Card.element.Enum) && (settings == null || settings.Any(x => x.weapon_type == Weapon.type));
		}

		/// <summary>
		/// ダミーデータ作成用コンストラクタ.Lv1で作る.
		/// </summary>
		public WeaponData(Weapon source, bool bEquipped = false)
		{
			WeaponId = source.id;
			BagId = 0;
			Exp = 0;
            LimitBreakGrade = 0;
            Rarity = source.rarity.rarity;
            IsEquipped = bEquipped;
			IsLocked = false;
			CardId = 0;
			SlotId = 0;
			ModificationDate = GameTime.SharedInstance.Now.ToString();
			CreationDate = GameTime.SharedInstance.Now.ToString();
		}
		public WeaponData(){}
	}
}