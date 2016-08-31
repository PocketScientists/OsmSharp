using System;
using System.IO;

namespace OsmSharp.Osm.PBF
{
  internal abstract class InputStream : Stream
  {
    private long pos;

    public override long Position
    {
      get
      {
        return this.pos;
      }
      set
      {
        if (this.pos != value)
          throw new NotImplementedException();
      }
    }

    public override long Length
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }

    protected abstract int ReadNextBlock(byte[] buffer, int offset, int count);

    public override sealed int Read(byte[] buffer, int offset, int count)
    {
      int num1 = 0;
      int num2;
      while (count > 0 && (num2 = this.ReadNextBlock(buffer, offset, count)) > 0)
      {
        count -= num2;
        offset += num2;
        num1 += num2;
        this.pos = this.pos + (long) num2;
      }
      return num1;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }
  }
}
