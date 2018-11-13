using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class LargeInteger : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.LargeInteger;
            }
        }

        public long Value;
        public LargeInteger(PsdBinaryReader reader)
        {
            Value = reader.ReadInt64 ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Value);
        }

        public override string ToString ()
        {
            return string.Format ("[LargeInteger: Key={0} Value={1}]", Key, Value);
        }
    }
}