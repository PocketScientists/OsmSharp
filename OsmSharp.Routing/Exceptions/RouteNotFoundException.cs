using System;

namespace OsmSharp.Routing.Exceptions
{
  public class RouteNotFoundException : Exception
  {
    public RouteNotFoundException(string message)
      : base(message)
    {
    }
  }
}
