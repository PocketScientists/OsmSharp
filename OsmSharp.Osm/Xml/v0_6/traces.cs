using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class traces
  {
    [XmlAttribute]
    public int count { get; set; }
  }
}
