using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

public static class SqliteConnectionExtension {

    public static List<T> Query<T>(this SqliteDatabase connection, string sql) where T : ISqliteObject
    {
        return connection.ExecuteQuery<T>(sql);
    }

    public static void MakeTransaction(this SqliteDatabase connection, System.Action<SqliteDatabase, object> execute)
    {
        var trans = connection.BeginTransaction ();
        try {
            execute(connection, trans);
            connection.Commit (trans);
        } catch (System.Exception ex) {
            UnityEngine.Debug.LogException (ex);
            connection.Rollback (trans);
        }
    }

    public static void Execute(this SqliteDatabase connection, string sql, object transaction)
    {
        System.Action<SqliteDatabase, object> execute = new System.Action<SqliteDatabase, object> ((con, trans) => {
            con.ExecuteNonQuery(sql);
        });
            
        if (transaction == null) {
            connection.MakeTransaction(execute);
        } else {
            execute (connection, transaction);
        }
    }

    public static void Execute<T>(this SqliteDatabase connection, string sql, T obj, System.Action<SqliteDatabase, IntPtr> bind, object transaction) where T : ISqliteObject
    {
        System.Action<SqliteDatabase, object> execute = new System.Action<SqliteDatabase, object> ((con, trans) => {
            sql += obj.CreateParameter();
            con.ExecuteNonQuery(sql, bind);
        });

        if (transaction == null) {
            connection.MakeTransaction(execute);
        } else {
            execute (connection, transaction);
        }
    }
}
