using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class version
  {
    private double minimumField;
    private bool minimumFieldSpecified;
    private double maximumField;
    private bool maximumFieldSpecified;

    [XmlAttribute]
    public double minimum
    {
      get
      {
        return this.minimumField;
      }
      set
      {
        this.minimumField = value;
      }
    }

    [XmlIgnore]
    public bool minimumSpecified
    {
      get
      {
        return this.minimumFieldSpecified;
      }
      set
      {
        this.minimumFieldSpecified = value;
      }
    }

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
