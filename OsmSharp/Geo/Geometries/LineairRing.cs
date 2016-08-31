using OsmSharp.Math.Geo;
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class LineairRing : LineString
  {
    public LineairRing()
    {
    }

    public LineairRing(IEnumerable<GeoCoordinate> coordinates)
      : base(coordinates)
    {
    }

    public LineairRing(IList<ICoordinate> coordinates)
      : base(coordinates)
    {
    }

    public LineairRing(params GeoCoordinate[] coordinates)
      : base(coordinates)
    {
    }

    public bool IsEar(int vertexIdx)
    {
      int index1 = vertexIdx == 0 ? this.Coordinates.Count - 1 : vertexIdx - 1;
      int index2 = vertexIdx == this.Coordinates.Count - 1 ? 0 : vertexIdx + 1;
      GeoCoordinate coordinate1 = this.Coordinates[vertexIdx];
      GeoCoordinate coordinate2 = this.Coordinates[index1];
      return this.Contains((this.Coordinates[index2] + coordinate2) / 2.0);
    }

    public GeoCoordinate[] GetNeigbours(int vertexIdx)
    {
      return new GeoCoordinate[2]
      {
        this.Coordinates[vertexIdx == 0 ? this.Coordinates.Count - 1 : vertexIdx - 1],
        this.Coordinates[vertexIdx == this.Coordinates.Count - 1 ? 0 : vertexIdx + 1]
      };
    }

    public bool Contains(GeoCoordinate coordinate)
    {
      bool flag1 = false;
      int index1 = 0;
      int index2 = this.Coordinates.Count - 1;
      for (; index1 < this.Coordinates.Count; index2 = index1++)
      {
        if (this.Coordinates[index2].Equals((object) coordinate))
          return true;
        bool flag2 = this.Coordinates[index1].Latitude <= coordinate.Latitude;
        if (flag2 != this.Coordinates[index2].Latitude <= coordinate.Latitude)
        {
          double num = (this.Coordinates[index2].Longitude - this.Coordinates[index1].Longitude) * (coordinate.Latitude - this.Coordinates[index1].Latitude) - (this.Coordinates[index2].Latitude - this.Coordinates[index1].Latitude) * (coordinate.Longitude - this.Coordinates[index1].Longitude);
          if (num > 0.0 & flag2 || num < 0.0 && !flag2)
            flag1 = !flag1;
          else if (num == 0.0)
            return true;
        }
      }
      return flag1;
    }

    public bool Contains(LineairRing lineairRing)
    {
      foreach (GeoCoordinate coordinate in lineairRing.Coordinates)
      {
        if (!this.Contains(coordinate))
          return false;
      }
      foreach (GeoCoordinate coordinate in this.Coordinates)
      {
        if (lineairRing.Contains(coordinate))
          return false;
      }
      return true;
    }
  }
}
