using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections
{
  public class StringTableDictionary<Type> : IDictionary<Type, Type>, ICollection<KeyValuePair<Type, Type>>, IEnumerable<KeyValuePair<Type, Type>>, IEnumerable
  {
    private ObjectTable<Type> _string_table;
    private Dictionary<uint, uint> _dictionary;

    public ICollection<Type> Keys
    {
      get
      {
        List<Type> ypeList = new List<Type>();
        foreach (uint key in this._dictionary.Keys)
          ypeList.Add(this._string_table.Get(key));
        return (ICollection<Type>) ypeList;
      }
    }

    public ICollection<Type> Values
    {
      get
      {
        List<Type> ypeList = new List<Type>();
        foreach (uint valueIdx in this._dictionary.Values)
          ypeList.Add(this._string_table.Get(valueIdx));
        return (ICollection<Type>) ypeList;
      }
    }

    public Type this[Type key]
    {
      get
      {
        return this._string_table.Get(this._dictionary[this._string_table.Add(key)]);
      }
      set
      {
        this._dictionary[this._string_table.Add(key)] = this._string_table.Add(value);
      }
    }

    public int Count
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

    public StringTableDictionary(ObjectTable<Type> string_table)
    {
      this._string_table = string_table;
      this._dictionary = new Dictionary<uint, uint>();
    }

    public void Add(Type key, Type value)
    {
      this._dictionary.Add(this._string_table.Add(key), this._string_table.Add(value));
    }

    public bool ContainsKey(Type key)
    {
      return this._dictionary.ContainsKey(this._string_table.Add(key));
    }

    public bool Remove(Type key)
    {
      return this._dictionary.Remove(this._string_table.Add(key));
    }

    public bool TryGetValue(Type key, out Type value)
    {
      uint key1 = this._string_table.Add(key);
      value = default (Type);
      uint valueIdx;
      if (this._dictionary.TryGetValue(key1, out valueIdx))
        value = this._string_table.Get(valueIdx);
      return false;
    }

    public void Add(KeyValuePair<Type, Type> item)
    {
      KeyValuePair<uint, uint> keyValuePair = new KeyValuePair<uint, uint>(this._string_table.Add(item.Key), this._string_table.Add(item.Key));
      this._dictionary.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public void Clear()
    {
      this._dictionary.Clear();
    }

    public bool Contains(KeyValuePair<Type, Type> item)
    {
      uint num;
      if (this._dictionary.TryGetValue(this._string_table.Add(item.Key), out num))
        return (int) num == (int) this._string_table.Add(item.Value);
      return false;
    }

    public void CopyTo(KeyValuePair<Type, Type>[] array, int arrayIndex)
    {
      foreach (KeyValuePair<Type, Type> keyValuePair in this)
      {
        array[arrayIndex] = keyValuePair;
        ++arrayIndex;
      }
    }

    public bool Remove(KeyValuePair<Type, Type> item)
    {
      return this.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<Type, Type>> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    private class StringTableDictionaryEnumerator : IEnumerator<KeyValuePair<Type, Type>>, IEnumerator, IDisposable
    {
      private KeyValuePair<Type, Type> _current;
      private IEnumerator<KeyValuePair<uint, uint>> _enumerator;
      private ObjectTable<Type> _string_table;

      public KeyValuePair<Type, Type> Current
      {
        get
        {
          return this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this._current;
        }
      }

      public StringTableDictionaryEnumerator(ObjectTable<Type> string_table, IEnumerator<KeyValuePair<uint, uint>> enumerator)
      {
        this._enumerator = enumerator;
        this._string_table = string_table;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (!this._enumerator.MoveNext())
          return false;
        this._current = new KeyValuePair<Type, Type>(this._string_table.Get(this._enumerator.Current.Key), this._string_table.Get(this._enumerator.Current.Value));
        return true;
      }

      public void Reset()
      {
        this._enumerator.Reset();
      }
    }
  }
}
