using OsmSharp.IO.Xml.Gpx.v1_0;
using OsmSharp.IO.Xml.Gpx.v1_1;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx
{
  public class GpxDocument
  {
    private object _gpx_object;
    private IXmlSource _source;
    private GpxVersion _version;

    public bool IsReadOnly
    {
      get
      {
        return this._source.IsReadOnly;
      }
    }

    public GpxVersion Version
    {
      get
      {
        return this._version;
      }
    }

    public object Gpx
    {
      get
      {
        this.DoReadGpx();
        return this._gpx_object;
      }
      set
      {
        this._gpx_object = value;
        this.FindVersionFromObject();
      }
    }

    public GpxDocument(IXmlSource source)
    {
      this._source = source;
      this._version = GpxVersion.Unknown;
    }

    public void Save()
    {
      this.DoWriteGpx();
    }

    private void FindVersionFromObject()
    {
      this._version = GpxVersion.Unknown;
      if (this._gpx_object is gpx)
      {
        this._version = GpxVersion.Gpxv1_0;
      }
      else
      {
        if (!(this._gpx_object is gpxType))
          return;
        this._version = GpxVersion.Gpxv1_1;
      }
    }

    private void FindVersionFromSource()
    {
      XmlReader reader = this._source.GetReader();
      while (!reader.EOF)
      {
        if (reader.NodeType == XmlNodeType.Element && reader.Name == "gpx")
        {
          string attribute = reader.GetAttribute("xmlns");
          if (!(attribute == "http://www.topografix.com/GPX/1/0"))
          {
            if (attribute == "http://www.topografix.com/GPX/1/1")
              this._version = GpxVersion.Gpxv1_1;
          }
          else
            this._version = GpxVersion.Gpxv1_0;
        }
        else if (reader.NodeType == XmlNodeType.Element)
          throw new XmlException("First element expected: gpx!");
        if (this._version != GpxVersion.Unknown)
          break;
        reader.Read();
      }
    }

    private void DoReadGpx()
    {
      if (this._gpx_object != null)
        return;
      Type type = (Type) null;
      this.FindVersionFromSource();
      switch (this._version)
      {
        case GpxVersion.Gpxv1_0:
          type = typeof (gpx);
          break;
        case GpxVersion.Gpxv1_1:
          type = typeof (gpxType);
          break;
        case GpxVersion.Unknown:
          throw new XmlException("Version could not be determined!");
      }
      this._gpx_object = new XmlSerializer(type).Deserialize(this._source.GetReader());
    }

    private void DoWriteGpx()
    {
      if (this._gpx_object == null)
        return;
      Type type = (Type) null;
      switch (this._version)
      {
        case GpxVersion.Gpxv1_0:
          type = typeof (gpx);
          break;
        case GpxVersion.Gpxv1_1:
          type = typeof (gpxType);
          break;
        case GpxVersion.Unknown:
          throw new XmlException("Version could not be determined!");
      }
      XmlSerializer xmlSerializer = new XmlSerializer(type);
      XmlWriter writer = this._source.GetWriter();
      XmlWriter xmlWriter = writer;
      object gpxObject = this._gpx_object;
      xmlSerializer.Serialize(xmlWriter, gpxObject);
      writer.Flush();
    }

    public void Close()
    {
    }
  }
}
