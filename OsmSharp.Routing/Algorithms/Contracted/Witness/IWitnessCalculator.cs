using OsmSharp.Routing.Graphs.Directed;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Contracted.Witness
{
  public interface IWitnessCalculator
  {
    void Calculate(DirectedGraph graph, uint source, List<uint> targets, List<float> weights, ref bool[] forwardWitness, ref bool[] backwardWitness, uint vertexToSkip);
  }
}
