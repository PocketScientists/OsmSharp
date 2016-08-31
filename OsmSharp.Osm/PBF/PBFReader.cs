using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;

namespace OsmSharp.Osm.PBF
{
  public class PBFReader
  {
    private readonly Type _blockHeaderType = typeof (BlobHeader);
    private readonly Type _blobType = typeof (Blob);
    private readonly Type _primitiveBlockType = typeof (PrimitiveBlock);
    private readonly Type _headerBlockType = typeof (HeaderBlock);
    private PrimitiveBlock _block = new PrimitiveBlock();
    private readonly Stream _stream;
    private readonly RuntimeTypeModel _runtimeTypeModel;

    public PBFReader(Stream stream)
    {
      this._stream = stream;
      this._runtimeTypeModel = TypeModel.Create();
      this._runtimeTypeModel.Add(this._blockHeaderType, true);
      this._runtimeTypeModel.Add(this._blobType, true);
      this._runtimeTypeModel.Add(this._primitiveBlockType, true);
      this._runtimeTypeModel.Add(this._headerBlockType, true);
    }

    public void Dispose()
    {
      this._stream.Dispose();
    }

    public PrimitiveBlock MoveNext()
    {
      if (this._block.primitivegroup != null)
        this._block.primitivegroup.Clear();
      if (this._block.stringtable != null)
        this._block.stringtable.s.Clear();
      PrimitiveBlock primitiveBlock = (PrimitiveBlock) null;
      bool flag = true;
      while (flag)
      {
        flag = false;
        int num;
        if (Serializer.TryReadLengthPrefix(this._stream, (PrefixStyle) 3, out num))
        {
          BlobHeader blobHeader;
          using (LimitedStream limitedStream = new LimitedStream(this._stream, (long) num))
            blobHeader = ((TypeModel) this._runtimeTypeModel).Deserialize((Stream) limitedStream, (object) null, this._blockHeaderType) as BlobHeader;
          Blob blob;
          using (LimitedStream limitedStream = new LimitedStream(this._stream, (long) blobHeader.datasize))
            blob = ((TypeModel) this._runtimeTypeModel).Deserialize((Stream) limitedStream, (object) null, this._blobType) as Blob;
          Stream stream = blob.zlib_data != null ? (Stream) new ZLibStreamWrapper((Stream) new MemoryStream(blob.zlib_data)) : (Stream) new MemoryStream(blob.raw);
          using (stream)
          {
            if (blobHeader.type == Encoder.OSMHeader)
            {
              ((TypeModel) this._runtimeTypeModel).Deserialize(stream, (object) null, this._headerBlockType);
              flag = true;
            }
            if (blobHeader.type == Encoder.OSMData)
              primitiveBlock = ((TypeModel) this._runtimeTypeModel).Deserialize(stream, (object) this._block, this._primitiveBlockType) as PrimitiveBlock;
          }
        }
      }
      return primitiveBlock;
    }

    private static int IntLittleEndianToBigEndian(uint i)
    {
      return (((int) i & (int) byte.MaxValue) << 24) + (((int) i & 65280) << 8) + (int) ((i & 16711680U) >> 8) + ((int) (i >> 24) & (int) byte.MaxValue);
    }
  }
}
