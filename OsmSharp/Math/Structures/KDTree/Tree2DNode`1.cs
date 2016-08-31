using OsmSharp.Math.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.Structures.KDTree
{
  internal class Tree2DNode<PointType> where PointType : PointF2D
  {
    private PointType _value;
    private int _dimension;
    private Tree2DNode<PointType> _lesser;
    private Tree2DNode<PointType> _bigger;
    private Tree2D<PointType>.Distance _distance_delegate;

    public Tree2DNode(Tree2D<PointType>.Distance distance_delegate, PointType value, int dimension)
    {
      this._distance_delegate = distance_delegate;
      this._value = value;
      this._dimension = dimension;
    }

    public Tree2DNode(Tree2D<PointType>.Distance distance_delegate, List<PointType>[] sorted_points, int dimension)
    {
      this._distance_delegate = distance_delegate;
      this._dimension = dimension;
      List<PointType> sortedPoint = sorted_points[this._dimension];
      int index = sortedPoint.Count / 2;
      this._value = sortedPoint[index];
      List<PointType>[] sorted_points1 = new List<PointType>[2];
      List<PointType>[] sorted_points2 = new List<PointType>[2];
      sorted_points1[this._dimension] = new List<PointType>((IEnumerable<PointType>) sortedPoint.GetRange(0, index - 1));
      sorted_points2[this._dimension] = new List<PointType>((IEnumerable<PointType>) sortedPoint.GetRange(index + 1, sortedPoint.Count - (index + 1)));
      int dimension1 = (this._dimension + 1) % 2;
      sorted_points1[dimension1] = new List<PointType>(sorted_points[dimension1].Except<PointType>((IEnumerable<PointType>) sorted_points2[this._dimension]));
      sorted_points1[dimension1].Remove(this._value);
      sorted_points2[dimension1] = new List<PointType>(sorted_points[dimension1].Except<PointType>((IEnumerable<PointType>) sorted_points1[this._dimension]));
      sorted_points2[dimension1].Remove(this._value);
      if (sorted_points1[dimension1].Count == 1)
        this._lesser = new Tree2DNode<PointType>(this._distance_delegate, sorted_points1[dimension1][0], dimension1);
      else if (sorted_points1[dimension1].Count > 1)
        this._lesser = new Tree2DNode<PointType>(this._distance_delegate, sorted_points1, dimension1);
      if (sorted_points2[dimension1].Count == 1)
      {
        this._bigger = new Tree2DNode<PointType>(this._distance_delegate, sorted_points2[dimension1][0], dimension1);
      }
      else
      {
        if (sorted_points2[dimension1].Count <= 1)
          return;
        this._bigger = new Tree2DNode<PointType>(this._distance_delegate, sorted_points2, dimension1);
      }
    }

    public void Add(PointType value)
    {
      if (value[this._dimension] < this._value[this._dimension])
      {
        if (this._lesser == null)
          this._lesser = new Tree2DNode<PointType>(this._distance_delegate, value, this._dimension + 1);
        else
          this._lesser.Add(value);
      }
      else if (this._bigger == null)
        this._bigger = new Tree2DNode<PointType>(this._distance_delegate, value, this._dimension + 1);
      else
        this._bigger.Add(value);
    }

    public PointType SearchNearestNeighbour(PointType point, ICollection<PointType> exceptions)
    {
      PointType x1 = default (PointType);
      double num1 = point[this._dimension];
      bool flag = true;
      double num2 = this._value[this._dimension];
      if (num1 < num2)
      {
        if (this._lesser == null)
        {
          if (exceptions == null || !exceptions.Contains(this._value))
            x1 = this._value;
        }
        else
          x1 = this._lesser.SearchNearestNeighbour(point, exceptions);
      }
      else
      {
        flag = false;
        if (this._bigger == null)
        {
          if (exceptions == null || !exceptions.Contains(this._value))
            x1 = this._value;
        }
        else
          x1 = this._bigger.SearchNearestNeighbour(point, exceptions);
      }
      double num3 = double.MaxValue;
      if ((PointF2D) x1 != (PointF2D) null)
      {
        double num4 = this._distance_delegate(x1, point);
      }
      double num5 = this._distance_delegate(this._value, point);
      if (num3 > num5)
      {
        x1 = this._value;
        num3 = num5;
      }
      double num6 = 0.0;
      if ((PointF2D) x1 != (PointF2D) null)
        System.Math.Abs(x1[this._dimension] - this._value[this._dimension]);
      if (num3 > num6)
      {
        PointType x2 = default (PointType);
        if (flag)
        {
          if (this._bigger != null)
            x2 = this._bigger.SearchNearestNeighbour(point, exceptions);
        }
        else if (this._lesser != null)
          x2 = this._lesser.SearchNearestNeighbour(point, exceptions);
        if ((PointF2D) x2 != (PointF2D) null && this._distance_delegate(x2, point) < num3)
          x1 = x2;
      }
      return x1;
    }
  }
}
