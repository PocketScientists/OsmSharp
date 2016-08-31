namespace OsmSharp.Math.Geo.Projections
{
  public interface IProjection
  {
    bool DirectionX { get; }

    bool DirectionY { get; }

    double[] ToPixel(double lat, double lon);

    double[] ToPixel(GeoCoordinate coordinate);

    GeoCoordinate ToGeoCoordinates(double x, double y);

    double LongitudeToX(double longitude);

    double LatitudeToY(double latitude);

    double YToLatitude(double y);

    double XToLongitude(double x);

    double ToZoomFactor(double zoomLevel);

    double ToZoomLevel(double zoomFactor);
  }
}
