using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections
{
  public class HugeHashSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private List<HashSet<T>> _set;
    private const int _MAX_SET_SIZE = 1000000;

    public int CountSets
    {
      get
      {
        return this._set.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public int Count
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this._set.Count; ++index)
          num += this._set[index].Count;
        return num;
      }
    }

    public HugeHashSet()
    {
      this._set = new List<HashSet<T>>();
      this._set.Add(new HashSet<T>());
    }

    public bool Add(T item)
    {
      HashSet<T> objSet = (HashSet<T>) null;
      for (int index = 0; index < this._set.Count; ++index)
      {
        if (this._set[index].Contains(item))
          return true;
        if (this._set[index].Count < 1000000)
          objSet = this._set[index];
      }
      if (objSet == null)
      {
        objSet = new HashSet<T>();
        this._set.Add(objSet);
      }
      return objSet.Add(item);
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

    public void Clear()
    {
      this._set.Clear();
      this._set.Add(new HashSet<T>());
    }

    public bool Contains(T item)
    {
      for (int index = 0; index < this._set.Count; ++index)
      {
        if (this._set[index].Contains(item))
          return true;
      }
      return false;
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
      for (int index = 0; index < this._set.Count; ++index)
      {
        if (this._set[index].Remove(item))
          return true;
      }
      return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
      IEnumerable<T> first = (IEnumerable<T>) this._set[0];
      for (int index = 1; index < this._set.Count; ++index)
        first = first.Concat<T>((IEnumerable<T>) this._set[index]);
      return first.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      IEnumerable<T> first = (IEnumerable<T>) this._set[0];
      for (int index = 1; index < this._set.Count; ++index)
        first = first.Concat<T>((IEnumerable<T>) this._set[index]);
      return (IEnumerator) first.GetEnumerator();
    }
  }
}
