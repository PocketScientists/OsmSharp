using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Cache;
using OsmSharp.Osm.Data;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Complete
{
  public class OsmSimpleCompleteStreamSource : OsmCompleteStreamSource
  {
    private readonly OsmDataCache _dataCache;
    private readonly OsmStreamSource _simpleSource;
    private bool _cachingDone;
    private ICompleteOsmGeo _current;
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
        return this._simpleSource.CanReset;
      }
    }

    public OsmSimpleCompleteStreamSource(OsmStreamSource source)
    {
      this._dataCache = (OsmDataCache) new OsmDataCacheMemory();
      this._simpleSource = source;
      this._nodesToInclude = new HashSet<long>();
      this._nodesUsedTwiceOrMore = new Dictionary<long, int>();
      this._waysToInclude = new HashSet<long>();
      this._waysUsedTwiceOrMore = new Dictionary<long, int>();
      this._relationsToInclude = new HashSet<long>();
      this._relationsUsedTwiceOrMore = new Dictionary<long, int>();
    }

    public OsmSimpleCompleteStreamSource(OsmStreamSource source, OsmDataCache cache)
    {
      this._dataCache = cache;
      this._simpleSource = source;
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
      if (!this._simpleSource.CanReset)
        throw new NotSupportedException("Creating a complete stream from a non-resettable simple stream is not supported. Wrap the source stream and create a resettable stream.");
    }

    public override bool MoveNext()
    {
      if (!this._cachingDone)
      {
        this.Seek();
        this._cachingDone = true;
      }
      while (this._simpleSource.MoveNext())
      {
        OsmGeo osmGeo = this._simpleSource.Current();
        long? nullable;
        switch (osmGeo.Type)
        {
          case OsmGeoType.Node:
            this._current = (ICompleteOsmGeo) (osmGeo as Node);
            if (this._current != null && this._current.Tags == null)
              this._current.Tags = (TagsCollectionBase) new TagsCollection();
            HashSet<long> nodesToInclude1 = this._nodesToInclude;
            nullable = osmGeo.Id;
            long num1 = nullable.Value;
            if (nodesToInclude1.Contains(num1))
            {
              this._dataCache.AddNode(osmGeo as Node);
              HashSet<long> nodesToInclude2 = this._nodesToInclude;
              nullable = osmGeo.Id;
              long num2 = nullable.Value;
              nodesToInclude2.Remove(num2);
              break;
            }
            break;
          case OsmGeoType.Way:
            this._current = (ICompleteOsmGeo) CompleteWay.CreateFrom(osmGeo as Way, (INodeSource) this._dataCache);
            HashSet<long> waysToInclude1 = this._waysToInclude;
            nullable = osmGeo.Id;
            long num3 = nullable.Value;
            if (waysToInclude1.Contains(num3))
            {
              this._dataCache.AddWay(osmGeo as Way);
              HashSet<long> waysToInclude2 = this._waysToInclude;
              nullable = osmGeo.Id;
              long num2 = nullable.Value;
              waysToInclude2.Remove(num2);
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
            this._current = (ICompleteOsmGeo) CompleteRelation.CreateFrom(osmGeo as Relation, (IOsmGeoSource) this._dataCache);
            HashSet<long> relationsToInclude = this._relationsToInclude;
            nullable = osmGeo.Id;
            long num4 = nullable.Value;
            if (!relationsToInclude.Contains(num4))
            {
              Relation relation = osmGeo as Relation;
              if (relation.Members != null)
              {
                using (List<RelationMember>.Enumerator enumerator = relation.Members.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RelationMember current = enumerator.Current;
                    switch (current.MemberType.Value)
                    {
                      case OsmGeoType.Node:
                        nullable = current.MemberId;
                        this.ReportNodeUsage(nullable.Value);
                        continue;
                      case OsmGeoType.Way:
                        nullable = current.MemberId;
                        this.ReportWayUsage(nullable.Value);
                        continue;
                      case OsmGeoType.Relation:
                        nullable = current.MemberId;
                        this.ReportRelationUsage(nullable.Value);
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
        if (this._current != null)
          return true;
      }
      return false;
    }

    private void Seek()
    {
      List<Relation> relationList = new List<Relation>();
      this._simpleSource.Initialize();
      while (this._simpleSource.MoveNext(true, false, false))
      {
        OsmGeo osmGeo = this._simpleSource.Current();
        switch (osmGeo.Type)
        {
          case OsmGeoType.Way:
            Way way = osmGeo as Way;
            if (way.Nodes != null)
            {
              using (List<long>.Enumerator enumerator = way.Nodes.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  this.MarkNodeAsChild(enumerator.Current);
                continue;
              }
            }
            else
              continue;
          case OsmGeoType.Relation:
            Relation relation = osmGeo as Relation;
            if (relation.Members != null)
            {
              foreach (RelationMember member in relation.Members)
              {
                switch (member.MemberType.Value)
                {
                  case OsmGeoType.Node:
                    this.MarkNodeAsChild(member.MemberId.Value);
                    continue;
                  case OsmGeoType.Way:
                    this.MarkWayAsChild(member.MemberId.Value);
                    continue;
                  case OsmGeoType.Relation:
                    this.MarkRelationAsChild(member.MemberId.Value);
                    continue;
                  default:
                    continue;
                }
              }
            }
            relationList.Add(relation);
            continue;
          default:
            continue;
        }
      }
      foreach (Relation relation in relationList)
      {
        if (this._relationsToInclude.Contains(relation.Id.Value))
          this._dataCache.AddRelation(relation);
      }
      this._simpleSource.Reset();
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
        if (!this._nodesUsedTwiceOrMore.TryGetValue(nodeId, out num1))
        {
          int num2 = 1;
          this._nodesUsedTwiceOrMore.Add(nodeId, num2);
        }
        else
        {
          int num2 = num1 + 1;
          this._nodesUsedTwiceOrMore[nodeId] = num2;
        }
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
        if (way == null || way.Nodes == null)
          return;
        way.Nodes.ForEach<long>((Action<long>) (x => this.ReportNodeUsage(x)));
      }
    }

    private void MarkWayAsChild(long wayId)
    {
      if (this._waysToInclude.Contains(wayId))
      {
        int num1;
        if (!this._waysUsedTwiceOrMore.TryGetValue(wayId, out num1))
        {
          int num2 = 1;
          this._waysUsedTwiceOrMore.Add(wayId, num2);
        }
        else
        {
          int num2 = num1 + 1;
          this._waysUsedTwiceOrMore[wayId] = num2;
        }
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
        if (!this._relationsUsedTwiceOrMore.TryGetValue(relationId, out num1))
        {
          int num2 = 1;
          this._relationsUsedTwiceOrMore.Add(relationId, num2);
        }
        else
        {
          int num2 = num1 + 1;
          this._relationsUsedTwiceOrMore[relationId] = num2;
        }
      }
      else
        this._relationsToInclude.Add(relationId);
    }

    public override ICompleteOsmGeo Current()
    {
      return this._current;
    }

    public override void Reset()
    {
      this._cachingDone = false;
      this._dataCache.Clear();
      this._simpleSource.Reset();
    }
  }
}
