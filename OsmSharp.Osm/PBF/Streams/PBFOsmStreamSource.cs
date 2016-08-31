using OsmSharp.Osm.Streams;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Osm.PBF.Streams
{
  public class PBFOsmStreamSource : OsmStreamSource, IPBFOsmPrimitiveConsumer
  {
    private readonly Stream _stream;
    private OsmGeo _current;
    private PBFReader _reader;
    private Queue<KeyValuePair<PrimitiveBlock, object>> _cachedPrimitives;

    public override bool CanReset
    {
      get
      {
        return this._stream.CanSeek;
      }
    }

    public PBFOsmStreamSource(Stream stream)
    {
      this._stream = stream;
    }

    public override void Initialize()
    {
      this._stream.Seek(0L, SeekOrigin.Begin);
      this.InitializePBFReader();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      for (KeyValuePair<PrimitiveBlock, object> nextPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations); nextPrimitive.Value != null; nextPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations))
      {
        OsmSharp.Osm.PBF.Node pbfNode = nextPrimitive.Value as OsmSharp.Osm.PBF.Node;
        if (pbfNode != null && !ignoreNodes)
        {
          this._current = (OsmGeo) Encoder.DecodeNode(nextPrimitive.Key, pbfNode);
          return true;
        }
        OsmSharp.Osm.PBF.Way pbfWay = nextPrimitive.Value as OsmSharp.Osm.PBF.Way;
        if (pbfWay != null && !ignoreWays)
        {
          this._current = (OsmGeo) Encoder.DecodeWay(nextPrimitive.Key, pbfWay);
          return true;
        }
        OsmSharp.Osm.PBF.Relation pbfRelation = nextPrimitive.Value as OsmSharp.Osm.PBF.Relation;
        if (pbfRelation != null && !ignoreRelations)
        {
          this._current = (OsmGeo) Encoder.DecodeRelation(nextPrimitive.Key, pbfRelation);
          return true;
        }
      }
      return false;
    }

    public override OsmGeo Current()
    {
      return this._current;
    }

    public override void Reset()
    {
      this._current = (OsmGeo) null;
      if (this._cachedPrimitives != null)
        this._cachedPrimitives.Clear();
      this._stream.Seek(0L, SeekOrigin.Begin);
    }

    private void InitializePBFReader()
    {
      this._reader = new PBFReader(this._stream);
      this.InitializeBlockCache();
    }

    private KeyValuePair<PrimitiveBlock, object> MoveToNextPrimitive(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      KeyValuePair<PrimitiveBlock, object> keyValuePair = this.DeQueuePrimitive();
      if (keyValuePair.Value == null)
      {
        PrimitiveBlock block = this._reader.MoveNext();
        while (block != null && !block.Decode((IPBFOsmPrimitiveConsumer) this, ignoreNodes, ignoreWays, ignoreRelations))
          block = this._reader.MoveNext();
        keyValuePair = this.DeQueuePrimitive();
      }
      return keyValuePair;
    }

    private void InitializeBlockCache()
    {
      this._cachedPrimitives = new Queue<KeyValuePair<PrimitiveBlock, object>>();
    }

    private void QueuePrimitive(PrimitiveBlock block, object primitive)
    {
      this._cachedPrimitives.Enqueue(new KeyValuePair<PrimitiveBlock, object>(block, primitive));
    }

    private KeyValuePair<PrimitiveBlock, object> DeQueuePrimitive()
    {
      if (this._cachedPrimitives.Count > 0)
        return this._cachedPrimitives.Dequeue();
      return new KeyValuePair<PrimitiveBlock, object>();
    }

    void IPBFOsmPrimitiveConsumer.ProcessNode(PrimitiveBlock block, OsmSharp.Osm.PBF.Node node)
    {
      this.QueuePrimitive(block, (object) node);
    }

    void IPBFOsmPrimitiveConsumer.ProcessWay(PrimitiveBlock block, OsmSharp.Osm.PBF.Way way)
    {
      this.QueuePrimitive(block, (object) way);
    }

    void IPBFOsmPrimitiveConsumer.ProcessRelation(PrimitiveBlock block, OsmSharp.Osm.PBF.Relation relation)
    {
      this.QueuePrimitive(block, (object) relation);
    }
  }
}
