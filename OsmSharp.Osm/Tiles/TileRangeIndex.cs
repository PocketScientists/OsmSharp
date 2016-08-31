using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Tiles
{
  public class TileRangeIndex
  {
    private Dictionary<ulong, HashSet<Tile>> _tilesIndex = new Dictionary<ulong, HashSet<Tile>>();
    private TileRange _range;

    public TileRangeIndex(TileRange tileRange)
    {
      this._range = tileRange;
    }

    public void Add(Tile tile)
    {
      if (tile.Zoom < this._range.Zoom && this._range.Zoom - tile.Zoom < 3)
      {
        foreach (Tile subTile in tile.GetSubTiles(this._range.Zoom))
        {
          if (this._range.Contains(subTile))
            this.Add(subTile.Id, tile);
        }
      }
      else if (tile.Zoom > this._range.Zoom && tile.Zoom - this._range.Zoom < 3)
      {
        foreach (Tile tile1 in this._range)
        {
          if (tile1.Overlaps(tile))
          {
            this.Add(tile1.Id, tile);
            return;
          }
        }
      }
      if (!this._range.Contains(tile))
        return;
      this.Add(tile.Id, tile);
    }

    public IEnumerable<Tile> ChooseBest(Tile tile, bool higherFirst)
    {
      List<Tile> tileList = new List<Tile>(this.Get(tile.Id));
      Comparison<Tile> comparison = (Comparison<Tile>) ((x, y) => TileRangeIndex.TileWeight(tile.Zoom, x.Zoom, higherFirst).CompareTo(TileRangeIndex.TileWeight(tile.Zoom, y.Zoom, higherFirst)));
      tileList.Sort(comparison);
      return (IEnumerable<Tile>) tileList;
    }

    public static int TileWeight(int zoom, int x, bool highestFirst)
    {
      if (highestFirst)
      {
        if (zoom >= x)
          return zoom - x;
        return 100 - (x - zoom);
      }
      if (zoom <= x)
        return x - zoom;
      return 100 + (zoom - x);
    }

    private void Add(ulong tileId, Tile tile)
    {
      HashSet<Tile> tileSet;
      if (!this._tilesIndex.TryGetValue(tileId, out tileSet))
      {
        tileSet = new HashSet<Tile>();
        this._tilesIndex.Add(tileId, tileSet);
      }
      tileSet.Add(tile);
    }

    private IEnumerable<Tile> Get(ulong tileId)
    {
      HashSet<Tile> tileSet;
      if (!this._tilesIndex.TryGetValue(tileId, out tileSet))
        tileSet = new HashSet<Tile>();
      return (IEnumerable<Tile>) tileSet;
    }
  }
}
