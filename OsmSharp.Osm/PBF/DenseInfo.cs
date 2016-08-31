using ProtoBuf;
using System.Collections.Generic;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "DenseInfo")]
  public class DenseInfo : IExtensible
  {
    private readonly List<int> _version = new List<int>();
    private readonly List<long> _timestamp = new List<long>();
    private readonly List<long> _changeset = new List<long>();
    private readonly List<int> _uid = new List<int>();
    private readonly List<int> _user_sid = new List<int>();
    private IExtension extensionObject;

    [ProtoMember(1)]
    public List<int> version
    {
      get
      {
        return this._version;
      }
    }

    [ProtoMember(2)]
    public List<long> timestamp
    {
      get
      {
        return this._timestamp;
      }
    }

    [ProtoMember(3)]
    public List<long> changeset
    {
      get
      {
        return this._changeset;
      }
    }

    [ProtoMember(4)]
    public List<int> uid
    {
      get
      {
        return this._uid;
      }
    }

    [ProtoMember(5)]
    public List<int> user_sid
    {
      get
      {
        return this._user_sid;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
