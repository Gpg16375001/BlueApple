using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

public static class EngineDataParser {

    enum ParseToken {
        NONE = 0,
        HASH_OPEN,
        HASH_CLOSE,

        ARRAY_OPEN,
        ARRAY_CLOSE,

        PROPARTY,

        STRING_OPEN,
        STRING_CLOSE,

        NUMBER,

        BOOLEAN_TRUE,
        BOOLEAN_FALSE,
    }
    public static object Parse(byte[] engineData)
    {
        object ret = null;
        using (MemoryStream stream = new MemoryStream (engineData)) {
            using (reader = new BinaryReader (stream)) {
                cachePosition = -1;
                ret = ParseValue ();
            }
            reader = null;
        }

        return ret;
    }

    private static object ParseValue(ParseToken token=ParseToken.NONE)
    {
        token = token == ParseToken.NONE ? Token () : token;
        object ret = null;
        switch (token) {
        case ParseToken.HASH_OPEN:
            ret = ReadHashtable ();
            break;
        case ParseToken.HASH_CLOSE:
            break;
        case ParseToken.ARRAY_OPEN:
            ret = ReadArray ();
            break;
        case ParseToken.ARRAY_CLOSE:
            break;
        case ParseToken.STRING_OPEN:
            string tmp = ReadString ();
            ret = (object)tmp;
            break;
        case ParseToken.STRING_CLOSE:
            break;
        case ParseToken.BOOLEAN_FALSE:
            ret = (object)false;
            break;
        case ParseToken.BOOLEAN_TRUE:
            ret = (object)true;
            break;
        case ParseToken.NUMBER:
            double d = 0;
            double.TryParse (ReadValue(), out d);
            ret = (object)d;
            break;
        }

        return ret;
    }

    private static ParseToken Token()
    {
        // 空白や改行をスキップする
        EatWhitespace ();

        var peek = PeekChar ();
        switch (peek) {
        case '<':
            if (ReadAsciiChar (2) == "<<") {
                return ParseToken.HASH_OPEN;
            }
            Seek (-2);
            break;
        case '>':
            if (ReadAsciiChar (2) == ">>") {
                return ParseToken.HASH_CLOSE;
            }
            Seek (-2);
            break;
        case '/':
            Seek (1);
            return ParseToken.PROPARTY;
        case '(':
            Seek (1);
            return ParseToken.STRING_OPEN;
        case ')':
            Seek (1);
            return ParseToken.STRING_CLOSE;
        case '[':
            Seek (1);
            return ParseToken.ARRAY_OPEN;
        case ']':
            Seek (1);
            return ParseToken.ARRAY_CLOSE;
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
        case '.':
        case '-':
            return ParseToken.NUMBER;

        case 't':
            if (ReadAsciiChar (4) == "true") {
                return ParseToken.BOOLEAN_TRUE;
            }
            Seek (-4);
            break;

        case 'f':
            if (ReadAsciiChar (5) == "false") {
                return ParseToken.BOOLEAN_FALSE;
            }
            Seek (-5);
            break;
        }

        return ParseToken.NONE;
    }

    static long? _StreamLength;
    static long StreamLength {
        get {
            if (!_StreamLength.HasValue) {
                _StreamLength = reader.BaseStream.Length;
            }
            return _StreamLength.Value;
        }
    }
    static bool EndOfStream {
        get {
            return StreamLength <= reader.BaseStream.Position;
        }
    }

    private static bool IsWhiteSpace {
        get {
            return (" \t\n\r".IndexOf (PeekChar ()) >= 0);
        }
    }
    private static void EatWhitespace()
    {
        while (!EndOfStream) {
            if (!IsWhiteSpace) {
                break;
            }
            Seek();
        }
    }

    static long cachePosition = -1;
    static string _peekCharCache;
    private static char PeekChar()
    {
        if (cachePosition != reader.BaseStream.Position) {
            cachePosition = reader.BaseStream.Position;
            var bytes = new byte[1] { (byte)reader.PeekChar () };
            _peekCharCache = Encoding.ASCII.GetString (bytes);
        }
        return _peekCharCache.ToCharArray()[0];
    }

    private static string ReadAsciiChar(int size=1)
    {
        var bytes = reader.ReadBytes (size);
        var s = Encoding.ASCII.GetString(bytes);
        return s;
    }

    private static byte ReadByte()
    {
        return reader.ReadByte();
    }

