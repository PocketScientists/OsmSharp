using System;

namespace OsmSharp.Routing.Attributes
{
  [Flags]
  public enum AttributesIndexMode
  {
    None = 0,
    IncreaseOne = 1,
    ReverseCollectionIndex = 2,
    ReverseStringIndex = 4,
    ReverseStringIndexKeysOnly = 8,
    ReverseAll = ReverseStringIndex | ReverseCollectionIndex,
  }
}
