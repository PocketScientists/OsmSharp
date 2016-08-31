namespace OsmSharp.Osm.Streams
{
  public class OsmStreamSourceEmpty : OsmStreamSource
  {
    public override bool CanReset
    {
      get
      {
        return true;
      }
    }

    public override void Initialize()
    {
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      return false;
    }

    public override OsmGeo Current()
    {
      return (OsmGeo) null;
    }

    public override void Reset()
    {
    }
  }
}