    static bool prevBackslash = false;
    private static string ReadString()
    {
        EatWhitespace ();

        prevBackslash = false;
        List<byte> bytes = new List<byte> ();
        while (!EndOfStream) {
            // 直前にバックスラッシュがある場合はエスケープ文字とする
            if (!prevBackslash && PeekChar() == ')') {
                Seek (1);
                break;
            }

            if (PeekChar() == '\\') {
                prevBackslash = true;
                Seek (1);
                continue;
            }
            prevBackslash = false;

            bytes.Add (ReadByte());
        }

        //UnityEngine.Debug.Log (string.Format ("{0}", string.Join (",", bytes.Select (x => x.ToString ("x02")).ToArray())));
        if (bytes.Count > 2 && bytes [0] == 0xFE && bytes [1] == 0xFF) {
            return Encoding.BigEndianUnicode.GetString (bytes.ToArray ());
        }
        return Encoding.UTF8.GetString(ConvertUtf8(bytes.ToArray ()));
    }


    // TODO: ここの処理すごく怪しいです。
    private static byte[] ConvertUtf8(byte[] org)
    {
        List<byte> newBytes = new List<byte> ();

        Encoding utf16BEEncode = Encoding.GetEncoding ("UTF-16BE", 
                                  new EncoderExceptionFallback (),
                                  new DecoderExceptionFallback ()
                              );
        Encoding utf8BEEncode = Encoding.GetEncoding ("UTF-8", 
            new EncoderExceptionFallback (),
            new DecoderExceptionFallback ()
        );

        bool utf8Zone = true;
        List<byte> utf8Bytes = new List<byte> ();
        for(int i = 0; i < org.Length; )
        {
            int j = 0;
            bool isEncode = false;
            if (utf8Zone) {
                utf8Bytes.Clear ();
                for (j = 0; j <= 2 && i + j < org.Length; ++j) {
                    utf8Bytes.Add (org [i + j]);
                    if (IsUtf8 (utf8Bytes)) {
                        isEncode = true;
                        newBytes.AddRange (utf8Bytes);
                        i += j + 1;
                        break;
                    }
                }
            }

            if(!isEncode) {
                utf8Zone = false;
                if (i + 1 < org.Length) {
                    var changeBytes = Encoding.Convert (utf16BEEncode, utf8BEEncode, org, i, 2);
                    newBytes.AddRange (changeBytes);
                    i += 2;
                } else {
                    var changeBytes = Encoding.Convert (utf16BEEncode, utf8BEEncode, org, i, 1);
                    newBytes.AddRange (changeBytes);
                    i ++;
                }
            }
                
        }

        return newBytes.ToArray ();
    }

    private static bool IsUtf8(List<byte> data)
    {
        if (data.Count == 1) {
            if (data [0] >= 0x00 && data [0] <= 0x7f) {
                return true;
            }
        } else if(data.Count == 2) {
            if (data [0] >= 0xc0 && data [0] <= 0xdf) {
                if (data [1] >= 0x80 && data [1] <= 0xbf) {
                    return true;
                }
            }
        } else if(data.Count == 3) {
            if (data [0] >= 0xe0 && data [0] <= 0xef) {
                if (data [1] >= 0x80 && data [1] <= 0xbf) {
                    if (data [2] >= 0x80 && data [2] <= 0xbf) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static string ReadPropartyName()
    {
        EatWhitespace ();

        List<byte> bytes = new List<byte> ();
        while (!EndOfStream) {
            if (IsWhiteSpace) {
                Seek (1);
                break;
            }
            bytes.Add (ReadByte());
        }
        return Encoding.ASCII.GetString(bytes.ToArray ());
    }

    private static string ReadValue()
    {
        EatWhitespace ();

        List<byte> bytes = new List<byte> ();
        while (!EndOfStream) {
            if (IsWhiteSpace) {
                Seek (1);
                break;
            }
            bytes.Add (ReadByte());
        }
        return Encoding.ASCII.GetString(bytes.ToArray ());
    }

    private static object ReadHashtable()
    {
        Hashtable table = new Hashtable ();
        while(!EndOfStream) {
            var token = Token ();
            if (token == ParseToken.PROPARTY) {
                var prop = ReadPropartyName ();
                var val = ParseValue ();
                //UnityEngine.Debug.Log (string.Format("{0}:{1}", prop, val.ToString()));
                table.Add(prop, val);
            }
            if (token == ParseToken.HASH_CLOSE) {
                break;
            }
        }
        return (object)table;
    }

    private static object ReadArray()
    {
        ArrayList list = new ArrayList ();
        while(!EndOfStream) {
            var token = Token ();
            if (token == ParseToken.ARRAY_CLOSE) {
                break;
            }
            list.Add(ParseValue (token));
        }
        return (object)list;
    }

    private static void Seek(int count=1)
    {
        reader.BaseStream.Seek (count, SeekOrigin.Current);
    }

    private static BinaryReader reader;
}
