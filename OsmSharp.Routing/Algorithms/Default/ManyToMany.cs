using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Default
{
  public class ManyToMany : AlgorithmBase
  {
    private readonly RouterDb _routerDb;
    private readonly Func<ushort, Factor> _getFactor;
    private readonly RouterPoint[] _sources;
    private readonly RouterPoint[] _targets;
    private readonly float _maxSearch;
    private OneToMany[] _sourceSearches;

    public float[][] Weights
    {
      get
      {
        float[][] numArray = new float[this._sources.Length][];
        for (int index = 0; index < this._sources.Length; ++index)
          numArray[index] = this._sourceSearches[index].Weights;
        return numArray;
      }
    }

    public ManyToMany(RouterDb routerDb, Profile profile, RouterPoint[] sources, RouterPoint[] targets, float maxSearch)
      : this(routerDb, (Func<ushort, Factor>) (p => profile.Factor(routerDb.EdgeProfiles.Get((uint) p))), sources, targets, maxSearch)
    {
    }

    public ManyToMany(RouterDb routerDb, Func<ushort, Factor> getFactor, RouterPoint[] sources, RouterPoint[] targets, float maxSearch)
    {
      this._routerDb = routerDb;
      this._getFactor = getFactor;
      this._sources = sources;
      this._targets = targets;
      this._maxSearch = maxSearch;
    }

    protected override void DoRun()
    {
      this._sourceSearches = new OneToMany[this._sources.Length];
      for (int index = 0; index < this._sources.Length; ++index)
      {
        this._sourceSearches[index] = new OneToMany(this._routerDb, this._getFactor, this._sources[index], (IList<RouterPoint>) this._targets, this._maxSearch);
        this._sourceSearches[index].Run();
      }
      this.HasSucceeded = true;
    }

    public float GetBestWeight(int source, int target)
    {
      this.CheckHasRunAndHasSucceeded();
      Path path = this._sourceSearches[source].GetPath(target);
      if (path != null)
        return path.Weight;
      return float.MaxValue;
    }

    public Path GetPath(int source, int target)
    {
      this.CheckHasRunAndHasSucceeded();
      return this._sourceSearches[source].GetPath(target) ?? (Path) null;
    }
  }
}
