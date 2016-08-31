namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterWithEvents : OsmStreamFilter
  {
    private readonly object _param;
    private OsmGeo _current;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public event OsmStreamFilterWithEvents.EmptyDelegate InitializeEvent;

    public event OsmStreamFilterWithEvents.SimpleOsmGeoDelegate MovedToNextEvent;

    public OsmStreamFilterWithEvents()
    {
      this._param = (object) null;
    }

    public OsmStreamFilterWithEvents(object param)
    {
      this._param = param;
    }

    public override void Initialize()
    {
      // ISSUE: reference to a compiler-generated field
      if (this.InitializeEvent != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.InitializeEvent();
      }
      this.Source.Initialize();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      while (this.DoMoveNext())
      {
        if (this.Current().Type == OsmGeoType.Node && !ignoreNodes || this.Current().Type == OsmGeoType.Way && !ignoreWays || this.Current().Type == OsmGeoType.Relation && !ignoreRelations)
          return true;
      }
      return false;
    }

    private bool DoMoveNext()
    {
      this._current = (OsmGeo) null;
      while (this._current == null)
      {
        if (!this.Source.MoveNext())
          return false;
        this._current = this.Source.Current();
        // ISSUE: reference to a compiler-generated field
        if (this.MovedToNextEvent != null)
        {
          // ISSUE: reference to a compiler-generated field
          this._current = this.MovedToNextEvent(this._current, this._param);
        }
      }
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

    public delegate void EmptyDelegate();

    public delegate OsmGeo SimpleOsmGeoDelegate(OsmGeo osmGeo, object param);
  }
}
