using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(EventQuestAchievementFormatter))]
	public partial class EventQuestAchievement
	{
		// response
		public int StageDetailId;
		public int StageType;
		public bool IsAchieved;
		public int[] AchievedMissionIdList;
		public int ReceivedMissionRewardCount;
		public bool IsOpen;
		public string StartDate;
		public string EndDate;

		class EventQuestAchievementFormatter : IMessagePackFormatter<EventQuestAchievement>
		{
			public int Serialize(ref byte[] bytes, int offset, EventQuestAchievement value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 8);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StageDetailId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.StageDetailId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StageType");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.StageType);
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
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsOpen");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsOpen);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StartDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.StartDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EndDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.EndDate);
				return offset - startOffset;
			}

			public EventQuestAchievement Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				EventQuestAchievement ret = new EventQuestAchievement();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "StageDetailId") {
						ret.StageDetailId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "StageType") {
						ret.StageType = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
					if (key == "IsOpen") {
						ret.IsOpen = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "StartDate") {
						ret.StartDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "EndDate") {
						ret.EndDate = MessagePackBinary.ReadString(bytes, offset, out readed);
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
