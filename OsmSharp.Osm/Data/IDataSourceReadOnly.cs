using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data
{
  public interface IDataSourceReadOnly : IOsmGeoSource, INodeSource, IWaySource, IRelationSource
  {
    GeoCoordinateBox BoundingBox { get; }

    Guid Id { get; }

    bool HasBoundinBox { get; }

    bool IsReadOnly { get; }

    IList<Node> GetNodes(IList<long> ids);

    IList<Relation> GetRelations(IList<long> ids);

    IList<Relation> GetRelationsFor(OsmGeoType type, long id);

    IList<Relation> GetRelationsFor(OsmGeo obj);

    IList<Way> GetWays(IList<long> ids);

    IList<Way> GetWaysFor(long id);

    IList<Way> GetWaysFor(Node node);

    IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter);
  }
}
