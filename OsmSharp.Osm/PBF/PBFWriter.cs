using System;
using System.IO;

namespace OsmSharp.Osm.PBF
{
  internal class PBFWriter
  {
    private Stream _stream;

    public PBFWriter(Stream stream)
    {
      this._stream = stream;
    }

    public void Dispose()
    {
      this._stream.Dispose();
    }

    public void ReadAll(PrimitiveBlock block)
    {
      if (block == null)
        throw new ArgumentNullException("block");
    }
  }
}
