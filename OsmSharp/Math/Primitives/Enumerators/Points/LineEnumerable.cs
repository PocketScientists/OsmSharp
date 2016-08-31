using OsmSharp.Math.Primitives.Enumerators.Lines;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Math.Primitives.Enumerators.Points
{
  public sealed class LineEnumerable : IEnumerable<LineF2D>, IEnumerable
  {
    private LineEnumerator _enumerator;

    internal LineEnumerable(LineEnumerator enumerator)
    {
      this._enumerator = enumerator;
    }

    public IEnumerator<LineF2D> GetEnumerator()
    {
      return (IEnumerator<LineF2D>) this._enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._enumerator;
    }
  }
}
