using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class UnitFloatStructure : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.UnitFloat;
            }
        }

        public string Unit;
        public double Value;

        public UnitFloatStructure(PsdBinaryReader reader)
        {
            Unit = reader.ReadAsciiChars (4);
            Value = reader.ReadDouble ();
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.WriteAsciiChars (Unit);
            writer.Write (Value);
        }

        public override string ToString ()
        {
            return string.Format ("[UnitFloatStructure: Key={0} Unit={1} Value={2}]", Key, Unit, Value);
        }
    }
}
