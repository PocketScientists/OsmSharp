using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using System;

namespace OsmSharp.Geo.Geometries
{
  public class Point : Geometry
  {
    public GeoCoordinate Coordinate { get; set; }

    public override GeoCoordinateBox Box
    {
      get
      {
        return new GeoCoordinateBox(this.Coordinate, this.Coordinate);
      }
    }

    public Point(GeoCoordinate coordinate)
    {
      this.Coordinate = coordinate;
    }

    public override bool IsInside(GeoCoordinateBox box)
    {
      if (box == null)
        throw new ArgumentNullException();
      return box.Contains((PointF2D) this.Coordinate);
    }
  }
}
