using System;

namespace OsmSharp.Collections
{
  public class SparseLargeBitArray32
  {
    private int _blockSize;
    private long _length;
    private LargeBitArray32[] _data;

    public bool this[long idx]
    {
      get
      {
        int index = (int) (idx / (long) this._blockSize);
        if (this._data[index] == null)
          return false;
        int num = (int) (idx % (long) this._blockSize);
        return this._data[index][(long) num];
      }
      set
      {
        int index = (int) (idx / (long) this._blockSize);
        LargeBitArray32 largeBitArray32_1 = this._data[index];
        if (largeBitArray32_1 == null)
        {
          if (!value)
            return;
          LargeBitArray32 largeBitArray32_2 = new LargeBitArray32((long) this._blockSize);
          int num = (int) (idx % (long) this._blockSize);
          largeBitArray32_2[(long) num] = true;
          this._data[index] = largeBitArray32_2;
        }
        else
        {
          int num = (int) (idx % (long) this._blockSize);
          largeBitArray32_1[(long) num] = value;
        }
      }
    }

    public long Length
    {
      get
      {
        return this._length;
      }
    }

    public SparseLargeBitArray32(long size, int blockSize)
    {
      if (size % 32L != 0L)
        throw new ArgumentOutOfRangeException("Size has to be divisible by 32.");
      if (size % (long) blockSize != 0L)
        throw new ArgumentOutOfRangeException("Size has to be divisible by blocksize.");
      this._length = size;
      this._blockSize = blockSize;
      this._data = new LargeBitArray32[this._length / (long) this._blockSize];
    }
  }
}
