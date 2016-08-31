using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class diffResult
  {
    [XmlAttribute]
    public string generator { get; set; }

    [XmlAttribute]
    public double version { get; set; }

    [XmlIgnore]
    public bool versionSpecified { get; set; }

    [XmlElement(ElementName = "node", Type = typeof (noderesult))]
    [XmlElement(ElementName = "way", Type = typeof (wayresult))]
    [XmlElement(ElementName = "relation", Type = typeof (relationresult))]
    public OsmSharp.Osm.Xml.v0_6.osmresult[] osmresult { get; set; }
  }
}
