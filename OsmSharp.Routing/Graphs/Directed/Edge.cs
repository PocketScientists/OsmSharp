namespace OsmSharp.Routing.Graphs.Directed
{
  public class Edge
  {
    public uint Neighbour { get; set; }

    public uint[] Data { get; set; }

    public uint Id { get; set; }

    internal Edge(DirectedGraph.EdgeEnumerator enumerator)
    {
      this.Neighbour = enumerator.Neighbour;
      this.Data = enumerator.Data;
      this.Id = enumerator.Id;
    }

    internal Edge(uint id, uint neighbour, uint[] data)
    {
      this.Neighbour = neighbour;
      this.Data = data;
      this.Id = id;
    }

    public override string ToString()
    {
      if (this.Data != null)
      {
        string str = "[" + this.Data[0].ToInvariantString();
        for (int index = 1; index < this.Data.Length; ++index)
          str = str + ", " + this.Data[index].ToInvariantString();
        return string.Format("{0} - {1} [{2}]", (object) this.Neighbour, (object) this.Id, (object) str);
      }
      return string.Format("{0} - {1} []", new object[2]
      {
        (object) this.Neighbour,
        (object) this.Id
      });
    }
  }
}
