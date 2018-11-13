using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class DoubleStructure : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.Double;
            }
        }
            
        public double Value;

        public DoubleStructure(PsdBinaryReader reader)
        {
            Value = reader.ReadDouble ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Value);
        }

        public override string ToString ()
        {
            return string.Format ("[DoubleStructure: Key={0} Value={1}]", Key, Value);
        }
    }
}
