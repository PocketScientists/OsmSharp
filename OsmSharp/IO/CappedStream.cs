using System;
using System.IO;

namespace OsmSharp.IO
{
  public class CappedStream : Stream
  {
    private readonly Stream _stream;
    private readonly long _offset;
    private readonly long _length;

    public override bool CanRead
    {
      get
      {
        return this._stream.CanRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return this._stream.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override long Length
    {
      get
      {
        return this._length;
      }
    }

    public override long Position
    {
      get
      {
        return this._stream.Position - this._offset;
      }
      set
      {
        this._stream.Position = value + this._offset;
      }
    }

    public CappedStream(Stream stream, long offset, long length)
    {
      this._stream = stream;
      this._stream.Seek(offset, SeekOrigin.Begin);
      this._offset = offset;
      this._length = length;
    }

    public override void Flush()
    {
      this._stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.Position + (long) count < this._length)
        return this._stream.Read(buffer, offset, count);
      count = (int) (this._length - this.Position);
      return this._stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (offset > this._length)
        throw new Exception("Cannot read past end of capped stream.");
      return this._stream.Seek(offset + this._offset, origin);
    }

    public override void SetLength(long value)
    {
      this._stream.SetLength(value + this._offset);
    }

    public bool WriteBytePossible()
    {
      return this.Position + 1L <= this.Length;
    }

    public bool WritePossible(int offset, int count)
    {
      return this.Position + (long) count <= this.Length;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.Position + (long) count > this.Length)
        throw new Exception("Cannot write past end of capped stream.");
      this._stream.Write(buffer, offset, count);
    }
  }
}
