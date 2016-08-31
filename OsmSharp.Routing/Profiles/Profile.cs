using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Profiles
{
  public class Profile
  {
    private static Dictionary<string, Profile> _staticProfiles = new Dictionary<string, Profile>();
    private readonly string _name;
    private readonly Func<TagsCollectionBase, Speed> _getSpeed;
    private readonly Func<TagsCollectionBase, Factor> _getFactor;
    private readonly Func<TagsCollectionBase, bool> _canStop;
    private readonly Func<TagsCollectionBase, TagsCollectionBase, bool> _equals;
    private readonly Func<Speed> _minSpeed;
    private readonly List<string> _vehicleTypes;
    private readonly ProfileMetric _metric;

    public virtual string Name
    {
      get
      {
        return this._name;
      }
    }

    public virtual ProfileMetric Metric
    {
      get
      {
        return this._metric;
      }
    }

    public virtual List<string> VehicleType
    {
      get
      {
        return this._vehicleTypes;
      }
    }

    public Profile(string name, Func<TagsCollectionBase, Speed> getSpeed, Func<Speed> minSpeed, Func<TagsCollectionBase, bool> canStop, Func<TagsCollectionBase, TagsCollectionBase, bool> equals, List<string> vehicleTypes, ProfileMetric metric)
    {
      if (metric == ProfileMetric.Custom)
        throw new ArgumentException("Cannot set a custom metric without a getFactor function.");
      this._minSpeed = minSpeed;
      this._getSpeed = getSpeed;
      this._canStop = canStop;
      this._equals = equals;
      this._vehicleTypes = vehicleTypes;
      this._name = name;
      this._metric = metric;
      this._getFactor = (Func<TagsCollectionBase, Factor>) null;
    }

    public Profile(string name, Func<TagsCollectionBase, Speed> getSpeed, Func<Speed> minSpeed, Func<TagsCollectionBase, bool> canStop, Func<TagsCollectionBase, TagsCollectionBase, bool> equals, List<string> vehicleTypes, Func<TagsCollectionBase, Factor> getFactor)
    {
      this._minSpeed = minSpeed;
      this._getSpeed = getSpeed;
      this._canStop = canStop;
      this._equals = equals;
      this._vehicleTypes = vehicleTypes;
      this._name = name;
      this._metric = ProfileMetric.Custom;
      this._getFactor = getFactor;
    }

    public virtual Factor Factor(TagsCollectionBase attributes)
    {
      if (this._metric == ProfileMetric.Custom)
        return this._getFactor(attributes);
      Speed speed = this._getSpeed(attributes);
      if (this._metric == ProfileMetric.DistanceInMeters)
        return new Factor()
        {
          Direction = speed.Direction,
          Value = 1f
        };
      if (this._metric == ProfileMetric.TimeInSeconds)
      {
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
      }
      throw new Exception(string.Format("Unknown metric used in profile: {0}", (object) this._name));
    }

    public virtual bool CanStopOn(TagsCollectionBase attributes)
    {
      return this._canStop(attributes);
    }

    public virtual Speed Speed(TagsCollectionBase attributes)
    {
      return this._getSpeed(attributes);
    }

    public virtual Speed MinSpeed()
    {
      return this._minSpeed();
    }

    public virtual bool Equals(TagsCollectionBase edge1, TagsCollectionBase edge2)
    {
      return this._equals(edge1, edge2);
    }

    public static void Register(Profile profile)
    {
      Profile._staticProfiles[profile.Name] = profile;
      Profile._staticProfiles[profile.Name.ToLowerInvariant()] = profile;
    }

    public static IEnumerable<Profile> GetAllRegistered()
    {
      return (IEnumerable<Profile>) Profile._staticProfiles.Values;
    }

    public static bool TryGet(string name, out Profile profile)
    {
      return Profile._staticProfiles.TryGetValue(name, out profile);
    }

    public static Profile Get(string name)
    {
      Profile profile;
      if (!Profile.TryGet(name, out profile))
        throw new Exception(string.Format("Profile {0} not found.", (object) name));
      return profile;
    }
  }
}
