namespace OsmSharp.Osm
{
  public class CompleteChange
  {
    private readonly ChangeType _type;
    private readonly CompleteOsmGeo _obj;

    public ChangeType Type
    {
      get
      {
        return this._type;
      }
    }

    public CompleteOsmGeo Object
    {
      get
      {
        return this._obj;
      }
    }

    public CompleteChange(ChangeType type, CompleteOsmGeo obj)
    {
      this._type = type;
      this._obj = obj;
    }
  }
}
