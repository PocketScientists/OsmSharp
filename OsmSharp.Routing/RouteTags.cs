namespace OsmSharp.Routing
{
  public class RouteTags : ICloneable
  {
    public string Key { get; set; }

    public string Value { get; set; }

    public object Clone()
    {
      return (object) new RouteTags()
      {
        Key = this.Key,
        Value = this.Value
      };
    }

    public override string ToString()
    {
      return string.Format("{0}={1}", new object[2]
      {
        (object) this.Key,
        (object) this.Value
      });
    }
  }
}
