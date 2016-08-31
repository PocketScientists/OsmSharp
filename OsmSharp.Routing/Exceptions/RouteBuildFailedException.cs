using System;

namespace OsmSharp.Routing.Exceptions
{
  public class RouteBuildFailedException : Exception
  {
    public RouteBuildFailedException(string message)
      : base(message)
    {
    }
  }
}
