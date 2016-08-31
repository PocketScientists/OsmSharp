using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "PrimitiveGroup")]
  public class PrimitiveGroup : IExtensible
  {
    private readonly List<Node> _nodes = new List<Node>();
    private readonly List<Way> _ways = new List<Way>();
    private readonly List<Relation> _relations = new List<Relation>();
    private readonly List<ChangeSet> _changesets = new List<ChangeSet>();
    private DenseNodes _dense;
    private IExtension extensionObject;

    [ProtoMember(1)]
    public List<Node> nodes
    {
      get
      {
        return this._nodes;
      }
    }

    [ProtoMember(2)]
    [DefaultValue(null)]
    public DenseNodes dense
    {
      get
      {
        return this._dense;
      }
      set
      {
        this._dense = value;
      }
    }

    [ProtoMember(3)]
    public List<Way> ways
    {
      get
      {
        return this._ways;
      }
    }

    [ProtoMember(4)]
    public List<Relation> relations
    {
      get
      {
        return this._relations;
      }
    }

    [ProtoMember(5)]
    public List<ChangeSet> changesets
    {
      get
      {
        return this._changesets;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
