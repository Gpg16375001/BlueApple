using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SqliteException : Exception
{
    public SqliteException (string message) : base (message)
    {
    
    }
}

public class SqliteDatabase
{
    private bool CanExQuery = true;

    const int SQLITE_OK = 0;

    const int SQLITE_ERROR = 1;   /* Generic error */
    const int SQLITE_INTERNAL = 2;   /* Internal logic error in SQLite */
    const int SQLITE_PERM = 3;   /* Access permission denied */
    const int SQLITE_ABORT = 4;   /* Callback routine requested an abort */
    const int SQLITE_BUSY = 5;   /* The database file is locked */
    const int SQLITE_LOCKED = 6;   /* A table in the database is locked */
    const int SQLITE_NOMEM = 7;   /* A malloc() failed */
    const int SQLITE_READONLY = 8;   /* Attempt to write a readonly database */
    const int SQLITE_INTERRUPT = 9;   /* Operation terminated by sqlite3_interrupt()*/
    const int SQLITE_IOERR = 10;   /* Some kind of disk I/O error occurred */
    const int SQLITE_CORRUPT = 11;   /* The database disk image is malformed */
    const int SQLITE_NOTFOUND = 12;   /* Unknown opcode in sqlite3_file_control() */
    const int SQLITE_FULL = 13;   /* Insertion failed because database is full */
    const int SQLITE_CANTOPEN = 14;   /* Unable to open the database file */
    const int SQLITE_PROTOCOL = 15;   /* Database lock protocol error */
    const int SQLITE_EMPTY = 16;   /* Internal use only */
    const int SQLITE_SCHEMA = 17;   /* The database schema changed */
    const int SQLITE_TOOBIG = 18;   /* String or BLOB exceeds size limit */
    const int SQLITE_CONSTRAINT = 19;   /* Abort due to constraint violation */
    const int SQLITE_MISMATCH = 20;   /* Data type mismatch */
    const int SQLITE_MISUSE = 21;   /* Library used incorrectly */
    const int SQLITE_NOLFS = 22;   /* Uses OS features not supported on host */
    const int SQLITE_AUTH = 23;   /* Authorization denied */
    const int SQLITE_FORMAT = 24;   /* Not used */
    const int SQLITE_RANGE = 25;   /* 2nd parameter to sqlite3_bind out of range */
    const int SQLITE_NOTADB = 26;   /* File opened that is not a database file */
    const int SQLITE_NOTICE = 27;   /* Notifications from sqlite3_log() */
    const int SQLITE_WARNING = 28;   /* Warnings from sqlite3_log() */

    const int SQLITE_ROW = 100;
    const int SQLITE_DONE = 101;

    const int SQLITE_INTEGER = 1;
    const int SQLITE_FLOAT = 2;
    const int SQLITE_TEXT = 3;
    const int SQLITE_BLOB = 4;
    const int SQLITE_NULL = 5;

