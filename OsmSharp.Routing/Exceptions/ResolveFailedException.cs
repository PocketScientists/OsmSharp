using System;

namespace OsmSharp.Routing.Exceptions
{
  public class ResolveFailedException : Exception
  {
    public ResolveFailedException(string message)
      : base(message)
    {
    }
  }
}
