using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterBoundingBox : OsmStreamFilter
  {
    private readonly OsmSharp.Collections.LongIndex.LongIndex.LongIndex _nodesIn = new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
    private readonly HashSet<long> _waysIn = new HashSet<long>();
    private readonly HashSet<long> _relationIn = new HashSet<long>();
    private readonly OsmSharp.Collections.LongIndex.LongIndex.LongIndex _nodesToInclude = new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
    private readonly HashSet<long> _relationsToInclude = new HashSet<long>();
    private readonly HashSet<long> _waysToInclude = new HashSet<long>();
    private readonly HashSet<long> _relationsConsidered = new HashSet<long>();
    private OsmGeoType _currentType;
    private bool _includeExtraMode;
    private readonly GeoCoordinateBox _box;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public OsmStreamFilterBoundingBox(GeoCoordinateBox box)
    {
      if (box == null)
        throw new ArgumentNullException("box");
      this._box = box;
      this.Meta.Add("bbox", this._box.ToInvariantString());
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
      if (!this._includeExtraMode)
      {
        if (this.Source.MoveNext())
        {
          bool flag1 = false;
          bool flag2 = false;
          while (this.Current().Type != this._currentType)
          {
            if (!this.Source.MoveNext())
            {
              flag1 = true;
              break;
            }
          }
          if (!flag1)
          {
            while (this.Current().Type == this._currentType && !flag2)
            {
              OsmGeo osmGeo = this.Source.Current();
              flag2 = this.IsInBB(osmGeo);
              if (flag2)
              {
                switch (osmGeo.Type)
                {
                  case OsmGeoType.Node:
                    this._nodesIn.Add(osmGeo.Id.Value);
                    goto label_19;
                  case OsmGeoType.Way:
                    this._waysIn.Add(osmGeo.Id.Value);
                    goto label_19;
                  case OsmGeoType.Relation:
                    this._relationIn.Add(osmGeo.Id.Value);
                    goto label_19;
                  default:
                    goto label_19;
                }
              }
              else
              {
                if (!this.Source.MoveNext())
                {
                  flag1 = true;
                  break;
                }
                while (this.Current().Type != this._currentType)
                {
                  if (!this.Source.MoveNext())
                  {
                    flag1 = true;
                    break;
                  }
                }
                if (flag1)
                  break;
              }
            }
          }
label_19:
          if (!flag1 && this.Current().Type == this._currentType)
            return true;
        }
        switch (this._currentType)
        {
          case OsmGeoType.Node:
            this.Source.Reset();
            this._currentType = OsmGeoType.Way;
            return this.MoveNext();
          case OsmGeoType.Way:
            this.Source.Reset();
            this._currentType = OsmGeoType.Relation;
            return this.MoveNext();
          case OsmGeoType.Relation:
            this.Source.Reset();
            this._includeExtraMode = true;
            return this.MoveNext();
          default:
            throw new InvalidOperationException("Unkown SimpleOsmGeoType");
        }
      }
      else
      {
        while (this.Source.MoveNext())
        {
          switch (this.Source.Current().Type)
          {
            case OsmGeoType.Node:
              if (this._nodesToInclude.Contains(this.Source.Current().Id.Value) && !this._nodesIn.Contains(this.Source.Current().Id.Value))
                return true;
              continue;
            case OsmGeoType.Way:
              if (this._waysToInclude.Contains(this.Source.Current().Id.Value) && !this._waysIn.Contains(this.Source.Current().Id.Value))
                return true;
              continue;
            case OsmGeoType.Relation:
              if (this._relationsToInclude.Contains(this.Source.Current().Id.Value) && !this._relationIn.Contains(this.Source.Current().Id.Value))
                return true;
              continue;
            default:
              continue;
          }
        }
        return false;
      }
    }

    public override OsmGeo Current()
    {
      return this.Source.Current();
    }

    public override void Reset()
    {
      this._waysIn.Clear();
      this._nodesIn.Clear();
      this._currentType = OsmGeoType.Node;
      this._includeExtraMode = false;
      this.Source.Reset();
    }

    private bool IsInBB(OsmGeo osmGeo)
    {
      bool flag = false;
      switch (osmGeo.Type)
      {
        case OsmGeoType.Node:
          GeoCoordinateBox box = this._box;
          double? nullable = (osmGeo as Node).Latitude;
          double latitude = nullable.Value;
          nullable = (osmGeo as Node).Longitude;
          double longitude = nullable.Value;
          GeoCoordinate geoCoordinate = new GeoCoordinate(latitude, longitude);
          flag = box.Contains((PointF2D) geoCoordinate);
          break;
        case OsmGeoType.Way:
          foreach (long node in (osmGeo as Way).Nodes)
          {
            if (this._nodesIn.Contains(node))
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            using (List<long>.Enumerator enumerator = (osmGeo as Way).Nodes.GetEnumerator())
            {
              while (enumerator.MoveNext())
                this._nodesToInclude.Add(enumerator.Current);
              break;
            }
          }
          else
            break;
        case OsmGeoType.Relation:
          if (!this._relationsConsidered.Contains(osmGeo.Id.Value))
          {
            foreach (RelationMember member in (osmGeo as Relation).Members)
            {
              switch (member.MemberType.Value)
              {
                case OsmGeoType.Node:
                  if (this._nodesIn.Contains(member.MemberId.Value))
                  {
                    flag = true;
                    continue;
                  }
                  continue;
                case OsmGeoType.Way:
                  if (this._waysIn.Contains(member.MemberId.Value))
                  {
                    flag = true;
                    continue;
                  }
                  continue;
                case OsmGeoType.Relation:
                  if (this._relationIn.Contains(member.MemberId.Value))
                  {
                    flag = true;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            }
            if (flag)
            {
              using (List<RelationMember>.Enumerator enumerator = (osmGeo as Relation).Members.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  RelationMember current = enumerator.Current;
                  switch (current.MemberType.Value)
                  {
                    case OsmGeoType.Node:
                      this._nodesToInclude.Add(current.MemberId.Value);
                      continue;
                    case OsmGeoType.Way:
                      this._waysToInclude.Add(current.MemberId.Value);
                      continue;
                    case OsmGeoType.Relation:
                      this._relationsToInclude.Add(current.MemberId.Value);
                      continue;
                    default:
                      continue;
                  }
                }
                break;
              }
            }
            else
              break;
          }
          else
            break;
      }
      return flag;
    }
  }
}
