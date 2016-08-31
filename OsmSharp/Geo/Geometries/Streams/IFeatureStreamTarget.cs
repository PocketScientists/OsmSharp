using OsmSharp.Geo.Features;

namespace OsmSharp.Geo.Geometries.Streams
{
  public interface IFeatureStreamTarget
  {
    void Initialize();

    void Add(Feature feature);

    void Close();
  }
}
