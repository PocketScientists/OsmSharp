using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "Node")]
  public class Node : IExtensible
  {
    private readonly List<uint> _keys = new List<uint>();
    private readonly List<uint> _vals = new List<uint>();
    private long _id;
    private Info _info;
    private long _lat;
    private long _lon;
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
    public long lat
    {
      get
      {
        return this._lat;
      }
      set
      {
        this._lat = value;
      }
    }

    [ProtoMember(9)]
    public long lon
    {
      get
      {
        return this._lon;
      }
      set
      {
        this._lon = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
