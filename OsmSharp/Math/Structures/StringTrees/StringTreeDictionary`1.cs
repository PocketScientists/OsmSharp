namespace OsmSharp.Math.Structures.StringTrees
{
  public class StringTreeDictionary<T>
  {
    private StringTreeDictionary<T>.StringTreeNode _root;

    public StringTreeDictionary()
    {
      this._root = new StringTreeDictionary<T>.StringTreeNode('㿿');
    }

    public void Add(string key, T value)
    {
      this._root.Add((short) 0, key, value);
    }

    public T SearchExact(string key)
    {
      if (this._root != null)
        return this._root.SearchExact((short) 0, key);
      return default (T);
    }

    private class StringTreeNode
    {
      private StringTreeDictionary<T>.StringTreeNode _lower_node;
      private StringTreeDictionary<T>.StringTreeNode _equal_node;
      private StringTreeDictionary<T>.StringTreeNode _higher_node;
      private char _split_char;
      private T _value;

      public StringTreeNode(char split_char)
      {
        this._split_char = split_char;
      }

      internal void Add(short idx, string key, T value)
      {
        char ch = key[(int) idx];
        if ((int) ch < (int) this._split_char)
        {
          if (this._lower_node == null)
          {
            this._lower_node = new StringTreeDictionary<T>.StringTreeNode(key[(int) idx]);
            this._lower_node.Add(idx, key, value);
          }
          else
            this._lower_node.Add(idx, key, value);
        }
        else if ((int) ch == (int) this._split_char)
        {
          if ((int) idx < key.Length - 1)
          {
            if (this._equal_node == null)
              this._equal_node = new StringTreeDictionary<T>.StringTreeNode(key[(int) idx + 1]);
            this._equal_node.Add((short) ((int) idx + 1), key, value);
          }
          else
            this._value = value;
        }
        else if (this._higher_node == null)
        {
          this._higher_node = new StringTreeDictionary<T>.StringTreeNode(key[(int) idx]);
          this._higher_node.Add(idx, key, value);
        }
        else
          this._higher_node.Add(idx, key, value);
      }

      internal T SearchExact(short idx, string key)
      {
        char ch = key[(int) idx];
        if ((int) ch < (int) this._split_char)
        {
          if (this._lower_node != null)
            return this._lower_node.SearchExact(idx, key);
        }
        else if ((int) ch == (int) this._split_char)
        {
          if ((int) idx >= key.Length - 1)
            return this._value;
          if (this._equal_node != null)
            return this._equal_node.SearchExact((short) ((int) idx + 1), key);
        }
        else if (this._higher_node != null)
          return this._higher_node.SearchExact(idx, key);
        return default (T);
      }
    }
  }
}
