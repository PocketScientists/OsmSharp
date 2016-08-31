using OsmSharp.Units.Speed;

namespace OsmSharp.Routing.Osm.Vehicles
{
  public class Moped : MotorVehicle
  {
    public override string UniqueName
    {
      get
      {
        return "Moped";
      }
    }

    public Moped()
    {
      this.AccessibleTags.Remove("motorway");
      this.AccessibleTags.Remove("motorway_link");
      this.VehicleTypes.Add("moped");
    }

    public override KilometerPerHour MaxSpeed()
    {
      return (KilometerPerHour) 40.0;
    }
  }
}
