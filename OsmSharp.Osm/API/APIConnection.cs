using OsmSharp.Collections.Tags;
using OsmSharp.IO.Xml;
using OsmSharp.IO.Xml.Sources;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Core.API;
using OsmSharp.Osm.Xml;
using OsmSharp.Osm.Xml.v0_6;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Osm.API
{
  public class APIConnection
  {
    private readonly string _user;
    private readonly string _pass;
    private APICapabilities _api_capabilities;
    private changeset _current_changeset;
    private osmChange _current_changes;

    public Uri Uri { get; private set; }

    public APIConnection(string url, string user, string pass)
    {
      this._user = user;
      this._pass = pass;
      this.Uri = new Uri(url);
    }

    private string DoApiCall(bool authenticate, string url, APIConnection.Method method, byte[] data)
    {
      WebRequest webRequest = WebRequest.Create(new Uri(this.Uri.AbsoluteUri + url));
      this.SetBasicAuthHeader(webRequest);
      switch (method)
      {
        case APIConnection.Method.PUT:
          webRequest.Method = "PUT";
          Stream requestStream1 = webRequest.GetRequestStream();
          byte[] buffer1 = data;
          int offset1 = 0;
          int length1 = data.Length;
          requestStream1.Write(buffer1, offset1, length1);
          requestStream1.Dispose();
          HttpWebResponse response1 = (HttpWebResponse) webRequest.GetResponse();
          Encoding encoding1 = Encoding.GetEncoding("Windows-1252");
          StreamReader streamReader1 = new StreamReader(response1.GetResponseStream(), encoding1);
          string end1 = streamReader1.ReadToEnd();
          response1.Dispose();
          streamReader1.Dispose();
          return end1;
        case APIConnection.Method.GET:
          string str = string.Empty;
          try
          {
            webRequest.Method = "GET";
            HttpWebResponse response2 = (HttpWebResponse) webRequest.GetResponse();
            Encoding encoding2 = Encoding.GetEncoding("Windows-1252");
            StreamReader streamReader2 = new StreamReader(response2.GetResponseStream(), encoding2);
            str = streamReader2.ReadToEnd();
            response2.Dispose();
            streamReader2.Dispose();
          }
          catch (WebException ex)
          {
            if (ex.Response != null)
            {
              if (ex.Response is HttpWebResponse)
              {
                switch ((ex.Response as HttpWebResponse).StatusCode)
                {
                  case HttpStatusCode.NotFound:
                  case HttpStatusCode.Gone:
                    break;
                  default:
                    throw new APIException(string.Format("Unexpected API response: {0}", (object) (ex.Response as HttpWebResponse).StatusCode.ToString()));
                }
              }
            }
          }
          return str;
        case APIConnection.Method.DELETE:
          webRequest.Method = "DELETE";
          Stream requestStream2 = webRequest.GetRequestStream();
          byte[] buffer2 = data;
          int offset2 = 0;
          int length2 = data.Length;
          requestStream2.Write(buffer2, offset2, length2);
          requestStream2.Dispose();
          HttpWebResponse response3 = (HttpWebResponse) webRequest.GetResponse();
          Encoding encoding3 = Encoding.GetEncoding("Windows-1252");
          StreamReader streamReader3 = new StreamReader(response3.GetResponseStream(), encoding3);
          string end2 = streamReader3.ReadToEnd();
          response3.Dispose();
          streamReader3.Dispose();
          return end2;
        default:
          throw new NotSupportedException(string.Format("Method {0} not supported!", (object) method.ToString()));
      }
    }

    private void SetBasicAuthHeader(WebRequest req)
    {
      string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(this._user + ":" + this._pass));
      req.Headers["Authorization"] = "Basic " + base64String;
    }

    public APICapabilities GetCapabilities()
    {
      if (this._api_capabilities != null)
        return this._api_capabilities;
      string s = this.DoApiCall(false, "/api/capabilities", APIConnection.Method.GET, (byte[]) null);
      if (s == null || s.Trim().Length <= 0)
        return (APICapabilities) null;
      api api = (new OsmDocument((IXmlSource) new XmlReaderSource(XmlReader.Create((TextReader) new StringReader(s)))).Osm as osm).api;
      this._api_capabilities = new APICapabilities();
      if (api.area.maximumSpecified)
        this._api_capabilities.AreaMaximum = api.area.maximum;
      if (api.changesets.maximum_elementsSpecified)
        this._api_capabilities.ChangeSetsMaximumElement = api.changesets.maximum_elements;
      if (api.timeout.secondsSpecified)
        this._api_capabilities.TimeoutSeconds = api.timeout.seconds;
      if (api.tracepoints.per_pageSpecified)
        this._api_capabilities.TracePointsPerPage = api.tracepoints.per_page;
      if (api.version.maximumSpecified)
        this._api_capabilities.VersionMaximum = api.version.maximum;
      if (api.version.minimumSpecified)
        this._api_capabilities.VersionMinimum = api.version.minimum;
      if (api.waynodes.maximumSpecified)
        this._api_capabilities.WayNodesMaximum = api.waynodes.maximum;
      return this._api_capabilities;
    }

    public List<OsmGeo> BoundingBoxGet(GeoCoordinateBox box)
    {
      string s = this.DoApiCall(0 != 0, string.Format("/api/0.6/map?bbox={0},{1},{2},{3}", (object) box.MinLon.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) box.MinLat.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) box.MaxLon.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) box.MaxLat.ToString((IFormatProvider) CultureInfo.InvariantCulture)), APIConnection.Method.GET, (byte[]) null);
      if (s == null || s.Trim().Length <= 0)
        return (List<OsmGeo>) null;
      OsmDocument osmDocument = new OsmDocument((IXmlSource) new XmlReaderSource(XmlReader.Create((TextReader) new StringReader(s))));
      List<OsmGeo> osmGeoList = new List<OsmGeo>();
      osm osm = osmDocument.Osm as osm;
      if (osm.node != null)
      {
        foreach (node xml_node in osm.node)
          osmGeoList.Add((OsmGeo) this.Convertv6XmlNode(xml_node));
      }
      if (osm.way != null)
      {
        foreach (way xml_way in osm.way)
          osmGeoList.Add((OsmGeo) this.Convertv6XmlWay(xml_way));
      }
      if (osm.relation != null)
      {
        foreach (relation xml_relation in osm.relation)
          osmGeoList.Add((OsmGeo) this.Convertv6XmlRelation(xml_relation));
      }
      return osmGeoList;
    }

    private osmChange GetCurrentChange()
    {
      if (this._current_changes == null)
        this._current_changes = new osmChange();
      return this._current_changes;
    }

    public ChangeSetInfo ChangeSetGet(long id)
    {
      string s = this.DoApiCall(0 != 0, string.Format("/api/0.6/changeset/{0}", (object) id), APIConnection.Method.GET, (byte[]) null);
      if (s != null && s.Trim().Length > 0)
        return this.Convertv6XmlChangeSet((new OsmDocument((IXmlSource) new XmlReaderSource(XmlReader.Create((TextReader) new StringReader(s)))).Osm as osm).changeset[0]);
      return (ChangeSetInfo) null;
    }

    public ChangeSet ChangesGet(long id)
    {
      string s = this.DoApiCall(0 != 0, string.Format("/api/0.6/changeset/{0}", (object) id), APIConnection.Method.GET, (byte[]) null);
      if (s != null && s.Trim().Length > 0)
        return this.Convertv6XmlChanges(new XmlSerializer(typeof (osmChange)).Deserialize((TextReader) new StringReader(s)) as osmChange);
      return (ChangeSet) null;
    }

    public long ChangeSetOpen(string comment)
    {
      return this.ChangeSetOpen(comment, string.Format("OsmSharp v{0}", (object) typeof (APIConnection).Assembly.GetName().Version.ToString(2)));
    }

    public long ChangeSetOpen(string comment, string created_by)
    {
      if (created_by == null || created_by.Length == 0)
        throw new ArgumentOutOfRangeException("A created by tag always has to exist and have a usefull value.");
      osm osm1 = new osm();
      osm1.changeset = new changeset[1];
      changeset changeset = new changeset();
      changeset.tag = new tag[1];
      changeset.tag[0] = new tag();
      changeset.tag[0].k = "created_by";
      changeset.tag[0].v = created_by;
      osm1.changeset[0] = changeset;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      string s = this.DoApiCall(true, "api/0.6/changeset/create", APIConnection.Method.PUT, memoryStream.ToArray());
      this._current_changeset = changeset;
      this._current_changeset.id = long.Parse(s);
      return this._current_changeset.id;
    }

    public void ChangeSetClose()
    {
      this.DoApiCall(1 != 0, string.Format("api/0.6/changeset/{0}/close", (object) this._current_changeset.id), APIConnection.Method.PUT, new byte[0]);
    }

    public Node NodeCreate(Node node)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      node node1 = node.ConvertTo();
      node1.changeset = this._current_changeset.id;
      node1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.node = new node[1];
      osm1.node[0] = node1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      string s = this.DoApiCall(true, "api/0.6/node/create", APIConnection.Method.PUT, memoryStream.ToArray());
      long result;
      if (!long.TryParse(s, out result))
        throw new APIException(string.Format("Invalid response when creating a new node: {0}", (object) s));
      node.Id = new long?(result);
      return node;
    }

    public void NodeUpdate(Node node)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      if (!node.Id.HasValue)
        throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
      node node1 = node.ConvertTo();
      node1.changeset = this._current_changeset.id;
      node1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.node = new node[1];
      osm1.node[0] = node1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      byte[] array = memoryStream.ToArray();
      this.DoApiCall(1 != 0, string.Format("api/0.6/node/{0}", (object) node.Id.Value), APIConnection.Method.PUT, array);
    }

    public void NodeDelete(Node node)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      if (!node.Id.HasValue)
        throw new ArgumentOutOfRangeException("Cannot delete an object without an id!");
      node node1 = node.ConvertTo();
      node1.changeset = this._current_changeset.id;
      node1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.node = new node[1];
      osm1.node[0] = node1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      byte[] array = memoryStream.ToArray();
      this.DoApiCall(1 != 0, string.Format("api/0.6/node/{0}", (object) node.Id.Value), APIConnection.Method.DELETE, array);
    }

    public Node NodeGet(long id)
    {
      string s = this.DoApiCall(0 != 0, string.Format("/api/0.6/node/{0}", (object) id), APIConnection.Method.GET, (byte[]) null);
      if (s != null && s.Trim().Length > 0)
        return this.Convertv6XmlNode((new OsmDocument((IXmlSource) new XmlReaderSource(XmlReader.Create((TextReader) new StringReader(s)))).Osm as osm).node[0]);
      return (Node) null;
    }

    private ChangeSetInfo Convertv6XmlChangeSet(changeset xml_changeset)
    {
      ChangeSetInfo changeSetInfo = new ChangeSetInfo();
      if (xml_changeset.idSpecified)
        changeSetInfo.Id = new long?(xml_changeset.id);
      if (xml_changeset.closed_atSpecified)
        changeSetInfo.ClosedAt = new DateTime?(xml_changeset.closed_at);
      if (xml_changeset.closed_atSpecified)
        changeSetInfo.CreatedAt = xml_changeset.created_at;
      if (xml_changeset.max_latSpecified)
        changeSetInfo.MaxLat = xml_changeset.max_lat;
      if (xml_changeset.max_lonSpecified)
        changeSetInfo.MaxLon = xml_changeset.max_lon;
      if (xml_changeset.min_latSpecified)
        changeSetInfo.MinLat = xml_changeset.min_lat;
      if (xml_changeset.min_lonSpecified)
        changeSetInfo.MinLon = xml_changeset.min_lon;
      if (xml_changeset.openSpecified)
        changeSetInfo.Open = xml_changeset.open;
      if (xml_changeset.tag != null)
      {
        changeSetInfo.Tags = new Dictionary<string, string>();
        foreach (tag tag in xml_changeset.tag)
          changeSetInfo.Tags.Add(tag.k, tag.v);
      }
      if (xml_changeset.uidSpecified)
        changeSetInfo.UserId = xml_changeset.uid;
      changeSetInfo.User = xml_changeset.user;
      return changeSetInfo;
    }

    private ChangeSet Convertv6XmlChanges(osmChange osm_change)
    {
      List<Change> changeList = new List<Change>();
      if (osm_change.create != null)
      {
        for (int index1 = 0; index1 < osm_change.create.Length; ++index1)
        {
          create create = osm_change.create[index1];
          List<OsmGeo> osmGeoList = new List<OsmGeo>();
          if (create.node != null)
          {
            for (int index2 = 0; index2 < create.node.Length; ++index2)
              osmGeoList.Add((OsmGeo) this.Convertv6XmlNode(create.node[index2]));
          }
          if (create.way != null)
          {
            for (int index2 = 0; index2 < create.way.Length; ++index2)
              osmGeoList.Add((OsmGeo) this.Convertv6XmlWay(create.way[index2]));
          }
          if (create.relation != null)
          {
            for (int index2 = 0; index2 < create.relation.Length; ++index2)
              osmGeoList.Add((OsmGeo) this.Convertv6XmlRelation(create.relation[index2]));
          }
          if (osmGeoList.Count > 0)
            changeList.Add(new Change()
            {
              OsmGeo = osmGeoList,
              Type = ChangeType.Create
            });
        }
      }
      return new ChangeSet()
      {
        Changes = changeList
      };
    }

    private Node Convertv6XmlNode(node xml_node)
    {
      Node node = new Node();
      node.Id = new long?(xml_node.id);
      node.Latitude = new double?(xml_node.lat);
      node.Longitude = new double?(xml_node.lon);
      if (xml_node.tag != null)
      {
        node.Tags = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag in xml_node.tag)
          node.Tags.Add(tag.k, tag.v);
      }
      node.ChangeSetId = new long?(xml_node.changeset);
      node.TimeStamp = new DateTime?(xml_node.timestamp);
      node.UserName = xml_node.user;
      node.UserId = new long?(xml_node.uid);
      node.Version = new ulong?(xml_node.version);
      node.Visible = new bool?(xml_node.visible);
      return node;
    }

    private Way Convertv6XmlWay(way xml_way)
    {
      Way way = new Way();
      way.Id = new long?(xml_way.id);
      if (xml_way.tag != null)
      {
        way.Tags = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag in xml_way.tag)
          way.Tags.Add(tag.k, tag.v);
      }
      if (xml_way.nd != null)
      {
        way.Nodes = new List<long>();
        foreach (nd nd in xml_way.nd)
          way.Nodes.Add(nd.@ref);
      }
      way.ChangeSetId = new long?(xml_way.changeset);
      way.TimeStamp = new DateTime?(xml_way.timestamp);
      way.UserName = xml_way.user;
      way.UserId = new long?(xml_way.uid);
      way.Version = new ulong?(xml_way.version);
      way.Visible = new bool?(xml_way.visible);
      return way;
    }

    private Relation Convertv6XmlRelation(relation xml_relation)
    {
      Relation relation = new Relation();
      relation.Id = new long?(xml_relation.id);
      if (xml_relation.tag != null)
      {
        relation.Tags = (TagsCollectionBase) new TagsCollection();
        foreach (tag tag in xml_relation.tag)
          relation.Tags.Add(tag.k, tag.v);
      }
      if (xml_relation.member != null)
      {
        relation.Members = new List<RelationMember>();
        foreach (member member in xml_relation.member)
        {
          OsmGeoType? nullable = new OsmGeoType?();
          switch (member.type)
          {
            case memberType.node:
              nullable = new OsmGeoType?(OsmGeoType.Node);
              break;
            case memberType.way:
              nullable = new OsmGeoType?(OsmGeoType.Way);
              break;
            case memberType.relation:
              nullable = new OsmGeoType?(OsmGeoType.Relation);
              break;
          }
          relation.Members.Add(new RelationMember()
          {
            MemberId = new long?(member.@ref),
            MemberRole = member.role,
            MemberType = nullable
          });
        }
      }
      relation.ChangeSetId = new long?(xml_relation.changeset);
      relation.TimeStamp = new DateTime?(xml_relation.timestamp);
      relation.UserName = xml_relation.user;
      relation.UserId = new long?(xml_relation.uid);
      relation.Version = new ulong?(xml_relation.version);
      relation.Visible = new bool?(xml_relation.visible);
      return relation;
    }

    public Way WayCreate(Way way)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      way way1 = way.ConvertTo();
      way1.changeset = this._current_changeset.id;
      way1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.way = new way[1];
      osm1.way[0] = way1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      string s = this.DoApiCall(true, "api/0.6/way/create", APIConnection.Method.PUT, memoryStream.ToArray());
      long result;
      if (!long.TryParse(s, out result))
        throw new APIException(string.Format("Invalid response when creating a new way: {0}", (object) s));
      way.Id = new long?(result);
      return way;
    }

    public void WayUpdate(Way way)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      if (!way.Id.HasValue)
        throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
      way way1 = way.ConvertTo();
      way1.changeset = this._current_changeset.id;
      way1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.way = new way[1];
      osm1.way[0] = way1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      byte[] array = memoryStream.ToArray();
      this.DoApiCall(1 != 0, string.Format("api/0.6/way/{0}", (object) way.Id.Value), APIConnection.Method.PUT, array);
    }

    public void WayDelete(Way way)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      if (!way.Id.HasValue)
        throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
      way way1 = way.ConvertTo();
      way1.changeset = this._current_changeset.id;
      way1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.way = new way[1];
      osm1.way[0] = way1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      byte[] array = memoryStream.ToArray();
      this.DoApiCall(1 != 0, string.Format("api/0.6/way/{0}", (object) way.Id.Value), APIConnection.Method.DELETE, array);
    }

    public Way WayGet(long id)
    {
      string s = this.DoApiCall(0 != 0, string.Format("/api/0.6/way/{0}", (object) id), APIConnection.Method.GET, (byte[]) null);
      if (s != null && s.Trim().Length > 0)
        return this.Convertv6XmlWay((new OsmDocument((IXmlSource) new XmlReaderSource(XmlReader.Create((TextReader) new StringReader(s)))).Osm as osm).way[0]);
      return (Way) null;
    }

    public Relation RelationGet(long id)
    {
      string s = this.DoApiCall(0 != 0, string.Format("/api/0.6/relation/{0}", (object) id), APIConnection.Method.GET, (byte[]) null);
      if (s != null && s.Trim().Length > 0)
        return this.Convertv6XmlRelation((new OsmDocument((IXmlSource) new XmlReaderSource(XmlReader.Create((TextReader) new StringReader(s)))).Osm as osm).relation[0]);
      return (Relation) null;
    }

    public Relation RelationCreate(Relation relation)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      relation relation1 = relation.ConvertTo();
      relation1.changeset = this._current_changeset.id;
      relation1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.relation = new relation[1];
      osm1.relation[0] = relation1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      string s = this.DoApiCall(true, "api/0.6/relation/create", APIConnection.Method.PUT, memoryStream.ToArray());
      long result;
      if (!long.TryParse(s, out result))
        throw new APIException(string.Format("Invalid response when creating a new relation: {0}", (object) s));
      relation.Id = new long?(result);
      return relation;
    }

    public void RelationUpdate(Relation relation)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      if (!relation.Id.HasValue)
        throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
      relation relation1 = relation.ConvertTo();
      relation1.changeset = this._current_changeset.id;
      relation1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.relation = new relation[1];
      osm1.relation[0] = relation1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      byte[] array = memoryStream.ToArray();
      this.DoApiCall(1 != 0, string.Format("api/0.6/relation/{0}", (object) relation.Id.Value), APIConnection.Method.PUT, array);
    }

    public void RelationDelete(Relation relation)
    {
      if (this._current_changeset == null)
        throw new InvalidOperationException("No open changeset found!");
      if (!relation.Id.HasValue)
        throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
      relation relation1 = relation.ConvertTo();
      relation1.changeset = this._current_changeset.id;
      relation1.changesetSpecified = true;
      osm osm1 = new osm();
      osm1.relation = new relation[1];
      osm1.relation[0] = relation1;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      Stream stream2 = stream1;
      osm osm2 = osm1;
      xmlSerializer.Serialize(stream2, (object) osm2);
      stream1.Flush();
      memoryStream.Flush();
      byte[] array = memoryStream.ToArray();
      this.DoApiCall(1 != 0, string.Format("api/0.6/relation/{0}", (object) relation.Id.Value), APIConnection.Method.DELETE, array);
    }

    private enum Method
    {
      PUT,
      GET,
      DELETE,
    }
  }
}
