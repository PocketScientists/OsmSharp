using OsmSharp.Osm.Streams.ChangeSets;
using OsmSharp.Osm.Xml.v0_6;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.Streams.ChangeSets
{
  public class XmlDataProcessorChangeSetSource : DataProcessorChangeSetSource
  {
    private ChangeSet _next;
    private XmlSerializer _ser_create;
    private XmlSerializer _ser_modify;
    private XmlSerializer _ser_delete;
    private XmlReader _reader;
    private Stream _stream;

    public XmlDataProcessorChangeSetSource(Stream stream)
    {
      this._stream = stream;
    }

    public override void Initialize()
    {
      this._next = (ChangeSet) null;
      this._ser_create = new XmlSerializer(typeof (create));
      this._ser_modify = new XmlSerializer(typeof (modify));
      this._ser_delete = new XmlSerializer(typeof (delete));
      this._reader = XmlReader.Create(this._stream, new XmlReaderSettings()
      {
        CloseInput = true,
        CheckCharacters = false,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true
      });
    }

    public override bool MoveNext()
    {
      while (this._reader.Read())
      {
        if (this._reader.NodeType == XmlNodeType.Element && (this._reader.Name == "modify" || this._reader.Name == "create" || this._reader.Name == "delete"))
        {
          string name = this._reader.Name;
          XmlReader xmlReader = XmlReader.Create((Stream) new MemoryStream(Encoding.UTF8.GetBytes(this._reader.ReadOuterXml())));
          if (!(name == "delete"))
          {
            if (!(name == "modify"))
            {
              if (name == "create")
              {
                object obj = this._ser_create.Deserialize(xmlReader);
                if (obj is create)
                {
                  this._next = XmlSimpleConverter.ConvertToSimple(obj as create);
                  return true;
                }
              }
            }
            else
            {
              object obj = this._ser_modify.Deserialize(xmlReader);
              if (obj is modify)
              {
                this._next = XmlSimpleConverter.ConvertToSimple(obj as modify);
                return true;
              }
            }
          }
          else
          {
            object obj = this._ser_delete.Deserialize(xmlReader);
            if (obj is delete)
            {
              this._next = XmlSimpleConverter.ConvertToSimple(obj as delete);
              return true;
            }
          }
        }
      }
      this._next = (ChangeSet) null;
      return false;
    }

    public override ChangeSet Current()
    {
      return this._next;
    }

    public override void Reset()
    {
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.CloseInput = true;
      settings.CheckCharacters = false;
      settings.IgnoreComments = true;
      settings.IgnoreProcessingInstructions = true;
      this._stream.Seek(0L, SeekOrigin.Begin);
      this._reader = XmlReader.Create(this._stream, settings);
    }

    public override void Close()
    {
    }
  }
}
