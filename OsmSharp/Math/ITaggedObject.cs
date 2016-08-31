using System.Collections.Generic;

namespace OsmSharp.Math
{
  public interface ITaggedObject
  {
    List<KeyValuePair<string, string>> Tags { get; }
  }
}
