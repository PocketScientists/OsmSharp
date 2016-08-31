namespace OsmSharp.Routing.Osm.Vehicles
{
  public class Bus : MotorVehicle
  {
    public override string UniqueName
    {
      get
      {
        return "Bus";
      }
    }

    public Bus()
    {
      this.VehicleTypes.Add("tourist_bus");
    }
  }
}
