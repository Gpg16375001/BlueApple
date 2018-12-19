using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(PvpTeamDataFormatter))]
	public partial class PvpTeamData
	{
		// response
		public int UserId;
		public string Nickname;
		public CardData MainCardData;
		public CardData[] MemberCardDataList;
		public PvpCardData[] MemberPvpCardDataList;
		public int TotalOverallIndex;
		public int RivalStrength;
		public int GainWinningPoint;
		public int WinningPoint;
		public int FormationId;
		public int FormationLevel;
		public int RankCorrectionPercentage;
		public int ContestId;
		public int WinCount;
		public int LoseCount;

		class PvpTeamDataFormatter : IMessagePackFormatter<PvpTeamData>
		{
			public int Serialize(ref byte[] bytes, int offset, PvpTeamData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 15);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UserId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.UserId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Nickname");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.Nickname);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MainCardData");
				if(value.MainCardData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<CardData> ().Serialize (ref bytes, offset, value.MainCardData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MemberCardDataList");
				if(value.MemberCardDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MemberCardDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.MemberCardDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<CardData> ().Serialize (ref bytes, offset, value.MemberCardDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MemberPvpCardDataList");
				if(value.MemberPvpCardDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MemberPvpCardDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.MemberPvpCardDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<PvpCardData> ().Serialize (ref bytes, offset, value.MemberPvpCardDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "TotalOverallIndex");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.TotalOverallIndex);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RivalStrength");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RivalStrength);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainWinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainWinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FormationId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FormationId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FormationLevel");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FormationLevel);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RankCorrectionPercentage");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RankCorrectionPercentage);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ContestId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ContestId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WinCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WinCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LoseCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LoseCount);
				return offset - startOffset;
			}

			public PvpTeamData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				PvpTeamData ret = new PvpTeamData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "UserId") {
						ret.UserId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Nickname") {
						ret.Nickname = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MainCardData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MainCardData = null;
						} else {
							ret.MainCardData = formatterResolver.GetFormatter<CardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "MemberCardDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MemberCardDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MemberCardDataList = new CardData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.MemberCardDataList[arrayIndex] = null;
								} else {
									ret.MemberCardDataList[arrayIndex] = formatterResolver.GetFormatter<CardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "MemberPvpCardDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MemberPvpCardDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MemberPvpCardDataList = new PvpCardData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.MemberPvpCardDataList[arrayIndex] = null;
								} else {
									ret.MemberPvpCardDataList[arrayIndex] = formatterResolver.GetFormatter<PvpCardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "TotalOverallIndex") {
						ret.TotalOverallIndex = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "RivalStrength") {
						ret.RivalStrength = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainWinningPoint") {
						ret.GainWinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "WinningPoint") {
						ret.WinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FormationId") {
						ret.FormationId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FormationLevel") {
						ret.FormationLevel = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "RankCorrectionPercentage") {
						ret.RankCorrectionPercentage = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ContestId") {
						ret.ContestId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "WinCount") {
						ret.WinCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LoseCount") {
						ret.LoseCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
