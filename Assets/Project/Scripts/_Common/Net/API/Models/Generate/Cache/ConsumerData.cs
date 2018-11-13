using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class ConsumerData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS ConsumerDataTable (
ConsumerId INTEGER NOT NULL PRIMARY KEY,
Count INTEGER NOT NULL,
ModificationDate TEXT NOT NULL,
CreationDate TEXT NOT NULL
);",
						trans);
				}
			);
		}

		static public ConsumerData CacheGet(int ConsumerId)
		{
			var objs = Filter (string.Format ("ConsumerId = {0}", ConsumerId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<ConsumerData>>();
		}

		static public List<ConsumerData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<ConsumerData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM ConsumerDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<ConsumerData> (builder.ToString());
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
				if(column == "ConsumerId") {
					ConsumerId = reader.ReadInt(i);
				}
				if(column == "Count") {
					Count = reader.ReadInt(i);
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
			connection.BindInteger(stmHandle, 1, ConsumerId);
			connection.BindInteger(stmHandle, 2, Count);
			connection.BindText(stmHandle, 3, ModificationDate);
			connection.BindText(stmHandle, 4, CreationDate);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO ConsumerDataTable (ConsumerId,Count,ModificationDate,CreationDate) VALUES (?1,?2,?3,?4);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM ConsumerDataTable WHERE ConsumerId={0};", ConsumerId), this, null, transaction);
		}
	}
}
