namespace OsmSharp
{
  public interface ITagsSource
  {
    bool TryGetValue(string key, out string value);
  }
}
