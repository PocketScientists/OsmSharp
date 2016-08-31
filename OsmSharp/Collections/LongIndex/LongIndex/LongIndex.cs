namespace OsmSharp.Collections.LongIndex.LongIndex
{
  public class LongIndex : ILongIndex
  {
    private readonly long _size = 34359738368;
    private readonly int _blockSize = 1048576;
    private long _count;
    private SparseLargeBitArray32 _positiveFlags;
    private SparseLargeBitArray32 _negativeFlags;

    public long Count
    {
      get
      {
        return this._count;
      }
    }

    public void Add(long number)
    {
      if (number >= 0L)
        this.PositiveAdd(number);
      else
        this.NegativeAdd(-number);
    }

    public void Remove(long number)
    {
      if (number >= 0L)
        this.PositiveRemove(number);
      else
        this.NegativeAdd(-number);
    }

    public bool Contains(long number)
    {
      if (number >= 0L)
        return this.PositiveContains(number);
      return this.NegativeContains(-number);
    }

    private void PositiveAdd(long number)
    {
      if (this._positiveFlags == null)
        this._positiveFlags = new SparseLargeBitArray32(this._size, this._blockSize);
      if (!this._positiveFlags[number])
        this._count = this._count + 1L;
      this._positiveFlags[number] = true;
    }

    private void PositiveRemove(long number)
    {
      if (this._positiveFlags == null)
        this._positiveFlags = new SparseLargeBitArray32(this._size, this._blockSize);
      if (this._positiveFlags[number])
        this._count = this._count - 1L;
      this._positiveFlags[number] = false;
    }

    private bool PositiveContains(long number)
    {
      if (this._positiveFlags == null)
        return false;
      return this._positiveFlags[number];
    }

    private void NegativeAdd(long number)
    {
      if (this._negativeFlags == null)
        this._negativeFlags = new SparseLargeBitArray32(this._size, this._blockSize);
      if (!this._negativeFlags[number])
        this._count = this._count + 1L;
      this._negativeFlags[number] = true;
    }

    private void NegativeRemove(long number)
    {
      if (this._negativeFlags == null)
        this._negativeFlags = new SparseLargeBitArray32(this._size, this._blockSize);
      if (this._negativeFlags[number])
        this._count = this._count - 1L;
      this._negativeFlags[number] = false;
    }

    private bool NegativeContains(long number)
    {
      if (this._negativeFlags == null)
        return false;
      return this._negativeFlags[number];
    }

    public void Clear()
    {
      this._negativeFlags = (SparseLargeBitArray32) null;
      this._positiveFlags = (SparseLargeBitArray32) null;
    }
  }
}
