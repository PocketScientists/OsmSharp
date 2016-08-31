namespace OsmSharp.Math.Random
{
  public interface IRandomGenerator
  {
    int Generate(int max);

    double Generate(double max);

    void Generate(byte[] buffer);

    string GenerateString(int length);
  }
}
