using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections.PriorityQueues
{
  public class BinaryHeap<T> : IPriorityQueue<T>, IEnumerable<T>, IEnumerable
  {
    private T[] _heap;
    private float[] _priorities;
    private int _count;
    private uint _latest_index;

    public int Count
    {
      get
      {
        return this._count;
      }
    }

    public BinaryHeap()
      : this(2U)
    {
    }

    public BinaryHeap(uint initialSize)
    {
      this._heap = new T[(int) initialSize];
      this._priorities = new float[(int) initialSize];
      this._count = 0;
      this._latest_index = 1U;
    }

    public void Push(T item, float priority)
    {
      this._count = this._count + 1;
      if ((long) this._latest_index == (long) (this._priorities.Length - 1))
      {
        Array.Resize<T>(ref this._heap, this._heap.Length + 100);
        Array.Resize<float>(ref this._priorities, this._priorities.Length + 100);
      }
      this._priorities[(int) this._latest_index] = priority;
      this._heap[(int) this._latest_index] = item;
      uint num1 = this._latest_index;
      this._latest_index = this._latest_index + 1U;
      uint num2;
      for (; (int) num1 != 1; num1 = num2)
      {
        num2 = num1 / 2U;
        if ((double) this._priorities[(int) num1] >= (double) this._priorities[(int) num2])
          break;
        float priority1 = this._priorities[(int) num2];
        T obj = this._heap[(int) num2];
        this._priorities[(int) num2] = this._priorities[(int) num1];
        this._heap[(int) num2] = this._heap[(int) num1];
        this._priorities[(int) num1] = priority1;
        this._heap[(int) num1] = obj;
      }
    }

    public float PeekWeight()
    {
      return this._priorities[1];
    }

    public T Peek()
    {
      return this._heap[1];
    }

    public T Pop()
    {
      if (this._count <= 0)
        return default (T);
      T obj1 = this._heap[1];
      this._count = this._count - 1;
      this._latest_index = this._latest_index - 1U;
      int index1 = 1;
      float priority1 = this._priorities[(int) this._latest_index];
      T obj2 = this._heap[(int) this._latest_index];
      this._heap[1] = obj2;
      this._priorities[1] = priority1;
      while (true)
      {
        int index2 = index1;
        float num;
        if ((long) (2 * index2 + 1) <= (long) this._latest_index)
        {
          num = this._priorities[2 * index2];
          float priority2 = this._priorities[2 * index2 + 1];
          if ((double) priority1 >= (double) num)
          {
            index1 = 2 * index2;
            if ((double) this._priorities[index1] >= (double) priority2)
            {
              num = priority2;
              index1 = 2 * index2 + 1;
            }
          }
          else if ((double) priority1 >= (double) priority2)
          {
            num = priority2;
            index1 = 2 * index2 + 1;
          }
          else
            break;
        }
        else if ((long) (2 * index2) <= (long) this._latest_index)
        {
          num = this._priorities[2 * index2];
          if ((double) priority1 >= (double) num)
            index1 = 2 * index2;
          else
            break;
        }
        else
          break;
        this._priorities[index2] = num;
        this._priorities[index1] = priority1;
        this._heap[index2] = this._heap[index1];
        this._heap[index1] = obj2;
      }
      return obj1;
    }

    public void Clear()
    {
      this._count = 0;
      this._latest_index = 1U;
    }

    public IEnumerator<T> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}
