using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  [XmlRoot("gpx", IsNullable = false, Namespace = "http://www.topografix.com/GPX/1/1")]
  public class gpxType
  {
    private metadataType metadataField;
    private wptType[] wptField;
    private rteType[] rteField;
    private trkType[] trkField;
    private string versionField;
    private string creatorField;

    public metadataType metadata
    {
      get
      {
        return this.metadataField;
      }
      set
      {
        this.metadataField = value;
      }
    }

    [XmlElement("wpt")]
    public wptType[] wpt
    {
      get
      {
        return this.wptField;
      }
      set
      {
        this.wptField = value;
      }
    }

    [XmlElement("rte")]
    public rteType[] rte
    {
      get
      {
        return this.rteField;
      }
      set
      {
        this.rteField = value;
      }
    }

    [XmlElement("trk")]
    public trkType[] trk
    {
      get
      {
        return this.trkField;
      }
      set
      {
        this.trkField = value;
      }
    }

    [XmlAttribute]
    public string version
    {
      get
      {
        return this.versionField;
      }
      set
      {
        this.versionField = value;
      }
    }

    [XmlAttribute]
    public string creator
    {
      get
      {
        return this.creatorField;
      }
      set
      {
        this.creatorField = value;
      }
    }

    public gpxType()
    {
      this.versionField = "1.1";
    }
  }
}
