using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (PolyStyleType))]
  [XmlInclude(typeof (LineStyleType))]
  [XmlInclude(typeof (LabelStyleType))]
  [XmlInclude(typeof (IconStyleType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public abstract class ColorStyleType : ObjectType
  {
    private byte[] colorField;
    private colorModeEnum colorModeField;

    [XmlElement(DataType = "hexBinary")]
    public byte[] color
    {
      get
      {
        return this.colorField;
      }
      set
      {
        this.colorField = value;
      }
    }

    [DefaultValue(colorModeEnum.normal)]
    public colorModeEnum colorMode
    {
      get
      {
        return this.colorModeField;
      }
      set
      {
        this.colorModeField = value;
      }
    }

    public ColorStyleType()
    {
      this.colorModeField = colorModeEnum.normal;
    }
  }
}
