using System;

namespace OsmSharp.IO.Web
{
  public class WebException : Exception
  {
    public HttpWebResponse Response { get; private set; }

    public WebException(HttpWebResponse response)
    {
      this.Response = response;
    }
  }
}
