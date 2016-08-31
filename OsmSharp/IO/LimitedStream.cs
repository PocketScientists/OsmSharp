using System.IO;

namespace OsmSharp.IO
{
  public class LimitedStream : Stream
  {
    private readonly long _offset;
    private readonly Stream _stream;

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
        return this._stream.CanWrite;
      }
    }

    public override long Length
    {
      get
      {
        return this._stream.Length - this._offset;
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

    public LimitedStream(Stream stream)
    {
      this._stream = stream;
      this._offset = this._stream.Position;
    }

    public override void Flush()
    {
      this._stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this._stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (origin == SeekOrigin.Begin)
        return this._stream.Seek(offset + this._offset, origin);
      return this._stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this._stream.SetLength(value + this._offset);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._stream.Write(buffer, offset, count);
    }
  }
}
