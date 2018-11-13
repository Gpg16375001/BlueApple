using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class FormationData : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS FormationDataTable (
FormationId INTEGER NOT NULL PRIMARY KEY,
FormationLevel INTEGER NOT NULL,
ModificationDate TEXT NOT NULL,
CreationDate TEXT NOT NULL
);",
						trans);
				}
			);
		}

		static public FormationData CacheGet(int FormationId)
		{
			var objs = Filter (string.Format ("FormationId = {0}", FormationId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<FormationData>>();
		}

		static public List<FormationData> CacheGetAll()
		{
			return Filter ();
		}

		static public List<FormationData> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM FormationDataTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<FormationData> (builder.ToString());
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
				if(column == "FormationId") {
					FormationId = reader.ReadInt(i);
				}
				if(column == "FormationLevel") {
					FormationLevel = reader.ReadInt(i);
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
			connection.BindInteger(stmHandle, 1, FormationId);
			connection.BindInteger(stmHandle, 2, FormationLevel);
			connection.BindText(stmHandle, 3, ModificationDate);
			connection.BindText(stmHandle, 4, CreationDate);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO FormationDataTable (FormationId,FormationLevel,ModificationDate,CreationDate) VALUES (?1,?2,?3,?4);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM FormationDataTable WHERE FormationId={0};", FormationId), this, null, transaction);
		}
	}
}
