using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Data;
using OsmSharp.Routing.Graphs.Geometric;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Profiles
{
  public class ProfileFactorCache
  {
    private readonly RouterDb _db;
    private readonly Dictionary<string, ProfileFactorCache.CachedFactor[]> _edgeProfileFactors;

    public RouterDb RouterDb
    {
      get
      {
        return this._db;
      }
    }

    public ProfileFactorCache(RouterDb db)
    {
      this._db = db;
      this._edgeProfileFactors = new Dictionary<string, ProfileFactorCache.CachedFactor[]>();
    }

    public bool ContainsAll(params Profile[] profiles)
    {
      for (int index = 0; index < profiles.Length; ++index)
      {
        if (!this._edgeProfileFactors.ContainsKey(profiles[index].Name))
          return false;
      }
      return true;
    }

    public void CalculateForAll()
    {
      this.CalculateFor(Profile.GetAllRegistered().ToArray<Profile>());
    }

    public void CalculateFor(params Profile[] profiles)
    {
      ProfileFactorCache.CachedFactor[][] cachedFactorArray1 = new ProfileFactorCache.CachedFactor[profiles.Length][];
      for (int index = 0; index < profiles.Length; ++index)
        cachedFactorArray1[index] = new ProfileFactorCache.CachedFactor[(int) this._db.EdgeProfiles.Count];
      for (uint tagsId = 0; tagsId < this._db.EdgeProfiles.Count; ++tagsId)
      {
        TagsCollectionBase attributes = this._db.EdgeProfiles.Get(tagsId);
        for (int index1 = 0; index1 < profiles.Length; ++index1)
        {
          Factor factor = profiles[index1].Factor(attributes);
          ProfileFactorCache.CachedFactor cachedFactor1;
          if (profiles[index1].CanStopOn(attributes))
          {
            ProfileFactorCache.CachedFactor[] cachedFactorArray2 = cachedFactorArray1[index1];
            int index2 = (int) tagsId;
            cachedFactor1 = new ProfileFactorCache.CachedFactor();
            cachedFactor1.Type = factor.Direction;
            cachedFactor1.Value = factor.Value;
            ProfileFactorCache.CachedFactor cachedFactor2 = cachedFactor1;
            cachedFactorArray2[index2] = cachedFactor2;
          }
          else
          {
            ProfileFactorCache.CachedFactor[] cachedFactorArray2 = cachedFactorArray1[index1];
            int index2 = (int) tagsId;
            cachedFactor1 = new ProfileFactorCache.CachedFactor();
            cachedFactor1.Type = (short) ((int) factor.Direction + 4);
            cachedFactor1.Value = factor.Value;
            ProfileFactorCache.CachedFactor cachedFactor2 = cachedFactor1;
            cachedFactorArray2[index2] = cachedFactor2;
          }
        }
      }
      for (int index = 0; index < profiles.Length; ++index)
        this._edgeProfileFactors[profiles[index].Name] = cachedFactorArray1[index];
    }

    public Func<GeometricEdge, bool> GetIsAcceptable(bool verifyCanStopOn, params Profile[] profiles)
    {
      if (!this.ContainsAll(profiles))
        throw new ArgumentException("Not all given profiles are supported.");
      ProfileFactorCache.CachedFactor[][] cachedFactors = new ProfileFactorCache.CachedFactor[profiles.Length][];
      for (int index = 0; index < profiles.Length; ++index)
        cachedFactors[index] = this._edgeProfileFactors[profiles[index].Name];
      return (Func<GeometricEdge, bool>) (edge =>
      {
        float distance;
        ushort profile;
        EdgeDataSerializer.Deserialize(edge.Data[0], out distance, out profile);
        for (int index = 0; index < profiles.Length; ++index)
        {
          ProfileFactorCache.CachedFactor cachedFactor = cachedFactors[index][(int) profile];
          if (verifyCanStopOn && (int) cachedFactor.Type > 4 || (double) cachedFactor.Value <= 0.0)
            return false;
        }
        return true;
      });
    }

    public Func<ushort, Factor> GetGetFactor(Profile profile)
    {
      if (!this.ContainsAll(profile))
        throw new ArgumentException("Given profile not supported.");
      ProfileFactorCache.CachedFactor[] cachedFactors = this._edgeProfileFactors[profile.Name];
      return (Func<ushort, Factor>) (p =>
      {
        ProfileFactorCache.CachedFactor cachedFactor = cachedFactors[(int) p];
        if ((int) cachedFactor.Type >= 4)
          return new Factor()
          {
            Direction = (short) ((int) cachedFactor.Type - 4),
            Value = cachedFactor.Value
          };
        return new Factor()
        {
          Direction = cachedFactor.Type,
          Value = cachedFactor.Value
        };
      });
    }

    public Factor GetFactor(ushort edgeProfile, string profileName)
    {
      ProfileFactorCache.CachedFactor[] cachedFactorArray;
      if (!this._edgeProfileFactors.TryGetValue(profileName, out cachedFactorArray))
        throw new ArgumentOutOfRangeException(string.Format("{0} not found.", (object) profileName));
      if ((int) edgeProfile >= cachedFactorArray.Length)
        throw new ArgumentOutOfRangeException("Edgeprofile invalid.");
      ProfileFactorCache.CachedFactor cachedFactor = cachedFactorArray[(int) edgeProfile];
      if ((int) cachedFactor.Type >= 4)
        return new Factor()
        {
          Direction = (short) ((int) cachedFactor.Type << 2),
          Value = cachedFactor.Value
        };
      return new Factor()
      {
        Direction = cachedFactor.Type,
        Value = cachedFactor.Value
      };
    }

    public bool CanStopOn(ushort edgeProfile, string profileName)
    {
      ProfileFactorCache.CachedFactor[] cachedFactorArray;
      if (!this._edgeProfileFactors.TryGetValue(profileName, out cachedFactorArray))
        throw new ArgumentOutOfRangeException(string.Format("{0} not found.", (object) profileName));
      if ((int) edgeProfile < cachedFactorArray.Length)
        return (int) cachedFactorArray[(int) edgeProfile].Type < 4;
      throw new ArgumentOutOfRangeException("Edgeprofile invalid.");
    }

    private struct CachedFactor
    {
      public float Value { get; set; }

      public short Type { get; set; }
    }
  }
}
