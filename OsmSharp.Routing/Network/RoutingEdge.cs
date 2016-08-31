using OsmSharp.Routing.Graphs.Geometric.Shapes;
using OsmSharp.Routing.Network.Data;

namespace OsmSharp.Routing.Network
{
  public class RoutingEdge
  {
    public uint Id { get; private set; }

    public uint From { get; private set; }

    public uint To { get; private set; }

    public EdgeData Data { get; private set; }

    public bool DataInverted { get; set; }

    public ShapeBase Shape { get; private set; }

    internal RoutingEdge(uint id, uint from, uint to, EdgeData data, bool edgeDataInverted, ShapeBase shape)
    {
      this.Id = id;
      this.To = to;
      this.From = from;
      this.Data = data;
      this.DataInverted = edgeDataInverted;
      this.Shape = shape;
    }

    internal RoutingEdge(RoutingNetwork.EdgeEnumerator enumerator)
    {
      this.Id = enumerator.Id;
      this.To = enumerator.To;
      this.From = enumerator.From;
      this.Data = enumerator.Data;
      this.DataInverted = enumerator.DataInverted;
      this.Shape = enumerator.Shape;
    }
  }
}
