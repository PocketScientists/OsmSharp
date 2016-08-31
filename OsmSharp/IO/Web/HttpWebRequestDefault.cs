using System;
using System.Net;

namespace OsmSharp.IO.Web
{
  internal class HttpWebRequestDefault : HttpWebRequest
  {
    private System.Net.HttpWebRequest _httpWebRequest;

    public override string Accept
    {
      get
      {
        return this._httpWebRequest.Accept;
      }
      set
      {
        this._httpWebRequest.Accept = value;
      }
    }

    public override bool IsUserAgentSupported
    {
      get
      {
        return false;
      }
    }

    public override string UserAgent
    {
      get
      {
        throw new NotSupportedException("Getting or setting the user-agent is not possible. Check IsUserAgentSupported.");
      }
      set
      {
        throw new NotSupportedException("Getting or setting the user-agent is not possible. Check IsUserAgentSupported.");
      }
    }

    public HttpWebRequestDefault(string url)
    {
      this._httpWebRequest = (System.Net.HttpWebRequest) WebRequest.Create(url);
    }

    public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
    {
      return this._httpWebRequest.BeginGetResponse(callback, state);
    }

    public override HttpWebResponse EndGetResponse(IAsyncResult iar)
    {
      return (HttpWebResponse) new HttpWebResponseDefault((System.Net.HttpWebResponse) this._httpWebRequest.EndGetResponse(iar));
    }

    public override void Abort()
    {
      this._httpWebRequest.Abort();
    }
  }
}
