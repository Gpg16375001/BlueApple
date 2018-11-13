using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(ShopProductDataFormatter))]
	public partial class ShopProductData
	{
		// response
		public int ShopProductId;
		public int ShopItemId;
		public int ShopCategory;
		public int ExchangeItemType;
		public int ExchangeItemId;
		public int ExchangeQuantity;
		public int PurchaseLimitation;
		public string StartDate;
		public string EndDate;
		public bool IsPurchasable;
		public string LastPurchaseDate;
		public int MaxPurchaseQuantity;
		public StockItemData[] StockItemDataList;

		class ShopProductDataFormatter : IMessagePackFormatter<ShopProductData>
		{
			public int Serialize(ref byte[] bytes, int offset, ShopProductData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 13);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ShopProductId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ShopProductId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ShopItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ShopItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ShopCategory");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ShopCategory);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ExchangeItemType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ExchangeItemType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ExchangeItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ExchangeItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ExchangeQuantity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ExchangeQuantity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "PurchaseLimitation");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.PurchaseLimitation);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StartDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.StartDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EndDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.EndDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsPurchasable");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsPurchasable);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LastPurchaseDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.LastPurchaseDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MaxPurchaseQuantity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MaxPurchaseQuantity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StockItemDataList");
				if(value.StockItemDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.StockItemDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.StockItemDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<StockItemData> ().Serialize (ref bytes, offset, value.StockItemDataList[i], formatterResolver);
						}
					}
				}
				return offset - startOffset;
			}

			public ShopProductData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ShopProductData ret = new ShopProductData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "ShopProductId") {
						ret.ShopProductId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ShopItemId") {
						ret.ShopItemId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ShopCategory") {
						ret.ShopCategory = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ExchangeItemType") {
						ret.ExchangeItemType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ExchangeItemId") {
						ret.ExchangeItemId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ExchangeQuantity") {
						ret.ExchangeQuantity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "PurchaseLimitation") {
						ret.PurchaseLimitation = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "StartDate") {
						ret.StartDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "EndDate") {
						ret.EndDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsPurchasable") {
						ret.IsPurchasable = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LastPurchaseDate") {
						ret.LastPurchaseDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MaxPurchaseQuantity") {
						ret.MaxPurchaseQuantity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "StockItemDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.StockItemDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.StockItemDataList = new StockItemData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.StockItemDataList[arrayIndex] = null;
								} else {
									ret.StockItemDataList[arrayIndex] = formatterResolver.GetFormatter<StockItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if(!isRead) {
						readed = MessagePackBinary.ReadNextBlock (bytes, offset);
						offset += readed;
					}
				}
				readSize = offset - startOffset;
				return ret;
			}
		}
	}
}
