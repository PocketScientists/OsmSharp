using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data
{
  public abstract class DataSourceReadOnlyBase : IDataSourceReadOnly, IOsmGeoSource, INodeSource, IWaySource, IRelationSource
  {
    public virtual bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public abstract GeoCoordinateBox BoundingBox { get; }

    public abstract Guid Id { get; }

    public abstract bool HasBoundinBox { get; }

    public virtual Node GetNode(long id)
    {
      IList<Node> nodes = this.GetNodes((IList<long>) new List<long>((IEnumerable<long>) new long[1]{ id }));
      if (nodes.Count > 0)
        return nodes[0];
      return (Node) null;
    }

    public abstract IList<Node> GetNodes(IList<long> ids);

    public virtual Relation GetRelation(long id)
    {
      IList<Relation> relations = this.GetRelations((IList<long>) new List<long>((IEnumerable<long>) new long[1]{ id }));
      if (relations.Count > 0)
        return relations[0];
      return (Relation) null;
    }

    public abstract IList<Relation> GetRelations(IList<long> ids);

    public abstract IList<Relation> GetRelationsFor(OsmGeoType type, long id);

    public IList<Relation> GetRelationsFor(OsmGeo obj)
    {
      return this.GetRelationsFor(obj.Type, obj.Id.Value);
    }

    public virtual Way GetWay(long id)
    {
      IList<Way> ways = this.GetWays((IList<long>) new List<long>((IEnumerable<long>) new long[1]{ id }));
      if (ways.Count > 0)
        return ways[0];
      return (Way) null;
    }

    public abstract IList<Way> GetWays(IList<long> ids);

    public abstract IList<Way> GetWaysFor(long id);

    public virtual IList<Way> GetWaysFor(Node node)
    {
      return this.GetWaysFor(node.Id.Value);
    }

    public abstract IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter);
  }
}
