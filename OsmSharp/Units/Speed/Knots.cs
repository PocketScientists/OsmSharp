using OsmSharp.Units.Distance;
using OsmSharp.Units.Time;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OsmSharp.Units.Speed
{
  public class Knots : OsmSharp.Units.Speed.Speed
  {
    private const string RegexUnitKnots = "\\s*(knots)\\s*";

    public Knots(double value)
      : base(value)
    {
    }

    public static implicit operator Knots(double value)
    {
      return new Knots(value);
    }

    public static implicit operator Knots(MeterPerSecond meterPerSec)
    {
      return (Knots) (meterPerSec.Value / (463.0 / 900.0));
    }

    public static implicit operator Knots(KilometerPerHour knot)
    {
      return (Knots) (knot.Value / 1.852);
    }

    public static implicit operator Knots(MilesPerHour mph)
    {
      return (Knots) (mph.Value / 1.150779);
    }

    public static Kilometer operator *(Knots knot, Hour hour)
    {
      return (KilometerPerHour) knot * hour;
    }

    public static bool TryParse(string s, out Knots result)
    {
      result = (Knots) null;
      double result1;
      if (double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        result = new Knots(result1);
        return true;
      }
      Match match = new Regex("^\\s*(\\d+(?:\\.\\d*)?)\\s*\\s*(knots)\\s*$", RegexOptions.IgnoreCase).Match(s);
      if (!match.Success)
        return false;
      result = new Knots(double.Parse(match.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture));
      return true;
    }

    public override string ToString()
    {
      return this.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "knots";
    }
  }
}
