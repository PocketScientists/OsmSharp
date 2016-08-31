using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public static class DirectedMetaGraphExtensions
  {
    public static void ExpandEdge(this DirectedMetaGraph graph, uint vertex1, uint vertex2, List<uint> vertices, bool inverted, bool forward)
    {
      MetaEdge shortestEdge = graph.GetShortestEdge(vertex1, vertex2, (Func<uint[], float?>) (data =>
      {
        float weight;
        bool? direction;
        ContractedEdgeDataSerializer.Deserialize(data[0], out weight, out direction);
        if (!direction.HasValue || direction.Value == forward)
          return new float?(weight);
        return new float?();
      }));
      if (shortestEdge == null)
        throw new Exception(string.Format("No edge found from {0} to {1}.", new object[2]
        {
          (object) vertex1,
          (object) vertex2
        }));
      uint contractedId = shortestEdge.GetContractedId();
      if ((int) contractedId == -2)
        return;
      if (inverted)
      {
        graph.ExpandEdge(contractedId, vertex1, vertices, false, !forward);
        vertices.Add(contractedId);
        graph.ExpandEdge(contractedId, vertex2, vertices, true, forward);
      }
      else
      {
        graph.ExpandEdge(contractedId, vertex2, vertices, false, forward);
        vertices.Add(contractedId);
        graph.ExpandEdge(contractedId, vertex1, vertices, true, !forward);
      }
    }

    public static void AddEdge(this DirectedMetaGraph graph, uint vertex1, uint vertex2, float weight, bool? direction, uint contractedId)
    {
      int num = (int) graph.AddEdge(vertex1, vertex2, ContractedEdgeDataSerializer.Serialize(weight, direction), contractedId);
    }

    public static void AddOrUpdateEdge(this DirectedMetaGraph graph, uint vertex1, uint vertex2, float weight, bool? direction, uint contractedId)
    {
      uint data1 = ContractedEdgeDataSerializer.Serialize(weight, direction);
      bool hasExistingEdge = false;
      bool hasExistingEdgeOnlySameDirection = true;
      if ((int) graph.UpdateEdge(vertex1, vertex2, (Func<uint[], bool>) (data =>
      {
        hasExistingEdge = true;
        if (ContractedEdgeDataSerializer.HasDirection(data[0], direction))
          return (double) weight < (double) ContractedEdgeDataSerializer.DeserializeWeight(data[0]);
        hasExistingEdgeOnlySameDirection = false;
        return false;
      }), new uint[1]{ data1 }, contractedId) != -1)
        return;
      if (!hasExistingEdge)
      {
        int num1 = (int) graph.AddEdge(vertex1, vertex2, data1, contractedId);
      }
      else
      {
        if (hasExistingEdgeOnlySameDirection)
          return;
        bool flag1 = false;
        float weight1 = float.MaxValue;
        uint metaData1 = uint.MaxValue;
        bool flag2 = false;
        float weight2 = float.MaxValue;
        uint metaData2 = uint.MaxValue;
        if (!direction.HasValue || direction.Value)
        {
          flag1 = true;
          weight1 = weight;
          metaData1 = contractedId;
        }
        if (!direction.HasValue || !direction.Value)
        {
          flag2 = true;
          weight2 = weight;
          metaData2 = contractedId;
        }
        DirectedMetaGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator(vertex1);
        while (edgeEnumerator.MoveNext())
        {
          if ((int) edgeEnumerator.Neighbour == (int) vertex2)
          {
            float weight3;
            bool? direction1;
            uint contractedId1;
            ContractedEdgeDataSerializer.Deserialize(edgeEnumerator.Data0, edgeEnumerator.MetaData0, out weight3, out direction1, out contractedId1);
            if ((!direction1.HasValue || direction1.Value) && (double) weight3 < (double) weight1)
            {
              weight1 = weight3;
              flag1 = true;
              metaData1 = contractedId1;
            }
            if ((!direction1.HasValue || !direction1.Value) && (double) weight3 < (double) weight2)
            {
              weight2 = weight3;
              flag2 = true;
              metaData2 = contractedId1;
            }
          }
        }
        graph.RemoveEdge(vertex1, vertex2);
        if (flag1 & flag2 && (double) weight1 == (double) weight2 && (int) metaData1 == (int) metaData2)
        {
          int num2 = (int) graph.AddEdge(vertex1, vertex2, ContractedEdgeDataSerializer.Serialize(weight1, new bool?()), metaData1);
        }
        else
        {
          if (flag1)
          {
            int num3 = (int) graph.AddEdge(vertex1, vertex2, ContractedEdgeDataSerializer.Serialize(weight1, new bool?(true)), metaData1);
          }
          if (!flag2)
            return;
          int num4 = (int) graph.AddEdge(vertex1, vertex2, ContractedEdgeDataSerializer.Serialize(weight2, new bool?(false)), metaData2);
        }
      }
    }

    public static void TryAddOrUpdateEdge(this DirectedMetaGraph graph, uint vertex1, uint vertex2, float weight, bool? direction, uint contractedId, out int added, out int removed)
    {
      bool flag1 = false;
      bool flag2 = true;
      int num1 = 0;
      DirectedMetaGraph.EdgeEnumerator edgeEnumerator1 = graph.GetEdgeEnumerator(vertex1);
      while (edgeEnumerator1.MoveNext())
      {
        if ((int) edgeEnumerator1.Neighbour == (int) vertex2)
        {
          ++num1;
          flag1 = true;
          if (ContractedEdgeDataSerializer.HasDirection(edgeEnumerator1.Data0, direction))
          {
            if ((double) weight < (double) ContractedEdgeDataSerializer.DeserializeWeight(edgeEnumerator1.Data0))
            {
              added = 1;
              removed = 1;
              return;
            }
            flag2 = false;
          }
        }
      }
      if (!flag1)
      {
        added = 1;
        removed = 0;
      }
      else if (flag2)
      {
        added = 0;
        removed = 0;
      }
      else
      {
        bool flag3 = false;
        float num2 = float.MaxValue;
        uint num3 = uint.MaxValue;
        bool flag4 = false;
        float num4 = float.MaxValue;
        uint num5 = uint.MaxValue;
        if (!direction.HasValue || direction.Value)
        {
          flag3 = true;
          num2 = weight;
          num3 = contractedId;
        }
        if (!direction.HasValue || !direction.Value)
        {
          flag4 = true;
          num4 = weight;
          num5 = contractedId;
        }
        DirectedMetaGraph.EdgeEnumerator edgeEnumerator2 = graph.GetEdgeEnumerator(vertex1);
        while (edgeEnumerator2.MoveNext())
        {
          if ((int) edgeEnumerator2.Neighbour == (int) vertex2)
          {
            float weight1;
            bool? direction1;
            uint contractedId1;
            ContractedEdgeDataSerializer.Deserialize(edgeEnumerator2.Data0, edgeEnumerator2.MetaData0, out weight1, out direction1, out contractedId1);
            if ((!direction1.HasValue || direction1.Value) && (double) weight1 < (double) num2)
            {
              num2 = weight1;
              flag3 = true;
              num3 = contractedId1;
            }
            if ((!direction1.HasValue || !direction1.Value) && (double) weight1 < (double) num4)
            {
              num4 = weight1;
              flag4 = true;
              num5 = contractedId1;
            }
          }
        }
        removed = num1;
        added = 0;
        if (flag3 & flag4 && (double) num2 == (double) num4 && (int) num3 == (int) num5)
        {
          added = added + 1;
        }
        else
        {
          if (flag3)
            added = added + 1;
          if (!flag4)
            return;
          added = added + 1;
        }
      }
    }
  }
}
