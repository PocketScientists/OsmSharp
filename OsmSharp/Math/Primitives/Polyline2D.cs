using System;

namespace OsmSharp.Math.Primitives
{
  public class Polyline2D
  {
    public static double Length(double[] x, double[] y)
    {
      double num1 = 0.0;
      if (x.Length > 1)
      {
        for (int index = 1; index < x.Length; ++index)
        {
          double num2 = x[index - 1] - x[index];
          double num3 = y[index - 1] - y[index];
          num1 += System.Math.Sqrt(num2 * num2 + num3 * num3);
        }
      }
      return num1;
    }

    public static PointF2D PositionAtPosition(double[] x, double[] y, double position)
    {
      if (x.Length < 2)
        throw new ArgumentOutOfRangeException("Given coordinates do not represent a polyline.");
      double num1 = 0.0;
      for (int index = 1; index < x.Length; ++index)
      {
        double num2 = x[index - 1] - x[index];
        double num3 = y[index - 1] - y[index];
        double num4 = System.Math.Sqrt(num2 * num2 + num3 * num3);
        if (num1 + num4 > position)
        {
          LineF2D lineF2D = new LineF2D(new PointF2D(x[index - 1], y[index - 1]), new PointF2D(x[index], y[index]));
          double num5 = position - num1;
          VectorF2D vectorF2D = lineF2D.Direction.Normalize() * num5;
          return lineF2D.Point1 + vectorF2D;
        }
        num1 += num4;
      }
      LineF2D lineF2D1 = new LineF2D(new PointF2D(x[x.Length - 2], y[x.Length - 2]), new PointF2D(x[x.Length - 1], y[x.Length - 1]));
      return lineF2D1.Point1 + lineF2D1.Direction.Normalize() * (position - num1);
    }
  }
}
