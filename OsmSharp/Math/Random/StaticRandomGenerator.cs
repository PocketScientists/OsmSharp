namespace OsmSharp.Math.Random
{
  public static class StaticRandomGenerator
  {
    private static IRandomGenerator _generator;

    public static IRandomGenerator Get()
    {
      if (StaticRandomGenerator._generator == null)
        StaticRandomGenerator._generator = (IRandomGenerator) new RandomGenerator();
      return StaticRandomGenerator._generator;
    }

    public static void Reset()
    {
      StaticRandomGenerator._generator = (IRandomGenerator) new RandomGenerator();
    }

    public static void Set(int seed)
    {
      StaticRandomGenerator._generator = (IRandomGenerator) new RandomGenerator(seed);
    }

    public static void Set(IRandomGenerator generator)
    {
      StaticRandomGenerator._generator = generator;
    }
  }
}
