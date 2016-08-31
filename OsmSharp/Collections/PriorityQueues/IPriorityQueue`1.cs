using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections.PriorityQueues
{
  public interface IPriorityQueue<T> : IEnumerable<T>, IEnumerable
  {
    int Count { get; }

    void Push(T item, float priority);

    float PeekWeight();

    T Peek();

    T Pop();
  }
}
