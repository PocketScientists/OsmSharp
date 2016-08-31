using System;

namespace OsmSharp.Collections.PriorityQueues
{
  public class BinaryHeapULong<T>
  {
    private T[] _heap;
    private ulong[] _priorities;
    private int _count;
    private uint _latest_index;

    public int Count
    {
      get
      {
        return this._count;
      }
    }

    public BinaryHeapULong()
      : this(2U)
    {
    }

    public BinaryHeapULong(uint initialSize)
    {
      this._heap = new T[(int) initialSize];
      this._priorities = new ulong[(int) initialSize];
      this._count = 0;
      this._latest_index = 1U;
    }

    public void Push(T item, ulong priority)
    {
      this._count = this._count + 1;
      if ((long) this._latest_index == (long) (this._priorities.Length - 1))
      {
        Array.Resize<T>(ref this._heap, this._heap.Length + 100);
        Array.Resize<ulong>(ref this._priorities, this._priorities.Length + 100);
      }
      this._priorities[(int) this._latest_index] = priority;
      this._heap[(int) this._latest_index] = item;
      uint num1 = this._latest_index;
      this._latest_index = this._latest_index + 1U;
      uint num2;
      for (; (int) num1 != 1; num1 = num2)
      {
        num2 = num1 / 2U;
        if (this._priorities[(int) num1] >= this._priorities[(int) num2])
          break;
        ulong priority1 = this._priorities[(int) num2];
        T obj = this._heap[(int) num2];
        this._priorities[(int) num2] = this._priorities[(int) num1];
        this._heap[(int) num2] = this._heap[(int) num1];
        this._priorities[(int) num1] = priority1;
        this._heap[(int) num1] = obj;
      }
    }

    public ulong PeekWeight()
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
      this._heap[1] = this._heap[(int) this._latest_index];
      this._priorities[1] = this._priorities[(int) this._latest_index];
      int index1 = 1;
      int index2;
      do
      {
        index2 = index1;
        if ((long) (2 * index2 + 1) <= (long) this._latest_index)
        {
          if (this._priorities[index2] >= this._priorities[2 * index2])
            index1 = 2 * index2;
          if (this._priorities[index1] >= this._priorities[2 * index2 + 1])
            index1 = 2 * index2 + 1;
        }
        else if ((long) (2 * index2) <= (long) this._latest_index && this._priorities[index2] >= this._priorities[2 * index2])
          index1 = 2 * index2;
        if (index2 != index1)
        {
          ulong priority = this._priorities[index2];
          T obj2 = this._heap[index2];
          this._priorities[index2] = this._priorities[index1];
          this._heap[index2] = this._heap[index1];
          this._priorities[index1] = priority;
          this._heap[index1] = obj2;
        }
      }
      while (index2 != index1);
      return obj1;
    }

    public void Clear()
    {
      this._count = 0;
      this._latest_index = 1U;
    }
  }
}
