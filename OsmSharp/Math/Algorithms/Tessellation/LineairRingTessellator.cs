using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Algorithms.Tessellation
{
  public class LineairRingTessellator
  {
    public GeoCoordinate[] Tessellate(LineairRing ring)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      if (ring.Coordinates.Count < 3)
        throw new ArgumentOutOfRangeException("Invalid ring detected, only 1 or 2 vertices.");
      List<GeoCoordinate> coordinates;
      for (LineairRing lineairRing = new LineairRing((IEnumerable<GeoCoordinate>) ring.Coordinates); lineairRing.Coordinates.Count > 3; lineairRing = new LineairRing((IEnumerable<GeoCoordinate>) coordinates))
      {
        int vertexIdx = 0;
        while (!lineairRing.IsEar(vertexIdx))
          ++vertexIdx;
        GeoCoordinate[] neigbours = lineairRing.GetNeigbours(vertexIdx);
        geoCoordinateList.Add(neigbours[0]);
        geoCoordinateList.Add(neigbours[1]);
        geoCoordinateList.Add(lineairRing.Coordinates[vertexIdx]);
        coordinates = lineairRing.Coordinates;
        int index = vertexIdx;
        coordinates.RemoveAt(index);
      }
      if (ring.Coordinates.Count == 3)
      {
        geoCoordinateList.Add(ring.Coordinates[0]);
        geoCoordinateList.Add(ring.Coordinates[1]);
        geoCoordinateList.Add(ring.Coordinates[2]);
      }
      return geoCoordinateList.ToArray();
    }
  }
}
