using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OsmSharp.Units.Speed
{
  public class MilesPerHour : OsmSharp.Units.Speed.Speed
  {
    private const string RegexUnitMilesPerHour = "\\s*(mph)\\s*";

    private MilesPerHour()
      : base(0.0)
    {
    }

    public MilesPerHour(double value)
      : base(value)
    {
    }

    public static implicit operator MilesPerHour(double value)
    {
      return new MilesPerHour(value);
    }

    public static implicit operator MilesPerHour(KilometerPerHour kph)
    {
      return (MilesPerHour) (kph.Value * 0.621371192);
    }

    public static bool TryParse(string s, out MilesPerHour result)
    {
      result = (MilesPerHour) null;
      double result1;
      if (double.TryParse(s, out result1))
      {
        result = new MilesPerHour(result1);
        return true;
      }
      Match match = new Regex("^\\s*(\\d+(?:\\.\\d*)?)\\s*\\s*(mph)\\s*$", RegexOptions.IgnoreCase).Match(s);
      if (!match.Success)
        return false;
      result = new MilesPerHour(double.Parse(match.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture));
      return true;
    }

    public override string ToString()
    {
      return this.Value.ToString() + "mph";
    }
  }
}
