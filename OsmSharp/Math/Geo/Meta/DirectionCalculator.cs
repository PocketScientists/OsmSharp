using OsmSharp.Units.Angle;
using System;

namespace OsmSharp.Math.Geo.Meta
{
  public static class DirectionCalculator
  {
    public static DirectionEnum Calculate(GeoCoordinate from, GeoCoordinate to)
    {
      double num = 0.01;
      GeoCoordinate point2 = new GeoCoordinate(from.Latitude + num, from.Longitude);
      Radian radian = new GeoCoordinateLine(from, to).Direction.Angle(new GeoCoordinateLine(from, point2).Direction);
      if ((Degree) radian < new Degree(22.5) || (Degree) radian >= new Degree(337.5))
        return DirectionEnum.North;
      if ((Degree) radian >= new Degree(22.5) && (Degree) radian < new Degree(67.5))
        return DirectionEnum.NorthEast;
      if ((Degree) radian >= new Degree(67.5) && (Degree) radian < new Degree(112.5))
        return DirectionEnum.East;
      if ((Degree) radian >= new Degree(112.5) && (Degree) radian < new Degree(157.5))
        return DirectionEnum.SouthEast;
      if ((Degree) radian >= new Degree(157.5) && (Degree) radian < new Degree(202.5))
        return DirectionEnum.South;
      if ((Degree) radian >= new Degree(202.5) && (Degree) radian < new Degree(247.5))
        return DirectionEnum.SouthWest;
      if ((Degree) radian >= new Degree(247.5) && (Degree) radian < new Degree(292.5))
        return DirectionEnum.West;
      if ((Degree) radian >= new Degree(292.5) && (Degree) radian < new Degree(337.5))
        return DirectionEnum.NorthWest;
      throw new ArgumentOutOfRangeException();
    }
  }
}
