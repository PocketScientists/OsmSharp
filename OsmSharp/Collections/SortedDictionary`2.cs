using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OsmSharp.Collections
{
  [DebuggerDisplay("Count={Count}")]
  public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection
  {
    private RBTree tree;
    private SortedDictionary<TKey, TValue>.NodeHelper hlp;

    public IComparer<TKey> Comparer
    {
      get
      {
        return this.hlp.cmp;
      }
    }

    public int Count
    {
      get
      {
        return this.tree.Count;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node) this.tree.Lookup<TKey>(key);
        if (node == null)
          throw new KeyNotFoundException();
        return node.value;
      }
      set
      {
        if ((object) key == null)
          throw new ArgumentNullException("key");
        ((SortedDictionary<TKey, TValue>.Node) this.tree.Intern<TKey>(key, (RBTree.Node) null)).value = value;
      }
    }

    public SortedDictionary<TKey, TValue>.KeyCollection Keys
    {
      get
      {
        return new SortedDictionary<TKey, TValue>.KeyCollection(this);
      }
    }

    public SortedDictionary<TKey, TValue>.ValueCollection Values
    {
      get
      {
        return new SortedDictionary<TKey, TValue>.ValueCollection(this);
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        return (ICollection<TKey>) new SortedDictionary<TKey, TValue>.KeyCollection(this);
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        return (ICollection<TValue>) new SortedDictionary<TKey, TValue>.ValueCollection(this);
      }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        return false;
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

    ICollection IDictionary.Keys
    {
      get
      {
        return (ICollection) new SortedDictionary<TKey, TValue>.KeyCollection(this);
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        return (ICollection) new SortedDictionary<TKey, TValue>.ValueCollection(this);
      }
    }

    object IDictionary.this[object key]
    {
      get
      {
        return (object) this[this.ToKey(key)];
      }
      set
      {
        this[this.ToKey(key)] = this.ToValue(value);
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

    public SortedDictionary()
      : this((IComparer<TKey>) null)
    {
    }

    public SortedDictionary(IComparer<TKey> comparer)
    {
      this.hlp = SortedDictionary<TKey, TValue>.NodeHelper.GetHelper(comparer);
      this.tree = new RBTree((object) this.hlp);
    }

    public SortedDictionary(IDictionary<TKey, TValue> dic)
      : this(dic, (IComparer<TKey>) null)
    {
    }

    public SortedDictionary(IDictionary<TKey, TValue> dic, IComparer<TKey> comparer)
      : this(comparer)
    {
      if (dic == null)
        throw new ArgumentNullException();
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) dic)
        this.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public void Add(TKey key, TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      RBTree.Node new_node = (RBTree.Node) new SortedDictionary<TKey, TValue>.Node(key, value);
      if (this.tree.Intern<TKey>(key, new_node) != new_node)
        throw new ArgumentException("key already present in dictionary", "key");
    }

    public void Clear()
    {
      this.tree.Clear();
    }

    public bool ContainsKey(TKey key)
    {
      return this.tree.Lookup<TKey>(key) != null;
    }

    public bool ContainsValue(TValue value)
    {
      IEqualityComparer<TValue> equalityComparer = (IEqualityComparer<TValue>) EqualityComparer<TValue>.Default;
      foreach (SortedDictionary<TKey, TValue>.Node node in this.tree)
      {
        if (equalityComparer.Equals(value, node.value))
          return true;
      }
      return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (this.Count == 0)
        return;
      if (array == null)
        throw new ArgumentNullException();
      if (arrayIndex < 0 || array.Length <= arrayIndex)
        throw new ArgumentOutOfRangeException();
      if (array.Length - arrayIndex < this.Count)
        throw new ArgumentException();
      foreach (SortedDictionary<TKey, TValue>.Node node in this.tree)
        array[arrayIndex++] = node.AsKV();
    }

    public SortedDictionary<TKey, TValue>.Enumerator GetEnumerator()
    {
      return new SortedDictionary<TKey, TValue>.Enumerator(this);
    }

    public bool Remove(TKey key)
    {
      return this.tree.Remove<TKey>(key) != null;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node) this.tree.Lookup<TKey>(key);
      value = node == null ? default (TValue) : node.value;
      return node != null;
    }

    private TKey ToKey(object key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (!(key is TKey))
        throw new ArgumentException(string.Format("Key \"{0}\" cannot be converted to the key type {1}.", new object[2]
        {
          key,
          (object) typeof (TKey)
        }));
      return (TKey) key;
    }

    private TValue ToValue(object value)
    {
      if (!(value is TValue) && (value != null || typeof (TValue).IsValueType))
        throw new ArgumentException(string.Format("Value \"{0}\" cannot be converted to the value type {1}.", new object[2]
        {
          value,
          (object) typeof (TValue)
        }));
      return (TValue) value;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      this.Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      TValue y;
      if (this.TryGetValue(item.Key, out y))
        return EqualityComparer<TValue>.Default.Equals(item.Value, y);
      return false;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      TValue y;
      if (this.TryGetValue(item.Key, out y) && EqualityComparer<TValue>.Default.Equals(item.Value, y))
        return this.Remove(item.Key);
      return false;
    }

    void IDictionary.Add(object key, object value)
    {
      this.Add(this.ToKey(key), this.ToValue(value));
    }

    bool IDictionary.Contains(object key)
    {
      return this.ContainsKey(this.ToKey(key));
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new SortedDictionary<TKey, TValue>.Enumerator(this);
    }

    void IDictionary.Remove(object key)
    {
      this.Remove(this.ToKey(key));
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (this.Count == 0)
        return;
      if (array == null)
        throw new ArgumentNullException();
      if (index < 0 || array.Length <= index)
        throw new ArgumentOutOfRangeException();
      if (array.Length - index < this.Count)
        throw new ArgumentException();
      foreach (SortedDictionary<TKey, TValue>.Node node in this.tree)
        array.SetValue((object) node.AsDE(), index++);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new SortedDictionary<TKey, TValue>.Enumerator(this);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<TKey, TValue>>) new SortedDictionary<TKey, TValue>.Enumerator(this);
    }

    private class Node : RBTree.Node
    {
      public TKey key;
      public TValue value;

      public Node(TKey key)
      {
        this.key = key;
      }

      public Node(TKey key, TValue value)
      {
        this.key = key;
        this.value = value;
      }

      public override void SwapValue(RBTree.Node other)
      {
        SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node) other;
        TKey key = this.key;
        this.key = node.key;
        node.key = key;
        TValue obj = this.value;
        this.value = node.value;
        node.value = obj;
      }

      public KeyValuePair<TKey, TValue> AsKV()
      {
        return new KeyValuePair<TKey, TValue>(this.key, this.value);
      }

      public DictionaryEntry AsDE()
      {
        return new DictionaryEntry((object) this.key, (object) this.value);
      }
    }

    private class NodeHelper : RBTree.INodeHelper<TKey>
    {
      private static SortedDictionary<TKey, TValue>.NodeHelper Default = new SortedDictionary<TKey, TValue>.NodeHelper((IComparer<TKey>) System.Collections.Generic.Comparer<TKey>.Default);
      public IComparer<TKey> cmp;

      private NodeHelper(IComparer<TKey> cmp)
      {
        this.cmp = cmp;
      }

      public int Compare(TKey key, RBTree.Node node)
      {
        return this.cmp.Compare(key, ((SortedDictionary<TKey, TValue>.Node) node).key);
      }

      public RBTree.Node CreateNode(TKey key)
      {
        return (RBTree.Node) new SortedDictionary<TKey, TValue>.Node(key);
      }

      public static SortedDictionary<TKey, TValue>.NodeHelper GetHelper(IComparer<TKey> cmp)
      {
        if (cmp == null || cmp == System.Collections.Generic.Comparer<TKey>.Default)
          return SortedDictionary<TKey, TValue>.NodeHelper.Default;
        return new SortedDictionary<TKey, TValue>.NodeHelper(cmp);
      }
    }

    [DebuggerDisplay("Count={Count}")]
    public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection
    {
      private SortedDictionary<TKey, TValue> _dic;

      public int Count
      {
        get
        {
          return this._dic.Count;
        }
      }

      bool ICollection<TValue>.IsReadOnly
      {
        get
        {
          return true;
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
          return (object) this._dic;
        }
      }

      public ValueCollection(SortedDictionary<TKey, TValue> dic)
      {
        this._dic = dic;
      }

      void ICollection<TValue>.Add(TValue item)
      {
        throw new NotSupportedException();
      }

      void ICollection<TValue>.Clear()
      {
        throw new NotSupportedException();
      }

      bool ICollection<TValue>.Contains(TValue item)
      {
        return this._dic.ContainsValue(item);
      }

      public void CopyTo(TValue[] array, int arrayIndex)
      {
        if (this.Count == 0)
          return;
        if (array == null)
          throw new ArgumentNullException();
        if (arrayIndex < 0 || array.Length <= arrayIndex)
          throw new ArgumentOutOfRangeException();
        if (array.Length - arrayIndex < this.Count)
          throw new ArgumentException();
        foreach (SortedDictionary<TKey, TValue>.Node node in this._dic.tree)
          array[arrayIndex++] = node.value;
      }

      bool ICollection<TValue>.Remove(TValue item)
      {
        throw new NotSupportedException();
      }

      IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
      {
        return (IEnumerator<TValue>) this.GetEnumerator();
      }

      public SortedDictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
      {
        return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dic);
      }

      void ICollection.CopyTo(Array array, int index)
      {
        if (this.Count == 0)
          return;
        if (array == null)
          throw new ArgumentNullException();
        if (index < 0 || array.Length <= index)
          throw new ArgumentOutOfRangeException();
        if (array.Length - index < this.Count)
          throw new ArgumentException();
        foreach (SortedDictionary<TKey, TValue>.Node node in this._dic.tree)
          array.SetValue((object) node.value, index++);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dic);
      }

      public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
      {
        private RBTree.NodeEnumerator host;
        private TValue current;

        public TValue Current
        {
          get
          {
            return this.current;
          }
        }

        object IEnumerator.Current
        {
          get
          {
            this.host.check_current();
            return (object) this.current;
          }
        }

        internal Enumerator(SortedDictionary<TKey, TValue> dic)
        {
          this = new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator();
          this.host = dic.tree.GetEnumerator();
        }

        public bool MoveNext()
        {
          if (!this.host.MoveNext())
            return false;
          this.current = ((SortedDictionary<TKey, TValue>.Node) this.host.Current).value;
          return true;
        }

        public void Dispose()
        {
          this.host.Dispose();
        }

        void IEnumerator.Reset()
        {
          this.host.Reset();
        }
      }
    }

    [DebuggerDisplay("Count={Count}")]
    public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection
    {
      private SortedDictionary<TKey, TValue> _dic;

      public int Count
      {
        get
        {
          return this._dic.Count;
        }
      }

      bool ICollection<TKey>.IsReadOnly
      {
        get
        {
          return true;
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
          return (object) this._dic;
        }
      }

      public KeyCollection(SortedDictionary<TKey, TValue> dic)
      {
        this._dic = dic;
      }

      void ICollection<TKey>.Add(TKey item)
      {
        throw new NotSupportedException();
      }

      void ICollection<TKey>.Clear()
      {
        throw new NotSupportedException();
      }

      bool ICollection<TKey>.Contains(TKey item)
      {
        return this._dic.ContainsKey(item);
      }

      IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
      {
        return (IEnumerator<TKey>) this.GetEnumerator();
      }

      public void CopyTo(TKey[] array, int arrayIndex)
      {
        if (this.Count == 0)
          return;
        if (array == null)
          throw new ArgumentNullException();
        if (arrayIndex < 0 || array.Length <= arrayIndex)
          throw new ArgumentOutOfRangeException();
        if (array.Length - arrayIndex < this.Count)
          throw new ArgumentException();
        foreach (SortedDictionary<TKey, TValue>.Node node in this._dic.tree)
          array[arrayIndex++] = node.key;
      }

      bool ICollection<TKey>.Remove(TKey item)
      {
        throw new NotSupportedException();
      }

      public SortedDictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
      {
        return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dic);
      }

      void ICollection.CopyTo(Array array, int index)
      {
        if (this.Count == 0)
          return;
        if (array == null)
          throw new ArgumentNullException();
        if (index < 0 || array.Length <= index)
          throw new ArgumentOutOfRangeException();
        if (array.Length - index < this.Count)
          throw new ArgumentException();
        foreach (SortedDictionary<TKey, TValue>.Node node in this._dic.tree)
          array.SetValue((object) node.key, index++);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dic);
      }

      public struct Enumerator : IEnumerator<TKey>, IEnumerator, IDisposable
      {
        private RBTree.NodeEnumerator host;
        private TKey current;

        public TKey Current
        {
          get
          {
            return this.current;
          }
        }

        object IEnumerator.Current
        {
          get
          {
            this.host.check_current();
            return (object) this.current;
          }
        }

        internal Enumerator(SortedDictionary<TKey, TValue> dic)
        {
          this = new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator();
          this.host = dic.tree.GetEnumerator();
        }

        public bool MoveNext()
        {
          if (!this.host.MoveNext())
            return false;
          this.current = ((SortedDictionary<TKey, TValue>.Node) this.host.Current).key;
          return true;
        }

        public void Dispose()
        {
          this.host.Dispose();
        }

        void IEnumerator.Reset()
        {
          this.host.Reset();
        }
      }
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable, IDictionaryEnumerator
    {
      private RBTree.NodeEnumerator host;
      private KeyValuePair<TKey, TValue> current;

      public KeyValuePair<TKey, TValue> Current
      {
        get
        {
          return this.current;
        }
      }

      private SortedDictionary<TKey, TValue>.Node CurrentNode
      {
        get
        {
          this.host.check_current();
          return (SortedDictionary<TKey, TValue>.Node) this.host.Current;
        }
      }

      DictionaryEntry IDictionaryEnumerator.Entry
      {
        get
        {
          return this.CurrentNode.AsDE();
        }
      }

      object IDictionaryEnumerator.Key
      {
        get
        {
          return (object) this.CurrentNode.key;
        }
      }

      object IDictionaryEnumerator.Value
      {
        get
        {
          return (object) this.CurrentNode.value;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.CurrentNode.AsDE();
        }
      }

      internal Enumerator(SortedDictionary<TKey, TValue> dic)
      {
        this = new SortedDictionary<TKey, TValue>.Enumerator();
        this.host = dic.tree.GetEnumerator();
      }

      public bool MoveNext()
      {
        if (!this.host.MoveNext())
          return false;
        this.current = ((SortedDictionary<TKey, TValue>.Node) this.host.Current).AsKV();
        return true;
      }

      public void Dispose()
      {
        this.host.Dispose();
      }

      void IEnumerator.Reset()
      {
        this.host.Reset();
      }
    }
  }
}
