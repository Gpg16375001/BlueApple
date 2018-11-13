using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(ConsumerDataFormatter))]
	public partial class ConsumerData
	{
		// response
		public int ConsumerId;
		public int Count;
		public string ModificationDate;
		public string CreationDate;

		class ConsumerDataFormatter : IMessagePackFormatter<ConsumerData>
		{
			public int Serialize(ref byte[] bytes, int offset, ConsumerData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 4);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConsumerId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ConsumerId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Count");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Count);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ModificationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ModificationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				return offset - startOffset;
			}

			public ConsumerData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ConsumerData ret = new ConsumerData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "ConsumerId") {
						ret.ConsumerId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Count") {
						ret.Count = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ModificationDate") {
						ret.ModificationDate = MessagePackBinary.ReadString(bytes, offset, out readed);
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
