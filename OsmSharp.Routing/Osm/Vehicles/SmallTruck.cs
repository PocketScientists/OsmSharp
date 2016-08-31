namespace OsmSharp.Routing.Osm.Vehicles
{
  public class SmallTruck : MotorVehicle
  {
    public override string UniqueName
    {
      get
      {
        return "SmallTruck";
      }
    }

    public SmallTruck()
    {
      this.VehicleTypes.Add("goods");
    }
  }
}
