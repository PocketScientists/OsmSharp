using OsmSharp.Math.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections.SpatialIndexes
{
  public interface ISpatialIndex<T> : ISpatialIndexReadonly<T>, IEnumerable<T>, IEnumerable
  {
    void Add(BoxF2D box, T item);

    void Remove(T item);

    void Remove(BoxF2D box, T item);
  }
}
