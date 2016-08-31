using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections
{
  public class PermutationEnumerator<T> : IEnumerator<T[]>, IEnumerator, IDisposable
  {
    private T[] _sequence;
    private PermutationEnumerator<T>.ElementStatus[] _status;

    public T[] Current
    {
      get
      {
        return this._sequence.Clone() as T[];
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.Current;
      }
    }

    internal PermutationEnumerator(T[] sequence)
    {
      this._sequence = new T[sequence.Length];
      for (uint index = 0; (long) index < (long) this._sequence.Length; ++index)
        this._sequence[(int) index] = sequence[(int) index];
      this._status = (PermutationEnumerator<T>.ElementStatus[]) null;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      if (this._status == null)
      {
        this._status = new PermutationEnumerator<T>.ElementStatus[this._sequence.Length];
        this._status[0] = new PermutationEnumerator<T>.ElementStatus(1U, new bool?());
        for (uint index = 1; (long) index < (long) this._sequence.Length; ++index)
          this._status[(int) index] = new PermutationEnumerator<T>.ElementStatus(index + 1U, new bool?(false));
        return true;
      }
      int num = 0;
      PermutationEnumerator<T>.ElementStatus elementStatus1 = new PermutationEnumerator<T>.ElementStatus(0U, new bool?());
      bool? nullable1;
      for (int index = 0; index < this._status.Length; ++index)
      {
        if (elementStatus1.Value < this._status[index].Value)
        {
          nullable1 = this._status[index].Direction;
          if (nullable1.HasValue)
          {
            num = index;
            elementStatus1 = this._status[index];
          }
        }
      }
      nullable1 = elementStatus1.Direction;
      if (!nullable1.HasValue)
        return false;
      int idx1 = num;
      int idx2 = elementStatus1.IsForward ? num + 1 : num - 1;
      this.Swap(idx1, idx2);
      if (idx2 > idx1 && idx2 < this._status.Length - 1)
      {
        if (this._status[idx2 + 1].Value > elementStatus1.Value)
        {
          PermutationEnumerator<T>.ElementStatus elementStatus2 = elementStatus1;
          nullable1 = new bool?();
          bool? nullable2 = nullable1;
          elementStatus2.Direction = nullable2;
        }
      }
      else if (idx2 < idx1 && idx2 > 0 && this._status[idx2 - 1].Value > elementStatus1.Value)
      {
        PermutationEnumerator<T>.ElementStatus elementStatus2 = elementStatus1;
        nullable1 = new bool?();
        bool? nullable2 = nullable1;
        elementStatus2.Direction = nullable2;
      }
      if (idx2 == 0 || idx2 == this._status.Length - 1)
      {
        PermutationEnumerator<T>.ElementStatus statu = this._status[idx2];
        nullable1 = new bool?();
        bool? nullable2 = nullable1;
        statu.Direction = nullable2;
      }
      for (int index = 0; index < this._status.Length; ++index)
      {
        if (this._status[index].Value > elementStatus1.Value)
          this._status[index].Direction = new bool?(index < idx2);
      }
      return true;
    }

    public void Swap(int idx1, int idx2)
    {
      PermutationEnumerator<T>.ElementStatus statu = this._status[idx1];
      this._status[idx1] = this._status[idx2];
      this._status[idx2] = statu;
      T obj = this._sequence[idx1];
      this._sequence[idx1] = this._sequence[idx2];
      this._sequence[idx2] = obj;
    }

    public void Reset()
    {
      T[] objArray = new T[this._sequence.Length];
      for (int index = 0; index < this._sequence.Length; ++index)
        objArray[index] = this._sequence[(int) this._status[index].Value];
      this._sequence = objArray;
      this._status = (PermutationEnumerator<T>.ElementStatus[]) null;
    }

    private class ElementStatus
    {
      public uint Value { get; private set; }

      public bool? Direction { get; set; }

      public bool IsForward
      {
        get
        {
          if (this.Direction.HasValue)
            return this.Direction.Value;
          return false;
        }
      }

      public bool IsBackward
      {
        get
        {
          if (this.Direction.HasValue)
            return !this.Direction.Value;
          return false;
        }
      }

      public ElementStatus(uint value, bool? direction)
      {
        this.Value = value;
        this.Direction = direction;
      }

      public override string ToString()
      {
        if (!this.Direction.HasValue)
          return string.Format("{0}:Undirected", (object) this.Value);
        if (this.IsForward)
          return string.Format("{0}:Forward", (object) this.Value);
        return string.Format("{0}:Backward", (object) this.Value);
      }
    }
  }
}
