using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(GachaProductDataFormatter))]
	public partial class GachaProductData
	{
		// response
		public int GachaProductId;
		public int GachaId;
		public int GachaType;
		public int DrawCount;
		public int ExchangeItemType;
		public int ExchangeItemId;
		public int ExchangeQuantity;
		public int PurchaseLimitation;
		public string StartDate;
		public string EndDate;
		public bool IsPurchasable;
		public string LastPurchaseDate;

		class GachaProductDataFormatter : IMessagePackFormatter<GachaProductData>
		{
			public int Serialize(ref byte[] bytes, int offset, GachaProductData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 12);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaProductId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaProductId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "DrawCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.DrawCount);
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
				return offset - startOffset;
			}

			public GachaProductData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				GachaProductData ret = new GachaProductData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "GachaProductId") {
						ret.GachaProductId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GachaId") {
						ret.GachaId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GachaType") {
						ret.GachaType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "DrawCount") {
						ret.DrawCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
