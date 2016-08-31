namespace OsmSharp.Units.Speed
{
  public abstract class Speed : Unit
  {
    internal Speed(double value)
      : base(value)
    {
    }

    public static bool TryParse(string s, out OsmSharp.Units.Speed.Speed result)
    {
      result = (OsmSharp.Units.Speed.Speed) null;
      if (string.IsNullOrWhiteSpace(s))
        return false;
      double result1;
      if (double.TryParse(s, out result1))
      {
        result = (OsmSharp.Units.Speed.Speed) new KilometerPerHour(result1);
        return true;
      }
      return OsmSharp.Units.Speed.Speed.TryParse(s, out result) || OsmSharp.Units.Speed.Speed.TryParse(s, out result) || (OsmSharp.Units.Speed.Speed.TryParse(s, out result) || OsmSharp.Units.Speed.Speed.TryParse(s, out result));
    }
  }
}
