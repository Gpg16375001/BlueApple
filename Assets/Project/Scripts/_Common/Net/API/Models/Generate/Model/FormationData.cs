using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(FormationDataFormatter))]
	public partial class FormationData
	{
		// response
		public int FormationId;
		public int FormationLevel;
		public string ModificationDate;
		public string CreationDate;

		class FormationDataFormatter : IMessagePackFormatter<FormationData>
		{
			public int Serialize(ref byte[] bytes, int offset, FormationData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 4);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FormationId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FormationId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FormationLevel");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FormationLevel);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ModificationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ModificationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				return offset - startOffset;
			}

			public FormationData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				FormationData ret = new FormationData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "FormationId") {
						ret.FormationId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FormationLevel") {
						ret.FormationLevel = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
