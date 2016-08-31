using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (LinkType))]
  [XmlInclude(typeof (IconType))]
  [XmlInclude(typeof (ScaleType))]
  [XmlInclude(typeof (OrientationType))]
  [XmlInclude(typeof (LocationType))]
  [XmlInclude(typeof (GeometryType))]
  [XmlInclude(typeof (ModelType))]
  [XmlInclude(typeof (PolygonType))]
  [XmlInclude(typeof (LinearRingType))]
  [XmlInclude(typeof (LineStringType))]
  [XmlInclude(typeof (PointType))]
  [XmlInclude(typeof (MultiGeometryType))]
  [XmlInclude(typeof (LodType))]
  [XmlInclude(typeof (LatLonBoxType))]
  [XmlInclude(typeof (LatLonAltBoxType))]
  [XmlInclude(typeof (RegionType))]
  [XmlInclude(typeof (ItemIconType))]
  [XmlInclude(typeof (ListStyleType))]
  [XmlInclude(typeof (BalloonStyleType))]
  [XmlInclude(typeof (IconStyleIconType))]
  [XmlInclude(typeof (ColorStyleType))]
  [XmlInclude(typeof (PolyStyleType))]
  [XmlInclude(typeof (LineStyleType))]
  [XmlInclude(typeof (LabelStyleType))]
  [XmlInclude(typeof (IconStyleType))]
  [XmlInclude(typeof (StyleSelectorType))]
  [XmlInclude(typeof (StyleMapType))]
  [XmlInclude(typeof (StyleType))]
  [XmlInclude(typeof (TimePrimitiveType))]
  [XmlInclude(typeof (TimeSpanType))]
  [XmlInclude(typeof (TimeStampType))]
  [XmlInclude(typeof (LookAtType))]
  [XmlInclude(typeof (FeatureType))]
  [XmlInclude(typeof (OverlayType))]
  [XmlInclude(typeof (ScreenOverlayType))]
  [XmlInclude(typeof (GroundOverlayType))]
  [XmlInclude(typeof (NetworkLinkType))]
  [XmlInclude(typeof (PlacemarkType))]
  [XmlInclude(typeof (ContainerType))]
  [XmlInclude(typeof (FolderType))]
  [XmlInclude(typeof (DocumentType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public abstract class ObjectType
  {
    private string idField;
    private string targetIdField;

    [XmlAttribute(DataType = "ID")]
    public string id
    {
      get
      {
        return this.idField;
      }
      set
      {
        this.idField = value;
      }
    }

    [XmlAttribute(DataType = "NCName")]
    public string targetId
    {
      get
      {
        return this.targetIdField;
      }
      set
      {
        this.targetIdField = value;
      }
    }
  }
}
