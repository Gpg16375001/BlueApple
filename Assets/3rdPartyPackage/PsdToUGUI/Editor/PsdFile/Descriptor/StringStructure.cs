using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class StringStructure : Descriptor {
        private string _key;
        public override string Key {
            get {
                return _key;
            }
        }

        public string Text;
        public StringStructure(PsdBinaryReader reader, string key)
        {
            _key = key;
            Text = reader.ReadUnicodeString ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.WriteUnicodeString (Text);
        }

        public override string ToString ()
        {
            return string.Format ("[StringStructure: Key={0} Text={1}]", Key, Text);
        }
    }
}