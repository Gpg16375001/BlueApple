using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(MissionAchievementFormatter))]
	public partial class MissionAchievement
	{
		// response
		public int MissionId;
		public int MissionType;
		public int MissionCategory;
		public int ClearCount;
		public bool IsAchieved;
		public bool IsReceived;

		class MissionAchievementFormatter : IMessagePackFormatter<MissionAchievement>
		{
			public int Serialize(ref byte[] bytes, int offset, MissionAchievement value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 6);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MissionId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MissionId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MissionType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MissionType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MissionCategory");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MissionCategory);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ClearCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ClearCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsAchieved");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsAchieved);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsReceived");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsReceived);
				return offset - startOffset;
			}

			public MissionAchievement Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				MissionAchievement ret = new MissionAchievement();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "MissionId") {
						ret.MissionId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MissionType") {
						ret.MissionType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MissionCategory") {
						ret.MissionCategory = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ClearCount") {
						ret.ClearCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsAchieved") {
						ret.IsAchieved = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsReceived") {
						ret.IsReceived = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
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
