using OsmSharp.Units.Speed;
using OsmSharp.Units.Time;

namespace OsmSharp.Units.Distance
{
  public class Kilometer : Unit
  {
    public Kilometer()
      : base(0.0)
    {
    }

    private Kilometer(double value)
      : base(value)
    {
    }

    public static implicit operator Kilometer(double value)
    {
      return new Kilometer(value);
    }

    public static implicit operator Kilometer(Meter meter)
    {
      return (Kilometer) (meter.Value / 1000.0);
    }

    public static Hour operator /(Kilometer distance, KilometerPerHour speed)
    {
      return (Hour) (distance.Value / speed.Value);
    }

    public static KilometerPerHour operator /(Kilometer kilometer, Hour hour)
    {
      return (KilometerPerHour) (kilometer.Value / hour.Value);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "Km";
    }
  }
}
