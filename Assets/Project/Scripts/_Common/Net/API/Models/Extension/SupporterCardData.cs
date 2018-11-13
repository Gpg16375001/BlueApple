using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SmileLab.Net.API
{
    public partial class SupporterCardData
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

        /// <summary>
        /// 現在レベル
        /// </summary>
        private int? level = null;

        public int Level {
            get {
                if (!level.HasValue) {
                    level = Mathf.Min(MaxLevel, MasterDataTable.card_level.GetLevel (Card.level_table_id, Exp));
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

        public CardData ConvertCardData()
        {
            CardData card = new CardData ();
            card.CardId = CardId;
            card.Rarity = Rarity;
            card.Level = Level;
            card.Exp = Exp;
            card.BoardDataList = BoardDataList;
            card.ModificationDate = ModificationDate;
            card.CreationDate = CreationDate;
            card.SetWeaponData (EquippedWeaponData);
            card.SetParmater (Parameter);

            return card;
        }
    }
}