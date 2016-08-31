using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Geo.Meta
{
  public class RelativeDirection
  {
    public RelativeDirectionEnum Direction { get; set; }

    public Degree Angle { get; set; }

    public override string ToString()
    {
      return string.Format("{0}@{1}", new object[2]
      {
        (object) this.Direction,
        (object) this.Angle
      });
    }
  }
}
