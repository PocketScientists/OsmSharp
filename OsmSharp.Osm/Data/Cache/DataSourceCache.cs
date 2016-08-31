using OsmSharp.Collections.Cache;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data.Cache
{
  public class DataSourceCache : DataSourceReadOnlyBase
  {
    private Guid _id = Guid.NewGuid();
    private LRUCache<long, Node> _nodesCache = new LRUCache<long, Node>(10000);
    private LRUCache<long, Way> _waysCache = new LRUCache<long, Way>(5000);
    private LRUCache<long, Relation> _relationsCache = new LRUCache<long, Relation>(1000);
    private IDataSourceReadOnly _source;

    public override GeoCoordinateBox BoundingBox
    {
      get
      {
        return this._source.BoundingBox;
      }
    }

    public override Guid Id
    {
      get
      {
        return this._id;
      }
    }

    public override bool HasBoundinBox
    {
      get
      {
        return this._source.HasBoundinBox;
      }
    }

    public DataSourceCache(IDataSourceReadOnly source)
    {
      this._source = source;
    }

    public override Node GetNode(long id)
    {
      Node node;
      if (!this._nodesCache.TryGet(id, out node))
      {
        node = this._source.GetNode(id);
        this._nodesCache.Add(id, node);
      }
      return node;
    }

    public override IList<Node> GetNodes(IList<long> ids)
    {
      List<Node> nodeList = new List<Node>(ids.Count);
      for (int index = 0; index < nodeList.Count; ++index)
        nodeList.Add(this.GetNode(ids[index]));
      return (IList<Node>) nodeList;
    }

    public override Relation GetRelation(long id)
    {
      Relation relation;
      if (!this._relationsCache.TryGet(id, out relation))
      {
        relation = this._source.GetRelation(id);
        this._relationsCache.Add(id, relation);
      }
      return relation;
    }

    public override IList<Relation> GetRelations(IList<long> ids)
    {
      List<Relation> relationList = new List<Relation>(ids.Count);
      for (int index = 0; index < relationList.Count; ++index)
        relationList.Add(this.GetRelation(ids[index]));
      return (IList<Relation>) relationList;
    }

    public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
    {
      IList<Relation> relationsFor = this._source.GetRelationsFor(type, id);
      foreach (Relation relation in (IEnumerable<Relation>) relationsFor)
        this._relationsCache.Add(relation.Id.Value, relation);
      return relationsFor;
    }

    public override Way GetWay(long id)
    {
      Way way;
      if (!this._waysCache.TryGet(id, out way))
      {
        way = this._source.GetWay(id);
        this._waysCache.Add(id, way);
      }
      return way;
    }

    public override IList<Way> GetWays(IList<long> ids)
    {
      List<Way> wayList = new List<Way>(ids.Count);
      for (int index = 0; index < wayList.Count; ++index)
        wayList.Add(this.GetWay(ids[index]));
      return (IList<Way>) wayList;
    }

    public override IList<Way> GetWaysFor(Node node)
    {
      return this.GetWaysFor(node.Id.Value);
    }

    public override IList<Way> GetWaysFor(long id)
    {
      IList<Way> waysFor = this._source.GetWaysFor(id);
      foreach (Way way in (IEnumerable<Way>) waysFor)
        this._waysCache.Add(way.Id.Value, way);
      return waysFor;
    }

    public override IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
    {
      IList<OsmGeo> osmGeoList = this._source.Get(box, filter);
      long? id;
      foreach (OsmGeo osmGeo in (IEnumerable<OsmGeo>) osmGeoList)
      {
        switch (osmGeo.Type)
        {
          case OsmGeoType.Node:
            LRUCache<long, Node> nodesCache = this._nodesCache;
            id = osmGeo.Id;
            long key1 = id.Value;
            Node node = osmGeo as Node;
            nodesCache.Add(key1, node);
            continue;
          case OsmGeoType.Way:
            LRUCache<long, Way> waysCache = this._waysCache;
            id = osmGeo.Id;
            long key2 = id.Value;
            Way way = osmGeo as Way;
            waysCache.Add(key2, way);
            continue;
          case OsmGeoType.Relation:
            LRUCache<long, Relation> relationsCache = this._relationsCache;
            id = osmGeo.Id;
            long key3 = id.Value;
            Relation relation = osmGeo as Relation;
            relationsCache.Add(key3, relation);
            continue;
          default:
            continue;
        }
      }
      return osmGeoList;
    }
  }
}
