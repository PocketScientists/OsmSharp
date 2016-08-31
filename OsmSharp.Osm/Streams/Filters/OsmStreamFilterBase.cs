using OsmSharp.Osm.Cache;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
  public abstract class OsmStreamFilterBase : OsmStreamFilter
  {
    private readonly OsmDataCache _dataCache;
    private bool _cachingDone;
    private OsmGeo _current;
    private readonly HashSet<long> _nodesToInclude;
    private readonly Dictionary<long, int> _nodesUsedTwiceOrMore;
    private readonly HashSet<long> _waysToInclude;
    private readonly Dictionary<long, int> _waysUsedTwiceOrMore;
    private readonly HashSet<long> _relationsToInclude;
    private readonly Dictionary<long, int> _relationsUsedTwiceOrMore;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public OsmStreamFilterBase(OsmDataCache cache)
    {
      this._dataCache = cache;
      this._nodesToInclude = new HashSet<long>();
      this._nodesUsedTwiceOrMore = new Dictionary<long, int>();
      this._waysToInclude = new HashSet<long>();
      this._waysUsedTwiceOrMore = new Dictionary<long, int>();
      this._relationsToInclude = new HashSet<long>();
      this._relationsUsedTwiceOrMore = new Dictionary<long, int>();
    }

    public OsmStreamFilterBase(OsmStreamSource source, OsmDataCache cache)
    {
      this._dataCache = cache;
      this._nodesToInclude = new HashSet<long>();
      this._nodesUsedTwiceOrMore = new Dictionary<long, int>();
      this._waysToInclude = new HashSet<long>();
      this._waysUsedTwiceOrMore = new Dictionary<long, int>();
      this._relationsToInclude = new HashSet<long>();
      this._relationsUsedTwiceOrMore = new Dictionary<long, int>();
    }

    public override void Initialize()
    {
      this._cachingDone = false;
      this._nodesToInclude.Clear();
      this._nodesUsedTwiceOrMore.Clear();
      this._waysToInclude.Clear();
      this._waysUsedTwiceOrMore.Clear();
      this._relationsToInclude.Clear();
      this._relationsUsedTwiceOrMore.Clear();
      if (!this.Source.CanReset)
        throw new NotSupportedException("Creating a complete stream from a non-resettable simple stream is not supported. Wrap the source stream and create a resettable stream.");
    }

    public abstract bool Include(OsmGeo osmGeo);

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
      if (!this._cachingDone)
      {
        this.Seek();
        this.CacheRelations();
        this._cachingDone = true;
      }
      if (!this.Source.MoveNext())
        return false;
      OsmGeo osmGeo;
      for (osmGeo = this.Source.Current(); !this.IsChild(osmGeo) && !this.Include(osmGeo); osmGeo = this.Source.Current())
      {
        if (!this.Source.MoveNext())
          return false;
      }
      switch (osmGeo.Type)
      {
        case OsmGeoType.Node:
          this._current = osmGeo;
          if (this._nodesToInclude.Contains(osmGeo.Id.Value))
          {
            this._dataCache.AddNode(osmGeo as Node);
            this._nodesToInclude.Remove(osmGeo.Id.Value);
            break;
          }
          break;
        case OsmGeoType.Way:
          this._current = osmGeo;
          if (this._waysToInclude.Contains(osmGeo.Id.Value))
          {
            this._dataCache.AddWay(osmGeo as Way);
            this._waysToInclude.Remove(osmGeo.Id.Value);
            break;
          }
          Way way = osmGeo as Way;
          if (way.Nodes != null)
          {
            way.Nodes.ForEach<long>((Action<long>) (x => this.ReportNodeUsage(x)));
            break;
          }
          break;
        case OsmGeoType.Relation:
          this._current = osmGeo;
          if (!this._relationsToInclude.Contains(osmGeo.Id.Value))
          {
            Relation relation = osmGeo as Relation;
            if (relation.Members != null)
            {
              using (List<RelationMember>.Enumerator enumerator = relation.Members.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  RelationMember current = enumerator.Current;
                  long? memberId;
                  switch (current.MemberType.Value)
                  {
                    case OsmGeoType.Node:
                      memberId = current.MemberId;
                      this.ReportNodeUsage(memberId.Value);
                      continue;
                    case OsmGeoType.Way:
                      memberId = current.MemberId;
                      this.ReportWayUsage(memberId.Value);
                      continue;
                    case OsmGeoType.Relation:
                      memberId = current.MemberId;
                      this.ReportRelationUsage(memberId.Value);
                      continue;
                    default:
                      continue;
                  }
                }
                break;
              }
            }
            else
              break;
          }
          else
            break;
      }
      return true;
    }

    private bool IsChild(OsmGeo currentSimple)
    {
      switch (currentSimple.Type)
      {
        case OsmGeoType.Node:
          HashSet<long> nodesToInclude = this._nodesToInclude;
          long? id1 = currentSimple.Id;
          long num1 = id1.Value;
          if (nodesToInclude.Contains(num1))
            return true;
          OsmDataCache dataCache1 = this._dataCache;
          id1 = currentSimple.Id;
          long id2 = id1.Value;
          return dataCache1.ContainsNode(id2);
        case OsmGeoType.Way:
          HashSet<long> waysToInclude = this._waysToInclude;
          long? id3 = currentSimple.Id;
          long num2 = id3.Value;
          if (waysToInclude.Contains(num2))
            return true;
          OsmDataCache dataCache2 = this._dataCache;
          id3 = currentSimple.Id;
          long id4 = id3.Value;
          return dataCache2.ContainsWay(id4);
        case OsmGeoType.Relation:
          HashSet<long> relationsToInclude = this._relationsToInclude;
          long? id5 = currentSimple.Id;
          long num3 = id5.Value;
          if (relationsToInclude.Contains(num3))
            return true;
          OsmDataCache dataCache3 = this._dataCache;
          id5 = currentSimple.Id;
          long id6 = id5.Value;
          return dataCache3.ContainsRelation(id6);
        default:
          return false;
      }
    }

    private void Seek()
    {
      HashSet<long> longSet1 = new HashSet<long>();
      HashSet<long> longSet2 = new HashSet<long>();
      foreach (OsmGeo osmGeo in this.Source)
      {
        if (this.Include(osmGeo))
        {
          switch (osmGeo.Type)
          {
            case OsmGeoType.Way:
              using (List<long>.Enumerator enumerator = (osmGeo as Way).Nodes.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  this.MarkNodeAsChild(enumerator.Current);
                continue;
              }
            case OsmGeoType.Relation:
              using (List<RelationMember>.Enumerator enumerator = (osmGeo as Relation).Members.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  RelationMember current = enumerator.Current;
                  switch (current.MemberType.Value)
                  {
                    case OsmGeoType.Node:
                      this.MarkNodeAsChild(current.MemberId.Value);
                      continue;
                    case OsmGeoType.Way:
                      this.MarkWayAsChild(current.MemberId.Value);
                      longSet1.Add(current.MemberId.Value);
                      continue;
                    case OsmGeoType.Relation:
                      this.MarkRelationAsChild(current.MemberId.Value);
                      longSet2.Add(current.MemberId.Value);
                      continue;
                    default:
                      continue;
                  }
                }
                continue;
              }
            default:
              continue;
          }
        }
      }
      HashSet<long> longSet3;
      for (; longSet2.Count > 0 || longSet1.Count > 0; longSet2 = longSet3)
      {
        this.Source.Reset();
        HashSet<long> longSet4 = new HashSet<long>();
        longSet3 = new HashSet<long>();
        foreach (OsmGeo osmGeo in this.Source)
        {
          switch (osmGeo.Type)
          {
            case OsmGeoType.Way:
              if (longSet1.Contains(osmGeo.Id.Value))
              {
                using (List<long>.Enumerator enumerator = (osmGeo as Way).Nodes.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                    this.MarkNodeAsChild(enumerator.Current);
                  continue;
                }
              }
              else
                continue;
            case OsmGeoType.Relation:
              if (longSet2.Contains(osmGeo.Id.Value))
              {
                using (List<RelationMember>.Enumerator enumerator = (osmGeo as Relation).Members.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RelationMember current = enumerator.Current;
                    long? memberId;
                    switch (current.MemberType.Value)
                    {
                      case OsmGeoType.Node:
                        memberId = current.MemberId;
                        this.MarkNodeAsChild(memberId.Value);
                        continue;
                      case OsmGeoType.Way:
                        memberId = current.MemberId;
                        this.MarkWayAsChild(memberId.Value);
                        HashSet<long> longSet5 = longSet4;
                        memberId = current.MemberId;
                        long num1 = memberId.Value;
                        longSet5.Add(num1);
                        continue;
                      case OsmGeoType.Relation:
                        memberId = current.MemberId;
                        this.MarkRelationAsChild(memberId.Value);
                        HashSet<long> longSet6 = longSet3;
                        memberId = current.MemberId;
                        long num2 = memberId.Value;
                        longSet6.Add(num2);
                        continue;
                      default:
                        continue;
                    }
                  }
                  continue;
                }
              }
              else
                continue;
            default:
              continue;
          }
        }
        longSet1 = longSet4;
      }
      this.Source.Reset();
    }

    private void CacheRelations()
    {
      foreach (OsmGeo osmGeo in this.Source)
      {
        if (osmGeo.Type == OsmGeoType.Relation && this.Include(osmGeo) && this._relationsToInclude.Contains(osmGeo.Id.Value))
          this._dataCache.AddRelation(osmGeo as Relation);
      }
      this.Source.Reset();
    }

    private void ReportNodeUsage(long nodeId)
    {
      int num1;
      if (this._nodesUsedTwiceOrMore.TryGetValue(nodeId, out num1))
      {
        int num2 = num1 - 1;
        if (num2 > 0)
          this._nodesUsedTwiceOrMore[nodeId] = num2;
        else
          this._nodesUsedTwiceOrMore.Remove(nodeId);
      }
      else
        this._dataCache.RemoveNode(nodeId);
    }

    private void MarkNodeAsChild(long nodeId)
    {
      if (this._nodesToInclude.Contains(nodeId))
      {
        int num1;
        if (this._nodesUsedTwiceOrMore.TryGetValue(nodeId, out num1))
          return;
        int num2 = 1;
        this._nodesUsedTwiceOrMore.Add(nodeId, num2);
      }
      else
        this._nodesToInclude.Add(nodeId);
    }

    private void ReportWayUsage(long wayId)
    {
      int num1;
      if (this._waysUsedTwiceOrMore.TryGetValue(wayId, out num1))
      {
        int num2 = num1 - 1;
        if (num2 > 0)
          this._waysUsedTwiceOrMore[wayId] = num2;
        else
          this._waysUsedTwiceOrMore.Remove(wayId);
      }
      else
      {
        Way way = this._dataCache.GetWay(wayId);
        this._dataCache.RemoveWay(wayId);
        if (way.Nodes == null)
          return;
        way.Nodes.ForEach<long>((Action<long>) (x => this.ReportNodeUsage(x)));
      }
    }

    private void MarkWayAsChild(long wayId)
    {
      if (this._waysToInclude.Contains(wayId))
      {
        int num1;
        if (this._waysUsedTwiceOrMore.TryGetValue(wayId, out num1))
          return;
        int num2 = 1;
        this._waysUsedTwiceOrMore.Add(wayId, num2);
      }
      else
        this._waysToInclude.Add(wayId);
    }

    private void ReportRelationUsage(long relationId)
    {
      int num1;
      if (this._relationsUsedTwiceOrMore.TryGetValue(relationId, out num1))
      {
        int num2 = num1 - 1;
        if (num2 > 0)
          this._relationsUsedTwiceOrMore[relationId] = num2;
        else
          this._relationsUsedTwiceOrMore.Remove(relationId);
      }
      else
      {
        this._dataCache.GetRelation(relationId);
        this._dataCache.RemoveRelation(relationId);
        this._relationsToInclude.Remove(relationId);
      }
    }

    private void MarkRelationAsChild(long relationId)
    {
      if (this._relationsToInclude.Contains(relationId))
      {
        int num1;
        if (this._relationsUsedTwiceOrMore.TryGetValue(relationId, out num1))
          return;
        int num2 = 1;
        this._relationsUsedTwiceOrMore.Add(relationId, num2);
      }
      else
        this._relationsToInclude.Add(relationId);
    }

    public override OsmGeo Current()
    {
      return this._current;
    }

    public override void Reset()
    {
      this._cachingDone = false;
      this._dataCache.Clear();
      this.Source.Reset();
    }
  }
}
