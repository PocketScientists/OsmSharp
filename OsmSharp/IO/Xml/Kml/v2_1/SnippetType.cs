using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class SnippetType
  {
    private int maxLinesField;
    private string valueField;

    [XmlAttribute]
    [DefaultValue(2)]
    public int maxLines
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

    public SnippetType()
    {
      this.maxLinesField = 2;
    }
  }
}
