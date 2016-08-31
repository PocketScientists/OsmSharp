using System;

namespace OsmSharp.Units.Time
{
  public class Second : Unit
  {
    public Second()
      : base(0.0)
    {
    }

    private Second(double value)
      : base(value)
    {
    }

    public static implicit operator Second(double value)
    {
      return new Second(value);
    }

    public static implicit operator Second(TimeSpan timespan)
    {
      Second second = new Second();
      return (Second) (timespan.TotalMilliseconds / 1000.0);
    }

    public static implicit operator Second(Hour hour)
    {
      Second second = new Second();
      return (Second) (hour.Value * 3600.0);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "s";
    }
  }
}
