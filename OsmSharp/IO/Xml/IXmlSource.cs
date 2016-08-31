using System.Xml;

namespace OsmSharp.IO.Xml
{
  public interface IXmlSource
  {
    bool IsReadOnly { get; }

    bool HasData { get; }

    XmlReader GetReader();

    XmlWriter GetWriter();

    void Close();
  }
}
