using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public partial class QuestAchievement : ISqliteObject
	{
		static public void CreateTable()
		{
			var connection = ServerModelCache.GetConnection ();
			connection.MakeTransaction (
				(con, trans) => {
					con.Execute (
@"CREATE TABLE IF NOT EXISTS QuestAchievementTable (
QuestType INTEGER NOT NULL,
QuestId INTEGER NOT NULL PRIMARY KEY,
IsAchieved INTEGER NOT NULL,
AchievedSelectionIdList BLOB NOT NULL,
AchievedMissionIdList BLOB NOT NULL,
ReceivedMissionRewardCount INTEGER NOT NULL,
ForceLockStatus INTEGER NOT NULL
);",
						trans);
				}
			);
		}

		static public QuestAchievement CacheGet(int QuestId)
		{
			var objs = Filter (string.Format ("QuestId = {0}", QuestId));
			if (objs.Count > 0) {
				return objs [0];
			}
			return null;
		}

		static public int GetRevision()
		{
			return ServerModelCache.GetRevision<IEnumerable<QuestAchievement>>();
		}

		static public List<QuestAchievement> CacheGetAll()
		{
			return Filter ();
		}

		static public List<QuestAchievement> Filter(string where=null)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("SELECT * FROM QuestAchievementTable");
			if (!string.IsNullOrEmpty (where)) {
				builder.Append (string.Format (" WHERE {0}", where));
			}
			builder.Append (";");
			return ServerModelCache.GetConnection ().Query<QuestAchievement> (builder.ToString());
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
				if(column == "QuestType") {
					QuestType = reader.ReadInt(i);
				}
				if(column == "QuestId") {
					QuestId = reader.ReadInt(i);
				}
				if(column == "IsAchieved") {
					IsAchieved = reader.ReadInt(i) != 0;
				}
				if(column == "AchievedSelectionIdList") {
					AchievedSelectionIdList = MessagePack.MessagePackSerializer.Deserialize<int[]> (reader.ReadBlob(i));
				}
				if(column == "AchievedMissionIdList") {
					AchievedMissionIdList = MessagePack.MessagePackSerializer.Deserialize<int[]> (reader.ReadBlob(i));
				}
				if(column == "ReceivedMissionRewardCount") {
					ReceivedMissionRewardCount = reader.ReadInt(i);
				}
				if(column == "ForceLockStatus") {
					ForceLockStatus = reader.ReadInt(i);
				}
			}
		}

		private void BindParamter(SqliteDatabase connection, IntPtr stmHandle)
		{
			connection.BindInteger(stmHandle, 1, QuestType);
			connection.BindInteger(stmHandle, 2, QuestId);
			connection.BindInteger(stmHandle, 3, IsAchieved ? 1: 0);
			connection.BindBlob(stmHandle, 4,
				MessagePack.MessagePackSerializer.Serialize<int[]>(AchievedSelectionIdList));
			connection.BindBlob(stmHandle, 5,
				MessagePack.MessagePackSerializer.Serialize<int[]>(AchievedMissionIdList));
			connection.BindInteger(stmHandle, 6, ReceivedMissionRewardCount);
			connection.BindInteger(stmHandle, 7, ForceLockStatus);
		}

		public void Save(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute ("REPLACE INTO QuestAchievementTable (QuestType,QuestId,IsAchieved,AchievedSelectionIdList,AchievedMissionIdList,ReceivedMissionRewardCount,ForceLockStatus) VALUES (?1,?2,?3,?4,?5,?6,?7);", this, BindParamter, transaction);
		}

		public void Delete(SqliteDatabase connection, object transaction=null)
		{
			connection.Execute (string.Format("DELETE FROM QuestAchievementTable WHERE QuestId={0};", QuestId), this, null, transaction);
		}
	}
}
