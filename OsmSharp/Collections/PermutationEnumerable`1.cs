using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections
{
  public class PermutationEnumerable<T> : IEnumerable<T[]>, IEnumerable
  {
    private T[] _sequence;

    public PermutationEnumerable(T[] sequence)
    {
      this._sequence = sequence;
    }

    public IEnumerator<T[]> GetEnumerator()
    {
      return (IEnumerator<T[]>) new PermutationEnumerator<T>(this._sequence);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new PermutationEnumerator<T>(this._sequence);
    }
  }
}
