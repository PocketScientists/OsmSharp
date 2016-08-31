using ProtoBuf;
using System.ComponentModel;
using System;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "BlockHeader")]
  public class BlobHeader : IExtensible
  {
    private string _type;
    private byte[] _indexdata;
    private int _datasize;
    private IExtension extensionObject;

    [ProtoMember(1)]
    public string type
    {
      get
      {
        return this._type;
      }
      set
      {
        this._type = value;
      }
    }

    [ProtoMember(2)]
    [DefaultValue(null)]
    public byte[] indexdata
    {
      get
      {
        return this._indexdata;
      }
      set
      {
        this._indexdata = value;
      }
    }

    [ProtoMember(3)]
    public int datasize
    {
      get
      {
        return this._datasize;
      }
      set
      {
        this._datasize = value;
      }
    }

        public IExtension GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
        }
    }
}
