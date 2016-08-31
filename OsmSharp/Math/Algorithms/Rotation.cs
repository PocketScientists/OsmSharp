using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Algorithms
{
  public class Rotation
  {
    public static PointF2D RotateAroundPoint(Radian angle, PointF2D center, PointF2D point)
    {
      double num1 = System.Math.Sin(angle.Value);
      double num2 = System.Math.Cos(angle.Value);
      return new PointF2D(center[0] + (num2 * (point[0] - center[0]) + num1 * (point[1] - center[1])), center[1] + (-num1 * (point[0] - center[0]) + num2 * (point[1] - center[1])));
    }

    public static PointF2D[] RotateAroundPoint(Radian angle, PointF2D center, PointF2D[] points)
    {
      double num1 = System.Math.Sin(angle.Value);
      double num2 = System.Math.Cos(angle.Value);
      PointF2D[] pointF2DArray = new PointF2D[points.Length];
      for (int index = 0; index < points.Length; ++index)
      {
        double x = center[0] + (num2 * (points[index][0] - center[0]) + num1 * (points[index][1] - center[1]));
        double y = center[1] + (-num1 * (points[index][0] - center[0]) + num2 * (points[index][1] - center[1]));
        pointF2DArray[index] = new PointF2D(x, y);
      }
      return pointF2DArray;
    }
  }
}
