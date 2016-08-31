using System;

namespace OsmSharp.Units.Time
{
  public class Hour : Unit
  {
    public Hour()
      : base(0.0)
    {
    }

    private Hour(double value)
      : base(value)
    {
    }

    public static implicit operator Hour(double value)
    {
      return new Hour(value);
    }

    public static implicit operator Hour(TimeSpan timespan)
    {
      Hour hour = new Hour();
      return (Hour) (timespan.TotalMilliseconds * 1000.0 * 3600.0);
    }

    public static implicit operator Hour(Second sec)
    {
      Hour hour = new Hour();
      return (Hour) (sec.Value / 3600.0);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "H";
    }
  }
}
