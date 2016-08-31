using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections
{
  public class SortedSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
  {
    private readonly List<T> _elements;
    private readonly IComparer<T> _comparer;

    public IComparer<T> Comparer
    {
      get
      {
        return this._comparer;
      }
    }

    public T Max
    {
      get
      {
        return this._elements[this._elements.Count - 1];
      }
    }

    public T Min
    {
      get
      {
        return this._elements[0];
      }
    }

    public int Count
    {
      get
      {
        return this._elements.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public object SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    public SortedSet()
    {
      this._elements = new List<T>();
      this._comparer = (IComparer<T>) System.Collections.Generic.Comparer<T>.Default;
    }

    public SortedSet(IEnumerable<T> enumerable)
    {
      this._elements = new List<T>();
      this._comparer = (IComparer<T>) System.Collections.Generic.Comparer<T>.Default;
      foreach (T obj in enumerable)
        this.Add(obj);
    }

    public SortedSet(IEnumerable<T> enumerable, IComparer<T> comparer)
    {
      this._elements = new List<T>();
      this._comparer = comparer;
      foreach (T obj in enumerable)
        this.Add(obj);
    }

    public SortedSet(IComparer<T> comparer)
    {
      this._elements = new List<T>();
      this._comparer = comparer;
    }

    public bool Add(T item)
    {
      int index1 = 0;
      int num = this.Count;
      while (num - index1 > 0)
      {
        int index2 = (num - index1) / 2 + index1;
        if (this._comparer.Compare(this._elements[index2], item) < 0)
          index1 = index2 + 1;
        else
          num = index2;
      }
      this._elements.Insert(index1, item);
      return false;
    }

    public void Clear()
    {
      this._elements.Clear();
    }

    public bool Contains(T item)
    {
      return this._elements.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      foreach (T obj in this)
      {
        array[arrayIndex] = obj;
        ++arrayIndex;
      }
    }

    public bool Remove(T item)
    {
      return this._elements.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) this._elements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public void CopyTo(Array array, int index)
    {
      foreach (T obj in this)
      {
        array.SetValue((object) obj, index);
        ++index;
      }
    }

    public void ExceptWith(IEnumerable<T> other)
    {
      foreach (T obj in other)
        this.Remove(obj);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
      HashSet<T> objSet = new HashSet<T>();
      foreach (T obj in other)
      {
        if (this.Contains(obj))
          objSet.Add(obj);
      }
      this.Clear();
      this.UnionWith((IEnumerable<T>) objSet);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
      HashSet<T> objSet = new HashSet<T>(other);
      foreach (T obj in this)
      {
        if (!objSet.Contains(obj))
          return false;
      }
      foreach (T obj in other)
      {
        if (!this.Contains(obj))
          return true;
      }
      return false;
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
      foreach (T obj in other)
      {
        if (!this.Contains(obj))
          return false;
      }
      HashSet<T> objSet = new HashSet<T>(other);
      foreach (T obj in this)
      {
        if (!objSet.Contains(obj))
          return true;
      }
      return false;
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
      HashSet<T> objSet = new HashSet<T>(other);
      foreach (T obj in this)
      {
        if (!objSet.Contains(obj))
          return false;
      }
      return true;
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
      foreach (T obj in other)
      {
        if (!this.Contains(obj))
          return false;
      }
      return true;
    }

    public bool Overlaps(IEnumerable<T> other)
    {
      foreach (T obj in other)
      {
        if (this.Contains(obj))
          return true;
      }
      HashSet<T> objSet = new HashSet<T>(other);
      foreach (T obj in this)
      {
        if (objSet.Contains(obj))
          return true;
      }
      return false;
    }

    public bool SetEquals(IEnumerable<T> other)
    {
      HashSet<T> objSet = new HashSet<T>(other);
      foreach (T obj in this)
        objSet.Remove(obj);
      return objSet.Count == 0;
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
      foreach (T obj in this.Intersect<T>(other))
        this.Remove(obj);
    }

    public void UnionWith(IEnumerable<T> other)
    {
      foreach (T obj in other)
      {
        if (!this.Contains(obj))
          this.Add(obj);
      }
    }

    void ICollection<T>.Add(T item)
    {
      this.Add(item);
    }
  }
}
