using OsmSharp.Collections.Tags;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams
{
  public abstract class OsmStreamTarget
  {
    private readonly TagsCollectionBase _meta;
    private OsmStreamSource _source;

    protected OsmStreamSource Source
    {
      get
      {
        return this._source;
      }
    }

    public TagsCollectionBase Meta
    {
      get
      {
        return this._meta;
      }
    }

    protected OsmStreamTarget()
    {
      this._meta = (TagsCollectionBase) new TagsCollection();
    }

    public abstract void Initialize();

    public abstract void AddNode(Node simpleNode);

    public abstract void AddWay(Way simpleWay);

    public abstract void AddRelation(Relation simpleRelation);

    public virtual void RegisterSource(OsmStreamSource source)
    {
      this._source = source;
    }

    public void Pull()
    {
      this._source.Initialize();
      this.Initialize();
      if (this.OnBeforePull())
      {
        this.DoPull();
        this.OnAfterPull();
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
      else if (obj is Way)
        this.AddWay(obj as Way);
      else if (obj is Relation)
        this.AddRelation(obj as Relation);
      return true;
    }

    protected void DoPull()
    {
      this.DoPull(false, false, false);
    }

    protected void DoPull(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      while (this._source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
      {
        object obj = (object) this._source.Current();
        if (obj is Node)
          this.AddNode(obj as Node);
        else if (obj is Way)
          this.AddWay(obj as Way);
        else if (obj is Relation)
          this.AddRelation(obj as Relation);
      }
    }

    public virtual bool OnBeforePull()
    {
      return true;
    }

    public virtual void OnAfterPull()
    {
    }

    public TagsCollection GetAllMeta()
    {
      TagsCollection allMeta = this.Source.GetAllMeta();
      TagsCollection tagsCollection = new TagsCollection((IEnumerable<Tag>) this._meta);
      allMeta.AddOrReplace((TagsCollectionBase) tagsCollection);
      return allMeta;
    }

    public virtual void Close()
    {
    }

    public virtual void Flush()
    {
    }
  }
}
