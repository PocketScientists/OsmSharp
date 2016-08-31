using OsmSharp.Routing.Graphs.Geometric.Shapes;

namespace OsmSharp.Routing.Graphs.Geometric
{
  public class GeometricEdge
  {
    public uint Id { get; private set; }

    public uint From { get; private set; }

    public uint To { get; private set; }

    public bool DataInverted { get; private set; }

    public uint[] Data { get; private set; }

    public ShapeBase Shape { get; private set; }

    public GeometricEdge(uint id, uint from, uint to, uint[] data, bool edgeDataInverted, ShapeBase shape)
    {
      this.Id = id;
      this.From = from;
      this.To = to;
      this.Data = data;
      this.DataInverted = edgeDataInverted;
      this.Shape = shape;
    }

    internal GeometricEdge(GeometricGraph.EdgeEnumerator enumerator)
    {
      this.Id = enumerator.Id;
      this.From = enumerator.From;
      this.To = enumerator.To;
      this.Data = enumerator.Data;
      this.DataInverted = enumerator.DataInverted;
      this.Shape = enumerator.Shape;
    }
  }
}
