using OsmSharp.Collections.Tags;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterTagsFilter : OsmStreamFilter
  {
    private OsmStreamFilterTagsFilter.TagsFilterDelegate _filter;
    private OsmGeo _current;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public OsmStreamFilterTagsFilter(OsmStreamFilterTagsFilter.TagsFilterDelegate tagsFilter)
    {
      this._filter = tagsFilter;
    }

    public override void Initialize()
    {
      this.Source.Initialize();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      while (this.DoMoveNext(ignoreNodes, ignoreWays, ignoreRelations))
      {
        if (this.Current().Type == OsmGeoType.Node && !ignoreNodes || this.Current().Type == OsmGeoType.Way && !ignoreWays || this.Current().Type == OsmGeoType.Relation && !ignoreRelations)
          return true;
      }
      return false;
    }

    private bool DoMoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      if (!this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
        return false;
      this._current = this.Source.Current();
      if (this._current.Tags != null && this._current.Tags.Count > 0)
        this._filter(this._current.Tags);
      return true;
    }

    public override OsmGeo Current()
    {
      return this._current;
    }

    public override void Reset()
    {
      this._current = (OsmGeo) null;
      this.Source.Reset();
    }

    public delegate void TagsFilterDelegate(TagsCollectionBase collection);
  }
}
