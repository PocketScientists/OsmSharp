using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "DenseNodes")]
  public class DenseNodes : IExtensible
  {
    private readonly List<long> _id = new List<long>();
    private readonly List<long> _lat = new List<long>();
    private readonly List<long> _lon = new List<long>();
    private readonly List<int> _keys_vals = new List<int>();
    private DenseInfo _denseinfo;
    private IExtension extensionObject;

    [ProtoMember(1)]
    public List<long> id
    {
      get
      {
        return this._id;
      }
    }

    [ProtoMember(5)]
    [DefaultValue(null)]
    public DenseInfo denseinfo
    {
      get
      {
        return this._denseinfo;
      }
      set
      {
        this._denseinfo = value;
      }
    }

    [ProtoMember(8)]
    public List<long> lat
    {
      get
      {
        return this._lat;
      }
    }

    [ProtoMember(9)]
    public List<long> lon
    {
      get
      {
        return this._lon;
      }
    }

    [ProtoMember(10)]
    public List<int> keys_vals
    {
      get
      {
        return this._keys_vals;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
