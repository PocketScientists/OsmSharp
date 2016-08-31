using OsmSharp.Collections.Sorting;
using Reminiscence.Arrays;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Graphs.Directed
{
  public class DirectedGraph : IDisposable
  {
    private readonly int _edgeSize = -1;
    private readonly int _edgeDataSize = -1;
    private const int VERTEX_SIZE = 2;
    private const int FIRST_EDGE = 0;
    private const int EDGE_COUNT = 1;
    private const int MINIMUM_EDGE_SIZE = 1;
    private const uint NO_EDGE = 4294967295;
    private readonly Action<uint, uint> _switchEdge;
    private readonly ArrayBase<uint> _vertices;
    private readonly ArrayBase<uint> _edges;
    private uint _nextEdgePointer;
    private bool _readonly;
    private long _edgeCount;

    public uint VertexCount
    {
      get
      {
        return (uint) ((ulong) this._vertices.Length / 2UL);
      }
    }

    public uint EdgeCount
    {
      get
      {
        return (uint) this._edgeCount;
      }
    }

    public bool IsReadonly
    {
      get
      {
        return this._readonly;
      }
    }

    public DirectedGraph(int edgeDataSize)
      : this(edgeDataSize, 1000L)
    {
    }

    public DirectedGraph(int edgeDataSize, Action<uint, uint> switchEdge)
      : this(edgeDataSize, 1000L, switchEdge)
    {
    }

    public DirectedGraph(int edgeDataSize, long sizeEstimate)
      : this(edgeDataSize, sizeEstimate, (Action<uint, uint>) null)
    {
    }

    public DirectedGraph(int edgeDataSize, long sizeEstimate, Action<uint, uint> switchEdge)
      : this(edgeDataSize, sizeEstimate, (ArrayBase<uint>) new MemoryArray<uint>(sizeEstimate), (ArrayBase<uint>) new MemoryArray<uint>(sizeEstimate * 3L * (long) edgeDataSize + 1L), switchEdge)
    {
    }

    public DirectedGraph(MemoryMap file, int edgeDataSize, long sizeEstimate, Action<uint, uint> switchEdge)
    {
      this._vertices = (ArrayBase<uint>) new Array<uint>(file, sizeEstimate);
      for (int index = 0; (long) index < sizeEstimate; ++index)
        this._vertices[(long) index] = 0U;
      this._edges = (ArrayBase<uint>) new Array<uint>(file, sizeEstimate);
      this._edgeCount = 0L;
      this._edgeDataSize = edgeDataSize;
      this._switchEdge = switchEdge;
    }

    private DirectedGraph(int edgeDataSize, long edgeCount, ArrayBase<uint> vertices, ArrayBase<uint> edges)
    {
      this._edgeSize = edgeDataSize + 1;
      this._edgeDataSize = edgeDataSize;
      this._edgeCount = edgeCount;
      this._vertices = vertices;
      this._edges = edges;
      this._nextEdgePointer = (uint) ((ulong) edges.Length / (ulong) this._edgeSize);
    }

    private DirectedGraph(int edgeDataSize, long sizeEstimate, ArrayBase<uint> vertices, ArrayBase<uint> edges, Action<uint, uint> switchEdge)
    {
      this._edgeSize = edgeDataSize + 1;
      this._edgeDataSize = edgeDataSize;
      this._nextEdgePointer = 0U;
      this._edgeCount = 0L;
      this._vertices = vertices;
      this._vertices.Resize(sizeEstimate);
      for (int index = 0; (long) index < sizeEstimate; ++index)
        this._vertices[(long) index] = 0U;
      this._edges = edges;
      this._edges.Resize(sizeEstimate * 3L * (long) this._edgeSize);
      this._switchEdge = switchEdge;
    }

    private void IncreaseVertexSize()
    {
      this.IncreaseVertexSize(this._vertices.Length + 10000L);
    }

    private void IncreaseVertexSize(long size)
    {
      if (this._readonly)
        throw new Exception("Graph is readonly.");
      this._vertices.Resize(size);
    }

    private void IncreaseEdgeSize()
    {
      this.IncreaseEdgeSize(this._edges.Length + 10000L);
    }

    private void IncreaseEdgeSize(long size)
    {
      if (this._readonly)
        throw new Exception("Graph is readonly.");
      this._edges.Resize(size);
    }

    public uint AddEdge(uint vertex1, uint vertex2, uint data)
    {
      if (this._readonly)
        throw new Exception("Graph is readonly.");
      if ((int) vertex1 == (int) vertex2)
        throw new ArgumentException("Given vertices must be different.");
      if ((long) (vertex1 * 2U) > this._vertices.Length - 1L)
        this.IncreaseVertexSize();
      if ((long) (vertex2 * 2U) > this._vertices.Length - 1L)
        this.IncreaseVertexSize();
      if (this._edgeDataSize != 1)
        throw new ArgumentOutOfRangeException("Dimension of data doesn't match.");
      uint num1 = vertex1 * 2U;
      uint num2 = this._vertices[(long) (num1 + 1U)];
      uint num3 = this._vertices[(long) (num1 + 0U)] * (uint) this._edgeSize;
      uint num4;
      if ((int) num2 == 0)
      {
        this._vertices[(long) (num1 + 1U)] = 1U;
        this._vertices[(long) (num1 + 0U)] = this._nextEdgePointer / (uint) this._edgeSize;
        num4 = this._nextEdgePointer / (uint) this._edgeSize;
        if ((long) this._nextEdgePointer + (long) (1 * this._edgeSize) >= this._edges.Length)
          this.IncreaseEdgeSize();
        this._edges[(long) this._nextEdgePointer] = vertex2;
        this._edges[(long) (uint) ((int) this._nextEdgePointer + 1 + 0)] = data;
        this._nextEdgePointer = this._nextEdgePointer + (uint) this._edgeSize;
      }
      else if (((int) num2 & (int) num2 - 1) == 0)
      {
        if ((long) this._nextEdgePointer + (long) num2 * (long) this._edgeSize >= this._edges.Length)
          this.IncreaseEdgeSize();
        if ((long) num3 == (long) this._nextEdgePointer - (long) num2 * (long) this._edgeSize)
        {
          this._edges[(long) this._nextEdgePointer] = vertex2;
          num4 = this._nextEdgePointer / (uint) this._edgeSize;
          this._edges[(long) (uint) ((int) this._nextEdgePointer + 1 + 0)] = data;
          this._nextEdgePointer = this._nextEdgePointer + (uint) ((ulong) num2 * (ulong) this._edgeSize);
          this._vertices[(long) (num1 + 1U)] = num2 + 1U;
        }
        else
        {
          this._vertices[(long) (num1 + 0U)] = this._nextEdgePointer / (uint) this._edgeSize;
          this._vertices[(long) (num1 + 1U)] = num2 + 1U;
          uint num5 = this._nextEdgePointer + (uint) ((ulong) (num2 * 2U) * (ulong) this._edgeSize);
          if ((long) this._nextEdgePointer + (long) num2 * (long) this._edgeSize >= this._edges.Length)
            this.IncreaseEdgeSize();
          for (int index1 = 0; (long) index1 < (long) num2; ++index1)
          {
            this._edges[(long) this._nextEdgePointer] = this._edges[(long) num3 + (long) (index1 * this._edgeSize)];
            for (uint index2 = 0; (long) index2 < (long) this._edgeDataSize; ++index2)
              this._edges[(long) (this._nextEdgePointer + 1U + index2)] = this._edges[(long) (num3 + 1U + index2) + (long) (index1 * this._edgeSize)];
            if (this._switchEdge != null)
              this._switchEdge((uint) (((ulong) num3 + (ulong) (index1 * this._edgeSize)) / (ulong) this._edgeSize), this._nextEdgePointer / (uint) this._edgeSize);
            this._nextEdgePointer = this._nextEdgePointer + (uint) this._edgeSize;
            if ((long) this._nextEdgePointer >= this._edges.Length)
              this.IncreaseEdgeSize();
          }
          if ((long) this._nextEdgePointer + (long) num2 * (long) this._edgeSize >= this._edges.Length)
            this.IncreaseEdgeSize();
          this._edges[(long) this._nextEdgePointer] = vertex2;
          num4 = this._nextEdgePointer / (uint) this._edgeSize;
          this._edges[(long) (uint) ((int) this._nextEdgePointer + 1 + 0)] = data;
          this._nextEdgePointer = num5;
        }
      }
      else
      {
        this._edges[(long) (num3 + num2 * (uint) this._edgeSize)] = vertex2;
        num4 = (num3 + num2 * (uint) this._edgeSize) / (uint) this._edgeSize;
        this._edges[(long) (uint) ((int) num3 + (int) num2 * this._edgeSize + 1 + 0)] = data;
        this._vertices[(long) (num1 + 1U)] = num2 + 1U;
      }
      this._edgeCount = this._edgeCount + 1L;
      return num4;
    }

    public uint AddEdge(uint vertex1, uint vertex2, params uint[] data)
    {
      if (this._readonly)
        throw new Exception("Graph is readonly.");
      if ((int) vertex1 == (int) vertex2)
        throw new ArgumentException("Given vertices must be different.");
      if ((long) (vertex1 * 2U) > this._vertices.Length - 1L)
        this.IncreaseVertexSize();
      if ((long) (vertex2 * 2U) > this._vertices.Length - 1L)
        this.IncreaseVertexSize();
      uint num1 = vertex1 * 2U;
      uint num2 = this._vertices[(long) (num1 + 1U)];
      uint num3 = this._vertices[(long) (num1 + 0U)] * (uint) this._edgeSize;
      uint num4;
      if ((int) num2 == 0)
      {
        this._vertices[(long) (num1 + 1U)] = 1U;
        this._vertices[(long) (num1 + 0U)] = this._nextEdgePointer / (uint) this._edgeSize;
        num4 = this._nextEdgePointer / (uint) this._edgeSize;
        if ((long) this._nextEdgePointer + (long) (1 * this._edgeSize) >= this._edges.Length)
          this.IncreaseEdgeSize();
        this._edges[(long) this._nextEdgePointer] = vertex2;
        for (uint index = 0; (long) index < (long) this._edgeDataSize; ++index)
          this._edges[(long) (this._nextEdgePointer + 1U + index)] = data[(int) index];
        this._nextEdgePointer = this._nextEdgePointer + (uint) this._edgeSize;
      }
      else if (((int) num2 & (int) num2 - 1) == 0)
      {
        if ((long) this._nextEdgePointer + (long) num2 * (long) this._edgeSize >= this._edges.Length)
          this.IncreaseEdgeSize();
        if ((long) num3 == (long) this._nextEdgePointer - (long) num2 * (long) this._edgeSize)
        {
          this._edges[(long) this._nextEdgePointer] = vertex2;
          num4 = this._nextEdgePointer / (uint) this._edgeSize;
          for (uint index = 0; (long) index < (long) this._edgeDataSize; ++index)
            this._edges[(long) (this._nextEdgePointer + 1U + index)] = data[(int) index];
          this._nextEdgePointer = this._nextEdgePointer + (uint) ((ulong) num2 * (ulong) this._edgeSize);
          this._vertices[(long) (num1 + 1U)] = num2 + 1U;
        }
        else
        {
          this._vertices[(long) (num1 + 0U)] = this._nextEdgePointer / (uint) this._edgeSize;
          this._vertices[(long) (num1 + 1U)] = num2 + 1U;
          uint num5 = this._nextEdgePointer + (uint) ((ulong) (num2 * 2U) * (ulong) this._edgeSize);
          if ((long) this._nextEdgePointer + (long) num2 * (long) this._edgeSize >= this._edges.Length)
            this.IncreaseEdgeSize();
          for (int index1 = 0; (long) index1 < (long) num2; ++index1)
          {
            this._edges[(long) this._nextEdgePointer] = this._edges[(long) num3 + (long) (index1 * this._edgeSize)];
            for (uint index2 = 0; (long) index2 < (long) this._edgeDataSize; ++index2)
              this._edges[(long) (this._nextEdgePointer + 1U + index2)] = this._edges[(long) (num3 + 1U + index2) + (long) (index1 * this._edgeSize)];
            if (this._switchEdge != null)
              this._switchEdge((uint) (((ulong) num3 + (ulong) (index1 * this._edgeSize)) / (ulong) this._edgeSize), this._nextEdgePointer / (uint) this._edgeSize);
            this._nextEdgePointer = this._nextEdgePointer + (uint) this._edgeSize;
            if ((long) this._nextEdgePointer >= this._edges.Length)
              this.IncreaseEdgeSize();
          }
          if ((long) this._nextEdgePointer + (long) num2 * (long) this._edgeSize >= this._edges.Length)
            this.IncreaseEdgeSize();
          this._edges[(long) this._nextEdgePointer] = vertex2;
          num4 = this._nextEdgePointer / (uint) this._edgeSize;
          for (uint index = 0; (long) index < (long) this._edgeDataSize; ++index)
            this._edges[(long) (this._nextEdgePointer + 1U + index)] = data[(int) index];
          this._nextEdgePointer = num5;
        }
      }
      else
      {
        this._edges[(long) (num3 + num2 * (uint) this._edgeSize)] = vertex2;
        num4 = (num3 + num2 * (uint) this._edgeSize) / (uint) this._edgeSize;
        for (uint index = 0; (long) index < (long) this._edgeDataSize; ++index)
          this._edges[(long) ((uint) ((int) num3 + (int) num2 * this._edgeSize + 1) + index)] = data[(int) index];
        this._vertices[(long) (num1 + 1U)] = num2 + 1U;
      }
      this._edgeCount = this._edgeCount + 1L;
      return num4;
    }

    public uint UpdateEdge(uint vertex1, uint vertex2, Func<uint[], bool> update, params uint[] data)
    {
      if (this._readonly)
        throw new Exception("Graph is readonly.");
      if ((int) vertex1 == (int) vertex2)
        throw new ArgumentException("Given vertices must be different.");
      if ((long) (vertex1 * 2U) > this._vertices.Length - 1L)
        this.IncreaseVertexSize();
      if ((long) (vertex2 * 2U) > this._vertices.Length - 1L)
        this.IncreaseVertexSize();
      uint num1 = vertex1 * 2U;
      uint num2 = this._vertices[(long) (num1 + 1U)];
      uint num3 = this._vertices[(long) (num1 + 0U)] * (uint) this._edgeSize;
      uint num4 = num3 + num2 * (uint) this._edgeSize;
      uint[] numArray = new uint[this._edgeDataSize];
      while (num3 < num4)
      {
        for (uint index = 0; (long) index < (long) this._edgeDataSize; ++index)
          numArray[(int) index] = this._edges[(long) (num3 + 1U + index)];
        if ((int) this._edges[(long) num3] == (int) vertex2 && update(numArray))
        {
          for (uint index = 0; (long) index < (long) this._edgeDataSize; ++index)
            this._edges[(long) (num3 + 1U + index)] = data[(int) index];
          return num3 / (uint) this._edgeSize;
        }
        num3 += (uint) this._edgeSize;
      }
      return uint.MaxValue;
    }

    public int RemoveEdges(uint vertex)
    {
      int num = 0;
      DirectedGraph.EdgeEnumerator edgeEnumerator = this.GetEdgeEnumerator(vertex);
      while (edgeEnumerator.MoveNext())
        num += this.RemoveEdge(vertex, edgeEnumerator.Neighbour);
      return num;
    }

    public int RemoveEdge(uint vertex1, uint vertex2)
    {
      if (this._readonly)
        throw new Exception("Graph is readonly.");
      if ((long) vertex1 >= this._vertices.Length || (long) vertex2 >= this._vertices.Length)
        return 0;
      int num1 = 0;
      uint num2 = vertex1 * 2U;
      uint num3 = this._vertices[(long) (num2 + 1U)];
      uint num4 = this._vertices[(long) (num2 + 0U)] * (uint) this._edgeSize;
      uint num5 = num4;
      while (num5 < num4 + num3 * (uint) this._edgeSize)
      {
        if ((int) this._edges[(long) num5] == (int) vertex2)
        {
          ++num1;
          this._edgeCount = this._edgeCount - 1L;
          --num3;
          if ((int) num5 != (int) num4 + (int) num3 * this._edgeSize)
          {
            this._edges[(long) num5] = this._edges[(long) (num4 + num3 * (uint) this._edgeSize)];
            for (int index = 0; index < this._edgeDataSize; ++index)
              this._edges[(long) (num5 + 1U) + (long) index] = this._edges[(long) (uint) ((int) num4 + (int) num3 * this._edgeSize + 1) + (long) index];
            if (this._switchEdge != null)
              this._switchEdge((uint) ((ulong) (num4 + num3 * (uint) this._edgeSize) / (ulong) this._edgeSize), (uint) ((ulong) num5 / (ulong) this._edgeSize));
            num5 -= (uint) this._edgeSize;
          }
          else
            break;
        }
        num5 += (uint) this._edgeSize;
      }
      this._vertices[(long) (num2 + 1U)] = num3;
      return num1;
    }

    public DirectedGraph.EdgeEnumerator GetEdgeEnumerator()
    {
      return new DirectedGraph.EdgeEnumerator(this);
    }

    public DirectedGraph.EdgeEnumerator GetEdgeEnumerator(uint vertex)
    {
      DirectedGraph.EdgeEnumerator edgeEnumerator = new DirectedGraph.EdgeEnumerator(this);
      int num = (int) vertex;
      edgeEnumerator.MoveTo((uint) num);
      return edgeEnumerator;
    }

    public void Trim()
    {
      long maxEdgeId;
      this.Trim(out maxEdgeId);
    }

    public void Trim(out long maxEdgeId)
    {
      uint num1 = 0;
      for (uint index = 0; (long) index < this._vertices.Length / 2L; ++index)
      {
        long num2 = (long) this._vertices[(long) (uint) ((int) index * 2 + 0)] * (long) this._edgeSize;
        uint num3 = this._vertices[(long) (uint) ((int) index * 2 + 1)];
        long num4 = num2;
        while (num4 < num2 + (long) num3 * (long) this._edgeSize)
        {
          uint num5 = this._edges[num4];
          if (num1 < num5)
            num1 = num5;
          num4 += (long) this._edgeSize;
        }
        if (num3 > 0U && num1 < index)
          num1 = index;
      }
      this._vertices.Resize((long) (uint) (((int) num1 + 1) * 2));
      uint num6 = this._nextEdgePointer;
      if ((int) num6 == 0)
        num6 = (uint) this._edgeSize;
      this._edges.Resize((long) num6);
      maxEdgeId = this._edges.Length / (long) (uint) this._edgeSize;
    }

    public void Compress(bool toReadonly)
    {
      long maxEdgeId;
      this.Compress(toReadonly, out maxEdgeId);
    }

    public void Compress(bool toReadonly, out long maxEdgeId)
    {
      this.Trim();
      MemoryArray<uint> sortedVertices = new MemoryArray<uint>(this._vertices.Length / 2L);
      for (uint index = 0; (long) index < ((ArrayBase<uint>) sortedVertices).Length; ++index)
        ((ArrayBase<uint>) sortedVertices)[(long) index] = index;
      QuickSort.Sort((Func<long, long>) (i => (long) this._vertices[(long) (((ArrayBase<uint>) sortedVertices)[i] * 2U)] * ((ArrayBase<uint>) sortedVertices).Length + (long) ((ArrayBase<uint>) sortedVertices)[i]), (Action<long, long>) ((i, j) =>
      {
        uint num = ((ArrayBase<uint>) sortedVertices)[i];
        ((ArrayBase<uint>) sortedVertices)[i] = ((ArrayBase<uint>) sortedVertices)[j];
        ((ArrayBase<uint>) sortedVertices)[j] = num;
      }), 0L, (long) (this.VertexCount - 1U));
      uint num1 = 0;
      for (uint index1 = 0; (long) index1 < ((ArrayBase<uint>) sortedVertices).Length; ++index1)
      {
        uint num2 = ((ArrayBase<uint>) sortedVertices)[(long) index1] * 2U;
        uint num3 = this._vertices[(long) (num2 + 1U)];
        uint num4 = this._vertices[(long) (num2 + 0U)] * (uint) this._edgeSize;
        this._vertices[(long) (num2 + 0U)] = num1 / (uint) this._edgeSize;
        uint num5 = 0;
        while (num5 < num3 * (uint) this._edgeSize)
        {
          if ((int) num1 != (int) num4)
          {
            this._edges[(long) (num1 + num5)] = this._edges[(long) (num4 + num5)];
            for (int index2 = 0; index2 < this._edgeDataSize; ++index2)
              this._edges[(long) (uint) ((int) num1 + (int) num5 + 1) + (long) index2] = this._edges[(long) (uint) ((int) num4 + (int) num5 + 1) + (long) index2];
            if (this._switchEdge != null)
              this._switchEdge((uint) ((ulong) (num4 + num5) / (ulong) this._edgeSize), (uint) ((ulong) (num1 + num5) / (ulong) this._edgeSize));
          }
          num5 += (uint) this._edgeSize;
        }
        if (!toReadonly && num3 > 2U && ((int) num3 & (int) num3 - 1) != 0)
        {
          uint num6 = num3 | num3 >> 1;
          uint num7 = num6 | num6 >> 2;
          uint num8 = num7 | num7 >> 4;
          uint num9 = num8 | num8 >> 8;
          num3 = (num9 | num9 >> 16) + 1U;
        }
        num1 += num3 * (uint) this._edgeSize;
      }
      this._nextEdgePointer = num1;
      this._readonly = toReadonly;
      this._edges.Resize((long) this._nextEdgePointer);
      maxEdgeId = this._edges.Length / (long) (uint) this._edgeSize;
    }

    public void Compress()
    {
      this.Compress(false);
    }

    public long Serialize(Stream stream)
    {
      return this.Serialize(stream, true);
    }

    public long Serialize(Stream stream, bool compress)
    {
      if (compress)
        this.Compress();
      long vertexCount = (long) this.VertexCount;
      long num1 = (long) this._nextEdgePointer / (long) this._edgeSize;
      long num2 = 1;
      stream.WriteByte((byte) 1);
      stream.Write(BitConverter.GetBytes(vertexCount), 0, 8);
      long num3 = 8;
      long num4 = num2 + num3;
      stream.Write(BitConverter.GetBytes(num1), 0, 8);
      long num5 = 8;
      long num6 = num4 + num5;
      stream.Write(BitConverter.GetBytes(2), 0, 4);
      long num7 = 4;
      long num8 = num6 + num7;
      stream.Write(BitConverter.GetBytes(this._edgeSize), 0, 4);
      long num9 = 4;
      return num8 + num9 + this._vertices.CopyTo(stream) + this._edges.CopyTo(stream);
    }

    public static DirectedGraph Deserialize(Stream stream, DirectedGraphProfile profile)
    {
      long position1 = stream.Position;
      long num1 = 1;
      int num2 = stream.ReadByte();
      if (num2 != 1)
        throw new Exception(string.Format("Cannot deserialize directed graph: Invalid version #: {0}.", (object) num2));
      byte[] buffer = new byte[8];
      stream.Read(buffer, 0, 8);
      long num3 = num1 + 8L;
      long int64_1 = BitConverter.ToInt64(buffer, 0);
      stream.Read(buffer, 0, 8);
      long num4 = num3 + 8L;
      long int64_2 = BitConverter.ToInt64(buffer, 0);
      stream.Read(buffer, 0, 4);
      long num5 = num4 + 4L;
      int int32_1 = BitConverter.ToInt32(buffer, 0);
      stream.Read(buffer, 0, 4);
      long num6 = num5 + 4L;
      int int32_2 = BitConverter.ToInt32(buffer, 0);
      ArrayBase<uint> vertices;
      ArrayBase<uint> edges;
      long num7;
      if (profile == null)
      {
        vertices = (ArrayBase<uint>) new MemoryArray<uint>(int64_1 * (long) int32_1);
        vertices.CopyFrom(stream);
        long num8 = num6 + int64_1 * (long) int32_1 * 4L;
        edges = (ArrayBase<uint>) new MemoryArray<uint>(int64_2 * (long) int32_2);
        edges.CopyFrom(stream);
        num7 = num8 + int64_2 * (long) int32_2 * 4L;
      }
      else
      {
        long position2 = stream.Position;
        vertices = (ArrayBase<uint>) new Array<uint>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position2, int64_1 * (long) int32_1 * 4L))).CreateUInt32(int64_1 * (long) int32_1), profile.VertexProfile);
        long num8 = num6 + int64_1 * (long) int32_1 * 4L;
        edges = (ArrayBase<uint>) new Array<uint>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position2 + int64_1 * (long) int32_1 * 4L, int64_2 * (long) int32_2 * 4L))).CreateUInt32(int64_2 * (long) int32_2), profile.EdgeProfile);
        num7 = num8 + int64_2 * (long) int32_2 * 4L;
      }
      stream.Seek(position1 + num7, SeekOrigin.Begin);
      return new DirectedGraph(int32_2 - 1, int64_2, vertices, edges);
    }

    public void Dispose()
    {
      this._edges.Dispose();
      this._vertices.Dispose();
    }

    public class EdgeEnumerator : IEnumerable<Edge>, IEnumerable, IEnumerator<Edge>, IEnumerator, IDisposable
    {
      private readonly DirectedGraph _graph;
      private uint _currentEdgePointer;
      private int _currentCount;
      private uint _startEdgeId;
      private uint _count;
      private uint _neighbour;

      public uint Neighbour
      {
        get
        {
          return this._graph._edges[(long) this._currentEdgePointer];
        }
      }

      public uint[] Data
      {
        get
        {
          uint[] numArray = new uint[this._graph._edgeDataSize];
          for (int index = 0; index < this._graph._edgeDataSize; ++index)
            numArray[index] = this._graph._edges[(long) (this._currentEdgePointer + 1U) + (long) index];
          return numArray;
        }
      }

      public uint Data0
      {
        get
        {
          return this._graph._edges[(long) (uint) ((int) this._currentEdgePointer + 1 + 0)];
        }
      }

      public uint Data1
      {
        get
        {
          return this._graph._edges[(long) (uint) ((int) this._currentEdgePointer + 1 + 1)];
        }
      }

      public uint Id
      {
        get
        {
          return this._currentEdgePointer / (uint) this._graph._edgeSize;
        }
      }

      public Edge Current
      {
        get
        {
          return new Edge(this);
        }
      }

      public int Count
      {
        get
        {
          return (int) this._count;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public EdgeEnumerator(DirectedGraph graph)
      {
        this._graph = graph;
        this._startEdgeId = 0U;
        this._count = 0U;
        this._neighbour = 0U;
        this._currentEdgePointer = uint.MaxValue;
        this._currentCount = -1;
      }

      public bool MoveNext()
      {
        if (this._currentCount < 0)
        {
          this._currentEdgePointer = this._startEdgeId;
          this._currentCount = 0;
        }
        else
        {
          this._currentEdgePointer = this._currentEdgePointer + (uint) this._graph._edgeSize;
          this._currentCount = this._currentCount + 1;
        }
        if ((long) this._currentCount >= (long) this._count)
          return false;
        while ((int) this._neighbour != 0 && (int) this._neighbour != (int) this.Neighbour)
        {
          this._currentEdgePointer = this._currentEdgePointer + (uint) this._graph._edgeSize;
          this._currentCount = this._currentCount + 1;
          if ((long) this._currentCount >= (long) this._count)
            return false;
        }
        return true;
      }

      public void Reset()
      {
        this._currentEdgePointer = uint.MaxValue;
        this._currentCount = -1;
      }

      public IEnumerator<Edge> GetEnumerator()
      {
        this.Reset();
        return (IEnumerator<Edge>) this;
      }

      public void Dispose()
      {
      }

      public bool MoveTo(uint vertex)
      {
        uint num = vertex * 2U;
        this._startEdgeId = this._graph._vertices[(long) (num + 0U)] * (uint) this._graph._edgeSize;
        this._count = this._graph._vertices[(long) (num + 1U)];
        this._neighbour = 0U;
        this._currentEdgePointer = uint.MaxValue;
        this._currentCount = -1;
        return this._count > 0U;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }
    }
  }
}
