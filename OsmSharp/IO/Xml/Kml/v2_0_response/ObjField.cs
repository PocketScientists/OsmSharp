using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class ObjField
  {
    private string nameField;
    private string typeField;
    private string idField;

    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    public string type
    {
      get
      {
        return this.typeField;
      }
      set
      {
        this.typeField = value;
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
