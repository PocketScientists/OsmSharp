using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OsmSharp.Collections
{
  [DebuggerDisplay("Count={Count}")]
  public class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection
  {
    private static readonly int INITIAL_SIZE = 16;
    private int inUse;
    private int modificationCount;
    private KeyValuePair<TKey, TValue>[] table;
    private IComparer<TKey> comparer;
    private int defaultCapacity;

    public int Count
    {
      get
      {
        return this.inUse;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    bool IDictionary.IsFixedSize
    {
      get
      {
        return false;
      }
    }

    bool IDictionary.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        if ((object) key == null)
          throw new ArgumentNullException("key");
        int index = this.Find(key);
        if (index >= 0)
          return this.table[index].Value;
        throw new KeyNotFoundException();
      }
      set
      {
        if ((object) key == null)
          throw new ArgumentNullException("key");
        this.PutImpl(key, value, true);
      }
    }

    object IDictionary.this[object key]
    {
      get
      {
        if (!(key is TKey))
          return (object) null;
        return (object) this[(TKey) key];
      }
      set
      {
        this[this.ToKey(key)] = this.ToValue(value);
      }
    }

    public int Capacity
    {
      get
      {
        return this.table.Length;
      }
      set
      {
        int length = this.table.Length;
        if (this.inUse > value)
          throw new ArgumentOutOfRangeException("capacity too small");
        if (value == 0)
        {
          KeyValuePair<TKey, TValue>[] keyValuePairArray = new KeyValuePair<TKey, TValue>[this.defaultCapacity];
          Array.Copy((Array) this.table, (Array) keyValuePairArray, this.inUse);
          this.table = keyValuePairArray;
        }
        else if (value > this.inUse)
        {
          KeyValuePair<TKey, TValue>[] keyValuePairArray = new KeyValuePair<TKey, TValue>[value];
          Array.Copy((Array) this.table, (Array) keyValuePairArray, this.inUse);
          this.table = keyValuePairArray;
        }
        else
        {
          if (value <= length)
            return;
          KeyValuePair<TKey, TValue>[] keyValuePairArray = new KeyValuePair<TKey, TValue>[value];
          Array.Copy((Array) this.table, (Array) keyValuePairArray, length);
          this.table = keyValuePairArray;
        }
      }
    }

    public IList<TKey> Keys
    {
      get
      {
        return (IList<TKey>) new SortedList<TKey, TValue>.ListKeys(this);
      }
    }

    public IList<TValue> Values
    {
      get
      {
        return (IList<TValue>) new SortedList<TKey, TValue>.ListValues(this);
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        return (ICollection) new SortedList<TKey, TValue>.ListKeys(this);
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        return (ICollection) new SortedList<TKey, TValue>.ListValues(this);
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        return (ICollection<TKey>) this.Keys;
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        return (ICollection<TValue>) this.Values;
      }
    }

    public IComparer<TKey> Comparer
    {
      get
      {
        return this.comparer;
      }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public SortedList()
      : this(SortedList<TKey, TValue>.INITIAL_SIZE, (IComparer<TKey>) null)
    {
    }

    public SortedList(int capacity)
      : this(capacity, (IComparer<TKey>) null)
    {
    }

    public SortedList(int capacity, IComparer<TKey> comparer)
    {
      if (capacity < 0)
        throw new ArgumentOutOfRangeException("initialCapacity");
      this.defaultCapacity = capacity != 0 ? SortedList<TKey, TValue>.INITIAL_SIZE : 0;
      this.Init(comparer, capacity, true);
    }

    public SortedList(IComparer<TKey> comparer)
      : this(SortedList<TKey, TValue>.INITIAL_SIZE, comparer)
    {
    }

    public SortedList(IDictionary<TKey, TValue> dictionary)
      : this(dictionary, (IComparer<TKey>) null)
    {
    }

    public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
    {
      if (dictionary == null)
        throw new ArgumentNullException("dictionary");
      this.Init(comparer, dictionary.Count, true);
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) dictionary)
        this.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public void Add(TKey key, TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      this.PutImpl(key, value, false);
    }

    public bool ContainsKey(TKey key)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      return this.Find(key) >= 0;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      for (int i = 0; i < this.inUse; ++i)
      {
        KeyValuePair<TKey, TValue> keyValuePair = this.table[i];
        yield return new KeyValuePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public bool Remove(TKey key)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      int index = this.IndexOfKey(key);
      if (index < 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      this.defaultCapacity = SortedList<TKey, TValue>.INITIAL_SIZE;
      this.table = new KeyValuePair<TKey, TValue>[this.defaultCapacity];
      this.inUse = 0;
      this.modificationCount = this.modificationCount + 1;
    }

    public void Clear()
    {
      this.defaultCapacity = SortedList<TKey, TValue>.INITIAL_SIZE;
      this.table = new KeyValuePair<TKey, TValue>[this.defaultCapacity];
      this.inUse = 0;
      this.modificationCount = this.modificationCount + 1;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (this.Count == 0)
        return;
      if (array == null)
        throw new ArgumentNullException();
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException();
      if (arrayIndex >= array.Length)
        throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
      if (this.Count > array.Length - arrayIndex)
        throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");
      int num = arrayIndex;
      foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
        array[num++] = keyValuePair;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
    {
      this.Add(keyValuePair.Key, keyValuePair.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
    {
      int index = this.Find(keyValuePair.Key);
      if (index >= 0)
        return System.Collections.Generic.Comparer<KeyValuePair<TKey, TValue>>.Default.Compare(this.table[index], keyValuePair) == 0;
      return false;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
      int index = this.Find(keyValuePair.Key);
      if (index < 0 || System.Collections.Generic.Comparer<KeyValuePair<TKey, TValue>>.Default.Compare(this.table[index], keyValuePair) != 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      for (int i = 0; i < this.inUse; ++i)
      {
        KeyValuePair<TKey, TValue> keyValuePair = this.table[i];
        yield return new KeyValuePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    void IDictionary.Add(object key, object value)
    {
      this.PutImpl(this.ToKey(key), this.ToValue(value), false);
    }

    bool IDictionary.Contains(object key)
    {
      if (key == null)
        throw new ArgumentNullException();
      if (!(key is TKey))
        return false;
      return this.Find((TKey) key) >= 0;
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new SortedList<TKey, TValue>.Enumerator(this, SortedList<TKey, TValue>.EnumeratorMode.ENTRY_MODE);
    }

    void IDictionary.Remove(object key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (!(key is TKey))
        return;
      int index = this.IndexOfKey((TKey) key);
      if (index < 0)
        return;
      this.RemoveAt(index);
    }

    void ICollection.CopyTo(Array array, int arrayIndex)
    {
      if (this.Count == 0)
        return;
      if (array == null)
        throw new ArgumentNullException();
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException();
      if (array.Rank > 1)
        throw new ArgumentException("array is multi-dimensional");
      if (arrayIndex >= array.Length)
        throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
      if (this.Count > array.Length - arrayIndex)
        throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");
      IEnumerator<KeyValuePair<TKey, TValue>> enumerator = this.GetEnumerator();
      int num = arrayIndex;
      while (enumerator.MoveNext())
        array.SetValue((object) enumerator.Current, num++);
    }

    public void RemoveAt(int index)
    {
      KeyValuePair<TKey, TValue>[] table = this.table;
      int count = this.Count;
      if (index < 0 || index >= count)
        throw new ArgumentOutOfRangeException("index out of range");
      if (index != count - 1)
        Array.Copy((Array) table, index + 1, (Array) table, index, count - 1 - index);
      else
        table[index] = new KeyValuePair<TKey, TValue>();
      this.inUse = this.inUse - 1;
      this.modificationCount = this.modificationCount + 1;
    }

    public int IndexOfKey(TKey key)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      int num;
      try
      {
        num = this.Find(key);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException();
      }
      return num | num >> 31;
    }

    public int IndexOfValue(TValue value)
    {
      if (this.inUse == 0)
        return -1;
      for (int index = 0; index < this.inUse; ++index)
      {
        KeyValuePair<TKey, TValue> keyValuePair = this.table[index];
        if (object.Equals((object) value, (object) keyValuePair.Value))
          return index;
      }
      return -1;
    }

    public bool ContainsValue(TValue value)
    {
      return this.IndexOfValue(value) >= 0;
    }

    public void TrimExcess()
    {
      if ((double) this.inUse >= (double) this.table.Length * 0.9)
        return;
      this.Capacity = this.inUse;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      int index = this.Find(key);
      if (index >= 0)
      {
        value = this.table[index].Value;
        return true;
      }
      value = default (TValue);
      return false;
    }

    private void EnsureCapacity(int n, int free)
    {
      KeyValuePair<TKey, TValue>[] table = this.table;
      KeyValuePair<TKey, TValue>[] keyValuePairArray = (KeyValuePair<TKey, TValue>[]) null;
      int capacity = this.Capacity;
      bool flag = free >= 0 && free < this.Count;
      if (n > capacity)
        keyValuePairArray = new KeyValuePair<TKey, TValue>[n << 1];
      if (keyValuePairArray != null)
      {
        if (flag)
        {
          int length1 = free;
          if (length1 > 0)
            Array.Copy((Array) table, 0, (Array) keyValuePairArray, 0, length1);
          int length2 = this.Count - free;
          if (length2 > 0)
            Array.Copy((Array) table, free, (Array) keyValuePairArray, free + 1, length2);
        }
        else
          Array.Copy((Array) table, (Array) keyValuePairArray, this.Count);
        this.table = keyValuePairArray;
      }
      else
      {
        if (!flag)
          return;
        Array.Copy((Array) table, free, (Array) table, free + 1, this.Count - free);
      }
    }

    private void PutImpl(TKey key, TValue value, bool overwrite)
    {
      if ((object) key == null)
        throw new ArgumentNullException("null key");
      KeyValuePair<TKey, TValue>[] table = this.table;
      int index;
      try
      {
        index = this.Find(key);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException();
      }
      if (index >= 0)
      {
        if (!overwrite)
          throw new ArgumentException("element already exists");
        table[index] = new KeyValuePair<TKey, TValue>(key, value);
        this.modificationCount = this.modificationCount + 1;
      }
      else
      {
        int free = ~index;
        if (free > this.Capacity + 1)
          throw new Exception("SortedList::internal error (" + (object) key + ", " + (object) value + ") at [" + (object) free + "]");
        this.EnsureCapacity(this.Count + 1, free);
        this.table[free] = new KeyValuePair<TKey, TValue>(key, value);
        this.inUse = this.inUse + 1;
        this.modificationCount = this.modificationCount + 1;
      }
    }

    private void Init(IComparer<TKey> comparer, int capacity, bool forceSize)
    {
      if (comparer == null)
        comparer = (IComparer<TKey>) System.Collections.Generic.Comparer<TKey>.Default;
      this.comparer = comparer;
      if (!forceSize && capacity < this.defaultCapacity)
        capacity = this.defaultCapacity;
      this.table = new KeyValuePair<TKey, TValue>[capacity];
      this.inUse = 0;
      this.modificationCount = 0;
    }

    private void CopyToArray(Array arr, int i, SortedList<TKey, TValue>.EnumeratorMode mode)
    {
      if (arr == null)
        throw new ArgumentNullException("arr");
      if (i < 0 || i + this.Count > arr.Length)
        throw new ArgumentOutOfRangeException("i");
      IEnumerator enumerator = (IEnumerator) new SortedList<TKey, TValue>.Enumerator(this, mode);
      while (enumerator.MoveNext())
        arr.SetValue(enumerator.Current, i++);
    }

    private int Find(TKey key)
    {
      KeyValuePair<TKey, TValue>[] table = this.table;
      int count = this.Count;
      if (count == 0)
        return -1;
      int num1 = 0;
      int num2 = count - 1;
      while (num1 <= num2)
      {
        int index = num1 + num2 >> 1;
        int num3 = this.comparer.Compare(table[index].Key, key);
        if (num3 == 0)
          return index;
        if (num3 < 0)
          num1 = index + 1;
        else
          num2 = index - 1;
      }
      return ~num1;
    }

    private TKey ToKey(object key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (!(key is TKey))
        throw new ArgumentException("The value \"" + key + "\" isn't of type \"" + (object) typeof (TKey) + "\" and can't be used in this generic collection.", "key");
      return (TKey) key;
    }

    private TValue ToValue(object value)
    {
      if (!(value is TValue))
        throw new ArgumentException("The value \"" + value + "\" isn't of type \"" + (object) typeof (TValue) + "\" and can't be used in this generic collection.", "value");
      return (TValue) value;
    }

    internal TKey KeyAt(int index)
    {
      if (index >= 0 && index < this.Count)
        return this.table[index].Key;
      throw new ArgumentOutOfRangeException("Index out of range");
    }

    internal TValue ValueAt(int index)
    {
      if (index >= 0 && index < this.Count)
        return this.table[index].Value;
      throw new ArgumentOutOfRangeException("Index out of range");
    }

    private enum EnumeratorMode
    {
      KEY_MODE,
      VALUE_MODE,
      ENTRY_MODE,
    }

    private sealed class Enumerator : ICloneable, IDictionaryEnumerator, IEnumerator
    {
      private static readonly string xstr = "SortedList.Enumerator: snapshot out of sync.";
      private SortedList<TKey, TValue> host;
      private int stamp;
      private int pos;
      private int size;
      private SortedList<TKey, TValue>.EnumeratorMode mode;
      private object currentKey;
      private object currentValue;
      private bool invalid;

      public DictionaryEntry Entry
      {
        get
        {
          if (this.invalid || this.pos >= this.size || this.pos == -1)
            throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
          return new DictionaryEntry(this.currentKey, this.currentValue);
        }
      }

      public object Key
      {
        get
        {
          if (this.invalid || this.pos >= this.size || this.pos == -1)
            throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
          return this.currentKey;
        }
      }

      public object Value
      {
        get
        {
          if (this.invalid || this.pos >= this.size || this.pos == -1)
            throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
          return this.currentValue;
        }
      }

      public object Current
      {
        get
        {
          if (this.invalid || this.pos >= this.size || this.pos == -1)
            throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
          switch (this.mode)
          {
            case SortedList<TKey, TValue>.EnumeratorMode.KEY_MODE:
              return this.currentKey;
            case SortedList<TKey, TValue>.EnumeratorMode.VALUE_MODE:
              return this.currentValue;
            case SortedList<TKey, TValue>.EnumeratorMode.ENTRY_MODE:
              return (object) this.Entry;
            default:
              throw new NotSupportedException(((int) this.mode).ToString() + " is not a supported mode.");
          }
        }
      }

      public Enumerator(SortedList<TKey, TValue> host, SortedList<TKey, TValue>.EnumeratorMode mode)
      {
        this.host = host;
        this.stamp = host.modificationCount;
        this.size = host.Count;
        this.mode = mode;
        this.Reset();
      }

      public Enumerator(SortedList<TKey, TValue> host)
        : this(host, SortedList<TKey, TValue>.EnumeratorMode.ENTRY_MODE)
      {
      }

      public void Reset()
      {
        if (this.host.modificationCount != this.stamp || this.invalid)
          throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
        this.pos = -1;
        this.currentKey = (object) null;
        this.currentValue = (object) null;
      }

      public bool MoveNext()
      {
        if (this.host.modificationCount != this.stamp || this.invalid)
          throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
        KeyValuePair<TKey, TValue>[] table = this.host.table;
        int num = this.pos + 1;
        this.pos = num;
        if (num < this.size)
        {
          KeyValuePair<TKey, TValue> keyValuePair = table[this.pos];
          this.currentKey = (object) keyValuePair.Key;
          this.currentValue = (object) keyValuePair.Value;
          return true;
        }
        this.currentKey = (object) null;
        this.currentValue = (object) null;
        return false;
      }

      public object Clone()
      {
        SortedList<TKey, TValue>.Enumerator enumerator = new SortedList<TKey, TValue>.Enumerator(this.host, this.mode);
        enumerator.stamp = this.stamp;
        enumerator.pos = this.pos;
        enumerator.size = this.size;
        enumerator.currentKey = this.currentKey;
        enumerator.currentValue = this.currentValue;
        int num = this.invalid ? 1 : 0;
        enumerator.invalid = num != 0;
        return (object) enumerator;
      }
    }

    private struct KeyEnumerator : IEnumerator<TKey>, IEnumerator, IDisposable
    {
      private const int NOT_STARTED = -2;
      private const int FINISHED = -1;
      private SortedList<TKey, TValue> l;
      private int idx;
      private int ver;

      public TKey Current
      {
        get
        {
          if (this.idx < 0)
            throw new InvalidOperationException();
          return this.l.KeyAt(this.l.Count - 1 - this.idx);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      internal KeyEnumerator(SortedList<TKey, TValue> l)
      {
        this.l = l;
        this.idx = -2;
        this.ver = l.modificationCount;
      }

      public void Dispose()
      {
        this.idx = -2;
      }

      public bool MoveNext()
      {
        if (this.ver != this.l.modificationCount)
          throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
        if (this.idx == -2)
          this.idx = this.l.Count;
        if (this.idx == -1)
          return false;
        int num = this.idx - 1;
        this.idx = num;
        return num != -1;
      }

      void IEnumerator.Reset()
      {
        if (this.ver != this.l.modificationCount)
          throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
        this.idx = -2;
      }
    }

    private struct ValueEnumerator : IEnumerator<TValue>, IEnumerator, IDisposable
    {
      private const int NOT_STARTED = -2;
      private const int FINISHED = -1;
      private SortedList<TKey, TValue> l;
      private int idx;
      private int ver;

      public TValue Current
      {
        get
        {
          if (this.idx < 0)
            throw new InvalidOperationException();
          return this.l.ValueAt(this.l.Count - 1 - this.idx);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      internal ValueEnumerator(SortedList<TKey, TValue> l)
      {
        this.l = l;
        this.idx = -2;
        this.ver = l.modificationCount;
      }

      public void Dispose()
      {
        this.idx = -2;
      }

      public bool MoveNext()
      {
        if (this.ver != this.l.modificationCount)
          throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
        if (this.idx == -2)
          this.idx = this.l.Count;
        if (this.idx == -1)
          return false;
        int num = this.idx - 1;
        this.idx = num;
        return num != -1;
      }

      void IEnumerator.Reset()
      {
        if (this.ver != this.l.modificationCount)
          throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
        this.idx = -2;
      }
    }

    private class ListKeys : IList<TKey>, ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection
    {
      private SortedList<TKey, TValue> host;

      public virtual TKey this[int index]
      {
        get
        {
          return this.host.KeyAt(index);
        }
        set
        {
          throw new NotSupportedException("attempt to modify a key");
        }
      }

      public virtual int Count
      {
        get
        {
          return this.host.Count;
        }
      }

      public virtual bool IsSynchronized
      {
        get
        {
          return ((ICollection) this.host).IsSynchronized;
        }
      }

      public virtual bool IsReadOnly
      {
        get
        {
          return true;
        }
      }

      public virtual object SyncRoot
      {
        get
        {
          return ((ICollection) this.host).SyncRoot;
        }
      }

      public ListKeys(SortedList<TKey, TValue> host)
      {
        if (host == null)
          throw new ArgumentNullException();
        this.host = host;
      }

      public virtual void Add(TKey item)
      {
        throw new NotSupportedException();
      }

      public virtual bool Remove(TKey key)
      {
        throw new NotSupportedException();
      }

      public virtual void Clear()
      {
        throw new NotSupportedException();
      }

      public virtual void CopyTo(TKey[] array, int arrayIndex)
      {
        if (this.host.Count == 0)
          return;
        if (array == null)
          throw new ArgumentNullException("array");
        if (arrayIndex < 0)
          throw new ArgumentOutOfRangeException();
        if (arrayIndex >= array.Length)
          throw new ArgumentOutOfRangeException("arrayIndex is greater than or equal to array.Length");
        if (this.Count > array.Length - arrayIndex)
          throw new ArgumentOutOfRangeException("Not enough space in array from arrayIndex to end of array");
        int num = arrayIndex;
        for (int index = 0; index < this.Count; ++index)
          array[num++] = this.host.KeyAt(index);
      }

      public virtual bool Contains(TKey item)
      {
        return this.host.IndexOfKey(item) > -1;
      }

      public virtual int IndexOf(TKey item)
      {
        return this.host.IndexOfKey(item);
      }

      public virtual void Insert(int index, TKey item)
      {
        throw new NotSupportedException();
      }

      public virtual void RemoveAt(int index)
      {
        throw new NotSupportedException();
      }

      public virtual IEnumerator<TKey> GetEnumerator()
      {
        return (IEnumerator<TKey>) new SortedList<TKey, TValue>.KeyEnumerator(this.host);
      }

      public virtual void CopyTo(Array array, int arrayIndex)
      {
        this.host.CopyToArray(array, arrayIndex, SortedList<TKey, TValue>.EnumeratorMode.KEY_MODE);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < this.host.Count; ++i)
          yield return (object) this.host.KeyAt(i);
      }
    }

    private class ListValues : IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection
    {
      private SortedList<TKey, TValue> host;

      public virtual TValue this[int index]
      {
        get
        {
          return this.host.ValueAt(index);
        }
        set
        {
          throw new NotSupportedException("attempt to modify a key");
        }
      }

      public virtual int Count
      {
        get
        {
          return this.host.Count;
        }
      }

      public virtual bool IsSynchronized
      {
        get
        {
          return ((ICollection) this.host).IsSynchronized;
        }
      }

      public virtual bool IsReadOnly
      {
        get
        {
          return true;
        }
      }

      public virtual object SyncRoot
      {
        get
        {
          return ((ICollection) this.host).SyncRoot;
        }
      }

      public ListValues(SortedList<TKey, TValue> host)
      {
        if (host == null)
          throw new ArgumentNullException();
        this.host = host;
      }

      public virtual void Add(TValue item)
      {
        throw new NotSupportedException();
      }

      public virtual bool Remove(TValue value)
      {
        throw new NotSupportedException();
      }

      public virtual void Clear()
      {
        throw new NotSupportedException();
      }

      public virtual void CopyTo(TValue[] array, int arrayIndex)
      {
        if (this.host.Count == 0)
          return;
        if (array == null)
          throw new ArgumentNullException("array");
        if (arrayIndex < 0)
          throw new ArgumentOutOfRangeException();
        if (arrayIndex >= array.Length)
          throw new ArgumentOutOfRangeException("arrayIndex is greater than or equal to array.Length");
        if (this.Count > array.Length - arrayIndex)
          throw new ArgumentOutOfRangeException("Not enough space in array from arrayIndex to end of array");
        int num = arrayIndex;
        for (int index = 0; index < this.Count; ++index)
          array[num++] = this.host.ValueAt(index);
      }

      public virtual bool Contains(TValue item)
      {
        return this.host.IndexOfValue(item) > -1;
      }

      public virtual int IndexOf(TValue item)
      {
        return this.host.IndexOfValue(item);
      }

      public virtual void Insert(int index, TValue item)
      {
        throw new NotSupportedException();
      }

      public virtual void RemoveAt(int index)
      {
        throw new NotSupportedException();
      }

      public virtual IEnumerator<TValue> GetEnumerator()
      {
        return (IEnumerator<TValue>) new SortedList<TKey, TValue>.ValueEnumerator(this.host);
      }

      public virtual void CopyTo(Array array, int arrayIndex)
      {
        this.host.CopyToArray(array, arrayIndex, SortedList<TKey, TValue>.EnumeratorMode.VALUE_MODE);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < this.host.Count; ++i)
          yield return (object) this.host.ValueAt(i);
      }
    }
  }
}
