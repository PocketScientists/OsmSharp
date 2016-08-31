using OsmSharp.Collections.LongIndex;
using OsmSharp.Routing.Algorithms.Contracted.Witness;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public class EdgeDifferencePriorityCalculator : IPriorityCalculator
  {
    private readonly DirectedMetaGraph _graph;
    private readonly Dictionary<uint, int> _contractionCount;
    private readonly Dictionary<long, int> _depth;
    private readonly IWitnessCalculator _witnessCalculator;

    public int DifferenceFactor { get; set; }

    public int DepthFactor { get; set; }

    public int ContractedFactor { get; set; }

    public EdgeDifferencePriorityCalculator(DirectedMetaGraph graph, IWitnessCalculator witnessCalculator)
    {
      this._graph = graph;
      this._witnessCalculator = witnessCalculator;
      this._contractionCount = new Dictionary<uint, int>();
      this._depth = new Dictionary<long, int>();
      this.DifferenceFactor = 1;
      this.DepthFactor = 2;
      this.ContractedFactor = 1;
    }

    public float Calculate(ILongIndex contractedFlags, uint vertex)
    {
      int num1 = 0;
      int num2 = 0;
      List<Edge> edgeList = new List<Edge>((IEnumerable<Edge>) this._graph.Graph.GetEdgeEnumerator(vertex));
      int index1 = 0;
      while (index1 < edgeList.Count)
      {
        DirectedMetaGraph.EdgeEnumerator edgeEnumerator = this._graph.GetEdgeEnumerator(edgeList[index1].Neighbour);
        edgeEnumerator.Reset();
        while (edgeEnumerator.MoveNext())
        {
          if ((int) edgeEnumerator.Neighbour == (int) vertex)
            ++num1;
        }
        if (contractedFlags.Contains((long) edgeList[index1].Neighbour))
        {
          edgeEnumerator.MoveTo(vertex);
          edgeEnumerator.Reset();
          while (edgeEnumerator.MoveNext())
          {
            if ((int) edgeEnumerator.Neighbour == (int) edgeList[index1].Neighbour)
              ++num1;
          }
          edgeList.RemoveAt(index1);
        }
        else
          ++index1;
      }
      for (int capacity = 1; capacity < edgeList.Count; ++capacity)
      {
        Edge edge1 = edgeList[capacity];
        float weight1;
        bool? direction1;
        ContractedEdgeDataSerializer.Deserialize(edge1.Data[0], out weight1, out direction1);
        bool flag1 = !direction1.HasValue || direction1.Value;
        bool flag2 = !direction1.HasValue || !direction1.Value;
        bool[] forwardWitness = new bool[capacity];
        bool[] backwardWitness = new bool[capacity];
        List<uint> targets = new List<uint>(capacity);
        List<float> weights = new List<float>(capacity);
        for (int index2 = 0; index2 < capacity; ++index2)
        {
          Edge edge2 = edgeList[index2];
          float weight2;
          bool? direction2;
          ContractedEdgeDataSerializer.Deserialize(edge2.Data[0], out weight2, out direction2);
          bool flag3 = !direction2.HasValue || direction2.Value;
          bool flag4 = !direction2.HasValue || !direction2.Value;
          forwardWitness[index2] = !(flag2 & flag3);
          backwardWitness[index2] = !(flag1 & flag4);
          targets.Add(edge2.Neighbour);
          weights.Add(weight1 + weight2);
        }
        this._witnessCalculator.Calculate(this._graph.Graph, edge1.Neighbour, targets, weights, ref forwardWitness, ref backwardWitness, vertex);
        for (int index2 = 0; index2 < capacity; ++index2)
        {
          Edge edge2 = edgeList[index2];
          int removed = 0;
          int added = 0;
          if (!forwardWitness[index2] && !backwardWitness[index2])
          {
            this._graph.TryAddOrUpdateEdge(edge1.Neighbour, edge2.Neighbour, weights[index2], new bool?(), vertex, out added, out removed);
            int num3 = num2 + added;
            int num4 = num1 + removed;
            this._graph.TryAddOrUpdateEdge(edge2.Neighbour, edge1.Neighbour, weights[index2], new bool?(), vertex, out added, out removed);
            num2 = num3 + added;
            num1 = num4 + removed;
          }
          else if (!forwardWitness[index2])
          {
            this._graph.TryAddOrUpdateEdge(edge1.Neighbour, edge2.Neighbour, weights[index2], new bool?(true), vertex, out added, out removed);
            int num3 = num2 + added;
            int num4 = num1 + removed;
            this._graph.TryAddOrUpdateEdge(edge2.Neighbour, edge1.Neighbour, weights[index2], new bool?(false), vertex, out added, out removed);
            num2 = num3 + added;
            num1 = num4 + removed;
          }
          else if (!backwardWitness[index2])
          {
            this._graph.TryAddOrUpdateEdge(edge1.Neighbour, edge2.Neighbour, weights[index2], new bool?(false), vertex, out added, out removed);
            int num3 = num2 + added;
            int num4 = num1 + removed;
            this._graph.TryAddOrUpdateEdge(edge2.Neighbour, edge1.Neighbour, weights[index2], new bool?(true), vertex, out added, out removed);
            num2 = num3 + added;
            num1 = num4 + removed;
          }
        }
      }
      int num5 = 0;
      this._contractionCount.TryGetValue(vertex, out num5);
      int num6 = 0;
      this._depth.TryGetValue((long) vertex, out num6);
      return (float) (this.DifferenceFactor * (num2 - num1) + this.DepthFactor * num6 + this.ContractedFactor * num5);
    }

    public void NotifyContracted(uint vertex)
    {
      this._contractionCount.Remove(vertex);
      DirectedMetaGraph.EdgeEnumerator edgeEnumerator = this._graph.GetEdgeEnumerator(vertex);
      edgeEnumerator.Reset();
      while (edgeEnumerator.MoveNext())
      {
        uint neighbour = edgeEnumerator.Neighbour;
        int num;
        this._contractionCount[neighbour] = this._contractionCount.TryGetValue(neighbour, out num) ? num++ : 1;
      }
      int num1 = 0;
      this._depth.TryGetValue((long) vertex, out num1);
      this._depth.Remove((long) vertex);
      int num2 = num1 + 1;
      edgeEnumerator.Reset();
      while (edgeEnumerator.MoveNext())
      {
        uint neighbour = edgeEnumerator.Neighbour;
        int num3 = 0;
        this._depth.TryGetValue((long) neighbour, out num3);
        if (num2 >= num3)
          this._depth[(long) neighbour] = num2;
      }
    }
  }
}
