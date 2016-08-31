namespace OsmSharp.Units.Weight
{
  public class Kilogram : Unit
  {
    public Kilogram()
      : base(0.0)
    {
    }

    private Kilogram(double value)
      : base(value)
    {
    }

    public static implicit operator Kilogram(double value)
    {
      return new Kilogram(value);
    }

    public static implicit operator Kilogram(Gram gram)
    {
      return (Kilogram) (gram.Value / 1000.0);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "Kg";
    }
  }
}
