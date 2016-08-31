namespace OsmSharp.Math.Primitives.Enumerators.Lines
{
  internal interface ILineList
  {
    int Count { get; }

    LineF2D this[int idx] { get; }
  }
}
