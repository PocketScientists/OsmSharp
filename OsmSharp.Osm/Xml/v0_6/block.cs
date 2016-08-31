using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class block
  {
    [XmlAttribute]
    public int count { get; set; }

    [XmlAttribute]
    public int active { get; set; }
  }
}
