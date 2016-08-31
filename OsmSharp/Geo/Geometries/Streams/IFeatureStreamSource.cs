using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries.Streams
{
  public interface IFeatureStreamSource : IEnumerator<Feature>, IEnumerator, IDisposable, IEnumerable<Feature>, IEnumerable
  {
    bool HasBounds { get; }

    void Initialize();

    bool CanReset();

    void Close();

    GeoCoordinateBox GetBounds();
  }
}
