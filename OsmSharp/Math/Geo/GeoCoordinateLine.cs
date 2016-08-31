using OsmSharp.Math.Primitives;
using OsmSharp.Units.Distance;

namespace OsmSharp.Math.Geo
{
  public class GeoCoordinateLine : LineF2D
  {
    public Meter LengthReal
    {
      get
      {
        return new GeoCoordinate(this.Point1).DistanceReal(new GeoCoordinate(this.Point2));
      }
    }

    public GeoCoordinateLine(GeoCoordinate point1, GeoCoordinate point2)
      : base((PointF2D) point1, (PointF2D) point2)
    {
    }

    public GeoCoordinateLine(GeoCoordinate point1, GeoCoordinate point2, bool is_segment1, bool is_segment2)
      : base((PointF2D) point1, (PointF2D) point2, is_segment1, is_segment2)
    {
    }

    public Meter DistanceReal(GeoCoordinate coordinate)
    {
      PointF2D point = this.ProjectOn((PointF2D) coordinate);
      if (point == (PointF2D) null)
        return (Meter) double.MaxValue;
      return new GeoCoordinate(point).DistanceReal(coordinate);
    }

    public override int GetHashCode()
    {
      return this.Point1.GetHashCode() ^ this.Point2.GetHashCode() ^ this.IsSegment1.GetHashCode() ^ this.IsSegment2.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      if (!(obj is GeoCoordinateLine) || !this.Point1.Equals((object) (obj as GeoCoordinateLine).Point1) || !this.Point2.Equals((object) (obj as GeoCoordinateLine).Point2))
        return false;
      bool flag = this.IsSegment1;
      if (!flag.Equals((obj as GeoCoordinateLine).IsSegment1))
        return false;
      flag = this.IsSegment2;
      return flag.Equals((obj as GeoCoordinateLine).IsSegment2);
    }
  }
}
