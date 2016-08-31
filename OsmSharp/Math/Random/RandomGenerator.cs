using System.Text;

namespace OsmSharp.Math.Random
{
  public class RandomGenerator : IRandomGenerator
  {
    private System.Random _random;

    public RandomGenerator()
    {
      this._random = new System.Random();
    }

    public RandomGenerator(int seed)
    {
      this._random = new System.Random(seed);
    }

    public int Generate(int max)
    {
      return this._random.Next(max);
    }

    public double Generate(double max)
    {
      return this._random.NextDouble() * max;
    }

    public void Generate(byte[] buffer)
    {
      this._random.NextBytes(buffer);
    }

    public string GenerateString(int length)
    {
      byte[] bytes = new byte[length * 2];
      int index = 0;
      while (index < length * 2)
      {
        int num = this.Generate(55295);
        bytes[index + 1] = (byte) ((num & 65280) >> 8);
        bytes[index] = (byte) (num & (int) byte.MaxValue);
        index += 2;
      }
      return Encoding.Unicode.GetString(bytes);
    }

    public int[] GenerateArray(int maxSize, int max)
    {
      int[] numArray = new int[this.Generate(maxSize)];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = this.Generate(max);
      return numArray;
    }
  }
}
