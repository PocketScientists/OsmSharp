namespace OsmSharp.IO
{
  public static class FastBitConverter
  {
    public static uint ToUInt32(byte[] value, int startIndex)
    {
      return (uint) ((int) value[startIndex] | (int) value[startIndex + 1] << 8 | (int) value[startIndex + 2] << 16 | (int) value[startIndex + 3] << 24);
    }
  }
}
