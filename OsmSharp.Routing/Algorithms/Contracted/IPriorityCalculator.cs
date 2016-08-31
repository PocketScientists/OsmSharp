using OsmSharp.Collections.LongIndex;

namespace OsmSharp.Routing.Algorithms.Contracted
{
  public interface IPriorityCalculator
  {
    float Calculate(ILongIndex contractedFlags, uint vertex);

    void NotifyContracted(uint vertex);
  }
}
