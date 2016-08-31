namespace OsmSharp.Routing.Graphs.Directed
{
  public class MetaEdge
  {
    public uint Neighbour { get; set; }

    public uint[] Data { get; set; }

    public uint[] MetaData { get; set; }

    public uint Id { get; set; }

    internal MetaEdge(DirectedMetaGraph.EdgeEnumerator enumerator)
    {
      this.Neighbour = enumerator.Neighbour;
      this.Data = enumerator.Data;
      this.MetaData = enumerator.MetaData;
      this.Id = enumerator.Id;
    }

    public override string ToString()
    {
      return string.Format("{0} - {1}", new object[2]
      {
        (object) this.Neighbour,
        (object) this.Data.ToInvariantString()
      });
    }
  }
}
