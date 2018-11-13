using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class MagikiteData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS MagikiteDataTable (
MagikiteId INTEGER NOT NULL,
BagId INTEGER NOT NULL PRIMARY KEY,
HPGain INTEGER NOT NULL,
ATKGain INTEGER NOT NULL,
DEFGain INTEGER NOT NULL,
SPDGain INTEGER NOT NULL,
IsEquipped INTEGER NOT NULL,
IsLocked INTEGER NOT NULL,
CardId INTEGER NOT NULL,
SlotId INTEGER NOT NULL,
ModificationDate TEXT NOT NULL,
CreationDate TEXT NOT NULL,
Rarity INTEGER NOT NULL
);",
						trans);
				}
			);
		}

		static public MagikiteData CacheGet(long BagId)
		{
			var objs = Filter (string.Format ("BagId = {0}", BagId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<MagikiteData>>();
		}

		static public List<MagikiteData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<MagikiteData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM MagikiteDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<MagikiteData> (builder.ToString());
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
				if(column == "MagikiteId") {
					MagikiteId = reader.ReadInt(i);
				}
				if(column == "BagId") {
					BagId = reader.ReadInt64(i);
				}
				if(column == "HPGain") {
					HPGain = reader.ReadInt(i);
				}
				if(column == "ATKGain") {
					ATKGain = reader.ReadInt(i);
				}
				if(column == "DEFGain") {
					DEFGain = reader.ReadInt(i);
				}
				if(column == "SPDGain") {
					SPDGain = reader.ReadInt(i);
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
				if(column == "Rarity") {
					Rarity = reader.ReadInt(i);
				}
			}
		}

		private void BindParamter(SqliteDatabase connection, IntPtr stmHandle)
		{
			connection.BindInteger(stmHandle, 1, MagikiteId);
			connection.BindInteger64(stmHandle, 2, BagId);
			connection.BindInteger(stmHandle, 3, HPGain);
			connection.BindInteger(stmHandle, 4, ATKGain);
			connection.BindInteger(stmHandle, 5, DEFGain);
			connection.BindInteger(stmHandle, 6, SPDGain);
			connection.BindInteger(stmHandle, 7, IsEquipped ? 1: 0);
			connection.BindInteger(stmHandle, 8, IsLocked ? 1: 0);
			connection.BindInteger(stmHandle, 9, CardId);
			connection.BindInteger(stmHandle, 10, SlotId);
			connection.BindText(stmHandle, 11, ModificationDate);
			connection.BindText(stmHandle, 12, CreationDate);
			connection.BindInteger(stmHandle, 13, Rarity);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO MagikiteDataTable (MagikiteId,BagId,HPGain,ATKGain,DEFGain,SPDGain,IsEquipped,IsLocked,CardId,SlotId,ModificationDate,CreationDate,Rarity) VALUES (?1,?2,?3,?4,?5,?6,?7,?8,?9,?10,?11,?12,?13);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM MagikiteDataTable WHERE BagId={0};", BagId), this, null, transaction);
		}
	}
}
