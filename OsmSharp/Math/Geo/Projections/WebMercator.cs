using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Geo.Projections
{
  public class WebMercator : IProjection
  {
    private const int DefaultZoom = 15;

    public bool DirectionX
    {
      get
      {
        return true;
      }
    }

    public bool DirectionY
    {
      get
      {
        return false;
      }
    }

    public double[] ToPixel(double lat, double lon)
    {
      double num = System.Math.Floor(System.Math.Pow(2.0, 15.0));
      Radian radian = (Radian) new Degree(lat);
      return new double[2]
      {
        (lon + 180.0) / 360.0 * num,
        (1.0 - System.Math.Log(System.Math.Tan(radian.Value) + 1.0 / System.Math.Cos(radian.Value)) / System.Math.PI) / 2.0 * num
      };
    }

    public double[] ToPixel(GeoCoordinate coordinate)
    {
      return this.ToPixel(coordinate.Latitude, coordinate.Longitude);
    }

    public GeoCoordinate ToGeoCoordinates(double x, double y)
    {
      return new GeoCoordinate(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(System.Math.PI - 2.0 * System.Math.PI * y / System.Math.Pow(2.0, 15.0))), x / System.Math.Pow(2.0, 15.0) * 360.0 - 180.0);
    }

    public double LongitudeToX(double lon)
    {
      double num = System.Math.Floor(System.Math.Pow(2.0, 15.0));
      return (lon + 180.0) / 360.0 * num;
    }

    public double LatitudeToY(double lat)
    {
      double num = System.Math.Floor(System.Math.Pow(2.0, 15.0));
      Radian radian = (Radian) new Degree(lat);
      return (1.0 - System.Math.Log(System.Math.Tan(radian.Value) + 1.0 / System.Math.Cos(radian.Value)) / System.Math.PI) / 2.0 * num;
    }

    public double YToLatitude(double y)
    {
      return 180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(System.Math.PI - 2.0 * System.Math.PI * y / System.Math.Pow(2.0, 15.0)));
    }

    public double XToLongitude(double x)
    {
      return x / System.Math.Pow(2.0, 15.0) * 360.0 - 180.0;
    }

    public double ToZoomFactor(double zoomLevel)
    {
      return System.Math.Pow(2.0, zoomLevel - 15.0) * 256.0;
    }

    public double ToZoomLevel(double zoomFactor)
    {
      return System.Math.Log(zoomFactor / 256.0, 2.0) + 15.0;
    }
  }
}
