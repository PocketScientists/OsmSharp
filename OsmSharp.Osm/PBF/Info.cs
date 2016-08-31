using ProtoBuf;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "Info")]
  public class Info : IExtensible
  {
    private int _version = -1;
    private int _timestamp;
    private long _changeset;
    private int _uid;
    private int _user_sid;
    private IExtension extensionObject;

    [ProtoMember(1)]
    [DefaultValue(-1)]
    public int version
    {
      get
      {
        return this._version;
      }
      set
      {
        this._version = value;
      }
    }

    [ProtoMember(2)]
    [DefaultValue(0)]
    public int timestamp
    {
      get
      {
        return this._timestamp;
      }
      set
      {
        this._timestamp = value;
      }
    }

    [ProtoMember(3)]
    [DefaultValue(0)]
    public long changeset
    {
      get
      {
        return this._changeset;
      }
      set
      {
        this._changeset = value;
      }
    }

    [ProtoMember(4)]
    [DefaultValue(0)]
    public int uid
    {
      get
      {
        return this._uid;
      }
      set
      {
        this._uid = value;
      }
    }

    [ProtoMember(5)]
    [DefaultValue(0)]
    public int user_sid
    {
      get
      {
        return this._user_sid;
      }
      set
      {
        this._user_sid = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
