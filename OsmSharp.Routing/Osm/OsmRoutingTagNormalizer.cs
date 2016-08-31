using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Osm.Vehicles;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm
{
  public static class OsmRoutingTagNormalizer
  {
    private static Dictionary<string, bool> _onewayValues;
    private static Dictionary<string, bool?> _rampValues;

    public static Dictionary<string, bool> OnewayValues
    {
      get
      {
        if (OsmRoutingTagNormalizer._onewayValues == null)
        {
          OsmRoutingTagNormalizer._onewayValues = new Dictionary<string, bool>();
          OsmRoutingTagNormalizer._onewayValues.Add("yes", true);
          OsmRoutingTagNormalizer._onewayValues.Add("-1", false);
          OsmRoutingTagNormalizer._onewayValues.Add("1", true);
        }
        return OsmRoutingTagNormalizer._onewayValues;
      }
    }

    public static Dictionary<string, bool?> RampValues
    {
      get
      {
        if (OsmRoutingTagNormalizer._rampValues == null)
        {
          OsmRoutingTagNormalizer._rampValues = new Dictionary<string, bool?>();
          OsmRoutingTagNormalizer._rampValues.Add("yes", new bool?());
        }
        return OsmRoutingTagNormalizer._rampValues;
      }
    }

    public static bool Normalize(this TagsCollection tags, TagsCollection profileTags, TagsCollection metaTags, IEnumerable<Vehicle> vehicles)
    {
      string str;
      if (!tags.TryGetValue("highway", out str))
        return false;
      tags.NormalizeMaxspeed(profileTags, metaTags);
      tags.NormalizeOneway(profileTags, metaTags);
      tags.NormalizeOnewayBicycle(profileTags, metaTags);
      tags.NormalizeJunction(profileTags, metaTags);
      // ISSUE: reference to a compiler-generated method
      long stringHash = str.GetHashCode();
      if (stringHash <= 2067622972U)
      {
        if (stringHash <= 841786498U)
        {
          if (stringHash <= 155541048U)
          {
            if ((int) stringHash != 4126140)
            {
              if ((int) stringHash != 155541048 || !(str == "footway"))
                goto label_50;
            }
            else if (!(str == "pedestrian"))
              goto label_50;
          }
          else if ((int) stringHash != 410259268)
          {
            if ((int) stringHash != 741716276)
            {
              if ((int) stringHash != 841786498 || !(str == "secondary_link"))
                goto label_50;
              else
                goto label_46;
            }
            else if (!(str == "steps"))
              goto label_50;
          }
          else if (str == "tertiary_link")
            goto label_46;
          else
            goto label_50;
          tags.NormalizeRamp(profileTags, metaTags, false);
          profileTags.Add("highway", str);
          goto label_50;
        }
        else if (stringHash <= 1266453457U)
        {
          if ((int) stringHash != 908164533)
          {
            if ((int) stringHash != 1025494171)
            {
              if ((int) stringHash != 1266453457 || !(str == "secondary"))
                goto label_50;
              else
                goto label_46;
            }
            else if (str == "road")
              goto label_46;
            else
              goto label_50;
          }
          else if (str == "residential")
            goto label_46;
          else
            goto label_50;
        }
        else if ((int) stringHash != 1457512036)
        {
          if ((int) stringHash != 1512988633)
          {
            if ((int) stringHash != 2067622972 || !(str == "track"))
              goto label_50;
            else
              goto label_46;
          }
          else if (!(str == "primary"))
            goto label_50;
        }
        else if (str == "service")
          goto label_46;
        else
          goto label_50;
      }
      else if (stringHash <= 3245824490U)
      {
        if (stringHash <= 2772881565U)
        {
          if ((int) stringHash != -2071507658)
          {
            if ((int) stringHash != -1522085731 || !(str == "unclassified"))
              goto label_50;
            else
              goto label_46;
          }
          else if (str == "path")
          {
            profileTags.Add("highway", str);
            goto label_50;
          }
          else
            goto label_50;
        }
        else if ((int) stringHash != -1306006370)
        {
          if ((int) stringHash != -1249381910)
          {
            if ((int) stringHash != -1049142806 || !(str == "motorway_link"))
              goto label_50;
          }
          else if (!(str == "primary_link"))
            goto label_50;
        }
        else if (str == "living_street")
          goto label_46;
        else
          goto label_50;
      }
      else if (stringHash <= 3647643189U)
      {
        if ((int) stringHash != -705505319)
        {
          if ((int) stringHash != -700661456)
          {
            if ((int) stringHash != -647324107 || !(str == "services"))
              goto label_50;
            else
              goto label_46;
          }
          else if (str == "cycleway")
          {
            profileTags.Add("highway", str);
            goto label_50;
          }
          else
            goto label_50;
        }
        else if (!(str == "motorway"))
          goto label_50;
      }
      else if ((int) stringHash != -581013891)
      {
        if ((int) stringHash != -424251197)
        {
          if ((int) stringHash != -37246338 || !(str == "trunk_link"))
            goto label_50;
        }
        else if (str == "tertiary")
          goto label_46;
        else
          goto label_50;
      }
      else if (!(str == "trunk"))
        goto label_50;
      profileTags.Add("highway", str);
      goto label_50;
label_46:
      profileTags.Add("highway", str);
label_50:
      foreach (Vehicle vehicle in vehicles)
        tags.NormalizeAccess(vehicle, str, profileTags);
      return true;
    }

    public static void NormalizeAccess(this TagsCollection tags, Vehicle vehicle, string highwayType, TagsCollection profileTags)
    {
      bool defaultAccess = vehicle.CanTraverse((TagsCollectionBase) new TagsCollection(new Tag[1]
      {
        Tag.Create("highway", highwayType)
      }));
      tags.NormalizeAccess(profileTags, defaultAccess, vehicle.VehicleTypes.ToArray());
    }

    public static void NormalizeAccess(this TagsCollection tags, TagsCollection profileTags, bool defaultAccess, params string[] accessTags)
    {
      bool? nullable1 = tags.InterpretAccessValue("access");
      for (int index = 0; index < accessTags.Length; ++index)
      {
        bool? nullable2 = tags.InterpretAccessValue(accessTags[index]);
        if (nullable2.HasValue)
          nullable1 = nullable2;
      }
      if (!nullable1.HasValue || nullable1.Value == defaultAccess)
        return;
      if (nullable1.Value)
        profileTags.Add(accessTags[accessTags.Length - 1], "yes");
      else
        profileTags.Add(accessTags[accessTags.Length - 1], "no");
    }

    public static void NormalizeOneway(this TagsCollection tags, TagsCollection profileTags, TagsCollection metaTags)
    {
      string key;
      bool flag;
      if (!tags.TryGetValue("oneway", out key) || !OsmRoutingTagNormalizer.OnewayValues.TryGetValue(key, out flag))
        return;
      if (flag)
        profileTags.Add("oneway", "yes");
      else
        profileTags.Add("oneway", "-1");
    }

    public static void NormalizeOnewayBicycle(this TagsCollection tags, TagsCollection profileTags, TagsCollection metaTags)
    {
      string str;
      if (!tags.TryGetValue("oneway:bicycle", out str) || !(str == "no"))
        return;
      profileTags.Add("oneway:bicycle", "no");
    }

    public static void NormalizeMaxspeed(this TagsCollection tags, TagsCollection profileTags, TagsCollection metaTags)
    {
      string s;
      if (!tags.TryGetValue("maxspeed", out s))
        return;
      int result;
      if (int.TryParse(s, out result) && result > 0 && result <= 200)
      {
        profileTags.Add("maxspeed", s);
      }
      else
      {
        if (!s.EndsWith("mph") || !int.TryParse(s.Substring(0, s.Length - 4), out result) || (result <= 0 || result > 150))
          return;
        profileTags.Add("maxspeed", s);
      }
    }

    public static void NormalizeJunction(this TagsCollection tags, TagsCollection profileTags, TagsCollection metaTags)
    {
      string str;
      if (!tags.TryGetValue("junction", out str) || !(str == "roundabout"))
        return;
      profileTags.Add("junction", "roundabout");
    }

    public static void NormalizeRamp(this TagsCollection tags, TagsCollection profileTags, TagsCollection metaTags, bool defaultAccess)
    {
      string key;
      bool? nullable1;
      if (!tags.TryGetValue("ramp", out key) || !OsmRoutingTagNormalizer.RampValues.TryGetValue(key, out nullable1))
        return;
      int num1 = defaultAccess ? 1 : 0;
      bool? nullable2 = nullable1;
      int num2 = nullable2.GetValueOrDefault() ? 1 : 0;
      if ((num1 == num2 ? (!nullable2.HasValue ? 1 : 0) : 1) == 0)
        return;
      profileTags.Add("ramp", key);
    }
  }
}
