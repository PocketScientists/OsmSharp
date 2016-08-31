using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Default
{
  public class OneToMany : AlgorithmBase
  {
    private readonly RouterDb _routerDb;
    private readonly RouterPoint _source;
    private readonly IList<RouterPoint> _targets;
    private readonly Func<ushort, Factor> _getFactor;
    private readonly float _maxSearch;
    private Path[] _best;

    public float[] Weights
    {
      get
      {
        float[] numArray = new float[this._best.Length];
        for (int index = 0; index < this._best.Length; ++index)
        {
          numArray[index] = float.MaxValue;
          if (this._best[index] != null)
            numArray[index] = this._best[index].Weight;
        }
        return numArray;
      }
    }

    public OneToMany(RouterDb routerDb, Profile profile, RouterPoint source, IList<RouterPoint> targets, float maxSearch)
      : this(routerDb, (Func<ushort, Factor>) (p => profile.Factor(routerDb.EdgeProfiles.Get((uint) p))), source, targets, maxSearch)
    {
    }

    public OneToMany(RouterDb routerDb, Func<ushort, Factor> getFactor, RouterPoint source, IList<RouterPoint> targets, float maxSearch)
    {
      this._routerDb = routerDb;
      this._getFactor = getFactor;
      this._source = source;
      this._targets = targets;
      this._maxSearch = float.MaxValue;
    }

    protected override void DoRun()
    {
      this._best = new Path[this._targets.Count];
      Path[] paths1 = this._source.ToPaths(this._routerDb, this._getFactor, true);
      Dictionary<uint, OneToMany.LinkedTarget> targetIndexesPerVertex = new Dictionary<uint, OneToMany.LinkedTarget>();
      IEnumerable<Path>[] targetPaths = new IEnumerable<Path>[this._targets.Count];
      for (int index1 = 0; index1 < this._targets.Count; ++index1)
      {
        Path[] paths2 = this._targets[index1].ToPaths(this._routerDb, this._getFactor, false);
        targetPaths[index1] = (IEnumerable<Path>) paths2;
        if ((int) this._source.EdgeId == (int) this._targets[index1].EdgeId)
          this._best[index1] = this._source.PathTo(this._routerDb, this._getFactor, this._targets[index1]);
        for (int index2 = 0; index2 < paths2.Length; ++index2)
        {
          OneToMany.LinkedTarget valueOrDefault = targetIndexesPerVertex.TryGetValueOrDefault<uint, OneToMany.LinkedTarget>(paths2[index2].Vertex);
          targetIndexesPerVertex[paths2[index2].Vertex] = new OneToMany.LinkedTarget()
          {
            Target = index1,
            Next = valueOrDefault
          };
        }
      }
      float sourceMax = 0.0f;
      for (int index = 0; index < this._best.Length; ++index)
      {
        if (this._best[index] == null)
          sourceMax = this._maxSearch;
        else if ((double) this._best[index].Weight > (double) sourceMax)
          sourceMax = this._best[index].Weight;
      }
      Dykstra dykstra = new Dykstra(this._routerDb.Network.GeometricGraph.Graph, this._getFactor, (IEnumerable<Path>) paths1, sourceMax, false);
      dykstra.WasFound += (Dykstra.WasFoundDelegate) ((vertex, weight) =>
      {
        OneToMany.LinkedTarget next;
        if (targetIndexesPerVertex.TryGetValue(vertex, out next))
        {
          for (; next != null; next = next.Next)
          {
            Path path1 = this._best[next.Target];
            foreach (Path path2 in targetPaths[next.Target])
            {
              Path visit;
              dykstra.TryGetVisit(vertex, out visit);
              if ((int) path2.Vertex == (int) vertex)
              {
                if (path1 != null)
                {
                  if ((double) path2.Weight + (double) weight >= (double) path1.Weight)
                    break;
                }
                path1 = !this._targets[next.Target].IsVertex(this._routerDb, visit.Vertex) ? new Path(this._targets[next.Target].VertexId(this._routerDb), path2.Weight + weight, visit) : visit;
                break;
              }
            }
            this._best[next.Target] = path1;
          }
        }
        return false;
      });
      dykstra.Run();
      this.HasSucceeded = true;
    }

    public Path GetPath(int target)
    {
      this.CheckHasRunAndHasSucceeded();
      Path path = this._best[target];
      if (path != null)
        return path;
      throw new InvalidOperationException("No path could be found to/from source/target.");
    }

    private class LinkedTarget
    {
      public int Target { get; set; }

      public OneToMany.LinkedTarget Next { get; set; }
    }
  }
}
