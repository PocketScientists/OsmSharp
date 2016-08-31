using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing.Data;
using OsmSharp.Routing.Graphs;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Default
{
  public class Dykstra : AlgorithmBase
  {
    private readonly Graph _graph;
    private readonly IEnumerable<Path> _sources;
    private readonly Func<ushort, Factor> _getFactor;
    private readonly float _sourceMax;
    private readonly bool _backward;
    private Graph.EdgeEnumerator _edgeEnumerator;
    private Dictionary<uint, Path> _visits;
    private Path _current;
    private BinaryHeap<Path> _heap;
    private Dictionary<uint, Factor> _factors;

    public bool MaxReached { get; private set; }

    public Dykstra.WasFoundDelegate WasFound { get; set; }

    public Dykstra.WasEdgeFoundDelegate WasEdgeFound { get; set; }

    public bool Backward
    {
      get
      {
        return this._backward;
      }
    }

    public Graph Graph
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

    public Dykstra(Graph graph, Func<ushort, Factor> getFactor, IEnumerable<Path> sources, float sourceMax, bool backward)
    {
      this._graph = graph;
      this._sources = sources;
      this._getFactor = getFactor;
      this._sourceMax = sourceMax;
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
      this._factors = new Dictionary<uint, Factor>();
      this._visits = new Dictionary<uint, Path>();
      this._heap = new BinaryHeap<Path>(1000U);
      foreach (Path source in this._sources)
        this._heap.Push(source, source.Weight);
      this._edgeEnumerator = this._graph.GetEdgeEnumerator();
    }

    public bool Step()
    {
      this._current = (Path) null;
      if (this._heap.Count > 0)
      {
        this._current = this._heap.Pop();
        while (this._current != null && this._visits.ContainsKey(this._current.Vertex) && this._heap.Count != 0)
          this._current = this._heap.Pop();
      }
      if (this._current == null || this._visits.ContainsKey(this._current.Vertex))
        return false;
      this._visits[this._current.Vertex] = this._current;
      if (this.WasFound != null && this.WasFound(this._current.Vertex, this._current.Weight))
        return false;
      this._edgeEnumerator.MoveTo(this._current.Vertex);
      while (this._edgeEnumerator.MoveNext())
      {
        Graph.EdgeEnumerator edgeEnumerator = this._edgeEnumerator;
        uint to = edgeEnumerator.To;
        if (this._current.From != null && (int) this._current.From.Vertex == (int) to)
        {
          if (this.WasEdgeFound != null && this.WasEdgeFound(this._current.Vertex, edgeEnumerator.Id, this._current.Weight))
            return false;
        }
        else if (this._visits.ContainsKey(to))
        {
          if (this.WasEdgeFound != null && this.WasEdgeFound(this._current.Vertex, edgeEnumerator.Id, this._current.Weight))
            return false;
        }
        else
        {
          float distance;
          ushort profile;
          EdgeDataSerializer.Deserialize(edgeEnumerator.Data0, out distance, out profile);
          Factor factor = Factor.NoFactor;
          if (!this._factors.TryGetValue((uint) profile, out factor))
          {
            factor = this._getFactor(profile);
            this._factors.Add((uint) profile, factor);
          }
          if ((double) factor.Value > 0.0 && ((int) factor.Direction == 0 || !this._backward && (int) factor.Direction == 1 != edgeEnumerator.DataInverted || this._backward && (int) factor.Direction == 1 == edgeEnumerator.DataInverted))
          {
            float num = this._current.Weight + distance * factor.Value;
            if ((double) num < (double) this._sourceMax)
              this._heap.Push(new Path(to, num, this._current), num);
          }
        }
      }
      return true;
    }

    public bool SetVisit(Path visit)
    {
      if (this._visits.ContainsKey(visit.Vertex))
        return false;
      this._heap.Push(visit, visit.Weight);
      return true;
    }

    public bool TryGetVisit(uint vertex, out Path visit)
    {
      return this._visits.TryGetValue(vertex, out visit);
    }

    public delegate bool WasFoundDelegate(uint vertex, float weight);

    public delegate bool WasEdgeFoundDelegate(uint vertex, uint edge, float weight);
  }
}
