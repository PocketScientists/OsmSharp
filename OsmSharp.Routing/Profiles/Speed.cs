namespace OsmSharp.Routing.Profiles
{
  public struct Speed
  {
    public float Value { get; set; }

    public short Direction { get; set; }

    public static Speed NoSpeed
    {
      get
      {
        return new Speed()
        {
          Direction = 0,
          Value = 0.0f
        };
      }
    }
  }
}
