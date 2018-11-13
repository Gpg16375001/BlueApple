using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// SqliteObject用のベースクラス。
/// </summary>
public interface ISqliteObject
{
    string CreateParameter ();
    void ReadObject (SqliteDatabase.SqliteDatabaseReader reader, string[] columnNames);
}
