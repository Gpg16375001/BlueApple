using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab.Net.API
{
	public partial class MagikiteData
    {

		/// <summary>
        /// マギカイトマスター
        /// </summary>
		private Magikite _magikite;

		public Magikite Magikite
        {
            get {
				_magikite = _magikite ?? MasterDataTable.magikite[MagikiteId];
                return _magikite;
            }
        }

        /// <summary>
        /// 売却時の価格.
        /// </summary>
        public int Price
        {
            get {
                if (Magikite != null && MasterDataTable.magikite_rarity != null) {
                    return MasterDataTable.magikite_rarity [Magikite.rarity].price;
                }
                return 0;
            }
        }

		/// <summary>
        /// ダミーデータ作成用コンストラクタ.
        /// </summary>
		public MagikiteData(Magikite source, bool bEquipped = false)
        {
			_magikite = source;
			MagikiteId = source.id;
            BagId = 0;
            IsEquipped = bEquipped;
            CardId = 0;
            SlotId = 0;
            ModificationDate = GameTime.SharedInstance.Now.ToString();
            CreationDate = GameTime.SharedInstance.Now.ToString();
        }
		public MagikiteData() { }
    }	
}
