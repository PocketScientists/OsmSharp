using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted.Witness
{
  public class DykstraWitnessCalculator : IWitnessCalculator
  {
    private readonly BinaryHeap<DykstraWitnessCalculator.SettledVertex> _heap;
    private int _hopLimit;
    private int _maxSettles;

    public int HopLimit
    {
      get
      {
        return this._hopLimit;
      }
      set
      {
        this._hopLimit = value;
      }
    }

    public int MaxSettles
    {
      get
      {
        return this._maxSettles;
      }
      set
      {
        this._maxSettles = value;
      }
    }

    public DykstraWitnessCalculator(int hopLimit)
      : this(hopLimit, int.MaxValue)
    {
    }

    public DykstraWitnessCalculator(int hopLimit, int maxSettles)
    {
      this._hopLimit = hopLimit;
      this._heap = new BinaryHeap<DykstraWitnessCalculator.SettledVertex>();
      this._maxSettles = maxSettles;
    }

    public void Calculate(DirectedGraph graph, uint source, List<uint> targets, List<float> weights, ref bool[] forwardWitness, ref bool[] backwardWitness, uint vertexToSkip)
    {
      if (this._hopLimit == 1)
      {
        this.ExistsOneHop(graph, source, targets, weights, ref forwardWitness, ref backwardWitness);
      }
      else
      {
        HashSet<uint> uintSet1 = new HashSet<uint>();
        HashSet<uint> uintSet2 = new HashSet<uint>();
        HashSet<uint> uintSet3 = new HashSet<uint>();
        HashSet<uint> uintSet4 = new HashSet<uint>();
        float num1 = 0.0f;
        float num2 = 0.0f;
        for (int index = 0; index < weights.Count; ++index)
        {
          if (!forwardWitness[index])
          {
            uintSet4.Add(targets[index]);
            if ((double) num1 < (double) weights[index])
              num1 = weights[index];
          }
          if (!backwardWitness[index])
          {
            uintSet3.Add(targets[index]);
            if ((double) num2 < (double) weights[index])
              num2 = weights[index];
          }
        }
        if ((double) num1 == 0.0 && (double) num2 == 0.0)
          return;
        Dictionary<uint, float> dictionary1 = new Dictionary<uint, float>();
        Dictionary<uint, float> dictionary2 = new Dictionary<uint, float>();
        this._heap.Clear();
        this._heap.Push(new DykstraWitnessCalculator.SettledVertex(source, 0.0f, 0U, (double) num1 > 0.0, (double) num2 > 0.0), 0.0f);
        DirectedGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator();
        while (this._heap.Count > 0)
        {
          DykstraWitnessCalculator.SettledVertex settledVertex1 = this._heap.Pop();
          if ((long) (settledVertex1.Hops + 1U) < (long) this._hopLimit && (int) settledVertex1.VertexId != (int) vertexToSkip)
          {
            bool flag1 = uintSet2.Contains(settledVertex1.VertexId);
            bool flag2 = uintSet1.Contains(settledVertex1.VertexId);
            if (!(flag1 & flag2))
            {
              if (settledVertex1.Forward)
              {
                uintSet2.Add(settledVertex1.VertexId);
                dictionary1.Remove(settledVertex1.VertexId);
                if (uintSet4.Contains(settledVertex1.VertexId))
                {
                  for (int index = 0; index < targets.Count; ++index)
                  {
                    if ((int) targets[index] == (int) settledVertex1.VertexId)
                    {
                      forwardWitness[index] = (double) settledVertex1.Weight < (double) weights[index];
                      uintSet4.Remove(settledVertex1.VertexId);
                    }
                  }
                }
              }
              if (settledVertex1.Backward)
              {
                uintSet1.Add(settledVertex1.VertexId);
                dictionary2.Remove(settledVertex1.VertexId);
                if (uintSet3.Contains(settledVertex1.VertexId))
                {
                  for (int index = 0; index < targets.Count; ++index)
                  {
                    if ((int) targets[index] == (int) settledVertex1.VertexId)
                    {
                      backwardWitness[index] = (double) settledVertex1.Weight < (double) weights[index];
                      uintSet3.Remove(settledVertex1.VertexId);
                    }
                  }
                }
              }
              if (uintSet4.Count == 0 && uintSet3.Count == 0 || uintSet2.Count >= this._maxSettles && uintSet1.Count >= this._maxSettles)
                break;
              bool flag3 = settledVertex1.Forward && uintSet4.Count > 0 && !flag1;
              bool flag4 = settledVertex1.Backward && uintSet3.Count > 0 && !flag2;
              if (flag3 | flag4)
              {
                edgeEnumerator.MoveTo(settledVertex1.VertexId);
                while (edgeEnumerator.MoveNext())
                {
                  uint neighbour = edgeEnumerator.Neighbour;
                  float weight1;
                  bool? direction;
                  ContractedEdgeDataSerializer.Deserialize(edgeEnumerator.Data0, out weight1, out direction);
                  bool flag5 = !direction.HasValue || direction.Value;
                  bool flag6 = !direction.HasValue || !direction.Value;
                  float weight2 = settledVertex1.Weight + weight1;
                  bool forward = flag3 & flag5 && (double) weight2 < (double) num1 && !uintSet2.Contains(neighbour);
                  bool backward = flag4 & flag6 && (double) weight2 < (double) num2 && !uintSet1.Contains(neighbour);
                  if (backward | forward)
                  {
                    float num3;
                    if (forward)
                    {
                      if (dictionary1.TryGetValue(neighbour, out num3))
                      {
                        if ((double) num3 <= (double) weight2)
                          forward = false;
                        else
                          dictionary1[neighbour] = weight2;
                      }
                      else
                        dictionary1[neighbour] = weight2;
                    }
                    if (backward)
                    {
                      if (dictionary2.TryGetValue(neighbour, out num3))
                      {
                        if ((double) num3 <= (double) weight2)
                          backward = false;
                        else
                          dictionary2[neighbour] = weight2;
                      }
                      else
                        dictionary2[neighbour] = weight2;
                    }
                    if (backward | forward)
                    {
                      DykstraWitnessCalculator.SettledVertex settledVertex2 = new DykstraWitnessCalculator.SettledVertex(neighbour, weight2, settledVertex1.Hops + 1U, forward, backward);
                      this._heap.Push(settledVertex2, settledVertex2.Weight);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    public void ExistsOneHop(DirectedGraph graph, uint source, List<uint> targets, List<float> weights, ref bool[] forwardExists, ref bool[] backwardExists)
    {
      HashSet<uint> uintSet = new HashSet<uint>();
      float num1 = 0.0f;
      for (int index = 0; index < weights.Count; ++index)
      {
        if (!forwardExists[index] || !backwardExists[index])
        {
          uintSet.Add(targets[index]);
          if ((double) num1 < (double) weights[index])
            num1 = weights[index];
        }
      }
      if (uintSet.Count <= 0)
        return;
      DirectedGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator(source);
      while (edgeEnumerator.MoveNext())
      {
        uint neighbour = edgeEnumerator.Neighbour;
        if (uintSet.Contains(neighbour))
        {
          int index = targets.IndexOf(neighbour);
          uintSet.Remove(neighbour);
          float weight;
          bool? direction;
          uint contractedId;
          ContractedEdgeDataSerializer.Deserialize(edgeEnumerator.Data0, edgeEnumerator.Data1, out weight, out direction, out contractedId);
          int num2 = !direction.HasValue ? 1 : (direction.Value ? 1 : 0);
          bool flag = !direction.HasValue || !direction.Value;
          if (num2 != 0 && (double) weight < (double) weights[index])
            forwardExists[index] = true;
          if (flag && (double) weight < (double) weights[index])
            backwardExists[index] = true;
          if (uintSet.Count == 0)
            break;
        }
      }
    }

    private class SettledVertex
    {
      public uint VertexId { get; set; }

      public float Weight { get; set; }

      public uint Hops { get; set; }

      public bool Forward { get; set; }

      public bool Backward { get; set; }

      public SettledVertex(uint vertex, float weight, uint hops, bool forward, bool backward)
      {
        this.VertexId = vertex;
        this.Weight = weight;
        this.Hops = hops;
        this.Forward = forward;
        this.Backward = backward;
      }
    }
  }
}
