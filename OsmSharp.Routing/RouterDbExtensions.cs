using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Algorithms.Contracted;
using OsmSharp.Routing.Algorithms.Contracted.Witness;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Network.Data;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public static class RouterDbExtensions
  {
    public static FeatureCollection GetFeatures(this RouterDb db)
    {
      RoutingNetwork network = db.Network;
      FeatureCollection featureCollection = new FeatureCollection();
      HashSet<long> longSet = new HashSet<long>();
      RoutingNetwork.EdgeEnumerator edgeEnumerator = network.GetEdgeEnumerator();
      for (uint vertex1 = 0; vertex1 < network.VertexCount; ++vertex1)
      {
        GeoCoordinateSimple vertex2 = network.GeometricGraph.GetVertex(vertex1);
        featureCollection.Add(new Feature((Geometry) new Point(new GeoCoordinate((double) vertex2.Latitude, (double) vertex2.Longitude)), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[1]
        {
          Tag.Create("id", vertex1.ToInvariantString())
        })));
        edgeEnumerator.MoveTo(vertex1);
        edgeEnumerator.Reset();
        while (edgeEnumerator.MoveNext())
        {
          if (!longSet.Contains((long) edgeEnumerator.Id))
          {
            longSet.Add((long) edgeEnumerator.Id);
            List<ICoordinate> shape = network.GetShape(edgeEnumerator.Current);
            List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
            foreach (ICoordinate coordinate in shape)
              geoCoordinateList.Add(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude));
            LineString lineString = new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList);
            TagsCollectionBase profileAndMeta = db.GetProfileAndMeta((uint) edgeEnumerator.Data.Profile, edgeEnumerator.Data.MetaId);
            profileAndMeta.AddOrReplace(Tag.Create("id", edgeEnumerator.Id.ToInvariantString()));
            featureCollection.Add(new Feature((Geometry) lineString, (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) profileAndMeta)));
          }
        }
      }
      return featureCollection;
    }

    public static FeatureCollection GetFeaturesIn(this RouterDb db, float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
    {
      RoutingNetwork network = db.Network;
      FeatureCollection featureCollection = new FeatureCollection();
      HashSet<uint> uintSet1 = network.GeometricGraph.Search(minLatitude, minLongitude, maxLatitude, maxLongitude);
      HashSet<long> longSet = new HashSet<long>();
      RoutingNetwork.EdgeEnumerator edgeEnumerator = network.GetEdgeEnumerator();
      HashSet<uint> uintSet2 = new HashSet<uint>();
      foreach (uint vertex1 in uintSet1)
      {
        GeoCoordinateSimple vertex2 = network.GeometricGraph.GetVertex(vertex1);
        featureCollection.Add(new Feature((Geometry) new Point(new GeoCoordinate((double) vertex2.Latitude, (double) vertex2.Longitude)), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[1]
        {
          Tag.Create("id", vertex1.ToInvariantString())
        })));
        edgeEnumerator.MoveTo(vertex1);
        edgeEnumerator.Reset();
        while (edgeEnumerator.MoveNext())
        {
          if (!longSet.Contains((long) edgeEnumerator.Id))
          {
            longSet.Add((long) edgeEnumerator.Id);
            List<ICoordinate> shape = network.GetShape(edgeEnumerator.Current);
            List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
            foreach (ICoordinate coordinate in shape)
              geoCoordinateList.Add(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude));
            LineString lineString = new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList);
            RouterDb db1 = db;
            EdgeData data = edgeEnumerator.Data;
            int profile = (int) data.Profile;
            data = edgeEnumerator.Data;
            int metaId = (int) data.MetaId;
            TagsCollectionBase profileAndMeta = db1.GetProfileAndMeta((uint) profile, (uint) metaId);
            profileAndMeta.AddOrReplace(Tag.Create("id", edgeEnumerator.Id.ToInvariantString()));
            featureCollection.Add(new Feature((Geometry) lineString, (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) profileAndMeta)));
            if (!uintSet1.Contains(edgeEnumerator.To))
              uintSet2.Add(edgeEnumerator.To);
          }
        }
      }
      foreach (uint vertex1 in uintSet2)
      {
        GeoCoordinateSimple vertex2 = network.GeometricGraph.GetVertex(vertex1);
        featureCollection.Add(new Feature((Geometry) new Point(new GeoCoordinate((double) vertex2.Latitude, (double) vertex2.Longitude)), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[1]
        {
          Tag.Create("id", vertex1.ToInvariantString())
        })));
      }
      return featureCollection;
    }

    public static void AddContracted(this RouterDb db, Profile profile)
    {
      DirectedMetaGraph directedMetaGraph = (DirectedMetaGraph) null;
      lock (db)
      {
        directedMetaGraph = new DirectedMetaGraph(ContractedEdgeDataSerializer.Size, ContractedEdgeDataSerializer.MetaSize);
        new DirectedGraphBuilder(db.Network.GeometricGraph.Graph, directedMetaGraph, (Func<ushort, Factor>) (p => profile.Factor(db.EdgeProfiles.Get((uint) p)))).Run();
      }
      new HierarchyBuilder(directedMetaGraph, (IPriorityCalculator) new EdgeDifferencePriorityCalculator(directedMetaGraph, (IWitnessCalculator) new DykstraWitnessCalculator(int.MaxValue))
      {
        DifferenceFactor = 5,
        DepthFactor = 5,
        ContractedFactor = 8
      }, (IWitnessCalculator) new DykstraWitnessCalculator(int.MaxValue)).Run();
      lock (db)
        db.AddContracted(profile, directedMetaGraph);
    }

    public static bool SupportsAll(this RouterDb db, params Profile[] profiles)
    {
      for (int index = 0; index < profiles.Length; ++index)
      {
        if (!db.Supports(profiles[index]))
          return false;
      }
      return true;
    }

    public static TagsCollectionBase GetProfileAndMeta(this RouterDb db, uint profileId, uint meta)
    {
      TagsCollection tagsCollection1 = new TagsCollection();
      TagsCollectionBase tagsCollection2 = db.EdgeMeta.Get(meta);
      if (tagsCollection2 != null)
        tagsCollection1.AddOrReplace(tagsCollection2);
      TagsCollectionBase tagsCollection3 = db.EdgeProfiles.Get(profileId);
      if (tagsCollection3 != null)
        tagsCollection1.AddOrReplace(tagsCollection3);
      return (TagsCollectionBase) tagsCollection1;
    }
  }
}
