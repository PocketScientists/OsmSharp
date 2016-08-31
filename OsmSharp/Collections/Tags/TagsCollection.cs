using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections.Tags
{
  public class TagsCollection : TagsCollectionBase
  {
    private readonly List<Tag> _tags;

    public override int Count
    {
      get
      {
        return this._tags.Count;
      }
    }

    public override bool IsReadonly
    {
      get
      {
        return false;
      }
    }

    public TagsCollection()
    {
      this._tags = new List<Tag>();
    }

    public TagsCollection(int capacity)
    {
      this._tags = new List<Tag>(capacity);
    }

    public TagsCollection(params Tag[] tags)
    {
      this._tags = new List<Tag>();
      if (tags == null)
        return;
      this._tags.AddRange((IEnumerable<Tag>) tags);
    }

    public TagsCollection(IEnumerable<Tag> tags)
    {
      this._tags = new List<Tag>();
      if (tags == null)
        return;
      this._tags.AddRange(tags);
    }

    public TagsCollection(IDictionary<string, string> tags)
    {
      this._tags = new List<Tag>();
      if (tags == null)
        return;
      foreach (KeyValuePair<string, string> tag in (IEnumerable<KeyValuePair<string, string>>) tags)
        this._tags.Add(new Tag(tag.Key, tag.Value));
    }

    public override void Add(string key, string value)
    {
      this._tags.Add(new Tag()
      {
        Key = key,
        Value = value
      });
    }

    public override void Add(Tag tag)
    {
      this._tags.Add(tag);
    }

    public override void AddOrReplace(string key, string value)
    {
      for (int index = 0; index < this._tags.Count; ++index)
      {
        Tag tag = this._tags[index];
        if (tag.Key == key)
        {
          tag.Value = value;
          this._tags[index] = tag;
          return;
        }
      }
      this.Add(key, value);
    }

    public override void AddOrReplace(Tag tag)
    {
      this.AddOrReplace(tag.Key, tag.Value);
    }

    public override bool ContainsKey(string key)
    {
      return this._tags.Any<Tag>((Func<Tag, bool>) (tag => tag.Key == key));
    }

    public override bool TryGetValue(string key, out string value)
    {
      foreach (Tag tag in this._tags)
      {
        if (tag.Key == key)
        {
          value = tag.Value;
          return true;
        }
      }
      value = string.Empty;
      return false;
    }

    public override bool ContainsKeyValue(string key, string value)
    {
      return this._tags.Any<Tag>((Func<Tag, bool>) (tag =>
      {
        if (tag.Key == key)
          return tag.Value == value;
        return false;
      }));
    }

    public override void Clear()
    {
      this._tags.Clear();
    }

    public override IEnumerator<Tag> GetEnumerator()
    {
      return (IEnumerator<Tag>) this._tags.GetEnumerator();
    }

    public override bool RemoveKey(string key)
    {
      return this._tags.RemoveAll((Predicate<Tag>) (tag => tag.Key == key)) > 0;
    }

    public override bool RemoveKeyValue(string key, string value)
    {
      return this._tags.RemoveAll((Predicate<Tag>) (tag =>
      {
        if (tag.Key == key)
          return tag.Value == value;
        return false;
      })) > 0;
    }

    public override void RemoveAll(Predicate<Tag> predicate)
    {
      this._tags.RemoveAll(predicate);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Tag tag in (TagsCollectionBase) this)
      {
        stringBuilder.Append(tag.ToString());
        stringBuilder.Append(',');
      }
      if (stringBuilder.Length > 0)
        return stringBuilder.ToString(0, stringBuilder.Length - 1);
      return "empty";
    }
  }
}
