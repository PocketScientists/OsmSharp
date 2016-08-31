using ProtoBuf;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "Blob")]
  public class Blob : IExtensible
  {
    private byte[] _raw;
    private int _raw_size;
    private byte[] _zlib_data;
    private byte[] _lzma_data;
    private byte[] _bzip2_data;
    private IExtension extensionObject;

    [ProtoMember(1)]
    [DefaultValue(null)]
    public byte[] raw
    {
      get
      {
        return this._raw;
      }
      set
      {
        this._raw = value;
      }
    }

    [ProtoMember(2)]
    [DefaultValue(0)]
    public int raw_size
    {
      get
      {
        return this._raw_size;
      }
      set
      {
        this._raw_size = value;
      }
    }

    [ProtoMember(3)]
    [DefaultValue(null)]
    public byte[] zlib_data
    {
      get
      {
        return this._zlib_data;
      }
      set
      {
        this._zlib_data = value;
      }
    }

    [ProtoMember(4)]
    [DefaultValue(null)]
    public byte[] lzma_data
    {
      get
      {
        return this._lzma_data;
      }
      set
      {
        this._lzma_data = value;
      }
    }

    [ProtoMember(5)]
    [DefaultValue(null)]
    public byte[] bzip2_data
    {
      get
      {
        return this._bzip2_data;
      }
      set
      {
        this._bzip2_data = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
