using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Complete
{
  public abstract class OsmCompleteStreamSource : IEnumerable<ICompleteOsmGeo>, IEnumerable, IEnumerator<ICompleteOsmGeo>, IEnumerator, IDisposable
  {
    public abstract bool CanReset { get; }

    ICompleteOsmGeo IEnumerator<ICompleteOsmGeo>.Current
    {
      get
      {
        return this.Current();
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.Current();
      }
    }

    public abstract void Initialize();

    public abstract bool MoveNext();

    public abstract ICompleteOsmGeo Current();

    public abstract void Reset();

    public IEnumerator<ICompleteOsmGeo> GetEnumerator()
    {
      this.Initialize();
      return (IEnumerator<ICompleteOsmGeo>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this.Initialize();
      return (IEnumerator) this;
    }

    public virtual void Dispose()
    {
    }
  }
}
