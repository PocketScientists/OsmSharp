using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Attributes;
using OsmSharp.Routing.Osm.Vehicles;

namespace OsmSharp.Routing.Osm
{
  public static class HighwayComparer
  {
    public static bool CompareOpposite(AttributesIndex tags, uint tags1, uint tags2)
    {
      TagsCollectionBase tags3 = tags.Get(tags1);
      TagsCollectionBase tags4 = tags.Get(tags2);
      bool? nullable1 = Vehicle.Car.IsOneWay(tags3);
      bool? nullable2 = Vehicle.Car.IsOneWay(tags4);
      if (nullable1.HasValue && nullable2.HasValue && nullable1.Value == nullable2.Value)
        return false;
      foreach (Tag tag in tags3)
      {
        if (tag.Key != "oneway" && !tags4.ContainsKeyValue(tag.Key, tag.Value))
          return false;
      }
      foreach (Tag tag in tags4)
      {
        if (tag.Key != "oneway" && !tags3.ContainsKeyValue(tag.Key, tag.Value))
          return false;
      }
      return true;
    }

    public static bool Compare(AttributesIndex tags, uint tags1, uint tags2)
    {
      TagsCollectionBase tags3 = tags.Get(tags1);
      TagsCollectionBase tags4 = tags.Get(tags2);
      bool? nullable1 = Vehicle.Car.IsOneWay(tags3);
      bool? nullable2 = Vehicle.Car.IsOneWay(tags4);
      if (nullable1.HasValue && nullable2.HasValue && nullable1.Value != nullable2.Value)
        return false;
      foreach (Tag tag in tags3)
      {
        if (tag.Key != "oneway" && !tags4.ContainsKeyValue(tag.Key, tag.Value))
          return false;
      }
      foreach (Tag tag in tags4)
      {
        if (tag.Key != "oneway" && !tags3.ContainsKeyValue(tag.Key, tag.Value))
          return false;
      }
      return true;
    }
  }
}
