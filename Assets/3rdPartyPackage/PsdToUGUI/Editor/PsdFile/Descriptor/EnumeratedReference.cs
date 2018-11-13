using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class EnumeratedReference : Descriptor {
        public override string Key {
            get {
                return ReferenceStructure.EnumeratedReferebce;
            }
        }

        public string NameFromClassID;
        public string ClassID;
        public string TypeID;
        public string Enum;

        public EnumeratedReference(PsdBinaryReader reader)
        {
            NameFromClassID = reader.ReadUnicodeString ();

            var length = reader.ReadInt32 ();
            ClassID = reader.ReadAsciiChars (length == 0 ? 4 : length);
            length = reader.ReadInt32 ();
            TypeID = reader.ReadAsciiChars (length == 0 ? 4 : length);
            length = reader.ReadInt32 ();
            Enum = reader.ReadAsciiChars (length == 0 ? 4 : length);
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (ClassID.Length == 4 ? 0 : ClassID.Length);
            writer.WriteAsciiChars (ClassID);
            writer.Write (TypeID.Length == 4 ? 0 : TypeID.Length);
            writer.WriteAsciiChars (TypeID);
            writer.Write (Enum.Length == 4 ? 0 : Enum.Length);
            writer.WriteAsciiChars (Enum);
        }

        public override string ToString ()
        {
            return string.Format ("[EnumeratedReference: Key={0} NameFromClassID={1} ClassID={2} TypeID={3} Enum={4}]",
                Key, NameFromClassID, ClassID, TypeID, Enum);
        }
    }
}