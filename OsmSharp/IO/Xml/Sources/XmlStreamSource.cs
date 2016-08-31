using System;
using System.IO;
using System.Text;
using System.Xml;

namespace OsmSharp.IO.Xml.Sources
{
  public class XmlStreamSource : IXmlSource
  {
    private Stream _stream;

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool HasData
    {
      get
      {
        return this._stream.Length > 1L;
      }
    }

    public XmlStreamSource(Stream stream)
    {
      this._stream = stream;
    }

    public XmlReader GetReader()
    {
      this._stream.Seek(0L, SeekOrigin.Begin);
      return XmlReader.Create(this._stream);
    }

    public XmlWriter GetWriter()
    {
      XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
      {
        CheckCharacters = true,
        CloseOutput = true,
        ConformanceLevel = ConformanceLevel.Document,
        Encoding = Encoding.UTF8,
        Indent = true,
        NewLineChars = Environment.NewLine,
        NewLineHandling = NewLineHandling.Entitize,
        OmitXmlDeclaration = true
      };
      this._stream.SetLength(0L);
      return XmlWriter.Create(this._stream);
    }

    public void Close()
    {
      this._stream = (Stream) null;
    }
  }
}
