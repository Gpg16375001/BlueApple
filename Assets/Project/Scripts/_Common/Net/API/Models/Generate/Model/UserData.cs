using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(UserDataFormatter))]
	public partial class UserData
	{
		// response
		public int UserId;
		public string CustomerId;
		public string Nickname;
		public string LastLoginDate;
		public int Level;
		public int Exp;
		public int ActionPoint;
		public string ActionPointFullDate;
		public int ActionPointTimeToFull;
		public int BattlePoint;
		public string BattlePointFullDate;
		public int BattlePointTimeToFull;
		public bool IsFollow;
		public bool IsFollower;
		public string FollowDate;
		public string FollowerDate;
		public int FollowCount;
		public int FollowerCount;
		public string Comment;
		public CardData MainCardData;
		public CardData[] SupportCardList;
		public SupporterCardData[] SupporterCardList;
		public int ReceivablePresentCount;
		public int[] ReceivableLoginbonusIdList;
		public int ReceivableMissionCount;
		public int FreeGemCount;
		public int PaidGemCount;
		public int GoldCount;
		public int FriendPointCount;
		public int PvpMedalCount;
		public int WeaponBagCapacity;
		public int MagikiteBagCapacity;
		public int RarestCardMissCount;
		public int ActionPointHealCost;
		public int BattlePointHealCost;
		public string CreationDate;
		public int GainFriendPointOnSupport;

		class UserDataFormatter : IMessagePackFormatter<UserData>
		{
			public int Serialize(ref byte[] bytes, int offset, UserData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 37);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UserId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.UserId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CustomerId");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CustomerId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Nickname");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.Nickname);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LastLoginDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.LastLoginDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Level");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Level);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Exp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Exp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ActionPoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ActionPoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ActionPointFullDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ActionPointFullDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ActionPointTimeToFull");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ActionPointTimeToFull);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePoint");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BattlePoint);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePointFullDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.BattlePointFullDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePointTimeToFull");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BattlePointTimeToFull);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsFollow");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsFollow);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsFollower");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsFollower);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FollowDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.FollowDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FollowerDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.FollowerDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FollowCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FollowCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FollowerCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FollowerCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Comment");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.Comment);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MainCardData");
				if(value.MainCardData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<CardData> ().Serialize (ref bytes, offset, value.MainCardData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SupportCardList");
				if(value.SupportCardList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.SupportCardList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.SupportCardList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<CardData> ().Serialize (ref bytes, offset, value.SupportCardList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SupporterCardList");
				if(value.SupporterCardList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.SupporterCardList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.SupporterCardList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<SupporterCardData> ().Serialize (ref bytes, offset, value.SupporterCardList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ReceivablePresentCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ReceivablePresentCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ReceivableLoginbonusIdList");
				if(value.ReceivableLoginbonusIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.ReceivableLoginbonusIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ReceivableLoginbonusIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ReceivableMissionCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ReceivableMissionCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FreeGemCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FreeGemCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "PaidGemCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.PaidGemCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GoldCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GoldCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "FriendPointCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.FriendPointCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "PvpMedalCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.PvpMedalCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WeaponBagCapacity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WeaponBagCapacity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MagikiteBagCapacity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MagikiteBagCapacity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "RarestCardMissCount");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.RarestCardMissCount);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ActionPointHealCost");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ActionPointHealCost);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePointHealCost");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BattlePointHealCost);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GainFriendPointOnSupport");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.GainFriendPointOnSupport);
				return offset - startOffset;
			}

			public UserData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				UserData ret = new UserData();

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
					if (key == "CustomerId") {
						ret.CustomerId = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Nickname") {
						ret.Nickname = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LastLoginDate") {
						ret.LastLoginDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Level") {
						ret.Level = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Exp") {
						ret.Exp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ActionPoint") {
						ret.ActionPoint = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ActionPointFullDate") {
						ret.ActionPointFullDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ActionPointTimeToFull") {
						ret.ActionPointTimeToFull = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
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
					if (key == "IsFollow") {
						ret.IsFollow = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsFollower") {
						ret.IsFollower = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FollowDate") {
						ret.FollowDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FollowerDate") {
						ret.FollowerDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FollowCount") {
						ret.FollowCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FollowerCount") {
						ret.FollowerCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Comment") {
						ret.Comment = MessagePackBinary.ReadString(bytes, offset, out readed);
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
					if (key == "SupportCardList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.SupportCardList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.SupportCardList = new CardData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.SupportCardList[arrayIndex] = null;
								} else {
									ret.SupportCardList[arrayIndex] = formatterResolver.GetFormatter<CardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "SupporterCardList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.SupporterCardList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.SupporterCardList = new SupporterCardData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.SupporterCardList[arrayIndex] = null;
								} else {
									ret.SupporterCardList[arrayIndex] = formatterResolver.GetFormatter<SupporterCardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "ReceivablePresentCount") {
						ret.ReceivablePresentCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ReceivableLoginbonusIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.ReceivableLoginbonusIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.ReceivableLoginbonusIdList = new int[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.ReceivableLoginbonusIdList[arrayIndex] = MessagePackBinary.ReadInt32(bytes, offset, out readed);
								offset += readed;
							}
						}
						isRead = true;
					}
					if (key == "ReceivableMissionCount") {
						ret.ReceivableMissionCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FreeGemCount") {
						ret.FreeGemCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "PaidGemCount") {
						ret.PaidGemCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GoldCount") {
						ret.GoldCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "FriendPointCount") {
						ret.FriendPointCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "PvpMedalCount") {
						ret.PvpMedalCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "WeaponBagCapacity") {
						ret.WeaponBagCapacity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MagikiteBagCapacity") {
						ret.MagikiteBagCapacity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "RarestCardMissCount") {
						ret.RarestCardMissCount = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ActionPointHealCost") {
						ret.ActionPointHealCost = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BattlePointHealCost") {
						ret.BattlePointHealCost = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CreationDate") {
						ret.CreationDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GainFriendPointOnSupport") {
						ret.GainFriendPointOnSupport = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
