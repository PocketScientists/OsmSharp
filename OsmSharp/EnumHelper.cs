using System;
using System.Collections.Generic;
using System.Globalization;

namespace OsmSharp
{
  public static class EnumHelper
  {
    public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct
    {
      return Enum.TryParse<TEnum>(value, out result);
    }

    public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct
    {
      return Enum.TryParse<TEnum>(value, ignoreCase, out result);
    }

    public static string ToInvariantString(this object obj)
    {
      if (obj is IConvertible)
        return ((IConvertible) obj).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (!(obj is IFormattable))
        return obj.ToString();
      return ((IFormattable) obj).ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static List<T> ShrinkAndCopyList<T>(this List<T> list, HashSet<int> toRemove)
    {
      List<T> objList = new List<T>(System.Math.Max(list.Count - toRemove.Count, 0));
      for (int index = 0; index < list.Count; ++index)
      {
        if (!toRemove.Contains(index))
          objList.Add(list[index]);
      }
      return objList;
    }

    public static T[][] SchrinkAndCopyMatrix<T>(this T[][] matrix, HashSet<int> toRemove)
    {
      T[][] objArray = new T[matrix.Length - toRemove.Count][];
      int index1 = 0;
      for (int index2 = 0; index2 < matrix.Length; ++index2)
      {
        if (!toRemove.Contains(index2))
        {
          int index3 = 0;
          objArray[index1] = new T[matrix.Length - toRemove.Count];
          for (int index4 = 0; index4 < matrix[index2].Length; ++index4)
          {
            if (!toRemove.Contains(index4))
            {
              objArray[index1][index3] = matrix[index2][index4];
              ++index3;
            }
          }
          ++index1;
        }
      }
      return objArray;
    }
  }
}
