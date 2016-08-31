using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing.Osm.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Osm
{
  public static class RouterDbExtensions
  {
    public static void LoadOsmData(this RouterDb db, Stream data, params Vehicle[] vehicles)
    {
      db.LoadOsmData(data, false, vehicles);
    }

    public static void LoadOsmData(this RouterDb db, Stream data, bool allCore, params Vehicle[] vehicles)
    {
      if (!db.IsEmpty)
        throw new ArgumentException("Can only load a new routing network into an empty router db.");
      PBFOsmStreamSource pbfOsmStreamSource = new PBFOsmStreamSource(data);
      db.LoadOsmData((OsmStreamSource) pbfOsmStreamSource, vehicles);
    }

    public static void LoadOsmData(this RouterDb db, OsmStreamSource source, params Vehicle[] vehicles)
    {
      db.LoadOsmData(source, false, vehicles);
    }

    public static void LoadOsmData(this RouterDb db, OsmStreamSource source, bool allCore, params Vehicle[] vehicles)
    {
      if (!db.IsEmpty)
        throw new ArgumentException("Can only load a new routing network into an empty router db.");
      RouterDbStreamTarget routerDbStreamTarget = new RouterDbStreamTarget(db, vehicles, allCore, 1, true, (IEnumerable<ITwoPassProcessor>) null);
      OsmStreamSource osmStreamSource = source;
      ((OsmStreamTarget) routerDbStreamTarget).RegisterSource(osmStreamSource);
      routerDbStreamTarget.Pull();
      db.Network.Sort();
    }
  }
}
