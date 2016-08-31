using OsmSharp.Units.Distance;
using OsmSharp.Units.Time;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OsmSharp.Units.Speed
{
  public class MeterPerSecond : OsmSharp.Units.Speed.Speed
  {
    private const string RegexUnitMetersPerSecond = "\\s*(m/s)?\\s*";

    private MeterPerSecond(double value)
      : base(value)
    {
    }

    public static implicit operator MeterPerSecond(double value)
    {
      return new MeterPerSecond(value);
    }

    public static implicit operator MeterPerSecond(KilometerPerHour kph)
    {
      return (MeterPerSecond) (kph.Value / 3.6);
    }

    public static implicit operator MeterPerSecond(Knots knot)
    {
      return (MeterPerSecond) (knot.Value * 1.852 / 3.6);
    }

    public static implicit operator MeterPerSecond(MilesPerHour mph)
    {
      return (MeterPerSecond) (mph.Value * 1.609344 / 3.6);
    }

    public static Meter operator *(MeterPerSecond meterPerSecond, Second second)
    {
      return (Meter) (meterPerSecond.Value * second.Value);
    }

    public static bool TryParse(string s, out MeterPerSecond result)
    {
      result = (MeterPerSecond) null;
      double result1;
      if (double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        result = new MeterPerSecond(result1);
        return true;
      }
      Match match = new Regex("^\\s*(\\d+(?:\\.\\d*)?)\\s*\\s*(m/s)?\\s*$", RegexOptions.IgnoreCase).Match(s);
      if (!match.Success)
        return false;
      result = new MeterPerSecond(double.Parse(match.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture));
      return true;
    }

    public override string ToString()
    {
      return this.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "m/s";
    }
  }
}
