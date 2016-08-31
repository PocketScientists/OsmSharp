using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections.Tags
{
  public class TagsTableCollection : TagsCollectionBase
  {
    private readonly List<uint> _tags;
    private readonly ObjectTable<Tag> _tagsTable;

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

    public TagsTableCollection(ObjectTable<Tag> tagsTable)
    {
      this._tags = new List<uint>();
    }

    public TagsTableCollection(ObjectTable<Tag> tagsTable, params Tag[] tags)
    {
      this._tagsTable = tagsTable;
      this._tags = new List<uint>();
      foreach (Tag tag in tags)
        this._tags.Add(this._tagsTable.Add(tag));
    }

    public TagsTableCollection(ObjectTable<Tag> tagsTable, IEnumerable<Tag> tags)
    {
      this._tagsTable = tagsTable;
      this._tags = new List<uint>();
      foreach (Tag tag in tags)
        this._tags.Add(this._tagsTable.Add(tag));
    }

    public override void Add(string key, string value)
    {
      this.Add(new Tag()
      {
        Key = key,
        Value = value
      });
    }

    public override void Add(Tag tag)
    {
      this._tags.Add(this._tagsTable.Add(tag));
    }

    public override void AddOrReplace(string key, string value)
    {
      for (int index = 0; index < this._tags.Count; ++index)
      {
        Tag tag = this._tagsTable.Get(this._tags[index]);
        if (tag.Key == key)
        {
          tag.Value = value;
          this._tags[index] = this._tagsTable.Add(tag);
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
      return this._tags.Any<uint>((Func<uint, bool>) (tagId => this._tagsTable.Get(tagId).Key == key));
    }

    public override bool TryGetValue(string key, out string value)
    {
      foreach (uint tag1 in this._tags)
      {
        Tag tag2 = this._tagsTable.Get(tag1);
        if (tag2.Key == key)
        {
          value = tag2.Value;
          return true;
        }
      }
      value = string.Empty;
      return false;
    }

    public override bool ContainsKeyValue(string key, string value)
    {
      return this._tags.Any<uint>((Func<uint, bool>) (tagId =>
      {
        Tag tag = this._tagsTable.Get(tagId);
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
      return this._tags.Select<uint, Tag>((Func<uint, Tag>) (tagId => this._tagsTable.Get(tagId))).GetEnumerator();
    }

    public override bool RemoveKey(string key)
    {
      return this._tags.RemoveAll((Predicate<uint>) (tagId => this._tagsTable.Get(tagId).Key == key)) > 0;
    }

    public override bool RemoveKeyValue(string key, string value)
    {
      return this._tags.RemoveAll((Predicate<uint>) (tagId =>
      {
        Tag tag = this._tagsTable.Get(tagId);
        if (tag.Key == key)
          return tag.Value == value;
        return false;
      })) > 0;
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

    public override void RemoveAll(Predicate<Tag> predicate)
    {
      this._tags.RemoveAll((Predicate<uint>) (x => predicate(this._tagsTable.Get(x))));
    }
  }
}
