using OsmSharp.Osm.Streams;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Osm.PBF.Streams
{
  public class PBFOsmStreamTarget : OsmStreamTarget
  {
    private readonly Type _blobHeaderType = typeof (BlobHeader);
    private readonly Type _blobType = typeof (Blob);
    private readonly Type _primitiveBlockType = typeof (PrimitiveBlock);
    private readonly Type _headerBlockType = typeof (HeaderBlock);
    private readonly Stream _stream;
    private readonly RuntimeTypeModel _runtimeTypeModel;
    private readonly bool _compress;
    private List<OsmGeo> _currentEntities;
    private Dictionary<string, int> _reverseStringTable;
    private MemoryStream _buffer;

    public PBFOsmStreamTarget(Stream stream)
    {
      this._stream = stream;
      this._currentEntities = new List<OsmGeo>();
      this._reverseStringTable = new Dictionary<string, int>();
      this._buffer = new MemoryStream();
      this._runtimeTypeModel = TypeModel.Create();
      this._runtimeTypeModel.Add(this._blobHeaderType, true);
      this._runtimeTypeModel.Add(this._blobType, true);
      this._runtimeTypeModel.Add(this._primitiveBlockType, true);
      this._runtimeTypeModel.Add(this._headerBlockType, true);
    }

    public override void Initialize()
    {
      this._currentEntities.Clear();
      this._buffer.Seek(0L, SeekOrigin.Begin);
      ((TypeModel) this._runtimeTypeModel).Serialize((Stream) this._buffer, (object) new HeaderBlock()
      {
        required_features = {
          "OsmSchema-V0.6",
          "DenseNodes"
        }
      });
      byte[] array = this._buffer.ToArray();
      this._buffer.SetLength(0L);
      ((TypeModel) this._runtimeTypeModel).Serialize((Stream) this._buffer, (object) new Blob() { raw = array });
      ((TypeModel) this._runtimeTypeModel).SerializeWithLengthPrefix(this._stream, (object) new BlobHeader()
      {
        datasize = (int) this._buffer.Length,
        indexdata = (byte[]) null,
        type = Encoder.OSMHeader
      }, this._blobHeaderType, (PrefixStyle) 3, 0);
      this._buffer.Seek(0L, SeekOrigin.Begin);
      this._buffer.CopyTo(this._stream);
    }

    private void FlushBlock()
    {
      if (this._currentEntities.Count == 0)
        return;
      PrimitiveBlock block = new PrimitiveBlock();
      block.Encode(this._reverseStringTable, this._currentEntities);
      this._currentEntities.Clear();
      this._reverseStringTable.Clear();
      this._buffer.SetLength(0L);
      ((TypeModel) this._runtimeTypeModel).Serialize((Stream) this._buffer, (object) block);
      byte[] array = this._buffer.ToArray();
      this._buffer.SetLength(0L);
      if (this._compress)
        throw new NotSupportedException();
      ((TypeModel) this._runtimeTypeModel).Serialize((Stream) this._buffer, (object) new Blob() { raw = array });
      ((TypeModel) this._runtimeTypeModel).SerializeWithLengthPrefix(this._stream, (object) new BlobHeader()
      {
        datasize = (int) this._buffer.Length,
        indexdata = (byte[]) null,
        type = Encoder.OSMData
      }, this._blobHeaderType, (PrefixStyle) 3, 0);
      this._buffer.Seek(0L, SeekOrigin.Begin);
      this._buffer.CopyTo(this._stream);
    }

    public override void AddNode(OsmSharp.Osm.Node node)
    {
      this._currentEntities.Add((OsmGeo) node);
      if (this._currentEntities.Count <= 8000)
        return;
      this.FlushBlock();
    }

    public override void AddWay(OsmSharp.Osm.Way way)
    {
      this._currentEntities.Add((OsmGeo) way);
      if (this._currentEntities.Count <= 8000)
        return;
      this.FlushBlock();
    }

    public override void AddRelation(OsmSharp.Osm.Relation relation)
    {
      this._currentEntities.Add((OsmGeo) relation);
      if (this._currentEntities.Count <= 8000)
        return;
      this.FlushBlock();
    }

    public override void Flush()
    {
      this.FlushBlock();
      this._stream.Flush();
    }

    public override void Close()
    {
      this.Flush();
    }
  }
}
