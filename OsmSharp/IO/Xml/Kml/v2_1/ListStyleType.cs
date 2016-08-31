using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("ListStyle", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class ListStyleType : ObjectType
  {
    private listItemTypeEnum listItemTypeField;
    private byte[] bgColorField;
    private ItemIconType[] itemIconField;

    [DefaultValue(listItemTypeEnum.check)]
    public listItemTypeEnum listItemType
    {
      get
      {
        return this.listItemTypeField;
      }
      set
      {
        this.listItemTypeField = value;
      }
    }

    [XmlElement(DataType = "hexBinary")]
    public byte[] bgColor
    {
      get
      {
        return this.bgColorField;
      }
      set
      {
        this.bgColorField = value;
      }
    }

    [XmlElement("ItemIcon")]
    public ItemIconType[] ItemIcon
    {
      get
      {
        return this.itemIconField;
      }
      set
      {
        this.itemIconField = value;
      }
    }

    public ListStyleType()
    {
      this.listItemTypeField = listItemTypeEnum.check;
    }
  }
}
