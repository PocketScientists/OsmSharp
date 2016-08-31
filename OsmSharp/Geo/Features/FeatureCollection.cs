using OsmSharp.Math.Geo;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Geo.Features
{
  public class FeatureCollection : IEnumerable<Feature>, IEnumerable
  {
    private readonly List<Feature> _features;

    public int Count
    {
      get
      {
        return this._features.Count;
      }
    }

    public Feature this[int idx]
    {
      get
      {
        return this._features[idx];
      }
    }

    public GeoCoordinateBox Box
    {
      get
      {
        GeoCoordinateBox geoCoordinateBox = (GeoCoordinateBox) null;
        foreach (Feature feature in this._features)
        {
          if (geoCoordinateBox == null)
            geoCoordinateBox = feature.Geometry.Box;
          else
            geoCoordinateBox += feature.Geometry.Box;
        }
        return geoCoordinateBox;
      }
    }

    public FeatureCollection()
    {
      this._features = new List<Feature>();
    }

    public FeatureCollection(IEnumerable<Feature> features)
    {
      this._features = new List<Feature>(features);
    }

    public void Add(Feature feature)
    {
      this._features.Add(feature);
    }

    public void AddRange(IEnumerable<Feature> features)
    {
      foreach (Feature feature in features)
        this.Add(feature);
    }

    public bool IsInside(GeoCoordinateBox box)
    {
      foreach (Feature feature in this._features)
      {
        if (feature.Geometry.IsInside(box))
          return true;
      }
      return false;
    }

    public void Clear()
    {
      this._features.Clear();
    }

    public IEnumerator<Feature> GetEnumerator()
    {
      return (IEnumerator<Feature>) this._features.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._features.GetEnumerator();
    }
  }
}
