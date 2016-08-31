namespace OsmSharp.Routing.Graphs
{
  public class Edge
  {
    public uint Id { get; private set; }

    public uint From { get; private set; }

    public uint To { get; private set; }

    public bool DataInverted { get; private set; }

    public uint[] Data { get; private set; }

    internal Edge(uint id, uint from, uint to, uint[] data, bool edgeDataInverted)
    {
      this.Id = id;
      this.To = to;
      this.From = from;
      this.Data = data;
      this.DataInverted = edgeDataInverted;
    }

    internal Edge(Graph.EdgeEnumerator enumerator)
    {
      this.Id = enumerator.Id;
      this.To = enumerator.To;
      this.From = enumerator.From;
      this.DataInverted = enumerator.DataInverted;
      this.Data = enumerator.Data;
    }

    public override string ToString()
    {
      return string.Format("{0} - {1}", new object[2]
      {
        (object) this.To,
        (object) this.Data.ToInvariantString()
      });
    }
  }
}
