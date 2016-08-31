namespace OsmSharp.Collections.LongIndex
{
  public interface ILongIndex
  {
    long Count { get; }

    void Add(long number);

    void Clear();

    bool Contains(long number);

    void Remove(long number);
  }
}
