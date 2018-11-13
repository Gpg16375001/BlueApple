using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveCognitosyncwatchdogIndexFormatter))]
	public partial class ReceiveCognitosyncwatchdogIndex : BaseReceiveAPI
	{
		// response
		public string Token;
		public string IdentityId;

		class ReceiveCognitosyncwatchdogIndexFormatter : IMessagePackFormatter<ReceiveCognitosyncwatchdogIndex>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveCognitosyncwatchdogIndex value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 3);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Token");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.Token);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IdentityId");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.IdentityId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				return offset - startOffset;
			}

			public ReceiveCognitosyncwatchdogIndex Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveCognitosyncwatchdogIndex ret = new ReceiveCognitosyncwatchdogIndex();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "Token") {
						ret.Token = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IdentityId") {
						ret.IdentityId = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MasterVersion") {
						ret.MasterVersion = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
