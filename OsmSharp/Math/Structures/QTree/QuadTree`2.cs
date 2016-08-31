using OsmSharp.Math.Primitives;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Structures.QTree
{
  public class QuadTree<TPointType, TDataType> : ILocatedObjectIndex<TPointType, TDataType> where TPointType : PointF2D
  {
    private QuadTree<TPointType, TDataType>.QuadTreeNode _root;

    public QuadTree()
    {
      this._root = (QuadTree<TPointType, TDataType>.QuadTreeNode) null;
    }

    public QuadTree(int dept, BoxF2D bounds)
    {
      this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(dept, bounds);
    }

    public QuadTree(int dept, double min0, double min1, double max0, double max1)
    {
      this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(dept, min0, min1, max0, max1);
    }

    private QuadTree<TPointType, TDataType>.QuadTreeNode GetOrCreateAt(TPointType point)
    {
      if (this._root == null)
        this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(0, point[0] - 0.01, point[1] - 0.01, point[0] + 0.01, point[1] + 0.01);
      if (this._root.IsInsideBox(point))
        return this._root.GetOrCreateAt(point);
      if (point[0] < this._root.Min0)
      {
        if (point[1] < this._root.Min1)
        {
          this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(true, true, this._root);
          return this.GetOrCreateAt(point);
        }
        this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(true, false, this._root);
        return this.GetOrCreateAt(point);
      }
      if (point[0] > this._root.Max0)
      {
        if (point[1] < this._root.Min1)
        {
          this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(false, true, this._root);
          return this.GetOrCreateAt(point);
        }
        this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(false, false, this._root);
        return this.GetOrCreateAt(point);
      }
      if (point[1] < this._root.Min1)
      {
        this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(false, true, this._root);
        return this.GetOrCreateAt(point);
      }
      if (point[1] <= this._root.Max1)
        throw new Exception("The point is not in the route but not outside of any bound!?");
      this._root = new QuadTree<TPointType, TDataType>.QuadTreeNode(false, false, this._root);
      return this.GetOrCreateAt(point);
    }

    public IEnumerable<TDataType> GetInside(BoxF2D box)
    {
      if (this._root == null)
        return (IEnumerable<TDataType>) new List<TDataType>();
      List<TDataType> dataTypeList = new List<TDataType>();
      this._root.AddInsideAtNode((IList<TDataType>) dataTypeList, this._root, box);
      return (IEnumerable<TDataType>) dataTypeList;
    }

    public void Add(TPointType location, TDataType data)
    {
      this.GetOrCreateAt(location).AddData(location, data);
    }

    public void Clear()
    {
      this._root = (QuadTree<TPointType, TDataType>.QuadTreeNode) null;
    }

    private class QuadTreeNode
    {
      private QuadTree<TPointType, TDataType>.QuadTreeNode _minMin;
      private QuadTree<TPointType, TDataType>.QuadTreeNode _minMax;
      private QuadTree<TPointType, TDataType>.QuadTreeNode _maxMin;
      private QuadTree<TPointType, TDataType>.QuadTreeNode _maxMax;
      private readonly BoxF2D _bounds;
      private readonly double _middle0;
      private readonly double _middle1;
      private readonly int _depth;
      private readonly List<KeyValuePair<TPointType, TDataType>> _data;

      public int Depth
      {
        get
        {
          return this._depth;
        }
      }

      public double Min0
      {
        get
        {
          return this._bounds.Min[0];
        }
      }

      public double Max0
      {
        get
        {
          return this._bounds.Max[0];
        }
      }

      public double Min1
      {
        get
        {
          return this._bounds.Min[1];
        }
      }

      public double Max1
      {
        get
        {
          return this._bounds.Max[1];
        }
      }

      public QuadTree<TPointType, TDataType>.QuadTreeNode MinMin
      {
        get
        {
          return this._minMin;
        }
      }

      public QuadTree<TPointType, TDataType>.QuadTreeNode MinMax
      {
        get
        {
          return this._minMax;
        }
      }

      public QuadTree<TPointType, TDataType>.QuadTreeNode MaxMin
      {
        get
        {
          return this._maxMin;
        }
      }

      public QuadTree<TPointType, TDataType>.QuadTreeNode MaxMax
      {
        get
        {
          return this._maxMax;
        }
      }

      public QuadTreeNode(int dept, BoxF2D bounds)
      {
        this._depth = dept;
        this._bounds = new BoxF2D(new PointF2D(bounds.Max[0], bounds.Max[1]), new PointF2D(bounds.Min[0], bounds.Min[1]));
        this._middle0 = (bounds.Min[0] + bounds.Max[0]) / 2.0;
        this._middle1 = (bounds.Min[1] + bounds.Max[1]) / 2.0;
        if (this._depth != 0)
          return;
        this._data = new List<KeyValuePair<TPointType, TDataType>>();
      }

      public QuadTreeNode(int dept, double min0, double min1, double max0, double max1)
      {
        this._depth = dept;
        this._bounds = new BoxF2D(new PointF2D(max0, max1), new PointF2D(min0, min1));
        this._middle0 = (min0 + max0) / 2.0;
        this._middle1 = (min1 + max1) / 2.0;
        if (this._depth != 0)
          return;
        this._data = new List<KeyValuePair<TPointType, TDataType>>();
      }

      public QuadTreeNode(bool min0, bool min1, QuadTree<TPointType, TDataType>.QuadTreeNode node)
      {
        this._depth = node.Depth + 1;
        double num1 = node.Max0 - node.Min0;
        double num2 = node.Max1 - node.Min1;
        double x1;
        double x2;
        double y1;
        double y2;
        if (min0)
        {
          x1 = node.Min0 - num1;
          x2 = node.Max0;
          if (min1)
          {
            y1 = node.Min1 - num2;
            y2 = node.Max1;
            this._maxMax = node;
          }
          else
          {
            y1 = node.Min1;
            y2 = node.Max1 + num2;
            this._maxMin = node;
          }
        }
        else
        {
          if (min1)
          {
            y1 = node.Min1 - num2;
            y2 = node.Max1;
            this._minMax = node;
          }
          else
          {
            y1 = node.Min1;
            y2 = node.Max1 + num2;
            this._minMin = node;
          }
          x1 = node.Min0;
          x2 = node.Max0 + num1;
        }
        this._middle0 = (x1 + x2) / 2.0;
        this._middle1 = (y1 + y2) / 2.0;
        this._bounds = new BoxF2D(new PointF2D(x2, y2), new PointF2D(x1, y1));
        if (this._depth != 0)
          return;
        this._data = new List<KeyValuePair<TPointType, TDataType>>();
      }

      public QuadTree<TPointType, TDataType>.QuadTreeNode GetOrCreateAt(TPointType point)
      {
        if (this._depth == 0)
          return this;
        if (this._middle0 > point[0])
        {
          if (this._middle1 > point[1])
          {
            if (this._minMin == null)
              this._minMin = new QuadTree<TPointType, TDataType>.QuadTreeNode(this._depth - 1, this.Min0, this.Min1, this._middle0, this._middle1);
            return this._minMin.GetOrCreateAt(point);
          }
          if (this._minMax == null)
            this._minMax = new QuadTree<TPointType, TDataType>.QuadTreeNode(this._depth - 1, this.Min0, this._middle1, this._middle0, this.Max1);
          return this._minMax.GetOrCreateAt(point);
        }
        if (this._middle1 > point[1])
        {
          if (this._maxMin == null)
            this._maxMin = new QuadTree<TPointType, TDataType>.QuadTreeNode(this._depth - 1, this._middle0, this.Min1, this.Max0, this._middle1);
          return this._maxMin.GetOrCreateAt(point);
        }
        if (this._maxMax == null)
          this._maxMax = new QuadTree<TPointType, TDataType>.QuadTreeNode(this._depth - 1, this._middle0, this._middle1, this.Max0, this.Max1);
        return this._maxMax.GetOrCreateAt(point);
      }

      public void AddInsideAtNode(IList<TDataType> data, QuadTree<TPointType, TDataType>.QuadTreeNode node, BoxF2D box)
      {
        if (!box.Overlaps(this._bounds))
          return;
        if (this._depth > 0)
        {
          if (this._minMin != null)
            this._minMin.AddInsideAtNode(data, node, box);
          if (this._minMax != null)
            this._minMax.AddInsideAtNode(data, node, box);
          if (this._maxMin != null)
            this._maxMin.AddInsideAtNode(data, node, box);
          if (this._maxMax == null)
            return;
          this._maxMax.AddInsideAtNode(data, node, box);
        }
        else
        {
          foreach (KeyValuePair<TPointType, TDataType> keyValuePair in this._data)
          {
            if (box.Contains((PointF2D) keyValuePair.Key))
              data.Add(keyValuePair.Value);
          }
        }
      }

      internal void AddData(TPointType point, TDataType data)
      {
        if (this._depth > 0)
          throw new Exception("Cannot add data to a non-leaf node!");
        this._data.Add(new KeyValuePair<TPointType, TDataType>(point, data));
      }

      internal bool IsInsideBox(TPointType point)
      {
        return this._bounds.Contains((PointF2D) point);
      }
    }
  }
}
