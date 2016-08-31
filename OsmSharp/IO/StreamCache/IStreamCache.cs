using System;
using System.IO;

namespace OsmSharp.IO.StreamCache
{
  public interface IStreamCache : IDisposable
  {
    Stream CreateNew();

    void Dispose(Stream stream);
  }
}
