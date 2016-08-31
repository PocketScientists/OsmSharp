using System;

namespace OsmSharp.Routing.Graphs
{
  public static class GraphExtensions
  {
    public static uint GetOther(this Edge edge, uint vertex)
    {
      if ((int) edge.From == (int) vertex)
        return edge.To;
      if ((int) edge.To == (int) vertex)
        return edge.From;
      throw new ArgumentOutOfRangeException(string.Format("Vertex {0} is not part of edge {1}.", new object[2]
      {
        (object) vertex,
        (object) edge.Id
      }));
    }
  }
}
