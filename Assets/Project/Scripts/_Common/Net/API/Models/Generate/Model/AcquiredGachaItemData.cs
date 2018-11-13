using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(AcquiredGachaItemDataFormatter))]
	public partial class AcquiredGachaItemData
	{
		// response
		public int GachaProductId;
		public int GachaId;
		public int GachaItemId;
		public int GachaGroupId;
		public int ItemType;
		public int ItemId;
		public int Quantity;
		public bool IsNew;
		public int ConvertedItemType;
		public int ConvertedItemId;
		public int ConvertedQuantity;
		public CardData CardData;
		public MagikiteData MagikiteData;
		public WeaponData WeaponData;

		class AcquiredGachaItemDataFormatter : IMessagePackFormatter<AcquiredGachaItemData>
		{
			public int Serialize(ref byte[] bytes, int offset, AcquiredGachaItemData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 14);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaProductId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaProductId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaGroupId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaGroupId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ItemType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ItemType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Quantity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Quantity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsNew");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsNew);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConvertedItemType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ConvertedItemType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConvertedItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ConvertedItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConvertedQuantity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ConvertedQuantity);
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

			public AcquiredGachaItemData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				AcquiredGachaItemData ret = new AcquiredGachaItemData();

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
					if (key == "GachaItemId") {
						ret.GachaItemId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GachaGroupId") {
						ret.GachaGroupId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
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
					if (key == "IsNew") {
						ret.IsNew = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ConvertedItemType") {
						ret.ConvertedItemType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ConvertedItemId") {
						ret.ConvertedItemId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ConvertedQuantity") {
						ret.ConvertedQuantity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
