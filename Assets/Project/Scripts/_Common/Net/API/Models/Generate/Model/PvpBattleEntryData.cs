using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(PvpBattleEntryDataFormatter))]
	public partial class PvpBattleEntryData
	{
		// response
		public int EntryId;
		public int ContestId;
		public PvpTeamData PvpTeamData;
		public int OpponentUserId;
		public PvpTeamData OpponentPvpTeamData;

		class PvpBattleEntryDataFormatter : IMessagePackFormatter<PvpBattleEntryData>
		{
			public int Serialize(ref byte[] bytes, int offset, PvpBattleEntryData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 5);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EntryId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.EntryId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ContestId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ContestId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "PvpTeamData");
				if(value.PvpTeamData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<PvpTeamData> ().Serialize (ref bytes, offset, value.PvpTeamData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "OpponentUserId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.OpponentUserId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "OpponentPvpTeamData");
				if(value.OpponentPvpTeamData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<PvpTeamData> ().Serialize (ref bytes, offset, value.OpponentPvpTeamData, formatterResolver);
				}
				return offset - startOffset;
			}

			public PvpBattleEntryData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				PvpBattleEntryData ret = new PvpBattleEntryData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "EntryId") {
						ret.EntryId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ContestId") {
						ret.ContestId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "PvpTeamData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.PvpTeamData = null;
						} else {
							ret.PvpTeamData = formatterResolver.GetFormatter<PvpTeamData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "OpponentUserId") {
						ret.OpponentUserId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "OpponentPvpTeamData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.OpponentPvpTeamData = null;
						} else {
							ret.OpponentPvpTeamData = formatterResolver.GetFormatter<PvpTeamData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
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
