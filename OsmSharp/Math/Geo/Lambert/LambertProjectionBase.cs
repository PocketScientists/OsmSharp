using OsmSharp.Math.Geo.Lambert.Ellipsoids;
using OsmSharp.Math.Geo.Lambert.International.Belgium;
using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Geo.Lambert
{
  public abstract class LambertProjectionBase
  {
    private string _name;
    private LambertEllipsoid _ellipsoid;
    private double _standard_parallel_1;
    public double _standard_parallel_1_radians;
    private double _standard_parallel_2;
    public double _standard_parallel_2_radians;
    private double _latitude_origin;
    public double _latitude_origin_radians;
    private double _longitude_origin;
    public double _longitude_origin_radians;
    private double _x_origin;
    private double _y_origin;
    private double _m_1;
    private double _m_2;
    private double _t_0;
    private double _t_1;
    private double _t_2;
    private double _n;
    private double _g;
    private double _r_0;
    private static Belgium1972LambertProjection _belgium_1972_lambert_projection;

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public static Belgium1972LambertProjection Belgium1972LambertProjection
    {
      get
      {
        if (LambertProjectionBase._belgium_1972_lambert_projection == null)
          LambertProjectionBase._belgium_1972_lambert_projection = new Belgium1972LambertProjection();
        return LambertProjectionBase._belgium_1972_lambert_projection;
      }
    }

    protected LambertProjectionBase(string name, LambertEllipsoid ellipsoid, double standard_parallel_1, double standard_parallel_2, double latitude_origin_1, double longitude_origin_2, double x_origin, double y_origin)
    {
      this._name = name;
      this._ellipsoid = ellipsoid;
      this._standard_parallel_1 = standard_parallel_1;
      this._standard_parallel_1_radians = new Degree(this._standard_parallel_1).Value;
      this._standard_parallel_2 = standard_parallel_2;
      this._standard_parallel_2_radians = new Degree(this._standard_parallel_2).Value;
      this._latitude_origin = latitude_origin_1;
      this._latitude_origin_radians = new Degree(this._latitude_origin).Value;
      this._longitude_origin = longitude_origin_2;
      this._longitude_origin_radians = new Degree(this._longitude_origin).Value;
      this._x_origin = x_origin;
      this._y_origin = y_origin;
      this._m_1 = System.Math.Cos(this._standard_parallel_1_radians) / System.Math.Sqrt(1.0 - this._ellipsoid.Eccentricity * this._ellipsoid.Eccentricity * System.Math.Pow(System.Math.Sin(this._standard_parallel_1_radians), 2.0));
      this._m_2 = System.Math.Cos(this._standard_parallel_2_radians) / System.Math.Sqrt(1.0 - this._ellipsoid.Eccentricity * this._ellipsoid.Eccentricity * System.Math.Pow(System.Math.Sin(this._standard_parallel_2_radians), 2.0));
      this._t_0 = System.Math.Tan(System.Math.PI / 4.0 - this._latitude_origin_radians / 2.0) / System.Math.Pow((1.0 - this._ellipsoid.Eccentricity * System.Math.Sin(this._latitude_origin_radians)) / (1.0 + this._ellipsoid.Eccentricity * System.Math.Sin(this._latitude_origin_radians)), this._ellipsoid.Eccentricity / 2.0);
      this._t_1 = System.Math.Tan(System.Math.PI / 4.0 - this._standard_parallel_1_radians / 2.0) / System.Math.Pow((1.0 - this._ellipsoid.Eccentricity * System.Math.Sin(this._standard_parallel_1_radians)) / (1.0 + this._ellipsoid.Eccentricity * System.Math.Sin(this._standard_parallel_1_radians)), this._ellipsoid.Eccentricity / 2.0);
      this._t_2 = System.Math.Tan(System.Math.PI / 4.0 - this._standard_parallel_2_radians / 2.0) / System.Math.Pow((1.0 - this._ellipsoid.Eccentricity * System.Math.Sin(this._standard_parallel_2_radians)) / (1.0 + this._ellipsoid.Eccentricity * System.Math.Sin(this._standard_parallel_2_radians)), this._ellipsoid.Eccentricity / 2.0);
      this._n = (System.Math.Log(this._m_1) - System.Math.Log(this._m_2)) / (System.Math.Log(this._t_1) - System.Math.Log(this._t_2));
      this._g = this._m_1 / (this._n * System.Math.Pow(this._t_1, this._n));
      this._r_0 = this._ellipsoid.SemiMajorAxis * this._g * System.Math.Pow(System.Math.Abs(this._t_0), this._n);
    }

    public GeoCoordinate ConvertToWGS84(double x, double y)
    {
      double d1 = System.Math.Pow(System.Math.Sqrt(System.Math.Pow(x - this._x_origin, 2.0) + System.Math.Pow(this._r_0 - (y - this._y_origin), 2.0)) / (this._ellipsoid.SemiMajorAxis * this._g), 1.0 / this._n);
      double num1 = System.Math.Atan((x - this._x_origin) / (this._r_0 - (y - this._y_origin))) / this._n + this._longitude_origin_radians;
      double a = System.Math.PI / 2.0 - 2.0 * System.Math.Atan(d1);
      double eccentricity1 = this._ellipsoid.Eccentricity;
      for (double num2 = 0.0; num2 != a; a = System.Math.PI / 2.0 - 2.0 * System.Math.Atan(d1 * System.Math.Pow((1.0 - eccentricity1 * System.Math.Sin(a)) / (1.0 + eccentricity1 * System.Math.Sin(a)), eccentricity1 / 2.0)))
        num2 = a;
      Hayford1924Ellipsoid hayford1924Ellipsoid = new Hayford1924Ellipsoid();
      double num3 = a;
      double num4 = num1;
      double num5 = 100.0;
      double semiMajorAxis1 = hayford1924Ellipsoid.SemiMajorAxis;
      double eccentricity2 = hayford1924Ellipsoid.Eccentricity;
      double num6 = eccentricity2 * eccentricity2;
      double num7 = System.Math.Sin(num3);
      double num8 = System.Math.Cos(num3);
      double num9 = System.Math.Sin(num4);
      double num10 = System.Math.Cos(num4);
      double num11 = semiMajorAxis1 / System.Math.Sqrt(1.0 - num6 * num7 * num7);
      double num12 = (num11 + num5) * num8 * num10;
      double num13 = (num11 + num5) * num8 * num9;
      double num14 = ((1.0 - num6) * num11 + num5) * num7;
      double num15 = 106.868628;
      double num16 = 52.297783;
      double num17 = 103.723893;
      double num18 = num12 - num15;
      double num19 = num13 + num16;
      double num20 = num17;
      double num21 = num14 - num20;
      double num22 = new Degree(9.34916666666667E-05).Value;
      double num23 = System.Math.Sin(num22);
      double num24 = System.Math.Cos(num22);
      double num25 = new Degree(-0.000126931944444444).Value;
      double num26 = System.Math.Sin(num25);
      double num27 = System.Math.Cos(num25);
      double num28 = new Degree(0.0005117175).Value;
      double num29 = System.Math.Sin(num28);
      double num30 = System.Math.Cos(num28);
      double num31 = num19 * num24 - num21 * num23;
      double num32 = num31 * num23 + num21 * num24;
      double num33 = num18 * num27 + num32 * num26;
      double num34 = num33 * -num26 + num32 * num27;
      double num35 = num33 * num30 - num31 * num29;
      double num36 = num35 * num29 + num31 * num30;
      Wgs1984Ellipsoid wgs1984Ellipsoid = new Wgs1984Ellipsoid();
      double eccentricity3 = wgs1984Ellipsoid.Eccentricity;
      double num37 = eccentricity3 * eccentricity3;
      double d2 = num35 * num35 + num36 * num36;
      double num38 = System.Math.Sqrt(d2);
      double num39 = num34 * num34;
      double num40 = System.Math.Sqrt(d2 + num39);
      double flattening = wgs1984Ellipsoid.Flattening;
      double semiMajorAxis2 = wgs1984Ellipsoid.SemiMajorAxis;
      double num41 = System.Math.Atan(num34 / num38 * (1.0 - flattening + num37 * semiMajorAxis2 / num40));
      double num42 = System.Math.Atan(num36 / num35);
      return new GeoCoordinate(new Radian(System.Math.Atan((num34 * (1.0 - flattening) + num37 * semiMajorAxis2 * System.Math.Pow(System.Math.Sin(num41), 3.0)) / ((1.0 - flattening) * (num38 - num37 * semiMajorAxis2 * System.Math.Pow(System.Math.Cos(num41), 3.0))))).Value, 
          new Radian(num42).Value);
    }
  }
}
