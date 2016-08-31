using System;

namespace OsmSharp.IO.Web
{
  public abstract class HttpWebRequest
  {
    public static HttpWebRequest.CreateNativeWebRequestDelegate CreateNativeWebRequest;

    public abstract string Accept { get; set; }

    public abstract bool IsUserAgentSupported { get; }

    public abstract string UserAgent { get; set; }

    public static HttpWebRequest Create(string url)
    {
      if (HttpWebRequest.CreateNativeWebRequest != null)
        return HttpWebRequest.CreateNativeWebRequest(url);
      return (HttpWebRequest) new HttpWebRequestDefault(url);
    }

    public abstract IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

    public abstract HttpWebResponse EndGetResponse(IAsyncResult iar);

    public abstract void Abort();

    public delegate HttpWebRequest CreateNativeWebRequestDelegate(string url);
  }
}
