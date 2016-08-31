using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.GeoJson;
using OsmSharp.Math.Geo;
using System.Collections.Generic;

namespace OsmSharp.Routing.Navigation
{
  public static class InstructionExtensions
  {
    public static string ToGeoJson(this IList<Instruction> instructions, Route route)
    {
      return GeoJsonConverter.ToGeoJson(instructions.ToFeatureCollection(route));
    }

    public static FeatureCollection ToFeatureCollection(this IList<Instruction> instructions, Route route)
    {
      FeatureCollection featureCollection = new FeatureCollection();
      for (int index = 0; index < instructions.Count; ++index)
      {
        Instruction instruction = instructions[index];
        RouteSegment segment = route.Segments[instruction.Segment];
        featureCollection.Add(new Feature((Geometry) new Point(new GeoCoordinate((double) segment.Latitude, (double) segment.Longitude)), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[2]
        {
          Tag.Create("text", instruction.Text),
          Tag.Create("type", instruction.Type.ToInvariantString())
        })));
      }
      return featureCollection;
    }
  }
}
