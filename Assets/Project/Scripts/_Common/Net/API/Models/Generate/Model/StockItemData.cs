using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(StockItemDataFormatter))]
	public partial class StockItemData
	{
		// response
		public int ItemType;
		public int ItemId;
		public int Quantity;
		public int Capacity;
		public CardData CardData;
		public MagikiteData MagikiteData;
		public WeaponData WeaponData;

		class StockItemDataFormatter : IMessagePackFormatter<StockItemData>
		{
			public int Serialize(ref byte[] bytes, int offset, StockItemData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 7);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ItemType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ItemType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Quantity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Quantity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Capacity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Capacity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CardData");
				if(value.CardData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<CardData> ().Serialize (ref bytes, offset, value.CardData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MagikiteData");
				if(value.MagikiteData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<MagikiteData> ().Serialize (ref bytes, offset, value.MagikiteData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WeaponData");
				if(value.WeaponData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<WeaponData> ().Serialize (ref bytes, offset, value.WeaponData, formatterResolver);
				}
				return offset - startOffset;
			}

			public StockItemData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				StockItemData ret = new StockItemData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "ItemType") {
						ret.ItemType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ItemId") {
						ret.ItemId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Quantity") {
						ret.Quantity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Capacity") {
						ret.Capacity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CardData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.CardData = null;
						} else {
							ret.CardData = formatterResolver.GetFormatter<CardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "MagikiteData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MagikiteData = null;
						} else {
							ret.MagikiteData = formatterResolver.GetFormatter<MagikiteData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "WeaponData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.WeaponData = null;
						} else {
							ret.WeaponData = formatterResolver.GetFormatter<WeaponData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
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
