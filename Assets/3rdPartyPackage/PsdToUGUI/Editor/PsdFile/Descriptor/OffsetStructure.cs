using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class OffsetStructure : Descriptor {
        public override string Key {
            get {
                return ReferenceStructure.Offset;
            }
        }

        public string NameFromClassID;
        public string ClassID;
        public int Offset;

        public OffsetStructure(PsdBinaryReader reader)
        {
            NameFromClassID = reader.ReadUnicodeString ();

            var length = reader.ReadInt32 ();
            ClassID = reader.ReadAsciiChars (length == 0 ? 4 : length);

            Offset = reader.ReadInt32 ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.WriteUnicodeString (NameFromClassID);
            writer.Write (ClassID.Length == 4 ? 0 : ClassID.Length);
            writer.WriteAsciiChars (ClassID);
            writer.Write (Offset);
        }

        public override string ToString ()
        {
            return string.Format ("[OffsetStructure: Key={0} NameFromClassID={1} ClassID={2} Offset={3}]", Key, NameFromClassID, ClassID, Offset);
        }
    }
}
