using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab
{
    /// <summary>
    /// platformに寄らず商品情報を管理する構造体
    /// </summary>
    public struct SkuItem {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string productID;

        /// <summary>
        /// 表示名
        /// </summary>
        public string title;

        /// <summary>
        /// 詳細情報
        /// </summary>
        public string description;

        /// <summary>
        /// 通貨記号付きのフォーマット済みアイテム価格
        /// </summary>
        public string formattedPrice;

        /// <summary>
        /// 通貨コード
        /// </summary>
        public string currencyCode;

        /// <summary>
        /// 価格
        /// </summary>
        public double price;
    }
}
