using System.IO;

namespace OsmSharp.IO.Web
{
  internal class HttpWebResponseDefault : HttpWebResponse
  {
    private System.Net.HttpWebResponse _httpWebResponse;

    public override HttpStatusCode StatusCode
    {
      get
      {
        switch (this._httpWebResponse.StatusCode)
        {
          case System.Net.HttpStatusCode.Forbidden:
            return HttpStatusCode.Forbidden;
          case System.Net.HttpStatusCode.NotFound:
            return HttpStatusCode.NotFound;
          default:
            return HttpStatusCode.Other;
        }
      }
    }

    public HttpWebResponseDefault(System.Net.HttpWebResponse httpWebResponse)
    {
      this._httpWebResponse = httpWebResponse;
    }

    public override Stream GetResponseStream()
    {
      return this._httpWebResponse.GetResponseStream();
    }

    public override void Close()
    {
    }
  }
}
