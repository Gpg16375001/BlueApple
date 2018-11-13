using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(BoardDataFormatter))]
	public partial class BoardData
	{
		// response
		public int Index;
		public bool IsAvailable;
		public int[] UnlockedSlotList;

		class BoardDataFormatter : IMessagePackFormatter<BoardData>
		{
			public int Serialize(ref byte[] bytes, int offset, BoardData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 3);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Index");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Index);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsAvailable");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsAvailable);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UnlockedSlotList");
				if(value.UnlockedSlotList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.UnlockedSlotList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.UnlockedSlotList[i]);
					}
				}
				return offset - startOffset;
			}

			public BoardData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				BoardData ret = new BoardData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "Index") {
						ret.Index = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsAvailable") {
						ret.IsAvailable = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "UnlockedSlotList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.UnlockedSlotList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.UnlockedSlotList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.UnlockedSlotList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
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
