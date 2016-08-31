using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "Relation")]
  public class Relation : IExtensible
  {
    private readonly List<uint> _keys = new List<uint>();
    private readonly List<uint> _vals = new List<uint>();
    private readonly List<int> _roles_sid = new List<int>();
    private readonly List<long> _memids = new List<long>();
    private readonly List<Relation.MemberType> _types = new List<Relation.MemberType>();
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
    public List<int> roles_sid
    {
      get
      {
        return this._roles_sid;
      }
    }

    [ProtoMember(9)]
    public List<long> memids
    {
      get
      {
        return this._memids;
      }
    }

    [ProtoMember(10)]
    public List<Relation.MemberType> types
    {
      get
      {
        return this._types;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }

    [ProtoContract(Name = "MemberType")]
    public enum MemberType
    {
      [ProtoEnum(Name = "NODE", Value = 0)] NODE,
      [ProtoEnum(Name = "WAY", Value = 1)] WAY,
      [ProtoEnum(Name = "RELATION", Value = 2)] RELATION,
    }
  }
}
