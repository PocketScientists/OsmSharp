using OsmSharp.IO;
using OsmSharp.Math.Random;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp
{
  public static class Utilities
  {
    public static long EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

    public static int Power10Floor(this double value)
    {
      return (int) System.Math.Pow(10.0, System.Math.Floor(System.Math.Log10(value)));
    }

    public static int Power10Floor(this float value)
    {
      return ((double) value).Power10Floor();
    }

    public static void CopyToReverse<T>(this List<T> list, T[] array, int arrayIndex)
    {
      list.CopyToReverse<T>(0, array, arrayIndex, list.Count);
    }

    public static void CopyToReverse<T>(this List<T> list, int index, T[] array, int arrayIndex, int count)
    {
      for (int index1 = index + count - 1; index1 >= index; --index1)
      {
        array[arrayIndex] = list[index1];
        ++arrayIndex;
      }
    }

    public static void CopyTo<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
    {
      for (int index1 = index; index1 < index + count; ++index1)
      {
        array[arrayIndex] = source[index1];
        ++arrayIndex;
      }
    }

    public static void CopyToReverse<T>(this T[] source, T[] array, int arrayIndex)
    {
      source.CopyToReverse<T>(0, array, arrayIndex, source.Length);
    }

    public static void CopyToReverse<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
    {
      for (int index1 = index + count - 1; index1 >= index; --index1)
      {
        array[arrayIndex] = source[index1];
        ++arrayIndex;
      }
    }

    public static void InsertTo<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
    {
      for (int index1 = array.Length - 1; index1 >= arrayIndex + count; --index1)
        array[index1] = array[index1 - count];
      source.CopyTo<T>(index, array, arrayIndex, count);
    }

    public static void InsertToReverse<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
    {
      for (int index1 = array.Length - 1; index1 >= arrayIndex + count; --index1)
        array[index1] = array[index1 - count];
      source.CopyToReverse<T>(index, array, arrayIndex, count);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
      list.Shuffle<T>(StaticRandomGenerator.Get());
    }

    public static void Shuffle<T>(this IList<T> list, IRandomGenerator random)
    {
      int count = list.Count;
      while (count > 1)
      {
        --count;
        int index = random.Generate(count + 1);
        T obj = list[index];
        list[index] = list[count];
        list[count] = obj;
      }
    }

    public static string RandomString(int length)
    {
      byte[] numArray = new byte[length];
      StaticRandomGenerator.Get().Generate(numArray);
      return Convert.ToBase64String(numArray).Remove(length);
    }

    public static DateTime FromUnixTime(this long milliseconds)
    {
      return new DateTime(Utilities.EpochTicks + milliseconds * 10000L);
    }

    public static long ToUnixTime(this DateTime date)
    {
      return (date.Ticks - Utilities.EpochTicks) / 10000L;
    }

    public static string Truncate(this string value, int maxLength)
    {
      if (value != null && value.Length > maxLength)
        return value.Substring(0, maxLength);
      return value;
    }

    public static string PadRightAndCut(this string s, int length)
    {
      return s.ToStringEmptyWhenNull().PadRight(length).Substring(0, length);
    }

    public static bool LevenshteinMatch(this string s, string t, float percentage)
    {
      if (s == null || t == null)
        return false;
      int length1 = s.Length;
      int length2 = t.Length;
      int[,] numArray = new int[length1 + 1, length2 + 1];
      int num1 = -1;
      int num2 = System.Math.Max(length1, length2);
      if (num2 == 0)
        return false;
      if (length1 == 0)
        num1 = length2;
      if (length2 == 0)
        num1 = length1;
      int index1 = 0;
      while (index1 <= length1)
        numArray[index1, 0] = index1++;
      int index2 = 0;
      while (index2 <= length2)
        numArray[0, index2] = index2++;
      for (int index3 = 1; index3 <= length1; ++index3)
      {
        for (int index4 = 1; index4 <= length2; ++index4)
        {
          int num3 = (int) t[index4 - 1] == (int) s[index3 - 1] ? 0 : 1;
          numArray[index3, index4] = System.Math.Min(System.Math.Min(numArray[index3 - 1, index4] + 1, numArray[index3, index4 - 1] + 1), numArray[index3 - 1, index4 - 1] + num3);
        }
      }
      int num4 = numArray[length1, length2];
      return (double) (num2 - num4) / (double) num2 > (double) percentage / 100.0;
    }

    public static string InitCap(this string value)
    {
      if (value == null)
        return (string) null;
      if (value.Length == 0)
        return value;
      StringBuilder stringBuilder = new StringBuilder(value);
      stringBuilder[0] = char.ToUpper(stringBuilder[0]);
      for (int index = 1; index < stringBuilder.Length; ++index)
        stringBuilder[index] = !char.IsWhiteSpace(stringBuilder[index - 1]) ? char.ToLower(stringBuilder[index]) : char.ToUpper(stringBuilder[index]);
      return stringBuilder.ToString();
    }

    public static int RemoveAll<T>(this List<T> list, Predicate<T> match)
    {
      int num = 0;
      for (int index = list.Count - 1; index >= 0; --index)
      {
        if (match(list[index]))
        {
          ++num;
          list.RemoveAt(index);
        }
      }
      return num;
    }

    public static string NumericPartFloat(this string value)
    {
      string str = string.Empty;
      if (value != null && value.Length > 0)
      {
        for (int length = 1; length <= value.Length; ++length)
        {
          string s = value.Substring(0, length);
          float result;
          if (float.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          {
            if ((int) value[length - 1] != 46)
              str = s;
          }
          else
            break;
        }
      }
      return str;
    }

    public static string NumericPartInt(this string value)
    {
      string str = string.Empty;
      if (value != null && value.Length > 0)
      {
        for (int length = 1; length <= value.Length; ++length)
        {
          string s = value.Substring(0, length);
          int result;
          if (int.TryParse(s, out result))
            str = s;
          else
            break;
        }
      }
      return str;
    }

    public static string[] SplitMultiple(this string value, int[] sizes)
    {
      string[] strArray = new string[sizes.Length];
      int startIndex = 0;
      for (int index = 0; index < sizes.Length; ++index)
      {
        strArray[index] = value.Substring(startIndex, sizes[index]);
        startIndex += sizes[index];
      }
      return strArray;
    }

    public static string ToStringEmptyWhenNull(this object obj)
    {
      if (obj == null)
        return string.Empty;
      return obj.ToString();
    }

    public static string RemoveWhitespace(this string input)
    {
      return new string(((IEnumerable<char>) input.ToCharArray()).Where<char>((Func<char, bool>) (c => !char.IsWhiteSpace(c))).ToArray<char>());
    }

    public static long[] ConvertToLongArray(this double[] doubleArray, int factor)
    {
      long[] numArray = new long[doubleArray.Length];
      for (int index = 0; index < doubleArray.Length; ++index)
        numArray[index] = (long) (doubleArray[index] * (double) factor);
      return numArray;
    }

    public static double[] ConvertFromLongArray(this long[] longArray, int factor)
    {
      double[] numArray = new double[longArray.Length];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (double) longArray[index] / (double) factor;
      return numArray;
    }

    public static double[] ConvertFromLongArray(this List<long> longArray, int factor)
    {
      double[] numArray = new double[longArray.Count];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (double) longArray[index] / (double) factor;
      return numArray;
    }

    public static long[] EncodeDelta(this long[] longArray)
    {
      if (longArray.Length != 0)
      {
        long num1 = longArray[0];
        for (int index = 1; index < longArray.Length; ++index)
        {
          long num2 = longArray[index] - num1;
          num1 = longArray[index];
          longArray[index] = num2;
        }
      }
      return longArray;
    }

    public static List<long> EncodeDelta(this List<long> longArray)
    {
      if (longArray.Count > 0)
      {
        long num1 = longArray[0];
        for (int index = 1; index < longArray.Count; ++index)
        {
          long num2 = longArray[index] - num1;
          num1 = longArray[index];
          longArray[index] = num2;
        }
      }
      return longArray;
    }

    public static long[] DecodeDelta(this long[] longArray)
    {
      if (longArray.Length != 0)
      {
        for (int index = 1; index < longArray.Length; ++index)
          longArray[index] = longArray[index - 1] + longArray[index];
      }
      return longArray;
    }

    public static List<long> DecodeDelta(this List<long> longArray)
    {
      if (longArray.Count > 0)
      {
        for (int index = 1; index < longArray.Count; ++index)
          longArray[index] = longArray[index - 1] + longArray[index];
      }
      return longArray;
    }

    public static void SerializeWithSize(this RuntimeTypeModel model, Stream dest, object value)
    {
      long position = dest.Position;
      dest.Seek(8L, SeekOrigin.Current);
      ((TypeModel) model).Serialize(dest, value);
      long offset = dest.Position - position - 8L;
      dest.Seek(position, SeekOrigin.Begin);
      byte[] bytes = BitConverter.GetBytes(offset);
      dest.Write(bytes, 0, 8);
      dest.Seek(offset, SeekOrigin.Current);
    }

    public static object DeserializeWithSize(this RuntimeTypeModel model, Stream source, object value, Type type)
    {
      byte[] buffer = new byte[8];
      source.Read(buffer, 0, 8);
      long int64 = BitConverter.ToInt64(buffer, 0);
      CappedStream cappedStream = new CappedStream(source, source.Position, int64);
      return ((TypeModel) model).Deserialize((Stream) cappedStream, value, type);
    }

    public static bool TryGetValue<T>(this Dictionary<string, object> dictionary, string key, out T value)
    {
      object obj;
      if (dictionary.TryGetValue(key, out obj))
      {
        value = (T) obj;
        return true;
      }
      value = default (T);
      return false;
    }
  }
}
