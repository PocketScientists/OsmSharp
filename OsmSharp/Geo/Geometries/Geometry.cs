using OsmSharp.Math.Geo;

namespace OsmSharp.Geo.Geometries
{
  public abstract class Geometry
  {
    public abstract GeoCoordinateBox Box { get; }

    public abstract bool IsInside(GeoCoordinateBox box);
  }
}
