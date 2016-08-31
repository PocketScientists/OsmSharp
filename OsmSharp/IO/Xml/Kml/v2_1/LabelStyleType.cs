using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("LabelStyle", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LabelStyleType : ColorStyleType
  {
    private float scaleField;

    [DefaultValue(typeof (float), "1")]
    public float scale
    {
      get
      {
        return this.scaleField;
      }
      set
      {
        this.scaleField = value;
      }
    }

    public LabelStyleType()
    {
      this.scaleField = 1f;
    }
  }
}
