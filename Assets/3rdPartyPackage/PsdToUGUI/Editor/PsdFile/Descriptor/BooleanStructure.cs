using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class BooleanStructure : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.Booolean;
            }
        }

        public bool Value;
        public BooleanStructure(PsdBinaryReader reader)
        {
            Value = reader.ReadBoolean ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Value);
        }

        public override string ToString ()
        {
            return string.Format ("[BooleanStructure: Key={0} Value={1}]", Key, Value);
        }
    }
}
