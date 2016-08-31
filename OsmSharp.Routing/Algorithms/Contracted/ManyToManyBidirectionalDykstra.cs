using OsmSharp.Routing.Graphs.Directed;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public class ManyToManyBidirectionalDykstra : AlgorithmBase
  {
    private readonly RouterDb _routerDb;
    private readonly DirectedMetaGraph _graph;
    private readonly Func<ushort, Factor> _getFactor;
    private readonly RouterPoint[] _sources;
    private readonly RouterPoint[] _targets;
    private readonly Dictionary<uint, Dictionary<int, float>> _buckets;
    private float[][] _weights;

    public float[][] Weights
    {
      get
      {
        return this._weights;
      }
    }

    public ManyToManyBidirectionalDykstra(RouterDb routerDb, Profile profile, RouterPoint[] sources, RouterPoint[] targets)
      : this(routerDb, profile, (Func<ushort, Factor>) (p => profile.Factor(routerDb.EdgeProfiles.Get((uint) p))), sources, targets)
    {
    }

    public ManyToManyBidirectionalDykstra(RouterDb routerDb, Profile profile, Func<ushort, Factor> getFactor, RouterPoint[] sources, RouterPoint[] targets)
    {
      this._routerDb = routerDb;
      this._getFactor = getFactor;
      this._sources = sources;
      this._targets = targets;
      if (!this._routerDb.TryGetContracted(profile, out this._graph))
        throw new NotSupportedException("Contraction-based many-to-many calculates are not supported in the given router db for the given profile.");
      this._buckets = new Dictionary<uint, Dictionary<int, float>>();
    }

    protected override void DoRun()
    {
      this._weights = new float[this._sources.Length][];
      for (int index1 = 0; index1 < this._sources.Length; ++index1)
      {
        RouterPoint source = this._sources[index1];
        this._weights[index1] = new float[this._targets.Length];
        for (int index2 = 0; index2 < this._targets.Length; ++index2)
        {
          RouterPoint target = this._targets[index2];
          this._weights[index1][index2] = float.MaxValue;
          if ((int) target.EdgeId == (int) source.EdgeId)
          {
            Path path = source.PathTo(this._routerDb, this._getFactor, target);
            if (path != null)
              this._weights[index1][index2] = path.Weight;
          }
        }
      }
      int num1;
      for (int i = 0; i < this._sources.Length; i = num1 + 1)
      {
        Dykstra dykstra = new Dykstra(this._graph, (IEnumerable<Path>) this._sources[i].ToPaths(this._routerDb, this._getFactor, true), false);
        dykstra.WasFound = dykstra.WasFound + (Func<uint, float, bool>) ((vertex, weight) => this.ForwardVertexFound(i, vertex, weight));
        dykstra.Run();
        num1 = i;
      }
      int num2;
      for (int i = 0; i < this._targets.Length; i = num2 + 1)
      {
        Dykstra dykstra = new Dykstra(this._graph, (IEnumerable<Path>) this._targets[i].ToPaths(this._routerDb, this._getFactor, false), true);
        dykstra.WasFound = dykstra.WasFound + (Func<uint, float, bool>) ((vertex, weight) => this.BackwardVertexFound(i, vertex, weight));
        dykstra.Run();
        num2 = i;
      }
      this.HasSucceeded = true;
    }

    private bool ForwardVertexFound(int i, uint vertex, float weight)
    {
      Dictionary<int, float> dictionary1;
      if (!this._buckets.TryGetValue(vertex, out dictionary1))
      {
        Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
        this._buckets.Add(vertex, dictionary2);
        dictionary2[i] = weight;
      }
      else
      {
        float num;
        if (dictionary1.TryGetValue(i, out num))
        {
          if ((double) weight < (double) num)
            dictionary1[i] = weight;
        }
        else
          dictionary1[i] = weight;
      }
      return false;
    }

    private bool BackwardVertexFound(int i, uint vertex, float weight)
    {
      Dictionary<int, float> dictionary;
      if (this._buckets.TryGetValue(vertex, out dictionary))
      {
        foreach (KeyValuePair<int, float> keyValuePair in dictionary)
        {
          float num = this._weights[keyValuePair.Key][i];
          if ((double) weight + (double) keyValuePair.Value < (double) num)
            this._weights[keyValuePair.Key][i] = weight + keyValuePair.Value;
        }
      }
      return false;
    }
  }
}
