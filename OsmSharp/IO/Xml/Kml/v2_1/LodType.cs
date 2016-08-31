using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Lod", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LodType : ObjectType
  {
    private float minLodPixelsField;
    private float maxLodPixelsField;
    private float minFadeExtentField;
    private float maxFadeExtentField;

    [DefaultValue(typeof (float), "0")]
    public float minLodPixels
    {
      get
      {
        return this.minLodPixelsField;
      }
      set
      {
        this.minLodPixelsField = value;
      }
    }

    [DefaultValue(typeof (float), "-1")]
    public float maxLodPixels
    {
      get
      {
        return this.maxLodPixelsField;
      }
      set
      {
        this.maxLodPixelsField = value;
      }
    }

    [DefaultValue(typeof (float), "0")]
    public float minFadeExtent
    {
      get
      {
        return this.minFadeExtentField;
      }
      set
      {
        this.minFadeExtentField = value;
      }
    }

    [DefaultValue(typeof (float), "0")]
    public float maxFadeExtent
    {
      get
      {
        return this.maxFadeExtentField;
      }
      set
      {
        this.maxFadeExtentField = value;
      }
    }

    public LodType()
    {
      this.minLodPixelsField = 0.0f;
      this.maxLodPixelsField = -1f;
      this.minFadeExtentField = 0.0f;
      this.maxFadeExtentField = 0.0f;
    }
  }
}
