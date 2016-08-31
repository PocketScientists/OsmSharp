using OsmSharp.Math.Primitives.Enumerators.Lines;
using OsmSharp.Math.Primitives.Enumerators.Points;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.Primitives
{
  public class PolygonF2D : PrimitiveF2D, IPointList, ILineList
  {
    private PointF2D[] _points;
    private BoxF2D _bounding_box;

    public PointF2D this[int idx]
    {
      get
      {
        return this._points[idx];
      }
    }

    public int Count
    {
      get
      {
        return this._points.Length;
      }
    }

    public BoxF2D BoundingBox
    {
      get
      {
        if (this._bounding_box == null)
          this._bounding_box = new BoxF2D(this._points);
        return this._bounding_box;
      }
    }

    int ILineList.Count
    {
      get
      {
        return this.Count;
      }
    }

    LineF2D ILineList.this[int idx]
    {
      get
      {
        if (idx < this.Count - 1)
          return new LineF2D(this[idx], this[idx + 1], true);
        return new LineF2D(this[idx], this[0], true);
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
        return this.Count;
      }
    }

    PointF2D IPointList.this[int idx]
    {
      get
      {
        return this[idx];
      }
    }

    public PointEnumerable PointEnumerator
    {
      get
      {
        return new PointEnumerable(new OsmSharp.Math.Primitives.Enumerators.Points.PointEnumerator((IPointList) this));
      }
    }

    public PolygonF2D(IList<PointF2D> points)
    {
      this._points = points.ToArray<PointF2D>();
      if (this._points.Length <= 2)
        throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
    }

    public PolygonF2D(PointF2D[] points)
    {
      this._points = new List<PointF2D>((IEnumerable<PointF2D>) points).ToArray();
      if (this._points.Length <= 2)
        throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
    }

    public PolygonF2D(IEnumerable<PointF2D> points)
    {
      this._points = new List<PointF2D>(points).ToArray();
      if (this._points.Length <= 2)
        throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
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

    public bool IsInside(PointF2D point)
    {
      return false;
    }

    public PointF2D[] Intersections(LineF2D line)
    {
      List<PointF2D> pointF2DList = new List<PointF2D>();
      foreach (LineF2D line1 in this.LineEnumerator)
      {
        PrimitiveF2D primitiveF2D = line.Intersection(line1);
        if (primitiveF2D != null)
        {
          if (primitiveF2D is LineF2D)
          {
            LineF2D lineF2D = primitiveF2D as LineF2D;
            pointF2DList.Add(lineF2D.Point1);
            pointF2DList.Add(lineF2D.Point2);
          }
          else if ((object) (primitiveF2D as PointF2D) != null)
          {
            PointF2D pointF2D = primitiveF2D as PointF2D;
            pointF2DList.Add(pointF2D);
          }
        }
      }
      return pointF2DList.ToArray();
    }
  }
}
