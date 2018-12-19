using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class UserData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS UserDataTable (
UserId INTEGER NOT NULL PRIMARY KEY,
CustomerId TEXT NOT NULL,
Nickname TEXT NOT NULL,
LastLoginDate TEXT NOT NULL,
Level INTEGER NOT NULL,
Exp INTEGER NOT NULL,
ActionPoint INTEGER NOT NULL,
ActionPointFullDate TEXT NOT NULL,
ActionPointTimeToFull INTEGER NOT NULL,
BattlePoint INTEGER NOT NULL,
BattlePointFullDate TEXT NOT NULL,
BattlePointTimeToFull INTEGER NOT NULL,
IsFollow INTEGER NOT NULL,
IsFollower INTEGER NOT NULL,
FollowDate TEXT NOT NULL,
FollowerDate TEXT NOT NULL,
FollowCount INTEGER NOT NULL,
FollowerCount INTEGER NOT NULL,
Comment TEXT NOT NULL,
MainCardData BLOB NOT NULL,
SupportCardList BLOB NOT NULL,
SupporterCardList BLOB NOT NULL,
ReceivablePresentCount INTEGER NOT NULL,
ReceivableLoginbonusIdList BLOB NOT NULL,
ReceivableMissionCount INTEGER NOT NULL,
FreeGemCount INTEGER NOT NULL,
PaidGemCount INTEGER NOT NULL,
GoldCount INTEGER NOT NULL,
FriendPointCount INTEGER NOT NULL,
PvpMedalCount INTEGER NOT NULL,
WeaponBagCapacity INTEGER NOT NULL,
MagikiteBagCapacity INTEGER NOT NULL,
RarestCardMissCount INTEGER NOT NULL,
ActionPointHealCost INTEGER NOT NULL,
BattlePointHealCost INTEGER NOT NULL,
CreationDate TEXT NOT NULL,
GainFriendPointOnSupport INTEGER NOT NULL
);",
						trans);
				}
			);
		}

		static public UserData CacheGet(int UserId)
		{
			var objs = Filter (string.Format ("UserId = {0}", UserId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<UserData>>();
		}

		static public List<UserData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<UserData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM UserDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<UserData> (builder.ToString());
		}

		public string CreateParameter()
		{
			return string.Empty;
		}

		public void ReadObject (SqliteDatabase.SqliteDatabaseReader reader, string[] columnNames)
		{
			int count = columnNames.Length;
			for (int i = 0; i < count; ++i) {
				string column = columnNames [i];
				if(column == "UserId") {
					UserId = reader.ReadInt(i);
				}
				if(column == "CustomerId") {
					CustomerId = reader.ReadString(i);
				}
				if(column == "Nickname") {
					Nickname = reader.ReadString(i);
				}
				if(column == "LastLoginDate") {
					LastLoginDate = reader.ReadString(i);
				}
				if(column == "Level") {
					Level = reader.ReadInt(i);
				}
				if(column == "Exp") {
					Exp = reader.ReadInt(i);
				}
				if(column == "ActionPoint") {
					ActionPoint = reader.ReadInt(i);
				}
				if(column == "ActionPointFullDate") {
					ActionPointFullDate = reader.ReadString(i);
				}
				if(column == "ActionPointTimeToFull") {
					ActionPointTimeToFull = reader.ReadInt(i);
				}
				if(column == "BattlePoint") {
					BattlePoint = reader.ReadInt(i);
				}
				if(column == "BattlePointFullDate") {
					BattlePointFullDate = reader.ReadString(i);
				}
				if(column == "BattlePointTimeToFull") {
					BattlePointTimeToFull = reader.ReadInt(i);
				}
				if(column == "IsFollow") {
					IsFollow = reader.ReadInt(i) != 0;
				}
				if(column == "IsFollower") {
					IsFollower = reader.ReadInt(i) != 0;
				}
				if(column == "FollowDate") {
					FollowDate = reader.ReadString(i);
				}
				if(column == "FollowerDate") {
					FollowerDate = reader.ReadString(i);
				}
				if(column == "FollowCount") {
					FollowCount = reader.ReadInt(i);
				}
				if(column == "FollowerCount") {
					FollowerCount = reader.ReadInt(i);
				}
				if(column == "Comment") {
					Comment = reader.ReadString(i);
				}
				if(column == "MainCardData") {
					MainCardData = MessagePack.MessagePackSerializer.Deserialize<CardData> (reader.ReadBlob(i));
				}
				if(column == "SupportCardList") {
					SupportCardList = MessagePack.MessagePackSerializer.Deserialize<CardData[]> (reader.ReadBlob(i));
				}
				if(column == "SupporterCardList") {
					SupporterCardList = MessagePack.MessagePackSerializer.Deserialize<SupporterCardData[]> (reader.ReadBlob(i));
				}
				if(column == "ReceivablePresentCount") {
					ReceivablePresentCount = reader.ReadInt(i);
				}
				if(column == "ReceivableLoginbonusIdList") {
					ReceivableLoginbonusIdList = MessagePack.MessagePackSerializer.Deserialize<int[]> (reader.ReadBlob(i));
				}
				if(column == "ReceivableMissionCount") {
					ReceivableMissionCount = reader.ReadInt(i);
				}
				if(column == "FreeGemCount") {
					FreeGemCount = reader.ReadInt(i);
				}
				if(column == "PaidGemCount") {
					PaidGemCount = reader.ReadInt(i);
				}
				if(column == "GoldCount") {
					GoldCount = reader.ReadInt(i);
				}
				if(column == "FriendPointCount") {
					FriendPointCount = reader.ReadInt(i);
				}
				if(column == "PvpMedalCount") {
					PvpMedalCount = reader.ReadInt(i);
				}
				if(column == "WeaponBagCapacity") {
					WeaponBagCapacity = reader.ReadInt(i);
				}
				if(column == "MagikiteBagCapacity") {
					MagikiteBagCapacity = reader.ReadInt(i);
				}
				if(column == "RarestCardMissCount") {
					RarestCardMissCount = reader.ReadInt(i);
				}
				if(column == "ActionPointHealCost") {
					ActionPointHealCost = reader.ReadInt(i);
				}
				if(column == "BattlePointHealCost") {
					BattlePointHealCost = reader.ReadInt(i);
				}
				if(column == "CreationDate") {
					CreationDate = reader.ReadString(i);
				}
				if(column == "GainFriendPointOnSupport") {
					GainFriendPointOnSupport = reader.ReadInt(i);
				}
			}
		}

		private void BindParamter(SqliteDatabase connection, IntPtr stmHandle)
		{
			connection.BindInteger(stmHandle, 1, UserId);
			connection.BindText(stmHandle, 2, CustomerId);
			connection.BindText(stmHandle, 3, Nickname);
			connection.BindText(stmHandle, 4, LastLoginDate);
			connection.BindInteger(stmHandle, 5, Level);
			connection.BindInteger(stmHandle, 6, Exp);
			connection.BindInteger(stmHandle, 7, ActionPoint);
			connection.BindText(stmHandle, 8, ActionPointFullDate);
			connection.BindInteger(stmHandle, 9, ActionPointTimeToFull);
			connection.BindInteger(stmHandle, 10, BattlePoint);
			connection.BindText(stmHandle, 11, BattlePointFullDate);
			connection.BindInteger(stmHandle, 12, BattlePointTimeToFull);
			connection.BindInteger(stmHandle, 13, IsFollow ? 1: 0);
			connection.BindInteger(stmHandle, 14, IsFollower ? 1: 0);
			connection.BindText(stmHandle, 15, FollowDate);
			connection.BindText(stmHandle, 16, FollowerDate);
			connection.BindInteger(stmHandle, 17, FollowCount);
			connection.BindInteger(stmHandle, 18, FollowerCount);
			connection.BindText(stmHandle, 19, Comment);
			connection.BindBlob(stmHandle, 20,
				MessagePack.MessagePackSerializer.Serialize<CardData>(MainCardData));
			connection.BindBlob(stmHandle, 21,
				MessagePack.MessagePackSerializer.Serialize<CardData[]>(SupportCardList));
			connection.BindBlob(stmHandle, 22,
				MessagePack.MessagePackSerializer.Serialize<SupporterCardData[]>(SupporterCardList));
			connection.BindInteger(stmHandle, 23, ReceivablePresentCount);
			connection.BindBlob(stmHandle, 24,
				MessagePack.MessagePackSerializer.Serialize<int[]>(ReceivableLoginbonusIdList));
			connection.BindInteger(stmHandle, 25, ReceivableMissionCount);
			connection.BindInteger(stmHandle, 26, FreeGemCount);
			connection.BindInteger(stmHandle, 27, PaidGemCount);
			connection.BindInteger(stmHandle, 28, GoldCount);
			connection.BindInteger(stmHandle, 29, FriendPointCount);
			connection.BindInteger(stmHandle, 30, PvpMedalCount);
			connection.BindInteger(stmHandle, 31, WeaponBagCapacity);
			connection.BindInteger(stmHandle, 32, MagikiteBagCapacity);
			connection.BindInteger(stmHandle, 33, RarestCardMissCount);
			connection.BindInteger(stmHandle, 34, ActionPointHealCost);
			connection.BindInteger(stmHandle, 35, BattlePointHealCost);
			connection.BindText(stmHandle, 36, CreationDate);
			connection.BindInteger(stmHandle, 37, GainFriendPointOnSupport);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO UserDataTable (UserId,CustomerId,Nickname,LastLoginDate,Level,Exp,ActionPoint,ActionPointFullDate,ActionPointTimeToFull,BattlePoint,BattlePointFullDate,BattlePointTimeToFull,IsFollow,IsFollower,FollowDate,FollowerDate,FollowCount,FollowerCount,Comment,MainCardData,SupportCardList,SupporterCardList,ReceivablePresentCount,ReceivableLoginbonusIdList,ReceivableMissionCount,FreeGemCount,PaidGemCount,GoldCount,FriendPointCount,PvpMedalCount,WeaponBagCapacity,MagikiteBagCapacity,RarestCardMissCount,ActionPointHealCost,BattlePointHealCost,CreationDate,GainFriendPointOnSupport) VALUES (?1,?2,?3,?4,?5,?6,?7,?8,?9,?10,?11,?12,?13,?14,?15,?16,?17,?18,?19,?20,?21,?22,?23,?24,?25,?26,?27,?28,?29,?30,?31,?32,?33,?34,?35,?36,?37);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM UserDataTable WHERE UserId={0};", UserId), this, null, transaction);
		}
	}
}
