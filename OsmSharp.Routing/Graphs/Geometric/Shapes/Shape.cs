using OsmSharp.Geo;
using OsmSharp.Math.Geo.Simple;
using Reminiscence.Arrays;

namespace OsmSharp.Routing.Graphs.Geometric.Shapes
{
  public class Shape : ShapeBase
  {
    private readonly ArrayBase<float> _coordinates;
    private readonly long _pointer;
    private readonly int _size;
    private readonly bool _reversed;

    public override int Count
    {
      get
      {
        return this._size;
      }
    }

    public override ICoordinate this[int i]
    {
      get
      {
        if (this._reversed)
          return (ICoordinate) new GeoCoordinateSimple()
          {
            Latitude = this._coordinates[this._pointer + (long) ((this._size - 1) * 2) - (long) (i * 2)],
            Longitude = this._coordinates[this._pointer + (long) ((this._size - 1) * 2) - (long) (i * 2) + 1L]
          };
        return (ICoordinate) new GeoCoordinateSimple()
        {
          Latitude = this._coordinates[this._pointer + (long) (i * 2)],
          Longitude = this._coordinates[this._pointer + (long) (i * 2) + 1L]
        };
      }
    }

    internal Shape(ArrayBase<float> coordinates, long pointer, int size)
    {
      this._coordinates = coordinates;
      this._pointer = pointer;
      this._size = size;
      this._reversed = false;
    }

    internal Shape(ArrayBase<float> coordinates, long pointer, int size, bool reversed)
    {
      this._coordinates = coordinates;
      this._pointer = pointer;
      this._size = size;
      this._reversed = reversed;
    }

    public override ShapeBase Reverse()
    {
      return (ShapeBase) new Shape(this._coordinates, this._pointer, this._size, !this._reversed);
    }
  }
}
