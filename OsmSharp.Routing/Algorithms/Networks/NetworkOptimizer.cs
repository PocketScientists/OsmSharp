using OsmSharp.Geo;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Network.Data;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Networks
{
  public class NetworkOptimizer : AlgorithmBase
  {
    private readonly RoutingNetwork _network;
    private readonly NetworkOptimizer.MergeDelegate _merge;

    public NetworkOptimizer(RoutingNetwork network, NetworkOptimizer.MergeDelegate merge)
    {
      this._network = network;
      this._merge = merge;
    }

    protected override void DoRun()
    {
      List<RoutingEdge> routingEdgeList = new List<RoutingEdge>();
      for (uint vertex = 0; vertex < this._network.VertexCount; ++vertex)
      {
        routingEdgeList.Clear();
        routingEdgeList.AddRange((IEnumerable<RoutingEdge>) this._network.GetEdgeEnumerator(vertex));
        EdgeData mergedEdgeData;
        bool mergedInverted;
        if (routingEdgeList.Count == 2 && (int) routingEdgeList[0].To != (int) routingEdgeList[1].To && (this._merge(routingEdgeList[0].Data, !routingEdgeList[0].DataInverted, routingEdgeList[1].Data, routingEdgeList[1].DataInverted, out mergedEdgeData, out mergedInverted) && (double) mergedEdgeData.Distance < (double) this._network.MaxEdgeDistance) && !this._network.ContainsEdge(routingEdgeList[0].To, routingEdgeList[1].To))
        {
          List<ICoordinate> coordinateList = new List<ICoordinate>();
          ShapeBase shape1 = routingEdgeList[0].Shape;
          if (shape1 != null)
          {
            if (!routingEdgeList[0].DataInverted)
              coordinateList.AddRange((IEnumerable<ICoordinate>) shape1.Reverse());
            else
              coordinateList.AddRange((IEnumerable<ICoordinate>) shape1);
          }
          coordinateList.Add((ICoordinate) this._network.GetVertex(vertex));
          ShapeBase shape2 = routingEdgeList[1].Shape;
          if (shape2 != null)
          {
            if (!routingEdgeList[1].DataInverted)
              coordinateList.AddRange((IEnumerable<ICoordinate>) shape2);
            else
              coordinateList.AddRange((IEnumerable<ICoordinate>) shape2.Reverse());
          }
          this._network.RemoveEdges(vertex);
          if (!mergedInverted)
          {
            int num1 = (int) this._network.AddEdge(routingEdgeList[0].To, routingEdgeList[1].To, mergedEdgeData, (IEnumerable<ICoordinate>) coordinateList);
          }
          else
          {
            coordinateList.Reverse();
            int num2 = (int) this._network.AddEdge(routingEdgeList[1].To, routingEdgeList[0].To, mergedEdgeData, (IEnumerable<ICoordinate>) coordinateList);
          }
        }
      }
    }

    public delegate bool MergeDelegate(EdgeData edgeData1, bool inverted1, EdgeData edgeData2, bool inverted2, out EdgeData mergedEdgeData, out bool mergedInverted);
  }
}
