namespace OsmSharp.Osm
{
  public class RelationMember
  {
    public OsmGeoType? MemberType { get; set; }

    public long? MemberId { get; set; }

    public string MemberRole { get; set; }

    public static RelationMember Create(int memberId, string memberRole, OsmGeoType memberType)
    {
      return new RelationMember()
      {
        MemberId = new long?((long) memberId),
        MemberRole = memberRole,
        MemberType = new OsmGeoType?(memberType)
      };
    }
  }
}
