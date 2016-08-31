using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using Reminiscence.Arrays;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Graphs.Geometric
{
  public class GeometricGraph
  {
    private const float NO_COORDINATE = 3.402823E+38f;
    private const int BLOCKSIZE = 1024;
    private readonly Graph _graph;
    private readonly ArrayBase<float> _coordinates;
    private readonly ShapesArray _shapes;

    public Graph Graph
    {
      get
      {
        return this._graph;
      }
    }

    public uint VertexCount
    {
      get
      {
        return this._graph.VertexCount;
      }
    }

    public long EdgeCount
    {
      get
      {
        return this._graph.EdgeCount;
      }
    }

    public long SizeInBytes
    {
      get
      {
        return 1L + this._graph.SizeInBytes + this._coordinates.Length * 4L + this._shapes.SizeInBytes;
      }
    }

    public GeometricGraph(int edgeDataSize)
      : this(edgeDataSize, 1024)
    {
    }

    public GeometricGraph(int edgeDataSize, int size)
    {
      this._graph = new Graph(edgeDataSize, (long) size);
      this._coordinates = (ArrayBase<float>) new MemoryArray<float>((long) (size * 2));
      for (int index = 0; (long) index < this._coordinates.Length; ++index)
        this._coordinates[(long) index] = float.MaxValue;
      this._shapes = new ShapesArray((long) size);
    }

    public GeometricGraph(MemoryMap map, int edgeDataSize)
      : this(map, edgeDataSize, 1024)
    {
    }

    public GeometricGraph(MemoryMap map, int edgeDataSize, int size)
    {
      this._graph = new Graph(map, edgeDataSize, (long) size);
      this._coordinates = (ArrayBase<float>) new Array<float>(map, (long) (size * 2));
      for (int index = 0; (long) index < this._coordinates.Length; ++index)
        this._coordinates[(long) index] = float.MaxValue;
      this._shapes = new ShapesArray(map, (long) size);
    }

    public GeometricGraph(MemoryMap map, GeometricGraphProfile profile, int edgeDataSize)
      : this(map, profile, edgeDataSize, 1024)
    {
    }

    public GeometricGraph(MemoryMap map, GeometricGraphProfile profile, int edgeDataSize, int size)
    {
      if (profile == null)
      {
        this._graph = new Graph(map, edgeDataSize, (long) size);
        this._coordinates = (ArrayBase<float>) new Array<float>(map, (long) (size * 2));
        for (int index = 0; (long) index < this._coordinates.Length; ++index)
          this._coordinates[(long) index] = float.MaxValue;
        this._shapes = new ShapesArray(map, (long) size);
      }
      else
      {
        this._graph = new Graph(map, profile.GraphProfile, edgeDataSize, (long) size);
        this._coordinates = (ArrayBase<float>) new Array<float>(map, (long) (size * 2), profile.CoordinatesProfile);
        for (int index = 0; (long) index < this._coordinates.Length; ++index)
          this._coordinates[(long) index] = float.MaxValue;
        this._shapes = new ShapesArray(map, (long) size);
      }
    }

    private GeometricGraph(Graph graph, ArrayBase<float> coordinates, ShapesArray shapes)
    {
      this._graph = graph;
      this._coordinates = coordinates;
      this._shapes = shapes;
    }

    public GeoCoordinateSimple GetVertex(uint vertex)
    {
      if ((long) vertex >= this._coordinates.Length)
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex));
      if (this._coordinates[(long) vertex].Equals(float.MaxValue))
        throw new ArgumentException(string.Format("Vertex {0} does not exist.", (object) vertex));
      return new GeoCoordinateSimple()
      {
        Latitude = this._coordinates[(long) (vertex * 2U)],
        Longitude = this._coordinates[(long) (uint) ((int) vertex * 2 + 1)]
      };
    }

    public bool GetVertex(uint vertex, out float latitude, out float longitude)
    {
      if ((long) (uint) ((int) vertex * 2 + 1) < this._coordinates.Length)
      {
        latitude = this._coordinates[(long) (vertex * 2U)];
        longitude = this._coordinates[(long) (uint) ((int) vertex * 2 + 1)];
        if (!latitude.Equals(float.MaxValue))
          return true;
      }
      latitude = 0.0f;
      longitude = 0.0f;
      return false;
    }

    public bool RemoveVertex(uint vertex)
    {
      if (!this._graph.RemoveVertex(vertex))
        return false;
      this._coordinates[(long) (vertex * 2U)] = float.MaxValue;
      this._coordinates[(long) (uint) ((int) vertex * 2 + 1)] = float.MaxValue;
      return true;
    }

    public void AddVertex(uint vertex, float latitude, float longitude)
    {
      this._graph.AddVertex(vertex);
      if ((long) (uint) ((int) vertex * 2 + 1) >= this._coordinates.Length)
      {
        int num = 1;
        while ((long) (uint) ((int) vertex * 2 + 1) >= this._coordinates.Length + (long) (num * 1024 * 2))
          ++num;
        long length = this._coordinates.Length;
        this._coordinates.Resize(this._coordinates.Length + (long) (num * 1024 * 2));
        for (long index = length; index < this._coordinates.Length; ++index)
          this._coordinates[index] = float.MaxValue;
      }
      this._coordinates[(long) (vertex * 2U)] = latitude;
      this._coordinates[(long) (uint) ((int) vertex * 2 + 1)] = longitude;
    }

    public uint AddEdge(uint vertex1, uint vertex2, uint[] data, ShapeBase shape)
    {
      uint num1 = this._graph.AddEdge(vertex1, vertex2, data);
      if ((long) num1 >= ((ArrayBase<ShapeBase>) this._shapes).Length)
      {
        int num2 = 1;
        while ((long) num1 >= ((ArrayBase<ShapeBase>) this._shapes).Length + (long) (1024 * num2))
          ++num2;
        ((ArrayBase<ShapeBase>) this._shapes).Resize(((ArrayBase<ShapeBase>) this._shapes).Length + (long) (num2 * 1024));
      }
      ((ArrayBase<ShapeBase>) this._shapes)[(long) num1] = shape;
      return num1;
    }

    public GeometricEdge GetEdge(uint edgeId)
    {
      Edge edge = this._graph.GetEdge(edgeId);
      return new GeometricEdge(edge.Id, edge.From, edge.To, edge.Data, edge.DataInverted, ((ArrayBase<ShapeBase>) this._shapes)[(long) edgeId]);
    }

    public int RemoveEdges(uint vertex)
    {
      int num = 0;
      GeometricGraph.EdgeEnumerator edgeEnumerator = this.GetEdgeEnumerator(vertex);
      while (edgeEnumerator.MoveNext())
      {
        ((ArrayBase<ShapeBase>) this._shapes)[(long) edgeEnumerator.Id] = (ShapeBase) null;
        if (this.RemoveEdge(edgeEnumerator.Id))
          ++num;
      }
      return num;
    }

    public bool RemoveEdge(uint edgeId)
    {
      if (!this._graph.RemoveEdge(edgeId))
        return false;
      ((ArrayBase<ShapeBase>) this._shapes)[(long) edgeId] = (ShapeBase) null;
      return true;
    }

    public bool RemoveEdge(uint vertex1, uint vertex2)
    {
      GeometricGraph.EdgeEnumerator edgeEnumerator = this.GetEdgeEnumerator(vertex1);
      while (edgeEnumerator.MoveNext())
      {
        if ((int) edgeEnumerator.To == (int) vertex2)
          return this.RemoveEdge(edgeEnumerator.Id);
      }
      return false;
    }

    public void Switch(uint vertex1, uint vertex2)
    {
      this._graph.Switch(vertex1, vertex2);
      float num1 = this._coordinates[(long) (vertex1 * 2U)];
      float num2 = this._coordinates[(long) (uint) ((int) vertex1 * 2 + 1)];
      this._coordinates[(long) (vertex1 * 2U)] = this._coordinates[(long) (vertex2 * 2U)];
      this._coordinates[(long) (uint) ((int) vertex1 * 2 + 1)] = this._coordinates[(long) (uint) ((int) vertex2 * 2 + 1)];
      this._coordinates[(long) (vertex2 * 2U)] = num1;
      this._coordinates[(long) (uint) ((int) vertex2 * 2 + 1)] = num2;
    }

    public GeometricGraph.EdgeEnumerator GetEdgeEnumerator()
    {
      return new GeometricGraph.EdgeEnumerator(this, this._graph.GetEdgeEnumerator());
    }

    public GeometricGraph.EdgeEnumerator GetEdgeEnumerator(uint vertex)
    {
      return new GeometricGraph.EdgeEnumerator(this, this._graph.GetEdgeEnumerator(vertex));
    }

    public void Compress(Action<uint, uint> updateEdgeId)
    {
      this._graph.Compress((Action<uint, uint>) ((originalId, newId) =>
      {
        updateEdgeId(originalId, newId);
        this._shapes.Switch((long) originalId, (long) newId);
      }));
      ((ArrayBase<ShapeBase>) this._shapes).Resize(this._graph.EdgeCount);
      this._coordinates.Resize(this._graph.VertexCapacity * 2L);
    }

    public void Compress()
    {
      this._graph.Compress((Action<uint, uint>) ((originalId, newId) => this._shapes.Switch((long) originalId, (long) newId)));
      ((ArrayBase<ShapeBase>) this._shapes).Resize(this._graph.EdgeCount);
      this._coordinates.Resize(this._graph.VertexCapacity * 2L);
    }

    public void Trim()
    {
      this._graph.Trim();
      this._coordinates.Resize((long) this._graph.VertexCount);
      ((ArrayBase<ShapeBase>) this._shapes).Resize(this._graph.EdgeCount);
    }

    public void Dispose()
    {
      this._graph.Dispose();
    }

    public long Serialize(Stream stream)
    {
      this.Compress();
      long num1 = 1;
      stream.WriteByte((byte) 1);
      long num2 = this._graph.Serialize(stream);
      return num1 + num2 + this._coordinates.CopyTo(stream) + ((ArrayBase<ShapeBase>) this._shapes).CopyTo(stream);
    }

    public static GeometricGraph Deserialize(Stream stream, GeometricGraphProfile profile)
    {
      int num1 = stream.ReadByte();
      if (num1 != 1)
        throw new Exception(string.Format("Cannot deserialize geometric graph: Invalid version #: {0}.", (object) num1));
      Graph graph = Graph.Deserialize(stream, profile == null ? (GraphProfile) null : profile.GraphProfile);
      long position1 = stream.Position;
      long num2 = 0;
      ArrayBase<float> coordinates;
      ShapesArray from;
      long num3;
      if (profile == null)
      {
        coordinates = (ArrayBase<float>) new MemoryArray<float>((long) (graph.VertexCount * 2U));
        coordinates.CopyFrom(stream);
        long num4 = num2 + (long) (uint) ((int) graph.VertexCount * 2 * 4);
        long size;
        from = ShapesArray.CreateFrom(stream, true, out size);
        num3 = num4 + size;
      }
      else
      {
        long position2 = stream.Position;
        coordinates = (ArrayBase<float>) new Array<float>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position2, (long) (uint) ((int) graph.VertexCount * 4 * 2)))).CreateSingle((long) (graph.VertexCount * 2U)), profile.CoordinatesProfile);
        long num4 = num2 + (long) (uint) ((int) graph.VertexCount * 2 * 4);
        stream.Seek(position2 + (long) (uint) ((int) graph.VertexCount * 4 * 2), SeekOrigin.Begin);
        long size;
        from = ShapesArray.CreateFrom(stream, false, out size);
        num3 = num4 + size;
      }
      stream.Seek(position1 + num3, SeekOrigin.Begin);
      return new GeometricGraph(graph, coordinates, from);
    }

    public class EdgeEnumerator : IEnumerable<GeometricEdge>, IEnumerable, IEnumerator<GeometricEdge>, IEnumerator, IDisposable
    {
      private readonly GeometricGraph _graph;
      private readonly Graph.EdgeEnumerator _enumerator;

      public bool HasData
      {
        get
        {
          return this._enumerator.HasData;
        }
      }

      public uint Id
      {
        get
        {
          return this._enumerator.Id;
        }
      }

      public uint From
      {
        get
        {
          return this._enumerator.From;
        }
      }

      public uint To
      {
        get
        {
          return this._enumerator.To;
        }
      }

      public uint[] Data
      {
        get
        {
          return this._enumerator.Data;
        }
      }

      public bool DataInverted
      {
        get
        {
          return this._enumerator.DataInverted;
        }
      }

      public ShapeBase Shape
      {
        get
        {
          return ((ArrayBase<ShapeBase>) this._graph._shapes)[(long) this._enumerator.Id];
        }
      }

      public GeometricEdge Current
      {
        get
        {
          return new GeometricEdge(this);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      internal EdgeEnumerator(GeometricGraph graph, Graph.EdgeEnumerator enumerator)
      {
        this._graph = graph;
        this._enumerator = enumerator;
      }

      public bool MoveTo(uint vertex)
      {
        return this._enumerator.MoveTo(vertex);
      }

      public bool MoveNext()
      {
        return this._enumerator.MoveNext();
      }

      public void Reset()
      {
        this._enumerator.Reset();
      }

      public void Dispose()
      {
        this._enumerator.Dispose();
      }

      public IEnumerator<GeometricEdge> GetEnumerator()
      {
        this.Reset();
        return (IEnumerator<GeometricEdge>) this;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
