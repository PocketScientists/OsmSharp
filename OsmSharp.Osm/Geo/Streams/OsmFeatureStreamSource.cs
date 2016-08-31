using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries.Streams;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Geo.Interpreter;
using OsmSharp.Osm.Streams.Complete;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Osm.Geo.Streams
{
  public class OsmFeatureStreamSource : IFeatureStreamSource, IEnumerator<Feature>, IEnumerator, IDisposable, IEnumerable<Feature>, IEnumerable
  {
    private FeatureInterpreter _interpreter;
    private OsmCompleteStreamSource _source;
    private Feature _current;
    private IEnumerator<Feature> _currentEnumerator;

    public bool HasBounds
    {
      get
      {
        return false;
      }
    }

    public Feature Current
    {
      get
      {
        return this._current;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this._current;
      }
    }

    public OsmFeatureStreamSource(OsmCompleteStreamSource source)
    {
      this._source = source;
      this._interpreter = (FeatureInterpreter) new SimpleFeatureInterpreter();
    }

    public OsmFeatureStreamSource(OsmCompleteStreamSource source, FeatureInterpreter interpreter)
    {
      this._source = source;
      this._interpreter = interpreter;
    }

    public void Initialize()
    {
      this._source.Reset();
      this._source.Initialize();
      this._current = (Feature) null;
    }

    public bool CanReset()
    {
      return this._source.CanReset;
    }

    public void Close()
    {
      this._current = (Feature) null;
    }

    public GeoCoordinateBox GetBounds()
    {
      throw new InvalidOperationException("This source has no bounds, check HasBounds.");
    }

    public bool MoveNext()
    {
      if (this._currentEnumerator != null && this._currentEnumerator.MoveNext())
      {
        this._current = this._currentEnumerator.Current;
        return true;
      }
      this._currentEnumerator = (IEnumerator<Feature>) null;
      while (this._source.MoveNext())
      {
        FeatureCollection featureCollection = this._interpreter.Interpret(this._source.Current());
        if (featureCollection != null)
        {
          this._currentEnumerator = featureCollection.GetEnumerator();
          if (this._currentEnumerator.MoveNext())
          {
            this._current = this._currentEnumerator.Current;
            return true;
          }
          this._currentEnumerator.Dispose();
          this._currentEnumerator = (IEnumerator<Feature>) null;
        }
      }
      return false;
    }

    public void Reset()
    {
      this._current = (Feature) null;
      this._source.Reset();
    }

    public void Dispose()
    {
    }

    public IEnumerator<Feature> GetEnumerator()
    {
      this.Reset();
      return (IEnumerator<Feature>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this.Reset();
      return (IEnumerator) this;
    }
  }
}
