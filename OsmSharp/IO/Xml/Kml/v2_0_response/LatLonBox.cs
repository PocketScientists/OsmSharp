using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class LatLonBox
  {
    private double northField;
    private double eastField;
    private double southField;
    private double westField;
    private Decimal rotationField;
    private bool rotationFieldSpecified;
    private string idField;

    public double north
    {
      get
      {
        return this.northField;
      }
      set
      {
        this.northField = value;
      }
    }

    public double east
    {
      get
      {
        return this.eastField;
      }
      set
      {
        this.eastField = value;
      }
    }

    public double south
    {
      get
      {
        return this.southField;
      }
      set
      {
        this.southField = value;
      }
    }

    public double west
    {
      get
      {
        return this.westField;
      }
      set
      {
        this.westField = value;
      }
    }

    public Decimal rotation
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

    [XmlIgnore]
    public bool rotationSpecified
    {
      get
      {
        return this.rotationFieldSpecified;
      }
      set
      {
        this.rotationFieldSpecified = value;
      }
    }

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
  }
}
