using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(QuestAchievementFormatter))]
	public partial class QuestAchievement
	{
		// response
		public int QuestType;
		public int QuestId;
		public bool IsAchieved;
		public int[] AchievedSelectionIdList;
		public int[] AchievedMissionIdList;
		public int ReceivedMissionRewardCount;
		public int ForceLockStatus;

		class QuestAchievementFormatter : IMessagePackFormatter<QuestAchievement>
		{
			public int Serialize(ref byte[] bytes, int offset, QuestAchievement value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 7);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "QuestType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.QuestType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "QuestId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.QuestId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsAchieved");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsAchieved);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "AchievedSelectionIdList");
				if(value.AchievedSelectionIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.AchievedSelectionIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.AchievedSelectionIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "AchievedMissionIdList");
				if(value.AchievedMissionIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.AchievedMissionIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.AchievedMissionIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ReceivedMissionRewardCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ReceivedMissionRewardCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ForceLockStatus");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ForceLockStatus);
				return offset - startOffset;
			}

			public QuestAchievement Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				QuestAchievement ret = new QuestAchievement();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "QuestType") {
						ret.QuestType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "QuestId") {
						ret.QuestId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsAchieved") {
						ret.IsAchieved = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "AchievedSelectionIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.AchievedSelectionIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.AchievedSelectionIdList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.AchievedSelectionIdList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
						isRead = true;
					}
					if (key == "AchievedMissionIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.AchievedMissionIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.AchievedMissionIdList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.AchievedMissionIdList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
						isRead = true;
					}
					if (key == "ReceivedMissionRewardCount") {
						ret.ReceivedMissionRewardCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ForceLockStatus") {
						ret.ForceLockStatus = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
