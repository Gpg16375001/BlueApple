using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(DailyQuestAchievementFormatter))]
	public partial class DailyQuestAchievement
	{
		// response
		public int QuestType;
		public int QuestId;
		public int DayOfWeek;
		public int LockStatus;
		public string UnlockDate;
		public int TimeToLock;
		public bool IsAchieved;
		public int[] AchievedMissionIdList;
		public int ReceivedMissionRewardCount;

		class DailyQuestAchievementFormatter : IMessagePackFormatter<DailyQuestAchievement>
		{
			public int Serialize(ref byte[] bytes, int offset, DailyQuestAchievement value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 9);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "QuestType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.QuestType);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "QuestId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.QuestId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "DayOfWeek");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.DayOfWeek);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LockStatus");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LockStatus);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UnlockDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.UnlockDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "TimeToLock");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.TimeToLock);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsAchieved");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsAchieved);
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
				return offset - startOffset;
			}

			public DailyQuestAchievement Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				DailyQuestAchievement ret = new DailyQuestAchievement();

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
					if (key == "DayOfWeek") {
						ret.DayOfWeek = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LockStatus") {
						ret.LockStatus = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "UnlockDate") {
						ret.UnlockDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "TimeToLock") {
						ret.TimeToLock = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsAchieved") {
						ret.IsAchieved = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
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
