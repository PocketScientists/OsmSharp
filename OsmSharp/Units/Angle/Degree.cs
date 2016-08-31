using System;

namespace OsmSharp.Units.Angle
{
  public class Degree : Unit
  {
    private Degree()
      : base(0.0)
    {
    }

    public Degree(double value)
      : base(Degree.Normalize(value))
    {
    }

    public static implicit operator Degree(double value)
    {
      return new Degree(value);
    }

    public static implicit operator Degree(Radian rad)
    {
      return new Degree(rad.Value / System.Math.PI * 180.0);
    }

    public static Degree operator -(Degree deg1, Degree deg2)
    {
      return (Degree) (deg1.Value - deg2.Value);
    }

    public static Degree operator +(Degree deg1, Degree deg2)
    {
      return (Degree) (deg1.Value + deg2.Value);
    }

    public static bool operator >(Degree deg1, Degree deg2)
    {
      return deg1.Value > deg2.Value;
    }

    public static bool operator <(Degree deg1, Degree deg2)
    {
      return deg1.Value < deg2.Value;
    }

    public static bool operator >=(Degree deg1, Degree deg2)
    {
      return deg1.Value >= deg2.Value;
    }

    public static bool operator <=(Degree deg1, Degree deg2)
    {
      return deg1.Value <= deg2.Value;
    }

    private static double Normalize(double value)
    {
      int num = (int)System.Math.Floor(value / 360.0);
      return value - (double) num * 360.0;
    }

    public override string ToString()
    {
      return string.Format("{0}°", (object) this.Value);
    }

    public double Range180()
    {
      if (this.Value > 180.0)
        return this.Value - 360.0;
      return this.Value;
    }

    public double SmallestDifference(Degree angle)
    {
      return (this - angle).Range180();
    }

    public Degree Abs()
    {
      return (Degree)System.Math.Abs(this.Value);
    }
  }
}
