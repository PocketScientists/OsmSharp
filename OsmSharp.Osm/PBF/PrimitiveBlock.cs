using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "PrimitiveBlock")]
  public class PrimitiveBlock : IExtensible
  {
    private readonly List<PrimitiveGroup> _primitivegroup = new List<PrimitiveGroup>();
    private int _granularity = 100;
    private int _date_granularity = 1000;
    private StringTable _stringtable;
    private long _lat_offset;
    private long _lon_offset;
    private IExtension extensionObject;

    [ProtoMember(1)]
    public StringTable stringtable
    {
      get
      {
        return this._stringtable;
      }
      set
      {
        this._stringtable = value;
      }
    }

    [ProtoMember(2)]
    public List<PrimitiveGroup> primitivegroup
    {
      get
      {
        return this._primitivegroup;
      }
    }

    [ProtoMember(17)]
    [DefaultValue(100)]
    public int granularity
    {
      get
      {
        return this._granularity;
      }
      set
      {
        this._granularity = value;
      }
    }

    [ProtoMember(19)]
    [DefaultValue(0)]
    public long lat_offset
    {
      get
      {
        return this._lat_offset;
      }
      set
      {
        this._lat_offset = value;
      }
    }

    [ProtoMember(20)]
    [DefaultValue(0)]
    public long lon_offset
    {
      get
      {
        return this._lon_offset;
      }
      set
      {
        this._lon_offset = value;
      }
    }

    [ProtoMember(18)]
    [DefaultValue(1000)]
    public int date_granularity
    {
      get
      {
        return this._date_granularity;
      }
      set
      {
        this._date_granularity = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
