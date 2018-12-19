using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveQuestsCloseQuestFormatter))]
	public partial class ReceiveQuestsCloseQuest : BaseReceiveAPI
	{
		// response
		public QuestAchievement QuestAchievement;
		public int[] RewardItemIdList;
		public BattleEntryData BattleEntryData;
		public ItemData[] QuestRewardItemList;
		public ItemData[] MissionRewardItemList;
		public UserData UserData;
		public CardData[] MemberCardDataList;
		public MaterialData[] MaterialDataList;
		public ConsumerData[] ConsumerDataList;
		public WeaponData[] WeaponDataList;
		public MagikiteData[] MagikiteDataList;
		public int GainUnitExp;
		public int GainUserExp;
		public int GainGold;
		public int GainEventPoint;
		public int GainBonusEventPoint;
		public int GainFriendPoint;

		class ReceiveQuestsCloseQuestFormatter : IMessagePackFormatter<ReceiveQuestsCloseQuest>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveQuestsCloseQuest value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 20);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "QuestAchievement");
				if(value.QuestAchievement == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<QuestAchievement> ().Serialize (ref bytes, offset, value.QuestAchievement, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RewardItemIdList");
				if(value.RewardItemIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.RewardItemIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RewardItemIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattleEntryData");
				if(value.BattleEntryData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<BattleEntryData> ().Serialize (ref bytes, offset, value.BattleEntryData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "QuestRewardItemList");
				if(value.QuestRewardItemList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.QuestRewardItemList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.QuestRewardItemList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<ItemData> ().Serialize (ref bytes, offset, value.QuestRewardItemList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MissionRewardItemList");
				if(value.MissionRewardItemList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MissionRewardItemList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.MissionRewardItemList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<ItemData> ().Serialize (ref bytes, offset, value.MissionRewardItemList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UserData");
				if(value.UserData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<UserData> ().Serialize (ref bytes, offset, value.UserData, formatterResolver);
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
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MaterialDataList");
				if(value.MaterialDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MaterialDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.MaterialDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<MaterialData> ().Serialize (ref bytes, offset, value.MaterialDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ConsumerDataList");
				if(value.ConsumerDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.ConsumerDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.ConsumerDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<ConsumerData> ().Serialize (ref bytes, offset, value.ConsumerDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WeaponDataList");
				if(value.WeaponDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.WeaponDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.WeaponDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<WeaponData> ().Serialize (ref bytes, offset, value.WeaponDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MagikiteDataList");
				if(value.MagikiteDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.MagikiteDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.MagikiteDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<MagikiteData> ().Serialize (ref bytes, offset, value.MagikiteDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainUnitExp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainUnitExp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainUserExp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainUserExp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainGold");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainGold);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainEventPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainEventPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainBonusEventPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainBonusEventPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainFriendPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainFriendPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceiveQuestsCloseQuest Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveQuestsCloseQuest ret = new ReceiveQuestsCloseQuest();

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
					if (key == "QuestAchievement") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.QuestAchievement = null;
						} else {
							ret.QuestAchievement = formatterResolver.GetFormatter<QuestAchievement> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "RewardItemIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.RewardItemIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.RewardItemIdList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.RewardItemIdList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
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
					if (key == "QuestRewardItemList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.QuestRewardItemList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.QuestRewardItemList = new ItemData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.QuestRewardItemList[arrayIndex] = null;
								} else {
									ret.QuestRewardItemList[arrayIndex] = formatterResolver.GetFormatter<ItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "MissionRewardItemList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MissionRewardItemList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MissionRewardItemList = new ItemData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.MissionRewardItemList[arrayIndex] = null;
								} else {
									ret.MissionRewardItemList[arrayIndex] = formatterResolver.GetFormatter<ItemData> ().Deserialize (bytes, offset, formatterResolver, out readed);
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
					if (key == "MaterialDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MaterialDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MaterialDataList = new MaterialData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.MaterialDataList[arrayIndex] = null;
								} else {
									ret.MaterialDataList[arrayIndex] = formatterResolver.GetFormatter<MaterialData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "ConsumerDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.ConsumerDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.ConsumerDataList = new ConsumerData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.ConsumerDataList[arrayIndex] = null;
								} else {
									ret.ConsumerDataList[arrayIndex] = formatterResolver.GetFormatter<ConsumerData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "WeaponDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.WeaponDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.WeaponDataList = new WeaponData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.WeaponDataList[arrayIndex] = null;
								} else {
									ret.WeaponDataList[arrayIndex] = formatterResolver.GetFormatter<WeaponData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "MagikiteDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.MagikiteDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.MagikiteDataList = new MagikiteData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.MagikiteDataList[arrayIndex] = null;
								} else {
									ret.MagikiteDataList[arrayIndex] = formatterResolver.GetFormatter<MagikiteData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "GainUnitExp") {
						ret.GainUnitExp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainUserExp") {
						ret.GainUserExp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainGold") {
						ret.GainGold = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainEventPoint") {
						ret.GainEventPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainBonusEventPoint") {
						ret.GainBonusEventPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainFriendPoint") {
						ret.GainFriendPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
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
