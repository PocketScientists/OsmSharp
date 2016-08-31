using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterMerge : OsmStreamFilter
  {
    private int _current = -1;
    private List<OsmStreamSource> _sources;

    public override bool CanReset
    {
      get
      {
        foreach (OsmStreamSource source in this._sources)
        {
          if (!source.CanReset)
            return false;
        }
        return true;
      }
    }

    public override bool IsSorted
    {
      get
      {
        return false;
      }
    }

    public OsmStreamFilterMerge()
    {
      this._sources = new List<OsmStreamSource>();
    }

    public override void RegisterSource(IEnumerable<OsmGeo> source)
    {
      this.RegisterSource(source.ToOsmStreamSource());
    }

    public override void RegisterSource(OsmStreamSource source)
    {
      this._sources.Add(source);
    }

    public override OsmGeo Current()
    {
      if (this._current < 0 || this._current > this._sources.Count)
        throw new InvalidOperationException("Cannot return a current object before moving to the first object.");
      return this._sources[this._current].Current();
    }

    public override void Initialize()
    {
      foreach (OsmStreamSource source in this._sources)
        source.Initialize();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      while (this.DoMoveNext())
      {
        if (this.Current().Type == OsmGeoType.Node && !ignoreNodes || this.Current().Type == OsmGeoType.Way && !ignoreWays || this.Current().Type == OsmGeoType.Relation && !ignoreRelations)
          return true;
      }
      return false;
    }

    private bool DoMoveNext()
    {
      if (this._current == -1)
        this._current = 0;
      for (bool flag = this._sources[this._current].MoveNext(); !flag; flag = this._sources[this._current].MoveNext())
      {
        this._current = this._current + 1;
        if (this._current >= this._sources.Count)
          return false;
      }
      return true;
    }

    public override TagsCollection GetAllMeta()
    {
      TagsCollection tagsCollection = new TagsCollection();
      foreach (OsmStreamSource source in this._sources)
        tagsCollection.AddOrReplace((TagsCollectionBase) source.GetAllMeta());
      tagsCollection.AddOrReplace((TagsCollectionBase) new TagsCollection((IEnumerable<Tag>) this.Meta));
      return tagsCollection;
    }

    public override void Reset()
    {
      this._current = -1;
      foreach (OsmStreamSource source in this._sources)
        source.Reset();
    }
  }
}
