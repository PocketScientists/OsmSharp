namespace OsmSharp.Math.Primitives.Enumerators.Points
{
  internal interface IPointList
  {
    int Count { get; }

    PointF2D this[int idx] { get; }
  }
}
