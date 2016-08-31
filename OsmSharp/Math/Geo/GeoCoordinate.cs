using OsmSharp.Geo;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Math.Primitives;
using OsmSharp.Math.Random;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;

namespace OsmSharp.Math.Geo
{
  public class GeoCoordinate : PointF2D, ICoordinate
  {
    public double Longitude
    {
      get
      {
        return this[0];
      }
    }

    public double Latitude
    {
      get
      {
        return this[1];
      }
    }

    float ICoordinate.Latitude
    {
      get
      {
        return (float) this.Latitude;
      }
    }

    float ICoordinate.Longitude
    {
      get
      {
        return (float) this.Longitude;
      }
    }

    public GeoCoordinate(params double[] values)
      : base(values)
    {
    }

    public GeoCoordinate(ICoordinate coordinate)
      : this((double) coordinate.Latitude, (double) coordinate.Longitude)
    {
    }

    public GeoCoordinate(PointF2D point)
      : this(point[1], point[0])
    {
    }

    public GeoCoordinate(double latitude, double longitude)
      : base(new double[2]
      {
        longitude,
        latitude
      })
    {
    }

    public static GeoCoordinate operator +(GeoCoordinate a, GeoCoordinate b)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] + b[index];
      return new GeoCoordinate(values);
    }

    public static GeoCoordinate operator /(GeoCoordinate a, double value)
    {
      double[] values = new double[2];
      for (int index = 0; index < 2; ++index)
        values[index] = a[index] / value;
      return new GeoCoordinate(values);
    }

    public double Distance(GeoCoordinate point)
    {
      return PointF2D.Distance((PointF2D) this, (PointF2D) point);
    }

    public static double DistanceEstimateInMeter(ICoordinate location1, ICoordinate location2)
    {
      return GeoCoordinate.DistanceEstimateInMeter((double) location1.Latitude, (double) location1.Longitude, (double) location2.Latitude, (double) location2.Longitude);
    }

    public static double DistanceEstimateInMeter(double latitude1, double longitude1, double latitude2, double longitude2)
    {
      double num1 = 6371000.0;
      double num2 = latitude1 / 180.0 * System.Math.PI;
      double num3 = longitude1 / 180.0 * System.Math.PI;
      double num4 = latitude2 / 180.0 * System.Math.PI;
      double num5 = (longitude2 / 180.0 * System.Math.PI - num3) * System.Math.Cos((num2 + num4) / 2.0);
      double num6 = num4 - num2;
      return System.Math.Sqrt(num5 * num5 + num6 * num6) * num1;
    }

    public Meter DistanceEstimate(GeoCoordinate point)
    {
      return (Meter) GeoCoordinate.DistanceEstimateInMeter(this.Latitude, this.Longitude, point.Latitude, point.Longitude);
    }

    public Meter DistanceReal(GeoCoordinate point)
    {
      Meter meter = (Meter) 6371000.0;
      Radian radian1 = (Radian) new Degree(this.Latitude);
      Radian radian2 = (Radian) new Degree(this.Longitude);
      Radian radian3 = (Radian) new Degree(point.Latitude);
      Radian radian4 = (Radian) new Degree(point.Longitude);
      double num1 = (radian3 - radian1).Value;
      Radian radian5 = radian2;
      double num2 = (radian4 - radian5).Value;
      double d = System.Math.Pow(System.Math.Sin(num1 / 2.0), 2.0) + System.Math.Cos(radian1.Value) * System.Math.Cos(radian3.Value) * System.Math.Pow(System.Math.Sin(num2 / 2.0), 2.0);
      double num3 = 2.0 * System.Math.Atan2(System.Math.Sqrt(d), System.Math.Sqrt(1.0 - d));
      return (Meter) (meter.Value * num3);
    }

    public GeoCoordinate OffsetWithDistances(Meter meter)
    {
      GeoCoordinate geoCoordinate1 = new GeoCoordinate(this.Latitude + 0.1, this.Longitude);
      GeoCoordinate geoCoordinate2 = new GeoCoordinate(this.Latitude, this.Longitude + 0.1);
      Meter meter1 = geoCoordinate1.DistanceReal(this);
      Meter meter2 = geoCoordinate2.DistanceReal(this);
      return new GeoCoordinate(this.Latitude + meter.Value / meter1.Value * 0.1, this.Longitude + meter.Value / meter2.Value * 0.1);
    }

    public GeoCoordinate OffsetWithDirection(Meter distance, DirectionEnum direction)
    {
      double num = 6371000.0;
      Radian radian1 = (Radian) (distance.Value / num);
      Radian latitude1 = (Radian) (Degree) this.Latitude;
      Radian longitude1 = (Radian) (Degree) this.Longitude;
      Radian radian2 = (Radian) (Degree) ((double) direction);
      Radian radian3 = (Radian) System.Math.Asin(System.Math.Sin(latitude1.Value) * System.Math.Cos(radian1.Value) + System.Math.Cos(latitude1.Value) * System.Math.Sin(radian1.Value) * System.Math.Cos(radian2.Value));
      Radian radian4 = (Radian) (longitude1.Value + System.Math.Atan2(System.Math.Sin(radian2.Value) * System.Math.Sin(radian1.Value) * System.Math.Cos(latitude1.Value), System.Math.Cos(radian1.Value) - System.Math.Sin(latitude1.Value) * System.Math.Sin(radian3.Value)));
      double latitude2 = radian3.Value;
      if (latitude2 > 180.0)
        latitude2 -= 360.0;
      double longitude2 = radian4.Value;
      if (longitude2 > 180.0)
        longitude2 -= 360.0;
      return new GeoCoordinate(latitude2, longitude2);
    }

    public GeoCoordinate OffsetRandom(Meter meter)
    {
      return this.OffsetRandom(StaticRandomGenerator.Get(), meter);
    }

    public GeoCoordinate OffsetRandom(IRandomGenerator randomGenerator, Meter meter)
    {
      GeoCoordinate geoCoordinate = this.OffsetWithDistances((Meter) (meter.Value / System.Math.Sqrt(2.0)));
      double num1 = geoCoordinate.Latitude - this.Latitude;
      double num2 = geoCoordinate.Longitude - this.Longitude;
      return new GeoCoordinate(this.Latitude + (1.0 - randomGenerator.Generate(2.0)) * num1, this.Longitude + (1.0 - randomGenerator.Generate(2.0)) * num2);
    }

    public override string ToString()
    {
      return string.Format("[{0},{1}]", new object[2]
      {
        (object) this.Latitude.ToInvariantString(),
        (object) this.Longitude.ToInvariantString()
      });
    }

    public override int GetHashCode()
    {
      double num = this.Latitude;
      int hashCode1 = num.GetHashCode();
      num = this.Longitude;
      int hashCode2 = num.GetHashCode();
      return hashCode1 ^ hashCode2;
    }

    public override bool Equals(object obj)
    {
      if (obj is GeoCoordinate && (obj as GeoCoordinate).Latitude.Equals(this.Latitude))
        return (obj as GeoCoordinate).Longitude.Equals(this.Longitude);
      return false;
    }
  }
}
