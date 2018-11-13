using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(MagikiteDataFormatter))]
	public partial class MagikiteData
	{
		// response
		public int MagikiteId;
		public long BagId;
		public int HPGain;
		public int ATKGain;
		public int DEFGain;
		public int SPDGain;
		public bool IsEquipped;
		public bool IsLocked;
		public int CardId;
		public int SlotId;
		public string ModificationDate;
		public string CreationDate;
		public int Rarity;

		class MagikiteDataFormatter : IMessagePackFormatter<MagikiteData>
		{
			public int Serialize(ref byte[] bytes, int offset, MagikiteData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 13);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "MagikiteId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.MagikiteId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BagId");
				offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.BagId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "HPGain");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.HPGain);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ATKGain");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.ATKGain);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "DEFGain");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.DEFGain);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "SPDGain");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.SPDGain);
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
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Rarity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Rarity);
				return offset - startOffset;
			}

			public MagikiteData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				MagikiteData ret = new MagikiteData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "MagikiteId") {
						ret.MagikiteId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BagId") {
						ret.BagId = MessagePackBinary.ReadInt64(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "HPGain") {
						ret.HPGain = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "ATKGain") {
						ret.ATKGain = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "DEFGain") {
						ret.DEFGain = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "SPDGain") {
						ret.SPDGain = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
					if (key == "Rarity") {
						ret.Rarity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
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
