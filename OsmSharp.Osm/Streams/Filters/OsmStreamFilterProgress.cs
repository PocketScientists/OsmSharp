using OsmSharp.Logging;
using System;

namespace OsmSharp.Osm.Streams.Filters
{
  public class OsmStreamFilterProgress : OsmStreamFilter
  {
    private OsmGeoType? _lastType = new OsmGeoType?(OsmGeoType.Node);
    private long _nodeInterval = 100000;
    private long _wayInterval = 10000;
    private long _relationInterval = 1000;
    private long _lastTypeStart;
    private int _pass;
    private long _node;
    private long _nodeTicks;
    private long _way;
    private long _wayTicks;
    private long _relation;
    private long _relationTicks;

    public override bool CanReset
    {
      get
      {
        return this.Source.CanReset;
      }
    }

    public OsmStreamFilterProgress()
    {
      this._pass = 1;
    }

    public OsmStreamFilterProgress(long nodesInterval, long waysInterval, long relationInterval)
    {
      this._nodeInterval = nodesInterval;
      this._wayInterval = waysInterval;
      this._relationInterval = relationInterval;
    }

    public override void Initialize()
    {
      if (this.Source == null)
        throw new Exception("No target registered!");
      this.Source.Initialize();
      this._lastTypeStart = 0L;
      this._lastType = new OsmGeoType?();
      this._node = 0L;
      this._nodeTicks = 0L;
      this._way = 0L;
      this._wayTicks = 0L;
      this._relation = 0L;
      this._relationTicks = 0L;
    }

    public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
    {
      return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
    }

    public override OsmGeo Current()
    {
      OsmGeo osmGeo = this.Source.Current();
      long ticks = DateTime.Now.Ticks;
      DateTime now;
      if (!this._lastType.HasValue)
      {
        now = DateTime.Now;
        this._lastTypeStart = now.Ticks;
        this._lastType = new OsmGeoType?(osmGeo.Type);
      }
      OsmGeoType? lastType1 = this._lastType;
      OsmGeoType type = osmGeo.Type;
      if ((lastType1.GetValueOrDefault() == type ? (!lastType1.HasValue ? 1 : 0) : 1) != 0)
      {
        long num = ticks - this._lastTypeStart;
        OsmGeoType? lastType2 = this._lastType;
        if (lastType2.HasValue)
        {
          switch (lastType2.GetValueOrDefault())
          {
            case OsmGeoType.Node:
              this._nodeTicks = this._nodeTicks + num;
              break;
            case OsmGeoType.Way:
              this._wayTicks = this._wayTicks + num;
              break;
            case OsmGeoType.Relation:
              this._relationTicks = this._relationTicks + num;
              break;
          }
        }
        now = DateTime.Now;
        this._lastTypeStart = now.Ticks;
        this._lastType = new OsmGeoType?(osmGeo.Type);
      }
      switch (osmGeo.Type)
      {
        case OsmGeoType.Node:
          this._node = this._node + 1L;
          if (this._node % this._nodeInterval == 0L)
          {
            Log.TraceEvent("StreamProgress", TraceEventType.Information, "Pass {2} - Node[{0}] @ {1}/s", (object) this._node, (object) System.Math.Round((double) this._node / new TimeSpan(this._nodeTicks + (ticks - this._lastTypeStart)).TotalSeconds, 0), (object) this._pass);
            break;
          }
          break;
        case OsmGeoType.Way:
          this._way = this._way + 1L;
          if (this._way % this._wayInterval == 0L)
          {
            Log.TraceEvent("StreamProgress", TraceEventType.Information, "Pass {2} - Way[{0}] @ {1}/s", (object) this._way, (object) System.Math.Round((double) this._way / new TimeSpan(this._wayTicks + (ticks - this._lastTypeStart)).TotalSeconds, 2), (object) this._pass);
            break;
          }
          break;
        case OsmGeoType.Relation:
          this._relation = this._relation + 1L;
          if (this._relation % this._relationInterval == 0L)
          {
            Log.TraceEvent("StreamProgress", TraceEventType.Information, "Pass {2} - Relation[{0}] @ {1}/s", (object) this._relation, (object) System.Math.Round((double) this._relation / new TimeSpan(this._relationTicks + (ticks - this._lastTypeStart)).TotalSeconds, 2), (object) this._pass);
            break;
          }
          break;
      }
      return osmGeo;
    }

    public override void Reset()
    {
      this._lastTypeStart = 0L;
      this._lastType = new OsmGeoType?();
      this._pass = this._pass + 1;
      this._node = 0L;
      this._nodeTicks = 0L;
      this._way = 0L;
      this._wayTicks = 0L;
      this._relation = 0L;
      this._relationTicks = 0L;
      this.Source.Reset();
    }
  }
}
