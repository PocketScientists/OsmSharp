using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Geo.Attributes
{
  public class SimpleGeometryAttributeCollection : GeometryAttributeCollection
  {
    private readonly List<GeometryAttribute> _attributes;

    public override int Count
    {
      get
      {
        return this._attributes.Count;
      }
    }

    public SimpleGeometryAttributeCollection()
    {
      this._attributes = new List<GeometryAttribute>();
    }

    public SimpleGeometryAttributeCollection(IEnumerable<GeometryAttribute> attributes)
    {
      this._attributes = new List<GeometryAttribute>();
      this._attributes.AddRange(attributes);
    }

    public SimpleGeometryAttributeCollection(IEnumerable<Tag> tags)
    {
      this._attributes = new List<GeometryAttribute>();
      if (tags == null)
        return;
      foreach (Tag tag in tags)
        this._attributes.Add(new GeometryAttribute()
        {
          Key = tag.Key,
          Value = (object) tag.Value
        });
    }

    public override void Add(string key, object value)
    {
      this._attributes.Add(new GeometryAttribute()
      {
        Key = key,
        Value = value
      });
    }

    public override void Add(GeometryAttribute attribute)
    {
      this._attributes.Add(attribute);
    }

    public override void AddOrReplace(string key, object value)
    {
      for (int index = 0; index < this._attributes.Count; ++index)
      {
        GeometryAttribute attribute = this._attributes[index];
        if (attribute.Key == key)
        {
          attribute.Value = value;
          this._attributes[index] = attribute;
          return;
        }
      }
      this.Add(key, value);
    }

    public override void AddOrReplace(GeometryAttribute tag)
    {
      this.AddOrReplace(tag.Key, tag.Value);
    }

    public override bool ContainsKey(string key)
    {
      return this.Any<GeometryAttribute>((Func<GeometryAttribute, bool>) (tag => tag.Key == key));
    }

    public override bool TryGetValue(string key, out object value)
    {
      using (IEnumerator<GeometryAttribute> enumerator = this.Where<GeometryAttribute>((Func<GeometryAttribute, bool>) (tag => tag.Key == key)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          GeometryAttribute current = enumerator.Current;
          value = current.Value;
          return true;
        }
      }
      value = (object) string.Empty;
      return false;
    }

    public override bool ContainsKeyValue(string key, object value)
    {
      return this.Any<GeometryAttribute>((Func<GeometryAttribute, bool>) (tag =>
      {
        if (!tag.Key.Equals(key))
          return false;
        if (tag.Value == null)
          return value == null;
        return tag.Value.Equals(value);
      }));
    }

    public override void Clear()
    {
      this._attributes.Clear();
    }

    public override IEnumerator<GeometryAttribute> GetEnumerator()
    {
      return (IEnumerator<GeometryAttribute>) this._attributes.GetEnumerator();
    }
  }
}
