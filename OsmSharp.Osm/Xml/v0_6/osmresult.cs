using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public abstract class osmresult
  {
    [XmlAttribute]
    public long old_id { get; set; }

    [XmlIgnore]
    public bool old_idSpecified { get; set; }

    [XmlAttribute]
    public long new_id { get; set; }

    [XmlIgnore]
    public bool new_idSpecified { get; set; }

    [XmlAttribute]
    public int new_version { get; set; }

    [XmlIgnore]
    public bool new_versionSpecified { get; set; }
  }
}
