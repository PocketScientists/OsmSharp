using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using Reminiscence.Arrays;
using Reminiscence.Collections;
using Reminiscence.Indexes;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Attributes
{
  public class AttributesIndex
  {
    private readonly Index<string> _stringIndex;
    private readonly Index<int[]> _collectionIndex;
    private readonly ArrayBase<uint> _index;
    private readonly bool _isReadonly;
    private readonly AttributesIndexMode _mode;
    private const uint NULL_ATTRIBUTES = 0;
    private const uint EMPTY_ATTRIBUTES = 1;
    private readonly IDictionary<string, int> _stringReverseIndex;
    private readonly IDictionary<int[], uint> _collectionReverseIndex;
    private uint _nextId;

    public bool IsReadonly
    {
      get
      {
        return this._isReadonly;
      }
    }

    public bool CheckDuplicates
    {
      get
      {
        return this._stringReverseIndex != null;
      }
    }

    public uint Count
    {
      get
      {
        if ((this._mode & AttributesIndexMode.IncreaseOne) == AttributesIndexMode.IncreaseOne || (this._mode & AttributesIndexMode.None) == AttributesIndexMode.None && this._index != null)
          return this._nextId + 2U;
        throw new Exception("Count cannot be calculated on a index that doesn't use 'IncreaseOne' mode.");
      }
    }

    public AttributesIndexMode IndexMode
    {
      get
      {
        return this._mode;
      }
    }

    public AttributesIndex(AttributesIndexMode mode = AttributesIndexMode.ReverseAll)
    {
      this._stringIndex = new Index<string>();
      this._collectionIndex = new Index<int[]>();
      this._isReadonly = false;
      this._mode = mode;
      this._stringReverseIndex = (IDictionary<string, int>) null;
      this._collectionReverseIndex = (IDictionary<int[], uint>) null;
      if ((this._mode & AttributesIndexMode.IncreaseOne) == AttributesIndexMode.IncreaseOne)
      {
        this._index = (ArrayBase<uint>) new MemoryArray<uint>(1024L);
        this._nextId = 0U;
      }
      if ((this._mode & AttributesIndexMode.ReverseStringIndex) == AttributesIndexMode.ReverseStringIndex || (this._mode & AttributesIndexMode.ReverseStringIndexKeysOnly) == AttributesIndexMode.ReverseStringIndexKeysOnly)
        this._stringReverseIndex = (IDictionary<string, int>) new Reminiscence.Collections.Dictionary<string, int>();
      if ((this._mode & AttributesIndexMode.ReverseCollectionIndex) != AttributesIndexMode.ReverseCollectionIndex)
        return;
      DelegateEqualityComparer<int[]>.GetHashCodeDelegate hashCodeDelegate1 = (DelegateEqualityComparer<int[]>.GetHashCodeDelegate) (obj =>
      {
        int hashCode = obj.Length.GetHashCode();
        for (int index = 0; index < obj.Length; ++index)
          hashCode ^= obj[index].GetHashCode();
        return hashCode;
      });
      this._collectionReverseIndex = (IDictionary<int[], uint>) new Reminiscence.Collections.Dictionary<int[], uint>((IEqualityComparer<int[]>) new DelegateEqualityComparer<int[]>(hashCodeDelegate1, (DelegateEqualityComparer<int[]>.EqualsDelegate) ((x, y) =>
      {
        if (x.Length != y.Length)
          return false;
        for (int index = 0; index < x.Length; ++index)
        {
          if (x[index] != y[index])
            return false;
        }
        return true;
      })));
    }

    public AttributesIndex(MemoryMap map, AttributesIndexMode mode = AttributesIndexMode.ReverseAll)
    {
      if (mode == AttributesIndexMode.None)
        throw new ArgumentException("Cannot create a new index without a valid operating mode.");
      this._stringIndex = new Index<string>(map);
      this._collectionIndex = new Index<int[]>(map);
      this._isReadonly = false;
      this._mode = mode;
      this._stringReverseIndex = (IDictionary<string, int>) null;
      this._collectionReverseIndex = (IDictionary<int[], uint>) null;
      if ((this._mode & AttributesIndexMode.IncreaseOne) == AttributesIndexMode.IncreaseOne)
      {
        this._index = (ArrayBase<uint>) new Array<uint>(map, 1024L);
        this._nextId = 0U;
      }
      if ((this._mode & AttributesIndexMode.ReverseStringIndex) == AttributesIndexMode.ReverseStringIndex || (this._mode & AttributesIndexMode.ReverseStringIndexKeysOnly) == AttributesIndexMode.ReverseStringIndexKeysOnly)
        this._stringReverseIndex = (IDictionary<string, int>) new Reminiscence.Collections.Dictionary<string, int>(map, 16384);
      if ((this._mode & AttributesIndexMode.ReverseCollectionIndex) != AttributesIndexMode.ReverseCollectionIndex)
        return;
      MemoryMap memoryMap = map;
      int num = 16384;
      DelegateEqualityComparer<int[]>.GetHashCodeDelegate hashCodeDelegate1 = (DelegateEqualityComparer<int[]>.GetHashCodeDelegate) (obj =>
      {
        int hashCode = obj.Length.GetHashCode();
        for (int index = 0; index < obj.Length; ++index)
          hashCode ^= obj[index].GetHashCode();
        return hashCode;
      });
      DelegateEqualityComparer<int[]> equalityComparer = new DelegateEqualityComparer<int[]>(hashCodeDelegate1, (DelegateEqualityComparer<int[]>.EqualsDelegate) ((x, y) =>
      {
        if (x.Length != y.Length)
          return false;
        for (int index = 0; index < x.Length; ++index)
        {
          if (x[index] != y[index])
            return false;
        }
        return true;
      }));
      this._collectionReverseIndex = (IDictionary<int[], uint>) new Reminiscence.Collections.Dictionary<int[], uint>(memoryMap, num, (IEqualityComparer<int[]>) equalityComparer);
    }

    internal AttributesIndex(Index<string> stringIndex, Index<int[]> tagsIndex)
    {
      this._stringIndex = stringIndex;
      this._collectionIndex = tagsIndex;
      this._isReadonly = true;
      this._index = (ArrayBase<uint>) null;
      this._nextId = uint.MaxValue;
      this._mode = AttributesIndexMode.None;
      this._stringReverseIndex = (IDictionary<string, int>) null;
      this._collectionReverseIndex = (IDictionary<int[], uint>) null;
    }

    internal AttributesIndex(Index<string> stringIndex, Index<int[]> tagsIndex, ArrayBase<uint> index)
    {
      this._stringIndex = stringIndex;
      this._collectionIndex = tagsIndex;
      this._isReadonly = true;
      this._index = index;
      this._nextId = (uint) index.Length;
      this._mode = AttributesIndexMode.None;
      this._stringReverseIndex = (IDictionary<string, int>) null;
      this._collectionReverseIndex = (IDictionary<int[], uint>) null;
    }

    public TagsCollectionBase Get(uint tagsId)
    {
      if ((int) tagsId == 0)
        return (TagsCollectionBase) null;
      if ((int) tagsId == 1)
        return (TagsCollectionBase) new TagsCollection();
      if (this._index == null)
        return (TagsCollectionBase) new AttributesIndex.InternalTagsCollection(this._stringIndex, this._collectionIndex.Get((long) (tagsId - 2U)));
      tagsId = this._index[(long) (tagsId - 2U)];
      return (TagsCollectionBase) new AttributesIndex.InternalTagsCollection(this._stringIndex, this._collectionIndex.Get((long) tagsId));
    }

    public uint Add(TagsCollectionBase tags)
    {
      if (tags == null)
        return 0;
      if (tags.Count == 0)
        return 1;
      if (this._isReadonly)
        throw new InvalidOperationException("This tags index is readonly. Check IsReadonly.");
            System.Collections.Generic.SortedSet<long> sortedSet = new System.Collections.Generic.SortedSet<long>();
      foreach (Tag tag in tags)
        sortedSet.Add((long) this.AddString(tag.Key, true) + (long) int.MaxValue * (long) this.AddString(tag.Value, false));
      int[] collection = new int[sortedSet.Count * 2];
      int index1 = 0;
      foreach (long num in sortedSet)
      {
        collection[index1] = (int) (num % (long) int.MaxValue);
        int index2 = index1 + 1;
        collection[index2] = (int) (num / (long) int.MaxValue);
        index1 = index2 + 1;
      }
      return this.AddCollection(collection);
    }

    private int AddString(string value, bool key)
    {
      if ((this._mode & AttributesIndexMode.ReverseStringIndex) != AttributesIndexMode.ReverseStringIndex && !((this._mode & AttributesIndexMode.ReverseStringIndexKeysOnly) == AttributesIndexMode.ReverseStringIndexKeysOnly & key))
        return (int) this._stringIndex.Add(value);
      int num;
      if (!this._stringReverseIndex.TryGetValue(value, out num))
      {
        num = (int) this._stringIndex.Add(value);
        this._stringReverseIndex.Add(value, num);
      }
      return num;
    }

    private uint AddCollection(int[] collection)
    {
      uint num;
      if (this._collectionReverseIndex != null && this._collectionReverseIndex.TryGetValue(collection, out num))
        return num + 2U;
      num = (uint) this._collectionIndex.Add(collection);
      if (this._index != null)
      {
        if ((long) this._nextId >= this._index.Length)
          this._index.Resize(this._index.Length + 1024L);
        this._index[(long) this._nextId] = num;
        num = this._nextId;
        this._nextId = this._nextId + 1U;
      }
      if (this._collectionReverseIndex != null)
        this._collectionReverseIndex.Add(collection, num);
      return num + 2U;
    }

    public long Serialize(Stream stream)
    {
      if (this._index == null)
      {
        stream.WriteByte((byte) 0);
        long withSize = this._collectionIndex.CopyToWithSize(stream);
        return this._stringIndex.CopyToWithSize(stream) + withSize + 1L;
      }
      this._index.Resize((long) this._nextId);
      stream.WriteByte((byte) 1);
      long num1 = this._collectionIndex.CopyToWithSize(stream) + this._stringIndex.CopyToWithSize(stream);
      stream.Write(BitConverter.GetBytes(this._index.Length), 0, 8);
      long num2 = this._index.CopyTo(stream);
      return num1 + num2 + 8L + 1L;
    }

    public static AttributesIndex Deserialize(Stream stream, bool copy = false)
    {
      if (stream.ReadByte() == 0)
      {
        long num;
        Index<int[]> fromWithSize1 = Index<int[]>.CreateFromWithSize(stream, out num, !copy);
        long offset1 = num + 8L + 1L;
        stream.Seek(offset1, SeekOrigin.Begin);
        Index<string> fromWithSize2 = Index<string>.CreateFromWithSize((Stream) new LimitedStream(stream), out num, !copy);
        long offset2 = offset1 + (num + 8L);
        stream.Seek(offset2, SeekOrigin.Begin);
        Index<int[]> tagsIndex = fromWithSize1;
        return new AttributesIndex(fromWithSize2, tagsIndex);
      }
      long num1;
      Index<int[]> fromWithSize3 = Index<int[]>.CreateFromWithSize(stream, out num1, !copy);
      long offset3 = num1 + 8L + 1L;
      stream.Seek(offset3, SeekOrigin.Begin);
      Index<string> fromWithSize4 = Index<string>.CreateFromWithSize((Stream) new LimitedStream(stream), out num1, !copy);
      long offset4 = offset3 + (num1 + 8L);
      stream.Seek(offset4, SeekOrigin.Begin);
      byte[] buffer = new byte[8];
      stream.Read(buffer, 0, 8);
      MemoryArray<uint> memoryArray1 = new MemoryArray<uint>(BitConverter.ToInt64(buffer, 0));
      ((ArrayBase<uint>) memoryArray1).CopyFrom(stream);
      Index<int[]> tagsIndex1 = fromWithSize3;
      MemoryArray<uint> memoryArray2 = memoryArray1;
      return new AttributesIndex(fromWithSize4, tagsIndex1, (ArrayBase<uint>) memoryArray2);
    }

    private class InternalTagsCollection : TagsCollectionBase
    {
      private Index<string> _stringIndex;
      private int[] _tags;

      public override int Count
      {
        get
        {
          return this._tags.Length / 2;
        }
      }

      public override bool IsReadonly
      {
        get
        {
          return true;
        }
      }

      public InternalTagsCollection(Index<string> stringIndex, int[] tags)
      {
        this._stringIndex = stringIndex;
        this._tags = tags;
      }

      public override void Add(string key, string value)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override void Add(Tag tag)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override void AddOrReplace(string key, string value)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override void AddOrReplace(Tag tag)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override bool ContainsKey(string key)
      {
        int index = 0;
        while (index < this._tags.Length)
        {
          if (key == this._stringIndex.Get((long) this._tags[index]))
            return true;
          index += 2;
        }
        return false;
      }

      public override bool TryGetValue(string key, out string value)
      {
        int index = 0;
        while (index < this._tags.Length)
        {
          if (key == this._stringIndex.Get((long) this._tags[index]))
          {
            value = this._stringIndex.Get((long) this._tags[index + 1]);
            return true;
          }
          index += 2;
        }
        value = (string) null;
        return false;
      }

      public override bool ContainsKeyValue(string key, string value)
      {
        int index = 0;
        while (index < this._tags.Length)
        {
          if (key == this._stringIndex.Get((long) this._tags[index]) && value == this._stringIndex.Get((long) this._tags[index + 1]))
            return true;
          index += 2;
        }
        return false;
      }

      public override bool RemoveKey(string key)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override bool RemoveKeyValue(string key, string value)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override void Clear()
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override void RemoveAll(Predicate<Tag> predicate)
      {
        throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
      }

      public override IEnumerator<Tag> GetEnumerator()
      {
        return (IEnumerator<Tag>) new AttributesIndex.InternalTagsEnumerator(this._stringIndex, this._tags);
      }
    }

    private class InternalTagsEnumerator : IEnumerator<Tag>, IEnumerator, IDisposable
    {
      private int _idx = -2;
      private Index<string> _stringIndex;
      private int[] _tags;

      public Tag Current
      {
        get
        {
          return new Tag()
          {
            Key = this._stringIndex.Get((long) this._tags[this._idx]),
            Value = this._stringIndex.Get((long) this._tags[this._idx + 1])
          };
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) new Tag()
          {
            Key = this._stringIndex.Get((long) this._tags[this._idx]),
            Value = this._stringIndex.Get((long) this._tags[this._idx + 1])
          };
        }
      }

      public InternalTagsEnumerator(Index<string> stringIndex, int[] tags)
      {
        this._stringIndex = stringIndex;
        this._tags = tags;
      }

      public bool MoveNext()
      {
        this._idx = this._idx + 2;
        return this._idx < this._tags.Length;
      }

      public void Reset()
      {
        this._idx = -2;
      }

      public void Dispose()
      {
        this._tags = (int[]) null;
        this._stringIndex = (Index<string>) null;
      }
    }
  }
}
