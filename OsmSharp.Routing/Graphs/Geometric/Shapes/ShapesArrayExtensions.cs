using OsmSharp.Geo;
using Reminiscence.Arrays;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graphs.Geometric.Shapes
{
  public static class ShapesArrayExtensions
  {
    public static void Set(this ShapesArray index, long id, IEnumerable<ICoordinate> shape)
    {
      ((ArrayBase<ShapeBase>) index)[id] = (ShapeBase) new ShapeEnumerable(shape);
    }

    public static void Set(this ShapesArray index, long id, params ICoordinate[] shape)
    {
      ((ArrayBase<ShapeBase>) index)[id] = (ShapeBase) new ShapeEnumerable((IEnumerable<ICoordinate>) shape);
    }
  }
}
