using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Math.Primitives.Enumerators.Points
{
  public sealed class PointEnumerable : IEnumerable<PointF2D>, IEnumerable
  {
    private PointEnumerator _enumerator;

    internal PointEnumerable(PointEnumerator enumerator)
    {
      this._enumerator = enumerator;
    }

    public IEnumerator<PointF2D> GetEnumerator()
    {
      return (IEnumerator<PointF2D>) this._enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._enumerator;
    }
  }
}
