using OsmSharp.Routing.Data;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs;
using OsmSharp.Routing.Graphs.Directed;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public class DirectedGraphBuilder : AlgorithmBase
  {
    private readonly Graph _source;
    private readonly DirectedMetaGraph _target;
    private readonly Func<ushort, Factor> _getFactor;

    public DirectedGraphBuilder(Graph source, DirectedMetaGraph target, Func<ushort, Factor> getFactor)
    {
      this._source = source;
      this._target = target;
      this._getFactor = getFactor;
    }

    protected override void DoRun()
    {
      bool? nullable = new bool?();
      Dictionary<ushort, Factor> dictionary = new Dictionary<ushort, Factor>();
      Graph.EdgeEnumerator edgeEnumerator = this._source.GetEdgeEnumerator();
      for (uint vertex = 0; vertex < this._source.VertexCount; ++vertex)
      {
        edgeEnumerator.MoveTo(vertex);
        edgeEnumerator.Reset();
        while (edgeEnumerator.MoveNext())
        {
          float distance;
          ushort profile;
          EdgeDataSerializer.Deserialize(edgeEnumerator.Data0, out distance, out profile);
          Factor factor = Factor.NoFactor;
          if (!dictionary.TryGetValue(profile, out factor))
          {
            factor = this._getFactor(profile);
            dictionary[profile] = factor;
          }
          if ((double) factor.Value != 0.0)
          {
            bool? direction = new bool?();
            if ((int) factor.Direction == 1)
            {
              direction = new bool?(true);
              if (edgeEnumerator.DataInverted)
                direction = new bool?(false);
            }
            else if ((int) factor.Direction == 2)
            {
              direction = new bool?(false);
              if (edgeEnumerator.DataInverted)
                direction = new bool?(true);
            }
            uint data = ContractedEdgeDataSerializer.Serialize(distance * factor.Value, direction);
            int num = (int) this._target.AddEdge(edgeEnumerator.From, edgeEnumerator.To, data, 4294967294U);
          }
        }
      }
      this.HasSucceeded = true;
    }
  }
}
