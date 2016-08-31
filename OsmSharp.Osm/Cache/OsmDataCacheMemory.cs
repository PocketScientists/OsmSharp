using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Cache
{
  public class OsmDataCacheMemory : OsmDataCache
  {
    private Dictionary<long, Node> _nodes;
    private Dictionary<long, Way> _ways;
    private Dictionary<long, Relation> _relations;

    public OsmDataCacheMemory()
    {
      this._nodes = new Dictionary<long, Node>();
      this._ways = new Dictionary<long, Way>();
      this._relations = new Dictionary<long, Relation>();
    }

    public override void AddNode(Node node)
    {
      if (node == null)
        throw new ArgumentNullException("node");
      if (!node.Id.HasValue)
        throw new Exception("node.Id is null");
      this._nodes[node.Id.Value] = node;
    }

    public override bool RemoveNode(long id)
    {
      return this._nodes.Remove(id);
    }

    public override bool TryGetNode(long id, out Node node)
    {
      return this._nodes.TryGetValue(id, out node);
    }

    public override void AddWay(Way way)
    {
      if (way == null)
        throw new ArgumentNullException("way");
      if (!way.Id.HasValue)
        throw new Exception("way.Id is null");
      this._ways[way.Id.Value] = way;
    }

    public override bool RemoveWay(long id)
    {
      return this._ways.Remove(id);
    }

    public override bool TryGetWay(long id, out Way way)
    {
      return this._ways.TryGetValue(id, out way);
    }

    public override void AddRelation(Relation relation)
    {
      if (relation == null)
        throw new ArgumentNullException("relation");
      if (!relation.Id.HasValue)
        throw new Exception("relation.Id is null");
      this._relations[relation.Id.Value] = relation;
    }

    public override bool RemoveRelation(long id)
    {
      return this._relations.Remove(id);
    }

    public override bool TryGetRelation(long id, out Relation relation)
    {
      return this._relations.TryGetValue(id, out relation);
    }

    public override void Clear()
    {
      this._nodes.Clear();
      this._ways.Clear();
      this._relations.Clear();
    }
  }
}
