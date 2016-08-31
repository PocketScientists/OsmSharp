using Reminiscence.Arrays;

namespace OsmSharp.Routing.Graphs.Directed
{
  public class DirectedGraphProfile
  {
    public static DirectedGraphProfile NoCache = new DirectedGraphProfile()
    {
      VertexProfile = (ArrayProfile) ArrayProfile.NoCache,
      EdgeProfile = (ArrayProfile) ArrayProfile.NoCache
    };
    public static DirectedGraphProfile OneBuffer = new DirectedGraphProfile()
    {
      VertexProfile = (ArrayProfile) ArrayProfile.OneBuffer,
      EdgeProfile = (ArrayProfile) ArrayProfile.Aggressive8
    };
    public static DirectedGraphProfile Aggressive24;

    public ArrayProfile VertexProfile { get; set; }

    public ArrayProfile EdgeProfile { get; set; }

    static DirectedGraphProfile()
    {
      DirectedGraphProfile directedGraphProfile = new DirectedGraphProfile();
      directedGraphProfile.VertexProfile = (ArrayProfile) ArrayProfile.Aggressive8;
      ArrayProfile arrayProfile = new ArrayProfile();
      int num1 = 1024;
      arrayProfile.BufferSize = num1;
      int num2 = 16;
      arrayProfile.CacheSize = num2;
      directedGraphProfile.EdgeProfile = arrayProfile;
      DirectedGraphProfile.Aggressive24 = directedGraphProfile;
    }
  }
}
