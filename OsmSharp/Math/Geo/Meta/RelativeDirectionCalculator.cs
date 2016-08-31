using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Geo.Meta
{
  public static class RelativeDirectionCalculator
  {
    public static RelativeDirection Calculate(GeoCoordinate from, GeoCoordinate along, GeoCoordinate to)
    {
      RelativeDirection relativeDirection = new RelativeDirection();
      double num1 = 65.0;
      double num2 = 10.0;
      double num3 = 5.0;
      Radian radian = new GeoCoordinateLine(from, along).Direction.Angle(new GeoCoordinateLine(along, to).Direction);
      if ((Degree) radian >= new Degree(360.0 - num2) || (Degree) radian < new Degree(num2))
        relativeDirection.Direction = RelativeDirectionEnum.StraightOn;
      else if ((Degree) radian >= new Degree(num2) && (Degree) radian < new Degree(90.0 - num1))
        relativeDirection.Direction = RelativeDirectionEnum.SlightlyLeft;
      else if ((Degree) radian >= new Degree(90.0 - num1) && (Degree) radian < new Degree(90.0 + num1))
        relativeDirection.Direction = RelativeDirectionEnum.Left;
      else if ((Degree) radian >= new Degree(90.0 + num1) && (Degree) radian < new Degree(180.0 - num3))
        relativeDirection.Direction = RelativeDirectionEnum.SharpLeft;
      else if ((Degree) radian >= new Degree(180.0 - num3) && (Degree) radian < new Degree(180.0 + num3))
        relativeDirection.Direction = RelativeDirectionEnum.TurnBack;
      else if ((Degree) radian >= new Degree(180.0 + num3) && (Degree) radian < new Degree(270.0 - num1))
        relativeDirection.Direction = RelativeDirectionEnum.SharpRight;
      else if ((Degree) radian >= new Degree(270.0 - num1) && (Degree) radian < new Degree(270.0 + num1))
        relativeDirection.Direction = RelativeDirectionEnum.Right;
      else if ((Degree) radian >= new Degree(270.0 + num1) && (Degree) radian < new Degree(360.0 - num2))
        relativeDirection.Direction = RelativeDirectionEnum.SlightlyRight;
      relativeDirection.Angle = (Degree) radian;
      return relativeDirection;
    }
  }
}
