using OsmSharp.Math.Geo.Lambert.Ellipsoids;

namespace OsmSharp.Math.Geo.Lambert
{
  public abstract class LambertEllipsoid
  {
    private readonly double _semiMajorAxis;
    private readonly double _flattening;
    private readonly double _eccentricity;
    private static Hayford1924Ellipsoid _hayford1924Ellipsoid;
    private static Wgs1984Ellipsoid _wgs1984Ellipsoid;

    public double Eccentricity
    {
      get
      {
        return this._eccentricity;
      }
    }

    public double SemiMajorAxis
    {
      get
      {
        return this._semiMajorAxis;
      }
    }

    public double Flattening
    {
      get
      {
        return this._flattening;
      }
    }

    public static Hayford1924Ellipsoid Hayford1924Ellipsoid
    {
      get
      {
        if (LambertEllipsoid._hayford1924Ellipsoid == null)
          LambertEllipsoid._hayford1924Ellipsoid = new Hayford1924Ellipsoid();
        return LambertEllipsoid._hayford1924Ellipsoid;
      }
    }

    public static Wgs1984Ellipsoid Wgs1984Ellipsoid
    {
      get
      {
        if (LambertEllipsoid._wgs1984Ellipsoid == null)
          LambertEllipsoid._wgs1984Ellipsoid = new Wgs1984Ellipsoid();
        return LambertEllipsoid._wgs1984Ellipsoid;
      }
    }

    protected LambertEllipsoid(double semi_major_axis, double flattening)
    {
      this._semiMajorAxis = semi_major_axis;
      this._flattening = flattening;
      this._eccentricity = System.Math.Sqrt(this._flattening * (2.0 - this._flattening));
    }
  }
}
