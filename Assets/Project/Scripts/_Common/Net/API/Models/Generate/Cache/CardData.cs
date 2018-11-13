using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class CardData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS CardDataTable (
CardId INTEGER NOT NULL PRIMARY KEY,
Exp INTEGER NOT NULL,
Level INTEGER NOT NULL,
LimitBreakGrade INTEGER NOT NULL,
BoardDataList BLOB NOT NULL,
Rarity INTEGER NOT NULL,
EquippedWeaponBagId INTEGER NOT NULL,
EquippedMagikiteBagIdList BLOB NOT NULL,
ModificationDate TEXT NOT NULL,
CreationDate TEXT NOT NULL
);",
						trans);
				}
			);
		}

		static public CardData CacheGet(int CardId)
		{
			var objs = Filter (string.Format ("CardId = {0}", CardId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<CardData>>();
		}

		static public List<CardData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<CardData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM CardDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<CardData> (builder.ToString());
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
				if(column == "CardId") {
					CardId = reader.ReadInt(i);
				}
				if(column == "Exp") {
					Exp = reader.ReadInt(i);
				}
				if(column == "Level") {
					Level = reader.ReadInt(i);
				}
				if(column == "LimitBreakGrade") {
					LimitBreakGrade = reader.ReadInt(i);
				}
				if(column == "BoardDataList") {
					BoardDataList = MessagePack.MessagePackSerializer.Deserialize<BoardData[]> (reader.ReadBlob(i));
				}
				if(column == "Rarity") {
					Rarity = reader.ReadInt(i);
				}
				if(column == "EquippedWeaponBagId") {
					EquippedWeaponBagId = reader.ReadInt64(i);
				}
				if(column == "EquippedMagikiteBagIdList") {
					EquippedMagikiteBagIdList = MessagePack.MessagePackSerializer.Deserialize<long[]> (reader.ReadBlob(i));
				}
				if(column == "ModificationDate") {
					ModificationDate = reader.ReadString(i);
				}
				if(column == "CreationDate") {
					CreationDate = reader.ReadString(i);
				}
			}
		}

		private void BindParamter(SqliteDatabase connection, IntPtr stmHandle)
		{
			connection.BindInteger(stmHandle, 1, CardId);
			connection.BindInteger(stmHandle, 2, Exp);
			connection.BindInteger(stmHandle, 3, Level);
			connection.BindInteger(stmHandle, 4, LimitBreakGrade);
			connection.BindBlob(stmHandle, 5,
				MessagePack.MessagePackSerializer.Serialize<BoardData[]>(BoardDataList));
			connection.BindInteger(stmHandle, 6, Rarity);
			connection.BindInteger64(stmHandle, 7, EquippedWeaponBagId);
			connection.BindBlob(stmHandle, 8,
				MessagePack.MessagePackSerializer.Serialize<long[]>(EquippedMagikiteBagIdList));
			connection.BindText(stmHandle, 9, ModificationDate);
			connection.BindText(stmHandle, 10, CreationDate);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO CardDataTable (CardId,Exp,Level,LimitBreakGrade,BoardDataList,Rarity,EquippedWeaponBagId,EquippedMagikiteBagIdList,ModificationDate,CreationDate) VALUES (?1,?2,?3,?4,?5,?6,?7,?8,?9,?10);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM CardDataTable WHERE CardId={0};", CardId), this, null, transaction);
		}
	}
}
