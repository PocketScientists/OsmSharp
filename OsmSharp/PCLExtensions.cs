using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp
{
  public static class PCLExtensions
  {
    public static string GetString(this Encoding encoding, byte[] bytes)
    {
      return encoding.GetString(bytes, 0, bytes.Length);
    }

    public static AssemblyName GetName(this Assembly assembly)
    {
      return new AssemblyName(assembly.FullName);
    }

    public static void ForEach<T>(this List<T> enumeration, Action<T> action)
    {
      foreach (T obj in enumeration)
        action(obj);
    }

    public static Stream GetRequestStream(this WebRequest request)
    {
      TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
      try
      {
        request.BeginGetRequestStream((AsyncCallback) (iar =>
        {
          try
          {
            tcs.SetResult(request.EndGetRequestStream(iar));
          }
          catch (Exception ex)
          {
            tcs.SetException(ex);
          }
        }), (object) null);
      }
      catch (Exception ex)
      {
        tcs.SetException(ex);
      }
      return tcs.Task.Result;
    }

    public static WebResponse GetResponse(this WebRequest request)
    {
      TaskCompletionSource<HttpWebResponse> tcs = new TaskCompletionSource<HttpWebResponse>();
      try
      {
        request.BeginGetResponse((AsyncCallback) (iar =>
        {
          try
          {
            tcs.SetResult((HttpWebResponse) request.EndGetResponse(iar));
          }
          catch (Exception ex)
          {
            tcs.SetException(ex);
          }
        }), (object) null);
      }
      catch (Exception ex)
      {
        tcs.SetException(ex);
      }
      try
      {
        return (WebResponse) tcs.Task.Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException is WebException)
          throw ex.InnerException;
        throw ex;
      }
    }
  }
}
