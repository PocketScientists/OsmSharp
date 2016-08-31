using OsmSharp.Osm.Data;

namespace OsmSharp.Osm.Cache
{
  public abstract class OsmDataCache : IOsmGeoSource, INodeSource, IWaySource, IRelationSource
  {
    public abstract void AddNode(Node node);

    public Node GetNode(long id)
    {
      Node node;
      if (this.TryGetNode(id, out node))
        return node;
      return (Node) null;
    }

    public abstract bool RemoveNode(long id);

    public virtual bool ContainsNode(long id)
    {
      Node node;
      return this.TryGetNode(id, out node);
    }

    public abstract bool TryGetNode(long id, out Node node);

    public abstract void AddWay(Way way);

    public abstract bool RemoveWay(long id);

    public Way GetWay(long id)
    {
      Way way;
      if (this.TryGetWay(id, out way))
        return way;
      return (Way) null;
    }

    public virtual bool ContainsWay(long id)
    {
      Way way;
      return this.TryGetWay(id, out way);
    }

    public abstract bool TryGetWay(long id, out Way way);

    public abstract void AddRelation(Relation relation);

    public Relation GetRelation(long id)
    {
      Relation relation;
      if (this.TryGetRelation(id, out relation))
        return relation;
      return (Relation) null;
    }

    public abstract bool RemoveRelation(long id);

    public virtual bool ContainsRelation(long id)
    {
      Relation relation;
      return this.TryGetRelation(id, out relation);
    }

    public abstract bool TryGetRelation(long id, out Relation relation);

    public abstract void Clear();
  }
}
