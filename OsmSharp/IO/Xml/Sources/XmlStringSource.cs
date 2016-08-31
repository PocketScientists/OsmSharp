using System.IO;
using System.Xml;

namespace OsmSharp.IO.Xml.Sources
{
  public class XmlStringSource : IXmlSource
  {
    private string _source;

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public bool HasData
    {
      get
      {
        if (this._source != null)
          return this._source.Length > 0;
        return false;
      }
    }

    public string Name
    {
      get
      {
        return "Generic String Source";
      }
    }

    public XmlStringSource(string source)
    {
      this._source = source;
    }

    public XmlReader GetReader()
    {
      if (this._source == null)
        return (XmlReader) null;
      return XmlReader.Create((TextReader) new StringReader(this._source));
    }

    public XmlWriter GetWriter()
    {
      return (XmlWriter) null;
    }

    public void Close()
    {
      this._source = (string) null;
    }
  }
}
