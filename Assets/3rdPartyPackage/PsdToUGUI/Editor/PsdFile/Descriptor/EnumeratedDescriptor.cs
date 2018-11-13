using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class EnumeratedDescriptor : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.Enumerated;
            }
        }

        public string Type;
        public string Enum;
        public EnumeratedDescriptor(PsdBinaryReader reader)
        {
            int length = reader.ReadInt32 ();
            Type = reader.ReadAsciiChars (length == 0 ? 4 : length);
            length = reader.ReadInt32 ();
            Enum = reader.ReadAsciiChars (length == 0 ? 4 : length);
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Type.Length == 4 ? 0 : Type.Length);
            writer.WriteAsciiChars (Type);
            writer.Write (Enum.Length == 4 ? 0 : Enum.Length);
            writer.WriteAsciiChars (Enum);
        }

        public override string ToString ()
        {
            return string.Format ("[EnumeratedDescriptor: Key={0} Type={1} Enum={2}]", Key, Type, Enum);
        }
    }
}
