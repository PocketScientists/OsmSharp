using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Geo.Streams.GeoJson
{
  public static class GeoJsonConverter
  {
    public static string ToGeoJson(this FeatureCollection featureCollection)
    {
      if (featureCollection == null)
        throw new ArgumentNullException("featureCollection");
      JTokenWriter jtokenWriter = new JTokenWriter();
      FeatureCollection featureCollection1 = featureCollection;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, (IEnumerable<Feature>) featureCollection1);
      return jtokenWriter.Token.ToString();
    }

    public static string ToGeoJson(this IEnumerable<Feature> featureCollection)
    {
      if (featureCollection == null)
        throw new ArgumentNullException("featureCollection");
      JTokenWriter jtokenWriter = new JTokenWriter();
      IEnumerable<Feature> featureCollection1 = featureCollection;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, featureCollection1);
      return jtokenWriter.Token.ToString();
    }

    public static FeatureCollection ToFeatureCollection(this string geoJson)
    {
      return GeoJsonConverter.ReadFeatureCollection((JsonReader) new JsonTextReader((TextReader) new StringReader(geoJson)));
    }

    public static FeatureCollection ReadFeatureCollection(JsonReader jsonReader)
    {
      string str = string.Empty;
      List<Feature> featureList = (List<Feature>) null;
      while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
      {
        if (jsonReader.TokenType == JsonToken.PropertyName)
        {
          if ((string) jsonReader.Value == "type")
            str = jsonReader.ReadAsString();
          else if ((string) jsonReader.Value == "features")
            featureList = GeoJsonConverter.ReadFeatureArray(jsonReader);
        }
      }
      if (!(str == "FeatureCollection"))
        throw new Exception("Invalid type.");
      if (featureList == null)
        return new FeatureCollection();
      return new FeatureCollection((IEnumerable<Feature>) featureList);
    }

    internal static List<Feature> ReadFeatureArray(JsonReader jsonReader)
    {
      List<Feature> featureList = new List<Feature>();
      jsonReader.Read();
      while (jsonReader.TokenType != JsonToken.EndArray)
      {
        Feature feature = GeoJsonConverter.ReadFeature(jsonReader);
        if (feature == null)
          return featureList;
        featureList.Add(feature);
        jsonReader.Read();
      }
      return featureList;
    }

    public static void Write(JsonWriter writer, IEnumerable<Feature> featureCollection)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (featureCollection == null)
        throw new ArgumentNullException("featureCollection");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("FeatureCollection");
      writer.WritePropertyName("features");
      writer.WriteStartArray();
      foreach (Feature feature in featureCollection)
        GeoJsonConverter.Write(writer, feature);
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static string ToGeoJson(this Feature feature)
    {
      if (feature == null)
        throw new ArgumentNullException("feature");
      JTokenWriter jtokenWriter = new JTokenWriter();
      Feature feature1 = feature;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, feature1);
      return jtokenWriter.Token.ToString();
    }

    public static Feature ToFeature(this string geoJson)
    {
      return GeoJsonConverter.ReadFeature((JsonReader) new JsonTextReader((TextReader) new StringReader(geoJson)));
    }

    public static Feature ReadFeature(JsonReader jsonReader)
    {
      string str = string.Empty;
      Geometry geometry = (Geometry) null;
      GeometryAttributeCollection attributes = (GeometryAttributeCollection) null;
      while (jsonReader.Read())
      {
        if (jsonReader.TokenType == JsonToken.EndArray)
          return (Feature) null;
        if (jsonReader.TokenType != JsonToken.EndObject)
        {
          if (jsonReader.TokenType == JsonToken.PropertyName)
          {
            if ((string) jsonReader.Value == "type")
              str = jsonReader.ReadAsString();
            else if ((string) jsonReader.Value == "geometry")
              geometry = GeoJsonConverter.ReadGeometry(jsonReader);
            else if ((string) jsonReader.Value == "properties")
              attributes = GeoJsonConverter.ReadAttributes(jsonReader);
          }
        }
        else
          break;
      }
      if (!(str == "Feature"))
        throw new Exception("Invalid type.");
      if (geometry == null)
        throw new Exception("No geometry found.");
      if (attributes != null)
        return new Feature(geometry, attributes);
      return new Feature(geometry);
    }

    internal static GeometryAttributeCollection ReadAttributes(JsonReader jsonReader)
    {
      SimpleGeometryAttributeCollection attributeCollection = new SimpleGeometryAttributeCollection();
      while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
      {
        if (jsonReader.TokenType == JsonToken.PropertyName)
        {
          string str = (string) jsonReader.Value;
          jsonReader.Read();
          object obj = jsonReader.Value;
          attributeCollection.Add(new GeometryAttribute()
          {
            Key = str,
            Value = obj
          });
        }
      }
      return (GeometryAttributeCollection) attributeCollection;
    }

    public static void Write(JsonWriter writer, Feature feature)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (feature == null)
        throw new ArgumentNullException("feature");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("Feature");
      writer.WritePropertyName("properties");
      GeoJsonConverter.Write(writer, feature.Attributes);
      writer.WritePropertyName("geometry");
      GeoJsonConverter.Write(writer, feature.Geometry);
      writer.WriteEndObject();
    }

    internal static void Write(JsonWriter writer, GeometryAttributeCollection attributes)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (attributes == null)
        throw new ArgumentNullException("attributes");
      writer.WriteStartObject();
      HashSet<string> stringSet = new HashSet<string>();
      foreach (GeometryAttribute attribute in attributes)
      {
        if (!stringSet.Contains(attribute.Key))
        {
          writer.WritePropertyName(attribute.Key);
          writer.WriteValue(attribute.Value);
          stringSet.Add(attribute.Key);
        }
      }
      writer.WriteEndObject();
    }

    internal static void Write(JsonWriter writer, Geometry geometry)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      if (geometry is LineairRing)
        GeoJsonConverter.Write(writer, geometry as LineairRing);
      else if (geometry is Point)
        GeoJsonConverter.Write(writer, geometry as Point);
      else if (geometry is LineString)
        GeoJsonConverter.Write(writer, geometry as LineString);
      else if (geometry is Polygon)
        GeoJsonConverter.Write(writer, geometry as Polygon);
      else if (geometry is MultiPoint)
        GeoJsonConverter.Write(writer, geometry as MultiPoint);
      else if (geometry is MultiPolygon)
        GeoJsonConverter.Write(writer, geometry as MultiPolygon);
      else if (geometry is MultiLineString)
        GeoJsonConverter.Write(writer, geometry as MultiLineString);
      else if (geometry is GeometryCollection)
        GeoJsonConverter.Write(writer, geometry as GeometryCollection);
      else
        throw new Exception(string.Format("Unknown geometry of type: {0}", (object) geometry.GetType()));
    }

    public static string ToGeoJson(this MultiPoint geometryCollection)
    {
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      JTokenWriter jtokenWriter = new JTokenWriter();
      MultiPoint geometryCollection1 = geometryCollection;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometryCollection1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, MultiPoint geometryCollection)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("MultiPoint");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      foreach (Point geometry in (GeometryCollectionBase<Point>) geometryCollection)
      {
        writer.WriteStartArray();
        writer.WriteValue(geometry.Coordinate.Longitude);
        writer.WriteValue(geometry.Coordinate.Latitude);
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static string ToGeoJson(this MultiLineString geometryCollection)
    {
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      JTokenWriter jtokenWriter = new JTokenWriter();
      MultiLineString geometryCollection1 = geometryCollection;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometryCollection1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, MultiLineString geometryCollection)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("MultiLineString");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      foreach (LineString geometry in (GeometryCollectionBase<LineString>) geometryCollection)
      {
        writer.WriteStartArray();
        foreach (GeoCoordinate coordinate in geometry.Coordinates)
        {
          writer.WriteStartArray();
          writer.WriteValue(coordinate.Longitude);
          writer.WriteValue(coordinate.Latitude);
          writer.WriteEndArray();
        }
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static string ToGeoJson(this LineairRing geometry)
    {
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      JTokenWriter jtokenWriter = new JTokenWriter();
      LineairRing geometry1 = geometry;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometry1);
      return jtokenWriter.Token.ToString();
    }

    public static string ToGeoJson(this MultiPolygon geometryCollection)
    {
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      JTokenWriter jtokenWriter = new JTokenWriter();
      MultiPolygon geometryCollection1 = geometryCollection;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometryCollection1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, MultiPolygon geometryCollection)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("MultiPolygon");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      foreach (Polygon geometry in (GeometryCollectionBase<Polygon>) geometryCollection)
      {
        writer.WriteStartArray();
        writer.WriteStartArray();
        foreach (GeoCoordinate coordinate in geometry.Ring.Coordinates)
        {
          writer.WriteStartArray();
          writer.WriteValue(coordinate.Longitude);
          writer.WriteValue(coordinate.Latitude);
          writer.WriteEndArray();
        }
        writer.WriteEndArray();
        foreach (LineairRing hole in geometry.Holes)
        {
          writer.WriteStartArray();
          foreach (GeoCoordinate coordinate in hole.Coordinates)
          {
            writer.WriteStartArray();
            writer.WriteValue(coordinate.Longitude);
            writer.WriteValue(coordinate.Latitude);
            writer.WriteEndArray();
          }
          writer.WriteEndArray();
        }
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    internal static void Write(JsonWriter writer, LineairRing geometry)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("Polygon");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      writer.WriteStartArray();
      foreach (GeoCoordinate coordinate in geometry.Coordinates)
      {
        writer.WriteStartArray();
        writer.WriteValue(coordinate.Longitude);
        writer.WriteValue(coordinate.Latitude);
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static string ToGeoJson(this Polygon geometry)
    {
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      JTokenWriter jtokenWriter = new JTokenWriter();
      Polygon geometry1 = geometry;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometry1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, Polygon geometry)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("Polygon");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      writer.WriteStartArray();
      foreach (GeoCoordinate coordinate in geometry.Ring.Coordinates)
      {
        writer.WriteStartArray();
        writer.WriteValue(coordinate.Longitude);
        writer.WriteValue(coordinate.Latitude);
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      foreach (LineairRing hole in geometry.Holes)
      {
        writer.WriteStartArray();
        foreach (GeoCoordinate coordinate in hole.Coordinates)
        {
          writer.WriteStartArray();
          writer.WriteValue(coordinate.Longitude);
          writer.WriteValue(coordinate.Latitude);
          writer.WriteEndArray();
        }
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static string ToGeoJson(this LineString geometry)
    {
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      JTokenWriter jtokenWriter = new JTokenWriter();
      LineString geometry1 = geometry;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometry1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, LineString geometry)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("LineString");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      foreach (GeoCoordinate coordinate in geometry.Coordinates)
      {
        writer.WriteStartArray();
        writer.WriteValue(coordinate.Longitude);
        writer.WriteValue(coordinate.Latitude);
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static string ToGeoJson(this Point geometry)
    {
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      JTokenWriter jtokenWriter = new JTokenWriter();
      Point geometry1 = geometry;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometry1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, Point geometry)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometry == null)
        throw new ArgumentNullException("geometry");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("Point");
      writer.WritePropertyName("coordinates");
      writer.WriteStartArray();
      writer.WriteValue(geometry.Coordinate.Longitude);
      writer.WriteValue(geometry.Coordinate.Latitude);
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    public static Geometry ToGeometry(this string geoJson)
    {
      return GeoJsonConverter.ReadGeometry((JsonReader) new JsonTextReader((TextReader) new StringReader(geoJson)));
    }

    internal static Geometry ReadGeometry(JsonReader jsonReader)
    {
      string s = string.Empty;
      List<object> coordinates = new List<object>();
      List<Geometry> geometries = (List<Geometry>) null;
      while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
      {
        if (jsonReader.TokenType == JsonToken.PropertyName)
        {
          if ((string) jsonReader.Value == "type")
            s = jsonReader.ReadAsString();
          else if ((string) jsonReader.Value == "geometries")
            geometries = GeoJsonConverter.ReadGeometryArray(jsonReader);
          else if ((string) jsonReader.Value == "coordinates")
          {
            jsonReader.Read();
            coordinates = GeoJsonConverter.ReadCoordinateArrays(jsonReader);
          }
        }
      }
            // ISSUE: reference to a compiler-generated method
            long stringHash = s.GetHashCode();// \u003CPrivateImplementationDetails\u003E.ComputeStringHash(s);
      if (stringHash <= 2386032169U)
      {
        if ((int) stringHash != 1547714260)
        {
          if ((int) stringHash != 2050635977)
          {
            if ((int) stringHash == -1908935127 && s == "Polygon")
              return (Geometry) GeoJsonConverter.BuildPolygon(coordinates);
          }
          else if (s == "MultiLineString")
            return (Geometry) GeoJsonConverter.BuildMultiLineString(coordinates);
        }
        else if (s == "MultiPolygon")
          return (Geometry) GeoJsonConverter.BuildMultiPolygon(coordinates);
      }
      else if (stringHash <= 3786658501U)
      {
        if ((int) stringHash != -600749884)
        {
          if ((int) stringHash == -508308795 && s == "GeometryCollection")
            return (Geometry) GeoJsonConverter.BuildGeometryCollection(geometries);
        }
        else if (s == "MultiPoint")
          return (Geometry) GeoJsonConverter.BuildMultiPoint(coordinates);
      }
      else if ((int) stringHash != -358027471)
      {
        if ((int) stringHash == -200799438 && s == "LineString")
          return (Geometry) GeoJsonConverter.BuildLineString(coordinates);
      }
      else if (s == "Point")
        return (Geometry) GeoJsonConverter.BuildPoint(coordinates);
      throw new Exception(string.Format("Unknown geometry type: {0}", (object) s));
    }

    internal static List<Geometry> ReadGeometryArray(JsonReader jsonReader)
    {
      List<Geometry> geometryList = new List<Geometry>();
      jsonReader.Read();
      while (jsonReader.TokenType != JsonToken.EndArray)
      {
        geometryList.Add(GeoJsonConverter.ReadGeometry(jsonReader));
        jsonReader.Read();
      }
      return geometryList;
    }

    internal static List<object> ReadCoordinateArrays(JsonReader jsonReader)
    {
      List<object> objectList = new List<object>();
      jsonReader.Read();
      if (jsonReader.TokenType == JsonToken.StartArray)
      {
        while (jsonReader.TokenType == JsonToken.StartArray)
        {
          objectList.Add((object) GeoJsonConverter.ReadCoordinateArrays(jsonReader));
          jsonReader.Read();
        }
      }
      else if (jsonReader.TokenType == JsonToken.Float)
      {
        while (jsonReader.TokenType != JsonToken.EndArray)
        {
          objectList.Add((object) (double) jsonReader.Value);
          jsonReader.Read();
        }
      }
      else
        throw new Exception(string.Format("Invalid token in coordinates array: {0}", (object) jsonReader.TokenType.ToInvariantString()));
      return objectList;
    }

    internal static Point BuildPoint(List<object> coordinates)
    {
      if (coordinates == null)
        throw new ArgumentNullException();
      if (coordinates != null && coordinates.Count == 2 && (coordinates[0] is double && coordinates[1] is double))
        return new Point(new GeoCoordinate((double) coordinates[1], (double) coordinates[0]));
      throw new Exception("Invalid coordinate collection.");
    }

    internal static LineString BuildLineString(List<object> coordinates)
    {
      if (coordinates == null)
        throw new ArgumentNullException();
      if (coordinates.Count <= 1)
        throw new Exception("Invalid coordinate collection.");
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      for (int index = 0; index < coordinates.Count; ++index)
      {
        List<object> coordinate = coordinates[index] as List<object>;
        if (coordinate != null && coordinate.Count == 2 && (coordinate[0] is double && coordinate[1] is double))
          geoCoordinateList.Add(new GeoCoordinate((double) coordinate[1], (double) coordinate[0]));
      }
      return new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList);
    }

    internal static Polygon BuildPolygon(List<object> coordinates)
    {
      if (coordinates == null)
        throw new ArgumentNullException();
      if (coordinates.Count < 1)
        throw new Exception("Invalid coordinate collection.");
      List<List<GeoCoordinate>> geoCoordinateListList = new List<List<GeoCoordinate>>();
      foreach (List<object> coordinate in coordinates)
      {
        List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
        for (int index = 0; index < coordinate.Count; ++index)
        {
          List<object> objectList = coordinate[index] as List<object>;
          if (objectList != null && objectList.Count == 2 && (objectList[0] is double && objectList[1] is double))
            geoCoordinateList.Add(new GeoCoordinate((double) objectList[1], (double) objectList[0]));
        }
        geoCoordinateListList.Add(geoCoordinateList);
      }
      LineairRing outline = new LineairRing((IEnumerable<GeoCoordinate>) geoCoordinateListList[0]);
      List<LineairRing> lineairRingList = new List<LineairRing>();
      for (int index = 1; index < geoCoordinateListList.Count; ++index)
        lineairRingList.Add(new LineairRing((IEnumerable<GeoCoordinate>) geoCoordinateListList[index]));
      return new Polygon(outline, (IEnumerable<LineairRing>) lineairRingList);
    }

    internal static MultiPoint BuildMultiPoint(List<object> coordinates)
    {
      if (coordinates == null)
        throw new ArgumentNullException();
      if (coordinates.Count <= 1)
        throw new Exception("Invalid coordinate collection.");
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      for (int index = 0; index < coordinates.Count; ++index)
      {
        List<object> coordinate = coordinates[index] as List<object>;
        if (coordinate != null && coordinate.Count == 2 && (coordinate[0] is double && coordinate[1] is double))
          geoCoordinateList.Add(new GeoCoordinate((double) coordinate[1], (double) coordinate[0]));
      }
      List<Point> pointList = new List<Point>();
      foreach (GeoCoordinate coordinate in geoCoordinateList)
        pointList.Add(new Point(coordinate));
      return new MultiPoint((IEnumerable<Point>) pointList);
    }

    internal static MultiLineString BuildMultiLineString(List<object> coordinates)
    {
      if (coordinates == null)
        throw new ArgumentNullException();
      if (coordinates.Count < 1)
        throw new Exception("Invalid coordinate collection.");
      List<List<GeoCoordinate>> geoCoordinateListList = new List<List<GeoCoordinate>>();
      foreach (List<object> coordinate in coordinates)
      {
        List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
        for (int index = 0; index < coordinate.Count; ++index)
        {
          List<object> objectList = coordinate[index] as List<object>;
          if (objectList != null && objectList.Count == 2 && (objectList[0] is double && objectList[1] is double))
            geoCoordinateList.Add(new GeoCoordinate((double) objectList[1], (double) objectList[0]));
        }
        geoCoordinateListList.Add(geoCoordinateList);
      }
      List<LineString> lineStringList = new List<LineString>();
      for (int index = 0; index < geoCoordinateListList.Count; ++index)
        lineStringList.Add(new LineString((IEnumerable<GeoCoordinate>) geoCoordinateListList[index]));
      return new MultiLineString((IEnumerable<LineString>) lineStringList);
    }

    internal static MultiPolygon BuildMultiPolygon(List<object> coordinates)
    {
      if (coordinates == null)
        throw new ArgumentNullException();
      if (coordinates.Count < 1)
        throw new Exception("Invalid coordinate collection.");
      List<Polygon> polygonList = new List<Polygon>();
      foreach (List<object> coordinate in coordinates)
        polygonList.Add(GeoJsonConverter.BuildPolygon(coordinate));
      return new MultiPolygon((IEnumerable<Polygon>) polygonList);
    }

    internal static GeometryCollection BuildGeometryCollection(List<Geometry> geometries)
    {
      return new GeometryCollection((IEnumerable<Geometry>) geometries);
    }

    public static string ToGeoJson(this GeometryCollection geometryCollection)
    {
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      JTokenWriter jtokenWriter = new JTokenWriter();
      GeometryCollection geometryCollection1 = geometryCollection;
      GeoJsonConverter.Write((JsonWriter) jtokenWriter, geometryCollection1);
      return jtokenWriter.Token.ToString();
    }

    internal static void Write(JsonWriter writer, GeometryCollection geometryCollection)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (geometryCollection == null)
        throw new ArgumentNullException("geometryCollection");
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue("GeometryCollection");
      writer.WritePropertyName("geometries");
      writer.WriteStartArray();
      foreach (Geometry geometry in (GeometryCollectionBase<Geometry>) geometryCollection)
        GeoJsonConverter.Write(writer, geometry);
      writer.WriteEndArray();
      writer.WriteEndObject();
    }
  }
}
