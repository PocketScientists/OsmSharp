using OsmSharp.Collections.LongIndex;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Logging;
using OsmSharp.Routing.Algorithms.Contracted.Witness;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public class HierarchyBuilder : AlgorithmBase
  {
    private int _k = 20;
    private readonly DirectedMetaGraph _graph;
    private readonly IPriorityCalculator _priorityCalculator;
    private readonly IWitnessCalculator _witnessCalculator;
    private BinaryHeap<uint> _queue;
    private ILongIndex _contractedFlags;
    private int _misses;
    private Queue<bool> _missesQueue;

    public HierarchyBuilder(DirectedMetaGraph graph, IPriorityCalculator priorityCalculator, IWitnessCalculator witnessCalculator)
    {
      this._graph = graph;
      this._priorityCalculator = priorityCalculator;
      this._witnessCalculator = witnessCalculator;
    }

    protected override void DoRun()
    {
      this._queue = new BinaryHeap<uint>((uint) this._graph.VertexCount);
      this._contractedFlags = (ILongIndex) new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
      this._missesQueue = new Queue<bool>();
      this.RemoveWitnessedEdges();
      this.CalculateQueue();
      uint? nullable = this.SelectNext();
      float num1 = 0.0f;
      int num2 = 0;
      long vertexCount = this._graph.VertexCount;
      while (nullable.HasValue)
      {
        this.Contract(nullable.Value);
        nullable = this.SelectNext();
        float num3 = (float) (System.Math.Floor((double) num2 / (double) vertexCount * 10000.0) / 100.0);
        if ((double) num3 < 99.0)
          num3 = (float) (System.Math.Floor((double) num2 / (double) vertexCount * 100.0) / 1.0);
        if ((double) num3 != (double) num1)
        {
          num1 = num3;
          int num4 = 0;
          int num5 = 0;
          int num6 = 0;
          Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
          for (uint vertex = 0; (long) vertex < this._graph.VertexCount; ++vertex)
          {
            if (!this._contractedFlags.Contains((long) vertex))
            {
              dictionary.Clear();
              DirectedMetaGraph.EdgeEnumerator edgeEnumerator = this._graph.GetEdgeEnumerator(vertex);
              if (edgeEnumerator != null)
              {
                int count = edgeEnumerator.Count;
                num4 = count + num4;
                if (num6 < count)
                  num6 = count;
              }
              ++num5;
            }
          }
          double num7 = (double) num4 / (double) num5;
          Log.TraceEvent("HierarchyBuilder", TraceEventType.Information, "Preprocessing... {0}% [{1}/{2}] {3}q #{4} max {5}", (object) num3, (object) num2, (object) vertexCount, (object) this._queue.Count, (object) num7, (object) num6);
        }
        ++num2;
      }
    }

    private void CalculateQueue()
    {
      Log.TraceEvent("HierarchyBuilder", TraceEventType.Information, "Calculating queue...");
      this._queue.Clear();
      for (uint vertex = 0; (long) vertex < this._graph.VertexCount; ++vertex)
      {
        if (!this._contractedFlags.Contains((long) vertex))
          this._queue.Push(vertex, this._priorityCalculator.Calculate(this._contractedFlags, vertex));
      }
    }

    private void RemoveWitnessedEdges()
    {
      Log.TraceEvent("HierarchyBuilder", TraceEventType.Information, "Removing witnessed edges...");
      List<MetaEdge> metaEdgeList = new List<MetaEdge>();
      List<float> weights = new List<float>();
      List<uint> targets = new List<uint>();
      for (uint index1 = 0; (long) index1 < this._graph.VertexCount; ++index1)
      {
        metaEdgeList.Clear();
        weights.Clear();
        targets.Clear();
        metaEdgeList.AddRange((IEnumerable<MetaEdge>) this._graph.GetEdgeEnumerator(index1));
        bool[] forwardWitness = new bool[metaEdgeList.Count];
        bool[] backwardWitness = new bool[metaEdgeList.Count];
        for (int index2 = 0; index2 < metaEdgeList.Count; ++index2)
        {
          MetaEdge metaEdge = metaEdgeList[index2];
          float weight;
          bool? direction;
          ContractedEdgeDataSerializer.Deserialize(metaEdge.Data[0], out weight, out direction);
          bool flag1 = !direction.HasValue || direction.Value;
          bool flag2 = !direction.HasValue || !direction.Value;
          forwardWitness[index2] = !flag1;
          backwardWitness[index2] = !flag2;
          weights.Add(weight);
          targets.Add(metaEdge.Neighbour);
        }
        this._witnessCalculator.Calculate(this._graph.Graph, index1, targets, weights, ref forwardWitness, ref backwardWitness, uint.MaxValue);
        for (int index2 = 0; index2 < metaEdgeList.Count; ++index2)
        {
          if (forwardWitness[index2] && backwardWitness[index2])
            this._graph.RemoveEdge(index1, targets[index2]);
          else if (forwardWitness[index2])
          {
            this._graph.RemoveEdge(index1, targets[index2]);
            this._graph.AddEdge(index1, targets[index2], weights[index2], new bool?(false), 4294967294U);
          }
          else if (backwardWitness[index2])
          {
            this._graph.RemoveEdge(index1, targets[index2]);
            this._graph.AddEdge(index1, targets[index2], weights[index2], new bool?(true), 4294967294U);
          }
        }
      }
    }

    private uint? SelectNext()
    {
      while (this._queue.Count > 0)
      {
        uint vertex = this._queue.Peek();
        if (this._contractedFlags.Contains((long) vertex))
        {
          int num1 = (int) this._queue.Pop();
        }
        else
        {
          float num2 = this._queue.PeekWeight();
          float priority = this._priorityCalculator.Calculate(this._contractedFlags, vertex);
          if ((double) priority != (double) num2)
          {
            this._missesQueue.Enqueue(true);
            this._misses = this._misses + 1;
          }
          else
            this._missesQueue.Enqueue(false);
          if (this._missesQueue.Count > this._k && this._missesQueue.Dequeue())
            this._misses = this._misses - 1;
          if (this._misses == this._k)
          {
            this.CalculateQueue();
            this._missesQueue.Clear();
            this._misses = 0;
          }
          else
          {
            if ((double) priority == (double) num2)
              return new uint?(this._queue.Pop());
            int num3 = (int) this._queue.Pop();
            this._queue.Push(vertex, priority);
          }
        }
      }
      return new uint?();
    }

    private void Contract(uint vertex)
    {
      List<MetaEdge> metaEdgeList = new List<MetaEdge>((IEnumerable<MetaEdge>) this._graph.GetEdgeEnumerator(vertex));
      int index1 = 0;
      while (index1 < metaEdgeList.Count)
      {
        this._graph.RemoveEdge(metaEdgeList[index1].Neighbour, vertex);
        if (this._contractedFlags.Contains((long) metaEdgeList[index1].Neighbour))
        {
          this._graph.RemoveEdge(vertex, metaEdgeList[index1].Neighbour);
          metaEdgeList.RemoveAt(index1);
        }
        else
          ++index1;
      }
      for (int capacity = 1; capacity < metaEdgeList.Count; ++capacity)
      {
        MetaEdge metaEdge1 = metaEdgeList[capacity];
        float weight1;
        bool? direction1;
        ContractedEdgeDataSerializer.Deserialize(metaEdge1.Data[0], out weight1, out direction1);
        bool flag1 = !direction1.HasValue || direction1.Value;
        bool flag2 = !direction1.HasValue || !direction1.Value;
        bool[] forwardWitness = new bool[capacity];
        bool[] backwardWitness = new bool[capacity];
        List<uint> targets = new List<uint>(capacity);
        List<float> weights = new List<float>(capacity);
        for (int index2 = 0; index2 < capacity; ++index2)
        {
          MetaEdge metaEdge2 = metaEdgeList[index2];
          float weight2;
          bool? direction2;
          ContractedEdgeDataSerializer.Deserialize(metaEdge2.Data[0], out weight2, out direction2);
          bool flag3 = !direction2.HasValue || direction2.Value;
          bool flag4 = !direction2.HasValue || !direction2.Value;
          forwardWitness[index2] = !(flag2 & flag3);
          backwardWitness[index2] = !(flag1 & flag4);
          targets.Add(metaEdge2.Neighbour);
          weights.Add(weight1 + weight2);
        }
        this._witnessCalculator.Calculate(this._graph.Graph, metaEdge1.Neighbour, targets, weights, ref forwardWitness, ref backwardWitness, vertex);
        for (int index2 = 0; index2 < capacity; ++index2)
        {
          MetaEdge metaEdge2 = metaEdgeList[index2];
          if ((int) metaEdge1.Neighbour != (int) metaEdge2.Neighbour)
          {
            if (!forwardWitness[index2] && !backwardWitness[index2])
            {
              this._graph.AddOrUpdateEdge(metaEdge1.Neighbour, metaEdge2.Neighbour, weights[index2], new bool?(), vertex);
              this._graph.AddOrUpdateEdge(metaEdge2.Neighbour, metaEdge1.Neighbour, weights[index2], new bool?(), vertex);
            }
            else if (!forwardWitness[index2])
            {
              this._graph.AddOrUpdateEdge(metaEdge1.Neighbour, metaEdge2.Neighbour, weights[index2], new bool?(true), vertex);
              this._graph.AddOrUpdateEdge(metaEdge2.Neighbour, metaEdge1.Neighbour, weights[index2], new bool?(false), vertex);
            }
            else if (!backwardWitness[index2])
            {
              this._graph.AddOrUpdateEdge(metaEdge1.Neighbour, metaEdge2.Neighbour, weights[index2], new bool?(false), vertex);
              this._graph.AddOrUpdateEdge(metaEdge2.Neighbour, metaEdge1.Neighbour, weights[index2], new bool?(true), vertex);
            }
          }
        }
      }
      this._contractedFlags.Add((long) vertex);
      this._priorityCalculator.NotifyContracted(vertex);
    }
  }
}
