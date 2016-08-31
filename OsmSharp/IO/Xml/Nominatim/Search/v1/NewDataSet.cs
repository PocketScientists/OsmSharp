using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Search.v1
{
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  public class NewDataSet
  {
    private searchresults[] itemsField;

    [XmlElement("searchresults")]
    public searchresults[] Items
    {
      get
      {
        return this.itemsField;
      }
      set
      {
        this.itemsField = value;
      }
    }
  }
}
