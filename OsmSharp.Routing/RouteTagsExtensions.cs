using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public static class RouteTagsExtensions
  {
    public static RouteTags[] ConvertFrom(this TagsCollectionBase tags)
    {
      List<RouteTags> routeTagsList = new List<RouteTags>();
      foreach (Tag tag in tags)
        routeTagsList.Add(new RouteTags()
        {
          Key = tag.Key,
          Value = tag.Value
        });
      return routeTagsList.ToArray();
    }

    public static TagsCollectionBase ConvertToTagsCollection(this RouteTags[] tags)
    {
      TagsCollection tagsCollection = new TagsCollection();
      if (tags != null)
      {
        foreach (RouteTags tag in tags)
          tagsCollection.Add(new Tag(tag.Key, tag.Value));
      }
      return (TagsCollectionBase) tagsCollection;
    }

    public static RouteTags[] ConvertFrom(this IDictionary<string, string> tags)
    {
      List<RouteTags> routeTagsList = new List<RouteTags>();
      foreach (KeyValuePair<string, string> tag in (IEnumerable<KeyValuePair<string, string>>) tags)
        routeTagsList.Add(new RouteTags()
        {
          Key = tag.Key,
          Value = tag.Value
        });
      return routeTagsList.ToArray();
    }

    public static RouteTags[] ConvertFrom(this List<KeyValuePair<string, string>> tags)
    {
      List<RouteTags> routeTagsList = new List<RouteTags>();
      if (tags != null)
      {
        foreach (KeyValuePair<string, string> tag in tags)
          routeTagsList.Add(new RouteTags()
          {
            Key = tag.Key,
            Value = tag.Value
          });
      }
      return routeTagsList.ToArray();
    }

    public static List<KeyValuePair<string, string>> ConvertTo(this RouteTags[] tags)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tags != null)
      {
        foreach (RouteTags tag in tags)
          keyValuePairList.Add(new KeyValuePair<string, string>(tag.Key, tag.Value));
      }
      return keyValuePairList;
    }

    public static string GetValueFirst(this RouteTags[] tags, string key)
    {
      string str = (string) null;
      if (tags != null)
      {
        foreach (RouteTags tag in tags)
        {
          if (tag.Key == key)
          {
            str = tag.Value;
            break;
          }
        }
      }
      return str;
    }

    public static List<string> GetValues(this RouteTags[] tags, string key)
    {
      List<string> stringList = new List<string>();
      if (tags != null)
      {
        foreach (RouteTags tag in tags)
        {
          if (tag.Key == key)
            stringList.Add(tag.Value);
        }
      }
      return stringList;
    }

    public static void AddOrReplace(ref RouteTags[] tags, RouteTags[] other)
    {
      List<RouteTags> routeTagsList = (List<RouteTags>) null;
      foreach (RouteTags routeTags in other)
      {
        bool flag = false;
        for (int index = 0; index < tags.Length; ++index)
        {
          if (tags[index].Key == routeTags.Key)
          {
            tags[index].Value = routeTags.Value;
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          if (routeTagsList == null)
            routeTagsList = new List<RouteTags>();
          routeTagsList.Add(routeTags);
        }
      }
      if (routeTagsList == null)
        return;
      Array.Resize<RouteTags>(ref tags, tags.Length + routeTagsList.Count);
      for (int index = tags.Length - routeTagsList.Count; index < tags.Length; ++index)
        tags[index] = routeTagsList[index - tags.Length + routeTagsList.Count];
    }
  }
}
