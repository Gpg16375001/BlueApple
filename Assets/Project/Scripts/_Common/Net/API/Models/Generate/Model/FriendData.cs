using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(FriendDataFormatter))]
	public partial class FriendData
	{
		// response
		public int UserId;
		public string CustomerId;
		public string Nickname;
		public string LastLoginDate;
		public int Level;
		public bool IsFollow;
		public bool IsFollower;
		public string FollowDate;
		public string FollowerDate;
		public int FollowCount;
		public int FollowerCount;
		public string Comment;
		public CardData MainCardData;
		public int BattlePointHealCost;

		class FriendDataFormatter : IMessagePackFormatter<FriendData>
		{
			public int Serialize(ref byte[] bytes, int offset, FriendData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 14);
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
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BattlePointHealCost");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.BattlePointHealCost);
				return offset - startOffset;
			}

			public FriendData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				FriendData ret = new FriendData();

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
					if (key == "BattlePointHealCost") {
						ret.BattlePointHealCost = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
