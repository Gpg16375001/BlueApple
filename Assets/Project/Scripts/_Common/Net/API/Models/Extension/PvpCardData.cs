using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SmileLab.Net.API
{
    public partial class PvpCardData
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
                    level = MasterDataTable.card_level.GetLevel (Card.level_table_id, Exp);
                }
                return level.Value;
            }
        }

        public CardData ConvertCardData()
        {
            CardData card = new CardData ();
            card.CardId = CardId;
            card.Rarity = Rarity;
            card.Exp = Exp;
            card.BoardDataList = BoardDataList;
            card.EquippedWeaponBagId = EquippedWeaponData.BagId;
            card.EquippedMagikiteBagIdList = EquippedMagikiteDataList.Select(x => x.BagId).ToArray();
            card.ModificationDate = ModificationDate;
            card.CreationDate = CreationDate;

            return card;
        }
    }
}