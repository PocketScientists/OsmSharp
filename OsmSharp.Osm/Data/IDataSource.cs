namespace OsmSharp.Osm.Data
{
  public interface IDataSource : IDataSourceReadOnly, IOsmGeoSource, INodeSource, IWaySource, IRelationSource
  {
    bool IsBaseIdGenerator { get; }

    bool IsCreator { get; }

    bool IsLive { get; }

    void Persist();

    void AddNode(Node node);

    void AddRelation(Relation relation);

    void AddWay(Way way);

    void ApplyChangeSet(CompleteChangeSet changeSet);
  }
}
