using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("ScreenOverlay", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class ScreenOverlayType : OverlayType
  {
    private vec2Type overlayXYField;
    private vec2Type screenXYField;
    private vec2Type rotationXYField;
    private vec2Type sizeField;
    private float rotationField;

    public vec2Type overlayXY
    {
      get
      {
        return this.overlayXYField;
      }
      set
      {
        this.overlayXYField = value;
      }
    }

    public vec2Type screenXY
    {
      get
      {
        return this.screenXYField;
      }
      set
      {
        this.screenXYField = value;
      }
    }

    public vec2Type rotationXY
    {
      get
      {
        return this.rotationXYField;
      }
      set
      {
        this.rotationXYField = value;
      }
    }

    public vec2Type size
    {
      get
      {
        return this.sizeField;
      }
      set
      {
        this.sizeField = value;
      }
    }

    [DefaultValue(typeof (float), "0")]
    public float rotation
    {
      get
      {
        return this.rotationField;
      }
      set
      {
        this.rotationField = value;
      }
    }

    public ScreenOverlayType()
    {
      this.rotationField = 0.0f;
    }
  }
}
