using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class UpdateType
  {
    private string targetHrefField;
    private object[] itemsField;

    [XmlElement(DataType = "anyURI")]
    public string targetHref
    {
      get
      {
        return this.targetHrefField;
      }
      set
      {
        this.targetHrefField = value;
      }
    }

    [XmlElement("Change", typeof (ChangeType))]
    [XmlElement("Create", typeof (CreateType))]
    [XmlElement("Delete", typeof (DeleteType))]
    [XmlElement("Replace", typeof (ReplaceType))]
    public object[] Items
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
  }
}
