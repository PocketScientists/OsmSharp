using OsmSharp.Math.Primitives;
using System.Collections.Generic;

namespace OsmSharp.Math.Structures
{
  public interface ILocatedObjectIndex<TPointType, TDataType> where TPointType : PointF2D
  {
    IEnumerable<TDataType> GetInside(BoxF2D box);

    void Add(TPointType location, TDataType data);

    void Clear();
  }
}
