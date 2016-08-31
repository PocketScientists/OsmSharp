using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class metadataType
  {
    private string nameField;
    private string descField;
    private personType authorField;
    private copyrightType copyrightField;
    private linkType[] linkField;
    private DateTime timeField;
    private bool timeFieldSpecified;
    private string keywordsField;
    private boundsType boundsField;

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

    public personType author
    {
      get
      {
        return this.authorField;
      }
      set
      {
        this.authorField = value;
      }
    }

    public copyrightType copyright
    {
      get
      {
        return this.copyrightField;
      }
      set
      {
        this.copyrightField = value;
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

    public DateTime time
    {
      get
      {
        return this.timeField;
      }
      set
      {
        this.timeField = value;
      }
    }

    [XmlIgnore]
    public bool timeSpecified
    {
      get
      {
        return this.timeFieldSpecified;
      }
      set
      {
        this.timeFieldSpecified = value;
      }
    }

    public string keywords
    {
      get
      {
        return this.keywordsField;
      }
      set
      {
        this.keywordsField = value;
      }
    }

    public boundsType bounds
    {
      get
      {
        return this.boundsField;
      }
      set
      {
        this.boundsField = value;
      }
    }
  }
}
