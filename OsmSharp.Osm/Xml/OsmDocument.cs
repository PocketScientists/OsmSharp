using OsmSharp.IO.Xml;
using OsmSharp.Osm.Xml.v0_6;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml
{
  public class OsmDocument
  {
    private object _osmObject;
    private IXmlSource _source;

    public bool IsReadOnly
    {
      get
      {
        return this._source.IsReadOnly;
      }
    }

    public object Osm
    {
      get
      {
        this.DoReadOsm();
        return this._osmObject;
      }
      set
      {
        this._osmObject = value;
      }
    }

    public OsmDocument(IXmlSource source)
    {
      this._source = source;
    }

    public void Save()
    {
      this.DoWriteOsm();
    }

    private void DoReadOsm()
    {
      if (this._osmObject != null || !this._source.HasData)
        return;
      this._osmObject = new XmlSerializer(typeof (osm)).Deserialize(this._source.GetReader());
    }

    private void DoWriteOsm()
    {
      if (this._osmObject == null)
        return;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (osm));
      XmlWriter writer = this._source.GetWriter();
      XmlWriter xmlWriter = writer;
      object osmObject = this._osmObject;
      xmlSerializer.Serialize(xmlWriter, osmObject);
      writer.Flush();
    }

    public void Close()
    {
      this._source = (IXmlSource) null;
      this._osmObject = (object) null;
    }
  }
}
