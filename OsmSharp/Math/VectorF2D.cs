using OsmSharp.Units.Angle;
using System;
using System.Globalization;

namespace OsmSharp.Math
{
  public class VectorF2D
  {
    private double[] _values;

    public double this[int idx]
    {
      get
      {
        return this._values[idx];
      }
    }

    public double Size
    {
      get
      {
        double d = 0.0;
        for (int index = 0; index < 2; ++index)
          d += this._values[index] * this._values[index];
        return System.Math.Sqrt(d);
      }
    }

    public VectorF2D Inverse
    {
      get
      {
        return new VectorF2D(new double[2]
        {
          -this._values[0],
          -this._values[1]
        });
      }
    }

    public VectorF2D InverseX
    {
      get
      {
        return new VectorF2D(new double[2]
        {
          -this._values[0],
          this._values[1]
        });
      }
    }

    public VectorF2D InverseY
    {
      get
      {
        return new VectorF2D(new double[2]
        {
          this._values[0],
          -this._values[1]
        });
      }
    }

    public VectorF2D(double a, double b)
    {
      this._values = new double[2];
      this._values[0] = a;
      this._values[1] = b;
    }

    public VectorF2D(double[] values)
    {
      this._values = values;
      if (this._values.Length != 2)
        throw new ArgumentException("Invalid # dimensions!");
    }

    public static VectorF2D operator -(VectorF2D a, VectorF2D b)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] - b[index];
      return new VectorF2D(values);
    }

    public static VectorF2D operator +(VectorF2D a, VectorF2D b)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] + b[index];
      return new VectorF2D(values);
    }

    public static VectorF2D operator *(VectorF2D a, double value)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] * value;
      return new VectorF2D(values);
    }

    public static VectorF2D operator /(VectorF2D a, double value)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] / value;
      return new VectorF2D(values);
    }

    public static bool operator ==(VectorF2D a, VectorF2D b)
    {
      if ((object) a == (object) b)
        return true;
      if ((object) a == null || (object) b == null)
        return false;
      for (int index = 0; index < 2; ++index)
      {
        if (a[index] != b[index])
          return false;
      }
      return true;
    }

    public static bool operator !=(VectorF2D a, VectorF2D b)
    {
      return !(a == b);
    }

    public override string ToString()
    {
      return string.Format("Vector({0},{1})", new object[2]
      {
        (object) this._values[0].ToString((IFormatProvider) CultureInfo.InvariantCulture),
        (object) this._values[1].ToString((IFormatProvider) CultureInfo.InvariantCulture)
      });
    }

    public static double Dot(VectorF2D a, VectorF2D b)
    {
      double num = 0.0;
      for (int index = 0; index < 2; ++index)
        num += a[index] * b[index];
      return num;
    }

    public static double Cross(VectorF2D a, VectorF2D b)
    {
      return a[0] * b[1] - a[1] * b[0];
    }

    public static Radian Angle(VectorF2D v1, VectorF2D v2)
    {
      double size1 = v1.Size;
      double size2 = v2.Size;
      double num1 = VectorF2D.Dot(v1, v2);
      double num2 = VectorF2D.Cross(v1, v2);
      if (v1[0] == v2[0] && v1[1] == v2[1])
        return (Radian) 0.0;
      if (v1[0] == v2[0] && v1[1] == -v2[1])
        return (Radian) (System.Math.PI / 2.0);
      if (v1[0] == -v2[0] && v1[1] == v2[1])
        return (Radian) (-1.0 * System.Math.PI / 2.0);
      if (v1[0] == -v2[0] && v1[1] == -v2[1])
        return (Radian) System.Math.PI;
      double num3;
      if (num1 > 0.0)
      {
        if (num2 > 0.0)
        {
          num3 = System.Math.Asin(num2 / (size1 * size2));
          if (num3 < System.Math.PI / 4.0)
            num3 = System.Math.Acos(num1 / (size1 * size2));
        }
        else
        {
          num3 = 2.0 * System.Math.PI + System.Math.Asin(num2 / (size1 * size2));
          if (num3 > 7.0 * System.Math.PI / 4.0)
            num3 = 2.0 * System.Math.PI - System.Math.Acos(num1 / (size1 * size2));
        }
      }
      else if (num2 > 0.0)
      {
        num3 = System.Math.PI - System.Math.Asin(num2 / (size1 * size2));
        if (num3 > 3.0 * System.Math.PI / 4.0)
          num3 = System.Math.Acos(num1 / (size1 * size2));
      }
      else
      {
        num3 = -(System.Math.Asin(num2 / (size1 * size2)) - System.Math.PI);
        if (num3 < 5.0 * System.Math.PI / 4.0)
          num3 = 2.0 * System.Math.PI - System.Math.Acos(num1 / (size1 * size2));
      }
      return (Radian) num3;
    }

    public Radian Angle(VectorF2D v)
    {
      return VectorF2D.Angle(this, v);
    }

    public static VectorF2D FromAngleY(Degree angle)
    {
      return new VectorF2D(System.Math.Sin(angle.Value), System.Math.Cos(angle.Value));
    }

    public VectorF2D Rotate90(bool clockwise)
    {
      if (clockwise)
        return new VectorF2D(this[1], -this[0]);
      return new VectorF2D(-this[1], this[0]);
    }

    public VectorF2D Normalize()
    {
      double size = this.Size;
      return new VectorF2D(this[0] / size, this[1] / size);
    }

    public bool CompareNormalized(VectorF2D other)
    {
      return this.CompareNormalized(other, 0.0);
    }

    public bool CompareNormalized(VectorF2D other, double epsilon)
    {
      VectorF2D vectorF2D1 = this.Normalize();
      VectorF2D vectorF2D2 = other.Normalize();
      return System.Math.Abs(vectorF2D1[0] - vectorF2D2[0]) + System.Math.Abs(vectorF2D1[1] - vectorF2D2[1]) < epsilon;
    }

    public override bool Equals(object obj)
    {
      if ((object) (obj as VectorF2D) != null)
        return this == obj as VectorF2D;
      return false;
    }

    public override int GetHashCode()
    {
      return "vector".GetHashCode() ^ this[0].GetHashCode() ^ this[1].GetHashCode();
    }
  }
}
