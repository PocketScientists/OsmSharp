using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("IconStyle", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class IconStyleType : ColorStyleType
  {
    private float scaleField;
    private float headingField;
    private IconStyleIconType iconField;
    private vec2Type hotSpotField;

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

    [DefaultValue(typeof (float), "0")]
    public float heading
    {
      get
      {
        return this.headingField;
      }
      set
      {
        this.headingField = value;
      }
    }

    public IconStyleIconType Icon
    {
      get
      {
        return this.iconField;
      }
      set
      {
        this.iconField = value;
      }
    }

    public vec2Type hotSpot
    {
      get
      {
        return this.hotSpotField;
      }
      set
      {
        this.hotSpotField = value;
      }
    }

    public IconStyleType()
    {
      this.scaleField = 1f;
      this.headingField = 0.0f;
    }
  }
}
