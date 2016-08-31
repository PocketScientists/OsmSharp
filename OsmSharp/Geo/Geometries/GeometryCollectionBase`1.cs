using OsmSharp.Math.Geo;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public abstract class GeometryCollectionBase<GeometryType> : Geometry, IEnumerable<GeometryType>, IEnumerable where GeometryType : Geometry
  {
    private readonly List<GeometryType> _geometries;

    public int Count
    {
      get
      {
        return this._geometries.Count;
      }
    }

    public GeometryType this[int idx]
    {
      get
      {
        return this._geometries[idx];
      }
    }

    public override GeoCoordinateBox Box
    {
      get
      {
        GeoCoordinateBox geoCoordinateBox = (GeoCoordinateBox) null;
        foreach (GeometryType geometry in this._geometries)
        {
          if (geoCoordinateBox == null)
            geoCoordinateBox = geometry.Box;
          else
            geoCoordinateBox += geometry.Box;
        }
        return geoCoordinateBox;
      }
    }

    public GeometryCollectionBase()
    {
      this._geometries = new List<GeometryType>();
    }

    public GeometryCollectionBase(IEnumerable<GeometryType> geometries)
    {
      this._geometries = new List<GeometryType>(geometries);
    }

    public void Add(GeometryType geometry)
    {
      this._geometries.Add(geometry);
    }

    public void AddRange(IEnumerable<GeometryType> geometries)
    {
      foreach (GeometryType geometry in geometries)
        this.Add(geometry);
    }

    public override bool IsInside(GeoCoordinateBox box)
    {
      foreach (GeometryType geometry in this._geometries)
      {
        if (geometry.IsInside(box))
          return true;
      }
      return false;
    }

    public void Clear()
    {
      this._geometries.Clear();
    }

    public IEnumerator<GeometryType> GetEnumerator()
    {
      return (IEnumerator<GeometryType>) this._geometries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._geometries.GetEnumerator();
    }
  }
}
