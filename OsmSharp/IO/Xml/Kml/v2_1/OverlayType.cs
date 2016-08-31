using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (ScreenOverlayType))]
  [XmlInclude(typeof (GroundOverlayType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public abstract class OverlayType : FeatureType
  {
    private byte[] colorField;
    private int drawOrderField;
    private LinkType iconField;

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

    [DefaultValue(0)]
    public int drawOrder
    {
      get
      {
        return this.drawOrderField;
      }
      set
      {
        this.drawOrderField = value;
      }
    }

    public LinkType Icon
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

    public OverlayType()
    {
      this.drawOrderField = 0;
    }
  }
}
