﻿using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveQuestsGetDailyAchievementFormatter))]
	public partial class ReceiveQuestsGetDailyAchievement : BaseReceiveAPI
	{
		// response
		public DailyQuestAchievement[] DailyQuestAchievementList;

		class ReceiveQuestsGetDailyAchievementFormatter : IMessagePackFormatter<ReceiveQuestsGetDailyAchievement>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveQuestsGetDailyAchievement value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 4);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "DailyQuestAchievementList");
				if(value.DailyQuestAchievementList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.DailyQuestAchievementList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.DailyQuestAchievementList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<DailyQuestAchievement> ().Serialize (ref bytes, offset, value.DailyQuestAchievementList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceiveQuestsGetDailyAchievement Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveQuestsGetDailyAchievement ret = new ReceiveQuestsGetDailyAchievement();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "ResultCode") {
						ret.ResultCode = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "DailyQuestAchievementList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.DailyQuestAchievementList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.DailyQuestAchievementList = new DailyQuestAchievement[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.DailyQuestAchievementList[arrayIndex] = null;
								} else {
									ret.DailyQuestAchievementList[arrayIndex] = formatterResolver.GetFormatter<DailyQuestAchievement> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "MasterVersion") {
						ret.MasterVersion = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ErrorMessage") {
						ret.ErrorMessage = MessagePackBinary.ReadString(bytes, offset, out readed);
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