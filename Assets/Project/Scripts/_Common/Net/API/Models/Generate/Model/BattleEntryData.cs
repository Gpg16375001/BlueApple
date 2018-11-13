using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(BattleEntryDataFormatter))]
	public partial class BattleEntryData
	{
		// response
		public int EntryId;
		public int StageId;
		public int[] MemberCardIdList;
		public int SupporterUserId;
		public int SupporterCardId;
		public SupporterCardData SupporterCardData;
		public int GuestUserId;
		public int GuestCardId;
		public int[] DropItemIdList;
		public StageEnemyData[] StageEnemyList;
		public string CreationDate;
		public int Status;

		class BattleEntryDataFormatter : IMessagePackFormatter<BattleEntryData>
		{
			public int Serialize(ref byte[] bytes, int offset, BattleEntryData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 12);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EntryId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.EntryId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StageId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.StageId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MemberCardIdList");
				if(value.MemberCardIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MemberCardIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MemberCardIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SupporterUserId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.SupporterUserId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SupporterCardId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.SupporterCardId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SupporterCardData");
				if(value.SupporterCardData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<SupporterCardData> ().Serialize (ref bytes, offset, value.SupporterCardData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GuestUserId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GuestUserId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GuestCardId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GuestCardId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "DropItemIdList");
				if(value.DropItemIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.DropItemIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.DropItemIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "StageEnemyList");
				if(value.StageEnemyList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.StageEnemyList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.StageEnemyList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<StageEnemyData> ().Serialize (ref bytes, offset, value.StageEnemyList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Status");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Status);
				return offset - startOffset;
			}

			public BattleEntryData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				BattleEntryData ret = new BattleEntryData();

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
					if (key == "StageId") {
						ret.StageId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MemberCardIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MemberCardIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MemberCardIdList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.MemberCardIdList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
						isRead = true;
					}
					if (key == "SupporterUserId") {
						ret.SupporterUserId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "SupporterCardId") {
						ret.SupporterCardId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "SupporterCardData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.SupporterCardData = null;
						} else {
							ret.SupporterCardData = formatterResolver.GetFormatter<SupporterCardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "GuestUserId") {
						ret.GuestUserId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GuestCardId") {
						ret.GuestCardId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "DropItemIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.DropItemIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.DropItemIdList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.DropItemIdList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
						isRead = true;
					}
					if (key == "StageEnemyList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.StageEnemyList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.StageEnemyList = new StageEnemyData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.StageEnemyList[arrayIndex] = null;
								} else {
									ret.StageEnemyList[arrayIndex] = formatterResolver.GetFormatter<StageEnemyData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "CreationDate") {
						ret.CreationDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Status") {
						ret.Status = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
