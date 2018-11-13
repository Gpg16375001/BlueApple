using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(SupporterCardDataFormatter))]
	public partial class SupporterCardData
	{
		// response
		public int CardId;
		public int Exp;
		public int LimitBreakGrade;
		public BoardData[] BoardDataList;
		public int Rarity;
		public WeaponData EquippedWeaponData;
		public MagikiteData[] EquippedMagikiteDataList;
		public string ModificationDate;
		public string CreationDate;

		class SupporterCardDataFormatter : IMessagePackFormatter<SupporterCardData>
		{
			public int Serialize(ref byte[] bytes, int offset, SupporterCardData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 9);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CardId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.CardId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Exp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Exp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "LimitBreakGrade");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.LimitBreakGrade);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "BoardDataList");
				if(value.BoardDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.BoardDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.BoardDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<BoardData> ().Serialize (ref bytes, offset, value.BoardDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Rarity");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Rarity);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EquippedWeaponData");
				if(value.EquippedWeaponData == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					offset += formatterResolver.GetFormatter<WeaponData> ().Serialize (ref bytes, offset, value.EquippedWeaponData, formatterResolver);
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EquippedMagikiteDataList");
				if(value.EquippedMagikiteDataList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.EquippedMagikiteDataList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						if(value.EquippedMagikiteDataList[i] == null) {
							offset += MessagePackBinary.WriteNil (ref bytes, offset);
						} else {
							offset += formatterResolver.GetFormatter<MagikiteData> ().Serialize (ref bytes, offset, value.EquippedMagikiteDataList[i], formatterResolver);
						}
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ModificationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ModificationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				return offset - startOffset;
			}

			public SupporterCardData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				SupporterCardData ret = new SupporterCardData();

				var headerCount = MessagePackBinary.ReadMapHeader (bytes, offset, out readed);
				offset += readed;
				for (int i = 0; i < headerCount; ++i) {
					string key = MessagePackBinary.ReadString (bytes, offset, out readed);
					offset += readed;
					bool isRead = false;
					if (key == "CardId") {
						ret.CardId = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "Exp") {
						ret.Exp = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "LimitBreakGrade") {
						ret.LimitBreakGrade = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "BoardDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.BoardDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.BoardDataList = new BoardData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.BoardDataList[arrayIndex] = null;
								} else {
									ret.BoardDataList[arrayIndex] = formatterResolver.GetFormatter<BoardData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
						isRead = true;
					}
					if (key == "Rarity") {
						ret.Rarity = MessagePackBinary.ReadInt32(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "EquippedWeaponData") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.EquippedWeaponData = null;
						} else {
							ret.EquippedWeaponData = formatterResolver.GetFormatter<WeaponData> ().Deserialize (bytes, offset, formatterResolver, out readed);
							offset += readed;
						}
						isRead = true;
					}
					if (key == "EquippedMagikiteDataList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.EquippedMagikiteDataList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.EquippedMagikiteDataList = new MagikiteData[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								if (MessagePackBinary.IsNil (bytes, offset)) {
									offset += 1;
									ret.EquippedMagikiteDataList[arrayIndex] = null;
								} else {
									ret.EquippedMagikiteDataList[arrayIndex] = formatterResolver.GetFormatter<MagikiteData> ().Deserialize (bytes, offset, formatterResolver, out readed);
									offset += readed;
								}
							}
						}
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
