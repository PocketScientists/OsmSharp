using ProtoBuf;
using System.Collections.Generic;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "StringTable")]
  public class StringTable : IExtensible
  {
    private readonly List<byte[]> _s = new List<byte[]>();
    private IExtension extensionObject;

    [ProtoMember(1)]
    public List<byte[]> s
    {
      get
      {
        return this._s;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
