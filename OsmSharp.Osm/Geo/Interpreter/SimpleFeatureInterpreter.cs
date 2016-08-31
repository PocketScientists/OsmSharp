using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Osm.Geo.Interpreter
{
  public class SimpleFeatureInterpreter : FeatureInterpreter
  {
    public override FeatureCollection Interpret(ICompleteOsmGeo osmObject)
    {
      FeatureCollection featureCollection = new FeatureCollection();
      if (osmObject != null)
      {
        switch (osmObject.Type)
        {
          case CompleteOsmType.Node:
            TagsCollection tagsCollection = new TagsCollection((IEnumerable<Tag>) osmObject.Tags);
            string key1 = "FIXME";
            tagsCollection.RemoveKey(key1);
            string key2 = "node";
            tagsCollection.RemoveKey(key2);
            string key3 = "source";
            tagsCollection.RemoveKey(key3);
            if (tagsCollection.Count > 0)
            {
              featureCollection.Add(new Feature((Geometry) new Point((osmObject as Node).Coordinate), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) osmObject.Tags)));
              break;
            }
            break;
          case CompleteOsmType.Way:
            TagsCollectionBase tags = osmObject.Tags;
            bool flag = false;
            if (tags.ContainsKey("building") && !tags.IsFalse("building") || tags.ContainsKey("landuse") && !tags.IsFalse("landuse") || (tags.ContainsKey("amenity") && !tags.IsFalse("amenity") || tags.ContainsKey("harbour") && !tags.IsFalse("harbour")) || (tags.ContainsKey("historic") && !tags.IsFalse("historic") || tags.ContainsKey("leisure") && !tags.IsFalse("leisure") || (tags.ContainsKey("man_made") && !tags.IsFalse("man_made") || tags.ContainsKey("military") && !tags.IsFalse("military"))) || (tags.ContainsKey("natural") && !tags.IsFalse("natural") || tags.ContainsKey("office") && !tags.IsFalse("office") || (tags.ContainsKey("place") && !tags.IsFalse("place") || tags.ContainsKey("power") && !tags.IsFalse("power")) || (tags.ContainsKey("public_transport") && !tags.IsFalse("public_transport") || tags.ContainsKey("shop") && !tags.IsFalse("shop") || (tags.ContainsKey("sport") && !tags.IsFalse("sport") || tags.ContainsKey("tourism") && !tags.IsFalse("tourism")))) || (tags.ContainsKey("waterway") && !tags.IsFalse("waterway") || tags.ContainsKey("wetland") && !tags.IsFalse("wetland") || (tags.ContainsKey("water") && !tags.IsFalse("water") || tags.ContainsKey("aeroway") && !tags.IsFalse("aeroway"))))
              flag = true;
            if (tags.IsTrue("area"))
              flag = true;
            else if (tags.IsFalse("area"))
              flag = false;
            if (flag)
            {
              Feature feature = new Feature((Geometry) new LineairRing((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) tags));
              featureCollection.Add(feature);
              break;
            }
            Feature feature1 = new Feature((Geometry) new LineString((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) tags));
            featureCollection.Add(feature1);
            break;
          case CompleteOsmType.Relation:
            CompleteRelation relation = osmObject as CompleteRelation;
            string str;
            if (relation.Tags.TryGetValue("type", out str))
            {
              if (str == "multipolygon")
              {
                Feature feature2 = this.InterpretMultipolygonRelation(relation);
                if (feature2 != null)
                {
                  featureCollection.Add(feature2);
                  break;
                }
                break;
              }
              int num = str == "boundary" ? 1 : 0;
              break;
            }
            break;
        }
      }
      return featureCollection;
    }

    public override bool IsPotentiallyArea(TagsCollectionBase tags)
    {
      if (tags == null || tags.Count == 0)
        return false;
      bool flag = false;
      if (tags.ContainsKey("building") && !tags.IsFalse("building") || tags.ContainsKey("landuse") && !tags.IsFalse("landuse") || (tags.ContainsKey("amenity") && !tags.IsFalse("amenity") || tags.ContainsKey("harbour") && !tags.IsFalse("harbour")) || (tags.ContainsKey("historic") && !tags.IsFalse("historic") || tags.ContainsKey("leisure") && !tags.IsFalse("leisure") || (tags.ContainsKey("man_made") && !tags.IsFalse("man_made") || tags.ContainsKey("military") && !tags.IsFalse("military"))) || (tags.ContainsKey("natural") && !tags.IsFalse("natural") || tags.ContainsKey("office") && !tags.IsFalse("office") || (tags.ContainsKey("place") && !tags.IsFalse("place") || tags.ContainsKey("power") && !tags.IsFalse("power")) || (tags.ContainsKey("public_transport") && !tags.IsFalse("public_transport") || tags.ContainsKey("shop") && !tags.IsFalse("shop") || (tags.ContainsKey("sport") && !tags.IsFalse("sport") || tags.ContainsKey("tourism") && !tags.IsFalse("tourism")))) || (tags.ContainsKey("waterway") && !tags.IsFalse("waterway") || tags.ContainsKey("wetland") && !tags.IsFalse("wetland") || (tags.ContainsKey("water") && !tags.IsFalse("water") || tags.ContainsKey("aeroway") && !tags.IsFalse("aeroway"))))
        flag = true;
      string str;
      if (tags.TryGetValue("type", out str))
      {
        if (str == "multipolygon")
          flag = true;
        else if (str == "boundary")
          flag = true;
      }
      if (tags.IsTrue("area"))
        flag = true;
      else if (tags.IsFalse("area"))
        flag = false;
      return flag;
    }

    private Feature InterpretMultipolygonRelation(CompleteRelation relation)
    {
      Feature feature = (Feature) null;
      if (relation.Members == null)
        return feature;
      List<KeyValuePair<bool, CompleteWay>> ways = new List<KeyValuePair<bool, CompleteWay>>();
      foreach (CompleteRelationMember member in (IEnumerable<CompleteRelationMember>) relation.Members)
      {
        if (member.Role == "inner" && member.Member is CompleteWay)
          ways.Add(new KeyValuePair<bool, CompleteWay>(false, member.Member as CompleteWay));
        else if (member.Role == "outer" && member.Member is CompleteWay)
          ways.Add(new KeyValuePair<bool, CompleteWay>(true, member.Member as CompleteWay));
      }
      List<KeyValuePair<bool, LineairRing>> rings;
      if (!this.AssignRings(ways, out rings))
        Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", TraceEventType.Error, string.Format("Ring assignment failed: invalid multipolygon relation [{0}] detected!", (object) relation.Id));
      Geometry geometry = this.GroupRings(rings);
      if (geometry != null)
        feature = new Feature(geometry, (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) relation.Tags));
      return feature;
    }

    private Geometry GroupRings(List<KeyValuePair<bool, LineairRing>> rings)
    {
      Geometry geometry1 = (Geometry) null;
      bool[][] containsFlags = new bool[rings.Count][];
      KeyValuePair<bool, LineairRing> ring;
      for (int index1 = 0; index1 < rings.Count; ++index1)
      {
        containsFlags[index1] = new bool[rings.Count];
        for (int index2 = 0; index2 < index1; ++index2)
        {
          bool[] flagArray = containsFlags[index1];
          int index3 = index2;
          ring = rings[index1];
          LineairRing lineairRing1 = ring.Value;
          ring = rings[index2];
          LineairRing lineairRing2 = ring.Value;
          int num = lineairRing1.Contains(lineairRing2) ? 1 : 0;
          flagArray[index3] = num != 0;
        }
      }
      bool[] used = new bool[rings.Count];
      MultiPolygon multiPolygon = (MultiPolygon) null;
      while (((IEnumerable<bool>) used).Contains<bool>(false))
      {
        LineairRing outline = (LineairRing) null;
        int index = -1;
        for (int ringIdx = 0; ringIdx < rings.Count; ++ringIdx)
        {
          if (!used[ringIdx] && this.CheckUncontained(rings, containsFlags, used, ringIdx))
          {
            ring = rings[ringIdx];
            if (!ring.Key)
              Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", TraceEventType.Error, "Invalid multipolygon relation: an 'inner' ring was detected without an 'outer'.");
            index = ringIdx;
            ring = rings[ringIdx];
            outline = ring.Value;
            used[ringIdx] = true;
            break;
          }
        }
        if (outline != null)
        {
          List<LineairRing> lineairRingList1 = new List<LineairRing>();
          for (int ringIdx = 0; ringIdx < rings.Count; ++ringIdx)
          {
            if (!used[ringIdx] && containsFlags[index][ringIdx] && this.CheckUncontained(rings, containsFlags, used, ringIdx))
            {
              List<LineairRing> lineairRingList2 = lineairRingList1;
              ring = rings[ringIdx];
              LineairRing lineairRing = ring.Value;
              lineairRingList2.Add(lineairRing);
              used[ringIdx] = true;
            }
          }
          bool flag = !((IEnumerable<bool>) used).Contains<bool>(false);
          if (((multiPolygon != null ? 0 : (lineairRingList1.Count == 0 ? 1 : 0)) & (flag ? 1 : 0)) != 0)
          {
            geometry1 = (Geometry) outline;
            break;
          }
          if (multiPolygon == null & flag)
          {
            geometry1 = (Geometry) new Polygon(outline, (IEnumerable<LineairRing>) lineairRingList1);
            break;
          }
          multiPolygon = new MultiPolygon();
          geometry1 = (Geometry) multiPolygon;
          Polygon geometry2 = new Polygon(outline, (IEnumerable<LineairRing>) lineairRingList1);
          multiPolygon.Add(geometry2);
        }
        else
        {
          Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", TraceEventType.Error, "Invalid multipolygon relation: Unassigned rings left.");
          break;
        }
      }
      return geometry1;
    }

    private bool CheckUncontained(List<KeyValuePair<bool, LineairRing>> rings, bool[][] containsFlags, bool[] used, int ringIdx)
    {
      for (int index = 0; index < rings.Count; ++index)
      {
        if (index != ringIdx && !used[index] && containsFlags[index][ringIdx])
          return false;
      }
      return true;
    }

    private bool AssignRings(List<KeyValuePair<bool, CompleteWay>> ways, out List<KeyValuePair<bool, LineairRing>> rings)
    {
      return this.AssignRings(ways, new bool[ways.Count], out rings);
    }

    private bool AssignRings(List<KeyValuePair<bool, CompleteWay>> ways, bool[] assignedFlags, out List<KeyValuePair<bool, LineairRing>> rings)
    {
      bool flag = false;
      for (int way = 0; way < ways.Count; ++way)
      {
        if (!assignedFlags[way])
        {
          flag = true;
          LineairRing ring;
          List<KeyValuePair<bool, LineairRing>> rings1;
          if (this.AssignRing(ways, way, assignedFlags, out ring) && this.AssignRings(ways, assignedFlags, out rings1))
          {
            rings = rings1;
            rings.Add(new KeyValuePair<bool, LineairRing>(ways[way].Key, ring));
            return true;
          }
        }
      }
      rings = new List<KeyValuePair<bool, LineairRing>>();
      return !flag;
    }

    private bool AssignRing(List<KeyValuePair<bool, CompleteWay>> ways, int way, bool[] assignedFlags, out LineairRing ring)
    {
      assignedFlags[way] = true;
      List<GeoCoordinate> geoCoordinateList;
      if (ways[way].Value.IsClosed())
      {
        geoCoordinateList = ways[way].Value.GetCoordinates();
      }
      else
      {
        bool key = ways[way].Key;
        List<Node> nodes = new List<Node>((IEnumerable<Node>) ways[way].Value.Nodes);
        if (this.CompleteRing(ways, assignedFlags, nodes, new bool?(key)))
        {
          geoCoordinateList = new List<GeoCoordinate>(nodes.Count);
          foreach (Node node in nodes)
            geoCoordinateList.Add(node.Coordinate);
        }
        else
        {
          assignedFlags[way] = false;
          ring = (LineairRing) null;
          return false;
        }
      }
      ring = new LineairRing((IEnumerable<GeoCoordinate>) geoCoordinateList);
      return true;
    }

    private bool CompleteRing(List<KeyValuePair<bool, CompleteWay>> ways, bool[] assignedFlags, List<Node> nodes, bool? role)
    {
      for (int index = 0; index < ways.Count; ++index)
      {
        if (!assignedFlags[index])
        {
          KeyValuePair<bool, CompleteWay> way = ways[index];
          CompleteWay completeWay = way.Value;
          if (!role.HasValue || way.Key == role.Value)
          {
            List<Node> nodeList = (List<Node>) null;
            long? id1 = nodes[nodes.Count - 1].Id;
            long? id2 = completeWay.Nodes[0].Id;
            if ((id1.GetValueOrDefault() == id2.GetValueOrDefault() ? (id1.HasValue == id2.HasValue ? 1 : 0) : 0) != 0)
            {
              nodeList = completeWay.Nodes.GetRange(1, completeWay.Nodes.Count - 1);
              assignedFlags[index] = true;
            }
            else
            {
              id2 = nodes[nodes.Count - 1].Id;
              id1 = completeWay.Nodes[completeWay.Nodes.Count - 1].Id;
              if ((id2.GetValueOrDefault() == id1.GetValueOrDefault() ? (id2.HasValue == id1.HasValue ? 1 : 0) : 0) != 0)
              {
                nodeList = completeWay.Nodes.GetRange(0, completeWay.Nodes.Count - 1);
                nodeList.Reverse();
                assignedFlags[index] = true;
              }
            }
            if (assignedFlags[index])
            {
              nodes.AddRange((IEnumerable<Node>) nodeList);
              id1 = nodes[nodes.Count - 1].Id;
              id2 = nodes[0].Id;
              if ((id1.GetValueOrDefault() == id2.GetValueOrDefault() ? (id1.HasValue == id2.HasValue ? 1 : 0) : 0) != 0 || this.CompleteRing(ways, assignedFlags, nodes, role))
                return true;
              assignedFlags[index] = false;
              nodes.RemoveRange(nodes.Count - nodeList.Count, nodeList.Count);
            }
          }
        }
      }
      return false;
    }
  }
}
