using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class AliasStructure : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.Alias;
            }
        }

        public string Path;
        public AliasStructure(PsdBinaryReader reader)
        {
            var length = reader.ReadInt32 ();
            Path = reader.ReadAsciiChars (length);
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (Path.Length);
            writer.WriteAsciiChars (Path);
        }

        public override string ToString ()
        {
            return string.Format ("[AliasStructure: Key={0} Path={1}]", Key, Path);
        }
    }
}