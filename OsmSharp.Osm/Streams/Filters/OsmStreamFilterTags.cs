using OsmSharp.Osm.Filters;
using System;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterTags : OsmStreamFilter
  {
    private readonly Filter _nodesFilter;
    private readonly Filter _waysFilter;
    private readonly bool _wayKeepNodes;
    private readonly Filter _relationsFilter;
    private readonly bool _relationKeepObjects;
    private OsmGeo _current;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public OsmStreamFilterTags(Filter nodesFilter, Filter waysFilter, Filter relationsFilter)
    {
      this._nodesFilter = nodesFilter;
      this._waysFilter = waysFilter;
      this._relationsFilter = relationsFilter;
      this._wayKeepNodes = false;
      this._relationKeepObjects = false;
    }

    public override void Initialize()
    {
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
      if (this._relationKeepObjects || this._wayKeepNodes)
        return false;
      while (this.Source.MoveNext())
      {
        OsmGeo osmGeo = this.Source.Current();
        switch (osmGeo.Type)
        {
          case OsmGeoType.Node:
            if (this._nodesFilter == null || this._nodesFilter.Evaluate(osmGeo))
            {
              this._current = osmGeo;
              return true;
            }
            continue;
          case OsmGeoType.Way:
            if (this._waysFilter == null || this._waysFilter.Evaluate(osmGeo))
            {
              this._current = osmGeo;
              return true;
            }
            continue;
          case OsmGeoType.Relation:
            if (this._relationsFilter == null || this._relationsFilter.Evaluate(osmGeo))
            {
              this._current = osmGeo;
              return true;
            }
            continue;
          default:
            continue;
        }
      }
      return false;
    }

    public override OsmGeo Current()
    {
      return this._current;
    }

    public override void Reset()
    {
      this._current = (OsmGeo) null;
      this.Source.Reset();
    }

    public override void RegisterSource(OsmStreamSource reader)
    {
      if ((this._wayKeepNodes || this._relationKeepObjects) && !reader.CanReset)
        throw new ArgumentException("The tags source cannot be reset!", "reader");
      base.RegisterSource(reader);
    }
  }
}
