namespace OsmSharp.Osm.Streams.ChangeSets
{
  public abstract class DataProcessorChangeSetFilter : DataProcessorChangeSetSource
  {
    private DataProcessorChangeSetSource _source;

    protected DataProcessorChangeSetSource Source
    {
      get
      {
        return this._source;
      }
    }

    public abstract override void Initialize();

    public abstract override bool MoveNext();

    public abstract override ChangeSet Current();

    public void RegisterSource(DataProcessorChangeSetSource source)
    {
      this._source = source;
    }
  }
}
