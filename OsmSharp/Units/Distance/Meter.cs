using OsmSharp.Units.Speed;
using OsmSharp.Units.Time;

namespace OsmSharp.Units.Distance
{
  public class Meter : Unit
  {
    public Meter()
      : base(0.0)
    {
    }

    private Meter(double value)
      : base(value)
    {
    }

    public static implicit operator Meter(double value)
    {
      return new Meter(value);
    }

    public static implicit operator Meter(Kilometer kilometer)
    {
      return (Meter) (kilometer.Value * 1000.0);
    }

    public static MeterPerSecond operator /(Meter meter, Second sec)
    {
      return (MeterPerSecond) (meter.Value / sec.Value);
    }

    public static Second operator /(Meter distance, MeterPerSecond speed)
    {
      return (Second) (distance.Value / speed.Value);
    }

    public static Second operator /(Meter distance, KilometerPerHour speed)
    {
      return (Second) ((Kilometer) distance / speed);
    }

    public static Meter operator +(Meter meter1, Meter meter2)
    {
      return (Meter) (meter1.Value + meter2.Value);
    }

    public static Meter operator -(Meter meter1, Meter meter2)
    {
      return (Meter) (meter1.Value - meter2.Value);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "m";
    }
  }
}
