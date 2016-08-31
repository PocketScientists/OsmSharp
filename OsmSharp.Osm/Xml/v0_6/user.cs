using System;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class user
  {
    [XmlAttribute]
    public int id { get; set; }

    [XmlAttribute]
    public string display_name { get; set; }

    [XmlAttribute]
    public DateTime account_created { get; set; }

    public string description { get; set; }

    public img img { get; set; }

    [XmlElement("contributor-terms")]
    public contributorterms contributorterms { get; set; }

    public role[] roles { get; set; }

    public userchangeset changesets { get; set; }

    public traces traces { get; set; }

    public block[] blocks { get; set; }
  }
}
