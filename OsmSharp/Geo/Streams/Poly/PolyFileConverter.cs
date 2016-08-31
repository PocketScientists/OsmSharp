using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OsmSharp.Geo.Streams.Poly
{
  public static class PolyFileConverter
  {
    private static string END_TOKEN = "END";

    public static Feature ReadPolygon(string poly)
    {
      return PolyFileConverter.ReadPolygon((TextReader) new StringReader(poly));
    }

    public static Feature ReadPolygon(Stream stream)
    {
      return PolyFileConverter.ReadPolygon((TextReader) new StreamReader(stream));
    }

    public static Feature ReadPolygon(TextReader reader)
    {
      string str = reader.ReadLine();
      LineairRing outline = PolyFileConverter.ReadRing(reader);
      LineairRing lineairRing = PolyFileConverter.ReadRing(reader);
      List<LineairRing> lineairRingList = new List<LineairRing>();
      for (; lineairRing != null; lineairRing = PolyFileConverter.ReadRing(reader))
        lineairRingList.Add(lineairRing);
      return new Feature((Geometry) new Polygon(outline, (IEnumerable<LineairRing>) lineairRingList), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<GeometryAttribute>) new GeometryAttribute[1]
      {
        new GeometryAttribute()
        {
          Key = "name",
          Value = (object) str
        }
      }));
    }

    private static LineairRing ReadRing(TextReader reader)
    {
      string str1 = reader.ReadLine();
      if (str1 == null || PolyFileConverter.END_TOKEN.Equals(str1))
        return (LineairRing) null;
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      for (string str2 = reader.ReadLine(); str2 != null && !PolyFileConverter.END_TOKEN.Equals(str2); str2 = reader.ReadLine())
      {
        string[] strArray = str2.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);
        double result1;
        double result2;
        if (!double.TryParse(strArray[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1) || !double.TryParse(strArray[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          throw new Exception("Could not parse coordinates in poly.");
        geoCoordinateList.Add(new GeoCoordinate(result2, result1));
      }
      if (geoCoordinateList.Count < 3)
        throw new Exception("Could not parse poly, a minimum of three coordinates are required.");
      if ((PointF2D) geoCoordinateList[1] == (PointF2D) geoCoordinateList[geoCoordinateList.Count - 1])
        geoCoordinateList.RemoveAt(0);
      else if ((PointF2D) geoCoordinateList[0] != (PointF2D) geoCoordinateList[geoCoordinateList.Count - 1])
        geoCoordinateList.Add(geoCoordinateList[0]);
      return new LineairRing((IEnumerable<GeoCoordinate>) geoCoordinateList);
    }
  }
}
