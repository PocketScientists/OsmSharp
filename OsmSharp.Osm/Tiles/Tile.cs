using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Tiles
{
  public class Tile
  {
    private ulong _id;

    public int X { get; private set; }

    public int Y { get; private set; }

    public int Zoom { get; private set; }

    public Tile Parent
    {
      get
      {
        return new Tile(this.X / 2, this.Y / 2, this.Zoom - 1);
      }
    }

    public GeoCoordinate TopLeft
    {
      get
      {
        return new GeoCoordinate(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(System.Math.PI - 2.0 * System.Math.PI * (double) this.Y / System.Math.Pow(2.0, (double) this.Zoom))), (double) this.X / System.Math.Pow(2.0, (double) this.Zoom) * 360.0 - 180.0);
      }
    }

    public GeoCoordinate BottomRight
    {
      get
      {
        return new GeoCoordinate(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(System.Math.PI - 2.0 * System.Math.PI * (double) (this.Y + 1) / System.Math.Pow(2.0, (double) this.Zoom))), (double) (this.X + 1) / System.Math.Pow(2.0, (double) this.Zoom) * 360.0 - 180.0);
      }
    }

    public GeoCoordinateBox Box
    {
      get
      {
        return new GeoCoordinateBox(this.TopLeft, this.BottomRight);
      }
    }

    public TileRange SubTiles
    {
      get
      {
        return new TileRange(2 * this.X, 2 * this.Y, 2 * this.X + 1, 2 * this.Y + 1, this.Zoom + 1);
      }
    }

    public ulong Id
    {
      get
      {
        return this._id;
      }
    }

    public bool IsValid
    {
      get
      {
        if (this.X < 0 || this.Y < 0 || this.Zoom < 0)
          return false;
        double num = System.Math.Pow(2.0, (double) this.Zoom);
        if ((double) this.X < num)
          return (double) this.Y < num;
        return false;
      }
    }

    public Tile(ulong id)
    {
      this._id = id;
      Tile tile = Tile.CalculateTile(id);
      this.X = tile.X;
      this.Y = tile.Y;
      this.Zoom = tile.Zoom;
    }

    public Tile(int x, int y, int zoom)
    {
      this.X = x;
      this.Y = y;
      this.Zoom = zoom;
      this._id = Tile.CalculateTileId(zoom, x, y);
    }

    public override int GetHashCode()
    {
      int num1 = this.X;
      int hashCode1 = num1.GetHashCode();
      num1 = this.Y;
      int hashCode2 = num1.GetHashCode();
      int num2 = hashCode1 ^ hashCode2;
      num1 = this.Zoom;
      int hashCode3 = num1.GetHashCode();
      return num2 ^ hashCode3;
    }

    public override bool Equals(object obj)
    {
      Tile tile = obj as Tile;
      if (tile != null && tile.X == this.X && tile.Y == this.Y)
        return tile.Zoom == this.Zoom;
      return false;
    }

    public override string ToString()
    {
      return string.Format("{0}x-{1}y@{2}z", (object) this.X, (object) this.Y, (object) this.Zoom);
    }

    public TileRange GetSubTiles(int zoom)
    {
      if (this.Zoom > zoom)
        throw new ArgumentOutOfRangeException("zoom", "Subtiles can only be calculated for higher zooms.");
      if (this.Zoom == zoom)
        return new TileRange(this.X, this.Y, this.X, this.Y, this.Zoom);
      int num = 1 << zoom - this.Zoom;
      return new TileRange(this.X * num, this.Y * num, this.X * num + num - 1, this.Y * num + num - 1, zoom);
    }

    public bool Overlaps(Tile tile)
    {
      if (tile == null)
        throw new ArgumentNullException("tile");
      if (tile.Zoom == this.Zoom)
        return tile.Equals((object) this);
      if (tile.Zoom > this.Zoom)
        return this.GetSubTiles(tile.Zoom).Contains(tile);
      return false;
    }

    public bool IsOverlappedBy(IEnumerable<Tile> tiles)
    {
      Dictionary<int, HashSet<Tile>> dictionary = new Dictionary<int, HashSet<Tile>>();
      foreach (Tile tile in tiles)
      {
        if (tile.Zoom <= this.Zoom)
        {
          if (tile.Overlaps(this))
            return true;
        }
        else
        {
          HashSet<Tile> tileSet;
          if (!dictionary.TryGetValue(tile.Zoom, out tileSet))
          {
            tileSet = new HashSet<Tile>();
            dictionary.Add(tile.Zoom, tileSet);
          }
          tileSet.Add(tile);
        }
      }
      foreach (KeyValuePair<int, HashSet<Tile>> keyValuePair in dictionary)
      {
        TileRange subTiles = this.GetSubTiles(keyValuePair.Key);
        int num = 0;
        foreach (Tile tile in keyValuePair.Value)
        {
          if (subTiles.Contains(tile))
            ++num;
        }
        if (subTiles.Count == keyValuePair.Value.Count)
          return true;
      }
      return false;
    }

    private static ulong CalculateTileId(int zoom)
    {
      if (zoom == 0)
        return 0;
      if (zoom == 1)
        return 1;
      if (zoom == 2)
        return 5;
      if (zoom == 3)
        return 21;
      if (zoom == 4)
        return 85;
      if (zoom == 5)
        return 341;
      if (zoom == 6)
        return 1365;
      if (zoom == 7)
        return 5461;
      if (zoom == 8)
        return 21845;
      if (zoom == 9)
        return 87381;
      if (zoom == 10)
        return 349525;
      if (zoom == 11)
        return 1398101;
      if (zoom == 12)
        return 5592405;
      if (zoom == 13)
        return 22369621;
      if (zoom == 14)
        return 89478485;
      if (zoom == 15)
        return 357913941;
      if (zoom == 16)
        return 1431655765;
      if (zoom == 17)
        return 5726623061;
      if (zoom == 18)
        return 22906492245;
      ulong num = (ulong) System.Math.Pow(2.0, (double) (2 * (zoom - 1)));
      return Tile.CalculateTileId(zoom - 1) + num;
    }

    private static ulong CalculateTileId(int zoom, int x, int y)
    {
      long tileId = (long) Tile.CalculateTileId(zoom);
      long num1 = (long) System.Math.Pow(2.0, (double) zoom);
      long num2 = (long) x;
      return (ulong) (tileId + num2 + (long) y * num1);
    }

    private static Tile CalculateTile(ulong id)
    {
      int zoom = 0;
      if (id > 0UL)
      {
        while (id >= Tile.CalculateTileId(zoom))
          ++zoom;
        --zoom;
      }
      long num1 = (long) id - (long) Tile.CalculateTileId(zoom);
      ulong num2 = (ulong) System.Math.Pow(2.0, (double) zoom);
      long num3 = (long) num2;
      int x = (int) ((ulong) num1 % (ulong) num3);
      long num4 = (long) num2;
      int y = (int) ((ulong) num1 / (ulong) num4);
      return new Tile(x, y, zoom);
    }

    public static Tile CreateAroundLocation(double latitude, double longitude, int zoom)
    {
      int num = (int) System.Math.Floor(System.Math.Pow(2.0, (double) zoom));
      Radian radian = (Radian) new Degree(latitude);
      return new Tile((int) ((longitude + 180.0) / 360.0 * (double) num), (int) ((1.0 - System.Math.Log(System.Math.Tan(radian.Value) + 1.0 / System.Math.Cos(radian.Value)) / System.Math.PI) / 2.0 * (double) num), zoom);
    }

    public static Tile CreateAroundLocation(GeoCoordinate location, int zoom)
    {
      return Tile.CreateAroundLocation(location.Latitude, location.Longitude, zoom);
    }

    public Tile InvertX()
    {
      return new Tile((int) System.Math.Floor(System.Math.Pow(2.0, (double) this.Zoom)) - this.X - 1, this.Y, this.Zoom);
    }

    public Tile InvertY()
    {
      return new Tile(this.X, (int) System.Math.Floor(System.Math.Pow(2.0, (double) this.Zoom)) - this.Y - 1, this.Zoom);
    }

    public BoxF2D ToBox(IProjection projection)
    {
      double x1 = projection.LongitudeToX(this.TopLeft[0]);
      double x2 = projection.LongitudeToX(this.BottomRight[0]);
      double y1 = projection.LatitudeToY(this.BottomRight[1]);
      double y2 = projection.LatitudeToY(this.TopLeft[1]);
      double y1_1 = y1;
      double x2_1 = x2;
      double y2_1 = y2;
      return new BoxF2D(x1, y1_1, x2_1, y2_1);
    }
  }
}
