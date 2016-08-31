using Reminiscence.Arrays;

namespace OsmSharp.Routing.Graphs.Directed
{
  public class DirectedMetaGraphProfile
  {
    public static DirectedMetaGraphProfile NoCache = new DirectedMetaGraphProfile()
    {
      DirectedGraphProfile = DirectedGraphProfile.NoCache,
      EdgeMetaProfile = (ArrayProfile) ArrayProfile.NoCache,
      VertexMetaProfile = (ArrayProfile) ArrayProfile.NoCache
    };
    public static DirectedMetaGraphProfile OneBuffer = new DirectedMetaGraphProfile()
    {
      DirectedGraphProfile = DirectedGraphProfile.OneBuffer,
      EdgeMetaProfile = (ArrayProfile) ArrayProfile.Aggressive8,
      VertexMetaProfile = (ArrayProfile) ArrayProfile.OneBuffer
    };
    public static DirectedMetaGraphProfile Aggressive40 = new DirectedMetaGraphProfile()
    {
      DirectedGraphProfile = DirectedGraphProfile.Aggressive24,
      EdgeMetaProfile = (ArrayProfile) ArrayProfile.Aggressive8,
      VertexMetaProfile = (ArrayProfile) ArrayProfile.Aggressive8
    };

    public DirectedGraphProfile DirectedGraphProfile { get; set; }

    public ArrayProfile VertexMetaProfile { get; set; }

    public ArrayProfile EdgeMetaProfile { get; set; }
  }
}
