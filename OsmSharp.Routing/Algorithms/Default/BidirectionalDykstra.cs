using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Default
{
  public class BidirectionalDykstra : AlgorithmBase
  {
    private uint _bestVertex = uint.MaxValue;
    private float _bestWeight = float.MaxValue;
    private float _maxForward = float.MaxValue;
    private float _maxBackward = float.MaxValue;
    private readonly Dykstra _sourceSearch;
    private readonly Dykstra _targetSearch;

    public Dykstra SourceSearch
    {
      get
      {
        return this._sourceSearch;
      }
    }

    public Dykstra TargetSearch
    {
      get
      {
        return this._targetSearch;
      }
    }

    public uint BestVertex
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._bestVertex;
      }
    }

    public BidirectionalDykstra(Dykstra sourceSearch, Dykstra targetSearch)
    {
      this._sourceSearch = sourceSearch;
      this._targetSearch = targetSearch;
    }

    protected override void DoRun()
    {
      this._bestVertex = uint.MaxValue;
      this._bestWeight = float.MaxValue;
      this._maxForward = float.MinValue;
      this._maxBackward = float.MinValue;
      this._sourceSearch.WasFound = (Dykstra.WasFoundDelegate) ((vertex, weight) =>
      {
        this._maxForward = weight;
        return this.ReachedVertexForward(vertex, weight);
      });
      this._targetSearch.WasFound = (Dykstra.WasFoundDelegate) ((vertex, weight) =>
      {
        this._maxBackward = weight;
        return this.ReachedVertexBackward(vertex, weight);
      });
      this._sourceSearch.Initialize();
      this._targetSearch.Initialize();
      bool flag1 = true;
      bool flag2 = true;
      while (flag1 | flag2)
      {
        flag1 = false;
        if ((double) this._maxForward < (double) this._bestWeight)
          flag1 = this._sourceSearch.Step();
        flag2 = false;
        if ((double) this._maxBackward < (double) this._bestWeight)
          flag2 = this._targetSearch.Step();
        if (this.HasSucceeded)
          break;
      }
    }

    private bool ReachedVertexForward(uint vertex, float weight)
    {
      Path visit;
      if (this._targetSearch.TryGetVisit(vertex, out visit))
      {
        weight += visit.Weight;
        if ((double) weight < (double) this._bestWeight)
        {
          this._bestWeight = weight;
          this._bestVertex = vertex;
          this.HasSucceeded = true;
        }
      }
      return false;
    }

    private bool ReachedVertexBackward(uint vertex, float weight)
    {
      Path visit;
      if (this._sourceSearch.TryGetVisit(vertex, out visit))
      {
        weight += visit.Weight;
        if ((double) weight < (double) this._bestWeight)
        {
          this._bestWeight = weight;
          this._bestVertex = vertex;
          this.HasSucceeded = true;
        }
      }
      return false;
    }

    public List<uint> GetPath(out float weight)
    {
      this.CheckHasRunAndHasSucceeded();
      weight = 0.0f;
      Path visit1;
      Path visit2;
      if (!this._sourceSearch.TryGetVisit(this._bestVertex, out visit1) || !this._targetSearch.TryGetVisit(this._bestVertex, out visit2))
        throw new InvalidOperationException("No path could be found to/from source/target.");
      List<uint> vertices = new List<uint>();
      weight = visit1.Weight + visit2.Weight;
      visit1.AddToList(vertices);
      if (visit2.From != null)
        visit2.From.AddToListReverse(vertices);
      return vertices;
    }

    public List<uint> GetPath()
    {
      float weight;
      return this.GetPath(out weight);
    }
  }
}
