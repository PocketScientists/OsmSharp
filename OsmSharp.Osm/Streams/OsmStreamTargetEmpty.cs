namespace OsmSharp.Osm.Streams
{
  public class OsmStreamTargetEmpty : OsmStreamTarget
  {
    public override void Initialize()
    {
    }

    public override void AddNode(Node simpleNode)
    {
    }

    public override void AddWay(Way simpleWay)
    {
    }

    public override void AddRelation(Relation simpleRelation)
    {
    }
  }
}
