namespace OsmSharp.Routing.Profiles
{
  public struct Factor
  {
    public float Value { get; set; }

    public short Direction { get; set; }

    public static Factor NoFactor
    {
      get
      {
        return new Factor()
        {
          Direction = 0,
          Value = 0.0f
        };
      }
    }
  }
}
