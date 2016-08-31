using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "Way")]
  public class Way : IExtensible
  {
    private readonly List<uint> _keys = new List<uint>();
    private readonly List<uint> _vals = new List<uint>();
    private readonly List<long> _refs = new List<long>();
    private long _id;
    private Info _info;
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
    public List<long> refs
    {
      get
      {
        return this._refs;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
