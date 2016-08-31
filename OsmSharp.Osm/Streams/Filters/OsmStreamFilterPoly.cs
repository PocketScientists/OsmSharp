using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.GeoJson;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterPoly : OsmStreamFilter
  {
    private readonly OsmSharp.Collections.LongIndex.LongIndex.LongIndex _nodesIn = new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
    private readonly OsmSharp.Collections.LongIndex.LongIndex.LongIndex _waysIn = new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
    private readonly LineairRing _poly;
    private readonly GeoCoordinateBox _box;
    private OsmGeoType _currentType;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public OsmStreamFilterPoly(LineairRing poly)
    {
      if (poly == null)
        throw new ArgumentNullException("poly");
      this._poly = poly;
      this._box = new GeoCoordinateBox((IList<GeoCoordinate>) poly.Coordinates);
      this.Meta.Add("poly", GeoJsonConverter.ToGeoJson(this._poly));
    }

    public override void Initialize()
    {
      this.Source.Initialize();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      while (this.DoMoveNext())
      {
        if (this.Current().Type == OsmGeoType.Node && !ignoreNodes || this.Current().Type == OsmGeoType.Way && !ignoreWays || this.Current().Type == OsmGeoType.Relation && !ignoreRelations)
          return true;
      }
      return false;
    }

    private bool DoMoveNext()
    {
      while (this.Source.MoveNext())
      {
        OsmGeo osmGeo = this.Source.Current();
        switch (osmGeo.Type)
        {
          case OsmGeoType.Node:
            if (this._currentType != OsmGeoType.Node)
              throw new OsmStreamNotSortedException("OsmStreamFilterPoly - Source stream is not sorted.");
            Node node = osmGeo as Node;
            double? nullable = node.Latitude;
            double latitude = nullable.Value;
            nullable = node.Longitude;
            double longitude = nullable.Value;
            if (this.IsInsidePoly(latitude, longitude))
            {
              this._nodesIn.Add(node.Id.Value);
              return true;
            }
            continue;
          case OsmGeoType.Way:
            if (this._currentType == OsmGeoType.Relation)
              throw new OsmStreamNotSortedException("OsmStreamFilterPoly - Source stream is not sorted.");
            if (this._currentType == OsmGeoType.Node)
              this._currentType = OsmGeoType.Way;
            Way way = osmGeo as Way;
            if (way.Nodes != null)
            {
              for (int index = 0; index < way.Nodes.Count; ++index)
              {
                if (this._nodesIn.Contains(way.Nodes[index]))
                {
                  this._waysIn.Add(way.Id.Value);
                  return true;
                }
              }
              continue;
            }
            continue;
          case OsmGeoType.Relation:
            if (this._currentType == OsmGeoType.Way || this._currentType == OsmGeoType.Node)
              this._currentType = OsmGeoType.Relation;
            Relation relation = osmGeo as Relation;
            if (relation.Members != null)
            {
              for (int index = 0; index < relation.Members.Count; ++index)
              {
                RelationMember member = relation.Members[index];
                switch (member.MemberType.Value)
                {
                  case OsmGeoType.Node:
                    if (this._nodesIn.Contains(member.MemberId.Value))
                      return true;
                    break;
                  case OsmGeoType.Way:
                    if (this._waysIn.Contains(member.MemberId.Value))
                      return true;
                    break;
                }
              }
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      return false;
    }

    private bool IsInsidePoly(double latitude, double longitude)
    {
      if (!this._box.Contains(longitude, latitude))
        return false;
      return this._poly.Contains(new GeoCoordinate(latitude, longitude));
    }

    public override OsmGeo Current()
    {
      return this.Source.Current();
    }

    public override void Reset()
    {
      this._nodesIn.Clear();
      this._currentType = OsmGeoType.Node;
      this.Source.Reset();
    }
  }
}
