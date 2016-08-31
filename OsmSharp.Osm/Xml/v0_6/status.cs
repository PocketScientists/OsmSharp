using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class status
  {
    [XmlAttribute]
    public string api { get; set; }

    [XmlAttribute]
    public string database { get; set; }

    [XmlAttribute]
    public string gpx { get; set; }
  }
}
