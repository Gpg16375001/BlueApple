using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(DistributionDataFormatter))]
	public partial class DistributionData
	{
		// response
		public string Description;
		public int ItemType;
		public int ItemId;
		public int Quantity;
		public string CreationDate;

		class DistributionDataFormatter : IMessagePackFormatter<DistributionData>
		{
			public int Serialize(ref byte[] bytes, int offset, DistributionData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 5);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Description");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.Description);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ItemType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ItemType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ItemId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ItemId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Quantity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Quantity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				return offset - startOffset;
			}

			public DistributionData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				DistributionData ret = new DistributionData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "Description") {
						ret.Description = MessagePackBinary.ReadString(bytes, offset, out readed);
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
					if (key == "CreationDate") {
						ret.CreationDate = MessagePackBinary.ReadString(bytes, offset, out readed);
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
