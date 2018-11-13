using System;

namespace PhotoshopFile
{
    public class LayerTypeToolInfo : LayerInfo
    {
        public override string Key
        {
            get { return "TySh"; }
        }


        public short Version;
        public double xx;
        public double xy;
        public double yx;
        public double yy;
        public double tx;
        public double ty;
        public short TextVersion;
        public int TextDescriptorVestion;
        public DescriptorStructure TextData;
        public short WarpVersion;
        public int WarpDescriptorVestion;
        public DescriptorStructure WarpData;
        public double left;
        public double top;
        public double right;
        public double bottom;
        public LayerTypeToolInfo()
        {
        }

        public LayerTypeToolInfo(PsdBinaryReader reader)
        {
            Version = reader.ReadInt16 ();
            xx = reader.ReadDouble ();
            xy = reader.ReadDouble ();
            yx = reader.ReadDouble ();
            yy = reader.ReadDouble ();
            tx = reader.ReadDouble ();
            ty = reader.ReadDouble ();
            TextVersion = reader.ReadInt16 ();
            TextDescriptorVestion = reader.ReadInt32 ();
            TextData = new DescriptorStructure (reader);
            WarpVersion = reader.ReadInt16 ();
            WarpDescriptorVestion = reader.ReadInt32 ();
            WarpData = new DescriptorStructure (reader);
            left = reader.ReadInt32 ();
            top = reader.ReadInt32 ();
            right = reader.ReadInt32 ();
            bottom = reader.ReadInt32 ();
        }

        protected override void WriteData(PsdBinaryWriter writer)
        {
            writer.Write (Version);
            writer.Write (xx);
            writer.Write (xy);
            writer.Write (yx);
            writer.Write (yy);
            writer.Write (tx);
            writer.Write (ty);
            writer.Write (TextVersion);
            writer.Write (TextDescriptorVestion);
            TextData.WriteData (writer);
            writer.Write (WarpVersion);
            writer.Write (WarpDescriptorVestion);
            WarpData.WriteData (writer);
            writer.Write (left);
            writer.Write (top);
            writer.Write (right);
            writer.Write (bottom);
        }
    }
}