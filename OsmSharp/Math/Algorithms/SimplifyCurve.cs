using OsmSharp.Math.Primitives;
using System;

namespace OsmSharp.Math.Algorithms
{
  public static class SimplifyCurve
  {
    public static PointF2D[] Simplify(PointF2D[] points, double epsilon)
    {
      return SimplifyCurve.SimplifyBetween(points, epsilon, 0, points.Length - 1);
    }

    public static PointF2D[] SimplifyBetween(PointF2D[] points, double epsilon, int first, int last)
    {
      if (points == null)
        throw new ArgumentNullException("points");
      if (epsilon <= 0.0)
        throw new ArgumentOutOfRangeException("epsilon");
      if (first > last)
        throw new ArgumentException(string.Format("first[{0}] must be smaller or equal than last[{1}]!", new object[2]
        {
          (object) first,
          (object) last
        }));
      if (first + 1 != last)
      {
        double num1 = 0.0;
        int num2 = -1;
        LineF2D lineF2D = new LineF2D(points[first], points[last]);
        for (int index = first + 1; index < last; ++index)
        {
          double num3 = lineF2D.Distance(points[index]);
          if (num3 > num1)
          {
            num1 = num3;
            num2 = index;
          }
        }
        if (num2 > 0 && num1 > epsilon)
        {
          PointF2D[] pointF2DArray1 = SimplifyCurve.SimplifyBetween(points, epsilon, first, num2);
          PointF2D[] pointF2DArray2 = SimplifyCurve.SimplifyBetween(points, epsilon, num2, last);
          PointF2D[] pointF2DArray3 = new PointF2D[pointF2DArray1.Length + pointF2DArray2.Length - 1];
          for (int index = 0; index < pointF2DArray1.Length - 1; ++index)
            pointF2DArray3[index] = pointF2DArray1[index];
          for (int index = 0; index < pointF2DArray2.Length; ++index)
            pointF2DArray3[index + pointF2DArray1.Length - 1] = pointF2DArray2[index];
          return pointF2DArray3;
        }
      }
      return new PointF2D[2]
      {
        points[first],
        points[last]
      };
    }

    public static double[][] Simplify(double[][] points, double epsilon)
    {
      return SimplifyCurve.SimplifyBetween(points, epsilon, 0, points[0].Length - 1);
    }

    public static double[][] SimplifyBetween(double[][] points, double epsilon, int first, int last)
    {
      if (points == null)
        throw new ArgumentNullException("points");
      if (points.Length != 2)
        throw new ArgumentException();
      if (epsilon < 0.0)
        throw new ArgumentOutOfRangeException("epsilon");
      if (first > last)
        throw new ArgumentException(string.Format("first[{0}] must be smaller or equal than last[{1}]!", new object[2]
        {
          (object) first,
          (object) last
        }));
      if (epsilon == 0.0)
        return points;
      if (first == last)
        return new double[2][]
        {
          new double[1]
          {
            points[0][first]
          },
          new double[1]
          {
            points[1][first]
          }
        };
      if (points[0][first] == points[0][last] && points[1][first] == points[1][last])
      {
        double[][] numArray1 = SimplifyCurve.SimplifyBetween(points, epsilon, first, last - 1);
        double[][] numArray2 = new double[2][]
        {
          new double[numArray1[0].Length + 1],
          new double[numArray1[0].Length + 1]
        };
        for (int index = 0; index < numArray1[0].Length; ++index)
        {
          numArray2[0][index] = numArray1[0][index];
          numArray2[1][index] = numArray1[1][index];
        }
        numArray2[0][numArray1[0].Length] = points[0][last];
        numArray2[1][numArray1[0].Length] = points[1][last];
        return numArray2;
      }
      if (first + 1 != last)
      {
        double num1 = 0.0;
        int num2 = -1;
        LineF2D lineF2D = new LineF2D(new PointF2D(points[0][first], points[1][first]), new PointF2D(points[0][last], points[1][last]));
        for (int index = first + 1; index < last; ++index)
        {
          double num3 = lineF2D.Distance(new PointF2D(points[0][index], points[1][index]));
          if (num3 > num1)
          {
            num1 = num3;
            num2 = index;
          }
        }
        if (num2 > 0 && num1 > epsilon)
        {
          double[][] numArray1 = SimplifyCurve.SimplifyBetween(points, epsilon, first, num2);
          double[][] numArray2 = SimplifyCurve.SimplifyBetween(points, epsilon, num2, last);
          double[][] numArray3 = new double[2][]
          {
            new double[numArray1[0].Length + numArray2[0].Length - 1],
            new double[numArray1[0].Length + numArray2[0].Length - 1]
          };
          for (int index = 0; index < numArray1[0].Length - 1; ++index)
          {
            numArray3[0][index] = numArray1[0][index];
            numArray3[1][index] = numArray1[1][index];
          }
          for (int index = 0; index < numArray2[0].Length; ++index)
          {
            numArray3[0][index + numArray1[0].Length - 1] = numArray2[0][index];
            numArray3[1][index + numArray1[0].Length - 1] = numArray2[1][index];
          }
          return numArray3;
        }
      }
      return new double[2][]
      {
        new double[2]
        {
          points[0][first],
          points[0][last]
        },
        new double[2]
        {
          points[1][first],
          points[1][last]
        }
      };
    }

    public static double[][] SimplifyPolygon(double[][] points, double epsilon)
    {
      if (points[0].Length <= 2)
        return points;
      return SimplifyCurve.SimplifyBetween(points, epsilon, 0, points[0].Length - 1);
    }
  }
}
