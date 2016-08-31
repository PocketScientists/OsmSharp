using OsmSharp.Collections.Tags;
using OsmSharp.IO;
using OsmSharp.Routing.Attributes;
using OsmSharp.Routing.Graphs.Directed;
using OsmSharp.Routing.Graphs.Geometric;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using Reminiscence.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OsmSharp.Routing
{
  public class RouterDb
  {
    private readonly RoutingNetwork _network;
    private readonly AttributesIndex _edgeProfiles;
    private readonly AttributesIndex _meta;
    private readonly TagsCollectionBase _dbMeta;
    private Guid _guid;
    private readonly Dictionary<string, DirectedMetaGraph> _contracted;
    private readonly HashSet<string> _supportedProfiles;

    public Guid Guid
    {
      get
      {
        return this._guid;
      }
    }

    public bool IsEmpty
    {
      get
      {
        return (int) this._network.VertexCount == 0;
      }
    }

    public RoutingNetwork Network
    {
      get
      {
        return this._network;
      }
    }

    public AttributesIndex EdgeProfiles
    {
      get
      {
        return this._edgeProfiles;
      }
    }

    public AttributesIndex EdgeMeta
    {
      get
      {
        return this._meta;
      }
    }

    public TagsCollectionBase Meta
    {
      get
      {
        return this._dbMeta;
      }
    }

    public bool HasContracted
    {
      get
      {
        return this._contracted.Count > 0;
      }
    }

    public RouterDb(float maxEdgeDistance = 5000f)
    {
      this._network = new RoutingNetwork(new GeometricGraph(1), maxEdgeDistance);
      this._edgeProfiles = new AttributesIndex(AttributesIndexMode.ReverseAll | AttributesIndexMode.IncreaseOne);
      this._meta = new AttributesIndex(AttributesIndexMode.ReverseStringIndexKeysOnly);
      this._dbMeta = (TagsCollectionBase) new TagsCollection();
      this._supportedProfiles = new HashSet<string>();
      this._contracted = new Dictionary<string, DirectedMetaGraph>();
      this._guid = Guid.NewGuid();
    }

    public RouterDb(MemoryMap map, float maxEdgeDistance = 5000f)
    {
      this._network = new RoutingNetwork(map, RoutingNetworkProfile.NoCache, maxEdgeDistance);
      this._edgeProfiles = new AttributesIndex(AttributesIndexMode.ReverseAll | AttributesIndexMode.IncreaseOne);
      this._meta = new AttributesIndex(map, AttributesIndexMode.ReverseStringIndexKeysOnly);
      this._dbMeta = (TagsCollectionBase) new TagsCollection();
      this._supportedProfiles = new HashSet<string>();
      this._contracted = new Dictionary<string, DirectedMetaGraph>();
      this._guid = Guid.NewGuid();
    }

    public RouterDb(MemoryMap map, RoutingNetworkProfile profile, float maxEdgeDistance = 5000f)
    {
      this._network = new RoutingNetwork(map, profile, maxEdgeDistance);
      this._edgeProfiles = new AttributesIndex(map, AttributesIndexMode.ReverseAll | AttributesIndexMode.IncreaseOne);
      this._meta = new AttributesIndex(map, AttributesIndexMode.ReverseAll);
      this._dbMeta = (TagsCollectionBase) new TagsCollection();
      this._supportedProfiles = new HashSet<string>();
      this._contracted = new Dictionary<string, DirectedMetaGraph>();
      this._guid = Guid.NewGuid();
    }

    public RouterDb(RoutingNetwork network, AttributesIndex profiles, AttributesIndex meta, TagsCollectionBase dbMeta, params Profile[] supportedProfiles)
    {
      this._network = network;
      this._edgeProfiles = profiles;
      this._meta = meta;
      this._dbMeta = dbMeta;
      this._supportedProfiles = new HashSet<string>();
      foreach (Profile supportedProfile in supportedProfiles)
        this._supportedProfiles.Add(supportedProfile.Name);
      this._contracted = new Dictionary<string, DirectedMetaGraph>();
      this._guid = Guid.NewGuid();
    }

    private RouterDb(Guid guid, RoutingNetwork network, AttributesIndex profiles, AttributesIndex meta, TagsCollectionBase dbMeta, string[] supportedProfiles)
    {
      this._guid = guid;
      this._network = network;
      this._edgeProfiles = profiles;
      this._meta = meta;
      this._dbMeta = dbMeta;
      this._supportedProfiles = new HashSet<string>();
      foreach (string supportedProfile in supportedProfiles)
        this._supportedProfiles.Add(supportedProfile);
      this._contracted = new Dictionary<string, DirectedMetaGraph>();
    }

    public void NewGuid()
    {
      this._guid = Guid.NewGuid();
    }

    public bool Supports(Profile profile)
    {
      return this._supportedProfiles.Contains(profile.Name);
    }

    public void AddSupportedProfile(Profile profile)
    {
      this._supportedProfiles.Add(profile.Name);
    }

    public void AddContracted(Profile profile, DirectedMetaGraph contracted)
    {
      if (!this.Supports(profile))
        throw new ArgumentOutOfRangeException("Cannot add a contracted version of the network for an unsupported profile.");
      this._contracted[profile.Name] = contracted;
    }

    public bool RemoveContracted(Profile profile)
    {
      return this._contracted.Remove(profile.Name);
    }

    public bool TryGetContracted(Profile profile, out DirectedMetaGraph contracted)
    {
      return this._contracted.TryGetValue(profile.Name, out contracted);
    }

    public bool HasContractedFor(Profile profile)
    {
      return this._contracted.ContainsKey(profile.Name);
    }

    public long Serialize(Stream stream)
    {
      return this.Serialize(stream, true);
    }

    public long Serialize(Stream stream, bool toReadonly)
    {
      long position = stream.Position;
      long num1 = 1;
      stream.WriteByte((byte) 1);
      stream.Write(this._guid.ToByteArray(), 0, 16);
      long num2 = num1 + 16L + stream.WriteWithSize(this._supportedProfiles.ToArray<string>()) + this._dbMeta.WriteWithSize(stream);
      if (this._contracted.Count > (int) byte.MaxValue)
        throw new Exception("Cannot serialize a router db with more than 255 contracted graphs.");
      stream.WriteByte((byte) this._contracted.Count);
      long num3 = num2 + 1L + this._edgeProfiles.Serialize((Stream) new LimitedStream(stream));
      stream.Seek(position + num3, SeekOrigin.Begin);
      long num4 = num3 + this._meta.Serialize((Stream) new LimitedStream(stream));
      stream.Seek(position + num4, SeekOrigin.Begin);
      long num5 = num4 + this._network.Serialize((Stream) new LimitedStream(stream));
      stream.Seek(position + num5, SeekOrigin.Begin);
      foreach (KeyValuePair<string, DirectedMetaGraph> keyValuePair in this._contracted)
      {
        num5 += stream.WriteWithSize(keyValuePair.Key);
        num5 += keyValuePair.Value.Serialize((Stream) new LimitedStream(stream), toReadonly);
      }
      return num5;
    }

    public long SerializeContracted(Profile profile, Stream stream)
    {
      DirectedMetaGraph contracted;
      if (!this.TryGetContracted(profile, out contracted))
        throw new Exception(string.Format("Contracted graph for profile {0} not found.", (object) profile.Name));
      Guid guid = this.Guid;
      long num1 = 16;
      stream.Write(guid.ToByteArray(), 0, 16);
      long num2 = stream.WriteWithSize(profile.Name);
      return num1 + num2 + contracted.Serialize(stream, true);
    }

    public void DeserializeAndAddContracted(Stream stream)
    {
      this.DeserializeAndAddContracted(stream, (DirectedMetaGraphProfile) null);
    }

    public void DeserializeAndAddContracted(Stream stream, DirectedMetaGraphProfile profile)
    {
      byte[] numArray = new byte[16];
      stream.Read(numArray, 0, 16);
      if (new Guid(numArray) != this.Guid)
        throw new Exception("Cannot add this contracted graph, guid's do not match.");
      this._contracted[stream.ReadWithSizeString()] = DirectedMetaGraph.Deserialize(stream, profile);
    }

    public static RouterDb Deserialize(Stream stream)
    {
      return RouterDb.Deserialize(stream, (RouterDbProfile) null);
    }

    public static RouterDb Deserialize(Stream stream, RouterDbProfile profile)
    {
      int num1 = stream.ReadByte();
      if (num1 != 1)
        throw new Exception(string.Format("Cannot deserialize routing db: Invalid version #: {0}.", (object) num1));
      byte[] numArray = new byte[16];
      stream.Read(numArray, 0, 16);
      Guid guid = new Guid(numArray);
      string[] strArray = stream.ReadWithSizeStringArray();
      TagsCollectionBase tagsCollectionBase = stream.ReadWithSizeTagsCollection();
      int num2 = stream.ReadByte();
      AttributesIndex attributesIndex1 = AttributesIndex.Deserialize((Stream) new LimitedStream(stream), true);
      AttributesIndex attributesIndex2 = AttributesIndex.Deserialize((Stream) new LimitedStream(stream), true);
      RoutingNetwork network = RoutingNetwork.Deserialize(stream, profile == null ? (RoutingNetworkProfile) null : profile.RoutingNetworkProfile);
      AttributesIndex profiles = attributesIndex1;
      AttributesIndex meta = attributesIndex2;
      TagsCollectionBase dbMeta = tagsCollectionBase;
      string[] supportedProfiles = strArray;
      RouterDb routerDb = new RouterDb(guid, network, profiles, meta, dbMeta, supportedProfiles);
      for (int index1 = 0; index1 < num2; ++index1)
      {
        string index2 = stream.ReadWithSizeString();
        DirectedMetaGraph directedMetaGraph = DirectedMetaGraph.Deserialize(stream, profile == null ? (DirectedMetaGraphProfile) null : profile.DirectedMetaGraphProfile);
        routerDb._contracted[index2] = directedMetaGraph;
      }
      return routerDb;
    }
  }
}
