using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections
{
  internal class RBTree : IEnumerable, IEnumerable<RBTree.Node>
  {
    private RBTree.Node root;
    private object hlp;
    private uint version;

    public int Count
    {
      get
      {
        if (this.root != null)
          return (int) this.root.Size;
        return 0;
      }
    }

    public RBTree.Node this[int index]
    {
      get
      {
        if (index < 0 || index >= this.Count)
          throw new IndexOutOfRangeException("index");
        RBTree.Node node = this.root;
        while (node != null)
        {
          int num = node.left == null ? 0 : (int) node.left.Size;
          if (index == num)
            return node;
          if (index < num)
          {
            node = node.left;
          }
          else
          {
            index -= num + 1;
            node = node.right;
          }
        }
        throw new Exception("Internal Error: index calculation");
      }
    }

    public RBTree(object hlp)
    {
      this.hlp = hlp;
    }

    private static List<RBTree.Node> alloc_path()
    {
      return new List<RBTree.Node>();
    }

    private static void release_path(List<RBTree.Node> path)
    {
    }

    public void Clear()
    {
      this.root = (RBTree.Node) null;
      this.version = this.version + 1U;
    }

    public RBTree.Node Intern<T>(T key, RBTree.Node new_node)
    {
      if (this.root == null)
      {
        if (new_node == null)
          new_node = ((RBTree.INodeHelper<T>) this.hlp).CreateNode(key);
        this.root = new_node;
        this.root.IsBlack = true;
        this.version = this.version + 1U;
        return this.root;
      }
      List<RBTree.Node> path = RBTree.alloc_path();
      int key1 = this.find_key<T>(key, path);
      RBTree.Node node = path[path.Count - 1];
      if (node == null)
      {
        if (new_node == null)
          new_node = ((RBTree.INodeHelper<T>) this.hlp).CreateNode(key);
        node = this.do_insert(key1, new_node, path);
      }
      RBTree.release_path(path);
      return node;
    }

    public RBTree.Node Remove<T>(T key)
    {
      if (this.root == null)
        return (RBTree.Node) null;
      List<RBTree.Node> path = RBTree.alloc_path();
      int key1 = this.find_key<T>(key, path);
      RBTree.Node node = (RBTree.Node) null;
      if (key1 == 0)
        node = this.do_remove(path);
      RBTree.release_path(path);
      return node;
    }

    public RBTree.Node Lookup<T>(T key)
    {
      RBTree.INodeHelper<T> hlp = (RBTree.INodeHelper<T>) this.hlp;
      RBTree.Node node;
      int num;
      for (node = this.root; node != null; node = num < 0 ? node.left : node.right)
      {
        num = hlp.Compare(key, node);
        if (num == 0)
          break;
      }
      return node;
    }

    public void Bound<T>(T key, ref RBTree.Node lower, ref RBTree.Node upper)
    {
      RBTree.INodeHelper<T> hlp = (RBTree.INodeHelper<T>) this.hlp;
      int num;
      for (RBTree.Node node = this.root; node != null; node = num < 0 ? node.left : node.right)
      {
        num = hlp.Compare(key, node);
        if (num <= 0)
          upper = node;
        if (num >= 0)
          lower = node;
        if (num == 0)
          break;
      }
    }

    public RBTree.NodeEnumerator GetEnumerator()
    {
      return new RBTree.NodeEnumerator(this);
    }

    public RBTree.NodeEnumerator GetSuffixEnumerator<T>(T key)
    {
      Stack<RBTree.Node> init_pennants = new Stack<RBTree.Node>();
      RBTree.INodeHelper<T> hlp = (RBTree.INodeHelper<T>) this.hlp;
      int num;
      for (RBTree.Node node = this.root; node != null; node = num < 0 ? node.left : node.right)
      {
        num = hlp.Compare(key, node);
        if (num <= 0)
          init_pennants.Push(node);
        if (num == 0)
          break;
      }
      return new RBTree.NodeEnumerator(this, init_pennants);
    }

    IEnumerator<RBTree.Node> IEnumerable<RBTree.Node>.GetEnumerator()
    {
      return (IEnumerator<RBTree.Node>) this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private int find_key<T>(T key, List<RBTree.Node> path)
    {
      RBTree.INodeHelper<T> hlp = (RBTree.INodeHelper<T>) this.hlp;
      int num = 0;
      RBTree.Node node1 = this.root;
      if (path != null)
        path.Add(this.root);
      while (node1 != null)
      {
        num = hlp.Compare(key, node1);
        if (num == 0)
          return num;
        RBTree.Node node2;
        if (num < 0)
        {
          node2 = node1.right;
          node1 = node1.left;
        }
        else
        {
          node2 = node1.left;
          node1 = node1.right;
        }
        if (path != null)
        {
          path.Add(node2);
          path.Add(node1);
        }
      }
      return num;
    }

    private RBTree.Node do_insert(int in_tree_cmp, RBTree.Node current, List<RBTree.Node> path)
    {
      path[path.Count - 1] = current;
      RBTree.Node node = path[path.Count - 3];
      if (in_tree_cmp < 0)
        node.left = current;
      else
        node.right = current;
      int index = 0;
      while (index < path.Count - 2)
      {
        ++path[index].Size;
        index += 2;
      }
      if (!node.IsBlack)
        this.rebalance_insert(path);
      if (!this.root.IsBlack)
        throw new Exception("Internal error: root is not black");
      this.version = this.version + 1U;
      return current;
    }

    private RBTree.Node do_remove(List<RBTree.Node> path)
    {
      int index1 = path.Count - 1;
      RBTree.Node node = path[index1];
      if (node.left != null)
      {
        RBTree.Node other = RBTree.right_most(node.left, node.right, path);
        node.SwapValue(other);
        if (other.left != null)
        {
          RBTree.Node left = other.left;
          path.Add((RBTree.Node) null);
          path.Add(left);
          other.SwapValue(left);
        }
      }
      else if (node.right != null)
      {
        RBTree.Node right = node.right;
        path.Add((RBTree.Node) null);
        path.Add(right);
        node.SwapValue(right);
      }
      int index2 = path.Count - 1;
      RBTree.Node orig = path[index2];
      if ((int) orig.Size != 1)
        throw new Exception("Internal Error: red-black violation somewhere");
      path[index2] = (RBTree.Node) null;
      this.node_reparent(index2 == 0 ? (RBTree.Node) null : path[index2 - 2], orig, 0U, (RBTree.Node) null);
      int index3 = 0;
      while (index3 < path.Count - 2)
      {
        --path[index3].Size;
        index3 += 2;
      }
      if (orig.IsBlack)
      {
        orig.IsBlack = false;
        if (index2 != 0)
          this.rebalance_delete(path);
      }
      if (this.root != null && !this.root.IsBlack)
        throw new Exception("Internal Error: root is not black");
      this.version = this.version + 1U;
      return orig;
    }

    private void rebalance_insert(List<RBTree.Node> path)
    {
      int curpos = path.Count - 1;
      while (path[curpos - 3] != null && !path[curpos - 3].IsBlack)
      {
        path[curpos - 2].IsBlack = path[curpos - 3].IsBlack = true;
        curpos -= 4;
        if (curpos == 0)
          return;
        path[curpos].IsBlack = false;
        if (path[curpos - 2].IsBlack)
          return;
      }
      this.rebalance_insert__rotate_final(curpos, path);
    }

    private void rebalance_delete(List<RBTree.Node> path)
    {
      int curpos = path.Count - 1;
      do
      {
        RBTree.Node node = path[curpos - 1];
        if (!node.IsBlack)
        {
          curpos = this.ensure_sibling_black(curpos, path);
          node = path[curpos - 1];
        }
        if (node.left != null && !node.left.IsBlack || node.right != null && !node.right.IsBlack)
        {
          this.rebalance_delete__rotate_final(curpos, path);
          return;
        }
        node.IsBlack = false;
        curpos -= 2;
        if (curpos == 0)
          return;
      }
      while (path[curpos].IsBlack);
      path[curpos].IsBlack = true;
    }

    private void rebalance_insert__rotate_final(int curpos, List<RBTree.Node> path)
    {
      RBTree.Node node1 = path[curpos];
      RBTree.Node node2 = path[curpos - 2];
      RBTree.Node orig = path[curpos - 4];
      uint size = orig.Size;
      bool flag1 = node2 == orig.left;
      bool flag2 = node1 == node2.left;
      RBTree.Node updated;
      if (flag1 & flag2)
      {
        orig.left = node2.right;
        node2.right = orig;
        updated = node2;
      }
      else if (flag1 && !flag2)
      {
        orig.left = node1.right;
        node1.right = orig;
        node2.right = node1.left;
        node1.left = node2;
        updated = node1;
      }
      else if (!flag1 & flag2)
      {
        orig.right = node1.left;
        node1.left = orig;
        node2.left = node1.right;
        node1.right = node2;
        updated = node1;
      }
      else
      {
        orig.right = node2.left;
        node2.left = orig;
        updated = node2;
      }
      int num1 = (int) orig.FixSize();
      orig.IsBlack = false;
      if (updated != node2)
      {
        int num2 = (int) node2.FixSize();
      }
      updated.IsBlack = true;
      this.node_reparent(curpos == 4 ? (RBTree.Node) null : path[curpos - 6], orig, size, updated);
    }

    private void rebalance_delete__rotate_final(int curpos, List<RBTree.Node> path)
    {
      RBTree.Node node = path[curpos - 1];
      RBTree.Node orig = path[curpos - 2];
      uint size = orig.Size;
      bool isBlack = orig.IsBlack;
      RBTree.Node updated;
      if (orig.right == node)
      {
        if (node.right == null || node.right.IsBlack)
        {
          RBTree.Node left = node.left;
          orig.right = left.left;
          left.left = orig;
          node.left = left.right;
          left.right = node;
          updated = left;
        }
        else
        {
          orig.right = node.left;
          node.left = orig;
          node.right.IsBlack = true;
          updated = node;
        }
      }
      else if (node.left == null || node.left.IsBlack)
      {
        RBTree.Node right = node.right;
        orig.left = right.right;
        right.right = orig;
        node.right = right.left;
        right.left = node;
        updated = right;
      }
      else
      {
        orig.left = node.right;
        node.right = orig;
        node.left.IsBlack = true;
        updated = node;
      }
      int num1 = (int) orig.FixSize();
      orig.IsBlack = true;
      if (updated != node)
      {
        int num2 = (int) node.FixSize();
      }
      updated.IsBlack = isBlack;
      this.node_reparent(curpos == 2 ? (RBTree.Node) null : path[curpos - 4], orig, size, updated);
    }

    private int ensure_sibling_black(int curpos, List<RBTree.Node> path)
    {
      RBTree.Node node = path[curpos];
      RBTree.Node updated = path[curpos - 1];
      RBTree.Node orig = path[curpos - 2];
      uint size = orig.Size;
      bool flag;
      if (orig.right == updated)
      {
        orig.right = updated.left;
        updated.left = orig;
        flag = true;
      }
      else
      {
        orig.left = updated.right;
        updated.right = orig;
        flag = false;
      }
      int num = (int) orig.FixSize();
      orig.IsBlack = false;
      updated.IsBlack = true;
      this.node_reparent(curpos == 2 ? (RBTree.Node) null : path[curpos - 4], orig, size, updated);
      if (curpos + 1 == path.Count)
      {
        path.Add((RBTree.Node) null);
        path.Add((RBTree.Node) null);
      }
      path[curpos - 2] = updated;
      path[curpos - 1] = flag ? updated.right : updated.left;
      path[curpos] = orig;
      path[curpos + 1] = flag ? orig.right : orig.left;
      path[curpos + 2] = node;
      return curpos + 2;
    }

    private void node_reparent(RBTree.Node orig_parent, RBTree.Node orig, uint orig_size, RBTree.Node updated)
    {
      if (updated != null && (int) updated.FixSize() != (int) orig_size)
        throw new Exception("Internal error: rotation");
      if (orig == this.root)
        this.root = updated;
      else if (orig == orig_parent.left)
      {
        orig_parent.left = updated;
      }
      else
      {
        if (orig != orig_parent.right)
          throw new Exception("Internal error: path error");
        orig_parent.right = updated;
      }
    }

    private static RBTree.Node right_most(RBTree.Node current, RBTree.Node sibling, List<RBTree.Node> path)
    {
      while (true)
      {
        path.Add(sibling);
        path.Add(current);
        if (current.right != null)
        {
          sibling = current.left;
          current = current.right;
        }
        else
          break;
      }
      return current;
    }

    public interface INodeHelper<T>
    {
      int Compare(T key, RBTree.Node node);

      RBTree.Node CreateNode(T key);
    }

    public abstract class Node
    {
      public RBTree.Node left;
      public RBTree.Node right;
      private uint size_black;
      private const uint black_mask = 1;
      private const int black_shift = 1;

      public bool IsBlack
      {
        get
        {
          return ((int) this.size_black & 1) == 1;
        }
        set
        {
          this.size_black = value ? this.size_black | 1U : this.size_black & 4294967294U;
        }
      }

      public uint Size
      {
        get
        {
          return this.size_black >> 1;
        }
        set
        {
          this.size_black = (uint) ((int) value << 1 | (int) this.size_black & 1);
        }
      }

      public Node()
      {
        this.size_black = 2U;
      }

      public uint FixSize()
      {
        this.Size = 1U;
        if (this.left != null)
          this.Size = this.Size + this.left.Size;
        if (this.right != null)
          this.Size = this.Size + this.right.Size;
        return this.Size;
      }

      public abstract void SwapValue(RBTree.Node other);
    }

    public struct NodeEnumerator : IEnumerator, IEnumerator<RBTree.Node>, IDisposable
    {
      private RBTree tree;
      private uint version;
      private Stack<RBTree.Node> pennants;
      private Stack<RBTree.Node> init_pennants;

      public RBTree.Node Current
      {
        get
        {
          return this.pennants.Peek();
        }
      }

      object IEnumerator.Current
      {
        get
        {
          this.check_current();
          return (object) this.Current;
        }
      }

      internal NodeEnumerator(RBTree tree)
      {
        this = new RBTree.NodeEnumerator();
        this.tree = tree;
        this.version = tree.version;
      }

      internal NodeEnumerator(RBTree tree, Stack<RBTree.Node> init_pennants)
      {
        this = new RBTree.NodeEnumerator(tree);
        this.init_pennants = init_pennants;
      }

      public void Reset()
      {
        this.check_version();
        this.pennants = (Stack<RBTree.Node>) null;
      }

      public bool MoveNext()
      {
        this.check_version();
        RBTree.Node node;
        if (this.pennants == null)
        {
          if (this.tree.root == null)
            return false;
          if (this.init_pennants != null)
          {
            this.pennants = this.init_pennants;
            this.init_pennants = (Stack<RBTree.Node>) null;
            return (uint) this.pennants.Count > 0U;
          }
          this.pennants = new Stack<RBTree.Node>();
          node = this.tree.root;
        }
        else
        {
          if (this.pennants.Count == 0)
            return false;
          node = this.pennants.Pop().right;
        }
        for (; node != null; node = node.left)
          this.pennants.Push(node);
        return (uint) this.pennants.Count > 0U;
      }

      public void Dispose()
      {
        this.tree = (RBTree) null;
        this.pennants = (Stack<RBTree.Node>) null;
      }

      private void check_version()
      {
        if (this.tree == null)
          throw new ObjectDisposedException("enumerator");
        if ((int) this.version != (int) this.tree.version)
          throw new InvalidOperationException("tree modified");
      }

      internal void check_current()
      {
        this.check_version();
        if (this.pennants == null)
          throw new InvalidOperationException("state invalid before the first MoveNext()");
      }
    }
  }
}
