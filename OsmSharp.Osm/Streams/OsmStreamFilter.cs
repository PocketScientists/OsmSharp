using OsmSharp.Collections.Tags;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams
{
  public abstract class OsmStreamFilter : OsmStreamSource
  {
    private OsmStreamSource _source;

    protected OsmStreamSource Source
    {
      get
      {
        return this._source;
      }
    }

    public virtual void RegisterSource(OsmStreamSource source)
    {
      this._source = source;
    }

    public virtual void RegisterSource(IEnumerable<OsmGeo> source)
    {
      this._source = source.ToOsmStreamSource();
    }

    public override TagsCollection GetAllMeta()
    {
      TagsCollection allMeta = this.Source.GetAllMeta();
      TagsCollection tagsCollection = new TagsCollection((IEnumerable<Tag>) this.Meta);
      allMeta.AddOrReplace((TagsCollectionBase) tagsCollection);
      return allMeta;
    }

    public abstract override void Initialize();

    public abstract override OsmGeo Current();
  }
}
