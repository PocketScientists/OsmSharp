using OsmSharp.Math.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections.SpatialIndexes
{
  public class RTreeMemoryIndex<T> : ISpatialIndex<T>, ISpatialIndexReadonly<T>, IEnumerable<T>, IEnumerable
  {
    private readonly int _maxLeafSize = 200;
    private readonly int _minLeafSize = 100;
    private int _count;
    private RTreeMemoryIndex<T>.Node _root;

    public int Count
    {
      get
      {
        return this._count;
      }
    }

    internal RTreeMemoryIndex<T>.Node Root
    {
      get
      {
        return this._root;
      }
    }

    public RTreeMemoryIndex()
    {
    }

    public RTreeMemoryIndex(int minLeafSize, int maxLeafSize)
    {
      this._minLeafSize = minLeafSize;
      this._maxLeafSize = maxLeafSize;
    }

    public void Add(BoxF2D box, T item)
    {
      this._count = this._count + 1;
      if (this._root == null)
      {
        this._root = new RTreeMemoryIndex<T>.Node();
        this._root.Boxes = new List<BoxF2D>();
        this._root.Children = (IList) new List<T>();
      }
      RTreeMemoryIndex<T>.Node node = RTreeMemoryIndex<T>.Add(RTreeMemoryIndex<T>.ChooseLeaf(this._root, box), box, item, this._minLeafSize, this._maxLeafSize);
      if (node == null)
        return;
      this._root = node;
    }

    public void Remove(T item)
    {
      throw new NotSupportedException();
    }

    public void Remove(BoxF2D box, T item)
    {
      if (!RTreeMemoryIndex<T>.RemoveSimple(this._root, box, item))
        return;
      this._count = this._count - 1;
    }

    public IEnumerable<T> Get(BoxF2D box)
    {
      HashSet<T> result = new HashSet<T>();
      RTreeMemoryIndex<T>.Get(this._root, box, result);
      return (IEnumerable<T>) result;
    }

    public void GetCancel()
    {
    }

    private static void Get(RTreeMemoryIndex<T>.Node node, BoxF2D box, HashSet<T> result)
    {
      if (node.Children is List<RTreeMemoryIndex<T>.Node>)
      {
        List<RTreeMemoryIndex<T>.Node> children = node.Children as List<RTreeMemoryIndex<T>.Node>;
        for (int index = 0; index < children.Count; ++index)
        {
          if (box.Overlaps(node.Boxes[index]))
          {
            if (box.Contains(node.Boxes[index]))
              RTreeMemoryIndex<T>.GetAll(children[index], result);
            else
              RTreeMemoryIndex<T>.Get(children[index], box, result);
          }
        }
      }
      else
      {
        List<T> children = node.Children as List<T>;
        if (children == null)
          return;
        for (int index = 0; index < node.Children.Count; ++index)
        {
          if (node.Boxes[index].Overlaps(box))
            result.Add(children[index]);
        }
      }
    }

    private static void GetAll(RTreeMemoryIndex<T>.Node node, HashSet<T> result)
    {
      if (node.Children is List<RTreeMemoryIndex<T>.Node>)
      {
        List<RTreeMemoryIndex<T>.Node> children = node.Children as List<RTreeMemoryIndex<T>.Node>;
        for (int index = 0; index < children.Count; ++index)
          RTreeMemoryIndex<T>.GetAll(children[index], result);
      }
      else
      {
        List<T> children = node.Children as List<T>;
        if (children == null)
          return;
        for (int index = 0; index < node.Children.Count; ++index)
          result.Add(children[index]);
      }
    }

    private static RTreeMemoryIndex<T>.Node Add(RTreeMemoryIndex<T>.Node leaf, BoxF2D box, T item, int minimumSize, int maximumSize)
    {
      if (box == null)
        throw new ArgumentNullException("box");
      if (leaf == null)
        throw new ArgumentNullException("leaf");
      RTreeMemoryIndex<T>.Node node1 = (RTreeMemoryIndex<T>.Node) null;
      if (leaf.Boxes.Count == maximumSize)
      {
        leaf.Boxes.Add(box);
        leaf.Children.Add((object) item);
        RTreeMemoryIndex<T>.Node[] nodeArray = RTreeMemoryIndex<T>.SplitNode(leaf, minimumSize);
        leaf.Boxes = nodeArray[0].Boxes;
        leaf.Children = nodeArray[0].Children;
        RTreeMemoryIndex<T>.SetParents(leaf);
        node1 = nodeArray[1];
      }
      else
      {
        leaf.Boxes.Add(box);
        leaf.Children.Add((object) item);
      }
      RTreeMemoryIndex<T>.Node child = leaf;
      RTreeMemoryIndex<T>.Node node2 = node1;
      RTreeMemoryIndex<T>.Node parent;
      for (; child.Parent != null; child = parent)
      {
        parent = child.Parent;
        RTreeMemoryIndex<T>.TightenFor(parent, child);
        if (node2 != null)
        {
          if (parent.Boxes.Count == maximumSize)
          {
            parent.Boxes.Add(node2.GetBox());
            parent.Children.Add((object) node2);
            RTreeMemoryIndex<T>.Node[] nodeArray = RTreeMemoryIndex<T>.SplitNode(parent, minimumSize);
            parent.Boxes = nodeArray[0].Boxes;
            parent.Children = nodeArray[0].Children;
            RTreeMemoryIndex<T>.SetParents(parent);
            node2 = nodeArray[1];
          }
          else
          {
            parent.Boxes.Add(node2.GetBox());
            parent.Children.Add((object) node2);
            node2.Parent = parent;
            node2 = (RTreeMemoryIndex<T>.Node) null;
          }
        }
      }
      if (node2 == null)
        return (RTreeMemoryIndex<T>.Node) null;
      RTreeMemoryIndex<T>.Node node3 = new RTreeMemoryIndex<T>.Node();
      node3.Boxes = new List<BoxF2D>();
      node3.Boxes.Add(child.GetBox());
      node3.Boxes.Add(node2.GetBox());
      node3.Children = (IList) new List<RTreeMemoryIndex<T>.Node>();
      node3.Children.Add((object) child);
      child.Parent = node3;
      node3.Children.Add((object) node2);
      node2.Parent = node3;
      return node3;
    }

    private static bool RemoveSimple(RTreeMemoryIndex<T>.Node node, BoxF2D box, T item)
    {
      if (node.Children is List<RTreeMemoryIndex<T>.Node>)
      {
        List<RTreeMemoryIndex<T>.Node> children = node.Children as List<RTreeMemoryIndex<T>.Node>;
        for (int index = 0; index < children.Count; ++index)
        {
          if (box.Overlaps(node.Boxes[index]) && RTreeMemoryIndex<T>.RemoveSimple(node.Children[index] as RTreeMemoryIndex<T>.Node, box, item))
            return true;
        }
      }
      else
      {
        List<T> children = node.Children as List<T>;
        if (children != null)
          return children.Remove(item);
      }
      return false;
    }

    private static void TightenFor(RTreeMemoryIndex<T>.Node parent, RTreeMemoryIndex<T>.Node child)
    {
      for (int index = 0; index < parent.Children.Count; ++index)
      {
        if (parent.Children[index] == child)
          parent.Boxes[index] = child.GetBox();
      }
    }

    private static RTreeMemoryIndex<T>.Node ChooseLeaf(RTreeMemoryIndex<T>.Node node, BoxF2D box)
    {
      if (box == null)
        throw new ArgumentNullException("box");
      if (node == null)
        throw new ArgumentNullException("node");
      RTreeMemoryIndex<T>.Node node1;
      for (; node.Children is List<RTreeMemoryIndex<T>.Node>; node = node1)
      {
        node1 = (RTreeMemoryIndex<T>.Node) null;
        BoxF2D boxF2D = (BoxF2D) null;
        double num1 = double.MaxValue;
        List<RTreeMemoryIndex<T>.Node> children = node.Children as List<RTreeMemoryIndex<T>.Node>;
        for (int index = 0; index < node.Boxes.Count; ++index)
        {
          double num2 = node.Boxes[index].Union(box).Surface - node.Boxes[index].Surface;
          if (num1 > num2)
          {
            num1 = num2;
            node1 = children[index];
            boxF2D = node.Boxes[index];
          }
          else if (boxF2D != null && num1 == num2 && node.Boxes[index].Surface < boxF2D.Surface)
          {
            node1 = children[index];
            boxF2D = node.Boxes[index];
          }
        }
        if (node1 == null)
          throw new Exception("Finding best child failed!");
      }
      return node;
    }

    private static void SetParents(RTreeMemoryIndex<T>.Node node)
    {
      if (!(node.Children is List<RTreeMemoryIndex<T>.Node>))
        return;
      List<RTreeMemoryIndex<T>.Node> children = node.Children as List<RTreeMemoryIndex<T>.Node>;
      for (int index = 0; index < node.Boxes.Count; ++index)
        children[index].Parent = node;
    }

    private static RTreeMemoryIndex<T>.Node[] SplitNode(RTreeMemoryIndex<T>.Node node, int minimumSize)
    {
      int num1 = node.Children is List<T> ? 1 : 0;
      RTreeMemoryIndex<T>.Node[] nodeArray = new RTreeMemoryIndex<T>.Node[2]
      {
        new RTreeMemoryIndex<T>.Node(),
        null
      };
      nodeArray[0].Boxes = new List<BoxF2D>();
      nodeArray[0].Children = num1 == 0 ? (IList) new List<RTreeMemoryIndex<T>.Node>() : (IList) new List<T>();
      nodeArray[1] = new RTreeMemoryIndex<T>.Node();
      nodeArray[1].Boxes = new List<BoxF2D>();
      nodeArray[1].Children = num1 == 0 ? (IList) new List<RTreeMemoryIndex<T>.Node>() : (IList) new List<T>();
      int[] numArray = RTreeMemoryIndex<T>.SelectSeeds(node.Boxes);
      nodeArray[0].Boxes.Add(node.Boxes[numArray[0]]);
      nodeArray[1].Boxes.Add(node.Boxes[numArray[1]]);
      nodeArray[0].Children.Add(node.Children[numArray[0]]);
      nodeArray[1].Children.Add(node.Children[numArray[1]]);
      BoxF2D[] nodeBoxes = new BoxF2D[2]
      {
        node.Boxes[numArray[0]],
        node.Boxes[numArray[1]]
      };
      node.Boxes.RemoveAt(numArray[0]);
      node.Boxes.RemoveAt(numArray[1]);
      node.Children.RemoveAt(numArray[0]);
      node.Children.RemoveAt(numArray[1]);
      while (node.Boxes.Count > 0)
      {
        if (nodeArray[0].Boxes.Count + node.Boxes.Count == minimumSize)
        {
          int num2 = 0;
          while (node.Boxes.Count > 0)
          {
            nodeBoxes[0] = nodeBoxes[0].Union(node.Boxes[0]);
            nodeArray[0].Boxes.Add(node.Boxes[0]);
            nodeArray[0].Children.Add(node.Children[0]);
            node.Boxes.RemoveAt(0);
            node.Children.RemoveAt(0);
            ++num2;
          }
        }
        else if (nodeArray[1].Boxes.Count + node.Boxes.Count == minimumSize)
        {
          int num2 = 0;
          while (node.Boxes.Count > 0)
          {
            nodeBoxes[1] = nodeBoxes[1].Union(node.Boxes[0]);
            nodeArray[1].Boxes.Add(node.Boxes[0]);
            nodeArray[1].Children.Add(node.Children[0]);
            node.Boxes.RemoveAt(0);
            node.Children.RemoveAt(0);
            ++num2;
          }
        }
        else
        {
          int nodeBoxIndex;
          int index = RTreeMemoryIndex<T>.PickNext(nodeBoxes, (IList<BoxF2D>) node.Boxes, out nodeBoxIndex);
          nodeBoxes[nodeBoxIndex] = nodeBoxes[nodeBoxIndex].Union(node.Boxes[index]);
          nodeArray[nodeBoxIndex].Boxes.Add(node.Boxes[index]);
          nodeArray[nodeBoxIndex].Children.Add(node.Children[index]);
          node.Boxes.RemoveAt(index);
          node.Children.RemoveAt(index);
        }
      }
      RTreeMemoryIndex<T>.SetParents(nodeArray[0]);
      RTreeMemoryIndex<T>.SetParents(nodeArray[1]);
      return nodeArray;
    }

    protected static int PickNext(BoxF2D[] nodeBoxes, IList<BoxF2D> boxes, out int nodeBoxIndex)
    {
      double num1 = double.MinValue;
      nodeBoxIndex = 0;
      int num2 = -1;
      for (int index = 0; index < boxes.Count; ++index)
      {
        BoxF2D box = boxes[index];
        double num3 = box.Union(nodeBoxes[0]).Surface - box.Surface;
        double num4 = box.Union(nodeBoxes[1]).Surface - box.Surface;
        double num5 = System.Math.Abs(num3 - num4);
        if (num1 < num5)
        {
          num1 = num5;
          nodeBoxIndex = num3 != num4 ? (num3 < num4 ? 0 : 1) : (nodeBoxes[0].Surface < nodeBoxes[1].Surface ? 0 : 1);
          num2 = index;
        }
      }
      return num2;
    }

    private static int[] SelectSeeds(List<BoxF2D> boxes)
    {
      if (boxes == null)
        throw new ArgumentNullException("boxes");
      if (boxes.Count < 2)
        throw new ArgumentException("Cannot select seeds from a list with less than two items.");
      int[] numArray = new int[2];
      double num1 = double.MinValue;
      for (int index1 = 0; index1 < boxes.Count; ++index1)
      {
        for (int index2 = 0; index2 < index1; ++index2)
        {
          double num2 = System.Math.Max(boxes[index1].Union(boxes[index2]).Surface - boxes[index1].Surface - boxes[index2].Surface, 0.0);
          if (num2 > num1)
          {
            num1 = num2;
            numArray[0] = index1;
            numArray[1] = index2;
          }
        }
      }
      return numArray;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) new RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator(this._root);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator(this._root);
    }

    internal class Node
    {
      public List<BoxF2D> Boxes { get; set; }

      public IList Children { get; set; }

      public RTreeMemoryIndex<T>.Node Parent { get; set; }

      public BoxF2D GetBox()
      {
        BoxF2D boxF2D = this.Boxes[0];
        for (int index = 1; index < this.Boxes.Count; ++index)
          boxF2D = boxF2D.Union(this.Boxes[index]);
        return boxF2D;
      }
    }

    internal class RTreeMemoryIndexEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
      private RTreeMemoryIndex<T>.Node _root;
      private RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition _current;

      public T Current
      {
        get
        {
          return (T) this._current.Node.Children[this._current.NodeIdx];
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public RTreeMemoryIndexEnumerator(RTreeMemoryIndex<T>.Node root)
      {
        this._root = root;
      }

      public void Dispose()
      {
        this._root = (RTreeMemoryIndex<T>.Node) null;
        this._current = (RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition) null;
      }

      public bool MoveNext()
      {
        if (this._current == null)
          this._current = new RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition()
          {
            Node = this._root,
            Parent = (RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition) null,
            NodeIdx = -1
          };
        this._current = RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.MoveNextFrom(this._current);
        return this._current != null;
      }

      private static RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition MoveNextFrom(RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition position)
      {
        for (++position.NodeIdx; position.Node.Children == null || position.Node.Children.Count <= position.NodeIdx || position.Node.Children[position.NodeIdx] is RTreeMemoryIndex<T>.Node; ++position.NodeIdx)
        {
          if (position.Node.Children != null && position.Node.Children.Count > position.NodeIdx && position.Node.Children[position.NodeIdx] is RTreeMemoryIndex<T>.Node)
          {
            RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition nodePosition = new RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition();
            nodePosition.Parent = position;
            nodePosition.NodeIdx = -1;
            RTreeMemoryIndex<T>.Node child = position.Node.Children[position.NodeIdx] as RTreeMemoryIndex<T>.Node;
            nodePosition.Node = child;
            position = nodePosition;
          }
          else
            position = position.Parent;
          if (position == null)
            break;
        }
        return position;
      }

      public void Reset()
      {
        this._current = (RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition) null;
      }

      private class NodePosition
      {
        public RTreeMemoryIndex<T>.RTreeMemoryIndexEnumerator.NodePosition Parent { get; set; }

        public RTreeMemoryIndex<T>.Node Node { get; set; }

        public int NodeIdx { get; set; }
      }
    }
  }
}
