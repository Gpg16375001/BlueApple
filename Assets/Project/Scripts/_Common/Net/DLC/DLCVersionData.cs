using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

[MessagePackFormatter(typeof(DLCVersionDataFormatter))]
public class DLCVersionData : ISqliteObject
{
    #region Define Prorety
    public Hash128 FileNameHash;
    public Hash128 FileHash;
    public bool IsAssetBundle;
    public bool IsMinimumContents;
    public bool IsExcludeDownload;
    public long FileSize;
    #endregion


    #region SQLite Object Method
    public string CreateParameter ()
    {
        return string.Format (" VALUES (\'{0}\', \'{1}\');", FileNameHash.ToString(), FileHash.ToString());
    }

    public void ReadObject (SqliteDatabase.SqliteDatabaseReader reader, string[] columnNames)
    {
        int count = columnNames.Length;
        for (int i = 0; i < count; ++i) {
            string column = columnNames [i];
            if (column == "filename_hash") {
                FileNameHash = Hash128.Parse(reader.ReadString(i));
            } else if (column == "file_hash") {
                FileHash = Hash128.Parse(reader.ReadString(i));
            }
        }
    }

    public void Save(SqliteDatabase connection, object transaction=null)
    {
        connection.Execute ("REPLACE INTO DLCVesion (filename_hash, file_hash)", this, null, transaction);
    }

    static public void CreateTable(SqliteDatabase connection, object transaction=null)
    {
        connection.Execute (
@"CREATE TABLE IF NOT EXISTS DLCVesion (
filename_hash TEXY NOT NULL primary key,
file_hash TEXT NOT NULL
);",
            transaction);
    }

    static public DLCVersionData Get(SqliteDatabase connection, Hash128 fileNameHash)
    {
        // sqlite側でキャッシュシステムが動くのでオブジェクト側ではキャッシュ制御しない。
        var objs = Filter (connection, string.Format ("filename_hash = \'{0}\'", fileNameHash.ToString()));
        if (objs.Count > 0) {
            return objs [0];
        }
        return null;
    }

    static public List<DLCVersionData> Filter(SqliteDatabase connection, string where=null)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder ();
        builder.Append ("SELECT * FROM DLCVesion");
        if (!string.IsNullOrEmpty (where)) {
            builder.Append (string.Format (" WHERE {0}", where));
        }
        builder.Append (";");
        return connection.Query<DLCVersionData> (builder.ToString());
    }

    static public void DropTable(SqliteDatabase connection, object transaction=null)
    {
        connection.Execute ("DROP TABLE IF EXISTS DLCVesion:", transaction);
    }
    #endregion

    #region MessagePack Formatter
    class DLCVersionDataFormatter : IMessagePackFormatter<DLCVersionData>
    {
        public int Serialize(ref byte[] bytes, int offset, DLCVersionData value, IFormatterResolver formatterResolver) {
            var startOffset = offset;
            // 変数の個数を設定
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 6);

            // FileNameHashをUint32x4の配列で設定
            var fileNameHashArray = value.FileNameHash.Hash128ToIntArray ();
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 4);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileNameHashArray[0]);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileNameHashArray[1]);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileNameHashArray[2]);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileNameHashArray[3]);

            // FileHashをUint32x4の配列で設定
            var fileHashArray = value.FileHash.Hash128ToIntArray ();
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 4);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileHashArray[0]);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileHashArray[1]);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileHashArray[2]);
            offset += MessagePackBinary.WriteUInt32(ref bytes, offset, fileHashArray[3]);

            // isAssetBundleをBoolで設定
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsAssetBundle);

            // IsMinimumContentsをBoolで設定
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsMinimumContents);

            // IsExcludeDownloadをBoolで設定
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsExcludeDownload);

            // ファイルサイズ
            offset += MessagePackBinary.WriteInt64 (ref bytes, offset, value.FileSize);


            return offset - startOffset;
        }

        public DLCVersionData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
            DLCVersionData ret = new DLCVersionData ();

            int startOffset = offset;
            int readed = 0;

            var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
            offset += readed;

            for (int i = 0; i < count; i++) {
                // FileNameHashの読み込み
                if (i == 0) {
                    MessagePackBinary.ReadArrayHeader (bytes, offset, out readed);
                    offset += readed;

                    var hash_0 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;
                    var hash_1 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;
                    var hash_2 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;
                    var hash_3 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;

                    ret.FileNameHash = new Hash128 (hash_0, hash_1, hash_2, hash_3);
                }
                // FileHashの読み込み
                else if (i == 1) {
                    MessagePackBinary.ReadArrayHeader (bytes, offset, out readed);
                    offset += readed;

                    var hash_0 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;
                    var hash_1 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;
                    var hash_2 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;
                    var hash_3 = MessagePackBinary.ReadUInt32 (bytes, offset, out readed);
                    offset += readed;

                    ret.FileHash = new Hash128 (hash_0, hash_1, hash_2, hash_3);
                }
                // isAssetBundleの読み込み
                else if (i == 2) {
                    ret.IsAssetBundle = MessagePackBinary.ReadBoolean (bytes, offset, out readed);
                    offset += readed;
                }
                else if (i == 3) {
                    ret.IsMinimumContents = MessagePackBinary.ReadBoolean (bytes, offset, out readed);
                    offset += readed;
                } 
                else if (i == 4) {
                    ret.IsExcludeDownload = MessagePackBinary.ReadBoolean (bytes, offset, out readed);
                    offset += readed;
                } 
                else if (i == 5) {
                    ret.FileSize = MessagePackBinary.ReadInt64 (bytes, offset, out readed);
                    offset += readed;
                } 
                else {
                    readed = MessagePackBinary.ReadNextBlock (bytes, offset);
                    offset += readed;
                }
            }

            readSize = offset - startOffset;
            return ret;
        }
    }
    #endregion
}