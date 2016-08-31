using OsmSharp.Routing.Graphs.Directed;
using OsmSharp.Routing.Network;

namespace OsmSharp.Routing
{
  public class RouterDbProfile
  {
    public static RouterDbProfile NoCache = new RouterDbProfile()
    {
      DirectedMetaGraphProfile = DirectedMetaGraphProfile.NoCache,
      RoutingNetworkProfile = RoutingNetworkProfile.NoCache
    };
    public static RouterDbProfile MobileLowEnd = new RouterDbProfile()
    {
      DirectedMetaGraphProfile = DirectedMetaGraphProfile.Aggressive40,
      RoutingNetworkProfile = RoutingNetworkProfile.NoCache
    };
    public static RouterDbProfile MobileHighEnd = new RouterDbProfile()
    {
      DirectedMetaGraphProfile = DirectedMetaGraphProfile.Aggressive40,
      RoutingNetworkProfile = RoutingNetworkProfile.Default
    };
    public static RouterDbProfile Default = new RouterDbProfile()
    {
      DirectedMetaGraphProfile = DirectedMetaGraphProfile.Aggressive40,
      RoutingNetworkProfile = RoutingNetworkProfile.Default
    };

    public RoutingNetworkProfile RoutingNetworkProfile { get; set; }

    public DirectedMetaGraphProfile DirectedMetaGraphProfile { get; set; }
  }
}
