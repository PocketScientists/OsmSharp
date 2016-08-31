using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace OsmSharp.Collections.Tags
{
  public abstract class TagsCollectionBase : IEnumerable<Tag>, IEnumerable, IEnumerable<KeyValuePair<string, string>>, ITagsSource
  {
    public abstract int Count { get; }

    public abstract bool IsReadonly { get; }

    public virtual string this[string key]
    {
      get
      {
        string str;
        if (this.TryGetValue(key, out str))
          return str;
        throw new KeyNotFoundException();
      }
      set
      {
        this.AddOrReplace(key, value);
      }
    }

    public abstract void Add(string key, string value);

    public abstract void Add(Tag tag);

    public void Add(TagsCollectionBase tagsCollection)
    {
      foreach (Tag tags in tagsCollection)
        this.Add(tags);
    }

    public void AddOrReplace(TagsCollectionBase tagsCollection)
    {
      foreach (Tag tags in tagsCollection)
        this.AddOrReplace(tags);
    }

    public abstract void AddOrReplace(string key, string value);

    public abstract void AddOrReplace(Tag tag);

    public abstract bool ContainsKey(string key);

    public virtual bool ContainsOneOfKeys(ICollection<string> keys)
    {
      foreach (Tag tag in this)
      {
        if (keys.Contains(tag.Key))
          return true;
      }
      return false;
    }

    public abstract bool TryGetValue(string key, out string value);

    public abstract bool ContainsKeyValue(string key, string value);

    public bool Contains(Tag tag)
    {
      return this.ContainsKeyValue(tag.Key, tag.Value);
    }

    public double? GetNumericValue(string key)
    {
      string s;
      double result;
      if (this.TryGetValue(key, out s) && double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return new double?(result);
      return new double?();
    }

    public abstract bool RemoveKey(string key);

    public virtual bool RemoveKeyValue(Tag tag)
    {
      return this.RemoveKeyValue(tag.Key, tag.Value);
    }

    public abstract bool RemoveKeyValue(string key, string value);

    public abstract void Clear();

    public abstract void RemoveAll(Predicate<Tag> predicate);

    public virtual TagsCollectionBase KeepKeysOf(ICollection<string> keys)
    {
      TagsCollection tagsCollection = new TagsCollection(this.Count);
      foreach (Tag tag in this)
      {
        if (keys.Contains(tag.Key))
          tagsCollection.Add(tag);
      }
      return (TagsCollectionBase) tagsCollection;
    }

    public void Intersect(TagsCollectionBase tags)
    {
      List<Tag> tagList = new List<Tag>();
      foreach (Tag tag in this)
      {
        if (!tags.Contains(tag))
          tagList.Add(tag);
      }
      foreach (Tag tag in tagList)
        this.RemoveKeyValue(tag);
    }

    public Dictionary<string, object> ToStringObjectDictionary()
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (Tag tag in this)
        dictionary[tag.Key] = (object) tag.Value;
      return dictionary;
    }

    public Dictionary<string, string> ToStringStringDictionary()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (Tag tag in this)
        dictionary[tag.Key] = tag.Value;
      return dictionary;
    }

    public abstract IEnumerator<Tag> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (obj is TagsCollectionBase)
      {
        TagsCollectionBase tagsCollectionBase = obj as TagsCollectionBase;
        if (tagsCollectionBase.Count == this.Count)
        {
          foreach (Tag tag in this)
          {
            if (!tagsCollectionBase.Contains(tag))
              return false;
          }
          foreach (Tag tag in tagsCollectionBase)
          {
            if (!this.Contains(tag))
              return false;
          }
          return true;
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      int hashCode = this.Count.GetHashCode();
      foreach (Tag tag in this)
        hashCode ^= tag.GetHashCode();
      return hashCode;
    }

    private class KeyValuePairEnumerator : IEnumerator<KeyValuePair<string, string>>, IEnumerator, IDisposable
    {
      private IEnumerator<Tag> _tagEnumerator;

      public KeyValuePair<string, string> Current
      {
        get
        {
          Tag current = this._tagEnumerator.Current;
          string key = current.Key;
          current = this._tagEnumerator.Current;
          string str = current.Value;
          return new KeyValuePair<string, string>(key, str);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public KeyValuePairEnumerator(IEnumerator<Tag> tagEnumerator)
      {
        this._tagEnumerator = tagEnumerator;
      }

      public void Dispose()
      {
        this._tagEnumerator.Dispose();
      }

      public bool MoveNext()
      {
        return this._tagEnumerator.MoveNext();
      }

      public void Reset()
      {
        this._tagEnumerator.Reset();
      }
    }
  }
}
