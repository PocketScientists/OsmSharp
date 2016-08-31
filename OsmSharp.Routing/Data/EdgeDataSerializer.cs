using System;

namespace OsmSharp.Routing.Data
{
  public static class EdgeDataSerializer
  {
    public const ushort MAX_PROFILE_COUNT = 16384;
    public const float MAX_DISTANCE = 26214.3f;

    public static int Size
    {
      get
      {
        return 1;
      }
    }

    public static void Deserialize(uint value, out float distance, out ushort profile)
    {
      distance = (float) (value >> 14) / 10f;
      profile = (ushort) (value & 16383U);
    }

    public static EdgeData Deserialize(uint[] data)
    {
      float distance;
      ushort profile;
      EdgeDataSerializer.Deserialize(data[0], out distance, out profile);
      return new EdgeData()
      {
        Profile = profile,
        Distance = distance
      };
    }

    public static uint[] Serialize(float distance, ushort profile)
    {
      if ((double) distance > 26214.30078125)
        throw new ArgumentOutOfRangeException("Cannot store distance on edge, too big.");
      if ((double) distance < 0.0)
        throw new ArgumentOutOfRangeException("Cannot store distance on edge, too small.");
      if ((int) profile >= 16384)
        throw new ArgumentOutOfRangeException("Cannot store profile id on edge, too big.");
      uint num = (uint) ((double) distance * 10.0) << 14;
      return new uint[1]
      {
        (uint) profile + num
      };
    }

    public static uint[] Serialize(EdgeData data)
    {
      return EdgeDataSerializer.Serialize(data.Distance, data.Profile);
    }
  }
}
