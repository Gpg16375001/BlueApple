using System.Collections;
using System.Collections.Generic;

namespace PhotoshopFile
{
    public static class DescriptorFactory {
        
        public const string Reference    = "obj ";
        public const string Descriptor   = "Objc";
        public const string List         = "VlLs";
        public const string Double       = "doub";
        public const string UnitFloat    = "UntF";
        public const string String       = "TEXT";
        public const string Enumerated   = "enum";
        public const string Integer      = "long";
        public const string LargeInteger = "comp";
        public const string Booolean     = "bool";
        public const string GlobalObject = "GlbO";
        public const string Class1       = "type";
        public const string Class2       = "GlbC";
        public const string Alias        = "alis";
        public const string RowData      = "tdta";

        public static Descriptor Load(PsdBinaryReader reader)
        {
            string key = reader.ReadAsciiChars (4);
            return Load (key, reader);
        }

        public static Descriptor Load(string key, PsdBinaryReader reader)
        {
            Descriptor result = null;
            switch (key) {
            case Reference:
                result = new ReferenceStructure (reader);
                break;
            case Descriptor:
            case GlobalObject:
                result = new DescriptorStructure (reader, key);
                break;
            case List:
                result = new ListStructure (reader);
                break;
            case Double:
                result = new DoubleStructure (reader);
                break;
            case UnitFloat:
                result = new UnitFloatStructure (reader);
                break;
            case String:
                result = new StringStructure (reader, key);
                break;
            case Enumerated:
                result = new EnumeratedDescriptor (reader);
                break;
            case Integer:
                result = new Integer (reader, key);
                break;
            case LargeInteger:
                result = new LargeInteger (reader);
                break;
            case Booolean:
                result = new BooleanStructure (reader);
                break;
            case Class1:
            case Class2:
                result = new ClassStructure (reader, key);
                break;
            case Alias:
                result = new AliasStructure (reader);
                break;
            case RowData:
                result = new RowData (reader);
                break;
            }
            return result;
        }
    }

    public abstract class Descriptor {
        public abstract string Key { get; }
        public abstract void WriteData(PsdBinaryWriter writer);
    }
}