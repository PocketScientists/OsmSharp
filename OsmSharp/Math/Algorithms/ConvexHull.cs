using OsmSharp.Math.Primitives;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Algorithms
{
  public static class ConvexHull
  {
    public static IList<PointF2D> Calculate(IList<PointF2D> points)
    {
      if (points.Count < 3)
        throw new ArgumentOutOfRangeException(string.Format("Cannot calculate the convex hull of {0} points!", (object) points.Count));
      PointF2D pointF2D1 = points[0];
      foreach (PointF2D point in (IEnumerable<PointF2D>) points)
      {
        if (pointF2D1[0] > point[0])
          pointF2D1 = point;
        else if (pointF2D1[0] == point[0] && pointF2D1[1] < point[1])
          pointF2D1 = point;
      }
      PointF2D pointF2D2 = new PointF2D(new double[2]
      {
        pointF2D1[0],
        pointF2D1[1] - 10.0
      });
      VectorF2D vectorF2D = pointF2D1 - pointF2D2;
      List<PointF2D> pointF2DList = new List<PointF2D>();
      PointF2D pointF2D3 = pointF2D1;
      pointF2DList.Add(pointF2D3);
      do
      {
        double num1 = double.MaxValue;
        PointF2D pointF2D4 = (PointF2D) null;
        foreach (PointF2D point in (IEnumerable<PointF2D>) points)
        {
          if (point != pointF2D3)
          {
            VectorF2D v = point - pointF2D3;
            double num2 = vectorF2D.Angle(v).Value;
            if (num2 < num1)
            {
              num1 = num2;
              pointF2D4 = point;
            }
          }
        }
        vectorF2D = pointF2D4 - pointF2D3;
        pointF2D3 = pointF2D4;
        pointF2DList.Add(pointF2D3);
      }
      while (pointF2D3 != pointF2D1);
      return (IList<PointF2D>) pointF2DList;
    }
  }
}
