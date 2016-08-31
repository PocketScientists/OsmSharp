using OsmSharp.Osm.Cache;

namespace OsmSharp.Osm.Streams.Complete
{
  public abstract class OsmCompleteStreamTarget
  {
    private OsmCompleteStreamSource _source;

    protected OsmCompleteStreamSource Source
    {
      get
      {
        return this._source;
      }
    }

    public abstract void Initialize();

    public abstract void AddNode(Node node);

    public abstract void AddWay(CompleteWay way);

    public abstract void AddRelation(CompleteRelation relation);

    public void RegisterSource(OsmCompleteStreamSource source)
    {
      this._source = source;
    }

    public void RegisterSource(OsmStreamSource source)
    {
      this._source = (OsmCompleteStreamSource) new OsmSimpleCompleteStreamSource(source);
    }

    public void RegisterSource(OsmStreamSource source, OsmDataCache cache)
    {
      this._source = (OsmCompleteStreamSource) new OsmSimpleCompleteStreamSource(source, cache);
    }

    public void Pull()
    {
      this._source.Initialize();
      this.Initialize();
      while (this._source.MoveNext())
      {
        ICompleteOsmGeo completeOsmGeo = this._source.Current();
        if (completeOsmGeo is Node)
          this.AddNode(completeOsmGeo as Node);
        else if (completeOsmGeo is CompleteWay)
          this.AddWay(completeOsmGeo as CompleteWay);
        else if (completeOsmGeo is CompleteRelation)
          this.AddRelation(completeOsmGeo as CompleteRelation);
      }
      this.Flush();
      this.Close();
    }

    public bool PullNext()
    {
      if (!this._source.MoveNext())
        return false;
      object obj = (object) this._source.Current();
      if (obj is Node)
        this.AddNode(obj as Node);
      else if (obj is CompleteWay)
        this.AddWay(obj as CompleteWay);
      else if (obj is CompleteRelation)
        this.AddRelation(obj as CompleteRelation);
      return true;
    }

    public virtual void Close()
    {
    }

    public virtual void Flush()
    {
    }
  }
}
