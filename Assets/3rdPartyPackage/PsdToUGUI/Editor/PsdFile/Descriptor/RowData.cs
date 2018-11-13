using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class RowData : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.RowData;
            }
        }

        public byte[] Data;
        public RowData(PsdBinaryReader reader)
        {
            int length = reader.ReadInt32 ();
            Data = reader.ReadBytes (length);
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Data.Length);
            writer.Write (Data);
        }

        public override string ToString ()
        {
            return string.Format ("[RowData: Key={0} DataLength={1}]", Key, Data.Length);
        }
    }
}