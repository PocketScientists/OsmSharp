using System;

namespace OsmSharp.Math.Primitives
{
  public class PointF2D : PrimitiveF2D
  {
    private double[] _values;

    public double this[int idx]
    {
      get
      {
        return this._values[idx];
      }
    }

    public PointF2D(double x, double y)
    {
      this._values = new double[2]{ x, y };
    }

    public PointF2D(params double[] values)
    {
      this._values = values;
      if (this._values.Length != 2)
        throw new ArgumentException("Invalid # dimensions!");
    }

    public static VectorF2D operator -(PointF2D a, PointF2D b)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] - b[index];
      return new VectorF2D(values);
    }

    public static PointF2D operator +(PointF2D a, VectorF2D b)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] + b[index];
      return new PointF2D(values);
    }

    public static PointF2D operator -(PointF2D a, VectorF2D b)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] - b[index];
      return new PointF2D(values);
    }

    public static bool operator ==(PointF2D a, PointF2D b)
    {
      if ((object) a != null && (object) b == null || (object) b != null && (object) a == null)
        return false;
      if ((object) a == null && (object) b == null)
        return true;
      if (a._values[0] == b._values[0])
        return a._values[1] == b._values[1];
      return false;
    }

    public static bool operator !=(PointF2D a, PointF2D b)
    {
      if ((object) a != null && (object) b == null || (object) b != null && (object) a == null)
        return true;
      if ((object) a == null && (object) b == null)
        return false;
      if (a._values[0] == b._values[0])
        return a._values[1] != b._values[1];
      return true;
    }

    internal double[] ToArray()
    {
      return this._values;
    }

    public override double Distance(PointF2D p)
    {
      return PointF2D.Distance(this, p);
    }

    protected static double Distance(PointF2D a, PointF2D b)
    {
      double d = 0.0;
      for (int index = 0; index < 2; ++index)
      {
        double num = a[index] - b[index];
        d += num * num;
      }
      return System.Math.Sqrt(d);
    }

    public BoxF2D CreateBox(double offset)
    {
      return new BoxF2D(this[0] - offset, this[1] - offset, this[0] + offset, this[1] + offset);
    }

    public override string ToString()
    {
      return string.Format("Point({0},{1})", new object[2]
      {
        (object) this._values[0].ToInvariantString(),
        (object) this._values[1].ToInvariantString()
      });
    }

    public override bool Equals(object obj)
    {
      PointF2D pointF2D = obj as PointF2D;
      if (obj != null && this._values[0] == pointF2D[0])
        return this._values[1] == pointF2D[1];
      return false;
    }

    public override int GetHashCode()
    {
      return "point".GetHashCode() ^ this[0].GetHashCode() ^ this[1].GetHashCode();
    }
  }
}
