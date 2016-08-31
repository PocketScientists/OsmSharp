using OsmSharp.Logging;
using OsmSharp.Math.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v2
{
  internal class RTreeStreamIndex<T> : ISpatialIndexReadonly<T>
  {
    private readonly RTreeStreamSerializer<T> _serializer;
    private readonly SpatialIndexSerializerStream _stream;

    public RTreeStreamIndex(RTreeStreamSerializer<T> serializer, SpatialIndexSerializerStream stream)
    {
      this._serializer = serializer;
      this._stream = stream;
    }

    public IEnumerable<T> Get(BoxF2D box)
    {
      HashSet<T> result = new HashSet<T>();
      long ticks1 = DateTime.Now.Ticks;
      this._stream.Seek(0L, SeekOrigin.Begin);
      this._serializer.Search(this._stream, box, result);
      long ticks2 = DateTime.Now.Ticks;
      Log.TraceEvent("RTreeStreamIndex", TraceEventType.Verbose, string.Format("Deserialized {0} objects in {1}ms.", new object[2]
      {
        (object) result.Count,
        (object) new TimeSpan(ticks2 - ticks1).TotalMilliseconds
      }));
      return (IEnumerable<T>) result;
    }

    public void GetCancel()
    {
      this._serializer.SearchCancel();
    }
  }
}
