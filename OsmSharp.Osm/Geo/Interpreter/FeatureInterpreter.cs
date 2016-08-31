using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Features;
using OsmSharp.Osm.Data;
using System;

namespace OsmSharp.Osm.Geo.Interpreter
{
  public abstract class FeatureInterpreter
  {
    private static FeatureInterpreter _defaultInterpreter;

    public static FeatureInterpreter DefaultInterpreter
    {
      get
      {
        if (FeatureInterpreter._defaultInterpreter == null)
          FeatureInterpreter._defaultInterpreter = (FeatureInterpreter) new SimpleFeatureInterpreter();
        return FeatureInterpreter._defaultInterpreter;
      }
      set
      {
        FeatureInterpreter._defaultInterpreter = value;
      }
    }

    public abstract FeatureCollection Interpret(ICompleteOsmGeo osmObject);

    public abstract bool IsPotentiallyArea(TagsCollectionBase tags);

    public virtual FeatureCollection Interpret(OsmGeo simpleOsmGeo, IDataSourceReadOnly data)
    {
      switch (simpleOsmGeo.Type)
      {
        case OsmGeoType.Node:
          return this.Interpret((ICompleteOsmGeo) (simpleOsmGeo as Node));
        case OsmGeoType.Way:
          return this.Interpret((ICompleteOsmGeo) CompleteWay.CreateFrom(simpleOsmGeo as Way, (INodeSource) data));
        case OsmGeoType.Relation:
          return this.Interpret((ICompleteOsmGeo) CompleteRelation.CreateFrom(simpleOsmGeo as Relation, (IOsmGeoSource) data));
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
