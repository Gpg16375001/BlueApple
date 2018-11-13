using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(GemProductDataFormatter))]
	public partial class GemProductData
	{
		// response
		public int GemProductId;
		public int BonusId;
		public int PurchaseLimitation;
		public string StartDate;
		public string EndDate;
		public bool IsPurchasable;
		public string LastPurchaseDate;
		public int MaxPurchaseQuantity;

		class GemProductDataFormatter : IMessagePackFormatter<GemProductData>
		{
			public int Serialize(ref byte[] bytes, int offset, GemProductData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 8);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GemProductId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GemProductId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BonusId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BonusId);
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
				return offset - startOffset;
			}

			public GemProductData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				GemProductData ret = new GemProductData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "GemProductId") {
						ret.GemProductId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BonusId") {
						ret.BonusId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
