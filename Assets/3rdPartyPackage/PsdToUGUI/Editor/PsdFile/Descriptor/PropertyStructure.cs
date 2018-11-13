using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class PropertyStructure : Descriptor {
        public override string Key {
            get {
                return ReferenceStructure.Property;
            }
        }

        public string NameFromClassID;
        public string ClassID;
        public string KeyID;

        public PropertyStructure(PsdBinaryReader reader)
        {
            NameFromClassID = reader.ReadUnicodeString ();

            int length = reader.ReadInt32 ();
            ClassID = reader.ReadAsciiChars (length == 0 ? 4 : length);

            length = reader.ReadInt32 ();
            KeyID = reader.ReadAsciiChars (length == 0 ? 4 : length);
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.WriteUnicodeString (NameFromClassID);
            writer.Write (ClassID.Length == 4 ? 0 : ClassID.Length);
            writer.WriteAsciiChars (ClassID);
            writer.Write (KeyID.Length == 4 ? 0 : KeyID.Length);
            writer.WriteAsciiChars (KeyID);
        }

        public override string ToString ()
        {
            return string.Format ("[PropertyStructure: Key={0} NameFromClassID={1} ClassID={2} KeyID={3}]", Key, NameFromClassID, ClassID, KeyID);
        }
    }
}
