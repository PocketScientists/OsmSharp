using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  public class area
  {
    private double maximumField;
    private bool maximumFieldSpecified;

    [XmlAttribute]
    public double maximum
    {
      get
      {
        return this.maximumField;
      }
      set
      {
        this.maximumField = value;
      }
    }

    [XmlIgnore]
    public bool maximumSpecified
    {
      get
      {
        return this.maximumFieldSpecified;
      }
      set
      {
        this.maximumFieldSpecified = value;
      }
    }
  }
}
