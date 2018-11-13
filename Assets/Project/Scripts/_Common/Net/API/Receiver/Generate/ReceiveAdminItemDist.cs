using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveAdminItemDistFormatter))]
	public partial class ReceiveAdminItemDist : BaseReceiveAPI
	{
		// response
		public int Dummy;

		class ReceiveAdminItemDistFormatter : IMessagePackFormatter<ReceiveAdminItemDist>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveAdminItemDist value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 1);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Dummy");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Dummy);
				return offset - startOffset;
			}

			public ReceiveAdminItemDist Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveAdminItemDist ret = new ReceiveAdminItemDist();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "Dummy") {
						ret.Dummy = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if(!isRead) {
						readed = MessagePackBinary.ReadNextBlock (bytes, offset);
						offset += readed;
					}
				}
				readSize = offset - startOffset;
				return ret;
			}
		}
	}
}
