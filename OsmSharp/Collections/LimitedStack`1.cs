using System.Collections.Generic;

namespace OsmSharp.Collections
{
  public class LimitedStack<T>
  {
    private List<T> _elements;
    private int _limit;

    public int Count
    {
      get
      {
        lock (this._elements)
          return this._elements.Count;
      }
    }

    public int Limit
    {
      get
      {
        lock (this._elements)
          return this._limit;
      }
      set
      {
        lock (this._elements)
        {
          this._limit = value;
          if (this._elements.Count <= this._limit)
            return;
          this._elements.RemoveRange(0, this._elements.Count - this._limit);
        }
      }
    }

    public LimitedStack()
    {
      this._limit = 10;
      this._elements = new List<T>();
    }

    public LimitedStack(IEnumerable<T> collection)
    {
      this._limit = 10;
      this._elements = new List<T>(collection);
      if (this._elements.Count <= this._limit)
        return;
      this._elements.RemoveRange(0, this._elements.Count - this._limit);
    }

    public LimitedStack(int capacity)
    {
      this._limit = 10;
      this._elements = new List<T>(capacity > this._limit ? this._limit : capacity);
    }

    public LimitedStack(int capacity, int limit)
    {
      this._limit = limit;
      this._elements = new List<T>(capacity > this._limit ? this._limit : capacity);
    }

    public void Clear()
    {
      lock (this._elements)
        this._elements.Clear();
    }

    public bool Contains(T item)
    {
      lock (this._elements)
        return this._elements.Contains(item);
    }

    public T Pop()
    {
      lock (this._elements)
      {
        T temp_12 = this._elements[this._elements.Count - 1];
        this._elements.RemoveAt(this._elements.Count - 1);
        return temp_12;
      }
    }

    public void Push(T item)
    {
      lock (this._elements)
      {
        if (this._elements.Count == this._limit)
          this._elements.RemoveAt(0);
        this._elements.Add(item);
      }
    }

    public void PushToTop(T item)
    {
      lock (this._elements)
      {
        if (this._elements.Contains(item))
          this._elements.Remove(item);
        this.Push(item);
      }
    }

    public T Peek()
    {
      lock (this._elements)
        return this._elements[this._elements.Count - 1];
    }
  }
}
