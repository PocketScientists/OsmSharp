using System;

namespace OsmSharp.Units.Angle
{
  public class Radian : Unit
  {
    private Radian()
      : base(0.0)
    {
    }

    public Radian(double value)
      : base(Radian.Normalize(value))
    {
    }

    public static implicit operator Radian(double value)
    {
      return new Radian(value);
    }

    public static implicit operator Radian(Degree deg)
    {
      return new Radian(deg.Value / 180.0 * System.Math.PI);
    }

    public static Radian operator -(Radian rad1, Radian rad2)
    {
      return (Radian) (rad1.Value - rad2.Value);
    }

    private static double Normalize(double value)
    {
      int num = (int)System.Math.Floor(value / (2.0 * System.Math.PI));
      return value - (double) num * (2.0 * System.Math.PI);
    }

    public override string ToString()
    {
      return string.Format("{0} rad", (object) this.Value);
    }
  }
}
