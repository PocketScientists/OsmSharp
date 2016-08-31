using OsmSharp.Collections.Tags;
using OsmSharp.Units.Distance;
using OsmSharp.Units.Speed;
using OsmSharp.Units.Weight;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace OsmSharp.Osm
{
  public static class TagExtensions
  {
    private static string[] BooleanTrueValues = new string[3]
    {
      "yes",
      "true",
      "1"
    };
    private static string[] BooleanFalseValues = new string[3]
    {
      "no",
      "false",
      "0"
    };
    private const string REGEX_DECIMAL = "\\s*(\\d+(?:\\.\\d*)?)\\s*";
    private const string REGEX_UNIT_TONNES = "\\s*(t|to|tonnes|tonnen)?\\s*";
    private const string REGEX_UNIT_METERS = "\\s*(m|meters|metres|meter)?\\s*";
    private const string REGEX_UNIT_KILOMETERS = "\\s*(km)?\\s*";

    public static bool IsTrue(this TagsCollectionBase tags, string tagKey)
    {
      string str;
      if (tags == null || string.IsNullOrWhiteSpace(tagKey) || !tags.TryGetValue(tagKey, out str))
        return false;
      return ((IEnumerable<string>) TagExtensions.BooleanTrueValues).Contains<string>(str.ToLowerInvariant());
    }

    public static bool IsFalse(this TagsCollectionBase tags, string tagKey)
    {
      string str;
      if (tags == null || string.IsNullOrWhiteSpace(tagKey) || !tags.TryGetValue(tagKey, out str))
        return false;
      return ((IEnumerable<string>) TagExtensions.BooleanFalseValues).Contains<string>(str.ToLowerInvariant());
    }

    public static string GetAccessTag(this TagsCollectionBase tags, IEnumerable<string> accessTagHierachy)
    {
      if (tags == null)
        return (string) null;
      foreach (string key in accessTagHierachy)
      {
        string str;
        if (tags.TryGetValue(key, out str))
          return str;
      }
      return (string) null;
    }

    public static bool TryGetMaxSpeed(this TagsCollectionBase tags, out KilometerPerHour result)
    {
      result = (KilometerPerHour) double.MaxValue;
      string s;
      if (tags == null || !tags.TryGetValue("maxspeed", out s) || (string.IsNullOrWhiteSpace(s) || s == "none") || (s == "signals" || s == "signs" || s == "no"))
        return false;
      return TagExtensions.TryParseSpeed(s, out result);
    }

    public static bool TryGetMaxWeight(this TagsCollectionBase tags, out Kilogram result)
    {
      result = (Kilogram) double.MaxValue;
      string s;
      if (tags == null || !tags.TryGetValue("maxweight", out s) || string.IsNullOrWhiteSpace(s))
        return false;
      return TagExtensions.TryParseWeight(s, out result);
    }

    public static bool TryGetMaxAxleLoad(this TagsCollectionBase tags, out Kilogram result)
    {
      result = (Kilogram) double.MaxValue;
      string s;
      if (tags == null || !tags.TryGetValue("maxaxleload", out s) || string.IsNullOrWhiteSpace(s))
        return false;
      return TagExtensions.TryParseWeight(s, out result);
    }

    public static bool TryGetMaxHeight(this TagsCollectionBase tags, out Meter result)
    {
      result = (Meter) double.MaxValue;
      string s;
      if (tags == null || !tags.TryGetValue("maxheight", out s) || string.IsNullOrWhiteSpace(s))
        return false;
      return TagExtensions.TryParseLength(s, out result);
    }

    public static bool TryGetMaxWidth(this TagsCollectionBase tags, out Meter result)
    {
      result = (Meter) double.MaxValue;
      string s;
      if (tags == null || !tags.TryGetValue("maxwidth", out s) || string.IsNullOrWhiteSpace(s))
        return false;
      return TagExtensions.TryParseLength(s, out result);
    }

    public static bool TryGetMaxLength(this IDictionary<string, string> tags, out Meter result)
    {
      result = (Meter) double.MaxValue;
      string s;
      if (tags == null || !tags.TryGetValue("maxlength", out s) || string.IsNullOrWhiteSpace(s))
        return false;
      return TagExtensions.TryParseLength(s, out result);
    }

    public static bool TryParseSpeed(string s, out KilometerPerHour result)
    {
      result = (KilometerPerHour) double.MaxValue;
      if (string.IsNullOrWhiteSpace(s) || (int) s[0] != 48 && (int) s[0] != 49 && ((int) s[0] != 50 && (int) s[0] != 51) && ((int) s[0] != 52 && (int) s[0] != 53 && ((int) s[0] != 54 && (int) s[0] != 55)) && ((int) s[0] != 56 && (int) s[0] != 57) || s.Contains(","))
        return false;
      double result1;
      if (double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
        result = (KilometerPerHour) result1;
      if (KilometerPerHour.TryParse(s, out result))
        return true;
      MilesPerHour result2;
      if (MilesPerHour.TryParse(s, out result2))
      {
        result = (KilometerPerHour) result2;
        return true;
      }
      Knots result3;
      if (!Knots.TryParse(s, out result3))
        return false;
      result = (KilometerPerHour) result3;
      return true;
    }

    public static bool TryParseWeight(string s, out Kilogram result)
    {
      result = (Kilogram) double.MaxValue;
      if (string.IsNullOrWhiteSpace(s))
        return false;
      Match match = new Regex("^\\s*(\\d+(?:\\.\\d*)?)\\s*\\s*(t|to|tonnes|tonnen)?\\s*$", RegexOptions.IgnoreCase).Match(s);
      if (!match.Success)
        return false;
      result = (Kilogram) (double.Parse(match.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture) * 1000.0);
      return true;
    }

    public static bool TryParseLength(string s, out Meter result)
    {
      result = (Meter) double.MaxValue;
      if (string.IsNullOrWhiteSpace(s))
        return false;
      Match match1 = new Regex("^\\s*(\\d+(?:\\.\\d*)?)\\s*\\s*(m|meters|metres|meter)?\\s*$", RegexOptions.IgnoreCase).Match(s);
      if (match1.Success)
      {
        result = (Meter) double.Parse(match1.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      Match match2 = new Regex("^(\\d+)\\'(\\d+)\\\"$", RegexOptions.IgnoreCase).Match(s);
      if (!match2.Success)
        return false;
      int num1 = int.Parse(match2.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture);
      int num2 = int.Parse(match2.Groups[2].Value, (IFormatProvider) CultureInfo.InvariantCulture);
      result = (Meter) ((double) num1 * 0.3048 + (double) num2 * 0.0254);
      return true;
    }
  }
}
