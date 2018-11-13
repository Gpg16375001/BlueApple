﻿using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveGachaTutorialFormatter))]
	public partial class ReceiveGachaTutorial : BaseReceiveAPI
	{
		// response
		public AcquiredGachaItemData[] AcquiredGachaItemDataList;
		public UserData UserData;

		class ReceiveGachaTutorialFormatter : IMessagePackFormatter<ReceiveGachaTutorial>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveGachaTutorial value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 4);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "AcquiredGachaItemDataList");
				if(value.AcquiredGachaItemDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.AcquiredGachaItemDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.AcquiredGachaItemDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<AcquiredGachaItemData> ().Serialize (ref bytes, offset, value.AcquiredGachaItemDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UserData");
				if(value.UserData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<UserData> ().Serialize (ref bytes, offset, value.UserData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				return offset - startOffset;
			}

			public ReceiveGachaTutorial Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveGachaTutorial ret = new ReceiveGachaTutorial();

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
					if (key == "AcquiredGachaItemDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.AcquiredGachaItemDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.AcquiredGachaItemDataList = new AcquiredGachaItemData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.AcquiredGachaItemDataList[arrayIndex] = null;
								} else {
									ret.AcquiredGachaItemDataList[arrayIndex] = formatterResolver.GetFormatter<AcquiredGachaItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
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
