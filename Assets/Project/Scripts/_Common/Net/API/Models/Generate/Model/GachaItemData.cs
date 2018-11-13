using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(GachaItemDataFormatter))]
	public partial class GachaItemData
	{
		// response
		public int GachaItemId;
		public int GachaGroupId;
		public int ItemType;
		public int ItemId;
		public int Quantity;

		class GachaItemDataFormatter : IMessagePackFormatter<GachaItemData>
		{
			public int Serialize(ref byte[] bytes, int offset, GachaItemData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 5);
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
				return offset - startOffset;
			}

			public GachaItemData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				GachaItemData ret = new GachaItemData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
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
