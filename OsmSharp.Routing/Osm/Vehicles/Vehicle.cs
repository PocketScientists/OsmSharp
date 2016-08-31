using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Routing.Profiles;
using OsmSharp.Units.Speed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Vehicles
{
  public abstract class Vehicle
  {
    public static readonly Car Car = new Car();
    public static readonly Pedestrian Pedestrian = new Pedestrian();
    public static readonly Bicycle Bicycle = new Bicycle();
    public static readonly Moped Moped = new Moped();
    public static readonly MotorCycle MotorCycle = new MotorCycle();
    public static readonly SmallTruck SmallTruck = new SmallTruck();
    public static readonly BigTruck BigTruck = new BigTruck();
    public static readonly Bus Bus = new Bus();
    private static Dictionary<string, Vehicle> VehiclesByName = (Dictionary<string, Vehicle>) null;
    private static HashSet<string> _relevantProfileKeys = new HashSet<string>()
    {
      "oneway",
      "highway",
      "vehicle",
      "motor_vehicle",
      "bicycle",
      "foot",
      "access",
      "maxspeed",
      "junction"
    };
    private static HashSet<string> _relevantMetaKeys = new HashSet<string>()
    {
      "name"
    };
    public readonly List<string> VehicleTypes = new List<string>();
    protected readonly Dictionary<string, string> AccessibleTags = new Dictionary<string, string>();

    public abstract string UniqueName { get; }

    public static void RegisterVehicles()
    {
      Vehicle.Car.Register();
      Vehicle.Pedestrian.Register();
      Vehicle.Bicycle.Register();
      Vehicle.Moped.Register();
      Vehicle.MotorCycle.Register();
      Vehicle.SmallTruck.Register();
      Vehicle.BigTruck.Register();
      Vehicle.Bus.Register();
    }

    public virtual void Register()
    {
      if (Vehicle.VehiclesByName == null)
        Vehicle.VehiclesByName = new Dictionary<string, Vehicle>();
      Vehicle.VehiclesByName[this.UniqueName.ToLowerInvariant()] = this;
      foreach (Profile profile in this.GetProfiles())
        Profile.Register(profile);
    }

    public static Vehicle GetByUniqueName(string uniqueName)
    {
      Vehicle vehicle;
      if (!Vehicle.TryGetByUniqueName(uniqueName, out vehicle))
        throw new ArgumentOutOfRangeException(string.Format("Vehicle profile with name {0} not found or not registered.", (object) uniqueName));
      return vehicle;
    }

    public static IEnumerable<Vehicle> GetAllRegistered()
    {
      return (IEnumerable<Vehicle>) Vehicle.VehiclesByName.Values;
    }

    public static bool TryGetByUniqueName(string uniqueName, out Vehicle vehicle)
    {
      if (uniqueName == null)
        throw new ArgumentNullException("uniqueName");
      if (Vehicle.VehiclesByName == null)
        Vehicle.RegisterVehicles();
      uniqueName = uniqueName.ToLowerInvariant();
      return Vehicle.VehiclesByName.TryGetValue(uniqueName, out vehicle);
    }

    public static bool IsRelevantForOneOrMore(string key)
    {
      if (Vehicle.VehiclesByName == null)
        Vehicle.RegisterVehicles();
      foreach (KeyValuePair<string, Vehicle> keyValuePair in Vehicle.VehiclesByName)
      {
        if (keyValuePair.Value.IsRelevant(key))
          return true;
      }
      return false;
    }

    public static bool IsRelevantForOneOrMore(string key, string value)
    {
      if (Vehicle.VehiclesByName == null)
        Vehicle.RegisterVehicles();
      foreach (KeyValuePair<string, Vehicle> keyValuePair in Vehicle.VehiclesByName)
      {
        if (keyValuePair.Value.IsRelevant(key, value))
          return true;
      }
      return false;
    }

    public virtual bool IsRelevantForProfile(string key)
    {
      return !string.IsNullOrWhiteSpace(key) && (Vehicle._relevantProfileKeys.Contains(key) || key.StartsWith("oneway"));
    }

    public virtual bool IsRelevantForMeta(string key)
    {
      return Vehicle._relevantMetaKeys.Contains(key);
    }

    protected bool TryGetHighwayType(TagsCollectionBase tags, out string highwayType)
    {
      highwayType = string.Empty;
      if (tags != null)
        return tags.TryGetValue("highway", out highwayType);
      return false;
    }

    public virtual bool CanTraverse(TagsCollectionBase tags)
    {
      string highwayType;
      if (this.TryGetHighwayType(tags, out highwayType))
        return this.IsVehicleAllowed(tags, highwayType);
      return false;
    }

    public virtual bool CanStopOn(TagsCollectionBase tags)
    {
      return true;
    }

    public abstract KilometerPerHour MaxSpeedAllowed(string highwayType);

    public abstract KilometerPerHour MaxSpeed();

    public abstract KilometerPerHour MinSpeed();

    public virtual KilometerPerHour MaxSpeedAllowed(TagsCollectionBase tags)
    {
      KilometerPerHour kilometerPerHour = (KilometerPerHour) 5.0;
      string highwayType;
      if (TagExtensions.TryGetMaxSpeed(tags, out kilometerPerHour) || !this.TryGetHighwayType(tags, out highwayType))
        return kilometerPerHour;
      kilometerPerHour = this.MaxSpeedAllowed(highwayType);
      return kilometerPerHour;
    }

    public virtual KilometerPerHour ProbableSpeed(TagsCollectionBase tags)
    {
      KilometerPerHour kilometerPerHour1 = this.MaxSpeedAllowed(tags);
      KilometerPerHour kilometerPerHour2 = this.MaxSpeed();
      if (kilometerPerHour2.Value < kilometerPerHour1.Value)
        return kilometerPerHour2;
      return kilometerPerHour1;
    }

    public virtual bool IsEqualFor(TagsCollectionBase tags1, TagsCollectionBase tags2)
    {
      return !(this.GetName(tags1) != this.GetName(tags2));
    }

    public virtual bool? IsOneWay(TagsCollectionBase tags)
    {
      string str1;
      if (tags.TryGetValue("oneway", out str1))
      {
        if (str1 == "yes")
          return new bool?(true);
        if (str1 == "no")
          return new bool?();
        return new bool?(false);
      }
      string str2;
      if (tags.TryGetValue("junction", out str2) && str2 == "roundabout")
        return new bool?(true);
      return new bool?();
    }

    private string GetName(TagsCollectionBase tags)
    {
      string str = string.Empty;
      if (tags.ContainsKey("name"))
        str = tags["name"];
      return str;
    }

    protected abstract bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType);

    public virtual bool IsRelevant(string key)
    {
      if (!this.IsRelevantForProfile(key))
        return this.IsRelevantForMeta(key);
      return true;
    }

    public virtual bool IsRelevant(string key, string value)
    {
      return this.IsRelevant(key);
    }

    public Profile Fastest()
    {
      return new Profile(this.UniqueName, this.GetGetSpeed(), this.GetGetMinSpeed(), this.GetCanStop(), this.GetEquals(), this.VehicleTypes, ProfileMetric.TimeInSeconds);
    }

    public Profile Shortest()
    {
      return new Profile(this.UniqueName + ".Shortest", this.GetGetSpeed(), this.GetGetMinSpeed(), this.GetCanStop(), this.GetEquals(), this.VehicleTypes, ProfileMetric.DistanceInMeters);
    }

    public virtual Profile[] GetProfiles()
    {
      return new Profile[2]
      {
        this.Fastest(),
        this.Shortest()
      };
    }

    internal Func<TagsCollectionBase, OsmSharp.Routing.Profiles.Speed> GetGetSpeed()
    {
      return (Func<TagsCollectionBase, OsmSharp.Routing.Profiles.Speed>) (tags =>
      {
        if (!this.CanTraverse(tags))
          return OsmSharp.Routing.Profiles.Speed.NoSpeed;
        OsmSharp.Routing.Profiles.Speed speed = new OsmSharp.Routing.Profiles.Speed()
        {
          Value = (float) this.ProbableSpeed(tags).Value / 3.6f,
          Direction = 0
        };
        bool? nullable = this.IsOneWay(tags);
        if (nullable.HasValue)
          speed.Direction = !nullable.Value ? (short) 2 : (short) 1;
        return speed;
      });
    }

    internal Func<OsmSharp.Routing.Profiles.Speed> GetGetMinSpeed()
    {
      return (Func<OsmSharp.Routing.Profiles.Speed>) (() => new OsmSharp.Routing.Profiles.Speed()
      {
        Value = (float) this.MinSpeed().Value / 3.6f,
        Direction = (short) 0
      });
    }

    internal Func<TagsCollectionBase, bool> GetCanStop()
    {
      return (Func<TagsCollectionBase, bool>) (tags => this.CanStopOn(tags));
    }

    internal Func<TagsCollectionBase, TagsCollectionBase, bool> GetEquals()
    {
      return (Func<TagsCollectionBase, TagsCollectionBase, bool>) ((edge1, edge2) => this.IsEqualFor(edge1, edge2));
    }

    internal Func<TagsCollectionBase, Factor> GetGetFactor()
    {
      Func<TagsCollectionBase, OsmSharp.Routing.Profiles.Speed> getSpeed = this.GetGetSpeed();
      return (Func<TagsCollectionBase, Factor>) (tags =>
      {
        OsmSharp.Routing.Profiles.Speed speed = getSpeed(tags);
        if ((double) speed.Value == 0.0)
          return new Factor()
          {
            Value = 0.0f,
            Direction = 0
          };
        return new Factor()
        {
          Value = 1f / speed.Value,
          Direction = speed.Direction
        };
      });
    }
  }
}
