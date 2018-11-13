using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace SmileLab.Net.API
{
	[Serializable]
	[MessagePackFormatter(typeof(CardDataFormatter))]
	public partial class CardData
	{
		// response
		public int CardId;
		public int Exp;
		public int Level;
		public int LimitBreakGrade;
		public BoardData[] BoardDataList;
		public int Rarity;
		public long EquippedWeaponBagId;
		public long[] EquippedMagikiteBagIdList;
		public string ModificationDate;
		public string CreationDate;

		class CardDataFormatter : IMessagePackFormatter<CardData>
		{
			public int Serialize(ref byte[] bytes, int offset, CardData value, IFormatterResolver formatterResolver) {
				if (value == null) {
					return MessagePackBinary.WriteNil (ref bytes, offset);
				}

				var startOffset = offset;
				offset += MessagePackBinary.WriteMapHeader (ref bytes, offset, 10);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CardId");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.CardId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Exp");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Exp);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "Level");
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Level);
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
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EquippedWeaponBagId");
				offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.EquippedWeaponBagId);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "EquippedMagikiteBagIdList");
				if(value.EquippedMagikiteBagIdList == null) {
					offset += MessagePackBinary.WriteNil (ref bytes, offset);
				} else {
					var count = value.EquippedMagikiteBagIdList.Length;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
					for(int i = 0; i < count; ++i) {
						offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.EquippedMagikiteBagIdList[i]);
					}
				}
				offset += MessagePackBinary.WriteString (ref bytes, offset, "ModificationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.ModificationDate);
				offset += MessagePackBinary.WriteString (ref bytes, offset, "CreationDate");
				offset += MessagePackBinary.WriteString(ref bytes, offset, value.CreationDate);
				return offset - startOffset;
			}

			public CardData Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize) {
				if (MessagePackBinary.IsNil (bytes, offset)) {
					readSize = 1;
					return null;
				}

				int readed = 0;
				var startOffset = offset;
				CardData ret = new CardData();

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
					if (key == "EquippedWeaponBagId") {
						ret.EquippedWeaponBagId = MessagePackBinary.ReadInt64(bytes, offset, out readed);
						offset += readed;
						isRead = true;
					}
					if (key == "EquippedMagikiteBagIdList") {
						if (MessagePackBinary.IsNil (bytes, offset)) {
							offset += 1;
							ret.EquippedMagikiteBagIdList = null;
						} else {
							var count = MessagePackBinary.ReadArrayHeader(bytes, offset, out readed);
							offset += readed;
							ret.EquippedMagikiteBagIdList = new long[count];
							for(int arrayIndex = 0; arrayIndex < count; ++arrayIndex) {
								ret.EquippedMagikiteBagIdList[arrayIndex] = MessagePackBinary.ReadInt64(bytes, offset, out readed);
								offset += readed;
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
