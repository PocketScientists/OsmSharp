using System.IO;

namespace OsmSharp.IO.Web
{
  public abstract class HttpWebResponse
  {
    public abstract HttpStatusCode StatusCode { get; }

    public abstract Stream GetResponseStream();

    public abstract void Close();
  }
}
