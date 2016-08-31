using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Data;
using OsmSharp.Routing.Graphs.Geometric;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using Reminiscence.Arrays;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Network
{
  public class RoutingNetwork
  {
    private readonly int _edgeDataSize = 2;
    private readonly float _maxEdgeDistance;
    private readonly GeometricGraph _graph;
    private readonly ArrayBase<uint> _edgeData;
    private const int BLOCK_SIZE = 1000;

    public float MaxEdgeDistance
    {
      get
      {
        return this._maxEdgeDistance;
      }
    }

    public GeometricGraph GeometricGraph
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

    public RoutingNetwork(float maxEdgeDistance = 5000f)
    {
      this._graph = new GeometricGraph(1);
      this._edgeData = (ArrayBase<uint>) new MemoryArray<uint>((long) this._edgeDataSize * this._graph.EdgeCount);
      this._maxEdgeDistance = maxEdgeDistance;
    }

    public RoutingNetwork(MemoryMap map, float maxEdgeDistance = 5000f)
    {
      this._graph = new GeometricGraph(map, 1);
      this._edgeData = (ArrayBase<uint>) new MemoryArray<uint>((long) this._edgeDataSize * this._graph.EdgeCount);
      this._maxEdgeDistance = maxEdgeDistance;
    }

    public RoutingNetwork(MemoryMap map, RoutingNetworkProfile profile, float maxEdgeDistance = 5000f)
    {
      this._maxEdgeDistance = maxEdgeDistance;
      if (profile == null)
      {
        this._graph = new GeometricGraph(map, 1);
        this._edgeData = (ArrayBase<uint>) new Array<uint>(map, (long) this._edgeDataSize * this._graph.EdgeCount);
      }
      else
      {
        this._graph = new GeometricGraph(map, profile.GeometricGraphProfile, 1);
        this._edgeData = (ArrayBase<uint>) new Array<uint>(map, (long) this._edgeDataSize * this._graph.EdgeCount, profile.EdgeDataProfile);
      }
    }

    public RoutingNetwork(GeometricGraph graph, float maxEdgeDistance = 5000f)
    {
      this._graph = graph;
      this._edgeData = (ArrayBase<uint>) new MemoryArray<uint>((long) this._edgeDataSize * graph.EdgeCount);
      this._maxEdgeDistance = maxEdgeDistance;
    }

    public RoutingNetwork(MemoryMap map, GeometricGraph graph, float maxEdgeDistance = 5000f)
    {
      this._graph = graph;
      this._edgeData = (ArrayBase<uint>) new MemoryArray<uint>((long) this._edgeDataSize * graph.EdgeCount);
      this._maxEdgeDistance = maxEdgeDistance;
    }

    private RoutingNetwork(GeometricGraph graph, ArrayBase<uint> edgeData, float maxEdgeDistance = 5000f)
    {
      this._graph = graph;
      this._edgeData = edgeData;
      this._maxEdgeDistance = maxEdgeDistance;
    }

    private void IncreaseSizeEdgeData(uint edgeId)
    {
      long length = this._edgeData.Length;
      while ((long) edgeId >= length)
        length += 1000L;
      this._edgeData.Resize(length);
    }

    public void AddVertex(uint vertex, float latitude, float longitude)
    {
      this._graph.AddVertex(vertex, latitude, longitude);
    }

    public GeoCoordinateSimple GetVertex(uint vertex)
    {
      return this._graph.GetVertex(vertex);
    }

    public bool GetVertex(uint vertex, out float latitude, out float longitude)
    {
      return this._graph.GetVertex(vertex, out latitude, out longitude);
    }

    public bool RemoveVertex(uint vertex)
    {
      return this._graph.RemoveVertex(vertex);
    }

    public uint AddEdge(uint vertex1, uint vertex2, OsmSharp.Routing.Network.Data.EdgeData data, ShapeBase shape)
    {
      if ((double) data.Distance > (double) this._maxEdgeDistance)
        throw new ArgumentException("data.Distance too big for this network.");
      uint edgeId = this._graph.AddEdge(vertex1, vertex2, EdgeDataSerializer.Serialize(data.Distance, data.Profile), shape);
      if ((long) edgeId >= this._edgeData.Length)
        this.IncreaseSizeEdgeData(edgeId);
      this._edgeData[(long) edgeId] = data.MetaId;
      return edgeId;
    }

    public RoutingEdge GetEdge(uint edgeId)
    {
      GeometricEdge edge = this._graph.GetEdge(edgeId);
      OsmSharp.Routing.Data.EdgeData edgeData = EdgeDataSerializer.Deserialize(edge.Data);
      OsmSharp.Routing.Network.Data.EdgeData data = new OsmSharp.Routing.Network.Data.EdgeData()
      {
        MetaId = this._edgeData[(long) edgeId],
        Distance = edgeData.Distance,
        Profile = edgeData.Profile
      };
      return new RoutingEdge(edge.Id, edge.From, edge.To, data, edge.DataInverted, edge.Shape);
    }

    public int RemoveEdges(uint vertex)
    {
      return this._graph.RemoveEdges(vertex);
    }

    public bool RemoveEdge(uint edgeId)
    {
      return this._graph.RemoveEdge(edgeId);
    }

    public bool RemoveEdge(uint vertex1, uint vertex2)
    {
      return this._graph.RemoveEdge(vertex1, vertex2);
    }

    public void Switch(uint vertex1, uint vertex2)
    {
      this._graph.Switch(vertex1, vertex2);
    }

    public RoutingNetwork.EdgeEnumerator GetEdgeEnumerator()
    {
      return new RoutingNetwork.EdgeEnumerator(this, this._graph.GetEdgeEnumerator());
    }

    public RoutingNetwork.EdgeEnumerator GetEdgeEnumerator(uint vertex)
    {
      return new RoutingNetwork.EdgeEnumerator(this, this._graph.GetEdgeEnumerator(vertex));
    }

    public void Compress()
    {
      this._graph.Compress((Action<uint, uint>) ((originalId, newId) => this._edgeData[(long) newId] = this._edgeData[(long) originalId]));
      this._edgeData.Resize(this._graph.EdgeCount);
    }

    public void Trim()
    {
      this._graph.Trim();
      this._edgeData.Resize(this._graph.EdgeCount);
    }

    public void Dispose()
    {
      this._graph.Dispose();
      this._edgeData.Dispose();
    }

    public long Serialize(Stream stream)
    {
      this.Compress();
      long num1 = 1;
      stream.WriteByte((byte) 2);
      byte[] bytes = BitConverter.GetBytes(this._maxEdgeDistance);
      stream.Write(bytes, 0, 4);
      long num2 = 4;
      long num3 = num1 + num2 + this._graph.Serialize(stream);
      this._edgeData.CopyTo(stream);
      long num4 = this._edgeData.Length * 4L;
      return num3 + num4;
    }

    public static RoutingNetwork Deserialize(Stream stream, RoutingNetworkProfile profile)
    {
      int num1 = stream.ReadByte();
      if (num1 > 2)
        throw new Exception(string.Format("Cannot deserialize routing network: Invalid version #: {0}, upgrade OsmSharp.Routing.", (object) num1));
      long position1 = stream.Position;
      long position2 = stream.Position;
      float num2 = 5000f;
      if (num1 == 2)
      {
        byte[] buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        num2 = BitConverter.ToSingle(buffer, 0);
      }
      GeometricGraph graph = GeometricGraph.Deserialize(stream, profile == null ? (GeometricGraphProfile) null : profile.GeometricGraphProfile);
      long num3 = stream.Position - position1;
      long edgeCount = graph.EdgeCount;
      int num4 = 1;
      ArrayBase<uint> arrayBase;
      long num5;
      if (profile == null)
      {
        arrayBase = (ArrayBase<uint>) new MemoryArray<uint>(edgeCount * (long) num4);
        arrayBase.CopyFrom(stream);
        num5 = num3 + edgeCount * (long) num4 * 4L;
      }
      else
      {
        long position3 = stream.Position;
        arrayBase = (ArrayBase<uint>) new Array<uint>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position3, edgeCount * (long) num4 * 4L))).CreateUInt32(edgeCount * (long) num4), profile.EdgeDataProfile);
        num5 = num3 + edgeCount * (long) num4 * 4L;
      }
      stream.Seek(position2 + num5, SeekOrigin.Begin);
      ArrayBase<uint> edgeData = arrayBase;
      double num6 = (double) num2;
      return new RoutingNetwork(graph, edgeData, (float) num6);
    }

    public class EdgeEnumerator : IEnumerable<RoutingEdge>, IEnumerable, IEnumerator<RoutingEdge>, IEnumerator, IDisposable
    {
      private readonly RoutingNetwork _network;
      private readonly GeometricGraph.EdgeEnumerator _enumerator;

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

      public OsmSharp.Routing.Network.Data.EdgeData Data
      {
        get
        {
          OsmSharp.Routing.Data.EdgeData edgeData = EdgeDataSerializer.Deserialize(this._enumerator.Data);
          return new OsmSharp.Routing.Network.Data.EdgeData()
          {
            MetaId = this._network._edgeData[(long) this._enumerator.Id],
            Distance = edgeData.Distance,
            Profile = edgeData.Profile
          };
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
          return this._enumerator.Shape;
        }
      }

      public RoutingEdge Current
      {
        get
        {
          return new RoutingEdge(this);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      internal EdgeEnumerator(RoutingNetwork network, GeometricGraph.EdgeEnumerator enumerator)
      {
        this._network = network;
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

      public IEnumerator<RoutingEdge> GetEnumerator()
      {
        this.Reset();
        return (IEnumerator<RoutingEdge>) this;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
