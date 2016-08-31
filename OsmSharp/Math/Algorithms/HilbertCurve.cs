using System.Collections.Generic;

namespace OsmSharp.Math.Algorithms
{
  public static class HilbertCurve
  {
    public static long HilbertDistance(float latitude, float longitude, long n)
    {
      long x = (long) (((double) longitude + 180.0) / 360.0 * (double) n);
      if (x >= n)
        x = n - 1L;
      long y = (long) (((double) latitude + 90.0) / 180.0 * (double) n);
      if (y >= n)
        y = n - 1L;
      return HilbertCurve.xy2d(n, x, y);
    }

    public static List<long> HilbertDistances(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude, long n)
    {
      float num1 = 180f / (float) n;
      float num2 = 360f / (float) n;
      List<long> longList = new List<long>((int) (((double) maxLatitude - (double) minLatitude) / (double) num1 * (((double) maxLongitude - (double) minLongitude) / (double) num2)));
      minLatitude = System.Math.Max(minLatitude - num1, -90f);
      minLongitude = System.Math.Max(minLongitude - num2, -180f);
      maxLatitude = System.Math.Min(maxLatitude + num1, 90f);
      maxLongitude = System.Math.Min(maxLongitude + num2, 180f);
      float latitude = minLatitude;
      while ((double) latitude < (double) maxLatitude)
      {
        float longitude = minLongitude;
        while ((double) longitude < (double) maxLongitude)
        {
          longList.Add(HilbertCurve.HilbertDistance(latitude, longitude, n));
          longitude += num2;
        }
        latitude += num1;
      }
      return longList;
    }

    private static long xy2d(long n, long x, long y)
    {
      long num = 0;
      long n1 = n / 2L;
      while (n1 > 0L)
      {
        long rx = (x & n1) > 0L ? 1L : 0L;
        long ry = (y & n1) > 0L ? 1L : 0L;
        num += n1 * n1 * (3L * rx ^ ry);
        HilbertCurve.rot(n1, ref x, ref y, rx, ry);
        n1 /= 2L;
      }
      return num;
    }

    private static void d2xy(long n, long d, out long x, long y)
    {
      long num = d;
      x = y = 0L;
      long n1 = 1;
      while (n1 < n)
      {
        long rx = 1L & num / 2L;
        long ry = 1L & (num ^ rx);
        HilbertCurve.rot(n1, ref x, ref y, rx, ry);
        x = x + n1 * rx;
        y += n1 * ry;
        num /= 4L;
        n1 *= 2L;
      }
    }

    private static void rot(long n, ref long x, ref long y, long rx, long ry)
    {
      if (ry != 0L)
        return;
      if (rx == 1L)
      {
        x = n - 1L - x;
        y = n - 1L - y;
      }
      long num = x;
      x = y;
      y = num;
    }
  }
}
