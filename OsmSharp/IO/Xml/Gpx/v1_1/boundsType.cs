using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class boundsType
  {
    private Decimal minlatField;
    private Decimal minlonField;
    private Decimal maxlatField;
    private Decimal maxlonField;

    [XmlAttribute]
    public Decimal minlat
    {
      get
      {
        return this.minlatField;
      }
      set
      {
        this.minlatField = value;
      }
    }

    [XmlAttribute]
    public Decimal minlon
    {
      get
      {
        return this.minlonField;
      }
      set
      {
        this.minlonField = value;
      }
    }

    [XmlAttribute]
    public Decimal maxlat
    {
      get
      {
        return this.maxlatField;
      }
      set
      {
        this.maxlatField = value;
      }
    }

    [XmlAttribute]
    public Decimal maxlon
    {
      get
      {
        return this.maxlonField;
      }
      set
      {
        this.maxlonField = value;
      }
    }
  }
}
