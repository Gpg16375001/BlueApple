using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class WeaponData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS WeaponDataTable (
WeaponId INTEGER NOT NULL,
BagId INTEGER NOT NULL PRIMARY KEY,
Exp INTEGER NOT NULL,
MaxExp INTEGER NOT NULL,
Level INTEGER NOT NULL,
LimitBreakGrade INTEGER NOT NULL,
Rarity INTEGER NOT NULL,
IsEquipped INTEGER NOT NULL,
IsLocked INTEGER NOT NULL,
CardId INTEGER NOT NULL,
SlotId INTEGER NOT NULL,
ModificationDate TEXT NOT NULL,
CreationDate TEXT NOT NULL
);",
						trans);
				}
			);
		}

		static public WeaponData CacheGet(long BagId)
		{
			var objs = Filter (string.Format ("BagId = {0}", BagId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<WeaponData>>();
		}

		static public List<WeaponData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<WeaponData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM WeaponDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<WeaponData> (builder.ToString());
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
				if(column == "WeaponId") {
					WeaponId = reader.ReadInt(i);
				}
				if(column == "BagId") {
					BagId = reader.ReadInt64(i);
				}
				if(column == "Exp") {
					Exp = reader.ReadInt(i);
				}
				if(column == "MaxExp") {
					MaxExp = reader.ReadInt(i);
				}
				if(column == "Level") {
					Level = reader.ReadInt(i);
				}
				if(column == "LimitBreakGrade") {
					LimitBreakGrade = reader.ReadInt(i);
				}
				if(column == "Rarity") {
					Rarity = reader.ReadInt(i);
				}
				if(column == "IsEquipped") {
					IsEquipped = reader.ReadInt(i) != 0;
				}
				if(column == "IsLocked") {
					IsLocked = reader.ReadInt(i) != 0;
				}
				if(column == "CardId") {
					CardId = reader.ReadInt(i);
				}
				if(column == "SlotId") {
					SlotId = reader.ReadInt(i);
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
			connection.BindInteger(stmHandle, 1, WeaponId);
			connection.BindInteger64(stmHandle, 2, BagId);
			connection.BindInteger(stmHandle, 3, Exp);
			connection.BindInteger(stmHandle, 4, MaxExp);
			connection.BindInteger(stmHandle, 5, Level);
			connection.BindInteger(stmHandle, 6, LimitBreakGrade);
			connection.BindInteger(stmHandle, 7, Rarity);
			connection.BindInteger(stmHandle, 8, IsEquipped ? 1: 0);
			connection.BindInteger(stmHandle, 9, IsLocked ? 1: 0);
			connection.BindInteger(stmHandle, 10, CardId);
			connection.BindInteger(stmHandle, 11, SlotId);
			connection.BindText(stmHandle, 12, ModificationDate);
			connection.BindText(stmHandle, 13, CreationDate);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO WeaponDataTable (WeaponId,BagId,Exp,MaxExp,Level,LimitBreakGrade,Rarity,IsEquipped,IsLocked,CardId,SlotId,ModificationDate,CreationDate) VALUES (?1,?2,?3,?4,?5,?6,?7,?8,?9,?10,?11,?12,?13);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM WeaponDataTable WHERE BagId={0};", BagId), this, null, transaction);
		}
	}
}
