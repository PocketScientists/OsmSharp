namespace OsmSharp.Osm.Data
{
  public interface IRelationSource
  {
    Relation GetRelation(long id);
  }
}
