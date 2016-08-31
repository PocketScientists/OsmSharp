namespace OsmSharp.Osm.Streams.ChangeSets
{
  public abstract class DataProcessorChangeSetTarget
  {
    private DataProcessorChangeSetSource _source;

    public abstract void ApplyChange(ChangeSet change);

    public void Pull()
    {
      this._source.Initialize();
      this.Initialize();
      while (this._source.MoveNext())
        this.ApplyChange(this._source.Current());
    }

    public abstract void Initialize();

    public virtual void Close()
    {
      this._source = (DataProcessorChangeSetSource) null;
    }

    public void RegisterSource(DataProcessorChangeSetSource source)
    {
      this._source = source;
    }
  }
}
