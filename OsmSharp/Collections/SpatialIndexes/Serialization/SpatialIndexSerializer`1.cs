using System;
using System.IO;
using System.Text;

namespace OsmSharp.Collections.SpatialIndexes.Serialization
{
  public abstract class SpatialIndexSerializer<T>
  {
    public abstract string VersionString { get; }

    private byte[] BuildVersionHeader()
    {
      return Encoding.UTF8.GetBytes(this.VersionString);
    }

    private void WriteVersionHeader(Stream stream)
    {
      stream.Seek(0L, SeekOrigin.Begin);
      byte[] buffer = this.BuildVersionHeader();
      stream.Write(buffer, 0, buffer.Length);
    }

    private bool ReadAndValidateHeader(Stream stream)
    {
      byte[] numArray = this.BuildVersionHeader();
      try
      {
        byte[] buffer = new byte[numArray.Length];
        stream.Seek(0L, SeekOrigin.Begin);
        stream.Read(buffer, 0, numArray.Length);
        for (int index = 0; index < numArray.Length; ++index)
        {
          if ((int) numArray[index] != (int) buffer[index])
            return false;
        }
        return true;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public virtual bool CanDeSerialize(Stream stream)
    {
      int num = this.ReadAndValidateHeader(stream) ? 1 : 0;
      stream.Seek(0L, SeekOrigin.Begin);
      return num != 0;
    }

    public void Serialize(Stream stream, RTreeMemoryIndex<T> index)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (index == null)
        throw new ArgumentNullException("index");
      this.WriteVersionHeader(stream);
      this.DoSerialize(new SpatialIndexSerializerStream(stream), index);
    }

    protected abstract void DoSerialize(SpatialIndexSerializerStream stream, RTreeMemoryIndex<T> index);

    public ISpatialIndexReadonly<T> Deserialize(Stream stream, bool lazy = true)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!this.CanDeSerialize(stream))
        throw new ArgumentOutOfRangeException("stream", "Cannot deserialize the given stream, version unsupported or content unrecognized!");
      this.ReadAndValidateHeader(stream);
      return this.DoDeserialize(new SpatialIndexSerializerStream(stream), lazy);
    }

    protected abstract ISpatialIndexReadonly<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy);
  }
}
