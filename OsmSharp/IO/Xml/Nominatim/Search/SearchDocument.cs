using OsmSharp.IO.Xml.Nominatim.Search.v1;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Search
{
  public class SearchDocument
  {
    private object _search_object;
    private IXmlSource _source;
    private SearchVersion _version;

    public bool IsReadOnly
    {
      get
      {
        return this._source.IsReadOnly;
      }
    }

    public SearchVersion Version
    {
      get
      {
        return this._version;
      }
    }

    public object Search
    {
      get
      {
        this.DoReadSearch();
        return this._search_object;
      }
      set
      {
        this._search_object = value;
        this.FindVersionFromObject();
      }
    }

    public SearchDocument(IXmlSource source)
    {
      this._source = source;
      this._version = SearchVersion.Unknown;
    }

    public void Save()
    {
      this.DoWriteSearch();
    }

    private void FindVersionFromObject()
    {
      this._version = SearchVersion.Unknown;
      if (!(this._search_object is searchresults))
        return;
      this._version = SearchVersion.Searchv1;
    }

    private void FindVersionFromSource()
    {
      XmlReader reader = this._source.GetReader();
      while (!reader.EOF)
      {
        if (reader.NodeType == XmlNodeType.Element && reader.Name == "searchresults")
          this._version = SearchVersion.Searchv1;
        else if (reader.NodeType == XmlNodeType.Element)
          throw new XmlException("First element expected: searchresults!");
        if (this._version != SearchVersion.Unknown)
          break;
        reader.Read();
      }
    }

    private void DoReadSearch()
    {
      if (this._search_object != null)
        return;
      Type type = (Type) null;
      this.FindVersionFromSource();
      switch (this._version)
      {
        case SearchVersion.Searchv1:
          type = typeof (searchresults);
          break;
        case SearchVersion.Unknown:
          throw new XmlException("Version could not be determined!");
      }
      XmlReader reader = this._source.GetReader();
      this._search_object = new XmlSerializer(type).Deserialize(reader);
    }

    private void DoWriteSearch()
    {
      if (this._search_object == null)
        return;
      Type type = (Type) null;
      switch (this._version)
      {
        case SearchVersion.Searchv1:
          type = typeof (searchresults);
          break;
        case SearchVersion.Unknown:
          throw new XmlException("Version could not be determined!");
      }
      XmlSerializer xmlSerializer = new XmlSerializer(type);
      XmlWriter writer = this._source.GetWriter();
      XmlWriter xmlWriter = writer;
      object searchObject = this._search_object;
      xmlSerializer.Serialize(xmlWriter, searchObject);
      writer.Flush();
    }

    public void Close()
    {
      this._search_object = (object) null;
      this._source = (IXmlSource) null;
    }
  }
}
