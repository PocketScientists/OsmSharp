using OsmSharp.Collections.Tags;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Vehicles
{
  public static class VehicleExtensions
  {
    private static Dictionary<string, bool?> _accessValues;

    public static bool AnyCanTraverse(this Vehicle[] vehicles, TagsCollectionBase tags)
    {
      for (int index = 0; index < vehicles.Length; ++index)
      {
        if (vehicles[index].CanTraverse(tags))
          return true;
      }
      return false;
    }

    public static bool IsRelevantForMeta(this Vehicle[] vehicles, string key)
    {
      if (string.IsNullOrEmpty(key))
        return false;
      for (int index = 0; index < vehicles.Length; ++index)
      {
        if (vehicles[index].IsRelevantForMeta(key))
          return true;
      }
      return false;
    }

    public static bool IsRelevantForProfile(this Vehicle[] vehicles, string key)
    {
      if (string.IsNullOrEmpty(key))
        return false;
      for (int index = 0; index < vehicles.Length; ++index)
      {
        if (vehicles[index].IsRelevantForProfile(key))
          return true;
      }
      return false;
    }

    private static Dictionary<string, bool?> GetAccessValues()
    {
      if (VehicleExtensions._accessValues == null)
      {
        VehicleExtensions._accessValues = new Dictionary<string, bool?>();
        VehicleExtensions._accessValues.Add("private", new bool?(false));
        VehicleExtensions._accessValues.Add("yes", new bool?(true));
        VehicleExtensions._accessValues.Add("no", new bool?(false));
        VehicleExtensions._accessValues.Add("permissive", new bool?(true));
        VehicleExtensions._accessValues.Add("destination", new bool?(true));
        VehicleExtensions._accessValues.Add("customers", new bool?(false));
        VehicleExtensions._accessValues.Add("agricultural", new bool?());
        VehicleExtensions._accessValues.Add("forestry", new bool?());
        VehicleExtensions._accessValues.Add("designated", new bool?(true));
        VehicleExtensions._accessValues.Add("public", new bool?(true));
        VehicleExtensions._accessValues.Add("discouraged", new bool?());
        VehicleExtensions._accessValues.Add("delivery", new bool?(true));
        VehicleExtensions._accessValues.Add("use_sidepath", new bool?(false));
      }
      return VehicleExtensions._accessValues;
    }

    public static bool? InterpretAccessValue(this TagsCollectionBase tags, string key)
    {
      string key1;
      bool? nullable;
      if (tags.TryGetValue(key, out key1) && VehicleExtensions.GetAccessValues().TryGetValue(key1, out nullable))
        return nullable;
      return new bool?();
    }

    public static bool InterpretAccessValues(this TagsCollectionBase tags, IEnumerable<string> keys, params string[] rootKeys)
    {
      bool? nullable1 = new bool?();
      for (int index = 0; index < rootKeys.Length; ++index)
      {
        bool? nullable2 = tags.InterpretAccessValue(rootKeys[index]);
        if (nullable2.HasValue)
          nullable1 = nullable2;
      }
      foreach (string key in keys)
      {
        bool? nullable2 = tags.InterpretAccessValue(key);
        if (nullable2.HasValue)
          nullable1 = nullable2;
      }
      if (nullable1.HasValue)
        return nullable1.Value;
      return true;
    }
  }
}
