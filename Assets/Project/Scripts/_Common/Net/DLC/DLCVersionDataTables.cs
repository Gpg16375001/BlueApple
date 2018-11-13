using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using System.Linq;

[MessagePackFormatter(typeof(DLCVersionDataTablesFormatter))]
public class DLCVersionDataTables
{
	public DLCVersionData[] VersionDataList;

	public DLCVersionDataTables()
	{
		VersionDataList = new DLCVersionData[0];
	}

	public DLCVersionDataTables(IEnumerable<DLCVersionData> list)
	{
		VersionDataList = list.ToArray ();
	}

	class DLCVersionDataTablesFormatter : IMessagePackFormatter<DLCVersionDataTables>
	{
		public int Serialize(ref byte[] bytes, int offset, DLCVersionDataTables value, IFormatterResolver formatterResolver) {
			var startOffset = offset;
			var count = value.VersionDataList.Length;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
			for (int i = 0; i < count; ++i) {
				if(value.VersionDataList[i] == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<DLCVersionData> ().Serialize (ref bytes, offset, value.VersionDataList [i], formatterResolver);
				}
			}
			return offset - startOffset;
		}

		public DLCVersionDataTables Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
			DLCVersionDataTables ret = new DLCVersionDataTables ();

			int startOffset = offset;
			int readed = 0;

			var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
			offset += readed;
			ret.VersionDataList = new DLCVersionData[count];
			for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					offset += 1;
					ret.VersionDataList[arrayIndex] = null;
				} else {
					ret.VersionDataList[arrayIndex] = formatterResolver.GetFormatter<DLCVersionData> ().Deserialize (bytes, offset, formatterResolver, out readed);
					offset += readed;
				}
			}
			readSize = offset - startOffset;
			return ret;
		}
	}
}
