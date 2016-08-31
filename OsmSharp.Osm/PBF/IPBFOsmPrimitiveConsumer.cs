namespace OsmSharp.Osm.PBF
{
  public interface IPBFOsmPrimitiveConsumer
  {
    void ProcessNode(PrimitiveBlock block, Node node);

    void ProcessWay(PrimitiveBlock block, Way way);

    void ProcessRelation(PrimitiveBlock block, Relation relation);
  }
}
