using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp
{
  public static class Range
  {
    public static IEnumerable<sbyte> SByte(sbyte from, sbyte to, int step)
    {
      return Range.Int32((int) from, (int) to, step).Select<int, sbyte>((Func<int, sbyte>) (i => (sbyte) i));
    }

    public static IEnumerable<byte> Byte(byte from, byte to, int step)
    {
      return Range.Int32((int) from, (int) to, step).Select<int, byte>((Func<int, byte>) (i => (byte) i));
    }

    public static IEnumerable<char> Char(char from, char to, int step)
    {
      return Range.Int32((int) from, (int) to, step).Select<int, char>((Func<int, char>) (i => (char) i));
    }

    public static IEnumerable<short> Int16(short from, short to, int step)
    {
      return Range.Int32((int) from, (int) to, step).Select<int, short>((Func<int, short>) (i => (short) i));
    }

    public static IEnumerable<ushort> UInt16(ushort from, ushort to, int step)
    {
      return Range.Int32((int) from, (int) to, step).Select<int, ushort>((Func<int, ushort>) (i => (ushort) i));
    }

    public static IEnumerable<int> Int32(int from, int to, int step)
    {
      if (step <= 0)
        step = step == 0 ? 1 : -step;
      if (from <= to)
      {
        int i = from;
        while (i <= to)
        {
          yield return i;
          i += step;
        }
      }
      else
      {
        int i = from;
        while (i >= to)
        {
          yield return i;
          i -= step;
        }
      }
    }

    public static IEnumerable<uint> UInt32(uint from, uint to, uint step)
    {
      if ((int) step == 0)
        step = 1U;
      if (from <= to)
      {
        uint ui = from;
        while (ui <= to)
        {
          yield return ui;
          ui += step;
        }
      }
      else
      {
        uint ui = from;
        while (ui >= to)
        {
          yield return ui;
          ui -= step;
        }
      }
    }

    public static IEnumerable<long> Int64(long from, long to, long step)
    {
      if (step <= 0L)
        step = step == 0L ? 1L : -step;
      if (from <= to)
      {
        long l = from;
        while (l <= to)
        {
          yield return l;
          l += step;
        }
      }
      else
      {
        long l = from;
        while (l >= to)
        {
          yield return l;
          l -= step;
        }
      }
    }

    public static IEnumerable<ulong> UInt64(ulong from, ulong to, ulong step)
    {
      if ((long) step == 0L)
        step = 1UL;
      if (from <= to)
      {
        ulong ul = from;
        while (ul <= to)
        {
          yield return ul;
          ul += step;
        }
      }
      else
      {
        ulong ul = from;
        while (ul >= to)
        {
          yield return ul;
          ul -= step;
        }
      }
    }

    public static IEnumerable<float> Single(float from, float to, float step)
    {
      if ((double) step <= 0.0)
        step = (double) step == 0.0 ? 1f : -step;
      if ((double) from <= (double) to)
      {
        float f = from;
        while ((double) f <= (double) to)
        {
          yield return f;
          f += step;
        }
      }
      else
      {
        float f = from;
        while ((double) f >= (double) to)
        {
          yield return f;
          f -= step;
        }
      }
    }

    public static IEnumerable<double> Double(double from, double to, double step)
    {
      if (step <= 0.0)
        step = step == 0.0 ? 1.0 : -step;
      if (from <= to)
      {
        double d = from;
        while (d <= to)
        {
          yield return d;
          d += step;
        }
      }
      else
      {
        double d = from;
        while (d >= to)
        {
          yield return d;
          d -= step;
        }
      }
    }

    public static IEnumerable<Decimal> Decimal(Decimal from, Decimal to, Decimal step)
    {
      if (step <= new Decimal(0, 0, 0, false, (byte) 1))
        step = step == new Decimal(0, 0, 0, false, (byte) 1) ? new Decimal(10, 0, 0, false, (byte) 1) : -step;
      if (from <= to)
      {
        Decimal m = from;
        while (m <= to)
        {
          yield return m;
          m += step;
        }
      }
      else
      {
        Decimal m = from;
        while (m >= to)
        {
          yield return m;
          m -= step;
        }
      }
    }

    public static IEnumerable<DateTime> DateTime(DateTime from, DateTime to, double step)
    {
      if (step <= 0.0)
        step = step == 0.0 ? 1.0 : -step;
      if (from <= to)
      {
        for (DateTime dt = from; dt <= to; dt = dt.AddDays(step))
          yield return dt;
      }
      else
      {
        for (DateTime dt = from; dt >= to; dt = dt.AddDays(-step))
          yield return dt;
      }
    }

    public static IEnumerable<sbyte> SByte(sbyte from, sbyte to)
    {
      return Range.SByte(from, to, 1);
    }

    public static IEnumerable<byte> Byte(byte from, byte to)
    {
      return Range.Byte(from, to, 1);
    }

    public static IEnumerable<char> Char(char from, char to)
    {
      return Range.Char(from, to, 1);
    }

    public static IEnumerable<short> Int16(short from, short to)
    {
      return Range.Int16(from, to, 1);
    }

    public static IEnumerable<ushort> UInt16(ushort from, ushort to)
    {
      return Range.UInt16(from, to, 1);
    }

    public static IEnumerable<int> Int32(int from, int to)
    {
      return Range.Int32(from, to, 1);
    }

    public static IEnumerable<uint> UInt32(uint from, uint to)
    {
      return Range.UInt32(from, to, 1U);
    }

    public static IEnumerable<long> Int64(long from, long to)
    {
      return Range.Int64(from, to, 1L);
    }

    public static IEnumerable<ulong> UInt64(ulong from, ulong to)
    {
      return Range.UInt64(from, to, 1UL);
    }

    public static IEnumerable<float> Single(float from, float to)
    {
      return Range.Single(from, to, 1f);
    }

    public static IEnumerable<double> Double(double from, double to)
    {
      return Range.Double(from, to, 1.0);
    }

    public static IEnumerable<Decimal> Decimal(Decimal from, Decimal to)
    {
      return Range.Decimal(from, to, new Decimal(10, 0, 0, false, (byte) 1));
    }

    public static IEnumerable<DateTime> DateTime(DateTime from, DateTime to)
    {
      return Range.DateTime(from, to, 1.0);
    }
  }
}
