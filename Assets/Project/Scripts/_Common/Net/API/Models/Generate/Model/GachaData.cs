using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(GachaDataFormatter))]
	public partial class GachaData
	{
		// response
		public int GachaId;
		public int GachaType;
		public GachaItemData[] GachaItemDataList;

		class GachaDataFormatter : IMessagePackFormatter<GachaData>
		{
			public int Serialize(ref byte[] bytes, int offset, GachaData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 3);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GachaType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GachaItemDataList");
				if(value.GachaItemDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.GachaItemDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.GachaItemDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<GachaItemData> ().Serialize (ref bytes, offset, value.GachaItemDataList[i], formatterResolver);
						}
					}
				}
				return offset - startOffset;
			}

			public GachaData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				GachaData ret = new GachaData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
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
					if (key == "GachaItemDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.GachaItemDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.GachaItemDataList = new GachaItemData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.GachaItemDataList[arrayIndex] = null;
								} else {
									ret.GachaItemDataList[arrayIndex] = formatterResolver.GetFormatter<GachaItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
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
