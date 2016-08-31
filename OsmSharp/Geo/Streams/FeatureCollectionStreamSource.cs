using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries.Streams;
using OsmSharp.Math.Geo;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Geo.Streams
{
  public class FeatureCollectionStreamSource : IFeatureStreamSource, IEnumerator<Feature>, IEnumerator, IDisposable, IEnumerable<Feature>, IEnumerable
  {
    private IEnumerator<Feature> _enumerator;

    public FeatureCollection FeatureCollection { get; private set; }

    public bool HasBounds
    {
      get
      {
        return true;
      }
    }

    public Feature Current
    {
      get
      {
        if (this._enumerator == null)
          throw new InvalidOperationException("Stream not initialized.");
        return this._enumerator.Current;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.Current;
      }
    }

    public FeatureCollectionStreamSource(FeatureCollection collection)
    {
      this.FeatureCollection = collection;
    }

    public virtual void Initialize()
    {
      this._enumerator = this.FeatureCollection.GetEnumerator();
    }

    public virtual bool CanReset()
    {
      return true;
    }

    public virtual void Close()
    {
      this._enumerator = (IEnumerator<Feature>) null;
    }

    public GeoCoordinateBox GetBounds()
    {
      return this.FeatureCollection.Box;
    }

    public virtual void Dispose()
    {
    }

    public virtual bool MoveNext()
    {
      if (this._enumerator == null)
        throw new InvalidOperationException("Stream not initialized.");
      return this._enumerator.MoveNext();
    }

    public virtual void Reset()
    {
      this._enumerator = (IEnumerator<Feature>) null;
      this.Initialize();
    }

    public IEnumerator<Feature> GetEnumerator()
    {
      this.Initialize();
      return (IEnumerator<Feature>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
