using Reminiscence.Arrays;

namespace OsmSharp.Routing.Graphs.Geometric
{
  public class GeometricGraphProfile
  {
    public static GeometricGraphProfile NoCache = new GeometricGraphProfile()
    {
      CoordinatesProfile = (ArrayProfile) ArrayProfile.NoCache,
      GraphProfile = GraphProfile.NoCache
    };
    public static GeometricGraphProfile OneBuffer = new GeometricGraphProfile()
    {
      CoordinatesProfile = (ArrayProfile) ArrayProfile.OneBuffer,
      GraphProfile = GraphProfile.OneBuffer
    };
    public static GeometricGraphProfile Aggressive32 = new GeometricGraphProfile()
    {
      CoordinatesProfile = (ArrayProfile) ArrayProfile.Aggressive8,
      GraphProfile = GraphProfile.Aggressive24
    };
    public static GeometricGraphProfile Default = new GeometricGraphProfile()
    {
      CoordinatesProfile = (ArrayProfile) ArrayProfile.NoCache,
      GraphProfile = GraphProfile.Aggressive24
    };

    public GraphProfile GraphProfile { get; set; }

    public ArrayProfile CoordinatesProfile { get; set; }
  }
}
