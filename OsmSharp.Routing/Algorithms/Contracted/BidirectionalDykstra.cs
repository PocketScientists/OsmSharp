using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public class BidirectionalDykstra : AlgorithmBase
  {
    private readonly DirectedMetaGraph _graph;
    private readonly IEnumerable<Path> _sources;
    private readonly IEnumerable<Path> _targets;
    private Tuple<uint, float> _best;
    private Dictionary<uint, Path> _forwardVisits;
    private Dictionary<uint, Path> _backwardVisits;

    public uint Best
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._best.Item1;
      }
    }

    public BidirectionalDykstra(DirectedMetaGraph graph, IEnumerable<Path> sources, IEnumerable<Path> targets)
    {
      this._graph = graph;
      this._sources = sources;
      this._targets = targets;
    }

    protected override void DoRun()
    {
      this._forwardVisits = new Dictionary<uint, Path>();
      this._backwardVisits = new Dictionary<uint, Path>();
      BinaryHeap<Path> queue1 = new BinaryHeap<Path>();
      BinaryHeap<Path> queue2 = new BinaryHeap<Path>();
      foreach (Path source in this._sources)
        queue1.Push(source, source.Weight);
      foreach (Path target in this._targets)
        queue2.Push(target, target.Weight);
      this._best = new Tuple<uint, float>(4294967294U, float.MaxValue);
      float num1 = queue2.PeekWeight();
      float num2 = queue1.PeekWeight();
      while ((queue2.Count != 0 || queue1.Count != 0) && ((double) this._best.Item2 >= (double) num2 || (double) this._best.Item2 >= (double) num1))
      {
        if (queue1.Count > 0)
        {
          Path current = queue1.Pop();
          while (current != null && this._forwardVisits.ContainsKey(current.Vertex))
            current = queue1.Pop();
          if (current != null)
          {
            Path path;
            if (this._backwardVisits.TryGetValue(current.Vertex, out path) && (double) current.Weight + (double) path.Weight < (double) this._best.Item2)
            {
              this._best = new Tuple<uint, float>(current.Vertex, current.Weight + path.Weight);
              this.HasSucceeded = true;
            }
            this.SearchForward(queue1, current);
          }
        }
        if (queue2.Count > 0)
        {
          Path current = queue2.Pop();
          while (current != null && this._backwardVisits.ContainsKey(current.Vertex))
            current = queue2.Pop();
          if (current != null)
          {
            Path path;
            if (this._forwardVisits.TryGetValue(current.Vertex, out path) && (double) current.Weight + (double) path.Weight < (double) this._best.Item2)
            {
              this._best = new Tuple<uint, float>(current.Vertex, current.Weight + path.Weight);
              this.HasSucceeded = true;
            }
            this.SearchBackward(queue2, current);
          }
        }
        if (queue1.Count > 0)
          num2 = queue1.PeekWeight();
        if (queue2.Count > 0)
          num1 = queue2.PeekWeight();
      }
    }

    private void SearchForward(BinaryHeap<Path> queue, Path current)
    {
      if (current == null)
        return;
      DirectedGraph.EdgeEnumerator edgeEnumerator = this._graph.Graph.GetEdgeEnumerator();
      Path path1;
      if (this._forwardVisits.TryGetValue(current.Vertex, out path1))
      {
        if ((double) path1.Weight > (double) current.Weight)
          this._forwardVisits[current.Vertex] = current;
      }
      else
        this._forwardVisits.Add(current.Vertex, current);
      edgeEnumerator.MoveTo(current.Vertex);
      while (edgeEnumerator.MoveNext())
      {
        float weight;
        bool? direction;
        ContractedEdgeDataSerializer.Deserialize(edgeEnumerator.Data0, out weight, out direction);
        if (!direction.HasValue || direction.Value)
        {
          uint neighbour = edgeEnumerator.Neighbour;
          if (!this._forwardVisits.ContainsKey(neighbour))
          {
            Path path2 = new Path(neighbour, current.Weight + weight, current);
            queue.Push(path2, path2.Weight);
          }
        }
      }
    }

    private void SearchBackward(BinaryHeap<Path> queue, Path current)
    {
      if (current == null)
        return;
      DirectedGraph.EdgeEnumerator edgeEnumerator = this._graph.Graph.GetEdgeEnumerator();
      Path path1;
      if (this._backwardVisits.TryGetValue(current.Vertex, out path1))
      {
        if ((double) path1.Weight > (double) current.Weight)
          this._backwardVisits[current.Vertex] = current;
      }
      else
        this._backwardVisits.Add(current.Vertex, current);
      edgeEnumerator.MoveTo(current.Vertex);
      while (edgeEnumerator.MoveNext())
      {
        float weight;
        bool? direction;
        ContractedEdgeDataSerializer.Deserialize(edgeEnumerator.Data0, out weight, out direction);
        if (!direction.HasValue || !direction.Value)
        {
          uint neighbour = edgeEnumerator.Neighbour;
          if (!this._backwardVisits.ContainsKey(neighbour))
          {
            Path path2 = new Path(neighbour, current.Weight + weight, current);
            queue.Push(path2, path2.Weight);
          }
        }
      }
    }

    public bool TryGetForwardVisit(uint vertex, out Path visit)
    {
      this.CheckHasRunAndHasSucceeded();
      return this._forwardVisits.TryGetValue(vertex, out visit);
    }

    public bool TryGetBackwardVisit(uint vertex, out Path visit)
    {
      this.CheckHasRunAndHasSucceeded();
      return this._backwardVisits.TryGetValue(vertex, out visit);
    }

    public List<uint> GetPath(out float weight)
    {
      this.CheckHasRunAndHasSucceeded();
      Path from1;
      Path from2;
      if (!this._forwardVisits.TryGetValue(this._best.Item1, out from1) || !this._backwardVisits.TryGetValue(this._best.Item1, out from2))
        throw new InvalidOperationException("No path could be found to/from source/target.");
      List<uint> vertices = new List<uint>();
      weight = from1.Weight + from2.Weight;
      vertices.Add(from1.Vertex);
      for (; from1.From != null; from1 = from1.From)
      {
        if ((int) from1.From.Vertex != -2)
          this._graph.ExpandEdge(from1.From.Vertex, from1.Vertex, vertices, false, true);
        vertices.Add(from1.From.Vertex);
      }
      vertices.Reverse();
      for (; from2.From != null; from2 = from2.From)
      {
        if ((int) from2.From.Vertex != -2)
          this._graph.ExpandEdge(from2.From.Vertex, from2.Vertex, vertices, false, false);
        vertices.Add(from2.From.Vertex);
      }
      return vertices;
    }

    public List<uint> GetPath()
    {
      float weight;
      return this.GetPath(out weight);
    }
  }
}
