using OsmSharp.Geo;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graphs.Geometric.Shapes
{
  public abstract class ShapeBase : IEnumerable<ICoordinate>, IEnumerable
  {
    public abstract int Count { get; }

    public abstract ICoordinate this[int i] { get; }

    public abstract ShapeBase Reverse();

    public IEnumerator<ICoordinate> GetEnumerator()
    {
      return (IEnumerator<ICoordinate>) new ShapeBase.ShapeBaseEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new ShapeBase.ShapeBaseEnumerator(this);
    }

    private struct ShapeBaseEnumerator : IEnumerator<ICoordinate>, IEnumerator, IDisposable
    {
      private readonly ShapeBase _shape;
      private int _i;

      public ICoordinate Current
      {
        get
        {
          return this._shape[this._i];
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public ShapeBaseEnumerator(ShapeBase shape)
      {
        this._shape = shape;
        this._i = -1;
      }

      public bool MoveNext()
      {
        this._i = this._i + 1;
        return this._shape.Count > this._i;
      }

      public void Reset()
      {
        this._i = -1;
      }

      public void Dispose()
      {
      }
    }
  }
}
