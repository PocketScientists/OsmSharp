using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Xml.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OsmSharp.Osm.Data.Memory
{
  public class MemoryDataSource : DataSourceReadOnlyBase
  {
    private Dictionary<long, Node> _nodes;
    private Dictionary<long, Way> _ways;
    private Dictionary<long, Relation> _relations;
    private Dictionary<long, HashSet<long>> _waysPerNode;
    private Dictionary<long, HashSet<long>> _relationsPerNode;
    private Dictionary<long, HashSet<long>> _relationsPerWay;
    private Dictionary<long, HashSet<long>> _relationsPerRelation;
    private GeoCoordinateBox _box;
    private Guid _id;

    public override GeoCoordinateBox BoundingBox
    {
      get
      {
        return this._box;
      }
    }

    public string Name { get; set; }

    public override Guid Id
    {
      get
      {
        return this._id;
      }
    }

    public override bool HasBoundinBox
    {
      get
      {
        return true;
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public MemoryDataSource()
    {
      this.InitializeDataStructures();
    }

    public MemoryDataSource(params OsmGeo[] initial)
    {
      this.InitializeDataStructures();
      foreach (OsmGeo osmGeo in initial)
        this.Add(osmGeo);
    }

    private void InitializeDataStructures()
    {
      this._id = Guid.NewGuid();
      this._nodes = new Dictionary<long, Node>();
      this._ways = new Dictionary<long, Way>();
      this._relations = new Dictionary<long, Relation>();
      this._waysPerNode = new Dictionary<long, HashSet<long>>();
      this._relationsPerNode = new Dictionary<long, HashSet<long>>();
      this._relationsPerWay = new Dictionary<long, HashSet<long>>();
      this._relationsPerRelation = new Dictionary<long, HashSet<long>>();
    }

    public void Add(OsmGeo osmGeo)
    {
      if (osmGeo is Node)
        this.AddNode(osmGeo as Node);
      else if (osmGeo is Way)
      {
        this.AddWay(osmGeo as Way);
      }
      else
      {
        if (!(osmGeo is Relation))
          return;
        this.AddRelation(osmGeo as Relation);
      }
    }

    public override Node GetNode(long id)
    {
      Node node = (Node) null;
      this._nodes.TryGetValue(id, out node);
      return node;
    }

    public override IList<Node> GetNodes(IList<long> ids)
    {
      List<Node> nodeList = new List<Node>();
      if (ids != null)
      {
        for (int index = 0; index < ids.Count; ++index)
          nodeList.Add(this.GetNode(ids[index]));
      }
      return (IList<Node>) nodeList;
    }

    public IEnumerable<Node> GetNodes()
    {
      return (IEnumerable<Node>) this._nodes.Values;
    }

    public void AddNode(Node node)
    {
      if (node == null)
        throw new ArgumentNullException();
      long? id = node.Id;
      if (!id.HasValue)
        throw new ArgumentException("Nodes without a valid id cannot be saved!");
      double? nullable = node.Latitude;
      if (nullable.HasValue)
      {
        nullable = node.Longitude;
        if (nullable.HasValue)
        {
          Dictionary<long, Node> nodes = this._nodes;
          id = node.Id;
          long index = id.Value;
          Node node1 = node;
          nodes[index] = node1;
          if (this._box == null)
          {
            nullable = node.Latitude;
            double latitude1 = nullable.Value;
            nullable = node.Longitude;
            double longitude1 = nullable.Value;
            GeoCoordinate a = new GeoCoordinate(latitude1, longitude1);
            nullable = node.Latitude;
            double latitude2 = nullable.Value;
            nullable = node.Longitude;
            double longitude2 = nullable.Value;
            GeoCoordinate b = new GeoCoordinate(latitude2, longitude2);
            this._box = new GeoCoordinateBox(a, b);
            return;
          }
          GeoCoordinateBox box = this._box;
          nullable = node.Latitude;
          double latitude = nullable.Value;
          nullable = node.Longitude;
          double longitude = nullable.Value;
          GeoCoordinate geoCoordinate = new GeoCoordinate(latitude, longitude);
          this._box = box + geoCoordinate;
          return;
        }
      }
      throw new ArgumentException("Nodes without a valid longitude/latitude pair cannot be saved!");
    }

    public void RemoveNode(long id)
    {
      this._nodes.Remove(id);
    }

    public override Relation GetRelation(long id)
    {
      Relation relation = (Relation) null;
      this._relations.TryGetValue(id, out relation);
      return relation;
    }

    public override IList<Relation> GetRelations(IList<long> ids)
    {
      List<Relation> relationList = new List<Relation>();
      if (ids != null)
      {
        for (int index = 0; index < ids.Count; ++index)
          relationList.Add(this.GetRelation(ids[index]));
      }
      return (IList<Relation>) relationList;
    }

    public IEnumerable<Relation> GetRelations()
    {
      return (IEnumerable<Relation>) this._relations.Values;
    }

    public void AddRelation(Relation relation)
    {
      if (relation == null)
        throw new ArgumentNullException();
      long? nullable = relation.Id;
      if (!nullable.HasValue)
        throw new ArgumentException("Relations without a valid id cannot be saved!");
      Dictionary<long, Relation> relations = this._relations;
      nullable = relation.Id;
      long index = nullable.Value;
      Relation relation1 = relation;
      relations[index] = relation1;
      if (relation.Members == null)
        return;
      foreach (RelationMember member in relation.Members)
      {
        HashSet<long> longSet1 = (HashSet<long>) null;
        switch (member.MemberType.Value)
        {
          case OsmGeoType.Node:
            Dictionary<long, HashSet<long>> relationsPerNode1 = this._relationsPerNode;
            nullable = member.MemberId;
            long key1 = nullable.Value;
            if (!relationsPerNode1.TryGetValue(key1, out longSet1))
            {
              longSet1 = new HashSet<long>();
              Dictionary<long, HashSet<long>> relationsPerNode2 = this._relationsPerNode;
              nullable = member.MemberId;
              long key2 = nullable.Value;
              HashSet<long> longSet2 = longSet1;
              relationsPerNode2.Add(key2, longSet2);
              break;
            }
            break;
          case OsmGeoType.Way:
            Dictionary<long, HashSet<long>> relationsPerWay1 = this._relationsPerWay;
            nullable = member.MemberId;
            long key3 = nullable.Value;
            if (!relationsPerWay1.TryGetValue(key3, out longSet1))
            {
              longSet1 = new HashSet<long>();
              Dictionary<long, HashSet<long>> relationsPerWay2 = this._relationsPerWay;
              nullable = member.MemberId;
              long key2 = nullable.Value;
              HashSet<long> longSet2 = longSet1;
              relationsPerWay2.Add(key2, longSet2);
              break;
            }
            break;
          case OsmGeoType.Relation:
            Dictionary<long, HashSet<long>> relationsPerRelation1 = this._relationsPerRelation;
            nullable = member.MemberId;
            long key4 = nullable.Value;
            if (!relationsPerRelation1.TryGetValue(key4, out longSet1))
            {
              longSet1 = new HashSet<long>();
              Dictionary<long, HashSet<long>> relationsPerRelation2 = this._relationsPerRelation;
              nullable = member.MemberId;
              long key2 = nullable.Value;
              HashSet<long> longSet2 = longSet1;
              relationsPerRelation2.Add(key2, longSet2);
              break;
            }
            break;
        }
        HashSet<long> longSet3 = longSet1;
        nullable = relation.Id;
        long num = nullable.Value;
        longSet3.Add(num);
      }
    }

    public void RemoveRelation(long id)
    {
      this._relations.Remove(id);
    }

    public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
    {
      List<Relation> relationList = new List<Relation>();
      HashSet<long> longSet = (HashSet<long>) null;
      switch (type)
      {
        case OsmGeoType.Node:
          if (!this._relationsPerNode.TryGetValue(id, out longSet))
            return (IList<Relation>) relationList;
          break;
        case OsmGeoType.Way:
          if (!this._relationsPerWay.TryGetValue(id, out longSet))
            return (IList<Relation>) relationList;
          break;
        case OsmGeoType.Relation:
          if (!this._relationsPerRelation.TryGetValue(id, out longSet))
            return (IList<Relation>) relationList;
          break;
      }
      foreach (long id1 in longSet)
        relationList.Add(this.GetRelation(id1));
      return (IList<Relation>) relationList;
    }

    public override Way GetWay(long id)
    {
      Way way = (Way) null;
      this._ways.TryGetValue(id, out way);
      return way;
    }

    public override IList<Way> GetWays(IList<long> ids)
    {
      List<Way> wayList = new List<Way>();
      if (ids != null)
      {
        for (int index = 0; index < ids.Count; ++index)
          wayList.Add(this.GetWay(ids[index]));
      }
      return (IList<Way>) wayList;
    }

    public IEnumerable<Way> GetWays()
    {
      return (IEnumerable<Way>) this._ways.Values;
    }

    public override IList<Way> GetWaysFor(long id)
    {
      HashSet<long> longSet = (HashSet<long>) null;
      List<Way> wayList = new List<Way>();
      if (this._waysPerNode.TryGetValue(id, out longSet))
      {
        foreach (long id1 in longSet)
          wayList.Add(this.GetWay(id1));
      }
      return (IList<Way>) wayList;
    }

    public IList<Way> GetWaysFor(IEnumerable<long> ids)
    {
      HashSet<long> longSet1 = new HashSet<long>();
      foreach (long id in ids)
      {
        HashSet<long> longSet2;
        if (this._waysPerNode.TryGetValue(id, out longSet2))
        {
          foreach (long num in longSet2)
            longSet1.Add(num);
        }
      }
      List<Way> wayList = new List<Way>();
      foreach (long id in longSet1)
        wayList.Add(this.GetWay(id));
      return (IList<Way>) wayList;
    }

    public void AddWay(Way way)
    {
      if (way == null)
        throw new ArgumentNullException();
      long? id = way.Id;
      if (!id.HasValue)
        throw new ArgumentException("Ways without a valid id cannot be saved!");
      Dictionary<long, Way> ways = this._ways;
      id = way.Id;
      long index = id.Value;
      Way way1 = way;
      ways[index] = way1;
      if (way.Nodes == null)
        return;
      foreach (long node in way.Nodes)
      {
        HashSet<long> longSet1;
        if (!this._waysPerNode.TryGetValue(node, out longSet1))
        {
          longSet1 = new HashSet<long>();
          this._waysPerNode.Add(node, longSet1);
        }
        HashSet<long> longSet2 = longSet1;
        id = way.Id;
        long num = id.Value;
        longSet2.Add(num);
      }
    }

    public void RemoveWay(long id)
    {
      this._ways.Remove(id);
    }

    public override IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
    {
      List<OsmGeo> osmGeoList1 = new List<OsmGeo>();
      HashSet<long> longSet1 = new HashSet<long>();
      foreach (Node node in this._nodes.Values)
      {
        if (filter == null || filter.Evaluate((OsmGeo) node))
        {
          GeoCoordinateBox geoCoordinateBox = box;
          double? nullable = node.Latitude;
          double latitude = nullable.Value;
          nullable = node.Longitude;
          double longitude = nullable.Value;
          GeoCoordinate geoCoordinate = new GeoCoordinate(latitude, longitude);
          if (geoCoordinateBox.Contains((PointF2D) geoCoordinate))
          {
            osmGeoList1.Add((OsmGeo) node);
            longSet1.Add(node.Id.Value);
          }
        }
      }
      osmGeoList1.AddRange(this.GetWaysFor((IEnumerable<long>) longSet1).Cast<OsmGeo>());
      List<Relation> source = new List<Relation>();
      HashSet<long> longSet2 = new HashSet<long>();
      foreach (OsmGeo osmGeo in osmGeoList1)
      {
        foreach (Relation relation in (IEnumerable<Relation>) this.GetRelationsFor(osmGeo))
        {
          if (!longSet2.Contains(relation.Id.Value))
          {
            source.Add(relation);
            longSet2.Add(relation.Id.Value);
          }
        }
      }
      do
      {
        osmGeoList1.AddRange(source.Cast<OsmGeo>());
        List<Relation> relationList = new List<Relation>();
        foreach (OsmGeo osmGeo in source)
        {
          foreach (Relation relation in (IEnumerable<Relation>) this.GetRelationsFor(osmGeo))
          {
            if (!longSet2.Contains(relation.Id.Value))
            {
              relationList.Add(relation);
              longSet2.Add(relation.Id.Value);
            }
          }
        }
        source = relationList;
      }
      while (source.Count > 0);
      if (filter != null)
      {
        List<OsmGeo> osmGeoList2 = new List<OsmGeo>();
        foreach (OsmGeo osmGeo in osmGeoList1)
        {
          if (filter.Evaluate(osmGeo))
            osmGeoList2.Add(osmGeo);
        }
      }
      return (IList<OsmGeo>) osmGeoList1;
    }

    public static MemoryDataSource CreateFrom(OsmStreamSource sourceStream)
    {
      if (sourceStream.CanReset)
        sourceStream.Reset();
      MemoryDataSource memoryDataSource = new MemoryDataSource();
      foreach (OsmGeo osmGeo in sourceStream)
      {
        if (osmGeo != null)
        {
          switch (osmGeo.Type)
          {
            case OsmGeoType.Node:
              memoryDataSource.AddNode(osmGeo as Node);
              continue;
            case OsmGeoType.Way:
              memoryDataSource.AddWay(osmGeo as Way);
              continue;
            case OsmGeoType.Relation:
              memoryDataSource.AddRelation(osmGeo as Relation);
              continue;
            default:
              continue;
          }
        }
      }
      return memoryDataSource;
    }

    public static MemoryDataSource CreateFromXmlStream(Stream stream)
    {
      return MemoryDataSource.CreateFrom((OsmStreamSource) new XmlOsmStreamSource(stream));
    }

    public static MemoryDataSource CreateFromPBFStream(Stream stream)
    {
      return MemoryDataSource.CreateFrom((OsmStreamSource) new PBFOsmStreamSource(stream));
    }
  }
}
