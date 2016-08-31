namespace OsmSharp.Routing.Osm.Vehicles
{
  public class BigTruck : MotorVehicle
  {
    public override string UniqueName
    {
      get
      {
        return "BigTruck";
      }
    }

    public BigTruck()
    {
      this.VehicleTypes.Add("hgv");
    }
  }
}
