using ProtoBuf;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "HeaderBBox")]
  public class HeaderBBox : IExtensible
  {
    private long _left;
    private long _right;
    private long _top;
    private long _bottom;
    private IExtension extensionObject;

    [ProtoMember(1)]
    public long left
    {
      get
      {
        return this._left;
      }
      set
      {
        this._left = value;
      }
    }

    [ProtoMember(2)]
    public long right
    {
      get
      {
        return this._right;
      }
      set
      {
        this._right = value;
      }
    }

    [ProtoMember(3)]
    public long top
    {
      get
      {
        return this._top;
      }
      set
      {
        this._top = value;
      }
    }

    [ProtoMember(4)]
    public long bottom
    {
      get
      {
        return this._bottom;
      }
      set
      {
        this._bottom = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
