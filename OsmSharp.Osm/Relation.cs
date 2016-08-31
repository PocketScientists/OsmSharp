using OsmSharp.Collections.Tags;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class Relation : OsmGeo
  {
    public List<RelationMember> Members { get; set; }

    public Relation()
    {
      this.Type = OsmGeoType.Relation;
    }

    public override string ToString()
    {
      string str = "{no tags}";
      if (this.Tags != null && this.Tags.Count > 0)
        str = this.Tags.ToString();
      if (!this.Id.HasValue)
        return string.Format("Relation[null]{0}", (object) str);
      return string.Format("Relation[{0}]{1}", new object[2]
      {
        (object) this.Id.Value,
        (object) str
      });
    }

    public static Relation Create(long id, params RelationMember[] members)
    {
      Relation relation = new Relation();
      long? nullable = new long?(id);
      relation.Id = nullable;
      List<RelationMember> relationMemberList = new List<RelationMember>((IEnumerable<RelationMember>) members);
      relation.Members = relationMemberList;
      return relation;
    }

    public static Relation Create(long id, TagsCollectionBase tags, params RelationMember[] members)
    {
      Relation relation = new Relation();
      long? nullable = new long?(id);
      relation.Id = nullable;
      List<RelationMember> relationMemberList = new List<RelationMember>((IEnumerable<RelationMember>) members);
      relation.Members = relationMemberList;
      TagsCollectionBase tagsCollectionBase = tags;
      relation.Tags = tagsCollectionBase;
      return relation;
    }
  }
}
