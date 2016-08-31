using Ionic.Zlib;
using System.IO;

namespace OsmSharp.Osm.PBF
{
  internal class ZLibStreamWrapper : InputStream
  {
    private ZlibStream reader;

    public ZLibStreamWrapper(Stream stream)
    {
      this.reader = new ZlibStream(stream, (CompressionMode) 1);
    }

    protected override int ReadNextBlock(byte[] buffer, int offset, int count)
    {
      return ((Stream) this.reader).Read(buffer, offset, count);
    }
  }
}
