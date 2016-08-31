using System;

namespace OsmSharp.Osm.Data.Core.API
{
  public class APIException : Exception
  {
    public APIException(string message)
      : base(message)
    {
    }
  }
}
