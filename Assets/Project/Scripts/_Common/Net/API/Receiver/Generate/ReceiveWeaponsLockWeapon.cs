﻿using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[MessagePackFormatter(typeof(ReceiveWeaponsLockWeaponFormatter))]
	public partial class ReceiveWeaponsLockWeapon : BaseReceiveAPI
	{
		// response
		public WeaponData WeaponData;

		class ReceiveWeaponsLockWeaponFormatter : IMessagePackFormatter<ReceiveWeaponsLockWeapon>
		{
			public int Serialize(ref byte[] bytes, int offset, ReceiveWeaponsLockWeapon value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 4);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ResultCode");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ResultCode);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WeaponData");
				if(value.WeaponData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<WeaponData> ().Serialize (ref bytes, offset, value.WeaponData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MasterVersion");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MasterVersion);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ErrorMessage");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ErrorMessage);
				return offset - startOffset;
			}

			public ReceiveWeaponsLockWeapon Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				ReceiveWeaponsLockWeapon ret = new ReceiveWeaponsLockWeapon();

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
					if (key == "WeaponData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.WeaponData = null;
						} else {
							ret.WeaponData = formatterResolver.GetFormatter<WeaponData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
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
