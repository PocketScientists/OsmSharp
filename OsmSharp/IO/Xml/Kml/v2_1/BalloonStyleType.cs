using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("BalloonStyle", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class BalloonStyleType : ObjectType
  {
    private byte[] itemField;
    private ItemChoiceType itemElementNameField;
    private byte[] textColorField;
    private string textField;

    [XmlElement("bgColor", typeof (byte[]), DataType = "hexBinary")]
    [XmlElement("color", typeof (byte[]), DataType = "hexBinary")]
    [XmlChoiceIdentifier("ItemElementName")]
    public byte[] Item
    {
      get
      {
        return this.itemField;
      }
      set
      {
        this.itemField = value;
      }
    }

    [XmlIgnore]
    public ItemChoiceType ItemElementName
    {
      get
      {
        return this.itemElementNameField;
      }
      set
      {
        this.itemElementNameField = value;
      }
    }

    [XmlElement(DataType = "hexBinary")]
    public byte[] textColor
    {
      get
      {
        return this.textColorField;
      }
      set
      {
        this.textColorField = value;
      }
    }

    public string text
    {
      get
      {
        return this.textField;
      }
      set
      {
        this.textField = value;
      }
    }
  }
}
