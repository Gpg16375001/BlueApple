using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceivePvpFinishBattleFormatter))]
	public partial class ReceivePvpFinishBattle : BaseReceiveAPI
	{
		// response
		public int BaseWinningPoint;
		public int RankCorrectedWinningPoint;
		public int ConsecutiveWinsBonusWinningPoint;
		public int WinningPoint;
		public int PvpMedal;
		public PvpUserData PvpUserData;
		public UserData UserData;

		class ReceivePvpFinishBattleFormatter : IMessagePackFormatter<ReceivePvpFinishBattle>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceivePvpFinishBattle value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 10);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BaseWinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BaseWinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RankCorrectedWinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RankCorrectedWinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConsecutiveWinsBonusWinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ConsecutiveWinsBonusWinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WinningPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WinningPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "PvpMedal");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.PvpMedal);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "PvpUserData");
				if(value.PvpUserData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<PvpUserData> ().Serialize (ref bytes, offset, value.PvpUserData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UserData");
				if(value.UserData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<UserData> ().Serialize (ref bytes, offset, value.UserData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceivePvpFinishBattle Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceivePvpFinishBattle ret = new ReceivePvpFinishBattle();

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
					if (key == "BaseWinningPoint") {
						ret.BaseWinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "RankCorrectedWinningPoint") {
						ret.RankCorrectedWinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ConsecutiveWinsBonusWinningPoint") {
						ret.ConsecutiveWinsBonusWinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "WinningPoint") {
						ret.WinningPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "PvpMedal") {
						ret.PvpMedal = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "PvpUserData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.PvpUserData = null;
						} else {
							ret.PvpUserData = formatterResolver.GetFormatter<PvpUserData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "UserData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.UserData = null;
						} else {
							ret.UserData = formatterResolver.GetFormatter<UserData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
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
