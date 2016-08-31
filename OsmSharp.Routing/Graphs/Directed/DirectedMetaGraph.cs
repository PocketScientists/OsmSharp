using Reminiscence.Arrays;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Graphs.Directed
{
  public class DirectedMetaGraph
  {
    private readonly int _edgeDataSize = int.MaxValue;
    private readonly DirectedGraph _graph;
    private readonly ArrayBase<uint> _edgeData;
    private const int BLOCK_SIZE = 1000;

    public DirectedGraph Graph
    {
      get
      {
        return this._graph;
      }
    }

    public long VertexCount
    {
      get
      {
        return (long) this._graph.VertexCount;
      }
    }

    public long EdgeCount
    {
      get
      {
        return (long) this._graph.EdgeCount;
      }
    }

    public DirectedMetaGraph(int edgeDataSize, int edgeMetaDataSize)
      : this(edgeDataSize, edgeMetaDataSize, 1000L)
    {
    }

    public DirectedMetaGraph(int edgeDataSize, int edgeMetaDataSize, long sizeEstimate)
    {
      this._edgeDataSize = edgeMetaDataSize;
      this._graph = new DirectedGraph(edgeDataSize, sizeEstimate, (Action<uint, uint>) ((x, y) => this.SwitchEdge(x, y)));
      this._edgeData = (ArrayBase<uint>) new MemoryArray<uint>((long) this._edgeDataSize * (long) this._graph.EdgeCount);
    }

    private DirectedMetaGraph(DirectedGraph graph, int edgeDataSize, ArrayBase<uint> edgeData)
    {
      this._graph = graph;
      this._edgeData = edgeData;
      this._edgeDataSize = edgeDataSize;
    }

    private void SwitchEdge(uint oldId, uint newId)
    {
      long num1 = (long) oldId * (long) this._edgeDataSize;
      long num2 = (long) newId * (long) this._edgeDataSize;
      if (num2 + (long) this._edgeDataSize > this._edgeData.Length)
        this.IncreaseSizeEdgeData((uint) ((ulong) num2 + (ulong) this._edgeDataSize));
      for (int index = 0; index < this._edgeDataSize; ++index)
        this._edgeData[num2 + (long) index] = this._edgeData[num1 + (long) index];
    }

    private void IncreaseSizeEdgeData(uint edgePointer)
    {
      long length = this._edgeData.Length;
      while ((long) edgePointer >= length)
        length += 1000L;
      this._edgeData.Resize(length);
    }

    public uint AddEdge(uint vertex1, uint vertex2, uint data, uint metaData)
    {
      if (this._edgeDataSize != 1)
        throw new ArgumentOutOfRangeException("Dimension of meta-data doesn't match.");
      uint edgePointer = this._graph.AddEdge(vertex1, vertex2, data);
      if ((long) edgePointer >= this._edgeData.Length)
        this.IncreaseSizeEdgeData(edgePointer);
      this._edgeData[(long) edgePointer * (long) this._edgeDataSize + 0L] = metaData;
      return edgePointer;
    }

    public uint AddEdge(uint vertex1, uint vertex2, uint[] data, params uint[] metaData)
    {
      uint edgePointer = this._graph.AddEdge(vertex1, vertex2, data);
      if ((long) edgePointer >= this._edgeData.Length)
        this.IncreaseSizeEdgeData(edgePointer);
      long num = (long) edgePointer * (long) this._edgeDataSize;
      for (int index = 0; index < this._edgeDataSize; ++index)
        this._edgeData[num + (long) index] = metaData[index];
      return edgePointer;
    }

    public uint UpdateEdge(uint vertex1, uint vertex2, Func<uint[], bool> update, uint[] data, params uint[] metaData)
    {
      uint num1 = this._graph.UpdateEdge(vertex1, vertex2, update, data);
      if ((int) num1 == -1)
        return uint.MaxValue;
      long num2 = (long) num1 * (long) this._edgeDataSize;
      for (int index = 0; index < this._edgeDataSize; ++index)
        this._edgeData[num2 + (long) index] = metaData[index];
      return num1;
    }

    public int RemoveEdges(uint vertex)
    {
      return this._graph.RemoveEdges(vertex);
    }

    public int RemoveEdge(uint vertex1, uint vertex2)
    {
      return this._graph.RemoveEdge(vertex1, vertex2);
    }

    public DirectedMetaGraph.EdgeEnumerator GetEdgeEnumerator()
    {
      return new DirectedMetaGraph.EdgeEnumerator(this, this._graph.GetEdgeEnumerator());
    }

    public DirectedMetaGraph.EdgeEnumerator GetEdgeEnumerator(uint vertex)
    {
      return new DirectedMetaGraph.EdgeEnumerator(this, this._graph.GetEdgeEnumerator(vertex));
    }

    public void Compress()
    {
      this.Compress(false);
    }

    public void Compress(bool toReadonly)
    {
      long maxEdgeId;
      this._graph.Compress(toReadonly, out maxEdgeId);
      this._edgeData.Resize(maxEdgeId);
    }

    public void Trim()
    {
      long maxEdgeId;
      this._graph.Trim(out maxEdgeId);
      this._edgeData.Resize(maxEdgeId);
    }

    public void Dispose()
    {
      this._graph.Dispose();
      this._edgeData.Dispose();
    }

    public long Serialize(Stream stream)
    {
      return this.Serialize(stream, false);
    }

    public long Serialize(Stream stream, bool toReadonly)
    {
      this.Compress(toReadonly);
      long num1 = 1;
      stream.WriteByte((byte) 1);
      long num2 = this._graph.Serialize(stream, false);
      long num3 = num1 + num2;
      stream.Write(BitConverter.GetBytes(0), 0, 4);
      long num4 = 4;
      long num5 = num3 + num4;
      stream.Write(BitConverter.GetBytes(this._edgeDataSize), 0, 4);
      long num6 = 4;
      return num5 + num6 + this._edgeData.CopyTo(stream);
    }

    public static DirectedMetaGraph Deserialize(Stream stream, DirectedMetaGraphProfile profile)
    {
      int num1 = stream.ReadByte();
      if (num1 != 1)
        throw new Exception(string.Format("Cannot deserialize directed meta graph: Invalid version #: {0}.", (object) num1));
      DirectedGraph graph = DirectedGraph.Deserialize(stream, profile == null ? (DirectedGraphProfile) null : profile.DirectedGraphProfile);
      long position1 = stream.Position;
      long num2 = 0;
      byte[] buffer = new byte[4];
      stream.Read(buffer, 0, 4);
      long num3 = num2 + 4L;
      BitConverter.ToInt32(buffer, 0);
      stream.Read(buffer, 0, 4);
      long num4 = num3 + 4L;
      int int32 = BitConverter.ToInt32(buffer, 0);
      uint edgeCount = graph.EdgeCount;
      ArrayBase<uint> arrayBase;
      long num5;
      if (profile == null)
      {
        arrayBase = (ArrayBase<uint>) new MemoryArray<uint>((long) edgeCount * (long) int32);
        arrayBase.CopyFrom(stream);
        num5 = num4 + (long) edgeCount * (long) int32 * 4L;
      }
      else
      {
        long position2 = stream.Position;
        arrayBase = (ArrayBase<uint>) new Array<uint>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position2, (long) edgeCount * (long) int32 * 4L))).CreateUInt32((long) edgeCount * (long) int32), profile.EdgeMetaProfile);
        num5 = num4 + (long) edgeCount * (long) int32 * 4L;
      }
      stream.Seek(position1 + num5, SeekOrigin.Begin);
      int edgeDataSize = int32;
      ArrayBase<uint> edgeData = arrayBase;
      return new DirectedMetaGraph(graph, edgeDataSize, edgeData);
    }

    public class EdgeEnumerator : IEnumerable<MetaEdge>, IEnumerable, IEnumerator<MetaEdge>, IEnumerator, IDisposable
    {
      private readonly DirectedMetaGraph _graph;
      private readonly DirectedGraph.EdgeEnumerator _enumerator;

      public uint Neighbour
      {
        get
        {
          return this._enumerator.Neighbour;
        }
      }

      public uint[] Data
      {
        get
        {
          return this._enumerator.Data;
        }
      }

      public uint Data0
      {
        get
        {
          return this._enumerator.Data0;
        }
      }

      public uint[] MetaData
      {
        get
        {
          uint[] numArray = new uint[this._graph._edgeDataSize];
          long num = (long) this._enumerator.Id * (long) this._graph._edgeDataSize;
          for (int index = 0; index < this._graph._edgeDataSize; ++index)
            numArray[index] = this._graph._edgeData[num + (long) index];
          return numArray;
        }
      }

      public uint MetaData0
      {
        get
        {
          return this._graph._edgeData[(long) this._enumerator.Id * (long) this._graph._edgeDataSize + 0L];
        }
      }

      public uint Id
      {
        get
        {
          return this._enumerator.Id;
        }
      }

      public MetaEdge Current
      {
        get
        {
          return new MetaEdge(this);
        }
      }

      public int Count
      {
        get
        {
          return this._enumerator.Count;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public EdgeEnumerator(DirectedMetaGraph graph, DirectedGraph.EdgeEnumerator enumerator)
      {
        this._graph = graph;
        this._enumerator = enumerator;
      }

      public bool MoveNext()
      {
        return this._enumerator.MoveNext();
      }

      public void Reset()
      {
        this._enumerator.Reset();
      }

      public IEnumerator<MetaEdge> GetEnumerator()
      {
        this.Reset();
        return (IEnumerator<MetaEdge>) this;
      }

      public void Dispose()
      {
      }

      public bool MoveTo(uint vertex)
      {
        return this._enumerator.MoveTo(vertex);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }
    }
  }
}