    [DllImport ("sqlite3", EntryPoint = "sqlite3_open")]
    private static extern int sqlite3_open (string filename, out IntPtr db);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_close")]
    private static extern int sqlite3_close (IntPtr db);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_prepare_v2")]
    private static extern int sqlite3_prepare_v2 (IntPtr db, string zSql, int nByte, out IntPtr ppStmpt, IntPtr pzTail);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_step")]
    private static extern int sqlite3_step (IntPtr stmHandle);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_finalize")]
    private static extern int sqlite3_finalize (IntPtr stmHandle);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_errmsg")]
    private static extern IntPtr sqlite3_errmsg (IntPtr db);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_count")]
    private static extern int sqlite3_column_count (IntPtr stmHandle);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_name")]
    private static extern IntPtr sqlite3_column_name (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_type")]
    private static extern int sqlite3_column_type (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_int")]
    private static extern int sqlite3_column_int (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_int64")]
    private static extern long sqlite3_column_int64 (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_text")]
    private static extern IntPtr sqlite3_column_text (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_double")]
    private static extern double sqlite3_column_double (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_blob")]
    private static extern IntPtr sqlite3_column_blob (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_column_bytes")]
    private static extern int sqlite3_column_bytes (IntPtr stmHandle, int iCol);

    public static readonly IntPtr SQLITE_STATIC = IntPtr.Zero;
    public static readonly IntPtr SQLITE_TRANSIENT = new IntPtr(-1);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_bind_blob")]
    private static extern int sqlite3_bind_blob (IntPtr stmHandle, int iCol, byte[] data, int length, IntPtr destructor);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_bind_null")]
    private static extern int sqlite3_bind_null (IntPtr stmHandle, int iCol);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_bind_double")]
    private static extern int sqlite3_bind_double (IntPtr stmHandle, int iCol, double data);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_bind_int")]
    private static extern int sqlite3_bind_int (IntPtr stmHandle, int iCol, int data);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_bind_int64")]
    private static extern int sqlite3_bind_int64 (IntPtr stmHandle, int iCol, long data);

    [DllImport ("sqlite3", EntryPoint = "sqlite3_bind_text")]
    private static extern int sqlite3_bind_text (IntPtr stmHandle, int iCol, string data, int length, IntPtr destructor);

    private IntPtr _connection;

    private bool IsConnectionOpen { get; set; }

    private string pathDB;

	
    #region Public Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDatabase"/> class.
    /// </summary>
    /// <param name='dbName'> 
    /// Data Base name. (the file needs exist in the streamingAssets folder)
    /// </param>
    public SqliteDatabase (string dbName)
    {
        pathDB = dbName;
    }

    /// <summary>
    /// Open Sqlite Database.
    /// </summary>
    public void Open ()
    {
        this.Open (pathDB);	
    }
    private void Open (string path)
    {
        if (IsConnectionOpen) {
            throw new SqliteException ("There is already an open connection");
        }
        
        if (sqlite3_open (path, out _connection) != SQLITE_OK) {
            throw new SqliteException ("Could not open database file: " + path);
        }
        
        IsConnectionOpen = true;
    }

    /// <summary>
    /// Close Sqlite Database.
    /// </summary>
    public void Close ()
    {
        if (IsConnectionOpen) {
            sqlite3_close (_connection);
        }
        
        IsConnectionOpen = false;
    }

    /// <summary>
    /// Begin transaction.
    /// </summary>
    /// <returns>transaction object.</returns>
    public object BeginTransaction ()
    {
        if (!CanExQuery) {
            Debug.Log ("ERROR: Can't execute the query, verify DB origin file");
            return null;
        }

        if (!IsConnectionOpen) {
            throw new SqliteException ("SQLite database is not open.");
        }

        object transaction = new object();
        var sql = "BEGIN TRANSACTION;";
        IntPtr stmHandle = Prepare (sql);

        if (sqlite3_step (stmHandle) != SQLITE_DONE) {
            throw new SqliteException ("Could not execute SQL statement.");
        }

        return transaction;
    }

    /// <summary>
    /// Commit transaction.
    /// </summary>
    /// <param name="transaction">transaction object.</param>
    public void Commit (object transaction)
    {
        if (!CanExQuery) {
            Debug.Log ("ERROR: Can't execute the query, verify DB origin file");
            return;
        }

        if (!IsConnectionOpen) {
            throw new SqliteException ("SQLite database is not open.");
        }

        var sql = "COMMIT TRANSACTION;";
        IntPtr stmHandle = Prepare (sql);

        if (sqlite3_step (stmHandle) != SQLITE_DONE) {
            throw new SqliteException ("Could not execute SQL statement.");
        }
    }

    /// <summary>
    /// Rollback transaction.
    /// </summary>
    /// <param name="transaction">transaction object.</param>
    public void Rollback (object transaction)
    {
        if (!CanExQuery) {
            Debug.Log ("ERROR: Can't execute the query, verify DB origin file");
            return;
        }

        if (!IsConnectionOpen) {
            throw new SqliteException ("SQLite database is not open.");
        }

        var sql = "ROLLBACK TRANSACTION;";
        IntPtr stmHandle = Prepare (sql);

        if (sqlite3_step (stmHandle) != SQLITE_DONE) {
            throw new SqliteException ("Could not execute SQL statement.");
        }
    }

    /// <summary>
    /// Executes a Update, Delete, etc  query.
    /// </summary>
    /// <param name='query'>
    /// Query.
    /// </param>
    /// <exception cref='SqliteException'>
    /// Is thrown when the sqlite exception.
    /// </exception>
    public void ExecuteNonQuery (string query, Action<SqliteDatabase, IntPtr> bind=null)
    {
        if (!CanExQuery) {
            Debug.Log ("ERROR: Can't execute the query, verify DB origin file");
            return;
        }

        if (!IsConnectionOpen) {
            throw new SqliteException ("SQLite database is not open.");
        }

        IntPtr stmHandle = Prepare (query);
        if (bind != null) {
            bind (this, stmHandle);
        }
        if (sqlite3_step (stmHandle) != SQLITE_DONE) {
            throw new SqliteException ("Could not execute SQL statement.");
        }
        
        Finalize (stmHandle);
    }

    /// <summary>
    /// Executes a query that requires a response (SELECT, etc).
    /// </summary>
    /// <returns>
    /// Dictionary with the response data
    /// </returns>
    /// <param name='query'>
    /// Query.
    /// </param>
    /// <exception cref='SqliteException'>
    /// Is thrown when the sqlite exception.
    /// </exception>
    public List<T> ExecuteQuery<T> (string query) where T : ISqliteObject
    {
        if (!CanExQuery) {
            Debug.Log ("ERROR: Can't execute the query, verify DB origin file");
            return null;
        }
		
        if (!IsConnectionOpen) {
            throw new SqliteException ("SQLite database is not open.");
        }
        
        IntPtr stmHandle = Prepare (query);
 
        int columnCount = sqlite3_column_count (stmHandle);
        string[] columnNames = new string[columnCount];

        for (int i = 0; i < columnCount; i++) {
            string columnName = Marshal.PtrToStringAnsi (sqlite3_column_name (stmHandle, i));
            columnNames [i] = columnName;
        }

        SqliteDatabaseReader reader = new SqliteDatabaseReader (stmHandle);
        List<T> ret = new List<T> ();
        while (sqlite3_step (stmHandle) == SQLITE_ROW) {
            T obj = (T)System.Activator.CreateInstance (typeof(T));
            obj.ReadObject (reader, columnNames);
            ret.Add (obj);
        }
        Finalize (stmHandle);
        return ret;
    }

    public void ExecuteScript (string script)
    {
        string[] statements = script.Split (';');
        
        foreach (string statement in statements) {
            if (!string.IsNullOrEmpty (statement.Trim ())) {
                ExecuteNonQuery (statement);
            }
        }
    }

    public void BindInteger(IntPtr stmHandle, int iCol, int data)
    {
        if (sqlite3_bind_int (stmHandle, iCol, data) != SQLITE_OK) {
            IntPtr errorMsg = sqlite3_errmsg (_connection);
            throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
        }
    }
    public void BindInteger64(IntPtr stmHandle, int iCol, long data)
    {
        if(sqlite3_bind_int64 (stmHandle, iCol, data) != SQLITE_OK) {
            IntPtr errorMsg = sqlite3_errmsg (_connection);
            throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
        }
    }
    public void BindDouble(IntPtr stmHandle, int iCol, double data)
    {
        if(sqlite3_bind_double (stmHandle, iCol, data) != SQLITE_OK) {
            IntPtr errorMsg = sqlite3_errmsg (_connection);
            throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
        }
    }
    public void BindText(IntPtr stmHandle, int iCol, string data)
    {
        if(sqlite3_bind_text (stmHandle, iCol, data, data.Length, SQLITE_TRANSIENT) != SQLITE_OK) {
            IntPtr errorMsg = sqlite3_errmsg (_connection);
            throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
        }
    }
    public void BindBlob(IntPtr stmHandle, int iCol, byte[] data)
    {
        if (data == null) {
            if(sqlite3_bind_null(stmHandle, iCol) != SQLITE_OK) {
                IntPtr errorMsg = sqlite3_errmsg (_connection);
                throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
            }
        } else {
            if(sqlite3_bind_blob (stmHandle, iCol, data, data.Length, SQLITE_TRANSIENT) != SQLITE_OK) {
                IntPtr errorMsg = sqlite3_errmsg (_connection);
                throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
            }
        }
    }

    #endregion

    #region Private Methods

    private IntPtr Prepare (string query)
    {
        IntPtr stmHandle;
        
        if (sqlite3_prepare_v2 (_connection, query, query.Length, out stmHandle, IntPtr.Zero) != SQLITE_OK) {
            IntPtr errorMsg = sqlite3_errmsg (_connection);
            throw new SqliteException (Marshal.PtrToStringAnsi (errorMsg));
        }
        
        return stmHandle;
    }

    private void Finalize (IntPtr stmHandle)
    {
        if (sqlite3_finalize (stmHandle) != SQLITE_OK) {
            throw new SqliteException ("Could not finalize SQL statement.");
        }
    }

    #endregion


    #region Sqlite Database Reader
    /// <summary>
    /// Sqlite database reader.
    /// </summary>
    public class SqliteDatabaseReader
    {
        /// <summary>
        /// The stream handler.
        /// </summary>
        private IntPtr _stmHandle;

        public SqliteDatabaseReader (IntPtr stmHandle)
        {
            _stmHandle = stmHandle;
        }

        /// <summary>
        /// Read the int.
        /// </summary>
        /// <returns>The int data.</returns>
        /// <param name="index">Index.</param>
        public int ReadInt (int index)
        {
            return sqlite3_column_int (_stmHandle, index);
        }

        public long ReadInt64 (int index)
        {
            return sqlite3_column_int64 (_stmHandle, index);
        }

        /// <summary>
        /// Read the string.
        /// </summary>
        /// <returns>The string data.</returns>
        /// <param name="index">Index.</param>
        public string ReadString (int index)
        {
            IntPtr text = sqlite3_column_text (_stmHandle, index);
            return Marshal.PtrToStringAnsi (text);
        }

        /// <summary>
        /// Read the double.
        /// </summary>
        /// <returns>The double data.</returns>
        /// <param name="index">Index.</param>
        public double ReadDouble (int index)
        {
            return sqlite3_column_double (_stmHandle, index);
        }

        /// <summary>
        /// Read the BLOB.
        /// </summary>
        /// <returns>The BLOB data.</returns>
        /// <param name="index">Index.</param>
        public byte[] ReadBlob (int index)
        {
            IntPtr blob = sqlite3_column_blob (_stmHandle, index);
            int size = sqlite3_column_bytes (_stmHandle, index);
            byte[] data = new byte[size];
            Marshal.Copy (blob, data, 0, size);
            return data;
        }
    }
    #endregion
}
