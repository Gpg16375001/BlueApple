using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(PvpUserDataFormatter))]
	public partial class PvpUserData
	{
		// response
		public int BattlePoint;
		public string BattlePointFullDate;
		public int BattlePointTimeToFull;
		public int ContestId;
		public int WinCount;
		public int LoseCount;
		public int ConsecutiveWins;
		public int WinningPoint;
		public int ListUpdateGem;

		class PvpUserDataFormatter : IMessagePackFormatter<PvpUserData>
		{
			public int Serialize(ref byte[] bytes, int offset, PvpUserData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 9);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BattlePoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePointFullDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.BattlePointFullDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePointTimeToFull");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BattlePointTimeToFull);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ContestId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ContestId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WinCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WinCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LoseCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LoseCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConsecutiveWins");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ConsecutiveWins);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ListUpdateGem");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ListUpdateGem);
				return offset - startOffset;
			}

			public PvpUserData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				PvpUserData ret = new PvpUserData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "BattlePoint") {
						ret.BattlePoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BattlePointFullDate") {
						ret.BattlePointFullDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BattlePointTimeToFull") {
						ret.BattlePointTimeToFull = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
					if (key == "ConsecutiveWins") {
						ret.ConsecutiveWins = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "WinningPoint") {
						ret.WinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ListUpdateGem") {
						ret.ListUpdateGem = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
