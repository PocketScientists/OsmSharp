using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class userchangeset
  {
    [XmlAttribute]
    public int count { get; set; }
  }
}
