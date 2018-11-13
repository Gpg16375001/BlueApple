using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveGachaPurchaseProductFormatter))]
	public partial class ReceiveGachaPurchaseProduct : BaseReceiveAPI
	{
		// response
		public AcquiredGachaItemData[] AcquiredGachaItemDataList;
		public UserData UserData;
		public int RarestCardMissGachaTriggerCount;
		public int RarestCardMissCount;
		public AcquiredGachaItemData RarestCardGachaItemData;
		public ConsumerData ConsumerData;

		class ReceiveGachaPurchaseProductFormatter : IMessagePackFormatter<ReceiveGachaPurchaseProduct>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveGachaPurchaseProduct value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 9);
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
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RarestCardMissGachaTriggerCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RarestCardMissGachaTriggerCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RarestCardMissCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RarestCardMissCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RarestCardGachaItemData");
				if(value.RarestCardGachaItemData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<AcquiredGachaItemData> ().Serialize (ref bytes, offset, value.RarestCardGachaItemData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConsumerData");
				if(value.ConsumerData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<ConsumerData> ().Serialize (ref bytes, offset, value.ConsumerData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceiveGachaPurchaseProduct Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveGachaPurchaseProduct ret = new ReceiveGachaPurchaseProduct();

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
					if (key == "RarestCardMissGachaTriggerCount") {
						ret.RarestCardMissGachaTriggerCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "RarestCardMissCount") {
						ret.RarestCardMissCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "RarestCardGachaItemData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.RarestCardGachaItemData = null;
						} else {
							ret.RarestCardGachaItemData = formatterResolver.GetFormatter<AcquiredGachaItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "ConsumerData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.ConsumerData = null;
						} else {
							ret.ConsumerData = formatterResolver.GetFormatter<ConsumerData> ().Deserialize (bytes, offset, formatterResolver, out readed);
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
