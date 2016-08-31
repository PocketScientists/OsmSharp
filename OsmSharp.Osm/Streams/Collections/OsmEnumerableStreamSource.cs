using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Collections
{
  internal class OsmEnumerableStreamSource : OsmStreamSource
  {
    private readonly IEnumerable<OsmGeo> _baseObjects;
    private IEnumerator<OsmGeo> _baseObjectEnumerator;

    public override bool CanReset
    {
      get
      {
        return true;
      }
    }

    public OsmEnumerableStreamSource(IEnumerable<OsmGeo> baseObjects)
    {
      this._baseObjects = baseObjects;
    }

    public override void Initialize()
    {
      this.Reset();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      if (this._baseObjectEnumerator == null)
        this._baseObjectEnumerator = this._baseObjects.GetEnumerator();
      while (this._baseObjectEnumerator.MoveNext())
      {
        if ((!ignoreNodes || this._baseObjectEnumerator.Current.Type != OsmGeoType.Node) && (!ignoreWays || this._baseObjectEnumerator.Current.Type != OsmGeoType.Way) && (!ignoreRelations || this._baseObjectEnumerator.Current.Type != OsmGeoType.Relation))
          return true;
      }
      this._baseObjectEnumerator = (IEnumerator<OsmGeo>) null;
      return false;
    }

    public override OsmGeo Current()
    {
      return this._baseObjectEnumerator.Current;
    }

    public override void Reset()
    {
      this._baseObjectEnumerator = (IEnumerator<OsmGeo>) null;
    }
  }
}
