using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public static class Extensions
  {
    public static GeoCoordinateBox ConvertFrom(this bounds bounds)
    {
      if (bounds != null)
        return new GeoCoordinateBox(new GeoCoordinate(bounds.maxlat, bounds.maxlon), new GeoCoordinate(bounds.minlat, bounds.minlon));
      return (GeoCoordinateBox) null;
    }

    public static GeoCoordinateBox ConvertFrom(this bound bound)
    {
      if (bound == null)
        return (GeoCoordinateBox) null;
      string[] strArray = bound.box.Split(',');
      return new GeoCoordinateBox(new GeoCoordinate(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture)), new GeoCoordinate(double.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture)));
    }

    public static Relation ConvertFrom(this relation xmlRelation)
    {
      Relation relation = new Relation();
      relation.Id = new long?(xmlRelation.id);
      relation.Members = new List<RelationMember>(xmlRelation.member.Length);
      for (int index = 0; index < xmlRelation.member.Length; ++index)
      {
        member member = xmlRelation.member[index];
        RelationMember relationMember = new RelationMember();
        relationMember.MemberId = new long?(member.@ref);
        relationMember.MemberRole = member.role;
        if (!member.refSpecified || !member.typeSpecified)
          return (Relation) null;
        switch (member.type)
        {
          case memberType.node:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Node);
            break;
          case memberType.way:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Way);
            break;
          case memberType.relation:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Relation);
            break;
        }
        relation.Members.Add(relationMember);
      }
      if (xmlRelation.tag != null)
      {
        relation.Tags = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag in xmlRelation.tag)
          relation.Tags.Add(tag.k, tag.v);
      }
      if (xmlRelation.uidSpecified)
        relation.UserId = new long?(xmlRelation.uid);
      relation.UserName = xmlRelation.user;
      if (xmlRelation.changesetSpecified)
        relation.ChangeSetId = new long?(xmlRelation.changeset);
      if (xmlRelation.timestampSpecified)
        relation.TimeStamp = new DateTime?(xmlRelation.timestamp);
      if (xmlRelation.versionSpecified)
        relation.Version = new ulong?(xmlRelation.version);
      relation.Visible = new bool?(xmlRelation.visible);
      return relation;
    }

    public static Way ConvertFrom(this way xmlWay)
    {
      Way way = new Way();
      way.Id = new long?(xmlWay.id);
      way.Nodes = new List<long>(xmlWay.nd.Length);
      for (int index = 0; index < xmlWay.nd.Length; ++index)
      {
        nd nd = xmlWay.nd[index];
        if (!nd.refSpecified)
          return (Way) null;
        way.Nodes.Add(nd.@ref);
      }
      if (xmlWay.tag != null)
      {
        way.Tags = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag in xmlWay.tag)
          way.Tags.Add(tag.k, tag.v);
      }
      if (xmlWay.uidSpecified)
        way.UserId = new long?(xmlWay.uid);
      way.UserName = xmlWay.user;
      if (xmlWay.changesetSpecified)
        way.ChangeSetId = new long?(xmlWay.changeset);
      if (xmlWay.timestampSpecified)
        way.TimeStamp = new DateTime?(xmlWay.timestamp);
      if (xmlWay.versionSpecified)
        way.Version = new ulong?(xmlWay.version);
      way.Visible = new bool?(xmlWay.visible);
      return way;
    }

    public static Node ConvertFrom(this node xmlNode)
    {
      Node node = new Node();
      node.Id = new long?(xmlNode.id);
      if (!xmlNode.latSpecified || !xmlNode.lonSpecified)
        throw new ArgumentNullException("Latitude and/or longitude cannot be null!");
      node.Latitude = new double?(xmlNode.lat);
      node.Longitude = new double?(xmlNode.lon);
      if (xmlNode.tag != null)
      {
        node.Tags = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag in xmlNode.tag)
          node.Tags.Add(tag.k, tag.v);
      }
      if (xmlNode.uidSpecified)
        node.UserId = new long?((long) (int) xmlNode.uid);
      node.UserName = xmlNode.user;
      if (xmlNode.changesetSpecified)
        node.ChangeSetId = new long?((long) (int) xmlNode.changeset);
      if (xmlNode.timestampSpecified)
        node.TimeStamp = new DateTime?(xmlNode.timestamp);
      if (xmlNode.versionSpecified)
        node.Version = new ulong?(xmlNode.version);
      node.Visible = new bool?(xmlNode.visible);
      return node;
    }

    public static bounds ConvertTo(this GeoCoordinateBox box)
    {
      bounds bounds = new bounds();
      bounds.maxlat = box.MaxLat;
      int num1 = 1;
      bounds.maxlatSpecified = num1 != 0;
      double maxLon = box.MaxLon;
      bounds.maxlon = maxLon;
      int num2 = 1;
      bounds.maxlonSpecified = num2 != 0;
      double minLat = box.MinLat;
      bounds.minlat = minLat;
      int num3 = 1;
      bounds.minlatSpecified = num3 != 0;
      double minLon = box.MinLon;
      bounds.minlon = minLon;
      int num4 = 1;
      bounds.minlonSpecified = num4 != 0;
      return bounds;
    }

    public static node ConvertTo(this Node node)
    {
      node node1 = new node();
      long? nullable1;
      if (node.ChangeSetId.HasValue)
      {
        node node2 = node1;
        nullable1 = node.ChangeSetId;
        long num = nullable1.Value;
        node2.changeset = num;
        node1.changesetSpecified = true;
      }
      nullable1 = node.Id;
      if (nullable1.HasValue)
      {
        node node2 = node1;
        nullable1 = node.Id;
        long num = nullable1.Value;
        node2.id = num;
        node1.idSpecified = true;
      }
      if (node.Tags != null)
      {
        node1.tag = new tag[node.Tags.Count];
        int index = 0;
        foreach (Tag tag in node.Tags)
        {
          node1.tag[index] = new tag()
          {
            k = tag.Key,
            v = tag.Value
          };
          ++index;
        }
      }
      if (node.TimeStamp.HasValue)
      {
        node1.timestamp = node.TimeStamp.Value;
        node1.timestampSpecified = true;
      }
      nullable1 = node.UserId;
      if (nullable1.HasValue)
      {
        node node2 = node1;
        nullable1 = node.UserId;
        long num = nullable1.Value;
        node2.uid = num;
        node1.uidSpecified = true;
      }
      node1.user = node1.user;
      ulong? version = node.Version;
      if (version.HasValue)
      {
        node node2 = node1;
        version = node.Version;
        long num = (long) version.Value;
        node2.version = (ulong) num;
        node1.versionSpecified = true;
      }
      if (node.Visible.HasValue)
      {
        node1.visible = node.Visible.Value;
        node1.visibleSpecified = true;
      }
      double? nullable2;
      if (node.Latitude.HasValue)
      {
        node node2 = node1;
        nullable2 = node.Latitude;
        double num = nullable2.Value;
        node2.lat = num;
        node1.latSpecified = true;
      }
      nullable2 = node.Longitude;
      if (nullable2.HasValue)
      {
        node node2 = node1;
        nullable2 = node.Longitude;
        double num = nullable2.Value;
        node2.lon = num;
        node1.lonSpecified = true;
      }
      return node1;
    }

    public static way ConvertTo(this Way way)
    {
      way way1 = new way();
      long? nullable;
      if (way.ChangeSetId.HasValue)
      {
        way way2 = way1;
        nullable = way.ChangeSetId;
        long num = nullable.Value;
        way2.changeset = num;
        way1.changesetSpecified = true;
      }
      nullable = way.Id;
      if (nullable.HasValue)
      {
        way way2 = way1;
        nullable = way.Id;
        long num = nullable.Value;
        way2.id = num;
        way1.idSpecified = true;
      }
      else
        way1.idSpecified = false;
      if (way.Tags != null)
      {
        way1.tag = new tag[way.Tags.Count];
        int index = 0;
        foreach (Tag tag in way.Tags)
        {
          way1.tag[index] = new tag()
          {
            k = tag.Key,
            v = tag.Value
          };
          ++index;
        }
      }
      if (way.TimeStamp.HasValue)
      {
        way1.timestamp = way.TimeStamp.Value;
        way1.timestampSpecified = true;
      }
      nullable = way.UserId;
      if (nullable.HasValue)
      {
        way way2 = way1;
        nullable = way.UserId;
        long num = nullable.Value;
        way2.uid = num;
        way1.uidSpecified = true;
      }
      way1.user = way1.user;
      ulong? version = way.Version;
      if (version.HasValue)
      {
        way way2 = way1;
        version = way.Version;
        long num = (long) version.Value;
        way2.version = (ulong) num;
        way1.versionSpecified = true;
      }
      if (way.Visible.HasValue)
      {
        way1.visible = way.Visible.Value;
        way1.visibleSpecified = true;
      }
      else
        way1.visibleSpecified = false;
      way1.nd = new nd[way.Nodes.Count];
      for (int index = 0; index < way.Nodes.Count; ++index)
        way1.nd[index] = new nd()
        {
          @ref = way.Nodes[index],
          refSpecified = true
        };
      return way1;
    }

    public static relation ConvertTo(this Relation relation)
    {
      relation relation1 = new relation();
      long? nullable;
      if (relation.ChangeSetId.HasValue)
      {
        relation relation2 = relation1;
        nullable = relation.ChangeSetId;
        long num = nullable.Value;
        relation2.changeset = num;
        relation1.changesetSpecified = true;
      }
      nullable = relation.Id;
      if (nullable.HasValue)
      {
        relation relation2 = relation1;
        nullable = relation.Id;
        long num = nullable.Value;
        relation2.id = num;
        relation1.idSpecified = true;
      }
      else
        relation1.idSpecified = false;
      if (relation.Tags != null)
      {
        relation1.tag = new tag[relation.Tags.Count];
        int index = 0;
        foreach (Tag tag in relation.Tags)
        {
          relation1.tag[index] = new tag()
          {
            k = tag.Key,
            v = tag.Value
          };
          ++index;
        }
      }
      if (relation.TimeStamp.HasValue)
      {
        relation1.timestamp = relation.TimeStamp.Value;
        relation1.timestampSpecified = true;
      }
      nullable = relation.UserId;
      if (nullable.HasValue)
      {
        relation relation2 = relation1;
        nullable = relation.UserId;
        long num = nullable.Value;
        relation2.uid = num;
        relation1.uidSpecified = true;
      }
      relation1.user = relation1.user;
      ulong? version = relation.Version;
      if (version.HasValue)
      {
        relation relation2 = relation1;
        version = relation.Version;
        long num = (long) version.Value;
        relation2.version = (ulong) num;
        relation1.versionSpecified = true;
      }
      if (relation.Visible.HasValue)
      {
        relation1.visible = relation.Visible.Value;
        relation1.visibleSpecified = true;
      }
      else
        relation1.visibleSpecified = false;
      relation1.member = new member[relation.Members.Count];
      for (int index = 0; index < relation.Members.Count; ++index)
      {
        RelationMember member1 = relation.Members[index];
        member member2 = new member();
        if (member1.MemberType.HasValue)
        {
          switch (member1.MemberType.Value)
          {
            case OsmGeoType.Node:
              member2.type = memberType.node;
              member2.typeSpecified = true;
              break;
            case OsmGeoType.Way:
              member2.type = memberType.way;
              member2.typeSpecified = true;
              break;
            case OsmGeoType.Relation:
              member2.type = memberType.relation;
              member2.typeSpecified = true;
              break;
          }
        }
        else
          member2.typeSpecified = false;
        nullable = member1.MemberId;
        if (nullable.HasValue)
        {
          member member3 = member2;
          nullable = member1.MemberId;
          long num = nullable.Value;
          member3.@ref = num;
          member2.refSpecified = true;
        }
        else
          member2.refSpecified = false;
        member2.role = member1.MemberRole;
        relation1.member[index] = member2;
      }
      return relation1;
    }
  }
}
