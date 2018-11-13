using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class ClassStructure : Descriptor {
        private string _key;
        public override string Key {
            get {
                return _key;
            }
        }

        public string NameFromClassID;
        public string ClassID;

        public ClassStructure(PsdBinaryReader reader, string key)
        {
            _key = key;

            NameFromClassID = reader.ReadUnicodeString ();
            int length = reader.ReadInt32 ();
            ClassID = reader.ReadAsciiChars (length == 0 ? 4 : length);
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.WriteUnicodeString (NameFromClassID);
            writer.Write (ClassID.Length == 4 ? 0 : ClassID.Length);
            writer.WriteAsciiChars (ClassID);
        }

        public override string ToString ()
        {
            return string.Format ("[ClassStructure: Key={0} NameFromClassID={1} ClassID={2}]", Key, NameFromClassID, ClassID);
        }
    }
}