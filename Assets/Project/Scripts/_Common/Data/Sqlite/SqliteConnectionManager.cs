using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SqliteConnectionManager : IDisposable
{
    public static SqliteConnectionManager ShardInstanse {
        get {
            instanse = instanse ?? new SqliteConnectionManager();
            return instanse;
        }
    }

    public void Dispose()
    {
        // 開いているコネクションを全て閉じる
        if (connections.Count > 0) {
            foreach (var obj in connections.Values) {
                SqliteDatabase connection = obj as SqliteDatabase;
                if (connection != null) {
                    connection.Close ();
                }
            }
            connections.Clear();
        }
        instanse = null;
    }

    public SqliteDatabase CreateConnection(string key, string dataSource)
    {
        // すでに同じキーがあれば何もしない
        if (connections.ContainsKey (key)) {
            return connections [key] as SqliteDatabase;
        }

        // メモリにSqliteDBをオープンする
        // 基本的にはアプリが起動している間はずっと開きっぱなしにする。
        SqliteDatabase connection = new SqliteDatabase (dataSource);
        connection.Open ();
        connections.Add (key, connection);

        return connection;
    }

    public SqliteDatabase GetConnection(string key)
    {
        if (connections.ContainsKey (key)) {
            return connections [key] as SqliteDatabase;
        }
        return null;
    }

    private SqliteConnectionManager()
    {
        connections.Clear ();
    }

    ~SqliteConnectionManager()
    {
        Dispose ();
    }

    private static SqliteConnectionManager instanse = null;
    private Hashtable connections = new Hashtable();
}
