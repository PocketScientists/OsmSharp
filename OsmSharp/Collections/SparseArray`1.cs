using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections
{
  public class SparseArray<T> : IEnumerable<T>, IEnumerable
  {
    private readonly int _blockSize;
    private long _virtualSize;
    private readonly Dictionary<long, SparseArray<T>.ArrayBlock> _arrayBlocks;
    private SparseArray<T>.ArrayBlock _lastAccessedBlock;

    public T this[long index]
    {
      get
      {
        if (index >= this._virtualSize)
          throw new IndexOutOfRangeException();
        if (this._lastAccessedBlock != null && this._lastAccessedBlock.Index <= index && this._lastAccessedBlock.Index + (long) this._blockSize > index)
          return this._lastAccessedBlock.Data[index - this._lastAccessedBlock.Index];
        SparseArray<T>.ArrayBlock arrayBlock;
        if (!this._arrayBlocks.TryGetValue(index / (long) this._blockSize, out arrayBlock))
          return default (T);
        this._lastAccessedBlock = arrayBlock;
        return this._lastAccessedBlock.Data[index - this._lastAccessedBlock.Index];
      }
      set
      {
        if (index >= this._virtualSize)
          throw new IndexOutOfRangeException();
        if (this._lastAccessedBlock != null && this._lastAccessedBlock.Index <= index && this._lastAccessedBlock.Index + (long) this._blockSize > index)
        {
          this._lastAccessedBlock.Data[index - this._lastAccessedBlock.Index] = value;
        }
        else
        {
          long key = index / (long) this._blockSize;
          SparseArray<T>.ArrayBlock arrayBlock;
          if (!this._arrayBlocks.TryGetValue(key, out arrayBlock) && (object) value != null)
          {
            arrayBlock = new SparseArray<T>.ArrayBlock(key * (long) this._blockSize, this._blockSize);
            this._arrayBlocks.Add(key, arrayBlock);
          }
          if (arrayBlock == null)
            return;
          this._lastAccessedBlock = arrayBlock;
          this._lastAccessedBlock.Data[index - this._lastAccessedBlock.Index] = value;
        }
      }
    }

    public long Length
    {
      get
      {
        return this._virtualSize;
      }
    }

    public SparseArray(long size)
    {
      this._virtualSize = size;
      this._blockSize = 256;
      this._arrayBlocks = new Dictionary<long, SparseArray<T>.ArrayBlock>();
      this._lastAccessedBlock = (SparseArray<T>.ArrayBlock) null;
    }

    public void Resize(long size)
    {
      if (size >= this._virtualSize)
      {
        this._virtualSize = size;
      }
      else
      {
        this._virtualSize = size;
        List<KeyValuePair<long, SparseArray<T>.ArrayBlock>> keyValuePairList = new List<KeyValuePair<long, SparseArray<T>.ArrayBlock>>();
        foreach (KeyValuePair<long, SparseArray<T>.ArrayBlock> arrayBlock in this._arrayBlocks)
        {
          if (arrayBlock.Value.Index > this._virtualSize)
            keyValuePairList.Add(arrayBlock);
        }
        foreach (KeyValuePair<long, SparseArray<T>.ArrayBlock> keyValuePair in keyValuePairList)
          this._arrayBlocks.Remove(keyValuePair.Key);
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) new SparseArray<T>.SparseArrayEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new SparseArray<T>.SparseArrayEnumerator(this);
    }

    private class ArrayBlock
    {
      public long Index { get; private set; }

      public T[] Data { get; private set; }

      public ArrayBlock(long index, int size)
      {
        this.Index = index;
        this.Data = new T[size];
      }
    }

    private class SparseArrayEnumerator : IEnumerator, IEnumerator<T>, IDisposable
    {
      private int _current = -1;
      private SparseArray<T> _array;

      public object Current
      {
        get
        {
          return (object) this._array[(long) this._current];
        }
      }

      T IEnumerator<T>.Current
      {
        get
        {
          return this._array[(long) this._current];
        }
      }

      public SparseArrayEnumerator(SparseArray<T> array)
      {
        this._array = array;
      }

      public bool MoveNext()
      {
        this._current = this._current + 1;
        return (long) this._current < this._array.Length;
      }

      public void Reset()
      {
        this._current = 0;
      }

      public void Dispose()
      {
      }
    }
  }
}
