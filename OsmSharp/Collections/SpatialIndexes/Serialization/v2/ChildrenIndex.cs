using ProtoBuf;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v2
{
  [ProtoContract]
  public class ChildrenIndex
  {
    [ProtoMember(1)]
    public float[] MinX { get; set; }

    [ProtoMember(2)]
    public float[] MinY { get; set; }

    [ProtoMember(3)]
    public float[] MaxX { get; set; }

    [ProtoMember(4)]
    public float[] MaxY { get; set; }

    [ProtoMember(5)]
    public int[] Starts { get; set; }

    [ProtoMember(6)]
    public int End { get; set; }

    [ProtoMember(7)]
    public bool[] IsLeaf { get; set; }
  }
}
