using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class osmChange
  {
    private OsmSharp.Osm.Xml.v0_6.create[] createField;
    private OsmSharp.Osm.Xml.v0_6.modify[] modifyField;
    private OsmSharp.Osm.Xml.v0_6.delete[] deleteField;
    private double versionField;
    private bool versionFieldSpecified;
    private string generatorField;

    [XmlElement("create")]
    public OsmSharp.Osm.Xml.v0_6.create[] create
    {
      get
      {
        return this.createField;
      }
      set
      {
        this.createField = value;
      }
    }

    [XmlElement("modify")]
    public OsmSharp.Osm.Xml.v0_6.modify[] modify
    {
      get
      {
        return this.modifyField;
      }
      set
      {
        this.modifyField = value;
      }
    }

    [XmlElement("delete")]
    public OsmSharp.Osm.Xml.v0_6.delete[] delete
    {
      get
      {
        return this.deleteField;
      }
      set
      {
        this.deleteField = value;
      }
    }

    [XmlAttribute]
    public double version
    {
      get
      {
        return this.versionField;
      }
      set
      {
        this.versionField = value;
      }
    }

    [XmlIgnore]
    public bool versionSpecified
    {
      get
      {
        return this.versionFieldSpecified;
      }
      set
      {
        this.versionFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public string generator
    {
      get
      {
        return this.generatorField;
      }
      set
      {
        this.generatorField = value;
      }
    }
  }
}
