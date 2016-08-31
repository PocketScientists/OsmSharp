using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class LineString : Geometry
  {
    public List<GeoCoordinate> Coordinates { get; private set; }

    public override GeoCoordinateBox Box
    {
      get
      {
        return new GeoCoordinateBox(this.Coordinates.ToArray());
      }
    }

    public LineString()
    {
      this.Coordinates = new List<GeoCoordinate>();
    }

    public LineString(IEnumerable<GeoCoordinate> coordinates)
    {
      this.Coordinates = new List<GeoCoordinate>(coordinates);
    }

    public LineString(IList<ICoordinate> coordinates)
    {
      this.Coordinates = new List<GeoCoordinate>(coordinates.Count);
      for (int index = 0; index < coordinates.Count; ++index)
        this.Coordinates.Add(new GeoCoordinate(coordinates[index]));
    }

    public LineString(params GeoCoordinate[] coordinates)
    {
      this.Coordinates = new List<GeoCoordinate>((IEnumerable<GeoCoordinate>) coordinates);
    }

    public override bool IsInside(GeoCoordinateBox box)
    {
      for (int index = 0; index < this.Coordinates.Count - 1; ++index)
      {
        if (box.IntersectsPotentially((PointF2D) this.Coordinates[index], (PointF2D) this.Coordinates[index + 1]) && box.Intersects((PointF2D) this.Coordinates[index], (PointF2D) this.Coordinates[index + 1]))
          return true;
      }
      return false;
    }
  }
}
