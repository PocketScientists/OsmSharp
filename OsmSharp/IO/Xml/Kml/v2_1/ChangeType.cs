using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class ChangeType
  {
    private ObjectType[] itemsField;
    private ItemsChoiceType[] itemsElementNameField;

    [XmlElement("BalloonStyle", typeof (BalloonStyleType))]
    [XmlElement("Document", typeof (DocumentType))]
    [XmlElement("Feature", typeof (FeatureType))]
    [XmlElement("Folder", typeof (FolderType))]
    [XmlElement("Geometry", typeof (GeometryType))]
    [XmlElement("GroundOverlay", typeof (GroundOverlayType))]
    [XmlElement("Icon", typeof (LinkType))]
    [XmlElement("IconStyle", typeof (IconStyleType))]
    [XmlElement("LabelStyle", typeof (LabelStyleType))]
    [XmlElement("LatLonBox", typeof (LatLonBoxType))]
    [XmlElement("LineString", typeof (LineStringType))]
    [XmlElement("LineStyle", typeof (LineStyleType))]
    [XmlElement("LinearRing", typeof (LinearRingType))]
    [XmlElement("Link", typeof (LinkType))]
    [XmlElement("ListStyle", typeof (ListStyleType))]
    [XmlElement("Location", typeof (LocationType))]
    [XmlElement("Lod", typeof (LodType))]
    [XmlElement("LookAt", typeof (LookAtType))]
    [XmlElement("Model", typeof (ModelType))]
    [XmlElement("MultiGeometry", typeof (MultiGeometryType))]
    [XmlElement("NetworkLink", typeof (NetworkLinkType))]
    [XmlElement("Object", typeof (ObjectType))]
    [XmlElement("Orientation", typeof (OrientationType))]
    [XmlElement("Placemark", typeof (PlacemarkType))]
    [XmlElement("Point", typeof (PointType))]
    [XmlElement("PolyStyle", typeof (PolyStyleType))]
    [XmlElement("Polygon", typeof (PolygonType))]
    [XmlElement("Region", typeof (RegionType))]
    [XmlElement("Scale", typeof (ScaleType))]
    [XmlElement("ScreenOverlay", typeof (ScreenOverlayType))]
    [XmlElement("Style", typeof (StyleType))]
    [XmlElement("StyleMap", typeof (StyleMapType))]
    [XmlElement("StyleSelector", typeof (StyleSelectorType))]
    [XmlElement("TimePrimitive", typeof (TimePrimitiveType))]
    [XmlElement("TimeSpan", typeof (TimeSpanType))]
    [XmlElement("TimeStamp", typeof (TimeStampType))]
    [XmlChoiceIdentifier("ItemsElementName")]
    public ObjectType[] Items
    {
      get
      {
        return this.itemsField;
      }
      set
      {
        this.itemsField = value;
      }
    }

    [XmlElement("ItemsElementName")]
    [XmlIgnore]
    public ItemsChoiceType[] ItemsElementName
    {
      get
      {
        return this.itemsElementNameField;
      }
      set
      {
        this.itemsElementNameField = value;
      }
    }
  }
}
