using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "ChangeSet")]
  public class ChangeSet : IExtensible
  {
    private readonly List<uint> _keys = new List<uint>();
    private readonly List<uint> _vals = new List<uint>();
    private long _id;
    private Info _info;
    private long _created_at;
    private long _closetime_delta;
    private bool _open;
    private HeaderBBox _bbox;
    private IExtension extensionObject;

    [ProtoMember(1)]
    public long id
    {
      get
      {
        return this._id;
      }
      set
      {
        this._id = value;
      }
    }

    [ProtoMember(2)]
    public List<uint> keys
    {
      get
      {
        return this._keys;
      }
    }

    [ProtoMember(3)]
    public List<uint> vals
    {
      get
      {
        return this._vals;
      }
    }

    [ProtoMember(4)]
    [DefaultValue(null)]
    public Info info
    {
      get
      {
        return this._info;
      }
      set
      {
        this._info = value;
      }
    }

    [ProtoMember(8)]
    public long created_at
    {
      get
      {
        return this._created_at;
      }
      set
      {
        this._created_at = value;
      }
    }

    [ProtoMember(9)]
    [DefaultValue(0)]
    public long closetime_delta
    {
      get
      {
        return this._closetime_delta;
      }
      set
      {
        this._closetime_delta = value;
      }
    }

    [ProtoMember(10)]
    public bool open
    {
      get
      {
        return this._open;
      }
      set
      {
        this._open = value;
      }
    }

    [ProtoMember(11)]
    [DefaultValue(null)]
    public HeaderBBox bbox
    {
      get
      {
        return this._bbox;
      }
      set
      {
        this._bbox = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
