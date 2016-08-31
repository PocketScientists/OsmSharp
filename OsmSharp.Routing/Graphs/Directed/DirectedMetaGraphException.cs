using System;

namespace OsmSharp.Routing.Graphs.Directed
{
  public static class DirectedMetaGraphException
  {
    public static MetaEdge GetShortestEdge(this DirectedMetaGraph graph, uint vertex1, uint vertex2, Func<uint[], float?> getWeight)
    {
      float maxValue = float.MaxValue;
      DirectedMetaGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator(vertex1);
      MetaEdge metaEdge = (MetaEdge) null;
      while (edgeEnumerator.MoveNext())
      {
        if ((int) edgeEnumerator.Neighbour == (int) vertex2)
        {
          float? nullable = getWeight(edgeEnumerator.Data);
          if (nullable.HasValue && (double) nullable.Value < (double) maxValue)
            metaEdge = edgeEnumerator.Current;
        }
      }
      return metaEdge;
    }
  }
}
