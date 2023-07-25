using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Chineser
{
    internal partial class PinYinConverter
    {
        public static String CHARACTER_NOT_SUPPORTED { get => "该字符不在简体中文扩展字符集中。"; }
        public static String EXCEED_BORDER_EXCEPTION { get => "笔画数不能为负数或大于汉字的最大笔画数。"; }
        public static String INDEX_OUT_OF_RANGE { get => "索引超出数组范围。"; }

		internal class CharDictionary
		{
			internal int Length;

			internal int Count;

			internal short Offset;

			internal readonly byte[] Reserved = new byte[24];

			internal List<CharUnit> CharUnitTable;

			internal readonly short EndMark = 32767;

			internal void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Length);
				binaryWriter.Write(this.Count);
				binaryWriter.Write(this.Offset);
				binaryWriter.Write(this.Reserved);
				for (int i = 0; i < this.Count; i++)
				{
					this.CharUnitTable[i].Serialize(binaryWriter);
				}
				binaryWriter.Write(this.EndMark);
			}

			internal static CharDictionary Deserialize(BinaryReader binaryReader)
			{
				CharDictionary charDictionary = new CharDictionary();
				//binaryReader.ReadInt32();
				charDictionary.Length = binaryReader.ReadInt32();
				charDictionary.Count = binaryReader.ReadInt32();
				charDictionary.Offset = binaryReader.ReadInt16();
				binaryReader.ReadBytes(24);
				charDictionary.CharUnitTable = new List<CharUnit>();
				for (int i = 0; i < charDictionary.Count; i++)
				{
					charDictionary.CharUnitTable.Add(CharUnit.Deserialize(binaryReader));
				}
				binaryReader.ReadInt16();
				return charDictionary;
			}

			internal CharUnit GetCharUnit(int index)
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index", INDEX_OUT_OF_RANGE);
				}
				return this.CharUnitTable[index];
			}

			internal CharUnit GetCharUnit(char ch)
			{
				CharUnitPredicate @object = new CharUnitPredicate(ch);
				return this.CharUnitTable.Find(new Predicate<CharUnit>(@object.Match));
			}
		}
		internal class CharUnit
		{
			internal char Char;

			internal byte StrokeNumber;

			internal byte PinyinCount;

			internal short[] PinyinIndexList;

			internal static CharUnit Deserialize(BinaryReader binaryReader)
			{
				CharUnit charUnit = new CharUnit();
				charUnit.Char = binaryReader.ReadChar();
				charUnit.StrokeNumber = binaryReader.ReadByte();
				charUnit.PinyinCount = binaryReader.ReadByte();
				charUnit.PinyinIndexList = new short[(int)charUnit.PinyinCount];
				for (int i = 0; i < (int)charUnit.PinyinCount; i++)
				{
					charUnit.PinyinIndexList[i] = binaryReader.ReadInt16();
				}
				return charUnit;
			}

			internal void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Char);
				binaryWriter.Write(this.StrokeNumber);
				binaryWriter.Write(this.PinyinCount);
				for (int i = 0; i < (int)this.PinyinCount; i++)
				{
					binaryWriter.Write(this.PinyinIndexList[i]);
				}
			}
		}
		internal class CharUnitPredicate
		{
			private char ExpectedChar;

			internal CharUnitPredicate(char ch)
			{
				this.ExpectedChar = ch;
			}

			internal bool Match(CharUnit charUnit)
			{
				return charUnit.Char == this.ExpectedChar;
			}
		}
		internal class HomophoneDictionary
		{
			internal int Length;

			internal short Offset;

			internal short Count;

			internal readonly byte[] Reserved = new byte[8];

			internal List<HomophoneUnit> HomophoneUnitTable;

			internal readonly short EndMark = 32767;

			internal void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Length);
				binaryWriter.Write(this.Count);
				binaryWriter.Write(this.Offset);
				binaryWriter.Write(this.Reserved);
				for (int i = 0; i < (int)this.Count; i++)
				{
					this.HomophoneUnitTable[i].Serialize(binaryWriter);
				}
				binaryWriter.Write(this.EndMark);
			}

			internal static HomophoneDictionary Deserialize(BinaryReader binaryReader)
			{
				HomophoneDictionary homophoneDictionary = new HomophoneDictionary();
				// binaryReader.ReadInt32();
				homophoneDictionary.Length = binaryReader.ReadInt32();
				homophoneDictionary.Count = binaryReader.ReadInt16();
				homophoneDictionary.Offset = binaryReader.ReadInt16();
				binaryReader.ReadBytes(8);
				homophoneDictionary.HomophoneUnitTable = new List<HomophoneUnit>();
				for (int i = 0; i < (int)homophoneDictionary.Count; i++)
				{
					homophoneDictionary.HomophoneUnitTable.Add(HomophoneUnit.Deserialize(binaryReader));
				}
				binaryReader.ReadInt16();
				return homophoneDictionary;
			}

			internal HomophoneUnit GetHomophoneUnit(int index)
			{
				if (index < 0 || index >= (int)this.Count)
				{
					throw new ArgumentOutOfRangeException("index", INDEX_OUT_OF_RANGE);
				}
				return this.HomophoneUnitTable[index];
			}

			internal HomophoneUnit GetHomophoneUnit(PinyinDictionary pinyinDictionary, string pinyin)
			{
				return this.GetHomophoneUnit(pinyinDictionary.GetPinYinUnitIndex(pinyin));
			}
		}
		internal class HomophoneUnit
		{
			internal short Count;

			internal char[] HomophoneList;

			internal static HomophoneUnit Deserialize(BinaryReader binaryReader)
			{
				HomophoneUnit homophoneUnit = new HomophoneUnit();
				homophoneUnit.Count = binaryReader.ReadInt16();
				homophoneUnit.HomophoneList = new char[(int)homophoneUnit.Count];
				for (int i = 0; i < (int)homophoneUnit.Count; i++)
				{
					homophoneUnit.HomophoneList[i] = binaryReader.ReadChar();
				}
				return homophoneUnit;
			}

			internal void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Count);
				for (int i = 0; i < (int)this.Count; i++)
				{
					binaryWriter.Write(this.HomophoneList[i]);
				}
			}
		}
		internal class PinyinDictionary
		{
			internal short Length;

			internal short Count;

			internal short Offset;

			internal readonly byte[] Reserved = new byte[8];

			internal List<PinyinUnit> PinyinUnitTable;

			internal readonly short EndMark = 32767;

			internal void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Length);
				binaryWriter.Write(this.Count);
				binaryWriter.Write(this.Offset);
				binaryWriter.Write(this.Reserved);
				for (int i = 0; i < (int)this.Count; i++)
				{
					this.PinyinUnitTable[i].Serialize(binaryWriter);
				}
				binaryWriter.Write(this.EndMark);
			}

			internal static PinyinDictionary Deserialize(BinaryReader binaryReader)
			{
				PinyinDictionary pinyinDictionary = new PinyinDictionary();
				//_ = binaryReader.ReadInt32();
				pinyinDictionary.Length = binaryReader.ReadInt16();
				pinyinDictionary.Count = binaryReader.ReadInt16();
				pinyinDictionary.Offset = binaryReader.ReadInt16();
				binaryReader.ReadBytes(8);
				pinyinDictionary.PinyinUnitTable = new List<PinyinUnit>();
				for (int i = 0; i < (int)pinyinDictionary.Count; i++)
				{
					pinyinDictionary.PinyinUnitTable.Add(PinyinUnit.Deserialize(binaryReader));
				}
				binaryReader.ReadInt16();
				return pinyinDictionary;
			}

			internal int GetPinYinUnitIndex(string pinyin)
			{
				PinyinUnitPredicate @object = new PinyinUnitPredicate(pinyin);
				return this.PinyinUnitTable.FindIndex(new Predicate<PinyinUnit>(@object.Match));
			}

			internal PinyinUnit GetPinYinUnit(string pinyin)
			{
				PinyinUnitPredicate @object = new PinyinUnitPredicate(pinyin);
				return this.PinyinUnitTable.Find(new Predicate<PinyinUnit>(@object.Match));
			}

			internal PinyinUnit GetPinYinUnitByIndex(int index)
			{
				if (index < 0 || index >= (int)this.Count)
				{
					throw new ArgumentOutOfRangeException("index", INDEX_OUT_OF_RANGE);
				}
				return this.PinyinUnitTable[index];
			}
		}
		internal class PinyinUnit
		{
			internal string Pinyin;

			internal static PinyinUnit Deserialize(BinaryReader binaryReader)
			{
				PinyinUnit pinyinUnit = new PinyinUnit();
				byte[] bytes = binaryReader.ReadBytes(7);
				pinyinUnit.Pinyin = Encoding.ASCII.GetString(bytes, 0, 7);
				char[] array = new char[1];
				char[] trimChars = array;
				pinyinUnit.Pinyin = pinyinUnit.Pinyin.TrimEnd(trimChars);
				return pinyinUnit;
			}

			internal void Serialize(BinaryWriter binaryWriter)
			{
				byte[] array = new byte[7];
				Encoding.ASCII.GetBytes(this.Pinyin, 0, this.Pinyin.Length, array, 0);
				binaryWriter.Write(array);
			}
		}
		internal class PinyinUnitPredicate
		{
			private string ExpectedPinyin;

			internal PinyinUnitPredicate(string pinyin)
			{
				this.ExpectedPinyin = pinyin;
			}

			internal bool Match(PinyinUnit pinyinUnit)
			{
				return string.Compare(pinyinUnit.Pinyin, this.ExpectedPinyin, true, CultureInfo.CurrentCulture) == 0;
			}
		}
		internal class StrokeDictionary
		{
			internal const short MaxStrokeNumber = 48;

			internal int Length;

			internal int Count;

			internal short Offset;

			internal readonly byte[] Reserved = new byte[24];

			internal List<StrokeUnit> StrokeUnitTable;

			internal readonly short EndMark = 32767;

			internal void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Length);
				binaryWriter.Write(this.Count);
				binaryWriter.Write(this.Offset);
				binaryWriter.Write(this.Reserved);
				for (int i = 0; i < this.Count; i++)
				{
					this.StrokeUnitTable[i].Serialize(binaryWriter);
				}
				binaryWriter.Write(this.EndMark);
			}

			internal static StrokeDictionary Deserialize(BinaryReader binaryReader)
			{
				StrokeDictionary strokeDictionary = new StrokeDictionary();
				// binaryReader.ReadInt32();
				strokeDictionary.Length = binaryReader.ReadInt32();
				strokeDictionary.Count = binaryReader.ReadInt32();
				strokeDictionary.Offset = binaryReader.ReadInt16();
				binaryReader.ReadBytes(24);
				strokeDictionary.StrokeUnitTable = new List<StrokeUnit>();
				for (int i = 0; i < strokeDictionary.Count; i++)
				{
					strokeDictionary.StrokeUnitTable.Add(StrokeUnit.Deserialize(binaryReader));
				}
				binaryReader.ReadInt16();
				return strokeDictionary;
			}

			internal StrokeUnit GetStrokeUnitByIndex(int index)
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index", INDEX_OUT_OF_RANGE);
				}
				return this.StrokeUnitTable[index];
			}

			internal StrokeUnit GetStrokeUnit(int strokeNum)
			{
				if (strokeNum <= 0 || strokeNum > 48)
				{
					throw new ArgumentOutOfRangeException("strokeNum");
				}
				StrokeUnitPredicate @object = new StrokeUnitPredicate(strokeNum);
				return this.StrokeUnitTable.Find(new Predicate<StrokeUnit>(@object.Match));
			}
		}
		internal class StrokeUnit
		{
			internal byte StrokeNumber;

			internal short CharCount;

			internal char[] CharList;

			internal static StrokeUnit Deserialize(BinaryReader binaryReader)
			{
				StrokeUnit strokeUnit = new StrokeUnit();
				strokeUnit.StrokeNumber = binaryReader.ReadByte();
				strokeUnit.CharCount = binaryReader.ReadInt16();
				strokeUnit.CharList = new char[(int)strokeUnit.CharCount];
				for (int i = 0; i < (int)strokeUnit.CharCount; i++)
				{
					strokeUnit.CharList[i] = binaryReader.ReadChar();
				}
				return strokeUnit;
			}

			internal void Serialize(BinaryWriter binaryWriter)
			{
				if (this.CharCount == 0)
				{
					return;
				}
				binaryWriter.Write(this.StrokeNumber);
				binaryWriter.Write(this.CharCount);
				binaryWriter.Write(this.CharList);
			}
		}
		internal class StrokeUnitPredicate
		{
			private int ExpectedStrokeNum;

			internal StrokeUnitPredicate(int strokeNum)
			{
				this.ExpectedStrokeNum = strokeNum;
			}

			internal bool Match(StrokeUnit strokeUnit)
			{
				return (int)strokeUnit.StrokeNumber == this.ExpectedStrokeNum;
			}
		}
	}
}
