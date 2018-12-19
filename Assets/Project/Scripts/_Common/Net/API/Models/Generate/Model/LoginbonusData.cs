using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(LoginbonusDataFormatter))]
	public partial class LoginbonusData
	{
		// response
		public int LoginbonusId;
		public int LastReceivedDayCount;
		public string LastReceivedDate;
		public int[] CurrentRoundDayCountList;
		public int[] NextRoundDayCountList;
		public string StartDate;
		public string EndDate;

		class LoginbonusDataFormatter : IMessagePackFormatter<LoginbonusData>
		{
			public int Serialize(ref byte[] bytes, int offset, LoginbonusData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 7);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LoginbonusId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LoginbonusId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LastReceivedDayCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LastReceivedDayCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LastReceivedDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.LastReceivedDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CurrentRoundDayCountList");
				if(value.CurrentRoundDayCountList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.CurrentRoundDayCountList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.CurrentRoundDayCountList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "NextRoundDayCountList");
				if(value.NextRoundDayCountList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.NextRoundDayCountList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.NextRoundDayCountList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StartDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.StartDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EndDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.EndDate);
				return offset - startOffset;
			}

			public LoginbonusData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				LoginbonusData ret = new LoginbonusData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "LoginbonusId") {
						ret.LoginbonusId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LastReceivedDayCount") {
						ret.LastReceivedDayCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LastReceivedDate") {
						ret.LastReceivedDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CurrentRoundDayCountList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.CurrentRoundDayCountList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.CurrentRoundDayCountList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.CurrentRoundDayCountList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
						isRead = true;
					}
					if (key == "NextRoundDayCountList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.NextRoundDayCountList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.NextRoundDayCountList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.NextRoundDayCountList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
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
