using OsmSharp.Math.Primitives;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Structures.KDTree
{
  public class Tree2D<PointType> where PointType : PointF2D
  {
    private Tree2DNode<PointType> _root;
    private Tree2D<PointType>.Distance _distance_delegate;

    public Tree2D(IEnumerable<PointType> points, Tree2D<PointType>.Distance distance_delegate)
    {
      this._distance_delegate = distance_delegate;
      List<PointType>[] sorted_points = new List<PointType>[2];
      int num;
      for (int dim = 0; dim < 2; dim = num + 1)
      {
        List<PointType> pointTypeList = new List<PointType>(points);
        pointTypeList.Sort((Comparison<PointType>) ((p1, p2) => p1[dim].CompareTo(p2[dim])));
        sorted_points[dim] = pointTypeList;
        num = dim;
      }
      this._root = new Tree2DNode<PointType>(this._distance_delegate, sorted_points, 0);
    }

    public void Add(PointType point)
    {
      this._root.Add(point);
    }

    public PointType SearchNearestNeighbour(PointType point)
    {
      return this._root.SearchNearestNeighbour(point, (ICollection<PointType>) null);
    }

    public PointType SearchNearestNeighbour(PointType point, ICollection<PointType> exceptions)
    {
      return this._root.SearchNearestNeighbour(point, exceptions);
    }

    public delegate double Distance(PointF2D x, PointF2D y);
  }
}
