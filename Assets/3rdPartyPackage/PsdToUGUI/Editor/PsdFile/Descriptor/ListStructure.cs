using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PhotoshopFile
{
    public class ListStructure : Descriptor {
        public override string Key {
            get {
                return DescriptorFactory.List;
            }
        }

        public Descriptor[] List;
        public ListStructure(PsdBinaryReader reader)
        {
            int number = reader.ReadInt32 ();
            List = new Descriptor[number];

            for (int i = 0; i < number; ++i) {
                List[i] = DescriptorFactory.Load (reader);
            }
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (List.Length);
            for (int i = 0; i < List.Length; ++i) {
                List [i].WriteData (writer);
            }
        }

        public override string ToString ()
        {
            return string.Format ("[ListStructure: Key={0} ListCount={1} List={2}]", Key, List.Length, string.Join(" ", List.Select(x => x.ToString()).ToArray()));
        }
    }
}