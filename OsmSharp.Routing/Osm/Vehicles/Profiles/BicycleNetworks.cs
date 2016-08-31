using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Profiles;
using System;

namespace OsmSharp.Routing.Osm.Vehicles.Profiles
{
  internal class BicycleNetworks : Profile
  {
    private const float HIGHEST_AVOID_FACTOR = 0.8f;
    private const float AVOID_FACTOR = 0.9f;
    private const float PREFER_FACTOR = 1.1f;
    private const float HIGHEST_PREFER_FACTOR = 1.2f;
    private const float CYCLE_NETWORK_PREFER_FACTOR = 40f;

    internal BicycleNetworks(Bicycle bicycle)
      : base(bicycle.UniqueName + ".Networks", bicycle.GetGetSpeed(), bicycle.GetGetMinSpeed(), bicycle.GetCanStop(), bicycle.GetEquals(), bicycle.VehicleTypes, BicycleNetworks.InternalGetFactor(bicycle))
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
        string empty = string.Empty;
        foreach (Tag tag in tags)
        {
          if (tag.Key.StartsWith("cyclenetwork"))
          {
            speed.Value = speed.Value * 40f;
            Factor factor = new Factor();
            factor.Value = 1f / speed.Value;
            factor.Direction = speed.Direction;
            return factor;
          }
          if (tag.Key == "highway")
            empty = tag.Value;
        }
        if (!string.IsNullOrWhiteSpace(empty))
        {
          long stringHash = empty.GetHashCode();
          if (stringHash <= 1266453457U)
          {
            if (stringHash <= 410259268U)
            {
              if ((int) stringHash != 4126140)
              {
                if ((int) stringHash != 155541048)
                {
                  if ((int) stringHash != 410259268 || !(empty == "tertiary_link"))
                    goto label_44;
                  else
                    goto label_41;
                }
                else if (empty == "footway")
                  goto label_43;
                else
                  goto label_44;
              }
              else if (empty == "pedestrian")
                goto label_43;
              else
                goto label_44;
            }
            else if (stringHash <= 841786498U)
            {
              if ((int) stringHash != 741716276)
              {
                if ((int) stringHash != 841786498 || !(empty == "secondary_link"))
                  goto label_44;
              }
              else if (empty == "steps")
                goto label_43;
              else
                goto label_44;
            }
            else if ((int) stringHash != 908164533)
            {
              if ((int) stringHash != 1266453457 || !(empty == "secondary"))
                goto label_44;
            }
            else if (empty == "residential")
            {
              speed.Value = speed.Value * 1.1f;
              goto label_44;
            }
            else
              goto label_44;
          }
          else if (stringHash <= 3045585386U)
          {
            if ((int) stringHash != 1512988633)
            {
              if ((int) stringHash != -2071507658)
              {
                if ((int) stringHash != -1249381910 || !(empty == "primary_link"))
                  goto label_44;
              }
              else if (empty == "path")
                goto label_43;
              else
                goto label_44;
            }
            else if (!(empty == "primary"))
              goto label_44;
          }
          else if (stringHash <= 3713953405U)
          {
            if ((int) stringHash != -700661456)
            {
              if ((int) stringHash != -581013891 || !(empty == "trunk"))
                goto label_44;
            }
            else if (empty == "cycleway")
              goto label_43;
            else
              goto label_44;
          }
          else if ((int) stringHash != -424251197)
          {
            if ((int) stringHash != -37246338 || !(empty == "trunk_link"))
              goto label_44;
          }
          else if (empty == "tertiary")
            goto label_41;
          else
            goto label_44;
          speed.Value = speed.Value * 0.8f;
          goto label_44;
label_41:
          speed.Value = speed.Value * 0.9f;
          goto label_44;
label_43:
          speed.Value = speed.Value * 1.2f;
        }
label_44:
        return new Factor()
        {
          Value = 1f / speed.Value,
          Direction = speed.Direction
        };
      });
    }
  }
}
