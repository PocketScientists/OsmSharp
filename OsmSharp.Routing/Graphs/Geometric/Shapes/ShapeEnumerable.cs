using OsmSharp.Geo;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graphs.Geometric.Shapes
{
  public class ShapeEnumerable : ShapeBase
  {
    private readonly List<ICoordinate> _coordinates;
    private readonly bool _reversed;

    public override int Count
    {
      get
      {
        return this._coordinates.Count;
      }
    }

    public override ICoordinate this[int i]
    {
      get
      {
        if (this._reversed)
          return this._coordinates[this._coordinates.Count - i - 1];
        return this._coordinates[i];
      }
    }

    public ShapeEnumerable(IEnumerable<ICoordinate> coordinates)
    {
      this._coordinates = new List<ICoordinate>(coordinates);
      this._reversed = false;
    }

    public ShapeEnumerable(IEnumerable<ICoordinate> coordinates, bool reversed)
    {
      this._coordinates = new List<ICoordinate>(coordinates);
      this._reversed = reversed;
    }

    public override ShapeBase Reverse()
    {
      return (ShapeBase) new ShapeEnumerable((IEnumerable<ICoordinate>) this._coordinates, !this._reversed);
    }
  }
}
