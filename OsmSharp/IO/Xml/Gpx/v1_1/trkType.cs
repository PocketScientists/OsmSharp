using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class trkType
  {
    private string nameField;
    private string cmtField;
    private string descField;
    private string srcField;
    private linkType[] linkField;
    private string numberField;
    private string typeField;
    private trksegType[] trksegField;

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

    public string cmt
    {
      get
      {
        return this.cmtField;
      }
      set
      {
        this.cmtField = value;
      }
    }

    public string desc
    {
      get
      {
        return this.descField;
      }
      set
      {
        this.descField = value;
      }
    }

    public string src
    {
      get
      {
        return this.srcField;
      }
      set
      {
        this.srcField = value;
      }
    }

    [XmlElement("link")]
    public linkType[] link
    {
      get
      {
        return this.linkField;
      }
      set
      {
        this.linkField = value;
      }
    }

    [XmlElement(DataType = "nonNegativeInteger")]
    public string number
    {
      get
      {
        return this.numberField;
      }
      set
      {
        this.numberField = value;
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

    [XmlElement("trkseg")]
    public trksegType[] trkseg
    {
      get
      {
        return this.trksegField;
      }
      set
      {
        this.trksegField = value;
      }
    }
  }
}
