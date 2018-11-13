using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public class DescriptorStructure : Descriptor {
        private string _key;
        public override string Key {
            get {
                return _key;
            }
        }
        public string ClassIDName;
        public string ClassID;
        public Dictionary<string, Descriptor> Descriptor;

        public Descriptor this[string key] {
            get {
                if (Descriptor.ContainsKey (key)) {
                    return Descriptor[key];
                }
                return null;
            }
        }

        public DescriptorStructure(PsdBinaryReader reader, string key = null)
        {
            _key = key;
            ClassIDName = reader.ReadUnicodeString ();
            var length = reader.ReadInt32 ();
            ClassID = reader.ReadAsciiChars (length == 0 ? 4 : length);

            var itemCount = reader.ReadInt32 ();
            Descriptor = new Dictionary<string, Descriptor> ();
            for (int i = 0; i < itemCount; i++) {
                length = reader.ReadInt32 ();
                string itemName = reader.ReadAsciiChars (length == 0 ? 4 : length);

                Descriptor.Add (itemName, DescriptorFactory.Load(reader));
            }
        }

        public T TryCastDiscriptor<T>(string key) where T: Descriptor
        {
            if (Descriptor.ContainsKey (key)) {
                return Descriptor [key] as T;
            }
            return null;
        }

        public System.Type GetType(string key)
        {
            if (Descriptor.ContainsKey (key)) {
                switch (Descriptor [key].Key) {
                case DescriptorFactory.Reference:
                    return typeof(ReferenceStructure);
                case DescriptorFactory.Descriptor:
                case DescriptorFactory.GlobalObject:
                    return typeof(DescriptorStructure);
                case DescriptorFactory.List:
                    return typeof(ListStructure);
                case DescriptorFactory.Double:
                    return typeof(DoubleStructure);
                case DescriptorFactory.UnitFloat:
                    return typeof(UnitFloatStructure);
                case DescriptorFactory.String:
                    return typeof(StringStructure);
                case DescriptorFactory.Enumerated:
                    return typeof(EnumeratedDescriptor);
                case DescriptorFactory.Integer:
                    return typeof(Integer);
                case DescriptorFactory.LargeInteger:
                    return typeof(LargeInteger);
                case DescriptorFactory.Booolean:
                    return typeof(BooleanStructure);
                case DescriptorFactory.Class1:
                case DescriptorFactory.Class2:
                    return typeof(ClassStructure);
                case DescriptorFactory.Alias:
                    return typeof(AliasStructure);
                case DescriptorFactory.RowData:
                    return typeof(RowData);
                }
            }
            return null;
        }

        public override void WriteData (PsdBinaryWriter writer)
        {
            if (!string.IsNullOrEmpty(Key)) {
                writer.WriteAsciiChars (Key);
            }

            writer.WriteUnicodeString (ClassIDName);

            if (ClassID.Length != 4) {
                writer.Write (ClassID.Length);
            } else {
                writer.Write (0);
            }
            writer.WriteAsciiChars (ClassID);

            writer.Write (Descriptor.Count);
            foreach (var pair in Descriptor) {
                writer.Write (pair.Key.Length == 4 ? 0 : pair.Key.Length);
                writer.WriteAsciiChars (pair.Key);
                pair.Value.WriteData (writer);
            }
        }

        public override string ToString ()
        {
            return string.Format ("[DescriptorStructure: Key={0} ClassIDName={1} ClassID={2}]", Key, ClassIDName, ClassID);
        }
    }
}
