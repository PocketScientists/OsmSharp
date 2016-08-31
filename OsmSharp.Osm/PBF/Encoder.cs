using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsmSharp.Osm.PBF
{
  public static class Encoder
  {
    public static string OSMHeader = "OSMHeader";
    public static string OSMData = "OSMData";

    public static bool Decode(this PrimitiveBlock block, IPBFOsmPrimitiveConsumer primitivesConsumer, bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      bool flag = false;
      if (block.primitivegroup != null)
      {
        foreach (PrimitiveGroup primitiveGroup in block.primitivegroup)
        {
          if (!ignoreNodes && primitiveGroup.dense != null)
          {
            int index1 = 0;
            long num1 = 0;
            long num2 = 0;
            long num3 = 0;
            long num4 = 0;
            long num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            for (int index2 = 0; index2 < primitiveGroup.dense.id.Count; ++index2)
            {
              num1 += primitiveGroup.dense.id[index2];
              num2 += primitiveGroup.dense.lat[index2];
              num3 += primitiveGroup.dense.lon[index2];
              if (primitiveGroup.dense.denseinfo != null)
              {
                num4 += primitiveGroup.dense.denseinfo.changeset[index2];
                num5 += primitiveGroup.dense.denseinfo.timestamp[index2];
                num6 += primitiveGroup.dense.denseinfo.uid[index2];
                num7 += primitiveGroup.dense.denseinfo.user_sid[index2];
                num8 += primitiveGroup.dense.denseinfo.version[index2];
              }
              Node node = new Node();
              node.id = num1;
              node.info = new Info();
              node.info.changeset = num4;
              node.info.timestamp = (int) num5;
              node.info.uid = num6;
              node.info.user_sid = num7;
              node.info.version = num8;
              node.lat = num2;
              node.lon = num3;
              int index3;
              for (List<int> keysVals = primitiveGroup.dense.keys_vals; keysVals.Count > index1 && keysVals[index1] != 0; index1 = index3 + 1)
              {
                node.keys.Add((uint) keysVals[index1]);
                index3 = index1 + 1;
                node.vals.Add((uint) keysVals[index3]);
              }
              ++index1;
              flag = true;
              primitivesConsumer.ProcessNode(block, node);
            }
          }
          else
          {
            if (!ignoreNodes && primitiveGroup.nodes != null)
            {
              foreach (Node node in primitiveGroup.nodes)
              {
                flag = true;
                primitivesConsumer.ProcessNode(block, node);
              }
            }
            if (!ignoreWays && primitiveGroup.ways != null)
            {
              foreach (Way way in primitiveGroup.ways)
              {
                flag = true;
                primitivesConsumer.ProcessWay(block, way);
              }
            }
            if (!ignoreRelations && primitiveGroup.relations != null)
            {
              foreach (Relation relation in primitiveGroup.relations)
              {
                flag = true;
                primitivesConsumer.ProcessRelation(block, relation);
              }
            }
          }
        }
      }
      return flag;
    }

    public static void Encode(this PrimitiveBlock block, Dictionary<string, int> reverseStringTable, List<OsmGeo> osmGeos)
    {
      int index1 = 0;
      int index2 = 0;
      int index3 = 0;
      if (block.stringtable != null && block.stringtable.s != null)
        block.stringtable.s.Clear();
      while (index2 < osmGeos.Count)
      {
        PrimitiveGroup primitiveGroup;
        if (index1 < block.primitivegroup.Count)
        {
          primitiveGroup = block.primitivegroup[index1] ?? new PrimitiveGroup();
          if (primitiveGroup.dense != null)
          {
            if (primitiveGroup.dense.denseinfo != null)
            {
              if (primitiveGroup.dense.denseinfo.changeset != null)
                primitiveGroup.dense.denseinfo.changeset.Clear();
              if (primitiveGroup.dense.denseinfo.timestamp != null)
                primitiveGroup.dense.denseinfo.timestamp.Clear();
              if (primitiveGroup.dense.denseinfo.uid != null)
                primitiveGroup.dense.denseinfo.uid.Clear();
              if (primitiveGroup.dense.denseinfo.user_sid != null)
                primitiveGroup.dense.denseinfo.user_sid.Clear();
              if (primitiveGroup.dense.denseinfo.version != null)
                primitiveGroup.dense.denseinfo.version.Clear();
            }
            if (primitiveGroup.dense.id != null)
              primitiveGroup.dense.id.Clear();
            if (primitiveGroup.dense.keys_vals != null)
              primitiveGroup.dense.keys_vals.Clear();
            if (primitiveGroup.dense.lat != null)
              primitiveGroup.dense.lat.Clear();
            if (primitiveGroup.dense.lon != null)
              primitiveGroup.dense.lon.Clear();
          }
          if (primitiveGroup.changesets != null)
            primitiveGroup.changesets.Clear();
          if (primitiveGroup.ways != null)
            primitiveGroup.ways.Clear();
          if (primitiveGroup.relations != null)
            primitiveGroup.relations.Clear();
        }
        else
        {
          primitiveGroup = new PrimitiveGroup();
          block.primitivegroup.Add(primitiveGroup);
        }
        OsmGeoType type = osmGeos[index2].Type;
        OsmGeo osmGeo = osmGeos[index2];
        for (; index2 < osmGeos.Count && osmGeos[index2].Type == type; ++index2)
        {
          switch (type)
          {
            case OsmGeoType.Node:
              if (primitiveGroup.nodes.Count > index3)
                Encoder.EncodeNode(block, reverseStringTable, primitiveGroup.nodes[index3], osmGeos[index2] as OsmSharp.Osm.Node);
              else
                primitiveGroup.nodes.Add(Encoder.EncodeNode(block, reverseStringTable, osmGeos[index2] as OsmSharp.Osm.Node));
              ++index3;
              break;
            case OsmGeoType.Way:
              primitiveGroup.ways.Add(Encoder.EncodeWay(block, reverseStringTable, osmGeos[index2] as OsmSharp.Osm.Way));
              break;
            case OsmGeoType.Relation:
              primitiveGroup.relations.Add(Encoder.EncodeRelation(block, reverseStringTable, osmGeos[index2] as OsmSharp.Osm.Relation));
              break;
          }
        }
        if (primitiveGroup.nodes != null)
        {
          while (index3 < primitiveGroup.nodes.Count)
            primitiveGroup.nodes.RemoveAt(index3);
        }
        ++index1;
      }
      while (index1 < block.primitivegroup.Count)
        block.primitivegroup.RemoveAt(index1);
    }

    public static OsmSharp.Osm.Node DecodeNode(PrimitiveBlock block, Node pbfNode, OsmSharp.Osm.Node node)
    {
      if (node.Tags != null)
        node.Tags.Clear();
      if (node.Tags == null)
        node.Tags = (TagsCollectionBase) new TagsCollection();
      node.ChangeSetId = new long?(pbfNode.info.changeset);
      node.Id = new long?(pbfNode.id);
      node.Latitude = new double?(Encoder.DecodeLatLon(pbfNode.lat, block.lat_offset, (long) block.granularity));
      node.Longitude = new double?(Encoder.DecodeLatLon(pbfNode.lon, block.lon_offset, (long) block.granularity));
      for (int index = 0; index < pbfNode.keys.Count; ++index)
        node.Tags.Add(new Tag()
        {
          Key = Encoding.UTF8.GetString(block.stringtable.s[(int) pbfNode.keys[index]]),
          Value = Encoding.UTF8.GetString(block.stringtable.s[(int) pbfNode.vals[index]])
        });
      if (pbfNode.info != null)
      {
        node.TimeStamp = new DateTime?(Encoder.DecodeTimestamp(pbfNode.info.timestamp, (long) block.date_granularity));
        node.Visible = new bool?(true);
        node.Version = new ulong?((ulong) (uint) pbfNode.info.version);
        node.UserId = new long?((long) pbfNode.info.uid);
        node.UserName = (string) null;
        if (block.stringtable != null)
          node.UserName = Encoding.UTF8.GetString(block.stringtable.s[pbfNode.info.user_sid]);
        node.Version = new ulong?((ulong) pbfNode.info.version);
      }
      node.Visible = new bool?(true);
      return node;
    }

    public static OsmSharp.Osm.Node DecodeNode(PrimitiveBlock block, Node pbfNode)
    {
      OsmSharp.Osm.Node node = new OsmSharp.Osm.Node();
      Encoder.DecodeNode(block, pbfNode, node);
      return node;
    }

    public static int EncodeString(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, string value)
    {
      if (value == null)
        return 0;
      if (block.stringtable == null)
      {
        block.stringtable = new StringTable();
        block.stringtable.s.Add(Encoding.UTF8.GetBytes(string.Empty));
        reverseStringTable.Add(string.Empty, 0);
      }
      int num;
      if (reverseStringTable.TryGetValue(value, out num))
        return num;
      Encoding.UTF8.GetBytes(value);
      block.stringtable.s.Add(Encoding.UTF8.GetBytes(value));
      reverseStringTable.Add(value, block.stringtable.s.Count - 1);
      return block.stringtable.s.Count - 1;
    }

    public static Node EncodeNode(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, OsmSharp.Osm.Node node)
    {
      Node pbfNode = new Node();
      Encoder.EncodeNode(block, reverseStringTable, pbfNode, node);
      return pbfNode;
    }

    public static Node EncodeNode(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, Node pbfNode, OsmSharp.Osm.Node node)
    {
      pbfNode.id = node.Id.Value;
      pbfNode.info = new Info();
      pbfNode.info.version = 0;
      pbfNode.info.changeset = !node.ChangeSetId.HasValue ? 0L : node.ChangeSetId.Value;
      DateTime? timeStamp = node.TimeStamp;
      if (timeStamp.HasValue)
      {
        Info info = pbfNode.info;
        timeStamp = node.TimeStamp;
        int num = Encoder.EncodeTimestamp(timeStamp.Value, (long) block.date_granularity);
        info.timestamp = num;
      }
      else
        pbfNode.info.timestamp = 0;
      long? userId = node.UserId;
      if (userId.HasValue)
      {
        Info info = pbfNode.info;
        userId = node.UserId;
        int num = (int) userId.Value;
        info.uid = num;
      }
      else
        pbfNode.info.uid = 0;
      pbfNode.info.user_sid = Encoder.EncodeString(block, reverseStringTable, node.UserName);
      pbfNode.info.version = !node.Version.HasValue ? 0 : (int) node.Version.Value;
      pbfNode.lat = Encoder.EncodeLatLon(node.Latitude.Value, block.lat_offset, (long) block.granularity);
      pbfNode.lon = Encoder.EncodeLatLon(node.Longitude.Value, block.lon_offset, (long) block.granularity);
      if (node.Tags != null)
      {
        foreach (Tag tag in node.Tags)
        {
          pbfNode.keys.Add((uint) Encoder.EncodeString(block, reverseStringTable, tag.Key));
          pbfNode.vals.Add((uint) Encoder.EncodeString(block, reverseStringTable, tag.Value));
        }
      }
      else
      {
        pbfNode.keys.Clear();
        pbfNode.vals.Clear();
      }
      return pbfNode;
    }

    public static void DecodeWay(PrimitiveBlock block, Way pbfWay, OsmSharp.Osm.Way way)
    {
      if (way.Nodes != null && way.Nodes.Count > 0)
        way.Nodes.Clear();
      if (way.Tags != null)
        way.Tags.Clear();
      if (way.Nodes == null)
        way.Nodes = new List<long>(pbfWay.refs.Count);
      if (way.Tags == null)
        way.Tags = (TagsCollectionBase) new TagsCollection(pbfWay.keys.Count);
      way.Id = new long?(pbfWay.id);
      if (pbfWay.refs.Count > 0)
      {
        long num = 0;
        for (int index = 0; index < pbfWay.refs.Count; ++index)
        {
          num += pbfWay.refs[index];
          way.Nodes.Add(num);
        }
      }
      if (pbfWay.keys.Count > 0)
      {
        for (int index = 0; index < pbfWay.keys.Count; ++index)
        {
          string key = Encoding.UTF8.GetString(block.stringtable.s[(int) pbfWay.keys[index]]);
          string str = Encoding.UTF8.GetString(block.stringtable.s[(int) pbfWay.vals[index]]);
          way.Tags.Add(new Tag(key, str));
        }
      }
      if (pbfWay.info != null)
      {
        way.ChangeSetId = new long?(pbfWay.info.changeset);
        way.TimeStamp = new DateTime?(Encoder.DecodeTimestamp(pbfWay.info.timestamp, (long) block.date_granularity));
        way.UserId = new long?((long) pbfWay.info.uid);
        way.UserName = (string) null;
        if (block.stringtable != null)
          way.UserName = Encoding.UTF8.GetString(block.stringtable.s[pbfWay.info.user_sid]);
        way.Version = new ulong?((ulong) pbfWay.info.version);
      }
      way.Visible = new bool?(true);
    }

    public static OsmSharp.Osm.Way DecodeWay(PrimitiveBlock block, Way pbfWay)
    {
      OsmSharp.Osm.Way way = new OsmSharp.Osm.Way();
      Encoder.DecodeWay(block, pbfWay, way);
      return way;
    }

    public static Way EncodeWay(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, OsmSharp.Osm.Way way)
    {
      Way way1 = new Way();
      way1.id = way.Id.Value;
      way1.info = new Info();
      long? nullable;
      if (way.ChangeSetId.HasValue)
      {
        Info info = way1.info;
        nullable = way.ChangeSetId;
        long num = nullable.Value;
        info.changeset = num;
      }
      if (way.TimeStamp.HasValue)
        way1.info.timestamp = Encoder.EncodeTimestamp(way.TimeStamp.Value, (long) block.date_granularity);
      nullable = way.UserId;
      if (nullable.HasValue)
      {
        Info info = way1.info;
        nullable = way.UserId;
        int num = (int) nullable.Value;
        info.uid = num;
      }
      way1.info.user_sid = Encoder.EncodeString(block, reverseStringTable, way.UserName);
      way1.info.version = 0;
      if (way.Version.HasValue)
        way1.info.version = (int) way.Version.Value;
      if (way.Tags != null)
      {
        foreach (Tag tag in way.Tags)
        {
          way1.keys.Add((uint) Encoder.EncodeString(block, reverseStringTable, tag.Key));
          way1.vals.Add((uint) Encoder.EncodeString(block, reverseStringTable, tag.Value));
        }
      }
      if (way.Nodes != null && way.Nodes.Count > 0)
      {
        way1.refs.Add(way.Nodes[0]);
        for (int index = 1; index < way.Nodes.Count; ++index)
          way1.refs.Add(way.Nodes[index] - way.Nodes[index - 1]);
      }
      return way1;
    }

    public static OsmSharp.Osm.Relation DecodeRelation(PrimitiveBlock block, Relation pbfRelation, OsmSharp.Osm.Relation relation)
    {
      if (relation.Members != null && relation.Members.Count > 0)
        relation.Members.Clear();
      if (relation.Tags != null)
        relation.Tags.Clear();
      if (relation.Members == null)
        relation.Members = new List<RelationMember>(pbfRelation.memids.Count);
      if (relation.Tags == null)
        relation.Tags = (TagsCollectionBase) new TagsCollection(pbfRelation.keys.Count);
      relation.Id = new long?(pbfRelation.id);
      long num = 0;
      for (int index = 0; index < pbfRelation.types.Count; ++index)
      {
        num += pbfRelation.memids[index];
        string str = Encoding.UTF8.GetString(block.stringtable.s[pbfRelation.roles_sid[index]]);
        RelationMember relationMember = new RelationMember();
        relationMember.MemberId = new long?(num);
        relationMember.MemberRole = str;
        switch (pbfRelation.types[index])
        {
          case Relation.MemberType.NODE:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Node);
            break;
          case Relation.MemberType.WAY:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Way);
            break;
          case Relation.MemberType.RELATION:
            relationMember.MemberType = new OsmGeoType?(OsmGeoType.Relation);
            break;
        }
        relation.Members.Add(relationMember);
      }
      for (int index = 0; index < pbfRelation.keys.Count; ++index)
      {
        string key = Encoding.UTF8.GetString(block.stringtable.s[(int) pbfRelation.keys[index]]);
        string str = Encoding.UTF8.GetString(block.stringtable.s[(int) pbfRelation.vals[index]]);
        relation.Tags.Add(new Tag(key, str));
      }
      if (pbfRelation.info != null)
      {
        relation.ChangeSetId = new long?(pbfRelation.info.changeset);
        relation.TimeStamp = new DateTime?(Encoder.DecodeTimestamp(pbfRelation.info.timestamp, (long) block.date_granularity));
        relation.UserId = new long?((long) pbfRelation.info.uid);
        relation.UserName = (string) null;
        if (block.stringtable != null)
          relation.UserName = Encoding.UTF8.GetString(block.stringtable.s[pbfRelation.info.user_sid]);
        relation.Version = new ulong?((ulong) pbfRelation.info.version);
      }
      relation.Visible = new bool?(true);
      return relation;
    }

    public static OsmSharp.Osm.Relation DecodeRelation(PrimitiveBlock block, Relation pbfRelation)
    {
      OsmSharp.Osm.Relation relation = new OsmSharp.Osm.Relation();
      Encoder.DecodeRelation(block, pbfRelation, relation);
      return relation;
    }

    public static Relation EncodeRelation(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, OsmSharp.Osm.Relation relation)
    {
      Relation relation1 = new Relation();
      relation1.id = relation.Id.Value;
      relation1.info = new Info();
      long? nullable;
      if (relation.ChangeSetId.HasValue)
      {
        Info info = relation1.info;
        nullable = relation.ChangeSetId;
        long num = nullable.Value;
        info.changeset = num;
      }
      if (relation.TimeStamp.HasValue)
        relation1.info.timestamp = Encoder.EncodeTimestamp(relation.TimeStamp.Value, (long) block.date_granularity);
      nullable = relation.UserId;
      if (nullable.HasValue)
      {
        Info info = relation1.info;
        nullable = relation.UserId;
        int num = (int) nullable.Value;
        info.uid = num;
      }
      relation1.info.user_sid = Encoder.EncodeString(block, reverseStringTable, relation.UserName);
      relation1.info.version = 0;
      if (relation.Version.HasValue)
        relation1.info.version = (int) relation.Version.Value;
      if (relation.Tags != null)
      {
        foreach (Tag tag in relation.Tags)
        {
          relation1.keys.Add((uint) Encoder.EncodeString(block, reverseStringTable, tag.Key));
          relation1.vals.Add((uint) Encoder.EncodeString(block, reverseStringTable, tag.Value));
        }
      }
      if (relation.Members != null && relation.Members.Count > 0)
      {
        List<long> memids1 = relation1.memids;
        nullable = relation.Members[0].MemberId;
        long num1 = nullable.Value;
        memids1.Add(num1);
        relation1.roles_sid.Add(Encoder.EncodeString(block, reverseStringTable, relation.Members[0].MemberRole));
        OsmGeoType? memberType = relation.Members[0].MemberType;
        switch (memberType.Value)
        {
          case OsmGeoType.Node:
            relation1.types.Add(Relation.MemberType.NODE);
            break;
          case OsmGeoType.Way:
            relation1.types.Add(Relation.MemberType.WAY);
            break;
          case OsmGeoType.Relation:
            relation1.types.Add(Relation.MemberType.RELATION);
            break;
        }
        for (int index = 1; index < relation.Members.Count; ++index)
        {
          List<long> memids2 = relation1.memids;
          nullable = relation.Members[index].MemberId;
          long num2 = nullable.Value;
          nullable = relation.Members[index - 1].MemberId;
          long num3 = nullable.Value;
          long num4 = num2 - num3;
          memids2.Add(num4);
          relation1.roles_sid.Add(Encoder.EncodeString(block, reverseStringTable, relation.Members[index].MemberRole));
          memberType = relation.Members[index].MemberType;
          switch (memberType.Value)
          {
            case OsmGeoType.Node:
              relation1.types.Add(Relation.MemberType.NODE);
              break;
            case OsmGeoType.Way:
              relation1.types.Add(Relation.MemberType.WAY);
              break;
            case OsmGeoType.Relation:
              relation1.types.Add(Relation.MemberType.RELATION);
              break;
          }
        }
      }
      return relation1;
    }

    public static long EncodeLatLon(double value, long offset, long granularity)
    {
      return ((long) (value / 1E-09) - offset) / granularity;
    }

    public static double DecodeLatLon(long valueOffset, long offset, long granularity)
    {
      return 1E-09 * (double) (offset + granularity * valueOffset);
    }

    public static int EncodeTimestamp(DateTime timestamp, long dateGranularity)
    {
      return (int) (timestamp.ToUnixTime() / dateGranularity);
    }

    public static DateTime DecodeTimestamp(int timestamp, long dateGranularity)
    {
      return ((long) timestamp * dateGranularity).FromUnixTime();
    }
  }
}
