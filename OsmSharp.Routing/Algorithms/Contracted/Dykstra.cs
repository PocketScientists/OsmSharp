using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public class Dykstra : AlgorithmBase
  {
    private readonly DirectedMetaGraph _graph;
    private readonly IEnumerable<Path> _sources;
    private readonly bool _backward;
    private DirectedGraph.EdgeEnumerator _edgeEnumerator;
    private Dictionary<uint, Path> _visits;
    private Path _current;
    private BinaryHeap<Path> _heap;

    public Func<uint, float, bool> WasFound { get; set; }

    public bool Backward
    {
      get
      {
        return this._backward;
      }
    }

    public DirectedMetaGraph Graph
    {
      get
      {
        return this._graph;
      }
    }

    public Path Current
    {
      get
      {
        return this._current;
      }
    }

    public Dykstra(DirectedMetaGraph graph, IEnumerable<Path> sources, bool backward)
    {
      this._graph = graph;
      this._sources = sources;
      this._backward = backward;
    }

    protected override void DoRun()
    {
      this.Initialize();
      do
        ;
      while (this.Step());
    }

    public void Initialize()
    {
      this.HasSucceeded = true;
      this._visits = new Dictionary<uint, Path>();
      this._heap = new BinaryHeap<Path>();
      foreach (Path source in this._sources)
        this._heap.Push(source, source.Weight);
      this._edgeEnumerator = this._graph.Graph.GetEdgeEnumerator();
    }

    public bool Step()
    {
      if (this._heap.Count == 0)
        return false;
      this._current = this._heap.Pop();
      if (this._current == null)
        return false;
      while (this._visits.ContainsKey(this._current.Vertex))
      {
        this._current = this._heap.Pop();
        if (this._current == null)
          return false;
      }
      this._visits.Add(this._current.Vertex, this._current);
      if (this.WasFound != null)
      {
        int num = this.WasFound(this._current.Vertex, this._current.Weight) ? 1 : 0;
      }
      this._edgeEnumerator.MoveTo(this._current.Vertex);
      while (this._edgeEnumerator.MoveNext())
      {
        float weight;
        bool? direction;
        ContractedEdgeDataSerializer.Deserialize(this._edgeEnumerator.Data0, out weight, out direction);
        if (!direction.HasValue || direction.Value == !this._backward)
        {
          uint neighbour = this._edgeEnumerator.Neighbour;
          if (!this._visits.ContainsKey(neighbour))
          {
            Path path = new Path(neighbour, this._current.Weight + weight, this._current);
            this._heap.Push(path, path.Weight);
          }
        }
      }
      return true;
    }

    public bool TryGetVisit(uint vertex, out Path visit)
    {
      return this._visits.TryGetValue(vertex, out visit);
    }
  }
}
