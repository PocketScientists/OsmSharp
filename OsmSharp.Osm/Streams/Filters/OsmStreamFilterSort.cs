using System;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterSort : OsmStreamFilter
  {
    private bool _firstWay = true;
    private bool _firstRelation = true;
    private OsmGeoType _currentType;
    private bool? _isSourceSorted;

    public override bool IsSorted
    {
      get
      {
        return true;
      }
    }

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public override void Initialize()
    {
      if (this.Source == null)
        throw new Exception("No target registered!");
      this.Source.Initialize();
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      if (ignoreNodes & ignoreWays && !ignoreRelations)
        return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
      if (((!ignoreNodes ? 0 : (!ignoreWays ? 1 : 0)) & (ignoreRelations ? 1 : 0)) != 0)
        return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
      if (!ignoreNodes & ignoreWays & ignoreRelations)
        return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
      while (this.DoMoveNext())
      {
        if (this.Current().Type == OsmGeoType.Node && !ignoreNodes || this.Current().Type == OsmGeoType.Way && !ignoreWays || this.Current().Type == OsmGeoType.Relation && !ignoreRelations)
          return true;
      }
      return false;
    }

    private bool DoMoveNext()
    {
      if (this.Source.IsSorted || this._isSourceSorted.HasValue && this._isSourceSorted.Value)
        return this.Source.MoveNext();
      if (this.Source.MoveNext())
      {
        bool flag1 = false;
        bool flag2 = this.Current().Type != this._currentType;
        while (this.Current().Type != this._currentType)
        {
          if (!this.Source.MoveNext())
          {
            flag1 = true;
            break;
          }
        }
        if (flag2 && !flag1)
        {
          if (this._currentType == OsmGeoType.Node)
            this._isSourceSorted = new bool?(false);
          else if (this._currentType == OsmGeoType.Way)
          {
            if (!this._firstWay)
              this._isSourceSorted = new bool?(false);
            this._firstWay = false;
          }
          else if (this._currentType == OsmGeoType.Relation)
          {
            if (!this._firstRelation)
              this._isSourceSorted = new bool?(false);
            this._firstRelation = false;
          }
        }
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
          if (!this._isSourceSorted.HasValue)
            this._isSourceSorted = new bool?(true);
          return false;
        default:
          throw new InvalidOperationException("Unkown SimpleOsmGeoType");
      }
    }

    public override OsmGeo Current()
    {
      return this.Source.Current();
    }

    public override void Reset()
    {
      this._currentType = OsmGeoType.Node;
      this.Source.Reset();
    }
  }
}
