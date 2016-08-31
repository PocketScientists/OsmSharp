using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class CompleteRelation : CompleteOsmGeo
  {
    private readonly IList<CompleteRelationMember> _members;

    public override CompleteOsmType Type
    {
      get
      {
        return CompleteOsmType.Relation;
      }
    }

    public IList<CompleteRelationMember> Members
    {
      get
      {
        return this._members;
      }
    }

    protected internal CompleteRelation(long id)
      : base(id)
    {
      this._members = (IList<CompleteRelationMember>) new List<CompleteRelationMember>();
    }

    protected internal CompleteRelation(ObjectTable<string> stringTable, long id)
      : base(stringTable, id)
    {
      this._members = (IList<CompleteRelationMember>) new List<CompleteRelationMember>();
    }

    public ICompleteOsmGeo FindMember(string role)
    {
      if (this.Members != null)
      {
        foreach (CompleteRelationMember member in (IEnumerable<CompleteRelationMember>) this.Members)
        {
          if (member.Role == role)
            return member.Member;
        }
      }
      return (ICompleteOsmGeo) null;
    }

    public override OsmGeo ToSimple()
    {
      Relation relation1 = new Relation();
      relation1.Id = new long?(this.Id);
      relation1.ChangeSetId = this.ChangeSetId;
      relation1.Tags = this.Tags;
      relation1.TimeStamp = this.TimeStamp;
      relation1.UserId = this.UserId;
      relation1.UserName = this.User;
      Relation relation2 = relation1;
      long? version = this.Version;
      ulong? nullable = version.HasValue ? new ulong?((ulong) version.GetValueOrDefault()) : new ulong?();
      relation2.Version = nullable;
      relation1.Visible = new bool?(this.Visible);
      relation1.Members = new List<RelationMember>();
      foreach (CompleteRelationMember member in (IEnumerable<CompleteRelationMember>) this.Members)
      {
        RelationMember relationMember = new RelationMember();
        relationMember.MemberId = new long?(member.Member.Id);
        relationMember.MemberRole = member.Role;
        switch (member.Member.Type)
        {
          case CompleteOsmType.Node:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Node);
            break;
          case CompleteOsmType.Way:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Way);
            break;
          case CompleteOsmType.Relation:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Relation);
            break;
        }
        relation1.Members.Add(relationMember);
      }
      return (OsmGeo) relation1;
    }

    public IList<GeoCoordinate> GetCoordinates()
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      for (int index = 0; index < this.Members.Count; ++index)
      {
        if (this.Members[index].Member is Node)
        {
          Node member = this.Members[index].Member as Node;
          geoCoordinateList.Add(member.Coordinate);
        }
        else if (this.Members[index].Member is CompleteWay)
        {
          CompleteWay member = this.Members[index].Member as CompleteWay;
          geoCoordinateList.AddRange((IEnumerable<GeoCoordinate>) member.GetCoordinates());
        }
        else if (this.Members[index].Member is CompleteRelation)
        {
          CompleteRelation member = this.Members[index].Member as CompleteRelation;
          geoCoordinateList.AddRange((IEnumerable<GeoCoordinate>) member.GetCoordinates());
        }
      }
      return (IList<GeoCoordinate>) geoCoordinateList;
    }

    public static CompleteRelation Create(long id)
    {
      return new CompleteRelation(id);
    }

    public static CompleteRelation CreateFrom(Relation simpleRelation, IDictionary<long, Node> nodes, IDictionary<long, CompleteWay> ways, IDictionary<long, CompleteRelation> relations)
    {
      if (simpleRelation == null)
        throw new ArgumentNullException("simpleRelation");
      if (nodes == null)
        throw new ArgumentNullException("nodes");
      if (ways == null)
        throw new ArgumentNullException("ways");
      if (relations == null)
        throw new ArgumentNullException("relations");
      if (!simpleRelation.Id.HasValue)
        throw new Exception("simpleRelation.Id is null");
      CompleteRelation completeRelation1 = CompleteRelation.Create(simpleRelation.Id.Value);
      completeRelation1.ChangeSetId = simpleRelation.ChangeSetId;
      foreach (Tag tag in simpleRelation.Tags)
        completeRelation1.Tags.Add(tag);
      long? nullable1;
      for (int index = 0; index < simpleRelation.Members.Count; ++index)
      {
        nullable1 = simpleRelation.Members[index].MemberId;
        long key = nullable1.Value;
        string memberRole = simpleRelation.Members[index].MemberRole;
        CompleteRelationMember completeRelationMember = new CompleteRelationMember();
        completeRelationMember.Role = memberRole;
        switch (simpleRelation.Members[index].MemberType.Value)
        {
          case OsmGeoType.Node:
            Node node = (Node) null;
            if (!nodes.TryGetValue(key, out node))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) node;
            break;
          case OsmGeoType.Way:
            CompleteWay completeWay = (CompleteWay) null;
            if (!ways.TryGetValue(key, out completeWay))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) completeWay;
            break;
          case OsmGeoType.Relation:
            CompleteRelation completeRelation2 = (CompleteRelation) null;
            if (!relations.TryGetValue(key, out completeRelation2))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) completeRelation2;
            break;
        }
        completeRelation1.Members.Add(completeRelationMember);
      }
      completeRelation1.TimeStamp = simpleRelation.TimeStamp;
      completeRelation1.User = simpleRelation.UserName;
      completeRelation1.UserId = simpleRelation.UserId;
      CompleteRelation completeRelation3 = completeRelation1;
      ulong? version = simpleRelation.Version;
      long? nullable2;
      if (!version.HasValue)
      {
        nullable1 = new long?();
        nullable2 = nullable1;
      }
      else
      {
        version = simpleRelation.Version;
        nullable2 = new long?((long) version.Value);
      }
      completeRelation3.Version = nullable2;
      completeRelation1.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
      return completeRelation1;
    }

    public static CompleteRelation CreateFrom(Relation simpleRelation, IOsmGeoSource osmGeoSource)
    {
      if (simpleRelation == null)
        throw new ArgumentNullException("simpleRelation");
      if (osmGeoSource == null)
        throw new ArgumentNullException("osmGeoSource");
      if (!simpleRelation.Id.HasValue)
        throw new Exception("simpleRelation.Id is null");
      CompleteRelation completeRelation1 = CompleteRelation.Create(simpleRelation.Id.Value);
      completeRelation1.ChangeSetId = simpleRelation.ChangeSetId;
      if (simpleRelation.Tags != null)
      {
        foreach (Tag tag in simpleRelation.Tags)
          completeRelation1.Tags.Add(tag);
      }
      long? nullable1;
      if (simpleRelation.Members != null)
      {
        for (int index = 0; index < simpleRelation.Members.Count; ++index)
        {
          nullable1 = simpleRelation.Members[index].MemberId;
          long id = nullable1.Value;
          string memberRole = simpleRelation.Members[index].MemberRole;
          CompleteRelationMember completeRelationMember = new CompleteRelationMember();
          completeRelationMember.Role = memberRole;
          switch (simpleRelation.Members[index].MemberType.Value)
          {
            case OsmGeoType.Node:
              Node node1 = osmGeoSource.GetNode(id);
              if (node1 == null)
                return (CompleteRelation) null;
              Node node2 = node1;
              if (node2 == null)
                return (CompleteRelation) null;
              completeRelationMember.Member = (ICompleteOsmGeo) node2;
              break;
            case OsmGeoType.Way:
              Way way = osmGeoSource.GetWay(id);
              if (way == null)
                return (CompleteRelation) null;
              CompleteWay from1 = CompleteWay.CreateFrom(way, (INodeSource) osmGeoSource);
              if (!((CompleteOsmBase) from1 != (CompleteOsmBase) null))
                return (CompleteRelation) null;
              completeRelationMember.Member = (ICompleteOsmGeo) from1;
              break;
            case OsmGeoType.Relation:
              Relation relation = osmGeoSource.GetRelation(id);
              if (relation == null)
                return (CompleteRelation) null;
              CompleteRelation from2 = CompleteRelation.CreateFrom(relation, osmGeoSource);
              if (!((CompleteOsmBase) from2 != (CompleteOsmBase) null))
                return (CompleteRelation) null;
              completeRelationMember.Member = (ICompleteOsmGeo) from2;
              break;
          }
          completeRelation1.Members.Add(completeRelationMember);
        }
      }
      completeRelation1.TimeStamp = simpleRelation.TimeStamp;
      completeRelation1.User = simpleRelation.UserName;
      completeRelation1.UserId = simpleRelation.UserId;
      CompleteRelation completeRelation2 = completeRelation1;
      long? nullable2;
      if (!simpleRelation.Version.HasValue)
      {
        nullable1 = new long?();
        nullable2 = nullable1;
      }
      else
        nullable2 = new long?((long) simpleRelation.Version.Value);
      completeRelation2.Version = nullable2;
      completeRelation1.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
      return completeRelation1;
    }

    public static CompleteRelation CreateFrom(Relation simpleRelation, IOsmGeoSource osmGeoSource, IDictionary<long, CompleteWay> ways, IDictionary<long, CompleteRelation> relations)
    {
      if (simpleRelation == null)
        throw new ArgumentNullException("simpleRelation");
      if (osmGeoSource == null)
        throw new ArgumentNullException("osmGeoSource");
      if (!simpleRelation.Id.HasValue)
        throw new Exception("simpleRelation.Id is null");
      CompleteRelation completeRelation1 = CompleteRelation.Create(simpleRelation.Id.Value);
      completeRelation1.ChangeSetId = simpleRelation.ChangeSetId;
      foreach (Tag tag in simpleRelation.Tags)
        completeRelation1.Tags.Add(tag);
      long? nullable1;
      for (int index = 0; index < simpleRelation.Members.Count; ++index)
      {
        nullable1 = simpleRelation.Members[index].MemberId;
        long num = nullable1.Value;
        string memberRole = simpleRelation.Members[index].MemberRole;
        CompleteRelationMember completeRelationMember = new CompleteRelationMember();
        completeRelationMember.Role = memberRole;
        switch (simpleRelation.Members[index].MemberType.Value)
        {
          case OsmGeoType.Node:
            Node node = osmGeoSource.GetNode(num);
            if (node == null)
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) node;
            break;
          case OsmGeoType.Way:
            CompleteWay from1;
            if (!ways.TryGetValue(num, out from1))
            {
              Way way = osmGeoSource.GetWay(num);
              if (way != null)
                from1 = CompleteWay.CreateFrom(way, (INodeSource) osmGeoSource);
            }
            if (!((CompleteOsmBase) from1 != (CompleteOsmBase) null))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) from1;
            break;
          case OsmGeoType.Relation:
            CompleteRelation from2;
            if (!relations.TryGetValue(num, out from2))
            {
              Relation relation = osmGeoSource.GetRelation(num);
              if (relation != null)
                from2 = CompleteRelation.CreateFrom(relation, osmGeoSource);
            }
            if (!((CompleteOsmBase) from2 != (CompleteOsmBase) null))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) from2;
            break;
        }
        completeRelation1.Members.Add(completeRelationMember);
      }
      completeRelation1.TimeStamp = simpleRelation.TimeStamp;
      completeRelation1.User = simpleRelation.UserName;
      completeRelation1.UserId = simpleRelation.UserId;
      CompleteRelation completeRelation2 = completeRelation1;
      long? nullable2;
      if (!simpleRelation.Version.HasValue)
      {
        nullable1 = new long?();
        nullable2 = nullable1;
      }
      else
        nullable2 = new long?((long) simpleRelation.Version.Value);
      completeRelation2.Version = nullable2;
      completeRelation1.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
      return completeRelation1;
    }

    public static CompleteRelation Create(ObjectTable<string> table, long id)
    {
      return new CompleteRelation(table, id);
    }

    public static CompleteRelation CreateFrom(ObjectTable<string> table, Relation simpleRelation, IDictionary<long, Node> nodes, IDictionary<long, CompleteWay> ways, IDictionary<long, CompleteRelation> relations)
    {
      if (simpleRelation == null)
        throw new ArgumentNullException("simpleRelation");
      if (nodes == null)
        throw new ArgumentNullException("nodes");
      if (ways == null)
        throw new ArgumentNullException("ways");
      if (relations == null)
        throw new ArgumentNullException("relations");
      if (!simpleRelation.Id.HasValue)
        throw new Exception("simpleRelation.Id is null");
      CompleteRelation completeRelation1 = CompleteRelation.Create(table, simpleRelation.Id.Value);
      completeRelation1.ChangeSetId = simpleRelation.ChangeSetId;
      foreach (Tag tag in simpleRelation.Tags)
        completeRelation1.Tags.Add(tag);
      long? nullable1;
      for (int index = 0; index < simpleRelation.Members.Count; ++index)
      {
        nullable1 = simpleRelation.Members[index].MemberId;
        long key = nullable1.Value;
        string memberRole = simpleRelation.Members[index].MemberRole;
        CompleteRelationMember completeRelationMember = new CompleteRelationMember();
        completeRelationMember.Role = memberRole;
        switch (simpleRelation.Members[index].MemberType.Value)
        {
          case OsmGeoType.Node:
            Node node = (Node) null;
            if (!nodes.TryGetValue(key, out node))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) node;
            break;
          case OsmGeoType.Way:
            CompleteWay completeWay = (CompleteWay) null;
            if (!ways.TryGetValue(key, out completeWay))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) completeWay;
            break;
          case OsmGeoType.Relation:
            CompleteRelation completeRelation2 = (CompleteRelation) null;
            if (!relations.TryGetValue(key, out completeRelation2))
              return (CompleteRelation) null;
            completeRelationMember.Member = (ICompleteOsmGeo) completeRelation2;
            break;
        }
        completeRelation1.Members.Add(completeRelationMember);
      }
      completeRelation1.TimeStamp = simpleRelation.TimeStamp;
      completeRelation1.User = simpleRelation.UserName;
      completeRelation1.UserId = simpleRelation.UserId;
      CompleteRelation completeRelation3 = completeRelation1;
      ulong? version = simpleRelation.Version;
      long? nullable2;
      if (!version.HasValue)
      {
        nullable1 = new long?();
        nullable2 = nullable1;
      }
      else
      {
        version = simpleRelation.Version;
        nullable2 = new long?((long) version.Value);
      }
      completeRelation3.Version = nullable2;
      completeRelation1.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
      return completeRelation1;
    }

    public static CompleteChangeSet CreateChangeSet(long id)
    {
      return new CompleteChangeSet(id);
    }

    public override string ToString()
    {
      return string.Format("http://www.openstreetmap.org/?relation={0}", (object) this.Id);
    }
  }
}
