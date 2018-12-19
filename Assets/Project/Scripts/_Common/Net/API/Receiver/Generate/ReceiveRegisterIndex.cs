using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveRegisterIndexFormatter))]
	public partial class ReceiveRegisterIndex : BaseReceiveAPI
	{
		// response
		public string AuthUsername;
		public string AuthPassword;
		public int UserId;
		public string CustomerId;
		public string IdentityId;

		class ReceiveRegisterIndexFormatter : IMessagePackFormatter<ReceiveRegisterIndex>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveRegisterIndex value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 8);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "AuthUsername");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.AuthUsername);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "AuthPassword");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.AuthPassword);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "UserId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.UserId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CustomerId");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CustomerId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IdentityId");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.IdentityId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceiveRegisterIndex Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveRegisterIndex ret = new ReceiveRegisterIndex();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "AuthUsername") {
						ret.AuthUsername = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "AuthPassword") {
						ret.AuthPassword = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "UserId") {
						ret.UserId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CustomerId") {
						ret.CustomerId = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IdentityId") {
						ret.IdentityId = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ResultCode") {
						ret.ResultCode = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
