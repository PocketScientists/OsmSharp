namespace OsmSharp.Geo.Attributes
{
  public class GeometryAttribute
  {
    public string Key { get; set; }

    public object Value { get; set; }

    public override string ToString()
    {
      if (this.Key != null)
      {
        if (this.Value != null)
          return string.Format("{0}={1}", new object[2]
          {
            (object) this.Key,
            (object) this.Value.ToString()
          });
        return string.Format("{0}=null", (object) this.Key);
      }
      if (this.Value == null)
        return "null=null";
      return string.Format("null={0}", (object) this.Value.ToString());
    }
  }
}
