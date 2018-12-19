using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveFgidGetLoginInfoFormatter))]
	public partial class ReceiveFgidGetLoginInfo : BaseReceiveAPI
	{
		// response
		public string LoginUrl;
		public string GameToken;
		public bool IsAssociated;
		public string CustomerId;
		public string Nickname;
		public int Exp;

		class ReceiveFgidGetLoginInfoFormatter : IMessagePackFormatter<ReceiveFgidGetLoginInfo>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveFgidGetLoginInfo value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 9);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LoginUrl");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.LoginUrl);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "GameToken");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.GameToken);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsAssociated");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsAssociated);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CustomerId");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CustomerId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Nickname");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.Nickname);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Exp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Exp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceiveFgidGetLoginInfo Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveFgidGetLoginInfo ret = new ReceiveFgidGetLoginInfo();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "ResultCode") {
						ret.ResultCode = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LoginUrl") {
						ret.LoginUrl = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "GameToken") {
						ret.GameToken = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsAssociated") {
						ret.IsAssociated = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CustomerId") {
						ret.CustomerId = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Nickname") {
						ret.Nickname = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Exp") {
						ret.Exp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MasterVersion") {
						ret.MasterVersion = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ErrorMessage") {
						ret.ErrorMessage = MessagePackBinary.ReadString(bytes, offset, out readed);
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
