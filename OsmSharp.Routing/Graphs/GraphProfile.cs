using Reminiscence.Arrays;

namespace OsmSharp.Routing.Graphs
{
  public class GraphProfile
  {
    public static GraphProfile NoCache = new GraphProfile()
    {
      VertexProfile = (ArrayProfile) ArrayProfile.NoCache,
      EdgeProfile = (ArrayProfile) ArrayProfile.NoCache
    };
    public static GraphProfile OneBuffer = new GraphProfile()
    {
      VertexProfile = (ArrayProfile) ArrayProfile.OneBuffer,
      EdgeProfile = (ArrayProfile) ArrayProfile.Aggressive8
    };
    public static GraphProfile Aggressive24;

    public ArrayProfile VertexProfile { get; set; }

    public ArrayProfile EdgeProfile { get; set; }

    static GraphProfile()
    {
      GraphProfile graphProfile = new GraphProfile();
      graphProfile.VertexProfile = (ArrayProfile) ArrayProfile.Aggressive8;
      ArrayProfile arrayProfile = new ArrayProfile();
      int num1 = 1024;
      arrayProfile.BufferSize = num1;
      int num2 = 16;
      arrayProfile.CacheSize = num2;
      graphProfile.EdgeProfile = arrayProfile;
      GraphProfile.Aggressive24 = graphProfile;
    }
  }
}
