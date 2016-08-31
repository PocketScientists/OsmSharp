namespace OsmSharp.Routing.Osm.Vehicles
{
  public class MotorCycle : MotorVehicle
  {
    public override string UniqueName
    {
      get
      {
        return "MotorCycle";
      }
    }

    public MotorCycle()
    {
      this.VehicleTypes.Add("motorcycle");
    }
  }
}
