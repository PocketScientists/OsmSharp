using System;
using System.IO;

namespace OsmSharp.Osm.PBF
{
  internal class LimitedStream : InputStream
  {
    private Stream stream;
    private long remaining;

    public LimitedStream(Stream stream, long length)
    {
      if (length < 0L)
        throw new ArgumentOutOfRangeException("length");
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanRead)
        throw new ArgumentException("stream");
      this.stream = stream;
      this.remaining = length;
    }

    protected override int ReadNextBlock(byte[] buffer, int offset, int count)
    {
      if ((long) count > this.remaining)
        count = (int) this.remaining;
      int num = this.stream.Read(buffer, offset, count);
      if (num > 0)
        this.remaining = this.remaining - (long) num;
      return num;
    }
  }
}
