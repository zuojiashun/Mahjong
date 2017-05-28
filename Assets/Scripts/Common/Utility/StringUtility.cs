﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class StringUtility : GameBase
{
	public void init() { }
	public static int getLastNotNumberPos(string str)
	{
		int strLen = str.Length;
		for (int i = 0; i < strLen; ++i)
		{
			if (str[strLen - i - 1] > '9' || str[strLen - i - 1] < '0')
			{
				return strLen - i - 1;
			}
		}
		return -1;
	}
	public static int getLastNumber(string str)
	{
		int lastPos = getLastNotNumberPos(str);
		if (lastPos == -1)
		{
			return -1;
		}
		string numStr = str.Substring(lastPos + 1, str.Length - lastPos - 1);
		if (numStr == "")
		{
			return 0;
		}
		return int.Parse(numStr);
	}
	public static int stringToInt(string str)
	{
		return int.Parse(str);
	}
	public static Vector2 stringToVector2(string value)
	{
		string[] spitList = value.Split(new char[] { ',' });

		if (spitList.Length < 2)
		{
			return new Vector2();
		}

		Vector2 v2 = new Vector2();
		v2.x = float.Parse(spitList[0]);
		v2.y = float.Parse(spitList[1]);

		return v2;
	}
	public static Vector3 stringToVector3(string value)
	{
		string[] spitList = value.Split(new char[] { ',' });

		if (spitList.Length < 3)
		{
			return new Vector3();
		}

		Vector3 v2 = new Vector3();
		v2.x = float.Parse(spitList[0]);
		v2.y = float.Parse(spitList[1]);
		v2.z = float.Parse(spitList[2]);
		return v2;
	}
	public static Vector4 stringToVector4(string value)
	{
		string[] spitList = value.Split(new char[] { ',' });

		if (spitList.Length < 4)
		{
			return new Vector4();
		}

		Vector4 v2 = new Vector4();
		v2.x = float.Parse(spitList[0]);
		v2.y = float.Parse(spitList[1]);
		v2.z = float.Parse(spitList[2]);
		v2.w = float.Parse(spitList[3]);
		return v2;
	}
	public static void removeLast(ref string stream, char key)
	{
		int lastCommaPos = stream.LastIndexOf(key);
		if (lastCommaPos != -1)
		{
			stream = stream.Remove(lastCommaPos, 1);
		}
	}
	// 去掉最后一个逗号
	public static void removeLastComma(ref string stream)
	{
		int lastCommaPos = stream.LastIndexOf(',');
		if (lastCommaPos != -1)
		{
			stream = stream.Remove(lastCommaPos, 1);
		}
	}
	// json
	public static void jsonStartArray(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "[";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonEndArray(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		removeLastComma(ref str);
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "],";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonStartStruct(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "{";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonEndStruct(ref string str, int preTableCount = 0, bool returnLine = false)
	{
		removeLastComma(ref str);
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "},";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static void jsonAddPair(ref string str, string name, string value, int preTableCount = 0, bool returnLine = false)
	{
		for (int i = 0; i < preTableCount; ++i)
		{
			str += "\t";
		}
		str += "\"" + name + "\": \"" + value + "\",";
		if (returnLine)
		{
			str += "\r\n";
		}
	}
	public static string removeSuffix(ref string str)
	{
		int dotPos = str.IndexOf('.');
		if (dotPos != -1)
		{
			return str.Substring(0, dotPos);
		}
		return str;
	}
	// 从文件路径中得到最后一级的文件夹名
	public static string getFolderName(ref string str)
	{
		rightToLeft(ref str);
		string ret = str;
		// 如果有文件名,则先去除文件名
		int dotPos = ret.LastIndexOf('.');
		int namePos = ret.LastIndexOf('/');
		if(dotPos != -1)
		{
			ret = ret.Substring(0, namePos + 1);
		}
		// 再去除当前目录的父级目录
		namePos = ret.LastIndexOf('/');
		if (namePos != -1)
		{
			ret = ret.Substring(namePos + 1, ret.Length - namePos - 1);
		}
		return ret;
	}
	// 得到文件路径
	public static string getFilePath(string fileName)
	{
		rightToLeft(ref fileName);
		int lastPos = fileName.LastIndexOf('/');
		if(lastPos != -1)
		{
			return fileName.Substring(0, lastPos);
		}
		return fileName;
	}
	public static string getFileName(string str)
	{
		rightToLeft(ref str);
		int dotPos = str.LastIndexOf('/');
		if (dotPos != -1)
		{
			return str.Substring(dotPos + 1);
		}
		return str;
	}
	public static string getFileNameNoSuffix(string str, bool removeDir = false)
	{
		rightToLeft(ref str);
		int namePos = str.LastIndexOf('/');
		string ret = str;
		// 先判断是否移除目录
		if(removeDir && namePos != -1)
		{
			ret = str.Substring(namePos + 1, str.Length - namePos - 1);
		}
		// 移除后缀
		int dotPos = ret.LastIndexOf('.');
		if (dotPos != -1)
		{
			ret = ret.Substring(0, dotPos);
		}
		return ret;
	}
	public static void rightToLeft(ref string str)
	{
		str = str.Replace('\\', '/');
	}
	public static void leftToRight(ref string str)
	{
		str = str.Replace('/', '\\');
	}
	public static void stringToFloatArray(string str, ref float[] values)
	{
		str = Regex.Replace(str, @"\s", "");
		string[] rangeList = str.Split(',');
		int len = rangeList.Length;
		if (values != null && len != values.Length)
		{
			UnityUtility.logError("error : count is not equal " + str.Length);
			return;
		}
		if (values == null)
		{
			values = new float[len];
		}

		for (int i = 0; i < len; ++i)
		{
			values[i] = stringToFloat(rangeList[i]);
		}
	}
	public static string floatArrayToString(float[] values)
	{
		string str = "";
		int count = values.Length;
		for (int i = 0; i < count; ++i)
		{
			str += floatToString(values[i], 2);
			if (i != count - 1)
			{
				str += ",";
			}
		}
		return str;
	}
	public static void stringToIntArray(string str, ref int[] values)
	{
		str = Regex.Replace(str, @"\s", "");
		string[] rangeList = str.Split(',');
		int len = rangeList.Length;
		if (values != null && len != values.Length)
		{
			UnityUtility.logError("error : count is not equal " + str.Length);
			return;
		}
		if (values == null)
		{
			values = new int[len];
		}
		for (int i = 0; i < len; ++i)
		{
			values[i] = stringToInt(rangeList[i]);
		}
	}
	public static string intArrayToString(int[] values)
	{
		string str = "";
		int count = values.Length;
		for(int i = 0; i < count; ++i)
		{
			str += intToString(values[i]);
			if(i != count - 1)
			{
				str += ",";
			}
		}
		return str;
	}
	public static int getStringLength(string str)
	{
		char[] charArray = str.ToCharArray();
		for (int i = 0; i < str.Length; ++i)
		{
			if (charArray[i] == 0)
			{
				return i;
			}
		}
		return str.Length;
	}
	public static string UTF8ToGB2312(string value)
	{
		byte[] byteList = System.Text.Encoding.UTF8.GetBytes(value);
		char[] chrs = System.Text.Encoding.GetEncoding("GB2312").GetChars(byteList);
		string s = "";
		for (int i = 0; i < chrs.Length; i++)
		{
			s += chrs[i].ToString();
		}
		return s;
	}
	public static string GB2312ToUTF8(string str)
	{
		try
		{
			Encoding utf8 = Encoding.UTF8;
			Encoding gb2312 = Encoding.GetEncoding("GB2312");
			byte[] unicodeBytes = gb2312.GetBytes(str);
			byte[] asciiBytes = Encoding.Convert(gb2312, utf8, unicodeBytes);
			char[] asciiChars = new char[utf8.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
			utf8.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
			string result = StringUtility.charArrayToString(asciiChars);
			return result;
		}
		catch
		{
			return "";
		}
	}
	public static string GB2312ToUnicode(string value)
	{
		byte[] byteList = System.Text.Encoding.GetEncoding("GB2312").GetBytes(value);
		char[] chrs = System.Text.Encoding.Unicode.GetChars(byteList);
		string s = "";
		for (int i = 0; i < chrs.Length; i++)
		{
			s += chrs[i].ToString();
		}
		return s;
	}
	// precision表示小数点后保留几位小数,removeTailZero表示是否去除末尾的0
	public static string floatToString(float value, int precision = 4, bool removeTailZero = true)
	{
		string str = value.ToString("f" + precision.ToString());
		// 去除末尾的0
		if (removeTailZero)
		{
			int removeCount = 0;
			int curLen = str.Length;
			// 从后面开始遍历
			for (int i = 0; i < curLen; ++i)
			{
				char c = str[curLen - 1 - i];
				// 遇到不是0的就退出循环
				if (c != '0' && c != '.')
				{
					removeCount = i;
					break;
				}
				// 遇到小数点就退出循环并且需要将小数点一起去除
				else if (c == '.')
				{
					removeCount = i + 1;
					break;
				}
			}
			str = str.Substring(0, curLen - removeCount);
		}
		return str;
	}
	public static string intToString(int value, int limitLen = 0)
	{
		string retString = value.ToString();
		int addLen = limitLen - retString.Length;
		if (addLen > 0)
		{
			for (int i = 0; i < addLen; ++i)
			{
				retString = "0" + retString;
			}
		}
		return retString;
	}
	public static string strReplace(string str, int begin, int end, string reStr)
	{
		string sub1 = str.Substring(0, begin);
		string sub2 = str.Substring(end, str.Length - end);
		return sub1 + reStr + sub2;
	}
	public static float stringToFloat(string str)
	{
		return float.Parse(str);
	}
	public static string charArrayToString(char[] charArray)
	{
		int charCount = 0;
		int arrayLength = charArray.Length;
		for (int i = 0; i < arrayLength; ++i)
		{
			if(charArray[i] == 0)
			{
				charCount = i;
				break;
			}
		}
		string newStr = new string(charArray);
		newStr = newStr.Substring(0, charCount);
		return newStr;
	}
	public static char[] stringToCharArray(string value) 
	{
		char[] mchar = new char[32];
		for (int i = 0; i < value.Length; i++)
		{
			mchar[i] = value[i];
		}
		return mchar;
	}
	public static int getStringLength(char[] charArray)
	{
		for (int i = 0; i < charArray.Length; ++i)
		{
			if (charArray[i] == 0)
			{
				return i;
			}
		}
		return charArray.Length;
	}
	public static bool charEquals(char[] textchar1, char[] textchar2)
	{
		if (getStringLength(textchar1) != getStringLength(textchar2))
		{
			return false;
		}

		for (int i = 0; i < textchar1.Length; i++)
		{
			if (textchar1[i] != textchar2[i])
			{
				return false;
			}
			if (textchar1[i] == 0)
			{
				return true;
			}
		}
		return true;
	}
}