using OsmSharp.Collections;
using OsmSharp.Geo;
using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Streams
{
  public class NodeCoordinatesDictionary
  {
    private byte[] longBytes = new byte[8];
    private readonly IDictionary<long, long> _data;

    public long Count
    {
      get
      {
        return (long) this._data.Count;
      }
    }

    public NodeCoordinatesDictionary()
    {
      this._data = (IDictionary<long, long>) new HugeDictionary<long, long>();
    }

    public void Add(long id, float latitude, float longitude)
    {
      BitConverter.GetBytes(latitude).CopyTo((Array) this.longBytes, 0);
      BitConverter.GetBytes(longitude).CopyTo((Array) this.longBytes, 4);
      long int64 = BitConverter.ToInt64(this.longBytes, 0);
      this._data[id] = int64;
    }

    public void Add(long id, ICoordinate coordinate)
    {
      this.Add(id, coordinate.Latitude, coordinate.Longitude);
    }

    public bool TryGetValue(long id, out float latitude, out float longitude)
    {
      long num;
      if (this._data.TryGetValue(id, out num))
      {
        byte[] bytes = BitConverter.GetBytes(num);
        latitude = BitConverter.ToSingle(bytes, 0);
        longitude = BitConverter.ToSingle(bytes, 4);
        return true;
      }
      latitude = 0.0f;
      longitude = 0.0f;
      return false;
    }

    public bool TryGetValue(long id, out ICoordinate coordinate)
    {
      float latitude;
      float longitude;
      if (this.TryGetValue(id, out latitude, out longitude))
      {
        coordinate = (ICoordinate) new GeoCoordinateSimple()
        {
          Latitude = latitude,
          Longitude = longitude
        };
        return true;
      }
      coordinate = (ICoordinate) null;
      return false;
    }
  }
}
