using OsmSharp.Routing.Network;
using OsmSharp.Routing.Network.Data;

namespace OsmSharp.Routing.Algorithms.Networks
{
  public class ZeroLengthLinksOptimizer : AlgorithmBase
  {
    private readonly RoutingNetwork _network;
    private readonly ZeroLengthLinksOptimizer.CanRemoveDelegate _canRemove;

    public ZeroLengthLinksOptimizer(RoutingNetwork network, ZeroLengthLinksOptimizer.CanRemoveDelegate canRemove)
    {
      this._network = network;
      this._canRemove = canRemove;
    }

    protected override void DoRun()
    {
      RoutingNetwork.EdgeEnumerator edgeEnumerator = this._network.GetEdgeEnumerator();
      for (uint vertex = 0; vertex < this._network.VertexCount; ++vertex)
      {
        edgeEnumerator.MoveTo(vertex);
        while (edgeEnumerator.MoveNext())
        {
          EdgeData data = edgeEnumerator.Data;
          if ((double) data.Distance == 0.0 && this._canRemove(data))
          {
            uint from = edgeEnumerator.From;
            uint to = edgeEnumerator.To;
            this._network.RemoveEdge(edgeEnumerator.Id);
            this._network.MergeVertices(from, to);
            --vertex;
            break;
          }
        }
      }
    }

    public delegate bool CanRemoveDelegate(EdgeData edge);
  }
}
