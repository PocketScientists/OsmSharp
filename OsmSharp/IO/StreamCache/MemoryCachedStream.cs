using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.IO.StreamCache
{
  public class MemoryCachedStream : IStreamCache, IDisposable
  {
    private readonly HashSet<Stream> _streams = new HashSet<Stream>();

    public Stream CreateNew()
    {
      MemoryStream memoryStream = new MemoryStream();
      this._streams.Add((Stream) memoryStream);
      return (Stream) memoryStream;
    }

    public void Dispose(Stream stream)
    {
      this._streams.Remove(stream);
      stream.Dispose();
    }

    public void Dispose()
    {
      foreach (Stream stream in this._streams)
        stream.Dispose();
      this._streams.Clear();
    }
  }
}
