using OsmSharp.Collections.Tags;
using OsmSharp.Units.Speed;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Vehicles
{
  public class Pedestrian : Vehicle
  {
    public override string UniqueName
    {
      get
      {
        return "Pedestrian";
      }
    }

    public Pedestrian()
    {
      this.AccessibleTags.Add("service", string.Empty);
      this.AccessibleTags.Add("services", string.Empty);
      this.AccessibleTags.Add("steps", string.Empty);
      this.AccessibleTags.Add("footway", string.Empty);
      this.AccessibleTags.Add("cycleway", string.Empty);
      this.AccessibleTags.Add("path", string.Empty);
      this.AccessibleTags.Add("road", string.Empty);
      this.AccessibleTags.Add("track", string.Empty);
      this.AccessibleTags.Add("pedestrian", string.Empty);
      this.AccessibleTags.Add("living_street", string.Empty);
      this.AccessibleTags.Add("residential", string.Empty);
      this.AccessibleTags.Add("unclassified", string.Empty);
      this.AccessibleTags.Add("secondary", string.Empty);
      this.AccessibleTags.Add("secondary_link", string.Empty);
      this.AccessibleTags.Add("primary", string.Empty);
      this.AccessibleTags.Add("primary_link", string.Empty);
      this.AccessibleTags.Add("tertiary", string.Empty);
      this.AccessibleTags.Add("tertiary_link", string.Empty);
      this.VehicleTypes.Add("foot");
    }

    protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
    {
      if (!tags.InterpretAccessValues((IEnumerable<string>) this.VehicleTypes, "access"))
        return false;
      if (tags.ContainsKey("foot"))
      {
        if (tags["foot"] == "designated" || tags["foot"] == "yes")
          return true;
        if (tags["foot"] == "no")
          return false;
      }
      return this.AccessibleTags.ContainsKey(highwayType);
    }

    public override KilometerPerHour MaxSpeedAllowed(string highwayType)
    {
      long stringHash = highwayType.GetHashCode();
      if (stringHash <= 2223459638U)
      {
        if (stringHash <= 823414723U)
        {
          if (stringHash <= 155541048U)
          {
            if ((int) stringHash != 4126140)
            {
              if ((int) stringHash != 155541048 || !(highwayType == "footway"))
                goto label_40;
            }
            else if (!(highwayType == "pedestrian"))
              goto label_40;
          }
          else if ((int) stringHash != 741716276)
          {
            if ((int) stringHash != 823414723 || !(highwayType == "proposed"))
              goto label_40;
          }
          else if (!(highwayType == "steps"))
            goto label_40;
        }
        else
        {
          if (stringHash <= 1025494171U)
          {
            if ((int) stringHash != 908164533)
            {
              if ((int) stringHash != 1025494171 || !(highwayType == "road"))
                goto label_40;
            }
            else if (highwayType == "residential")
              goto label_37;
            else
              goto label_40;
          }
          else if ((int) stringHash != 1512988633)
          {
            if ((int) stringHash != 2067622972)
            {
              if ((int) stringHash != -2071507658 || !(highwayType == "path"))
                goto label_40;
              else
                goto label_35;
            }
            else if (!(highwayType == "track"))
              goto label_40;
          }
          else if (highwayType == "primary")
            goto label_39;
          else
            goto label_40;
          return (KilometerPerHour) 4.5;
        }
      }
      else
      {
        if (stringHash <= 3245824490U)
        {
          if (stringHash <= 2988960926U)
          {
            if ((int) stringHash != -1522085731)
            {
              if ((int) stringHash != -1306006370 || !(highwayType == "living_street"))
                goto label_40;
              else
                goto label_35;
            }
            else if (highwayType == "unclassified")
              goto label_37;
            else
              goto label_40;
          }
          else if ((int) stringHash != -1249381910)
          {
            if ((int) stringHash != -1049142806 || !(highwayType == "motorway_link"))
              goto label_40;
          }
          else if (highwayType == "primary_link")
            goto label_39;
          else
            goto label_40;
        }
        else if (stringHash <= 3594305840U)
        {
          if ((int) stringHash != -705505319)
          {
            if ((int) stringHash != -700661456 || !(highwayType == "cycleway"))
              goto label_40;
            else
              goto label_35;
          }
          else if (!(highwayType == "motorway"))
            goto label_40;
        }
        else if ((int) stringHash != -647324107)
        {
          if ((int) stringHash != -581013891)
          {
            if ((int) stringHash != -37246338 || !(highwayType == "trunk_link"))
              goto label_40;
            else
              goto label_39;
          }
          else if (highwayType == "trunk")
            goto label_39;
          else
            goto label_40;
        }
        else if (highwayType == "services")
          goto label_35;
        else
          goto label_40;
        return (KilometerPerHour) 4.3;
      }
label_35:
      return (KilometerPerHour) 5.0;
label_37:
      return (KilometerPerHour) 4.4;
label_39:
      return (KilometerPerHour) 4.2;
label_40:
      return (KilometerPerHour) 4.0;
    }

    public override bool? IsOneWay(TagsCollectionBase tags)
    {
      return new bool?();
    }

    public override KilometerPerHour MaxSpeed()
    {
      return (KilometerPerHour) 5.0;
    }

    public override KilometerPerHour MinSpeed()
    {
      return (KilometerPerHour) 3.0;
    }
  }
}
