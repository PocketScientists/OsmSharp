using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Math.Primitives.Enumerators.Points
{
  internal class PointEnumerator : IEnumerator<PointF2D>, IEnumerator, IDisposable
  {
    private IPointList _enumerable;
    private PointF2D _current_point;
    private int _current_idx;

    public PointF2D Current
    {
      get
      {
        return this._current_point;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this._current_point;
      }
    }

    public PointEnumerator(IPointList enumerable)
    {
      this._enumerable = enumerable;
    }

    public void Dispose()
    {
      this._current_point = (PointF2D) null;
    }

    public bool MoveNext()
    {
      this._current_idx = this._current_idx + 1;
      if (this._enumerable.Count <= this._current_idx)
        return false;
      this._current_point = this._enumerable[this._current_idx];
      return true;
    }

    public void Reset()
    {
      this._current_idx = this._current_idx - 1;
      this._current_point = (PointF2D) null;
    }

    bool IEnumerator.MoveNext()
    {
      return this.MoveNext();
    }

    void IEnumerator.Reset()
    {
      this.Reset();
    }
  }
}
