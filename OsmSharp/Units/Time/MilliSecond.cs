using System;

namespace OsmSharp.Units.Time
{
  public class MilliSecond : Unit
  {
    public MilliSecond()
      : base(0.0)
    {
    }

    private MilliSecond(double value)
      : base(value)
    {
    }

    public static implicit operator MilliSecond(double value)
    {
      return new MilliSecond(value);
    }

    public static implicit operator MilliSecond(TimeSpan timespan)
    {
      MilliSecond milliSecond = new MilliSecond();
      return (MilliSecond) timespan.TotalMilliseconds;
    }

    public static implicit operator MilliSecond(Hour hour)
    {
      MilliSecond milliSecond = new MilliSecond();
      return (MilliSecond) (hour.Value * 3600.0 * 1000.0);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "ms";
    }
  }
}
