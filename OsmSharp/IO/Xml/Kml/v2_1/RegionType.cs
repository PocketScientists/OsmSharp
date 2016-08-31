using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Region", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class RegionType : ObjectType
  {
    private LatLonAltBoxType latLonAltBoxField;
    private LodType lodField;

    public LatLonAltBoxType LatLonAltBox
    {
      get
      {
        return this.latLonAltBoxField;
      }
      set
      {
        this.latLonAltBoxField = value;
      }
    }

    public LodType Lod
    {
      get
      {
        return this.lodField;
      }
      set
      {
        this.lodField = value;
      }
    }
  }
}
