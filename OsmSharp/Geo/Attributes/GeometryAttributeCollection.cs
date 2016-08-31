using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Geo.Attributes
{
  public abstract class GeometryAttributeCollection : IEnumerable<GeometryAttribute>, IEnumerable
  {
    public abstract int Count { get; }

    public virtual object this[string key]
    {
      get
      {
        object obj;
        if (this.TryGetValue(key, out obj))
          return obj;
        throw new KeyNotFoundException();
      }
      set
      {
        this.AddOrReplace(key, value);
      }
    }

    public abstract void Add(string key, object value);

    public abstract void Add(GeometryAttribute attribute);

    public void Add(GeometryAttributeCollection attributeCollection)
    {
      foreach (GeometryAttribute attribute in attributeCollection)
        this.Add(attribute);
    }

    public void AddOrReplace(GeometryAttributeCollection attributeCollection)
    {
      foreach (GeometryAttribute attribute in attributeCollection)
        this.AddOrReplace(attribute);
    }

    public abstract void AddOrReplace(string key, object value);

    public abstract void AddOrReplace(GeometryAttribute attribute);

    public abstract bool ContainsKey(string key);

    public abstract bool TryGetValue(string key, out object value);

    public abstract bool ContainsKeyValue(string key, object value);

    public abstract void Clear();

    public abstract IEnumerator<GeometryAttribute> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
