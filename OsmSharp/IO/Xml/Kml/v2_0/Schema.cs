using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Schema
  {
    private string nameField;
    private ObjArrayField objArrayFieldField;
    private ObjField objFieldField;
    private SimpleArrayField simpleArrayFieldField;
    private SimpleField simpleFieldField;
    private string parentField;
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

    public ObjArrayField ObjArrayField
    {
      get
      {
        return this.objArrayFieldField;
      }
      set
      {
        this.objArrayFieldField = value;
      }
    }

    public ObjField ObjField
    {
      get
      {
        return this.objFieldField;
      }
      set
      {
        this.objFieldField = value;
      }
    }

    public SimpleArrayField SimpleArrayField
    {
      get
      {
        return this.simpleArrayFieldField;
      }
      set
      {
        this.simpleArrayFieldField = value;
      }
    }

    public SimpleField SimpleField
    {
      get
      {
        return this.simpleFieldField;
      }
      set
      {
        this.simpleFieldField = value;
      }
    }

    public string parent
    {
      get
      {
        return this.parentField;
      }
      set
      {
        this.parentField = value;
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
