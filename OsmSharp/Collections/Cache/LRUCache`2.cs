using OsmSharp.Collections.PriorityQueues;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections.Cache
{
  public class LRUCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
  {
    private Dictionary<TKey, LRUCache<TKey, TValue>.CacheEntry> _data;
    private ulong _id;
    private ulong _lastId;
    public LRUCache<TKey, TValue>.OnRemoveDelegate OnRemove;

    public int MaxCapacity { get; private set; }

    public int MinCapacity { get; private set; }

    public int Count
    {
      get
      {
        return this._data.Count;
      }
    }

    public LRUCache(int capacity)
    {
      this._id = 0UL;
      this._lastId = this._id;
      this._data = new Dictionary<TKey, LRUCache<TKey, TValue>.CacheEntry>();
      this.MaxCapacity = capacity / 100 * 10 + capacity + 1;
      this.MinCapacity = capacity;
    }

    public void Add(TKey key, TValue value)
    {
      LRUCache<TKey, TValue>.CacheEntry cacheEntry = new LRUCache<TKey, TValue>.CacheEntry()
      {
        Id = this._id,
        Value = value
      };
      lock (this._data)
      {
        this._id = this._id + 1UL;
        this._data[key] = cacheEntry;
      }
      this.ResizeCache();
    }

    public bool TryGet(TKey key, out TValue value)
    {
      lock (this._data)
      {
        this._id = this._id + 1UL;
        LRUCache<TKey, TValue>.CacheEntry local_2;
        if (this._data.TryGetValue(key, out local_2))
        {
          local_2.Id = this._id;
          value = local_2.Value;
          return true;
        }
      }
      value = default (TValue);
      return false;
    }

    public bool TryPeek(TKey key, out TValue value)
    {
      lock (this._data)
      {
        LRUCache<TKey, TValue>.CacheEntry local_2;
        if (this._data.TryGetValue(key, out local_2))
        {
          value = local_2.Value;
          return true;
        }
      }
      value = default (TValue);
      return false;
    }

    public void Clear()
    {
      lock (this._data)
      {
        if (this.OnRemove != null)
        {
          foreach (KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry> item_0 in this._data)
            this.OnRemove(item_0.Value.Value);
        }
        this._data.Clear();
      }
      this._id = 0UL;
      this._lastId = this._id;
    }

    public void Remove(TKey id)
    {
      lock (this._data)
        this._data.Remove(id);
    }

    private void ResizeCache()
    {
      lock (this._data)
      {
        if (this._data.Count <= this.MaxCapacity)
          return;
        int local_2 = this.MaxCapacity - this.MinCapacity + 1;
        Dictionary<TKey, LRUCache<TKey, TValue>.CacheEntry>.Enumerator local_3 = this._data.GetEnumerator();
        BinaryHeapULong<KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry>> local_4 = new BinaryHeapULong<KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry>>((uint) (local_2 + 1));
        while (local_4.Count < local_2 && local_3.MoveNext())
        {
          KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry> local_6 = local_3.Current;
          local_4.Push(local_6, ulong.MaxValue - local_6.Value.Id);
        }
        ulong local_5 = local_4.PeekWeight();
        while (local_3.MoveNext())
        {
          KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry> local_7 = local_3.Current;
          if (local_5 < ulong.MaxValue - local_7.Value.Id)
          {
            local_4.Push(local_7, ulong.MaxValue - local_7.Value.Id);
            local_4.Pop();
            local_5 = local_4.PeekWeight();
          }
        }
        while (local_4.Count > 0)
        {
          KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry> local_8 = local_4.Pop();
          if (this.OnRemove != null)
            this.OnRemove(local_8.Value.Value);
          this._data.Remove(local_8.Key);
          this._lastId = this._lastId + 1UL;
        }
      }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return this._data.Select<KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry>, KeyValuePair<TKey, TValue>>((Func<KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry>, KeyValuePair<TKey, TValue>>) (source => new KeyValuePair<TKey, TValue>(source.Key, source.Value.Value))).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._data.Select<KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry>, KeyValuePair<TKey, TValue>>((Func<KeyValuePair<TKey, LRUCache<TKey, TValue>.CacheEntry>, KeyValuePair<TKey, TValue>>) (source => new KeyValuePair<TKey, TValue>(source.Key, source.Value.Value))).GetEnumerator();
    }

    public delegate void OnRemoveDelegate(TValue item);

    private class CacheEntry
    {
      public ulong Id { get; set; }

      public TValue Value { get; set; }
    }
  }
}
