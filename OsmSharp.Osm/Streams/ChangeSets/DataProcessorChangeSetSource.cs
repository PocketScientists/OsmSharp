namespace OsmSharp.Osm.Streams.ChangeSets
{
  public abstract class DataProcessorChangeSetSource
  {
    public abstract void Initialize();

    public abstract bool MoveNext();

    public abstract ChangeSet Current();

    public abstract void Reset();

    public abstract void Close();
  }
}
