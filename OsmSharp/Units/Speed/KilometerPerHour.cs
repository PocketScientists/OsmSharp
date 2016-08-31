using OsmSharp.Units.Distance;
using OsmSharp.Units.Time;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OsmSharp.Units.Speed
{
  public class KilometerPerHour : OsmSharp.Units.Speed.Speed
  {
    private const string RegexUnitKilometersPerHour = "\\s*(km/h|kmh|kph|kmph)?\\s*";

    public KilometerPerHour(double value)
      : base(value)
    {
    }

    public static implicit operator KilometerPerHour(double value)
    {
      return new KilometerPerHour(value);
    }

    public static implicit operator KilometerPerHour(MeterPerSecond meterPerSec)
    {
      return (KilometerPerHour) (meterPerSec.Value * 3.6);
    }

    public static implicit operator KilometerPerHour(Knots knot)
    {
      return (KilometerPerHour) (knot.Value * 1.852);
    }

    public static implicit operator KilometerPerHour(MilesPerHour mph)
    {
      return (KilometerPerHour) (mph.Value / 0.621371192);
    }

    public static Kilometer operator *(KilometerPerHour kilometerPerHour, Hour hour)
    {
      return (Kilometer) (kilometerPerHour.Value * hour.Value);
    }

    public static bool TryParse(string s, out KilometerPerHour result)
    {
      s = s.ToStringEmptyWhenNull().Trim().ToLower();
      result = (KilometerPerHour) null;
      double result1;
      if (double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        result = new KilometerPerHour(result1);
        return true;
      }
      Match match = new Regex("^\\s*(\\d+(?:\\.\\d*)?)\\s*\\s*(km/h|kmh|kph|kmph)?\\s*$", RegexOptions.IgnoreCase).Match(s);
      if (!match.Success)
        return false;
      result = new KilometerPerHour(double.Parse(match.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture));
      return true;
    }

    public override string ToString()
    {
      return this.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "Km/h";
    }
  }
}
