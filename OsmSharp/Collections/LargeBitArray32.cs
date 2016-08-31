using System;

namespace OsmSharp.Collections
{
  public class LargeBitArray32
  {
    private uint[] _array;
    private long _length;

    public bool this[long idx]
    {
      get
      {
        return ((ulong) this._array[(int) (idx >> 5)] & (ulong) (1L << (int) (idx % 32L))) > 0UL;
      }
      set
      {
        int index = (int) (idx >> 5);
        long num = 1L << (int) (idx % 32L);
        if (value)
          this._array[index] = (uint) ((ulong) num | (ulong) this._array[index]);
        else
          this._array[index] = (uint) ((ulong) ~num & (ulong) this._array[index]);
      }
    }

    public long Length
    {
      get
      {
        return this._length;
      }
    }

    public LargeBitArray32(long size)
    {
      this._length = size;
      this._array = new uint[(int)System.Math.Ceiling((double) size / 32.0)];
    }
  }
}
