using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections.Tags
{
  public class StringTableTagsCollection : TagsCollectionBase
  {
    private readonly List<StringTableTagsCollection.TagEncoded> _tagsList;
    private readonly ObjectTable<string> _stringTable;

    public override int Count
    {
      get
      {
        return this._tagsList.Count;
      }
    }

    public override bool IsReadonly
    {
      get
      {
        return false;
      }
    }

    public StringTableTagsCollection(ObjectTable<string> stringTable)
    {
      this._stringTable = stringTable;
      this._tagsList = new List<StringTableTagsCollection.TagEncoded>();
    }

    public override void Add(string key, string value)
    {
      this._tagsList.Add(new StringTableTagsCollection.TagEncoded()
      {
        Key = this._stringTable.Add(key),
        Value = this._stringTable.Add(value)
      });
    }

    public override void Add(Tag tag)
    {
      this.Add(tag.Key, tag.Value);
    }

    public override void AddOrReplace(string key, string value)
    {
      uint num1 = this._stringTable.Add(key);
      uint num2 = this._stringTable.Add(value);
      for (int index = 0; index < this._tagsList.Count; ++index)
      {
        StringTableTagsCollection.TagEncoded tags = this._tagsList[index];
        if ((int) tags.Key == (int) num1)
        {
          tags.Value = num2;
          this._tagsList[index] = tags;
          return;
        }
      }
      this._tagsList.Add(new StringTableTagsCollection.TagEncoded()
      {
        Key = num1,
        Value = num2
      });
    }

    public override void AddOrReplace(Tag tag)
    {
      this.AddOrReplace(tag.Key, tag.Value);
    }

    public override bool ContainsKey(string key)
    {
      uint keyInt = this._stringTable.Add(key);
      return this._tagsList.Any<StringTableTagsCollection.TagEncoded>((Func<StringTableTagsCollection.TagEncoded, bool>) (tag => (int) tag.Key == (int) keyInt));
    }

    public override bool TryGetValue(string key, out string value)
    {
      uint keyInt = this._stringTable.Add(key);
      using (IEnumerator<StringTableTagsCollection.TagEncoded> enumerator = this._tagsList.Where<StringTableTagsCollection.TagEncoded>((Func<StringTableTagsCollection.TagEncoded, bool>) (tagEncoded => (int) tagEncoded.Key == (int) keyInt)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          StringTableTagsCollection.TagEncoded current = enumerator.Current;
          value = this._stringTable.Get(current.Value);
          return true;
        }
      }
      value = (string) null;
      return false;
    }

    public override bool ContainsKeyValue(string key, string value)
    {
      uint keyInt = this._stringTable.Add(key);
      uint valueInt = this._stringTable.Add(value);
      return this._tagsList.Any<StringTableTagsCollection.TagEncoded>((Func<StringTableTagsCollection.TagEncoded, bool>) (tagEncoded =>
      {
        if ((int) tagEncoded.Key == (int) keyInt)
          return (int) tagEncoded.Value == (int) valueInt;
        return false;
      }));
    }

    public override IEnumerator<Tag> GetEnumerator()
    {
      foreach (StringTableTagsCollection.TagEncoded tags in this._tagsList)
        yield return new Tag()
        {
          Key = this._stringTable.Get(tags.Key),
          Value = this._stringTable.Get(tags.Value)
        };
      List<StringTableTagsCollection.TagEncoded>.Enumerator enumerator = new List<StringTableTagsCollection.TagEncoded>.Enumerator();
    }

    public override void Clear()
    {
      this._tagsList.Clear();
    }

    public override bool RemoveKey(string key)
    {
      uint keyInt = this._stringTable.Add(key);
      return this._tagsList.RemoveAll((Predicate<StringTableTagsCollection.TagEncoded>) (tagEncoded => (int) tagEncoded.Key == (int) keyInt)) > 0;
    }

    public override bool RemoveKeyValue(string key, string value)
    {
      uint keyInt = this._stringTable.Add(key);
      uint valueInt = this._stringTable.Add(value);
      return this._tagsList.RemoveAll((Predicate<StringTableTagsCollection.TagEncoded>) (tagEncoded =>
      {
        if ((int) tagEncoded.Key == (int) keyInt)
          return (int) tagEncoded.Value == (int) valueInt;
        return false;
      })) > 0;
    }

    public override void RemoveAll(Predicate<Tag> predicate)
    {
      this._tagsList.RemoveAll((Predicate<StringTableTagsCollection.TagEncoded>) (x => predicate(new Tag(this._stringTable.Get(x.Key), this._stringTable.Get(x.Value)))));
    }

    private struct TagEncoded
    {
      public uint Key { get; set; }

      public uint Value { get; set; }
    }
  }
}
