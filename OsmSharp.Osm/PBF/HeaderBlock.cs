using ProtoBuf;
using System.Collections.Generic;
using System.ComponentModel;

namespace OsmSharp.Osm.PBF
{
  [ProtoContract(Name = "HeaderBlock")]
  public class HeaderBlock : IExtensible
  {
    private readonly List<string> _required_features = new List<string>();
    private readonly List<string> _optional_features = new List<string>();
    private string _writingprogram = "";
    private string _source = "";
    private HeaderBBox _bbox;
    private IExtension extensionObject;

    [ProtoMember(1)]
    [DefaultValue(null)]
    public HeaderBBox bbox
    {
      get
      {
        return this._bbox;
      }
      set
      {
        this._bbox = value;
      }
    }

    [ProtoMember(4)]
    public List<string> required_features
    {
      get
      {
        return this._required_features;
      }
    }

    [ProtoMember(5)]
    public List<string> optional_features
    {
      get
      {
        return this._optional_features;
      }
    }

    [ProtoMember(16)]
    [DefaultValue("")]
    public string writingprogram
    {
      get
      {
        return this._writingprogram;
      }
      set
      {
        this._writingprogram = value;
      }
    }

    [ProtoMember(17)]
    [DefaultValue("")]
    public string source
    {
      get
      {
        return this._source;
      }
      set
      {
        this._source = value;
      }
    }

    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
    {
      return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
    }
  }
}
