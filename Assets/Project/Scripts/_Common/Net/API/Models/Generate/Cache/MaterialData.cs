using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class MaterialData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS MaterialDataTable (
MaterialId INTEGER NOT NULL PRIMARY KEY,
Count INTEGER NOT NULL,
ModificationDate TEXT NOT NULL,
CreationDate TEXT NOT NULL
);",
						trans);
				}
			);
		}

		static public MaterialData CacheGet(int MaterialId)
		{
			var objs = Filter (string.Format ("MaterialId = {0}", MaterialId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<MaterialData>>();
		}

		static public List<MaterialData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<MaterialData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM MaterialDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<MaterialData> (builder.ToString());
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
				if(column == "MaterialId") {
					MaterialId = reader.ReadInt(i);
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
			connection.BindInteger(stmHandle, 1, MaterialId);
			connection.BindInteger(stmHandle, 2, Count);
			connection.BindText(stmHandle, 3, ModificationDate);
			connection.BindText(stmHandle, 4, CreationDate);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO MaterialDataTable (MaterialId,Count,ModificationDate,CreationDate) VALUES (?1,?2,?3,?4);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM MaterialDataTable WHERE MaterialId={0};", MaterialId), this, null, transaction);
		}
	}
}
