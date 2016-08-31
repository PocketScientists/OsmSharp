using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class Polygon : Geometry
  {
    public IEnumerable<LineairRing> Holes { get; private set; }

    public LineairRing Ring { get; set; }

    public override GeoCoordinateBox Box
    {
      get
      {
        GeoCoordinateBox box = this.Ring.Box;
        foreach (Geometry hole in this.Holes)
        {
          if (box == null)
            box = hole.Box;
          else
            box += hole.Box;
        }
        return box;
      }
    }

    public Polygon()
    {
      this.Holes = (IEnumerable<LineairRing>) new List<LineairRing>();
      this.Ring = new LineairRing();
    }

    public Polygon(LineairRing outline)
    {
      this.Holes = (IEnumerable<LineairRing>) new List<LineairRing>();
      this.Ring = outline;
    }

    public Polygon(LineairRing outline, IEnumerable<LineairRing> holes)
    {
      this.Holes = holes;
      this.Ring = outline;
    }

    public bool Contains(GeoCoordinate point)
    {
      if (!this.Ring.Contains(point))
        return false;
      foreach (LineairRing hole in this.Holes)
      {
        if (hole.Contains(point))
          return false;
      }
      return true;
    }

    public bool Contains(LineairRing lineairRing)
    {
      foreach (GeoCoordinate coordinate in lineairRing.Coordinates)
      {
        if (!this.Contains(coordinate))
          return false;
      }
      foreach (GeoCoordinate coordinate in this.Ring.Coordinates)
      {
        if (lineairRing.Contains(coordinate))
          return false;
      }
      foreach (LineString hole in this.Holes)
      {
        foreach (GeoCoordinate coordinate in hole.Coordinates)
        {
          if (lineairRing.Contains(coordinate))
            return false;
        }
      }
      return true;
    }

    public bool Contains(Polygon polygon)
    {
      return this.Contains(polygon);
    }

    public override bool IsInside(GeoCoordinateBox box)
    {
      throw new NotImplementedException();
    }
  }
}
