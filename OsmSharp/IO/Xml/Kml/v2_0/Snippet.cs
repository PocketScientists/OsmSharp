using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Snippet
  {
    private byte maxLinesField;
    private bool maxLinesFieldSpecified;
    private string valueField;

    [XmlAttribute]
    public byte maxLines
    {
      get
      {
        return this.maxLinesField;
      }
      set
      {
        this.maxLinesField = value;
      }
    }

    [XmlIgnore]
    public bool maxLinesSpecified
    {
      get
      {
        return this.maxLinesFieldSpecified;
      }
      set
      {
        this.maxLinesFieldSpecified = value;
      }
    }

    [XmlText]
    public string Value
    {
      get
      {
        return this.valueField;
      }
      set
      {
        this.valueField = value;
      }
    }
  }
}
