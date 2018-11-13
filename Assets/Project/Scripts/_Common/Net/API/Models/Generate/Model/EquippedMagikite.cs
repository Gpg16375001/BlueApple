using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(EquippedMagikiteFormatter))]
	public partial class EquippedMagikite
	{
		// response
		public int CardId;
		public long[] MagikiteBagIdList;

		class EquippedMagikiteFormatter : IMessagePackFormatter<EquippedMagikite>
		{
			public int Serialize(ref byte[] bytes, int offset, EquippedMagikite value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 2);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CardId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.CardId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MagikiteBagIdList");
				if(value.MagikiteBagIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MagikiteBagIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.MagikiteBagIdList[i]);
					}
				}
				return offset - startOffset;
			}

			public EquippedMagikite Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				EquippedMagikite ret = new EquippedMagikite();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "CardId") {
						ret.CardId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MagikiteBagIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MagikiteBagIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MagikiteBagIdList = new long[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.MagikiteBagIdList[arrayIndex] = MessagePackBinary.ReadInt64(bytes, offset, out readed);
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
