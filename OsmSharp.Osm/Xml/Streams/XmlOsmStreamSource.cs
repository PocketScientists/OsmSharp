using Ionic.Zlib;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Xml.v0_6;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.Streams
{
  public class XmlOsmStreamSource : OsmStreamSource
  {
    private XmlReader _reader;
    private XmlSerializer _serNode;
    private XmlSerializer _serWay;
    private XmlSerializer _serRelation;
    private OsmGeo _next;
    private Stream _stream;
    private readonly bool _gzip;
    private readonly bool _disposeStream;

    public override bool CanReset
    {
      get
      {
        return this._stream.CanSeek;
      }
    }

    public XmlOsmStreamSource(Stream stream)
      : this(stream, false)
    {
    }

    public XmlOsmStreamSource(Stream stream, bool gzip)
    {
      this._stream = stream;
      this._gzip = gzip;
    }

    public override void Initialize()
    {
      this._next = (OsmGeo) null;
      this._serNode = new XmlSerializer(typeof (node));
      this._serWay = new XmlSerializer(typeof (way));
      this._serRelation = new XmlSerializer(typeof (relation));
      this.Reset();
    }

    public override void Reset()
    {
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.CloseInput = true;
      settings.CheckCharacters = false;
      settings.IgnoreComments = true;
      settings.IgnoreProcessingInstructions = true;
      if (this._stream.CanSeek)
        this._stream.Seek(0L, SeekOrigin.Begin);
      if (this._gzip)
        this._stream = (Stream) new GZipStream(this._stream, (CompressionMode) 1);
      this._reader = XmlReader.Create((TextReader) new StreamReader(this._stream, Encoding.UTF8), settings);
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      while (this._reader.Read())
      {
        if (this._reader.NodeType == XmlNodeType.Element && this._reader.Name == "node" && !ignoreNodes || (this._reader.Name == "way" && !ignoreWays || this._reader.Name == "relation" && !ignoreRelations))
        {
          string name = this._reader.Name;
          XmlReader xmlReader = XmlReader.Create((Stream) new MemoryStream(Encoding.UTF8.GetBytes(this._reader.ReadOuterXml())));
          if (!(name == "node"))
          {
            if (!(name == "way"))
            {
              if (name == "relation")
              {
                object obj = this._serRelation.Deserialize(xmlReader);
                if (obj is relation)
                {
                  this._next = (OsmGeo) XmlSimpleConverter.ConvertToSimple(obj as relation);
                  return true;
                }
              }
            }
            else
            {
              object obj = this._serWay.Deserialize(xmlReader);
              if (obj is way)
              {
                this._next = (OsmGeo) XmlSimpleConverter.ConvertToSimple(obj as way);
                return true;
              }
            }
          }
          else
          {
            object obj = this._serNode.Deserialize(xmlReader);
            if (obj is node)
            {
              this._next = (OsmGeo) XmlSimpleConverter.ConvertToSimple(obj as node);
              return true;
            }
          }
        }
      }
      this._next = (OsmGeo) null;
      return false;
    }

    public override OsmGeo Current()
    {
      return this._next;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (!this._disposeStream)
        return;
      this._stream.Dispose();
    }
  }
}
