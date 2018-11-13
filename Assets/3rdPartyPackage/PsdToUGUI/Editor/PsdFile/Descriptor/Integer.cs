using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class Integer : Descriptor {
        private string _key;
        public override string Key {
            get {
                return _key;
            }
        }

        public int Value;
        public Integer(PsdBinaryReader reader, string key)
        {
            _key = key;
            Value = reader.ReadInt32 ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Value);
        }

        public override string ToString ()
        {
            return string.Format ("[Integer: Key={0} Value={1}]", Key, Value);
        }
    }
}