using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Xml.v0_6;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.Streams
{
  public class XmlOsmStreamTarget : OsmStreamTarget, IDisposable
  {
    private Stream _stream;
    private StreamWriter _streamWriter;
    private XmlWriterSettings _settings;
    private readonly bool _disposeStream;
    private bool _initialized;
    private bool _closed;

    public XmlOsmStreamTarget(Stream stream)
    {
      this._stream = stream;
      this._streamWriter = new StreamWriter(stream, Encoding.UTF8);
      this._settings = new XmlWriterSettings();
      this._settings.OmitXmlDeclaration = true;
      this._settings.Indent = true;
    }

    public override void Initialize()
    {
      if (this._initialized)
        return;
      this._streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
      this._streamWriter.WriteLine("<osm version=\"0.6\" generator=\"OsmSharp\">");
      this._streamWriter.Flush();
      this._initialized = true;
    }

    public override void AddNode(Node simpleNode)
    {
      node node1 = new node();
      node1.idSpecified = false;
      long? nullable1;
      if (simpleNode.Id.HasValue)
      {
        node node2 = node1;
        nullable1 = simpleNode.Id;
        long num = nullable1.Value;
        node2.id = num;
        node1.idSpecified = true;
      }
      node1.changesetSpecified = false;
      nullable1 = simpleNode.ChangeSetId;
      if (nullable1.HasValue)
      {
        node node2 = node1;
        nullable1 = simpleNode.ChangeSetId;
        long num = nullable1.Value;
        node2.changeset = num;
        node1.changesetSpecified = true;
      }
      node1.visibleSpecified = false;
      if (simpleNode.Visible.HasValue)
      {
        node1.visible = simpleNode.Visible.Value;
        node1.visibleSpecified = true;
      }
      node1.timestampSpecified = false;
      if (simpleNode.TimeStamp.HasValue)
      {
        node1.timestamp = simpleNode.TimeStamp.Value;
        node1.timestampSpecified = true;
      }
      node1.latSpecified = false;
      double? nullable2;
      if (simpleNode.Latitude.HasValue)
      {
        node node2 = node1;
        nullable2 = simpleNode.Latitude;
        double num = nullable2.Value;
        node2.lat = num;
        node1.latSpecified = true;
      }
      node1.lonSpecified = false;
      nullable2 = simpleNode.Longitude;
      if (nullable2.HasValue)
      {
        node node2 = node1;
        nullable2 = simpleNode.Longitude;
        double num = nullable2.Value;
        node2.lon = num;
        node1.lonSpecified = true;
      }
      node1.uidSpecified = false;
      nullable1 = simpleNode.UserId;
      if (nullable1.HasValue)
      {
        node node2 = node1;
        nullable1 = simpleNode.UserId;
        long num = nullable1.Value;
        node2.uid = num;
        node1.uidSpecified = true;
      }
      node1.user = simpleNode.UserName;
      node1.tag = this.ConvertToXmlTags(simpleNode.Tags);
      if (simpleNode.Version.HasValue)
      {
        node1.version = simpleNode.Version.Value;
        node1.versionSpecified = true;
      }
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add(string.Empty, string.Empty);
      MemoryStream memoryStream = new MemoryStream();
      XmlWriterSettings settings = this._settings;
      XmlWriter xmlWriter = XmlWriter.Create((Stream) memoryStream, settings);
      new XmlSerializer(typeof (node), string.Empty).Serialize(xmlWriter, (object) node1, namespaces);
      Stream stream = this._stream;
      memoryStream.WriteTo(stream);
      this._stream.Flush();
      this._streamWriter.WriteLine();
      this._streamWriter.Flush();
    }

    public override void AddWay(Way simpleWay)
    {
      way way1 = new way();
      way1.idSpecified = false;
      long? nullable;
      if (simpleWay.Id.HasValue)
      {
        way1.idSpecified = true;
        way way2 = way1;
        nullable = simpleWay.Id;
        long num = nullable.Value;
        way2.id = num;
      }
      way1.changesetSpecified = false;
      nullable = simpleWay.ChangeSetId;
      if (nullable.HasValue)
      {
        way way2 = way1;
        nullable = simpleWay.ChangeSetId;
        long num = nullable.Value;
        way2.changeset = num;
        way1.changesetSpecified = true;
      }
      way1.visibleSpecified = false;
      if (simpleWay.Visible.HasValue)
      {
        way1.visible = simpleWay.Visible.Value;
        way1.visibleSpecified = true;
      }
      way1.timestampSpecified = false;
      if (simpleWay.TimeStamp.HasValue)
      {
        way1.timestamp = simpleWay.TimeStamp.Value;
        way1.timestampSpecified = true;
      }
      way1.uidSpecified = false;
      nullable = simpleWay.UserId;
      if (nullable.HasValue)
      {
        way way2 = way1;
        nullable = simpleWay.UserId;
        long num = nullable.Value;
        way2.uid = num;
        way1.uidSpecified = true;
      }
      way1.user = simpleWay.UserName;
      way1.tag = this.ConvertToXmlTags(simpleWay.Tags);
      if (simpleWay.Nodes != null)
      {
        way1.nd = new nd[simpleWay.Nodes.Count];
        for (int index = 0; index < simpleWay.Nodes.Count; ++index)
          way1.nd[index] = new nd()
          {
            refSpecified = true,
            @ref = simpleWay.Nodes[index]
          };
      }
      if (simpleWay.Version.HasValue)
      {
        way1.version = simpleWay.Version.Value;
        way1.versionSpecified = true;
      }
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add(string.Empty, string.Empty);
      MemoryStream memoryStream = new MemoryStream();
      XmlWriterSettings settings = this._settings;
      XmlWriter xmlWriter = XmlWriter.Create((Stream) memoryStream, settings);
      new XmlSerializer(typeof (way), string.Empty).Serialize(xmlWriter, (object) way1, namespaces);
      Stream stream = this._stream;
      memoryStream.WriteTo(stream);
      this._stream.Flush();
      this._streamWriter.WriteLine();
      this._streamWriter.Flush();
    }

    public override void AddRelation(Relation simpleRelation)
    {
      relation relation1 = new relation();
      relation1.idSpecified = false;
      long? nullable;
      if (simpleRelation.Id.HasValue)
      {
        relation1.idSpecified = true;
        relation relation2 = relation1;
        nullable = simpleRelation.Id;
        long num = nullable.Value;
        relation2.id = num;
      }
      relation1.changesetSpecified = false;
      nullable = simpleRelation.ChangeSetId;
      if (nullable.HasValue)
      {
        relation relation2 = relation1;
        nullable = simpleRelation.ChangeSetId;
        long num = nullable.Value;
        relation2.changeset = num;
        relation1.changesetSpecified = true;
      }
      relation1.visibleSpecified = false;
      if (simpleRelation.Visible.HasValue)
      {
        relation1.visible = simpleRelation.Visible.Value;
        relation1.visibleSpecified = true;
      }
      relation1.timestampSpecified = false;
      if (simpleRelation.TimeStamp.HasValue)
      {
        relation1.timestamp = simpleRelation.TimeStamp.Value;
        relation1.timestampSpecified = true;
      }
      relation1.uidSpecified = false;
      nullable = simpleRelation.UserId;
      if (nullable.HasValue)
      {
        relation relation2 = relation1;
        nullable = simpleRelation.UserId;
        long num = nullable.Value;
        relation2.uid = num;
        relation1.uidSpecified = true;
      }
      relation1.user = simpleRelation.UserName;
      relation1.tag = this.ConvertToXmlTags(simpleRelation.Tags);
      if (simpleRelation.Members != null)
      {
        relation1.member = new member[simpleRelation.Members.Count];
        for (int index = 0; index < simpleRelation.Members.Count; ++index)
        {
          member member1 = new member();
          RelationMember member2 = simpleRelation.Members[index];
          member1.refSpecified = false;
          nullable = member2.MemberId;
          if (nullable.HasValue)
          {
            member member3 = member1;
            nullable = member2.MemberId;
            long num = nullable.Value;
            member3.@ref = num;
            member1.refSpecified = true;
          }
          member1.typeSpecified = false;
          if (member2.MemberType.HasValue)
          {
            switch (member2.MemberType.Value)
            {
              case OsmGeoType.Node:
                member1.type = memberType.node;
                break;
              case OsmGeoType.Way:
                member1.type = memberType.way;
                break;
              case OsmGeoType.Relation:
                member1.type = memberType.relation;
                break;
            }
            member1.typeSpecified = true;
          }
          member1.role = member2.MemberRole;
          relation1.member[index] = member1;
        }
      }
      if (simpleRelation.Version.HasValue)
      {
        relation1.version = simpleRelation.Version.Value;
        relation1.versionSpecified = true;
      }
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add(string.Empty, string.Empty);
      MemoryStream memoryStream = new MemoryStream();
      XmlWriterSettings settings = this._settings;
      XmlWriter xmlWriter = XmlWriter.Create((Stream) memoryStream, settings);
      new XmlSerializer(typeof (relation), string.Empty).Serialize(xmlWriter, (object) relation1, namespaces);
      Stream stream = this._stream;
      memoryStream.WriteTo(stream);
      this._stream.Flush();
      this._streamWriter.WriteLine();
      this._streamWriter.Flush();
    }

    private tag[] ConvertToXmlTags(TagsCollectionBase tags)
    {
      if (tags == null)
        return (tag[]) null;
      tag[] tagArray = new tag[tags.Count];
      int index = 0;
      foreach (Tag tag in tags)
      {
        tagArray[index] = new tag();
        tagArray[index].k = tag.Key;
        tagArray[index].v = tag.Value;
        ++index;
      }
      return tagArray;
    }

    public override void Close()
    {
      base.Close();
      if (this._closed)
        return;
      this._streamWriter.WriteLine("</osm>");
      this._streamWriter.Flush();
      this._closed = true;
    }

    public void Dispose()
    {
      if (!this._disposeStream || this._streamWriter == null)
        return;
      this._stream.Dispose();
      this._streamWriter.Dispose();
    }
  }
}
