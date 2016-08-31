using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Math.Primitives.Enumerators.Lines
{
  internal class LineEnumerator : IEnumerator<LineF2D>, IEnumerator, IDisposable
  {
    private ILineList _enumerable;
    private LineF2D _current_line;
    private int _current_idx;

    public LineF2D Current
    {
      get
      {
        return this._current_line;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this._current_line;
      }
    }

    public LineEnumerator(ILineList enumerable)
    {
      this._enumerable = enumerable;
    }

    public void Dispose()
    {
      this._current_line = (LineF2D) null;
    }

    public bool MoveNext()
    {
      this._current_idx = this._current_idx + 1;
      if (this._current_idx >= this._enumerable.Count)
        return false;
      this._current_line = this._enumerable[this._current_idx];
      return true;
    }

    public void Reset()
    {
      this._current_idx = -1;
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
