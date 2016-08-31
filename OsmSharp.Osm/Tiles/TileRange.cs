using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Osm.Tiles
{
  public class TileRange : IEnumerable<Tile>, IEnumerable
  {
    public int XMin { get; private set; }

    public int YMin { get; private set; }

    public int XMax { get; private set; }

    public int YMax { get; private set; }

    public int Zoom { get; private set; }

    public int Count
    {
      get
      {
        return System.Math.Abs(this.XMax - this.XMin + 1) * System.Math.Abs(this.YMax - this.YMin + 1);
      }
    }

    public TileRange(int xMin, int yMin, int xMax, int yMax, int zoom)
    {
      this.XMin = xMin;
      this.XMax = xMax;
      this.YMin = yMin;
      this.YMax = yMax;
      this.Zoom = zoom;
    }

    public bool Contains(Tile tile)
    {
      if (this.XMax >= tile.X && this.XMin <= tile.X && this.YMax >= tile.Y)
        return this.YMin <= tile.Y;
      return false;
    }

    public bool IsBorderAt(int x, int y, int zoom)
    {
      if (x == this.XMin || x == this.XMax || (y == this.YMin || y == this.YMin))
        return this.Zoom == zoom;
      return false;
    }

    public bool IsBorderAt(Tile tile)
    {
      return this.IsBorderAt(tile.X, tile.Y, tile.Zoom);
    }

    public static TileRange CreateAroundBoundingBox(GeoCoordinateBox box, int zoom)
    {
      int num1 = (int) System.Math.Floor(System.Math.Pow(2.0, (double) zoom));
      Radian radian1 = (Radian) new Degree(box.MaxLat);
      int xMin = (int) ((box.MinLon + 180.0) / 360.0 * (double) num1);
      int num2 = (int) ((1.0 - System.Math.Log(System.Math.Tan(radian1.Value) + 1.0 / System.Math.Cos(radian1.Value)) / System.Math.PI) / 2.0 * (double) num1);
      Radian radian2 = (Radian) new Degree(box.MinLat);
      int num3 = (int) ((box.MaxLon + 180.0) / 360.0 * (double) num1);
      int num4 = (int) ((1.0 - System.Math.Log(System.Math.Tan(radian2.Value) + 1.0 / System.Math.Cos(radian2.Value)) / System.Math.PI) / 2.0 * (double) num1);
      int yMin = num2;
      int xMax = num3;
      int yMax = num4;
      int zoom1 = zoom;
      return new TileRange(xMin, yMin, xMax, yMax, zoom1);
    }

    public IEnumerator<Tile> GetEnumerator()
    {
      return (IEnumerator<Tile>) new TileRange.TileRangeEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public IEnumerable<Tile> EnumerateInCenterFirst()
    {
      return (IEnumerable<Tile>) new TileRange.TileRangeCenterFirst(this);
    }

    private class TileRangeEnumerator : IEnumerator<Tile>, IEnumerator, IDisposable
    {
      private TileRange _range;
      private Tile _current;

      public Tile Current
      {
        get
        {
          return this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public TileRangeEnumerator(TileRange range)
      {
        this._range = range;
      }

      public void Dispose()
      {
        this._range = (TileRange) null;
      }

      public bool MoveNext()
      {
        if (this._current == null)
        {
          this._current = new Tile(this._range.XMin, this._range.YMin, this._range.Zoom);
          return true;
        }
        int x1 = this._current.X;
        int y = this._current.Y;
        int x2;
        if (x1 == this._range.XMax)
        {
          if (y == this._range.YMax)
            return false;
          ++y;
          x2 = this._range.XMin;
        }
        else
          x2 = x1 + 1;
        this._current = new Tile(x2, y, this._current.Zoom);
        return true;
      }

      public void Reset()
      {
        this._current = (Tile) null;
      }
    }

    public class TileRangeCenteredEnumerator : IEnumerator<Tile>, IEnumerator, IDisposable
    {
      private HashSet<Tile> _enumeratedTiles = new HashSet<Tile>();
      private TileRange _range;
      private Tile _current;
      private TileRange.TileRangeCenteredEnumerator.DirectionEnum _direction;

      public Tile Current
      {
        get
        {
          return this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public TileRangeCenteredEnumerator(TileRange range)
      {
        this._range = range;
      }

      public void Dispose()
      {
        this._range = (TileRange) null;
      }

      public bool MoveNext()
      {
        if (this._current == null)
        {
          this._current = new Tile((int) System.Math.Floor((double) (this._range.XMax + this._range.XMin) / 2.0), (int) System.Math.Ceiling((double) (this._range.YMax + this._range.YMin) / 2.0), this._range.Zoom);
          this._enumeratedTiles.Add(this._current);
          return true;
        }
        if (this._range.Count <= this._enumeratedTiles.Count)
          return false;
        Tile tile = (Tile) null;
        while (tile == null)
        {
          switch (this._direction)
          {
            case TileRange.TileRangeCenteredEnumerator.DirectionEnum.Up:
              tile = new Tile(this._current.X, this._current.Y - 1, this._range.Zoom);
              if (this._enumeratedTiles.Contains(tile))
              {
                this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Left;
                tile = (Tile) null;
                break;
              }
              this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Right;
              break;
            case TileRange.TileRangeCenteredEnumerator.DirectionEnum.Right:
              tile = new Tile(this._current.X + 1, this._current.Y, this._range.Zoom);
              if (this._enumeratedTiles.Contains(tile))
              {
                this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Up;
                tile = (Tile) null;
                break;
              }
              this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Down;
              break;
            case TileRange.TileRangeCenteredEnumerator.DirectionEnum.Down:
              tile = new Tile(this._current.X, this._current.Y + 1, this._range.Zoom);
              if (this._enumeratedTiles.Contains(tile))
              {
                this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Right;
                tile = (Tile) null;
                break;
              }
              this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Left;
              break;
            case TileRange.TileRangeCenteredEnumerator.DirectionEnum.Left:
              tile = new Tile(this._current.X - 1, this._current.Y, this._range.Zoom);
              if (this._enumeratedTiles.Contains(tile))
              {
                this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Down;
                tile = (Tile) null;
                break;
              }
              this._direction = TileRange.TileRangeCenteredEnumerator.DirectionEnum.Up;
              break;
          }
          if (tile != null && !this._range.Contains(tile))
          {
            this._current = tile;
            tile = (Tile) null;
          }
        }
        this._current = tile;
        this._enumeratedTiles.Add(this._current);
        return true;
      }

      public void Reset()
      {
        this._current = (Tile) null;
        this._enumeratedTiles.Clear();
      }

      private enum DirectionEnum
      {
        Up,
        Right,
        Down,
        Left,
      }
    }

    private class TileRangeCenterFirst : IEnumerable<Tile>, IEnumerable
    {
      private TileRange _tileRange;

      public TileRangeCenterFirst(TileRange tileRange)
      {
        this._tileRange = tileRange;
      }

      public IEnumerator<Tile> GetEnumerator()
      {
        return (IEnumerator<Tile>) new TileRange.TileRangeCenteredEnumerator(this._tileRange);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new TileRange.TileRangeCenteredEnumerator(this._tileRange);
      }
    }
  }
}
