using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Profiles;
using System;

namespace OsmSharp.Routing.Osm.Vehicles.Profiles
{
  internal class CarClassifications : Profile
  {
    private static float CLASS_FACTOR = 4f;
    private static float MOTORWAY = 10f;
    private static float TRUNK = 9f;
    private static float PRIMARY = 8f;
    private static float SECONDARY = 7f;
    private static float TERTIARY = 6f;
    private static float RESIDENTIAL = 5f;
    private static float REST = 4f;

    internal CarClassifications(Car car)
      : base(car.UniqueName + ".Classifications", car.GetGetSpeed(), car.GetGetMinSpeed(), car.GetCanStop(), car.GetEquals(), car.VehicleTypes, CarClassifications.InternalGetFactor(car))
    {
    }

    private static Func<TagsCollectionBase, Factor> InternalGetFactor(Car car)
    {
      car.GetGetFactor();
      Func<TagsCollectionBase, Speed> getSpeedDefault = car.GetGetSpeed();
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
          // ISSUE: reference to a compiler-generated method
          long stringHash = s.GetHashCode();
          if (stringHash <= 1512988633U)
          {
            if (stringHash <= 841786498U)
            {
              if ((int) stringHash != 410259268)
              {
                if ((int) stringHash != 841786498 || !(s == "secondary_link"))
                  goto label_30;
              }
              else if (s == "tertiary_link")
                goto label_28;
              else
                goto label_30;
            }
            else if ((int) stringHash != 908164533)
            {
              if ((int) stringHash != 1266453457)
              {
                if ((int) stringHash != 1512988633 || !(s == "primary"))
                  goto label_30;
                else
                  goto label_26;
              }
              else if (!(s == "secondary"))
                goto label_30;
            }
            else if (s == "residential")
            {
              speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.RESIDENTIAL;
              goto label_31;
            }
            else
              goto label_30;
            speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.SECONDARY;
            goto label_31;
          }
          else if (stringHash <= 3589461977U)
          {
            if ((int) stringHash != -1249381910)
            {
              if ((int) stringHash != -1049142806)
              {
                if ((int) stringHash != -705505319 || !(s == "motorway"))
                  goto label_30;
              }
              else if (!(s == "motorway_link"))
                goto label_30;
              speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.MOTORWAY;
              goto label_31;
            }
            else if (!(s == "primary_link"))
              goto label_30;
          }
          else
          {
            if ((int) stringHash != -581013891)
            {
              if ((int) stringHash != -424251197)
              {
                if ((int) stringHash != -37246338 || !(s == "trunk_link"))
                  goto label_30;
              }
              else if (s == "tertiary")
                goto label_28;
              else
                goto label_30;
            }
            else if (!(s == "trunk"))
              goto label_30;
            speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.TRUNK;
            goto label_31;
          }
label_26:
          speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.PRIMARY;
          goto label_31;
label_28:
          speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.TERTIARY;
          goto label_31;
label_30:
          speed.Value = speed.Value * CarClassifications.CLASS_FACTOR * CarClassifications.REST;
        }
label_31:
        return new Factor()
        {
          Value = 1f / speed.Value,
          Direction = speed.Direction
        };
      });
    }
  }
}
