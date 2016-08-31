using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Primitives
{
  public class LineF2D : PrimitiveF2D
  {
    private PointF2D _a;
    private PointF2D _b;
    private VectorF2D _dir;
    private bool _is_segment1;
    private bool _is_segment2;

    public PointF2D Point1
    {
      get
      {
        return this._a;
      }
    }

    public PointF2D Point2
    {
      get
      {
        return this._b;
      }
    }

    public VectorF2D Direction
    {
      get
      {
        return this._dir;
      }
    }

    public double Length
    {
      get
      {
        return this.Direction.Size;
      }
    }

    public bool IsSegment
    {
      get
      {
        if (this._is_segment1)
          return this._is_segment2;
        return false;
      }
    }

    public bool IsSegment1
    {
      get
      {
        return this._is_segment1;
      }
    }

    public bool IsSegment2
    {
      get
      {
        return this._is_segment2;
      }
    }

    internal double A
    {
      get
      {
        return this.Point2[1] - this.Point1[1];
      }
    }

    internal double B
    {
      get
      {
        return this.Point1[0] - this.Point2[0];
      }
    }

    internal double C
    {
      get
      {
        return this.A * this.Point1[0] + this.B * this.Point1[1];
      }
    }

    public LineF2D(PointF2D a, PointF2D b)
      : this(a, b, false)
    {
    }

    public LineF2D(PointF2D a, PointF2D b, bool is_segment)
    {
      this._a = a;
      this._b = b;
      this._dir = this._b - this._a;
      this._is_segment1 = is_segment;
      this._is_segment2 = is_segment;
    }

    public LineF2D(PointF2D a, PointF2D b, bool is_segment1, bool is_segment2)
    {
      this._a = a;
      this._b = b;
      this._dir = this._b - this._a;
      this._is_segment1 = is_segment1;
      this._is_segment2 = is_segment2;
    }

    public LineF2D(double xa, double ya, double xb, double yb)
      : this(new PointF2D(xa, ya), new PointF2D(xb, yb))
    {
    }

    public LineF2D(double xa, double ya, double xb, double yb, bool is_segment)
      : this(new PointF2D(xa, ya), new PointF2D(xb, yb), is_segment)
    {
    }

    public LineF2D(double xa, double ya, double xb, double yb, bool is_segment1, bool is_segment2)
      : this(new PointF2D(xa, ya), new PointF2D(xb, yb), is_segment1, is_segment2)
    {
    }

    public LinePointPosition PositionOfPoint(PointF2D point)
    {
      double num = VectorF2D.Cross(this.Direction, point - this._a);
      if (num > 0.0)
        return LinePointPosition.Left;
      return num < 0.0 ? LinePointPosition.Right : LinePointPosition.On;
    }

    public override double Distance(PointF2D point)
    {
      VectorF2D vectorF2D = point - this._a;
      double num1 = VectorF2D.Cross(this.Direction, vectorF2D);
      double length = this.Length;
      double num2 = point.Distance(this._a);
      double num3 = point.Distance(this._b);
      double num4 = System.Math.Abs(num1 / length);
      if (this.IsSegment)
      {
        if (VectorF2D.Dot(vectorF2D, this.Direction) < 0.0 && num1 != 0.0)
          num4 = this._a.Distance(point);
        else if (num1 == 0.0 && (num2 >= length || num3 >= length))
          num4 = num2 <= num3 ? this._a.Distance(point) : this._b.Distance(point);
        if (VectorF2D.Dot(point - this._b, this.Direction.Inverse) < 0.0 && num1 != 0.0)
          num4 = this._b.Distance(point);
      }
      return num4;
    }

    public bool Intersects(LineF2D line)
    {
      return this.Intersection(line) != null;
    }

    public bool Intersects(LineF2D line, bool doSegment)
    {
      return this.Intersection(line, doSegment) != null;
    }

    public PrimitiveF2D Intersection(LineF2D line)
    {
      return this.Intersection(line, true);
    }

    public PrimitiveF2D Intersection(LineF2D line, bool doSegment)
    {
      if (line == this)
        return (PrimitiveF2D) line;
      if (line.A == this.A && line.B == this.B && line.C == this.C)
      {
        KeyValuePair<double, PointF2D> keyValuePair1 = new KeyValuePair<double, PointF2D>(0.0, this.Point1);
        KeyValuePair<double, PointF2D> keyValuePair2 = new KeyValuePair<double, PointF2D>(this.Point1.Distance(this.Point2), this.Point2);
        if (keyValuePair2.Key < keyValuePair1.Key)
        {
          KeyValuePair<double, PointF2D> keyValuePair3 = keyValuePair2;
          keyValuePair2 = keyValuePair1;
          keyValuePair1 = keyValuePair3;
        }
        KeyValuePair<double, PointF2D> keyValuePair4 = new KeyValuePair<double, PointF2D>(this.Point1.Distance(line.Point1), line.Point1);
        if (keyValuePair4.Key < keyValuePair2.Key)
        {
          KeyValuePair<double, PointF2D> keyValuePair3 = keyValuePair4;
          keyValuePair4 = keyValuePair2;
          keyValuePair2 = keyValuePair3;
        }
        if (keyValuePair2.Key < keyValuePair1.Key)
        {
          KeyValuePair<double, PointF2D> keyValuePair3 = keyValuePair2;
          keyValuePair2 = keyValuePair1;
          keyValuePair1 = keyValuePair3;
        }
        keyValuePair4 = new KeyValuePair<double, PointF2D>(this.Point1.Distance(line.Point2), line.Point2);
        if (keyValuePair4.Key < keyValuePair2.Key)
        {
          KeyValuePair<double, PointF2D> keyValuePair3 = keyValuePair4;
          keyValuePair4 = keyValuePair2;
          keyValuePair2 = keyValuePair3;
        }
        if (keyValuePair2.Key < keyValuePair1.Key)
        {
          KeyValuePair<double, PointF2D> keyValuePair3 = keyValuePair2;
          keyValuePair2 = keyValuePair1;
          keyValuePair1 = keyValuePair3;
        }
        if (doSegment && (this.IsSegment1 || this.IsSegment2))
        {
          double num = this.Point1.Distance(this.Point2);
          if (this.IsSegment)
          {
            if (System.Math.Min(this.Point1.Distance(keyValuePair1.Value), this.Point1.Distance(keyValuePair2.Value)) > num)
              return (PrimitiveF2D) null;
          }
          else if (this.IsSegment1)
          {
            if (this.Point1.Distance(keyValuePair1.Value) > num)
              return (PrimitiveF2D) null;
          }
          else if (this.IsSegment2 && this.Point2.Distance(keyValuePair1.Value) > num)
            return (PrimitiveF2D) null;
        }
        if (doSegment && (line.IsSegment1 || line.IsSegment2))
        {
          double num = line.Point1.Distance(line.Point2);
          if (line.IsSegment)
          {
            if (System.Math.Min(line.Point1.Distance(keyValuePair1.Value), line.Point1.Distance(keyValuePair2.Value)) > num)
              return (PrimitiveF2D) null;
          }
          else if (line.IsSegment1)
          {
            if (line.Point1.Distance(keyValuePair1.Value) > num)
              return (PrimitiveF2D) null;
          }
          else if (line.IsSegment2 && line.Point2.Distance(keyValuePair2.Value) > num)
            return (PrimitiveF2D) null;
        }
        return (PrimitiveF2D) new LineF2D(keyValuePair1.Value, keyValuePair2.Value, true);
      }
      double num1 = line.A * this.B - this.A * line.B;
      if (num1 == 0.0)
        return (PrimitiveF2D) null;
      PointF2D p = new PointF2D(new double[2]
      {
        (this.B * line.C - line.B * this.C) / num1,
        (line.A * this.C - this.A * line.C) / num1
      });
      if (doSegment && (this.IsSegment1 || this.IsSegment2))
      {
        double num2 = this.Point1.Distance(this.Point2);
        if (this.IsSegment)
        {
          if (this.Point1.Distance(p) > num2)
            return (PrimitiveF2D) null;
          if (this.Point2.Distance(p) > num2)
            return (PrimitiveF2D) null;
        }
        else if (this.IsSegment1 && this.Point2.Distance(p) > num2)
        {
          if ((p - this.Point2).CompareNormalized(this.Direction.Inverse, 0.001))
            return (PrimitiveF2D) null;
        }
        else if (this.IsSegment1 && this.Point1.Distance(p) > num2 && (p - this.Point1).CompareNormalized(this.Direction, 0.001))
          return (PrimitiveF2D) null;
      }
      if (doSegment && (line.IsSegment1 || line.IsSegment2))
      {
        double num2 = line.Point1.Distance(line.Point2);
        if (line.IsSegment)
        {
          if (line.Point1.Distance(p) > num2)
            return (PrimitiveF2D) null;
          if (line.Point2.Distance(p) > num2)
            return (PrimitiveF2D) null;
        }
        else if (line.IsSegment1 && line.Point2.Distance(p) > num2)
        {
          if ((p - line.Point2).CompareNormalized(line.Direction.Inverse, 0.001))
            return (PrimitiveF2D) null;
        }
        else if (line.IsSegment1 && line.Point1.Distance(p) > num2 && (p - line.Point1).CompareNormalized(line.Direction, 0.001))
          return (PrimitiveF2D) null;
      }
      return (PrimitiveF2D) p;
    }

    public PointF2D ProjectOn(PointF2D point)
    {
      if (this.Length == 0.0 && this.IsSegment)
        return (PointF2D) null;
      VectorF2D vectorF2D = this.Direction.Rotate90(true);
      PointF2D b = new PointF2D((point + vectorF2D).ToArray());
      if (point[0] == b[0] && point[1] == b[1])
        return b;
      PrimitiveF2D primitiveF2D = this.Intersection(new LineF2D(point, b, false, false), true);
      if (primitiveF2D == null)
        return (PointF2D) null;
      if ((object) (primitiveF2D as PointF2D) != null)
        return primitiveF2D as PointF2D;
      throw new InvalidOperationException();
    }

    public override string ToString()
    {
      return string.Format("Line{0}{1},{2}{3}", (object) (this.IsSegment1 ? "[" : "]"), (object) this.Point1.ToString(), (object) this.Point2.ToString(), (object) (this.IsSegment2 ? "]" : "["));
    }
  }
}
