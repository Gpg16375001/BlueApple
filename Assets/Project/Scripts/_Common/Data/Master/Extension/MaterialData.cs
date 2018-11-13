using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab.Net.API
{ 
	public partial class MaterialData
    {

        /// <summary>
        /// 属性情報. 
        /// </summary>
        public Element Element
        {
            get {
                var mat = MasterDataTable.chara_material[MaterialId];
                return mat.element != null ? MasterDataTable.element.DataList.Find(m => m.Enum == mat.element) : null;
            }
        }

        /// <summary>
        /// キャラ強化素材情報.
        /// </summary>
		public CharaMaterial CharaMaterialInfo
		{
			get {
				return MasterDataTable.chara_material[MaterialId];
			}
		}

        /// <summary>
        /// キャラ強化素材の定義情報.
        /// </summary>
		public CharaMaterialDefine CharaMaterialDefineInfo
		{
			get {
				return MasterDataTable.chara_material_define[CharaMaterialInfo.rarity];
			}
		}

		public MaterialData(int id, int count)
		{
			MaterialId = id;
			Count = count;
			ModificationDate = GameTime.SharedInstance.Now.ToString();
			CreationDate = GameTime.SharedInstance.Now.ToString();
		}
		public MaterialData(){}
    }

    /// <summary>MaterialDataヘルパークラス.</summary>
	public static class MaterialDataHelper
	{
		/// <summary>
        /// 対象の属性から強化ポイントを取得.
        /// </summary>
		public static int GetEnhancePoint(this MaterialData self, Element targetElement)
        {
            if (self.Element == null) {
                return 0;
            }
			return self.CharaMaterialDefineInfo.GetEnhancePoint(self.Element.Enum, targetElement);
        }
	}	
}