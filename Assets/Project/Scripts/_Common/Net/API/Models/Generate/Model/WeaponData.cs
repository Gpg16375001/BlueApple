using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(WeaponDataFormatter))]
	public partial class WeaponData
	{
		// response
		public int WeaponId;
		public long BagId;
		public int Exp;
		public int MaxExp;
		public int Level;
		public int LimitBreakGrade;
		public int Rarity;
		public bool IsEquipped;
		public bool IsLocked;
		public int CardId;
		public int SlotId;
		public string ModificationDate;
		public string CreationDate;

		class WeaponDataFormatter : IMessagePackFormatter<WeaponData>
		{
			public int Serialize(ref byte[] bytes, int offset, WeaponData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 13);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "WeaponId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.WeaponId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BagId");
				offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.BagId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Exp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Exp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MaxExp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MaxExp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Level");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Level);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LimitBreakGrade");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LimitBreakGrade);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Rarity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Rarity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsEquipped");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsEquipped);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "IsLocked");
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.IsLocked);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CardId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.CardId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SlotId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.SlotId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ModificationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ModificationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				return offset - startOffset;
			}

			public WeaponData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				WeaponData ret = new WeaponData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "WeaponId") {
						ret.WeaponId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BagId") {
						ret.BagId = MessagePackBinary.ReadInt64(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Exp") {
						ret.Exp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "MaxExp") {
						ret.MaxExp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Level") {
						ret.Level = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LimitBreakGrade") {
						ret.LimitBreakGrade = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Rarity") {
						ret.Rarity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsEquipped") {
						ret.IsEquipped = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "IsLocked") {
						ret.IsLocked = MessagePackBinary.ReadBoolean(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CardId") {
						ret.CardId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "SlotId") {
						ret.SlotId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ModificationDate") {
						ret.ModificationDate = MessagePackBinary.ReadString(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "CreationDate") {
						ret.CreationDate = MessagePackBinary.ReadString(bytes, offset, out readed);
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
