using OsmSharp.Collections.Sorting;
using Reminiscence.Arrays;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Graphs
{
  public class Graph : IDisposable
  {
    private readonly int _edgeSize = -1;
    private readonly int _edgeDataSize = -1;
    private const int MINIMUM_EDGE_SIZE = 4;
    private const int NODEA = 0;
    private const int NODEB = 1;
    private const int NEXTNODEA = 2;
    private const int NEXTNODEB = 3;
    private const int BLOCKSIZE = 1000;
    private readonly ArrayBase<uint> _vertices;
    private readonly ArrayBase<uint> _edges;
    private uint _nextEdgeId;
    private long _edgeCount;
    private uint? _maxVertex;

    public uint VertexCount
    {
      get
      {
        if (!this._maxVertex.HasValue)
          return 0;
        return this._maxVertex.Value + 1U;
      }
    }

    public long VertexCapacity
    {
      get
      {
        return this._vertices.Length;
      }
    }

    public long EdgeCount
    {
      get
      {
        return this._edgeCount;
      }
    }

    public long EdgeCapacity
    {
      get
      {
        return this._edges.Length / (long) this._edgeSize;
      }
    }

    public long SizeInBytes
    {
      get
      {
        return (long) (uint) (25 + (int) this.VertexCount * 4) + this.EdgeCount * 4L * 5L;
      }
    }

    public Graph(int edgeDataSize)
      : this(edgeDataSize, 1000L)
    {
    }

    public Graph(int edgeDataSize, long sizeEstimate)
      : this(edgeDataSize, sizeEstimate, (ArrayBase<uint>) new MemoryArray<uint>(sizeEstimate), (ArrayBase<uint>) new MemoryArray<uint>(sizeEstimate * 3L * (long) (4 + edgeDataSize)))
    {
    }

    private Graph(int edgeDataSize, ArrayBase<uint> vertices, ArrayBase<uint> edges)
    {
      this._edgeDataSize = edgeDataSize;
      this._edgeSize = 4 + edgeDataSize;
      this._vertices = vertices;
      this._maxVertex = new uint?();
      if (this._vertices.Length > 0L)
        this._maxVertex = new uint?((uint) ((ulong) vertices.Length - 1UL));
      this._edges = edges;
      this._nextEdgeId = (uint) edges.Length;
      this._edgeCount = (long) this._nextEdgeId / (long) this._edgeSize;
    }

    private Graph(int edgeDataSize, long sizeEstimate, ArrayBase<uint> vertices, ArrayBase<uint> edges)
    {
      this._edgeDataSize = edgeDataSize;
      this._edgeSize = 4 + edgeDataSize;
      this._nextEdgeId = 0U;
      this._vertices = vertices;
      this._vertices.Resize(sizeEstimate);
      for (int index = 0; (long) index < sizeEstimate; ++index)
        this._vertices[(long) index] = 4294967294U;
      this._edges = edges;
      this._edges.Resize(sizeEstimate * 3L * (long) this._edgeSize);
      for (int index = 0; (long) index < sizeEstimate * 3L * (long) this._edgeSize; ++index)
        this._edges[(long) index] = uint.MaxValue;
    }

    public Graph(MemoryMap map, int edgeDataSize, long estimatedSize)
    {
      this._edgeDataSize = edgeDataSize;
      this._edgeSize = 4 + edgeDataSize;
      this._vertices = (ArrayBase<uint>) new Array<uint>(map, estimatedSize);
      for (int index = 0; (long) index < this._vertices.Length; ++index)
        this._vertices[(long) index] = 4294967294U;
      this._edges = (ArrayBase<uint>) new Array<uint>(map, estimatedSize * 3L * (long) this._edgeSize);
      for (int index = 0; (long) index < this._edges.Length; ++index)
        this._edges[(long) index] = uint.MaxValue;
    }

    public Graph(MemoryMap map, GraphProfile profile, int edgeDataSize, long estimatedSize)
    {
      this._edgeDataSize = edgeDataSize;
      this._edgeSize = 4 + edgeDataSize;
      this._vertices = (ArrayBase<uint>) new Array<uint>(map, estimatedSize, profile.VertexProfile);
      for (int index = 0; (long) index < this._vertices.Length; ++index)
        this._vertices[(long) index] = 4294967294U;
      this._edges = (ArrayBase<uint>) new Array<uint>(map, estimatedSize * 3L * (long) this._edgeSize, profile.EdgeProfile);
      for (int index = 0; (long) index < this._edges.Length; ++index)
        this._edges[(long) index] = uint.MaxValue;
    }

    private void IncreaseVertexSize()
    {
      this.IncreaseVertexSize(this._vertices.Length + 1000L);
    }

    private void IncreaseVertexSize(long min)
    {
      long num = (long) (System.Math.Floor((double) (min / 1000L)) + 1.0) * 1000L;
      if (num < this._vertices.Length)
        return;
      long length = this._vertices.Length;
      this._vertices.Resize(num);
      for (long index = length; index < num; ++index)
        this._vertices[index] = 4294967294U;
    }

    private void IncreaseEdgeSize()
    {
      this.IncreaseEdgeSize(this._edges.Length + 10000L);
    }

    private void IncreaseEdgeSize(long size)
    {
      long length = this._edges.Length;
      this._edges.Resize(size);
      for (long index = length; index < size; ++index)
        this._edges[index] = uint.MaxValue;
    }

    public void AddVertex(uint vertex)
    {
      if ((long) vertex > this._vertices.Length - 1L)
        this.IncreaseVertexSize((long) vertex);
      if (!this._maxVertex.HasValue || this._maxVertex.Value < vertex)
        this._maxVertex = new uint?(vertex);
      if ((int) this._vertices[(long) vertex] != -2)
        return;
      this._vertices[(long) vertex] = uint.MaxValue;
    }

    public bool HasVertex(uint vertex)
    {
      return (long) vertex <= this._vertices.Length - 1L && (int) this._vertices[(long) vertex] != -2;
    }

    public bool RemoveVertex(uint vertex)
    {
      if ((long) vertex > this._vertices.Length - 1L || (int) this._vertices[(long) vertex] == -2)
        return false;
      this.RemoveEdges(vertex);
      this._vertices[(long) vertex] = 4294967294U;
      int num = (int) vertex;
      uint? maxVertex = this._maxVertex;
      int valueOrDefault = (int) maxVertex.GetValueOrDefault();
      if ((num == valueOrDefault ? (maxVertex.HasValue ? 1 : 0) : 0) != 0)
      {
        while (vertex > 0U)
        {
          --vertex;
          if ((int) this._vertices[(long) vertex] != -2)
          {
            this._maxVertex = new uint?(vertex);
            break;
          }
          if ((int) vertex == 0)
            this._maxVertex = new uint?();
        }
      }
      return true;
    }

    public uint AddEdge(uint vertex1, uint vertex2, params uint[] data)
    {
      if ((int) vertex1 == (int) vertex2)
        throw new ArgumentException("Given vertices must be different.");
      if ((long) vertex1 > this._vertices.Length - 1L)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex1));
      if ((long) vertex2 > this._vertices.Length - 1L)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex2));
      if ((int) this._vertices[(long) vertex1] == -2)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex1));
      if ((int) this._vertices[(long) vertex2] == -2)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex2));
      if (data.Length != this._edgeDataSize)
        throw new ArgumentException("Data block has incorrect size, needs to match exactly edge data size.");
      uint nextEdgeId;
      if ((int) this._vertices[(long) vertex1] != -1)
      {
        uint num1 = this._vertices[(long) vertex1];
        uint num2 = 0;
        while ((int) num1 != -1)
        {
          uint num3 = num1;
          bool flag = true;
          uint num4;
          if ((int) this._edges[(long) (num1 + 0U)] == (int) vertex1)
          {
            num4 = this._edges[(long) (num1 + 1U)];
            num2 = num1 + 2U;
            num1 = this._edges[(long) (num1 + 2U)];
          }
          else
          {
            num4 = this._edges[(long) (num1 + 0U)];
            num2 = num1 + 3U;
            num1 = this._edges[(long) (num1 + 3U)];
            flag = false;
          }
          if ((int) num4 == (int) vertex2)
          {
            if (!flag)
            {
              uint num5 = this._edges[(long) (num3 + 0U)];
              this._edges[(long) (num3 + 0U)] = this._edges[(long) (num3 + 1U)];
              this._edges[(long) (num3 + 1U)] = num5;
              uint num6 = this._edges[(long) (num3 + 2U)];
              this._edges[(long) (num3 + 2U)] = this._edges[(long) (num3 + 3U)];
              this._edges[(long) (num3 + 3U)] = num6;
            }
            for (int index = 0; index < this._edgeDataSize; ++index)
              this._edges[(long) (num3 + 4U) + (long) index] = data[index];
            return (uint) ((ulong) num3 / (ulong) this._edgeSize);
          }
        }
        nextEdgeId = this._nextEdgeId;
        if ((long) (this._nextEdgeId + 3U) >= this._edges.Length)
          this.IncreaseEdgeSize();
        this._edges[(long) (this._nextEdgeId + 0U)] = vertex1;
        this._edges[(long) (this._nextEdgeId + 1U)] = vertex2;
        this._edges[(long) (this._nextEdgeId + 2U)] = uint.MaxValue;
        this._edges[(long) (this._nextEdgeId + 3U)] = uint.MaxValue;
        this._nextEdgeId = (uint) ((ulong) this._nextEdgeId + (ulong) this._edgeSize);
        this._edges[(long) num2] = nextEdgeId;
        for (int index = 0; index < this._edgeDataSize; ++index)
          this._edges[(long) (nextEdgeId + 4U) + (long) index] = data[index];
        this._edgeCount = this._edgeCount + 1L;
      }
      else
      {
        nextEdgeId = this._nextEdgeId;
        this._vertices[(long) vertex1] = this._nextEdgeId;
        if ((long) (this._nextEdgeId + 3U) >= this._edges.Length)
          this.IncreaseEdgeSize();
        this._edges[(long) (this._nextEdgeId + 0U)] = vertex1;
        this._edges[(long) (this._nextEdgeId + 1U)] = vertex2;
        this._edges[(long) (this._nextEdgeId + 2U)] = uint.MaxValue;
        this._edges[(long) (this._nextEdgeId + 3U)] = uint.MaxValue;
        this._nextEdgeId = (uint) ((ulong) this._nextEdgeId + (ulong) this._edgeSize);
        for (int index = 0; index < this._edgeDataSize; ++index)
          this._edges[(long) (nextEdgeId + 4U) + (long) index] = data[index];
        this._edgeCount = this._edgeCount + 1L;
      }
      uint num7 = this._vertices[(long) vertex2];
      if ((int) num7 != -1)
      {
        uint num1 = 0;
        while ((int) num7 != -1)
        {
          if ((int) this._edges[(long) (num7 + 0U)] == (int) vertex2)
          {
            int num2 = (int) this._edges[(long) (num7 + 1U)];
            num1 = num7 + 2U;
            num7 = this._edges[(long) (num7 + 2U)];
          }
          else
          {
            int num2 = (int) this._edges[(long) (num7 + 0U)];
            num1 = num7 + 3U;
            num7 = this._edges[(long) (num7 + 3U)];
          }
        }
        this._edges[(long) num1] = nextEdgeId;
      }
      else
        this._vertices[(long) vertex2] = nextEdgeId;
      return (uint) ((ulong) nextEdgeId / (ulong) this._edgeSize);
    }

    public Edge GetEdge(uint edgeId)
    {
      uint num = edgeId * (uint) this._edgeSize;
      if (this._edges.Length < (long) (num + (uint) this._edgeSize))
        throw new ArgumentOutOfRangeException();
      uint[] data = new uint[this._edgeDataSize];
      for (int index = 0; index < this._edgeDataSize; ++index)
        data[index] = this._edges[(long) (num + 4U) + (long) index];
      return new Edge(edgeId, this._edges[(long) (num + 0U)], this._edges[(long) (num + 1U)], data, false);
    }

    public int RemoveEdges(uint vertex)
    {
      int num = 0;
      Graph.EdgeEnumerator edgeEnumerator = this.GetEdgeEnumerator(vertex);
      while (edgeEnumerator.MoveNext())
      {
        if (this.RemoveEdge(vertex, edgeEnumerator.To))
          ++num;
      }
      return num;
    }

    public bool RemoveEdge(uint edgeId)
    {
      edgeId *= (uint) this._edgeSize;
      if (this._edges.Length < (long) (edgeId + (uint) this._edgeSize))
        return false;
      return this.RemoveEdge(this._edges[(long) (edgeId + 0U)], this._edges[(long) (edgeId + 1U)]);
    }

    public bool RemoveEdge(uint vertex1, uint vertex2)
    {
      if ((int) vertex1 == (int) vertex2)
        throw new ArgumentException("Given vertices must be different.");
      if ((long) vertex1 > this._vertices.Length - 1L)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex1));
      if ((long) vertex2 > this._vertices.Length - 1L)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex2));
      if ((int) this._vertices[(long) vertex1] == -2)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex1));
      if ((int) this._vertices[(long) vertex2] == -2)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex2));
      if ((int) this._vertices[(long) vertex1] == -1 || (int) this._vertices[(long) vertex2] == -1)
        return false;
      bool flag = false;
      uint num1 = this._vertices[(long) vertex1];
      uint num2 = 0;
      uint num3 = 0;
      uint num4 = 0;
      while ((int) num1 != -1)
      {
        uint num5 = num1;
        uint num6 = num2;
        uint num7;
        if ((int) this._edges[(long) (num1 + 0U)] == (int) vertex1)
        {
          num7 = this._edges[(long) (num1 + 1U)];
          num2 = num1 + 2U;
          num1 = this._edges[(long) (num1 + 2U)];
        }
        else
        {
          num7 = this._edges[(long) (num1 + 0U)];
          num2 = num1 + 3U;
          num1 = this._edges[(long) (num1 + 3U)];
        }
        if ((int) num7 == (int) vertex2)
        {
          if ((int) this._vertices[(long) vertex1] == (int) num5)
            this._vertices[(long) vertex1] = num1;
          else
            this._edges[(long) num6] = num1;
          flag = true;
          this._edgeCount = this._edgeCount - 1L;
          break;
        }
      }
      if (flag)
      {
        uint num5 = this._vertices[(long) vertex2];
        uint num6 = 0;
        num3 = 0U;
        num4 = 0U;
        while ((int) num5 != -1)
        {
          uint num7 = num5;
          uint num8 = num6;
          uint num9;
          if ((int) this._edges[(long) (num5 + 0U)] == (int) vertex2)
          {
            num9 = this._edges[(long) (num5 + 1U)];
            num6 = num5 + 2U;
            num5 = this._edges[(long) (num5 + 2U)];
          }
          else
          {
            num9 = this._edges[(long) (num5 + 0U)];
            num6 = num5 + 3U;
            num5 = this._edges[(long) (num5 + 3U)];
          }
          if ((int) num9 == (int) vertex1)
          {
            if ((int) this._vertices[(long) vertex2] == (int) num7)
              this._vertices[(long) vertex2] = num5;
            else
              this._edges[(long) num8] = num5;
            this._edges[(long) (num7 + 0U)] = uint.MaxValue;
            this._edges[(long) (num7 + 1U)] = uint.MaxValue;
            this._edges[(long) (num7 + 2U)] = uint.MaxValue;
            this._edges[(long) (num7 + 3U)] = uint.MaxValue;
            for (int index = 0; index < this._edgeDataSize; ++index)
              this._edges[(long) (num7 + 4U) + (long) index] = uint.MaxValue;
            return true;
          }
        }
        throw new Exception("Edge could not be reached from vertex2. Data in graph is invalid.");
      }
      return flag;
    }

    public void Switch(uint vertex1, uint vertex2)
    {
      if ((int) vertex1 == (int) vertex2)
        throw new ArgumentException("Given vertices must be different.");
      if ((long) vertex1 > this._vertices.Length - 1L)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex1));
      if ((long) vertex2 > this._vertices.Length - 1L)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex2));
      if ((int) this._vertices[(long) vertex1] == -2)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex1));
      if ((int) this._vertices[(long) vertex2] == -2)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex2));
      uint num1 = this._vertices[(long) vertex1];
      while ((int) num1 != -1)
      {
        if ((int) this._edges[(long) (num1 + 0U)] == (int) vertex1)
        {
          if ((int) this._edges[(long) (num1 + 1U)] == (int) vertex2)
            this._edges[(long) (num1 + 1U)] = vertex1;
          this._edges[(long) (num1 + 0U)] = vertex2;
          num1 = this._edges[(long) (num1 + 2U)];
        }
        else
        {
          if ((int) this._edges[(long) (num1 + 0U)] == (int) vertex2)
            this._edges[(long) (num1 + 0U)] = vertex1;
          this._edges[(long) (num1 + 1U)] = vertex2;
          num1 = this._edges[(long) (num1 + 3U)];
        }
      }
      uint num2 = this._vertices[(long) vertex2];
      while ((int) num2 != -1)
      {
        if ((int) this._edges[(long) (num2 + 0U)] == (int) vertex2)
        {
          if ((int) this._edges[(long) (num2 + 1U)] == (int) vertex1)
          {
            num2 = this._edges[(long) (num2 + 3U)];
          }
          else
          {
            this._edges[(long) (num2 + 0U)] = vertex1;
            num2 = this._edges[(long) (num2 + 2U)];
          }
        }
        else if ((int) this._edges[(long) (num2 + 0U)] == (int) vertex1)
        {
          num2 = this._edges[(long) (num2 + 2U)];
        }
        else
        {
          this._edges[(long) (num2 + 1U)] = vertex1;
          num2 = this._edges[(long) (num2 + 3U)];
        }
      }
      uint num3 = this._vertices[(long) vertex1];
      this._vertices[(long) vertex1] = this._vertices[(long) vertex2];
      this._vertices[(long) vertex2] = num3;
    }

    public Graph.EdgeEnumerator GetEdgeEnumerator()
    {
      return new Graph.EdgeEnumerator(this);
    }

    public Graph.EdgeEnumerator GetEdgeEnumerator(uint vertex)
    {
      if ((long) vertex >= this._vertices.Length)
        throw new ArgumentOutOfRangeException("vertex", "vertex is not part of this graph.");
      Graph.EdgeEnumerator edgeEnumerator = new Graph.EdgeEnumerator(this);
      int num = (int) vertex;
      edgeEnumerator.MoveTo((uint) num);
      return edgeEnumerator;
    }

    public void Compress(Action<uint, uint> updateEdgeId)
    {
      if (this._edgeCount == this._edges.Length / (long) this._edgeSize)
        return;
      uint newEdgeId = 0;
      for (uint oldEdgeId = 0; oldEdgeId < this._nextEdgeId; oldEdgeId = (uint) ((ulong) oldEdgeId + (ulong) this._edgeSize))
      {
        if ((int) this._edges[(long) oldEdgeId] != -1)
        {
          if ((int) oldEdgeId != (int) newEdgeId)
          {
            this.MoveEdge(oldEdgeId, newEdgeId);
            if (updateEdgeId != null)
              updateEdgeId((uint) ((ulong) oldEdgeId / (ulong) this._edgeSize), (uint) ((ulong) newEdgeId / (ulong) this._edgeSize));
          }
          newEdgeId = (uint) ((ulong) newEdgeId + (ulong) this._edgeSize);
        }
      }
      this._nextEdgeId = newEdgeId;
      this.Trim();
    }

    public void Compress()
    {
      this.Compress((Action<uint, uint>) null);
    }

    public void Trim()
    {
      this._edges.Resize((long) this._nextEdgeId);
      long length = this._vertices.Length;
      while (length > 0L && (int) this._vertices[length - 1L] == -2)
        --length;
      this._vertices.Resize(length);
    }

    public void Sort(ArrayBase<uint> transformations)
    {
      int num1 = 0;
      while ((long) num1 < (long) this._nextEdgeId)
      {
        if ((int) this._edges[(long) (num1 + 0)] != -1)
          this._edges[(long) (num1 + 0)] = transformations[(long) this._edges[(long) (num1 + 0)]];
        if ((int) this._edges[(long) (num1 + 1)] != -1)
          this._edges[(long) (num1 + 1)] = transformations[(long) this._edges[(long) (num1 + 1)]];
        num1 += this._edgeSize;
      }
      QuickSort.Sort((Func<long, long>) (i =>
      {
        if (i < 0L || i >= transformations.Length)
          return long.MaxValue;
        return (long) transformations[i];
      }), (Action<long, long>) ((i, j) =>
      {
        uint num2 = this._vertices[i];
        this._vertices[i] = this._vertices[j];
        this._vertices[j] = num2;
        uint num3 = transformations[i];
        transformations[i] = transformations[j];
        transformations[j] = num3;
      }), 0L, (long) this.VertexCount);
    }

    public void Dispose()
    {
      this._edges.Dispose();
      this._vertices.Dispose();
    }

    public long Serialize(Stream stream)
    {
      this.Compress();
      if (this._vertices.Length > (long) this.VertexCount)
        this.Trim();
      long length = this._vertices.Length;
      long num1 = (long) this._nextEdgeId / (long) this._edgeSize;
      long num2 = 1;
      stream.WriteByte((byte) 1);
      stream.Write(BitConverter.GetBytes(length), 0, 8);
      long num3 = 8;
      long num4 = num2 + num3;
      stream.Write(BitConverter.GetBytes(num1), 0, 8);
      long num5 = 8;
      long num6 = num4 + num5;
      stream.Write(BitConverter.GetBytes(1), 0, 4);
      long num7 = 4;
      long num8 = num6 + num7;
      stream.Write(BitConverter.GetBytes(this._edgeSize), 0, 4);
      long num9 = 4;
      return num8 + num9 + this._vertices.CopyTo(stream) + this._edges.CopyTo(stream);
    }

    public static Graph Deserialize(Stream stream, GraphProfile profile)
    {
      long position1 = stream.Position;
      long num1 = 1;
      int num2 = stream.ReadByte();
      if (num2 != 1)
        throw new Exception(string.Format("Cannot deserialize graph: Invalid version #: {0}.", (object) num2));
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
      return new Graph(int32_2 - 4, vertices, edges);
    }

    private void MoveEdge(uint oldEdgeId, uint newEdgeId)
    {
      this._edges[(long) (newEdgeId + 0U)] = this._edges[(long) (oldEdgeId + 0U)];
      this._edges[(long) (newEdgeId + 1U)] = this._edges[(long) (oldEdgeId + 1U)];
      this._edges[(long) (newEdgeId + 2U)] = this._edges[(long) (oldEdgeId + 2U)];
      this._edges[(long) (newEdgeId + 3U)] = this._edges[(long) (oldEdgeId + 3U)];
      for (int index = 0; index < this._edgeDataSize; ++index)
        this._edges[(long) (newEdgeId + 4U) + (long) index] = this._edges[(long) (oldEdgeId + 4U) + (long) index];
      uint num1 = this._edges[(long) (oldEdgeId + 0U)];
      uint num2 = this._vertices[(long) num1];
      if ((int) num2 == (int) oldEdgeId)
      {
        this._vertices[(long) num1] = newEdgeId;
      }
      else
      {
        while ((int) num2 != -1)
        {
          uint num3 = num2 + 3U;
          if ((int) this._edges[(long) (num2 + 0U)] == (int) num1)
            num3 = num2 + 2U;
          num2 = this._edges[(long) num3];
          if ((int) num2 == (int) oldEdgeId)
          {
            this._edges[(long) num3] = newEdgeId;
            break;
          }
        }
      }
      uint num4 = this._edges[(long) (oldEdgeId + 1U)];
      uint num5 = this._vertices[(long) num4];
      if ((int) num5 == (int) oldEdgeId)
      {
        this._vertices[(long) num4] = newEdgeId;
      }
      else
      {
        while ((int) num5 != -1)
        {
          uint num3 = num5 + 3U;
          if ((int) this._edges[(long) (num5 + 0U)] == (int) num4)
            num3 = num5 + 2U;
          num5 = this._edges[(long) num3];
          if ((int) num5 == (int) oldEdgeId)
          {
            this._edges[(long) num3] = newEdgeId;
            break;
          }
        }
      }
      this._edges[(long) (oldEdgeId + 0U)] = uint.MaxValue;
      this._edges[(long) (oldEdgeId + 1U)] = uint.MaxValue;
      this._edges[(long) (oldEdgeId + 2U)] = uint.MaxValue;
      this._edges[(long) (oldEdgeId + 3U)] = uint.MaxValue;
      for (int index = 0; index < this._edgeDataSize; ++index)
        this._edges[(long) (oldEdgeId + 4U) + (long) index] = uint.MaxValue;
    }

    public class EdgeEnumerator : IEnumerable<Edge>, IEnumerable, IEnumerator<Edge>, IEnumerator, IDisposable
    {
      private readonly Graph _graph;
      private uint _nextEdgeId;
      private uint _vertex;
      private uint _currentEdgeId;
      private bool _currentEdgeInverted;
      private uint _startVertex;
      private uint _startEdge;
      private uint _neighbour;

      public bool HasData
      {
        get
        {
          if ((int) this._startVertex != -1)
            return (int) this._graph._vertices[(long) this._startVertex] != -1;
          return false;
        }
      }

      public uint From
      {
        get
        {
          return this._vertex;
        }
      }

      public uint To
      {
        get
        {
          return this._neighbour;
        }
      }

      public uint[] Data
      {
        get
        {
          uint[] numArray = new uint[this._graph._edgeDataSize];
          for (int index = 0; index < this._graph._edgeDataSize; ++index)
            numArray[index] = this._graph._edges[(long) (this._currentEdgeId + 4U) + (long) index];
          return numArray;
        }
      }

      public uint Data0
      {
        get
        {
          return this._graph._edges[(long) (uint) ((int) this._currentEdgeId + 4 + 0)];
        }
      }

      public uint Data1
      {
        get
        {
          return this._graph._edges[(long) (uint) ((int) this._currentEdgeId + 4 + 1)];
        }
      }

      public bool DataInverted
      {
        get
        {
          return this._currentEdgeInverted;
        }
      }

      public uint Id
      {
        get
        {
          return (uint) ((ulong) this._currentEdgeId / (ulong) this._graph._edgeSize);
        }
      }

      public Edge Current
      {
        get
        {
          return new Edge(this);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      internal EdgeEnumerator(Graph graph)
      {
        this._graph = graph;
        this._currentEdgeId = uint.MaxValue;
        this._vertex = uint.MaxValue;
        this._startVertex = uint.MaxValue;
        this._startEdge = uint.MaxValue;
        this._currentEdgeInverted = false;
      }

      public bool MoveNext()
      {
        if ((int) this._nextEdgeId == -1)
          return false;
        this._currentEdgeId = this._nextEdgeId;
        this._neighbour = 0U;
        if ((int) this._graph._edges[(long) (this._nextEdgeId + 0U)] == (int) this._vertex)
        {
          this._neighbour = this._graph._edges[(long) (this._nextEdgeId + 1U)];
          this._nextEdgeId = this._graph._edges[(long) (this._nextEdgeId + 2U)];
          this._currentEdgeInverted = false;
        }
        else
        {
          this._neighbour = this._graph._edges[(long) (this._nextEdgeId + 0U)];
          this._nextEdgeId = this._graph._edges[(long) (this._nextEdgeId + 3U)];
          this._currentEdgeInverted = true;
        }
        return true;
      }

      public void Reset()
      {
        this._nextEdgeId = this._startEdge;
        this._currentEdgeId = 0U;
        this._vertex = this._startVertex;
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
        uint num = this._graph._vertices[(long) vertex];
        this._nextEdgeId = num;
        this._currentEdgeId = 0U;
        this._vertex = vertex;
        this._startVertex = vertex;
        this._startEdge = num;
        this._currentEdgeInverted = false;
        return (int) num != -1;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
