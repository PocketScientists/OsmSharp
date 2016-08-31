using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public class RouteMetric : ICloneable
  {
    public string Key { get; set; }

    public double Value { get; set; }

    public static RouteMetric[] ConvertFrom(IDictionary<string, double> tags)
    {
      List<RouteMetric> routeMetricList = new List<RouteMetric>();
      foreach (KeyValuePair<string, double> tag in (IEnumerable<KeyValuePair<string, double>>) tags)
        routeMetricList.Add(new RouteMetric()
        {
          Key = tag.Key,
          Value = tag.Value
        });
      return routeMetricList.ToArray();
    }

    public static List<KeyValuePair<string, double>> ConvertTo(RouteMetric[] tags)
    {
      List<KeyValuePair<string, double>> keyValuePairList = new List<KeyValuePair<string, double>>();
      if (tags != null)
      {
        foreach (RouteMetric tag in tags)
          keyValuePairList.Add(new KeyValuePair<string, double>(tag.Key, tag.Value));
      }
      return keyValuePairList;
    }

    public object Clone()
    {
      return (object) new RouteMetric()
      {
        Key = this.Key,
        Value = this.Value
      };
    }
  }
}
