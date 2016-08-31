using OsmSharp.Math.Primitives.Enumerators.Lines;
using OsmSharp.Math.Primitives.Enumerators.Points;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.Primitives
{
  public class BoxF2D : PrimitiveF2D, IPointList, ILineList, IEnumerable<PointF2D>, IEnumerable, IEnumerable<LineF2D>
  {
    private double[] _max;
    private double[] _min;
    private double[] _delta;

    public double[] Delta
    {
      get
      {
        if (this._delta == null)
        {
          this._delta = new double[2];
          for (int index = 0; index < 2; ++index)
            this._delta[index] = System.Math.Abs(this._max[index] - this._min[index]);
        }
        return this._delta;
      }
    }

    public double[] Max
    {
      get
      {
        return this._max;
      }
    }

    public double[] Min
    {
      get
      {
        return this._min;
      }
    }

    public virtual PointF2D[] Corners
    {
      get
      {
        PointF2D[] pointF2DArray = new PointF2D[(int) System.Math.Pow(2.0, 2.0)];
        for (int index1 = 0; index1 < (int) System.Math.Pow(2.0, 2.0); ++index1)
        {
          double[] values = new double[2];
          for (int index2 = 0; index2 < this._max.Length; ++index2)
            values[index2] = index1 / (int) System.Math.Pow(2.0, (double) index2) % (int) System.Math.Pow(2.0, (double) (index2 + 1)) != 0 ? this._min[index2] : this._max[index2];
          pointF2DArray[index1] = new PointF2D(values);
        }
        return pointF2DArray;
      }
    }

    public PointF2D Middle
    {
      get
      {
        double[] values = new double[2];
        for (int index = 0; index < 2; ++index)
          values[index] = (this._max[index] + this._min[index]) / 2.0;
        return new PointF2D(values);
      }
    }

    public double Surface
    {
      get
      {
        double num1 = 1.0;
        foreach (double num2 in this.Delta)
          num1 = num2 * num1;
        return num1;
      }
    }

    int ILineList.Count
    {
      get
      {
        return 4;
      }
    }

    LineF2D ILineList.this[int idx]
    {
      get
      {
        IPointList pointList = (IPointList) this;
        switch (idx)
        {
          case 0:
            return new LineF2D(pointList[0], pointList[1]);
          case 1:
            return new LineF2D(pointList[1], pointList[2]);
          case 2:
            return new LineF2D(pointList[2], pointList[3]);
          case 3:
            return new LineF2D(pointList[3], pointList[0]);
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public LineEnumerable LineEnumerator
    {
      get
      {
        return new LineEnumerable(new OsmSharp.Math.Primitives.Enumerators.Lines.LineEnumerator((ILineList) this));
      }
    }

    int IPointList.Count
    {
      get
      {
        return 4;
      }
    }

    PointF2D IPointList.this[int idx]
    {
      get
      {
        switch (idx)
        {
          case 0:
            return new PointF2D(new double[2]
            {
              this._min[0],
              this._min[1]
            });
          case 1:
            return new PointF2D(new double[2]
            {
              this._max[0],
              this._min[1]
            });
          case 2:
            return new PointF2D(new double[2]
            {
              this._min[0],
              this._max[1]
            });
          case 3:
            return new PointF2D(new double[2]
            {
              this._max[0],
              this._max[1]
            });
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public PointEnumerable PointEnumerator
    {
      get
      {
        return new PointEnumerable(new OsmSharp.Math.Primitives.Enumerators.Points.PointEnumerator((IPointList) this));
      }
    }

    public BoxF2D(double x1, double y1, double x2, double y2)
      : this(new PointF2D(x1, y1), new PointF2D(x2, y2))
    {
    }

    public BoxF2D(PointF2D a)
      : this(new PointF2D[1]{ a })
    {
    }

    public BoxF2D(PointF2D a, PointF2D b)
      : this(new PointF2D[2]{ a, b })
    {
    }

    public BoxF2D(PointF2D[] points)
    {
      this.Mutate(points);
    }

    public BoxF2D(IList<PointF2D> points)
      : this(points.ToArray<PointF2D>())
    {
    }

    public BoxF2D(double[] x, double[] y)
    {
      PointF2D[] points = new PointF2D[x.Length];
      for (int index = 0; index < x.Length; ++index)
        points[index] = new PointF2D(x[index], y[index]);
      this.Mutate(points);
    }

    protected void Mutate(PointF2D[] points)
    {
      this._max = new double[2];
      this._min = new double[2];
      for (int index1 = 0; index1 < 2; ++index1)
      {
        this._max[index1] = double.MinValue;
        this._min[index1] = double.MaxValue;
        for (int index2 = 0; index2 < points.Length; ++index2)
        {
          PointF2D point = points[index2];
          if (this._max[index1] < point[index1])
            this._max[index1] = point[index1];
          if (this._min[index1] > point[index1])
            this._min[index1] = point[index1];
        }
      }
    }

    public override double Distance(PointF2D p)
    {
      double num1 = double.MaxValue;
      foreach (PrimitiveF2D primitiveF2D in this.LineEnumerator)
      {
        double num2 = primitiveF2D.Distance(p);
        if (num2 < num1)
          num1 = num2;
      }
      return num1;
    }

    public bool Contains(double x, double y)
    {
      return this.Contains(new PointF2D(x, y));
    }

    public bool Contains(PointF2D a)
    {
      bool flag = true;
      for (int index = 0; index < 2; ++index)
        flag = flag && (this._max[index] > a[index] && a[index] >= this._min[index]);
      return flag;
    }

    public bool Contains(BoxF2D box)
    {
      foreach (PointF2D corner in box.Corners)
      {
        if (!this.Contains(corner))
          return false;
      }
      return true;
    }

    public List<PointF2D> Contains(List<PointF2D> points)
    {
      List<PointF2D> pointF2DList = new List<PointF2D>();
      foreach (PointF2D point in points)
      {
        if (this.Contains(point))
          pointF2DList.Add(point);
      }
      return pointF2DList;
    }

    public List<PointF2D> Contains(PointF2D[] points)
    {
      List<PointF2D> pointF2DList = new List<PointF2D>();
      foreach (PointF2D point in points)
      {
        if (this.Contains(point))
          pointF2DList.Add(point);
      }
      return pointF2DList;
    }

    public bool ContainsAny(PointF2D[] points)
    {
      if (points != null)
      {
        for (int index = 0; index < points.Length; ++index)
        {
          if (this.Contains(points[index]))
            return true;
        }
      }
      return false;
    }

    public bool Overlaps(BoxF2D box)
    {
      double num1 = System.Math.Max(this.Min[0], box.Min[0]);
      double num2 = System.Math.Max(this.Min[1], box.Min[1]);
      double num3 = System.Math.Min(this.Max[0], box.Max[0]);
      double num4 = System.Math.Min(this.Max[1], box.Max[1]);
      double num5 = num3;
      return num1 <= num5 && num2 <= num4;
    }

    public bool IntersectsPotentially(double x1, double y1, double x2, double y2)
    {
      return (x1 <= this._max[0] || x2 <= this._max[0]) && (x1 >= this._min[0] || x2 >= this._min[0]) && ((y1 <= this._max[1] || y2 <= this._max[1]) && (y1 >= this._min[1] || y2 >= this._min[1]));
    }

    public bool IntersectsPotentially(PointF2D point1, PointF2D point2)
    {
      for (int index = 0; index < 2; ++index)
      {
        if (point1[index] > this._max[index] && point2[index] > this._max[index] || point1[index] < this._min[index] && point2[index] < this._min[index])
          return false;
      }
      return true;
    }

    public bool IntersectsPotentially(LineF2D line)
    {
      return this.IntersectsPotentially(line.Point1, line.Point2);
    }

    public bool Intersects(LineF2D line)
    {
      PointF2D[] corners = this.Corners;
      LinePointPosition linePointPosition = line.PositionOfPoint(corners[0]);
      for (int index = 1; index <= corners.Length; ++index)
      {
        if (line.PositionOfPoint(corners[index]) != linePointPosition)
          return true;
      }
      return false;
    }

    public bool Intersects(PointF2D point1, PointF2D point2)
    {
      PointF2D[] corners = this.Corners;
      LineF2D lineF2D = new LineF2D(point1, point2, true);
      LinePointPosition linePointPosition = lineF2D.PositionOfPoint(corners[0]);
      for (int index = 1; index < corners.Length; ++index)
      {
        if (lineF2D.PositionOfPoint(corners[index]) != linePointPosition)
          return true;
      }
      return false;
    }

    public BoxF2D Intersection(BoxF2D box)
    {
      double x1 = System.Math.Max(this.Min[0], box.Min[0]);
      double y1 = System.Math.Max(this.Min[1], box.Min[1]);
      double x2 = System.Math.Min(this.Max[0], box.Max[0]);
      double y2 = System.Math.Min(this.Max[1], box.Max[1]);
      if (x1 <= x2 && y1 <= y2)
        return new BoxF2D(new PointF2D(x1, y1), new PointF2D(x2, y2));
      return (BoxF2D) null;
    }

    public BoxF2D Union(BoxF2D box)
    {
      double x1 = System.Math.Min(this.Min[0], box.Min[0]);
      double num = System.Math.Min(this.Min[1], box.Min[1]);
      double x2 = System.Math.Max(this.Max[0], box.Max[0]);
      double y1 = System.Math.Max(this.Max[1], box.Max[1]);
      double y2 = num;
      return new BoxF2D(new PointF2D(x1, y2), new PointF2D(x2, y1));
    }

    public BoxF2D ScaleWith(double factor)
    {
      if (factor <= 0.0)
        throw new ArgumentOutOfRangeException();
      PointF2D middle = this.Middle;
      double num1 = this.Delta[0] * factor / 2.0;
      double num2 = this.Delta[1] * factor / 2.0;
      return new BoxF2D(new PointF2D(middle[0] - num1, middle[1] - num2), new PointF2D(middle[0] + num1, middle[1] + num2));
    }

    public BoxF2D ResizeWith(double delta)
    {
      return new BoxF2D(new PointF2D(this.Max[0] + delta, this.Max[1] + delta), new PointF2D(this.Min[0] - delta, this.Min[1] - delta));
    }

    public override string ToString()
    {
      return string.Format("RectF:[({0},{1}),({2},{3})]", (object) this.Min[0].ToInvariantString(), (object) this.Min[1].ToInvariantString(), (object) this.Max[0].ToInvariantString(), (object) this.Max[1].ToInvariantString());
    }

    public IEnumerator<PointF2D> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator<LineF2D> IEnumerable<LineF2D>.GetEnumerator()
    {
      return (IEnumerator<LineF2D>) new List<LineF2D>()
      {
        new LineF2D(this.Corners[0], this.Corners[1], true),
        new LineF2D(this.Corners[1], this.Corners[2], true),
        new LineF2D(this.Corners[2], this.Corners[3], true),
        new LineF2D(this.Corners[3], this.Corners[0], true)
      }.GetEnumerator();
    }
  }
}
