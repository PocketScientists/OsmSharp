using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Xml.v0_6;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Xml.Streams
{
  internal static class XmlSimpleConverter
  {
    internal static ChangeSet ConvertToSimple(delete delete)
    {
      ChangeSet changeSet = new ChangeSet();
      Change change = new Change();
      change.Type = ChangeType.Delete;
      change.OsmGeo = new List<OsmGeo>();
      if (delete.relation != null)
      {
        foreach (relation re in delete.relation)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(re));
      }
      if (delete.way != null)
      {
        foreach (way wa in delete.way)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(wa));
      }
      if (delete.node != null)
      {
        foreach (node nd in delete.node)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(nd));
      }
      changeSet.Changes = new List<Change>();
      changeSet.Changes.Add(change);
      return changeSet;
    }

    internal static ChangeSet ConvertToSimple(modify modify)
    {
      ChangeSet changeSet = new ChangeSet();
      Change change = new Change();
      change.Type = ChangeType.Modify;
      change.OsmGeo = new List<OsmGeo>();
      if (modify.relation != null)
      {
        foreach (relation re in modify.relation)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(re));
      }
      if (modify.way != null)
      {
        foreach (way wa in modify.way)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(wa));
      }
      if (modify.node != null)
      {
        foreach (node nd in modify.node)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(nd));
      }
      changeSet.Changes = new List<Change>();
      changeSet.Changes.Add(change);
      return changeSet;
    }

    internal static ChangeSet ConvertToSimple(create create)
    {
      ChangeSet changeSet = new ChangeSet();
      Change change = new Change();
      change.Type = ChangeType.Create;
      change.OsmGeo = new List<OsmGeo>();
      if (create.node != null)
      {
        foreach (node nd in create.node)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(nd));
      }
      if (create.way != null)
      {
        foreach (way wa in create.way)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(wa));
      }
      if (create.relation != null)
      {
        foreach (relation re in create.relation)
          change.OsmGeo.Add((OsmGeo) XmlSimpleConverter.ConvertToSimple(re));
      }
      changeSet.Changes = new List<Change>();
      changeSet.Changes.Add(change);
      return changeSet;
    }

    internal static Node ConvertToSimple(node nd)
    {
      Node node = new Node();
      if (nd.idSpecified)
        node.Id = new long?(nd.id);
      if (nd.changesetSpecified)
        node.ChangeSetId = new long?(nd.changeset);
      if (nd.visibleSpecified)
        node.Visible = new bool?(nd.visible);
      else
        node.Visible = new bool?(true);
      if (nd.timestampSpecified)
        node.TimeStamp = new DateTime?(nd.timestamp);
      if (nd.latSpecified)
        node.Latitude = new double?(nd.lat);
      if (nd.lonSpecified)
        node.Longitude = new double?(nd.lon);
      if (nd.uidSpecified)
        node.UserId = new long?(nd.uid);
      if (nd.versionSpecified)
        node.Version = new ulong?(nd.version);
      node.UserName = nd.user;
      node.Tags = XmlSimpleConverter.ConvertToTags(nd.tag);
      return node;
    }

    internal static Way ConvertToSimple(way wa)
    {
      Way way = new Way();
      if (wa.idSpecified)
        way.Id = new long?(wa.id);
      if (wa.changesetSpecified)
        way.ChangeSetId = new long?(wa.changeset);
      if (wa.visibleSpecified)
        way.Visible = new bool?(wa.visible);
      else
        way.Visible = new bool?(true);
      if (wa.timestampSpecified)
        way.TimeStamp = new DateTime?(wa.timestamp);
      if (wa.uidSpecified)
        way.UserId = new long?(wa.uid);
      if (wa.versionSpecified)
        way.Version = new ulong?(wa.version);
      way.UserName = wa.user;
      way.Tags = XmlSimpleConverter.ConvertToTags(wa.tag);
      if (wa.nd != null && wa.nd.Length != 0)
      {
        way.Nodes = new List<long>();
        for (int index = 0; index < wa.nd.Length; ++index)
          way.Nodes.Add(wa.nd[index].@ref);
      }
      return way;
    }

    internal static Relation ConvertToSimple(relation re)
    {
      Relation relation = new Relation();
      if (re.idSpecified)
        relation.Id = new long?(re.id);
      if (re.changesetSpecified)
        relation.ChangeSetId = new long?(re.changeset);
      if (re.visibleSpecified)
        relation.Visible = new bool?(re.visible);
      else
        relation.Visible = new bool?(true);
      if (re.timestampSpecified)
        relation.TimeStamp = new DateTime?(re.timestamp);
      if (re.uidSpecified)
        relation.UserId = new long?(re.uid);
      if (re.versionSpecified)
        relation.Version = new ulong?(re.version);
      relation.UserName = re.user;
      relation.Tags = XmlSimpleConverter.ConvertToTags(re.tag);
      if (re.member != null && re.member.Length != 0)
      {
        relation.Members = new List<RelationMember>();
        for (int index = 0; index < re.member.Length; ++index)
        {
          member member = re.member[index];
          RelationMember relationMember = new RelationMember();
          if (member.refSpecified)
            relationMember.MemberId = new long?(member.@ref);
          relationMember.MemberRole = member.role;
          if (member.typeSpecified)
          {
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
          }
          relation.Members.Add(relationMember);
        }
      }
      return relation;
    }

    private static TagsCollectionBase ConvertToTags(tag[] tag)
    {
      TagsCollectionBase tagsCollectionBase = (TagsCollectionBase) null;
      if (tag != null && tag.Length != 0)
      {
        tagsCollectionBase = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag1 in tag)
          tagsCollectionBase.Add(tag1.k, tag1.v);
      }
      return tagsCollectionBase;
    }
  }
}
