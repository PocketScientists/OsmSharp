using OsmSharp.IO.Xml.Kml.v2_1;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml
{
  public class KmlDocument
  {
    private object _kml_object;
    private IXmlSource _source;
    private KmlVersion _version;

    public bool IsReadOnly
    {
      get
      {
        return this._source.IsReadOnly;
      }
    }

    public KmlVersion Version
    {
      get
      {
        return this._version;
      }
    }

    public object Kml
    {
      get
      {
        this.DoReadKml();
        return this._kml_object;
      }
      set
      {
        this._kml_object = value;
        this.FindVersionFromObject();
      }
    }

    public KmlDocument(IXmlSource source)
    {
      this._source = source;
      this._version = KmlVersion.Unknown;
    }

    public void Save()
    {
      this.DoWriteKml();
    }

    private void FindVersionFromObject()
    {
      this._version = KmlVersion.Unknown;
      if (this._kml_object is OsmSharp.IO.Xml.Kml.v2_0.kml)
        this._version = KmlVersion.Kmlv2_0;
      else if (this._kml_object is OsmSharp.IO.Xml.Kml.v2_0_response.kml)
      {
        this._version = KmlVersion.Kmlv2_0_response;
      }
      else
      {
        if (!(this._kml_object is KmlType))
          return;
        this._version = KmlVersion.Kmlv2_1;
      }
    }

    private void FindVersionFromSource()
    {
      XmlReader reader = this._source.GetReader();
      while (!reader.EOF)
      {
        if (reader.NodeType == XmlNodeType.Element && reader.Name == "kml")
        {
          string attribute = reader.GetAttribute("xmlns");
          if (!(attribute == "http://earth.google.com/kml/2.0"))
          {
            if (attribute == "http://earth.google.com/kml/2.1")
              this._version = KmlVersion.Kmlv2_1;
          }
          else
          {
            reader.Read();
            while (!reader.EOF)
            {
              if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "response")
              {
                this._version = KmlVersion.Kmlv2_0_response;
                break;
              }
              if (reader.NodeType == XmlNodeType.Element)
              {
                this._version = KmlVersion.Kmlv2_0;
                break;
              }
              reader.Read();
            }
          }
        }
        else if (reader.NodeType == XmlNodeType.Element)
          throw new XmlException("First element expected: kml!");
        if (this._version != KmlVersion.Unknown)
          break;
        reader.Read();
      }
    }

    private void DoReadKml()
    {
      if (this._kml_object != null || this._source == null)
        return;
      Type type = (Type) null;
      this.FindVersionFromSource();
      switch (this._version)
      {
        case KmlVersion.Kmlv2_1:
          type = typeof (KmlType);
          break;
        case KmlVersion.Kmlv2_0:
          type = typeof (OsmSharp.IO.Xml.Kml.v2_0.kml);
          break;
        case KmlVersion.Kmlv2_0_response:
          type = typeof (OsmSharp.IO.Xml.Kml.v2_0_response.kml);
          break;
        case KmlVersion.Unknown:
          throw new XmlException("Version could not be determined!");
      }
      XmlReader reader = this._source.GetReader();
      this._kml_object = new XmlSerializer(type).Deserialize(reader);
    }

    private void DoWriteKml()
    {
      if (this._kml_object == null)
        return;
      Type type = (Type) null;
      switch (this._version)
      {
        case KmlVersion.Kmlv2_1:
          type = typeof (KmlType);
          break;
        case KmlVersion.Kmlv2_0:
          type = typeof (OsmSharp.IO.Xml.Kml.v2_0.kml);
          break;
        case KmlVersion.Kmlv2_0_response:
          type = typeof (OsmSharp.IO.Xml.Kml.v2_0_response.kml);
          break;
        case KmlVersion.Unknown:
          throw new XmlException("Version could not be determined!");
      }
      XmlSerializer xmlSerializer = new XmlSerializer(type);
      XmlWriter writer = this._source.GetWriter();
      XmlWriter xmlWriter = writer;
      object kmlObject = this._kml_object;
      xmlSerializer.Serialize(xmlWriter, kmlObject);
      writer.Flush();
    }

    public void Close()
    {
      this._kml_object = (object) null;
      this._source = (IXmlSource) null;
    }
  }
}
