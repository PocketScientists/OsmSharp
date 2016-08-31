using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class contributorterms
  {
    [XmlAttribute]
    public bool agreed { get; set; }

    [XmlAttribute]
    public bool pd { get; set; }
  }
}
