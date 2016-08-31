using OsmSharp.Math.Primitives;
using System.Collections.Generic;

namespace OsmSharp.Collections.SpatialIndexes
{
  public interface ISpatialIndexReadonly<T>
  {
    IEnumerable<T> Get(BoxF2D box);

    void GetCancel();
  }
}
