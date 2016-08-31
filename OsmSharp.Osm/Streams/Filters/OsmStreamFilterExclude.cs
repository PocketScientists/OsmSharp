using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterExclude : OsmStreamFilter
  {
    private bool _excludeNodes = true;
    private bool _excludeWays = true;
    private bool _excludeRelations = true;
    private List<OsmStreamSource> _sources;
    private HashSet<long> _nodesToExclude;
    private HashSet<long> _waysToExclude;
    private HashSet<long> _relationsToExclude;

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
        if (this._sources.Count == 0)
          return false;
        return this._sources[0].IsSorted;
      }
    }

    public OsmStreamFilterExclude()
      : this(true, true, true)
    {
    }

    public OsmStreamFilterExclude(bool excludeNodes, bool excludeWays, bool excludeRelations)
    {
      this._sources = new List<OsmStreamSource>();
      this._excludeNodes = excludeNodes;
      this._excludeWays = excludeWays;
      this._excludeRelations = excludeRelations;
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
      return this._sources[0].Current();
    }

    public override void Initialize()
    {
      foreach (OsmStreamSource source in this._sources)
        source.Initialize();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      this._nodesToExclude = new HashSet<long>();
      this._waysToExclude = new HashSet<long>();
      this._relationsToExclude = new HashSet<long>();
      long? id;
      for (int index = 1; index < this._sources.Count; ++index)
      {
        while (this._sources[index].MoveNext())
        {
          OsmGeo osmGeo = this._sources[index].Current();
          switch (osmGeo.Type)
          {
            case OsmGeoType.Node:
              HashSet<long> nodesToExclude = this._nodesToExclude;
              id = osmGeo.Id;
              long num1 = id.Value;
              nodesToExclude.Add(num1);
              continue;
            case OsmGeoType.Way:
              HashSet<long> waysToExclude = this._waysToExclude;
              id = osmGeo.Id;
              long num2 = id.Value;
              waysToExclude.Add(num2);
              continue;
            case OsmGeoType.Relation:
              HashSet<long> relationsToExclude = this._relationsToExclude;
              id = osmGeo.Id;
              long num3 = id.Value;
              relationsToExclude.Add(num3);
              continue;
            default:
              continue;
          }
        }
      }
      while (this._sources[0].MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
      {
        OsmGeo osmGeo = this._sources[0].Current();
        bool flag = false;
        switch (osmGeo.Type)
        {
          case OsmGeoType.Node:
            int num1;
            if (this._excludeNodes)
            {
              HashSet<long> nodesToExclude = this._nodesToExclude;
              id = osmGeo.Id;
              long num2 = id.Value;
              num1 = nodesToExclude.Contains(num2) ? 1 : 0;
            }
            else
              num1 = 0;
            flag = num1 != 0;
            break;
          case OsmGeoType.Way:
            int num3;
            if (this._excludeWays)
            {
              HashSet<long> waysToExclude = this._waysToExclude;
              id = osmGeo.Id;
              long num2 = id.Value;
              num3 = waysToExclude.Contains(num2) ? 1 : 0;
            }
            else
              num3 = 0;
            flag = num3 != 0;
            break;
          case OsmGeoType.Relation:
            int num4;
            if (this._excludeRelations)
            {
              HashSet<long> relationsToExclude = this._relationsToExclude;
              id = osmGeo.Id;
              long num2 = id.Value;
              num4 = relationsToExclude.Contains(num2) ? 1 : 0;
            }
            else
              num4 = 0;
            flag = num4 != 0;
            break;
        }
        if (!flag)
          return true;
      }
      return false;
    }

    public override void Reset()
    {
      this._nodesToExclude = (HashSet<long>) null;
      this._waysToExclude = (HashSet<long>) null;
      this._relationsToExclude = (HashSet<long>) null;
      foreach (OsmStreamSource source in this._sources)
        source.Reset();
    }
  }
}
