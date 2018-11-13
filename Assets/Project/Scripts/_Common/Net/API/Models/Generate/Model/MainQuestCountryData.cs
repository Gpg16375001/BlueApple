using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(MainQuestCountryDataFormatter))]
	public partial class MainQuestCountryData
	{
		// response
		public int MainQuestCountry;
		public bool IsOpen;
		public bool IsNew;
		public bool IsClear;

		class MainQuestCountryDataFormatter : IMessagePackFormatter<MainQuestCountryData>
		{
			public int Serialize(ref byte[] bytes, int offset, MainQuestCountryData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 4);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MainQuestCountry");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MainQuestCountry);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsOpen");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsOpen);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsNew");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsNew);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsClear");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsClear);
				return offset - startOffset;
			}

			public MainQuestCountryData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				MainQuestCountryData ret = new MainQuestCountryData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "MainQuestCountry") {
						ret.MainQuestCountry = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsOpen") {
						ret.IsOpen = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsNew") {
						ret.IsNew = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsClear") {
						ret.IsClear = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
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
