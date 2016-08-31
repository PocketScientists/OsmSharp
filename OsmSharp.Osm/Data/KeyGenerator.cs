namespace OsmSharp.Osm.Data
{
  public static class KeyGenerator
  {
    private static int _current_id;

    public static int GenerateNew()
    {
      --KeyGenerator._current_id;
      return KeyGenerator._current_id;
    }
  }
}
