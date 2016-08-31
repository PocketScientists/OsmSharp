using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Style", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class StyleType : StyleSelectorType
  {
    private IconStyleType iconStyleField;
    private LabelStyleType labelStyleField;
    private LineStyleType lineStyleField;
    private PolyStyleType polyStyleField;
    private BalloonStyleType balloonStyleField;
    private ListStyleType listStyleField;

    public IconStyleType IconStyle
    {
      get
      {
        return this.iconStyleField;
      }
      set
      {
        this.iconStyleField = value;
      }
    }

    public LabelStyleType LabelStyle
    {
      get
      {
        return this.labelStyleField;
      }
      set
      {
        this.labelStyleField = value;
      }
    }

    public LineStyleType LineStyle
    {
      get
      {
        return this.lineStyleField;
      }
      set
      {
        this.lineStyleField = value;
      }
    }

    public PolyStyleType PolyStyle
    {
      get
      {
        return this.polyStyleField;
      }
      set
      {
        this.polyStyleField = value;
      }
    }

    public BalloonStyleType BalloonStyle
    {
      get
      {
        return this.balloonStyleField;
      }
      set
      {
        this.balloonStyleField = value;
      }
    }

    public ListStyleType ListStyle
    {
      get
      {
        return this.listStyleField;
      }
      set
      {
        this.listStyleField = value;
      }
    }
  }
}
