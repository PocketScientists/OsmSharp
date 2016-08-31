using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections
{
  public class HugeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
  {
    private readonly int _maxDictionarySize = 1000000;
    private readonly List<IDictionary<TKey, TValue>> _dictionary;

    public ICollection<TKey> Keys
    {
      get
      {
        IEnumerable<TKey>[] keysArray = new IEnumerable<TKey>[this._dictionary.Count];
        for (int index = 0; index < keysArray.Length; ++index)
          keysArray[index] = (IEnumerable<TKey>) this._dictionary[index].Keys;
        return (ICollection<TKey>) new HugeDictionary<TKey, TValue>.ReadonlyEnumerableCollection<TKey>(keysArray);
      }
    }

    public ICollection<TValue> Values
    {
      get
      {
        IEnumerable<TValue>[] objsArray = new IEnumerable<TValue>[this._dictionary.Count];
        for (int index = 0; index < objsArray.Length; ++index)
          objsArray[index] = (IEnumerable<TValue>) this._dictionary[index].Values;
        return (ICollection<TValue>) new HugeDictionary<TKey, TValue>.ReadonlyEnumerableCollection<TValue>(objsArray);
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        TValue obj;
        if (this.TryGetValue(key, out obj))
          return obj;
        throw new KeyNotFoundException();
      }
      set
      {
        for (int index = 0; index < this._dictionary.Count; ++index)
        {
          if (this._dictionary[index].ContainsKey(key))
          {
            this._dictionary[index][key] = value;
            return;
          }
        }
        this.Add(key, value);
      }
    }

    public int Count
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this._dictionary.Count; ++index)
          num += this._dictionary[index].Count;
        return num;
      }
    }

    public int CountDictionaries
    {
      get
      {
        return this._dictionary.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public HugeDictionary()
    {
      this._dictionary = new List<IDictionary<TKey, TValue>>();
      this._dictionary.Add((IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>());
    }

    public HugeDictionary(int maxDictionarySize)
    {
      this._maxDictionarySize = maxDictionarySize;
      this._dictionary = new List<IDictionary<TKey, TValue>>();
      this._dictionary.Add((IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>());
    }

    public void Add(TKey key, TValue value)
    {
      bool flag = false;
      for (int index = 0; index < this._dictionary.Count; ++index)
      {
        if (this._dictionary[index].ContainsKey(key))
          throw new ArgumentException("An element with the same key already exists in the System.Collections.Generic.IDictionary<TKey,TValue>.");
        if (!flag && this._dictionary[index].Count < this._maxDictionarySize)
        {
          this._dictionary[index].Add(key, value);
          flag = true;
        }
      }
      if (flag)
        return;
      this._dictionary.Add((IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>());
      this._dictionary[this._dictionary.Count - 1].Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
      for (int index = 0; index < this._dictionary.Count; ++index)
      {
        if (this._dictionary[index].ContainsKey(key))
          return true;
      }
      return false;
    }

    public bool Remove(TKey key)
    {
      for (int index = 0; index < this._dictionary.Count; ++index)
      {
        if (this._dictionary[index].Remove(key))
        {
          if (this._dictionary[index].Count == 0 && this._dictionary.Count > 1)
            this._dictionary.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      value = default (TValue);
      for (int index = 0; index < this._dictionary.Count; ++index)
      {
        if (this._dictionary[index].TryGetValue(key, out value))
          return true;
      }
      return false;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      this.Add(item.Key, item.Value);
    }

    public void Clear()
    {
      this._dictionary.Clear();
      this._dictionary.Add((IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>());
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      for (int index = 0; index < this._dictionary.Count; ++index)
      {
        if (this._dictionary[index].Contains(item))
          return true;
      }
      return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
      {
        array[arrayIndex] = keyValuePair;
        ++arrayIndex;
      }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return this.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      if (this._dictionary.Count == 0)
        return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
      IEnumerable<KeyValuePair<TKey, TValue>> first = (IEnumerable<KeyValuePair<TKey, TValue>>) this._dictionary[0];
      for (int index = 1; index < this._dictionary.Count; ++index)
        first = first.Concat<KeyValuePair<TKey, TValue>>((IEnumerable<KeyValuePair<TKey, TValue>>) this._dictionary[index]);
      return first.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private class ReadonlyEnumerableCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
      private IEnumerable<T> _enumerable;

      int ICollection<T>.Count
      {
        get
        {
          return this._enumerable.Count<T>();
        }
      }

      bool ICollection<T>.IsReadOnly
      {
        get
        {
          return true;
        }
      }

      public ReadonlyEnumerableCollection(params IEnumerable<T>[] enumerables)
      {
        this._enumerable = enumerables[0];
        for (int index = 1; index < enumerables.Length; ++index)
          this._enumerable = this._enumerable.Concat<T>(enumerables[index]);
      }

      void ICollection<T>.Add(T item)
      {
        throw new InvalidOperationException("Collection is readonly.");
      }

      void ICollection<T>.Clear()
      {
        throw new InvalidOperationException("Collection is readonly.");
      }

      bool ICollection<T>.Contains(T item)
      {
        return this._enumerable.Contains<T>(item);
      }

      void ICollection<T>.CopyTo(T[] array, int arrayIndex)
      {
        foreach (T obj in this._enumerable)
        {
          array[arrayIndex] = obj;
          ++arrayIndex;
        }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this._enumerable.GetEnumerator();
      }

      IEnumerator<T> IEnumerable<T>.GetEnumerator()
      {
        return this._enumerable.GetEnumerator();
      }

      bool ICollection<T>.Remove(T item)
      {
        throw new InvalidOperationException("Collection is readonly.");
      }
    }
  }
}
