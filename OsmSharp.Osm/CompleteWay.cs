using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class CompleteWay : CompleteOsmGeo
  {
    private readonly List<Node> _nodes;

    public override CompleteOsmType Type
    {
      get
      {
        return CompleteOsmType.Way;
      }
    }

    public List<Node> Nodes
    {
      get
      {
        return this._nodes;
      }
    }

    protected internal CompleteWay(long id)
      : base(id)
    {
      this._nodes = new List<Node>();
    }

    protected internal CompleteWay(ObjectTable<string> stringTable, long id)
      : base(stringTable, id)
    {
      this._nodes = new List<Node>();
    }

    public List<GeoCoordinate> GetCoordinates()
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      for (int index = 0; index < this.Nodes.Count; ++index)
        geoCoordinateList.Add(this.Nodes[index].Coordinate);
      return geoCoordinateList;
    }

    public void CopyTo(CompleteWay w)
    {
      foreach (Tag tag in this.Tags)
        w.Tags.Add(tag.Key, tag.Value);
      w.Nodes.AddRange((IEnumerable<Node>) this.Nodes);
      w.TimeStamp = this.TimeStamp;
      w.User = this.User;
      w.UserId = this.UserId;
      w.Version = this.Version;
      w.Visible = this.Visible;
    }

    public CompleteWay Copy()
    {
      CompleteWay w = new CompleteWay(this.Id);
      this.CopyTo(w);
      return w;
    }

    public bool HasNode(Node node)
    {
      return this.Nodes.Contains(node);
    }

    public bool IsClosed()
    {
      if (this.Nodes == null || this.Nodes.Count <= 1)
        return false;
      long? id1 = this.Nodes[0].Id;
      long? id2 = this.Nodes[this.Nodes.Count - 1].Id;
      if (id1.GetValueOrDefault() != id2.GetValueOrDefault())
        return false;
      return id1.HasValue == id2.HasValue;
    }

    public override OsmGeo ToSimple()
    {
      Way way1 = new Way();
      way1.Id = new long?(this.Id);
      way1.ChangeSetId = this.ChangeSetId;
      way1.Tags = this.Tags;
      way1.TimeStamp = this.TimeStamp;
      way1.UserId = this.UserId;
      way1.UserName = this.User;
      Way way2 = way1;
      long? version = this.Version;
      ulong? nullable = version.HasValue ? new ulong?((ulong) version.GetValueOrDefault()) : new ulong?();
      way2.Version = nullable;
      way1.Visible = new bool?(this.Visible);
      way1.Nodes = new List<long>();
      foreach (Node node in this.Nodes)
        way1.Nodes.Add(node.Id.Value);
      return (OsmGeo) way1;
    }

    public override string ToString()
    {
      return string.Format("http://www.openstreetmap.org/?way={0}", (object) this.Id);
    }

    public static CompleteWay Create(long id)
    {
      return new CompleteWay(id);
    }

    public static CompleteWay CreateFrom(Way simpleWay, IDictionary<long, Node> nodes)
    {
      if (simpleWay == null)
        throw new ArgumentNullException("simpleWay");
      if (nodes == null)
        throw new ArgumentNullException("nodes");
      if (!simpleWay.Id.HasValue)
        throw new Exception("simpleWay.id is null");
      CompleteWay completeWay = CompleteWay.Create(simpleWay.Id.Value);
      completeWay.ChangeSetId = simpleWay.ChangeSetId;
      foreach (Tag tag in simpleWay.Tags)
        completeWay.Tags.Add(tag);
      for (int index = 0; index < simpleWay.Nodes.Count; ++index)
      {
        long node1 = simpleWay.Nodes[index];
        Node node2 = (Node) null;
        if (!nodes.TryGetValue(node1, out node2))
          return (CompleteWay) null;
        completeWay.Nodes.Add(node2);
      }
      completeWay.TimeStamp = simpleWay.TimeStamp;
      completeWay.User = simpleWay.UserName;
      completeWay.UserId = simpleWay.UserId;
      completeWay.Version = simpleWay.Version.HasValue ? new long?((long) simpleWay.Version.Value) : new long?();
      completeWay.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;
      return completeWay;
    }

    public static CompleteWay CreateFrom(Way simpleWay, INodeSource nodeSource)
    {
      if (simpleWay == null)
        throw new ArgumentNullException("simpleWay");
      if (nodeSource == null)
        throw new ArgumentNullException("nodeSource");
      if (!simpleWay.Id.HasValue)
        throw new Exception("simpleWay.id is null");
      CompleteWay completeWay = CompleteWay.Create(simpleWay.Id.Value);
      completeWay.ChangeSetId = simpleWay.ChangeSetId;
      if (simpleWay.Tags != null)
      {
        foreach (Tag tag in simpleWay.Tags)
          completeWay.Tags.Add(tag);
      }
      if (simpleWay.Nodes != null)
      {
        for (int index = 0; index < simpleWay.Nodes.Count; ++index)
        {
          long node1 = simpleWay.Nodes[index];
          Node node2 = nodeSource.GetNode(node1);
          if (node2 == null)
            return (CompleteWay) null;
          Node node3 = node2;
          if (node3 == null)
            return (CompleteWay) null;
          completeWay.Nodes.Add(node3);
        }
      }
      completeWay.TimeStamp = simpleWay.TimeStamp;
      completeWay.User = simpleWay.UserName;
      completeWay.UserId = simpleWay.UserId;
      completeWay.Version = simpleWay.Version.HasValue ? new long?((long) simpleWay.Version.Value) : new long?();
      completeWay.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;
      return completeWay;
    }

    public static CompleteWay Create(ObjectTable<string> table, long id)
    {
      return new CompleteWay(table, id);
    }

    public static CompleteWay CreateFrom(ObjectTable<string> table, Way simpleWay, IDictionary<long, Node> nodes)
    {
      if (table == null)
        throw new ArgumentNullException("table");
      if (simpleWay == null)
        throw new ArgumentNullException("simpleWay");
      if (nodes == null)
        throw new ArgumentNullException("nodes");
      if (!simpleWay.Id.HasValue)
        throw new Exception("simpleWay.id is null");
      CompleteWay completeWay = CompleteWay.Create(table, simpleWay.Id.Value);
      completeWay.ChangeSetId = simpleWay.ChangeSetId;
      foreach (Tag tag in simpleWay.Tags)
        completeWay.Tags.Add(tag);
      for (int index = 0; index < simpleWay.Nodes.Count; ++index)
      {
        long node1 = simpleWay.Nodes[index];
        Node node2 = (Node) null;
        if (!nodes.TryGetValue(node1, out node2))
          return (CompleteWay) null;
        completeWay.Nodes.Add(node2);
      }
      completeWay.TimeStamp = simpleWay.TimeStamp;
      completeWay.User = simpleWay.UserName;
      completeWay.UserId = simpleWay.UserId;
      completeWay.Version = simpleWay.Version.HasValue ? new long?((long) simpleWay.Version.Value) : new long?();
      completeWay.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;
      return completeWay;
    }
  }
}
