﻿using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveBattlesGetOpenBattleEntryDataFormatter))]
	public partial class ReceiveBattlesGetOpenBattleEntryData : BaseReceiveAPI
	{
		// response
		public BattleEntryData BattleEntryData;

		class ReceiveBattlesGetOpenBattleEntryDataFormatter : IMessagePackFormatter<ReceiveBattlesGetOpenBattleEntryData>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveBattlesGetOpenBattleEntryData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 3);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattleEntryData");
				if(value.BattleEntryData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<BattleEntryData> ().Serialize (ref bytes, offset, value.BattleEntryData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				return offset - startOffset;
			}

			public ReceiveBattlesGetOpenBattleEntryData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveBattlesGetOpenBattleEntryData ret = new ReceiveBattlesGetOpenBattleEntryData();

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
					if (key == "BattleEntryData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.BattleEntryData = null;
						} else {
							ret.BattleEntryData = formatterResolver.GetFormatter<BattleEntryData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "MasterVersion") {
						ret.MasterVersion = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
