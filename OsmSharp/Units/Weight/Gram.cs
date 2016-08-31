namespace OsmSharp.Units.Weight
{
  public class Gram : Unit
  {
    public Gram()
      : base(0.0)
    {
    }

    private Gram(double value)
      : base(value)
    {
    }

    public static implicit operator Gram(double value)
    {
      return new Gram(value);
    }

    public static implicit operator Gram(Kilogram kilogram)
    {
      return (Gram) (kilogram.Value * 1000.0);
    }

    public override string ToString()
    {
      return this.Value.ToString() + "g";
    }
  }
}
