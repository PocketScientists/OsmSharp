namespace OsmSharp.Units
{
  public abstract class Unit
  {
    private double _value;

    public double Value
    {
      get
      {
        return this._value;
      }
    }

    protected Unit(double value)
    {
      this._value = value;
    }
  }
}
