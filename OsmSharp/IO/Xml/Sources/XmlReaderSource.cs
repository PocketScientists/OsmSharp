using System.Xml;

namespace OsmSharp.IO.Xml.Sources
{
  public class XmlReaderSource : IXmlSource
  {
    private XmlReader _reader;

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
        return this._reader != null;
      }
    }

    public string Name
    {
      get
      {
        return "Generic Reader Source";
      }
    }

    public XmlReaderSource(XmlReader reader)
    {
      this._reader = reader;
    }

    public XmlReader GetReader()
    {
      return this._reader;
    }

    public XmlWriter GetWriter()
    {
      return (XmlWriter) null;
    }

    public void Close()
    {
      this._reader = (XmlReader) null;
    }
  }
}
