using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class ReferenceStructure : Descriptor
    {
        public const string Property            = "prop";
        public const string Class               = "Clss";
        public const string EnumeratedReferebce = "Enmr";
        public const string Offset              = "rele";
        public const string Identifier          = "Idnt";
        public const string Index               = "indx";
        public const string Name                = "name";

        public override string Key {
            get {
                return DescriptorFactory.Reference;
            }
        }

        public Descriptor[] References;

        public ReferenceStructure(PsdBinaryReader reader)
        {
            int itemCount = reader.ReadInt32 ();

            References = new Descriptor[itemCount];
            for (int i = 0; i < itemCount; ++i) {
                var key = reader.ReadAsciiChars (4);
                Descriptor item = null;
                switch (key) {
                case Property:
                    item = new PropertyStructure (reader);
                    break;

                case Class:
                    item = new ClassStructure (reader, key);
                    break;

                case EnumeratedReferebce:
                    item = new EnumeratedReference (reader);
                    break;

                case Offset:
                    item = new OffsetStructure (reader);
                    break;

                case Identifier:
                    item = new Integer (reader, key);
                    break;

                case Index:
                    item = new Integer (reader, key);
                    break;

                case Name:
                    item = new StringStructure (reader, key);
                    break;
                }
                References [i] = item;
            }
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            writer.WriteAsciiChars (Key);
            writer.Write (References.Length);
            for (int i = 0; i < References.Length; ++i) {
                References [i].WriteData (writer);
            }
        }

        public override string ToString ()
        {
            return string.Format ("[ReferenceStructure: Key={0}]", Key);
        }
    }
}
