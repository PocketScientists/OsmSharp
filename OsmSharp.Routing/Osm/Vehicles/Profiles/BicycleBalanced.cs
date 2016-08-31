using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Profiles;
using System;

namespace OsmSharp.Routing.Osm.Vehicles.Profiles
{
  internal class BicycleBalanced : Profile
  {
    private const float HIGHEST_AVOID_FACTOR = 0.8f;
    private const float AVOID_FACTOR = 0.9f;
    private const float PREFER_FACTOR = 1.1f;
    private const float HIGHEST_PREFER_FACTOR = 1.2f;

    internal BicycleBalanced(Bicycle bicycle)
      : base(bicycle.UniqueName + ".Balanced", bicycle.GetGetSpeed(), bicycle.GetGetMinSpeed(), bicycle.GetCanStop(), bicycle.GetEquals(), bicycle.VehicleTypes, BicycleBalanced.InternalGetFactor(bicycle))
    {
    }

    private static Func<TagsCollectionBase, Factor> InternalGetFactor(Bicycle bicycle)
    {
      bicycle.GetGetFactor();
      Func<TagsCollectionBase, Speed> getSpeedDefault = bicycle.GetGetSpeed();
      return (Func<TagsCollectionBase, Factor>) (tags =>
      {
        Speed speed = getSpeedDefault(tags);
        if ((double) speed.Value == 0.0)
          return new Factor()
          {
            Value = 0.0f,
            Direction = 0
          };
        string s;
        if (tags.TryGetValue("highway", out s))
        {
              long stringHash = s.GetHashCode();
          if (stringHash <= 1266453457U)
          {
            if (stringHash <= 410259268U)
            {
              if ((int) stringHash != 4126140)
              {
                if ((int) stringHash != 155541048)
                {
                  if ((int) stringHash != 410259268 || !(s == "tertiary_link"))
                    goto label_34;
                  else
                    goto label_31;
                }
                else if (s == "footway")
                  goto label_33;
                else
                  goto label_34;
              }
              else if (s == "pedestrian")
                goto label_33;
              else
                goto label_34;
            }
            else if (stringHash <= 841786498U)
            {
              if ((int) stringHash != 741716276)
              {
                if ((int) stringHash != 841786498 || !(s == "secondary_link"))
                  goto label_34;
              }
              else if (s == "steps")
                goto label_33;
              else
                goto label_34;
            }
            else if ((int) stringHash != 908164533)
            {
              if ((int) stringHash != 1266453457 || !(s == "secondary"))
                goto label_34;
            }
            else if (s == "residential")
            {
              speed.Value = speed.Value * 1.1f;
              goto label_34;
            }
            else
              goto label_34;
          }
          else if (stringHash <= 3045585386U)
          {
            if ((int) stringHash != 1512988633)
            {
              if ((int) stringHash != -2071507658)
              {
                if ((int) stringHash != -1249381910 || !(s == "primary_link"))
                  goto label_34;
              }
              else if (s == "path")
                goto label_33;
              else
                goto label_34;
            }
            else if (!(s == "primary"))
              goto label_34;
          }
          else if (stringHash <= 3713953405U)
          {
            if ((int) stringHash != -700661456)
            {
              if ((int) stringHash != -581013891 || !(s == "trunk"))
                goto label_34;
            }
            else if (s == "cycleway")
              goto label_33;
            else
              goto label_34;
          }
          else if ((int) stringHash != -424251197)
          {
            if ((int) stringHash != -37246338 || !(s == "trunk_link"))
              goto label_34;
          }
          else if (s == "tertiary")
            goto label_31;
          else
            goto label_34;
          speed.Value = speed.Value * 0.8f;
          goto label_34;
label_31:
          speed.Value = speed.Value * 0.9f;
          goto label_34;
label_33:
          speed.Value = speed.Value * 1.2f;
        }
label_34:
        return new Factor()
        {
          Value = 1f / speed.Value,
          Direction = speed.Direction
        };
      });
    }
  }
}
