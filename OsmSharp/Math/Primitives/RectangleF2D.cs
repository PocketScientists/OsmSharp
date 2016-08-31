using OsmSharp.Math.Algorithms;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Primitives
{
  public class RectangleF2D : PrimitiveF2D
  {
    private VectorF2D _vectorX;
    private VectorF2D _vectorY;
    private PointF2D _bottomLeft;

    public PointF2D BottomLeft
    {
      get
      {
        return this._bottomLeft;
      }
    }

    public PointF2D TopLeft
    {
      get
      {
        return this._bottomLeft + this._vectorY;
      }
    }

    public PointF2D BottomRight
    {
      get
      {
        return this._bottomLeft + this._vectorX;
      }
    }

    public PointF2D TopRight
    {
      get
      {
        return this._bottomLeft + this._vectorX + this._vectorY;
      }
    }

    public PointF2D Center
    {
      get
      {
        return this._bottomLeft + this._vectorX / 2.0 + this._vectorY / 2.0;
      }
    }

    public double Width
    {
      get
      {
        return this._vectorX.Size;
      }
    }

    public double Height
    {
      get
      {
        return this._vectorY.Size;
      }
    }

    public Degree Angle
    {
      get
      {
        return (Degree) this._vectorY.Angle(new VectorF2D(0.0, 1.0));
      }
    }

    public BoxF2D BoundingBox
    {
      get
      {
        return new BoxF2D(new PointF2D[4]
        {
          this.BottomLeft,
          this.TopRight,
          this.BottomRight,
          this.TopLeft
        });
      }
    }

    public VectorF2D DirectionX
    {
      get
      {
        return this._vectorX;
      }
    }

    public VectorF2D DirectionY
    {
      get
      {
        return this._vectorY;
      }
    }

    public RectangleF2D(double x, double y, double width, double height)
    {
      this._bottomLeft = new PointF2D(x, y);
      this._vectorX = new VectorF2D(width, 0.0);
      this._vectorY = new VectorF2D(0.0, height);
    }

    public RectangleF2D(double x, double y, double width, double height, Degree angleY)
    {
      this._bottomLeft = new PointF2D(x, y);
      VectorF2D vectorF2D = VectorF2D.FromAngleY(angleY);
      this._vectorY = vectorF2D * height;
      this._vectorX = vectorF2D.Rotate90(true) * width;
    }

    public RectangleF2D(double x, double y, double width, double height, VectorF2D directionY)
    {
      this._bottomLeft = new PointF2D(x, y);
      directionY = directionY.Normalize();
      this._vectorY = directionY * height;
      this._vectorX = directionY.Rotate90(true) * width;
    }

    public RectangleF2D(PointF2D bottomLeft, double width, double height, VectorF2D directionY)
    {
      this._bottomLeft = bottomLeft;
      VectorF2D vectorF2D = directionY.Normalize();
      this._vectorY = vectorF2D * height;
      this._vectorX = vectorF2D.Rotate90(true) * width;
    }

    public static RectangleF2D FromBoundsAndCenter(double width, double height, double centerX, double centerY, Degree angleY)
    {
      return RectangleF2D.FromBoundsAndCenter(width, height, centerX, centerY, VectorF2D.FromAngleY(angleY));
    }

    public static RectangleF2D FromBoundsAndCenter(double width, double height, double centerX, double centerY, VectorF2D directionY)
    {
      VectorF2D vectorF2D1 = directionY.Normalize();
      VectorF2D vectorF2D2 = vectorF2D1.Rotate90(true);
      return new RectangleF2D(new PointF2D(centerX, centerY) - vectorF2D1 * (height / 2.0) - vectorF2D2 * (width / 2.0), width, height, directionY);
    }

    public RectangleF2D Fit(PointF2D[] points, double percentage)
    {
      if (points == null)
        throw new ArgumentNullException("points");
      if (points.Length < 2)
        throw new ArgumentOutOfRangeException("Rectangle fit needs at least two points.");
      double[] values = new double[2]
      {
        points[0][0],
        points[0][1]
      };
      for (int index = 1; index < points.Length; ++index)
      {
        values[0] = values[0] + points[index][0];
        values[1] = values[1] + points[index][1];
      }
      values[0] = values[0] / (double) points.Length;
      values[1] = values[1] / (double) points.Length;
      PointF2D p = new PointF2D(values);
      double num1 = 0.0;
      for (int index = 0; index < points.Length; ++index)
      {
        double num2 = new LineF2D(points[index], points[index] + this._vectorY).Distance(p);
        if (num2 > num1)
          num1 = num2;
      }
      double num3 = num1 * 2.0;
      double num4 = 0.0;
      for (int index = 0; index < points.Length; ++index)
      {
        double num2 = new LineF2D(points[index], points[index] + this._vectorX).Distance(p);
        if (num2 > num4)
          num4 = num2;
      }
      double num5 = num4 * 2.0;
      return RectangleF2D.FromBoundsAndCenter(num3 + num3 / 100.0 * percentage, num5 + num5 / 100.0 * percentage, p[0], p[1], this.DirectionY);
    }

    public RectangleF2D FitAndKeepAspectRatio(PointF2D[] points, double percentage)
    {
      RectangleF2D rectangleF2D = this;
      if (points.Length > 1)
        rectangleF2D = this.Fit(points, percentage);
      else if (points.Length == 1)
        rectangleF2D = new RectangleF2D(points[0][0], points[0][1], this.Width, this.Height, this.Angle);
      double width = rectangleF2D.Width;
      double height = rectangleF2D.Height;
      double num1 = this.Width / this.Height;
      if (rectangleF2D.Height > rectangleF2D.Width)
      {
        double num2 = rectangleF2D.Height * num1;
        if (num2 < rectangleF2D.Width)
          height = rectangleF2D.Width / num1;
        else
          width = num2;
      }
      else
      {
        double num2 = rectangleF2D.Width / num1;
        if (num2 < rectangleF2D.Height)
          width = rectangleF2D.Height * num1;
        else
          height = num2;
      }
      return RectangleF2D.FromBoundsAndCenter(width, height, rectangleF2D.Center[0], rectangleF2D.Center[1], rectangleF2D.DirectionY);
    }

    public bool Contains(PointF2D point)
    {
      double[] numArray = this.TransformTo(100.0, 100.0, false, false, point);
      if (numArray[0] >= 0.0 && numArray[0] <= 100.0 && numArray[1] >= 0.0)
        return numArray[1] <= 100.0;
      return false;
    }

    public bool Contains(double x, double y)
    {
      return this.Contains(new PointF2D(x, y));
    }

    public bool Overlaps(BoxF2D box)
    {
      if (box.Contains(this.BottomLeft) || box.Contains(this.BottomRight) || (box.Contains(this.TopLeft) || box.Contains(this.TopRight)) || (this.Contains(box.Corners[0]) || this.Contains(box.Corners[2]) || (this.Contains(box.Corners[3]) || this.Contains(box.Corners[0]))))
        return true;
      List<LineF2D> lineF2DList = new List<LineF2D>();
      lineF2DList.Add(new LineF2D(this.BottomLeft, this.BottomRight, true));
      lineF2DList.Add(new LineF2D(this.BottomRight, this.TopRight, true));
      lineF2DList.Add(new LineF2D(this.TopRight, this.TopLeft, true));
      lineF2DList.Add(new LineF2D(this.TopLeft, this.BottomLeft, true));
      foreach (LineF2D lineF2D in (IEnumerable<LineF2D>) box)
      {
        foreach (LineF2D line in lineF2DList)
        {
          if (lineF2D.Intersects(line))
            return true;
        }
      }
      return false;
    }

    public bool Overlaps(RectangleF2D rectangle)
    {
      if (rectangle.Contains(this.BottomLeft) || rectangle.Contains(this.BottomRight) || (rectangle.Contains(this.TopLeft) || rectangle.Contains(this.TopRight)) || (this.Contains(rectangle.BottomLeft) || this.Contains(rectangle.BottomRight) || (this.Contains(rectangle.TopLeft) || this.Contains(rectangle.TopRight))))
        return true;
      List<LineF2D> lineF2DList1 = new List<LineF2D>();
      lineF2DList1.Add(new LineF2D(this.BottomLeft, this.BottomRight, true));
      lineF2DList1.Add(new LineF2D(this.BottomRight, this.TopRight, true));
      lineF2DList1.Add(new LineF2D(this.TopRight, this.TopLeft, true));
      lineF2DList1.Add(new LineF2D(this.TopLeft, this.BottomLeft, true));
      List<LineF2D> lineF2DList2 = new List<LineF2D>();
      lineF2DList2.Add(new LineF2D(rectangle.BottomLeft, rectangle.BottomRight, true));
      lineF2DList2.Add(new LineF2D(rectangle.BottomRight, rectangle.TopRight, true));
      lineF2DList2.Add(new LineF2D(rectangle.TopRight, rectangle.TopLeft, true));
      lineF2DList2.Add(new LineF2D(rectangle.TopLeft, rectangle.BottomLeft, true));
      foreach (LineF2D lineF2D in lineF2DList1)
      {
        foreach (LineF2D line in lineF2DList2)
        {
          if (lineF2D.Intersects(line))
            return true;
        }
      }
      return false;
    }

    public RectangleF2D RotateAroundCenter(Degree angle)
    {
      return this.RotateAround(angle, this.Center);
    }

    public RectangleF2D RotateAround(Degree angle, PointF2D center)
    {
      PointF2D[] points = new PointF2D[4]
      {
        this.TopLeft,
        this.TopRight,
        this.BottomLeft,
        this.BottomRight
      };
      PointF2D[] pointF2DArray = Rotation.RotateAroundPoint((Radian) angle, center, points);
      return new RectangleF2D(pointF2DArray[2], this.Width, this.Height, pointF2DArray[0] - pointF2DArray[2]);
    }

    public double[] TransformFrom(double width, double height, bool reverseX, bool reverseY, double[] coordinates)
    {
      return this.TransformFrom(width, height, reverseX, reverseY, coordinates[0], coordinates[1]);
    }

    public double[] TransformFrom(double width, double height, bool reverseX, bool reverseY, double x, double y)
    {
      PointF2D pointF2D = this._bottomLeft;
      VectorF2D vectorF2D1 = this._vectorX;
      VectorF2D vectorF2D2 = this._vectorY;
      if (reverseX && !reverseY)
      {
        pointF2D = this.BottomRight;
        vectorF2D1 = this._vectorX * -1.0;
      }
      else if (!reverseX & reverseY)
      {
        pointF2D = this.TopLeft;
        vectorF2D2 = this._vectorY * -1.0;
      }
      else if (reverseX & reverseY)
      {
        pointF2D = this.TopRight;
        vectorF2D1 = this._vectorX * -1.0;
        vectorF2D2 = this._vectorY * -1.0;
      }
      double num1 = x / width;
      double num2 = y / height;
      return (pointF2D + vectorF2D1 * num1 + vectorF2D2 * num2).ToArray();
    }

    public double[][] TransformFrom(double width, double height, bool reverseX, bool reverseY, double[] x, double[] y)
    {
      PointF2D pointF2D1 = this._bottomLeft;
      VectorF2D vectorF2D1 = this._vectorX;
      VectorF2D vectorF2D2 = this._vectorY;
      if (reverseX && !reverseY)
      {
        pointF2D1 = this.BottomRight;
        vectorF2D1 = this._vectorX * -1.0;
      }
      else if (!reverseX & reverseY)
      {
        pointF2D1 = this.TopLeft;
        vectorF2D2 = this._vectorY * -1.0;
      }
      else if (reverseX & reverseY)
      {
        pointF2D1 = this.TopRight;
        vectorF2D1 = this._vectorX * -1.0;
        vectorF2D2 = this._vectorY * -1.0;
      }
      double[][] numArray = new double[x.Length][];
      for (int index = 0; index < x.Length; ++index)
      {
        double num1 = x[index] / width;
        double num2 = y[index] / height;
        PointF2D pointF2D2 = pointF2D1 + vectorF2D1 * num1 + vectorF2D2 * num2;
        numArray[index] = pointF2D2.ToArray();
      }
      return numArray;
    }

    public double[] TransformTo(double width, double height, bool reverseX, bool reverseY, double[] coordinates)
    {
      return this.TransformTo(width, height, reverseX, reverseY, new PointF2D(coordinates[0], coordinates[1]));
    }

    public double[] TransformTo(double width, double height, bool reverseX, bool reverseY, double x, double y)
    {
      return this.TransformTo(width, height, reverseX, reverseY, new PointF2D(x, y));
    }

    public void TransformTo(double width, double height, bool reverseX, bool reverseY, double x, double y, double[] transformed)
    {
      this.TransformTo(width, height, reverseX, reverseY, new PointF2D(x, y), transformed);
    }

    public double[][] TransformTo(double width, double height, bool reverseX, bool reverseY, double[] x, double[] y)
    {
      PointF2D a1 = this._bottomLeft;
      VectorF2D vectorF2D1 = this._vectorX;
      VectorF2D vectorF2D2 = this._vectorY;
      if (reverseX && !reverseY)
      {
        a1 = this.BottomRight;
        vectorF2D1 = this._vectorX * -1.0;
      }
      else if (!reverseX & reverseY)
      {
        a1 = this.TopLeft;
        vectorF2D2 = this._vectorY * -1.0;
      }
      else if (reverseX & reverseY)
      {
        a1 = this.TopRight;
        vectorF2D1 = this._vectorX * -1.0;
        vectorF2D2 = this._vectorY * -1.0;
      }
      double[][] numArray = new double[x.Length][];
      for (int index = 0; index < x.Length; ++index)
      {
        PointF2D a2 = new PointF2D(x[index], y[index]);
        VectorF2D vectorF2D3 = vectorF2D1;
        PointF2D b1 = a2 + vectorF2D3;
        int num1 = 0;
        VectorF2D vectorF2D4 = (new LineF2D(a2, b1, num1 != 0).Intersection(new LineF2D(a1, a1 + vectorF2D2)) as PointF2D) - a1;
        double num2 = vectorF2D4.Size / vectorF2D2.Size;
        VectorF2D other1 = vectorF2D2;
        double epsilon1 = 0.0001;
        if (!vectorF2D4.CompareNormalized(other1, epsilon1))
          num2 = -num2;
        VectorF2D vectorF2D5 = vectorF2D2;
        PointF2D b2 = a2 + vectorF2D5;
        VectorF2D vectorF2D6 = (new LineF2D(a2, b2).Intersection(new LineF2D(a1, a1 + vectorF2D1)) as PointF2D) - a1;
        double num3 = vectorF2D6.Size / vectorF2D1.Size;
        VectorF2D other2 = vectorF2D1;
        double epsilon2 = 0.0001;
        if (!vectorF2D6.CompareNormalized(other2, epsilon2))
          num3 = -num3;
        numArray[index] = new double[2]
        {
          num3 * width,
          num2 * height
        };
      }
      return numArray;
    }

    public void TransformTo(double width, double height, bool reverseX, bool reverseY, PointF2D point, double[] transformed)
    {
      if (transformed == null)
        throw new ArgumentNullException();
      if (transformed.Length != 2)
        throw new ArgumentException("Tranformed array needs to be of length 2.");
      PointF2D a = this._bottomLeft;
      VectorF2D vectorF2D1 = this._vectorX;
      VectorF2D vectorF2D2 = this._vectorY;
      if (reverseX && !reverseY)
      {
        a = this.BottomRight;
        vectorF2D1 = this._vectorX * -1.0;
      }
      else if (!reverseX & reverseY)
      {
        a = this.TopLeft;
        vectorF2D2 = this._vectorY * -1.0;
      }
      else if (reverseX & reverseY)
      {
        a = this.TopRight;
        vectorF2D1 = this._vectorX * -1.0;
        vectorF2D2 = this._vectorY * -1.0;
      }
      VectorF2D vectorF2D3 = (new LineF2D(point, point + vectorF2D1, false).Intersection(new LineF2D(a, a + vectorF2D2)) as PointF2D) - a;
      double num1 = vectorF2D3.Size / vectorF2D2.Size;
      VectorF2D other1 = vectorF2D2;
      double epsilon1 = 0.0001;
      if (!vectorF2D3.CompareNormalized(other1, epsilon1))
        num1 = -num1;
      VectorF2D vectorF2D4 = (new LineF2D(point, point + vectorF2D2).Intersection(new LineF2D(a, a + vectorF2D1)) as PointF2D) - a;
      double num2 = vectorF2D4.Size / vectorF2D1.Size;
      VectorF2D other2 = vectorF2D1;
      double epsilon2 = 0.0001;
      if (!vectorF2D4.CompareNormalized(other2, epsilon2))
        num2 = -num2;
      transformed[0] = num2 * width;
      transformed[1] = num1 * height;
    }

    public double[] TransformTo(double width, double height, bool reverseX, bool reverseY, PointF2D point)
    {
      PointF2D a = this._bottomLeft;
      VectorF2D vectorF2D1 = this._vectorX;
      VectorF2D vectorF2D2 = this._vectorY;
      if (reverseX && !reverseY)
      {
        a = this.BottomRight;
        vectorF2D1 = this._vectorX * -1.0;
      }
      else if (!reverseX & reverseY)
      {
        a = this.TopLeft;
        vectorF2D2 = this._vectorY * -1.0;
      }
      else if (reverseX & reverseY)
      {
        a = this.TopRight;
        vectorF2D1 = this._vectorX * -1.0;
        vectorF2D2 = this._vectorY * -1.0;
      }
      VectorF2D vectorF2D3 = (new LineF2D(point, point + vectorF2D1, false).Intersection(new LineF2D(a, a + vectorF2D2)) as PointF2D) - a;
      double num1 = vectorF2D3.Size / vectorF2D2.Size;
      VectorF2D other1 = vectorF2D2;
      double epsilon1 = 0.0001;
      if (!vectorF2D3.CompareNormalized(other1, epsilon1))
        num1 = -num1;
      VectorF2D vectorF2D4 = (new LineF2D(point, point + vectorF2D2).Intersection(new LineF2D(a, a + vectorF2D1)) as PointF2D) - a;
      double num2 = vectorF2D4.Size / vectorF2D1.Size;
      VectorF2D other2 = vectorF2D1;
      double epsilon2 = 0.0001;
      if (!vectorF2D4.CompareNormalized(other2, epsilon2))
        num2 = -num2;
      return new double[2]
      {
        num2 * width,
        num1 * height
      };
    }

    public override double Distance(PointF2D p)
    {
      double num1 = new LineF2D(this.BottomLeft, this.BottomRight, true).Distance(p);
      double num2 = new LineF2D(this.BottomRight, this.TopRight, true).Distance(p);
      if (num2 < num1)
        num1 = num2;
      double num3 = new LineF2D(this.TopRight, this.TopLeft, true).Distance(p);
      if (num3 < num1)
        num1 = num3;
      double num4 = new LineF2D(this.TopLeft, this.BottomLeft, true).Distance(p);
      if (num4 < num1)
        num1 = num4;
      return num1;
    }

    public override bool Equals(object obj)
    {
      RectangleF2D rectangleF2D = obj as RectangleF2D;
      if (rectangleF2D != null && rectangleF2D.BottomLeft.Equals((object) this.BottomLeft) && rectangleF2D.DirectionX.Equals((object) this.DirectionX))
        return rectangleF2D.DirectionY.Equals((object) this.DirectionY);
      return false;
    }

    public override int GetHashCode()
    {
      return this.BottomLeft.GetHashCode() ^ this.DirectionX.GetHashCode() ^ this.DirectionY.GetHashCode();
    }
  }
}
