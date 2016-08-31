namespace OsmSharp.Math.Geo
{
  public static class Extensions
  {
    public static double DistanceEstimate(this GeoCoordinate[] coordinates, int start, int lenght)
    {
      double num = 0.0;
      for (int index = start; index < lenght + start; ++index)
      {
        if (index + 1 < lenght + start)
          num += coordinates[index].DistanceEstimate(coordinates[index + 1]).Value;
      }
      return num;
    }
  }
}
