using OsmSharp.Routing.Osm.Vehicles.Profiles;
using OsmSharp.Routing.Profiles;

namespace OsmSharp.Routing.Osm.Vehicles
{
  public class Car : MotorVehicle
  {
    public override string UniqueName
    {
      get
      {
        return "Car";
      }
    }

    public Car()
    {
      this.VehicleTypes.Add("motorcar");
    }

    public override Profile[] GetProfiles()
    {
      return new Profile[3]
      {
        this.Fastest(),
        this.Shortest(),
        this.Classifications()
      };
    }

    public Profile Classifications()
    {
      return (Profile) new CarClassifications(this);
    }
  }
}
