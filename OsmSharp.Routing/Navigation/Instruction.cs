namespace OsmSharp.Routing.Navigation
{
  public class Instruction
  {
    public string Type { get; set; }

    public string Text { get; set; }

    public int Segment { get; set; }

    public override string ToString()
    {
      return string.Format("[{0}] {1}", new object[2]
      {
        (object) this.Type,
        (object) this.Text
      });
    }
  }
}
