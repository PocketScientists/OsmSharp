using OsmSharp.Collections.Tags;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams
{
  public abstract class OsmStreamSource : IEnumerable<OsmGeo>, IEnumerable, IEnumerator<OsmGeo>, IEnumerator, IDisposable
  {
    private readonly TagsCollectionBase _meta;

    public abstract bool CanReset { get; }

    public virtual bool IsSorted
    {
      get
      {
        return false;
      }
    }

    public TagsCollectionBase Meta
    {
      get
      {
        return this._meta;
      }
    }

    OsmGeo IEnumerator<OsmGeo>.Current
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

    protected OsmStreamSource()
    {
      this._meta = (TagsCollectionBase) new TagsCollection();
    }

    public abstract void Initialize();

    public bool MoveNext()
    {
      return this.MoveNext(false, false, false);
    }

    public bool MoveNextNode()
    {
      return this.MoveNext(false, true, true);
    }

    public bool MoveNextWay()
    {
      return this.MoveNext(true, false, true);
    }

    public bool MoveNextRelation()
    {
      return this.MoveNext(true, true, false);
    }

    public abstract bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations);

    public abstract OsmGeo Current();

    public abstract void Reset();

    public virtual TagsCollection GetAllMeta()
    {
      return new TagsCollection((IEnumerable<Tag>) this._meta);
    }

    public IEnumerator<OsmGeo> GetEnumerator()
    {
      this.Initialize();
      return (IEnumerator<OsmGeo>) this;
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
