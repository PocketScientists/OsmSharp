using OsmSharp.Routing.Graphs.Geometric;
using Reminiscence.Arrays;

namespace OsmSharp.Routing.Network
{
  public class RoutingNetworkProfile
  {
    public static RoutingNetworkProfile NoCache = new RoutingNetworkProfile()
    {
      GeometricGraphProfile = GeometricGraphProfile.NoCache,
      EdgeDataProfile = (ArrayProfile) ArrayProfile.NoCache
    };
    public static RoutingNetworkProfile OneBuffer = new RoutingNetworkProfile()
    {
      GeometricGraphProfile = GeometricGraphProfile.OneBuffer,
      EdgeDataProfile = (ArrayProfile) ArrayProfile.OneBuffer
    };
    public static RoutingNetworkProfile Default = new RoutingNetworkProfile()
    {
      GeometricGraphProfile = GeometricGraphProfile.Default,
      EdgeDataProfile = (ArrayProfile) ArrayProfile.Aggressive8
    };

    public GeometricGraphProfile GeometricGraphProfile { get; set; }

    public ArrayProfile EdgeDataProfile { get; set; }
  }
}
