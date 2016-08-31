using OsmSharp.Geo;
using ProtoBuf;

namespace OsmSharp.Math.Geo.Simple
{
  [ProtoContract]
  public struct GeoCoordinateSimple : ICoordinate
  {
    [ProtoMember(1)]
    public float Latitude { get; set; }

    [ProtoMember(2)]
    public float Longitude { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is GeoCoordinateSimple))
        return false;
      GeoCoordinateSimple coordinateSimple = (GeoCoordinateSimple) obj;
      if ((double) coordinateSimple.Latitude == (double) this.Latitude)
        return (double) coordinateSimple.Longitude == (double) this.Longitude;
      return false;
    }

    public override int GetHashCode()
    {
      float num = this.Longitude;
      int hashCode1 = num.GetHashCode();
      num = this.Latitude;
      int hashCode2 = num.GetHashCode();
      return hashCode1 ^ hashCode2;
    }

    public override string ToString()
    {
      return string.Format("[{0}, {1}]", new object[2]
      {
        (object) this.Latitude.ToInvariantString(),
        (object) this.Longitude.ToInvariantString()
      });
    }
  }
}
