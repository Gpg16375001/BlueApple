using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(StageEnemyDataFormatter))]
	public partial class StageEnemyData
	{
		// response
		public int EnemyId;
		public ItemData[] DropItemList;

		class StageEnemyDataFormatter : IMessagePackFormatter<StageEnemyData>
		{
			public int Serialize(ref byte[] bytes, int offset, StageEnemyData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 2);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EnemyId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.EnemyId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "DropItemList");
				if(value.DropItemList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.DropItemList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.DropItemList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<ItemData> ().Serialize (ref bytes, offset, value.DropItemList[i], formatterResolver);
						}
					}
				}
				return offset - startOffset;
			}

			public StageEnemyData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				StageEnemyData ret = new StageEnemyData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "EnemyId") {
						ret.EnemyId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "DropItemList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.DropItemList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.DropItemList = new ItemData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.DropItemList[arrayIndex] = null;
								} else {
									ret.DropItemList[arrayIndex] = formatterResolver.GetFormatter<ItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
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
