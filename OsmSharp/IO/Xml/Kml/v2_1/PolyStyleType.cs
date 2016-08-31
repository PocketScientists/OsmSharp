using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("PolyStyle", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class PolyStyleType : ColorStyleType
  {
    private bool fillField;
    private bool outlineField;

    [DefaultValue(true)]
    public bool fill
    {
      get
      {
        return this.fillField;
      }
      set
      {
        this.fillField = value;
      }
    }

    [DefaultValue(true)]
    public bool outline
    {
      get
      {
        return this.outlineField;
      }
      set
      {
        this.outlineField = value;
      }
    }

    public PolyStyleType()
    {
      this.fillField = true;
      this.outlineField = true;
    }
  }
}
