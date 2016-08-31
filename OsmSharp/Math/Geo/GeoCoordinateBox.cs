using OsmSharp.Math.Primitives;
using OsmSharp.Math.Random;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.Geo
{
  public class GeoCoordinateBox : BoxF2D
  {
    public double MaxLon
    {
      get
      {
        return this.Max[0];
      }
    }

    public double MaxLat
    {
      get
      {
        return this.Max[1];
      }
    }

    public double MinLon
    {
      get
      {
        return this.Min[0];
      }
    }

    public double MinLat
    {
      get
      {
        return this.Min[1];
      }
    }

    public GeoCoordinate TopLeft
    {
      get
      {
        return new GeoCoordinate(this.MaxLat, this.MinLon);
      }
    }

    public GeoCoordinate TopRight
    {
      get
      {
        return new GeoCoordinate(this.MaxLat, this.MaxLon);
      }
    }

    public GeoCoordinate BottomLeft
    {
      get
      {
        return new GeoCoordinate(this.MinLat, this.MinLon);
      }
    }

    public GeoCoordinate BottomRight
    {
      get
      {
        return new GeoCoordinate(this.MinLat, this.MaxLon);
      }
    }

    public double DeltaLon
    {
      get
      {
        return this.Delta[0];
      }
    }

    public double DeltaLat
    {
      get
      {
        return this.Delta[1];
      }
    }

    public GeoCoordinate Center
    {
      get
      {
        return new GeoCoordinate((this.MaxLat + this.MinLat) / 2.0, (this.MaxLon + this.MinLon) / 2.0);
      }
    }

    public override PointF2D[] Corners
    {
      get
      {
        return new PointF2D[4]
        {
          (PointF2D) this.TopLeft,
          (PointF2D) this.TopRight,
          (PointF2D) this.BottomLeft,
          (PointF2D) this.BottomRight
        };
      }
    }

    public GeoCoordinateBox(IList<GeoCoordinate> points)
      : base(points.Cast<PointF2D>().ToArray<PointF2D>())
    {
    }

    public GeoCoordinateBox(GeoCoordinate[] points)
      : base((PointF2D[]) points)
    {
    }

    public GeoCoordinateBox(GeoCoordinate a, GeoCoordinate b)
      : base((PointF2D) a, (PointF2D) b)
    {
    }

    public static GeoCoordinateBox operator +(GeoCoordinateBox a, GeoCoordinate b)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      IEnumerable<GeoCoordinate> collection = a.Corners.Cast<GeoCoordinate>();
      geoCoordinateList.AddRange(collection);
      GeoCoordinate geoCoordinate = b;
      geoCoordinateList.Add(geoCoordinate);
      return new GeoCoordinateBox((IList<GeoCoordinate>) geoCoordinateList);
    }

    public static GeoCoordinateBox operator +(GeoCoordinateBox a, GeoCoordinateBox b)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      IEnumerable<GeoCoordinate> collection1 = a.Corners.Cast<GeoCoordinate>();
      geoCoordinateList.AddRange(collection1);
      IEnumerable<GeoCoordinate> collection2 = b.Corners.Cast<GeoCoordinate>();
      geoCoordinateList.AddRange(collection2);
      return new GeoCoordinateBox((IList<GeoCoordinate>) geoCoordinateList);
    }

    public static GeoCoordinateBox operator -(GeoCoordinateBox a, GeoCoordinateBox b)
    {
      if (!a.Overlaps((BoxF2D) b))
        return (GeoCoordinateBox) null;
      List<double> doubleList1 = new List<double>();
      doubleList1.Add(a.MinLon);
      doubleList1.Add(a.MaxLon);
      doubleList1.Add(b.MinLon);
      doubleList1.Add(b.MaxLon);
      doubleList1.Sort();
      List<double> doubleList2 = new List<double>();
      doubleList2.Add(a.MinLat);
      doubleList2.Add(a.MaxLat);
      doubleList2.Add(b.MinLat);
      doubleList2.Add(b.MaxLat);
      doubleList2.Sort();
      return new GeoCoordinateBox(new GeoCoordinate(doubleList2[1], doubleList1[1]), new GeoCoordinate(doubleList2[2], doubleList1[2]));
    }

    public void ExpandWith(GeoCoordinate coordinate)
    {
      if (this.Contains((PointF2D) coordinate))
        return;
      this.Mutate(new PointF2D[3]
      {
        (PointF2D) this.TopLeft,
        (PointF2D) this.BottomRight,
        (PointF2D) coordinate
      });
    }

    public GeoCoordinate GenerateRandomIn()
    {
      return this.GenerateRandomIn(StaticRandomGenerator.Get());
    }

    public GeoCoordinate GenerateRandomIn(IRandomGenerator rand)
    {
      return new GeoCoordinate(this.MinLat + rand.Generate(1.0) * this.DeltaLat, this.MinLon + rand.Generate(1.0) * this.DeltaLon);
    }

    public GeoCoordinate GenerateRandomIn(System.Random rand)
    {
      return new GeoCoordinate(this.MinLat + rand.NextDouble() * this.DeltaLat, this.MinLon + rand.NextDouble() * this.DeltaLon);
    }

    public GeoCoordinateBox Scale(double factor)
    {
      if (factor <= 0.0)
        throw new ArgumentOutOfRangeException();
      GeoCoordinate center = this.Center;
      double num1 = this.DeltaLat * factor / 2.0;
      double num2 = this.DeltaLon * factor / 2.0;
      return new GeoCoordinateBox(new GeoCoordinate(center.Latitude - num1, center.Longitude - num2), new GeoCoordinate(center.Latitude + num1, center.Longitude + num2));
    }

    public GeoCoordinateBox Intersection(GeoCoordinateBox box)
    {
      double longitude1 = System.Math.Max(this.Min[0], box.Min[0]);
      double latitude1 = System.Math.Max(this.Min[1], box.Min[1]);
      double longitude2 = System.Math.Min(this.Max[0], box.Max[0]);
      double latitude2 = System.Math.Min(this.Max[1], box.Max[1]);
      if (longitude1 <= longitude2 && latitude1 <= latitude2)
        return new GeoCoordinateBox(new GeoCoordinate(latitude1, longitude1), new GeoCoordinate(latitude2, longitude2));
      return (GeoCoordinateBox) null;
    }

    public GeoCoordinateBox Union(GeoCoordinateBox box)
    {
      double num = System.Math.Min(this.Min[0], box.Min[0]);
      double latitude1 = System.Math.Min(this.Min[1], box.Min[1]);
      double longitude1 = System.Math.Max(this.Max[0], box.Max[0]);
      double latitude2 = System.Math.Max(this.Max[1], box.Max[1]);
      double longitude2 = num;
      return new GeoCoordinateBox(new GeoCoordinate(latitude1, longitude2), new GeoCoordinate(latitude2, longitude1));
    }

    public GeoCoordinateBox Resize(double delta)
    {
      return new GeoCoordinateBox(new GeoCoordinate(this.MaxLat + delta, this.MaxLon + delta), new GeoCoordinate(this.MinLat - delta, this.MinLon - delta));
    }
  }
}
