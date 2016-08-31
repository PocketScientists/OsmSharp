using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.IO.Xml;
using OsmSharp.IO.Xml.Kml;
using OsmSharp.IO.Xml.Kml.v2_0_response;
using OsmSharp.IO.Xml.Kml.v2_1;
using OsmSharp.IO.Xml.Sources;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OsmSharp.Geo.Streams.Kml
{
  public class KmlFeatureStreamSource : FeatureCollectionStreamSource
  {
    private readonly Stream _stream;
    private bool _read;

    public KmlFeatureStreamSource(Stream stream)
      : base(new FeatureCollection())
    {
      this._stream = stream;
    }

    public override void Initialize()
    {
      if (!this._read)
        this.DoReadKml();
      base.Initialize();
    }

    private void DoReadKml()
    {
      if (this._stream.CanSeek)
        this._stream.Seek(0L, SeekOrigin.Begin);
      KmlDocument kmlDocument = new KmlDocument((IXmlSource) new XmlStreamSource(this._stream));
      object kml = kmlDocument.Kml;
      switch (kmlDocument.Version)
      {
        case KmlVersion.Kmlv2_1:
          this.ConvertKml(kml as KmlType);
          break;
        case KmlVersion.Kmlv2_0:
          this.ConvertKml(kml as OsmSharp.IO.Xml.Kml.v2_0.kml);
          break;
        case KmlVersion.Kmlv2_0_response:
          this.ConvertKml(kml as OsmSharp.IO.Xml.Kml.v2_0_response.kml);
          break;
      }
    }

    private void ConvertKml(KmlType kmlType)
    {
      this.FeatureCollection.Clear();
      this.ConvertFeature(kmlType.Item);
    }

    private void ConvertKml(OsmSharp.IO.Xml.Kml.v2_0_response.kml kml)
    {
      this.FeatureCollection.Clear();
      if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Document)
        this.ConvertDocument(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
      else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Folder)
        this.ConvertFolder(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
      else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Placemark)
      {
        this.ConvertPlacemark(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
      }
      else
      {
        if (!(kml.Item is Response))
          return;
        this.ConvertResponse(kml.Item as Response);
      }
    }

    private void ConvertKml(OsmSharp.IO.Xml.Kml.v2_0.kml kml)
    {
      this.FeatureCollection.Clear();
      if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0.Document)
        this.ConvertDocument(kml.Item as OsmSharp.IO.Xml.Kml.v2_0.Document);
      else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0.Folder)
      {
        this.ConvertFolder(kml.Item as OsmSharp.IO.Xml.Kml.v2_0.Folder);
      }
      else
      {
        if (!(kml.Item is OsmSharp.IO.Xml.Kml.v2_0.Placemark))
          return;
        this.ConvertPlacemark(kml.Item as OsmSharp.IO.Xml.Kml.v2_0.Placemark);
      }
    }

    private void ConvertPlacemark(OsmSharp.IO.Xml.Kml.v2_0.Placemark placemark)
    {
      for (int index = 0; index < placemark.Items.Length; ++index)
      {
        switch (placemark.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.LineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertLineString(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.LineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiGeometry:
            this.ConvertMultiGeometry(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiGeometry);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiLineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiLineString(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiLineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiPoint:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPoint(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiPoint));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiPolygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPolygon(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiPolygon));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.Point:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPoint(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Point));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.Polygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPolygon(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Polygon));
            break;
        }
      }
    }

    private static Feature ConvertPolygon(OsmSharp.IO.Xml.Kml.v2_0.Polygon polygon)
    {
      Feature feature = KmlFeatureStreamSource.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.Polygon(KmlFeatureStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing).Geometry as LineairRing, (IEnumerable<LineairRing>) new LineairRing[1]
      {
        feature.Geometry as LineairRing
      }));
    }

    private static Feature ConvertLinearRing(OsmSharp.IO.Xml.Kml.v2_0.LinearRing linearRing)
    {
      return new Feature((Geometry) new LineairRing((IEnumerable<GeoCoordinate>) KmlFeatureStreamSource.ConvertCoordinates(linearRing.coordinates)))
      {
        Attributes = {
          {
            "id",
            (object) linearRing.id
          }
        }
      };
    }

    private static Feature ConvertPoint(OsmSharp.IO.Xml.Kml.v2_0.Point point)
    {
      Feature feature = new Feature((Geometry) new OsmSharp.Geo.Geometries.Point(KmlFeatureStreamSource.ConvertCoordinates(point.coordinates)[0]));
      if (point.altitudeModeSpecified)
        feature.Attributes.Add("altitude", (object) point.altitudeMode);
      if (point.extrudeSpecified)
        feature.Attributes.Add("extrude", (object) point.extrude);
      if (point.id != null)
        feature.Attributes.Add("id", (object) point.id);
      return feature;
    }

    private static Feature ConvertMultiPolygon(OsmSharp.IO.Xml.Kml.v2_0.MultiPolygon multiPolygon)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.MultiPolygon(new OsmSharp.Geo.Geometries.Polygon[1]
      {
        KmlFeatureStreamSource.ConvertPolygon(multiPolygon.Polygon).Geometry as OsmSharp.Geo.Geometries.Polygon
      }));
    }

    private static Feature ConvertMultiPoint(OsmSharp.IO.Xml.Kml.v2_0.MultiPoint multiPoint)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.MultiPoint(new OsmSharp.Geo.Geometries.Point[1]
      {
        KmlFeatureStreamSource.ConvertPoint(multiPoint.Point).Geometry as OsmSharp.Geo.Geometries.Point
      }));
    }

    private static Feature ConvertMultiLineString(OsmSharp.IO.Xml.Kml.v2_0.MultiLineString multiLineString)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.MultiLineString(new OsmSharp.Geo.Geometries.LineString[1]
      {
        KmlFeatureStreamSource.ConvertLineString(multiLineString.LineString).Geometry as OsmSharp.Geo.Geometries.LineString
      }));
    }

    private void ConvertMultiGeometry(OsmSharp.IO.Xml.Kml.v2_0.MultiGeometry multiGeometry)
    {
      for (int index = 0; index < multiGeometry.Items.Length; ++index)
      {
        switch (multiGeometry.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.LineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertLineString(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.LineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiGeometry:
            this.ConvertMultiGeometry(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiGeometry);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiLineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiLineString(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiLineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiPoint:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPoint(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiPoint));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiPolygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPolygon(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.MultiPolygon));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.Point:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPoint(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Point));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.Polygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPolygon(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Polygon));
            break;
        }
      }
    }

    private static Feature ConvertLineString(OsmSharp.IO.Xml.Kml.v2_0.LineString lineString)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.LineString((IEnumerable<GeoCoordinate>) KmlFeatureStreamSource.ConvertCoordinates(lineString.coordinates)))
      {
        Attributes = {
          {
            "id",
            (object) lineString.id
          }
        }
      };
    }

    private void ConvertFolder(OsmSharp.IO.Xml.Kml.v2_0.Folder folder)
    {
      for (int index = 0; index < folder.Items.Length; ++index)
      {
        switch (folder.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType2.Document:
            this.ConvertDocument(folder.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Document);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType2.Folder:
            this.ConvertFolder(folder.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Folder);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType2.Placemark:
            this.ConvertPlacemark(folder.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Placemark);
            break;
        }
      }
    }

    private void ConvertDocument(OsmSharp.IO.Xml.Kml.v2_0.Document document)
    {
      for (int index = 0; index < document.Items.Length; ++index)
      {
        switch (document.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType3.Document:
            this.ConvertDocument(document.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Document);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType3.Folder:
            this.ConvertFolder(document.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Folder);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType3.Placemark:
            this.ConvertPlacemark(document.Items[index] as OsmSharp.IO.Xml.Kml.v2_0.Placemark);
            break;
        }
      }
    }

    private void ConvertResponse(Response response)
    {
      foreach (object obj in response.Items)
      {
        if (obj is OsmSharp.IO.Xml.Kml.v2_0_response.Document)
          this.ConvertDocument(obj as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
        else if (obj is OsmSharp.IO.Xml.Kml.v2_0_response.Folder)
          this.ConvertFolder(obj as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
        else if (obj is OsmSharp.IO.Xml.Kml.v2_0_response.Placemark)
          this.ConvertPlacemark(obj as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
        else if (obj is Response)
          this.ConvertResponse(obj as Response);
      }
    }

    private void ConvertPlacemark(OsmSharp.IO.Xml.Kml.v2_0_response.Placemark placemark)
    {
      for (int index = 0; index < placemark.Items.Length; ++index)
      {
        switch (placemark.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.LineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertLineString(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.LineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiGeometry:
            this.ConvertMultiGeometry(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiGeometry);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiLineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiLineString(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiLineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPoint:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPoint(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPoint));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPolygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPolygon(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPolygon));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.Point:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPoint(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Point));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.Polygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPolygon(placemark.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Polygon));
            break;
        }
      }
    }

    private static Feature ConvertPolygon(OsmSharp.IO.Xml.Kml.v2_0_response.Polygon polygon)
    {
      Feature feature = KmlFeatureStreamSource.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.Polygon(KmlFeatureStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing).Geometry as LineairRing, (IEnumerable<LineairRing>) new LineairRing[1]
      {
        feature.Geometry as LineairRing
      }));
    }

    private static Feature ConvertLinearRing(OsmSharp.IO.Xml.Kml.v2_0_response.LinearRing linearRing)
    {
      return new Feature((Geometry) new LineairRing((IEnumerable<GeoCoordinate>) KmlFeatureStreamSource.ConvertCoordinates(linearRing.coordinates)))
      {
        Attributes = {
          {
            "id",
            (object) linearRing.id
          }
        }
      };
    }

    private static Feature ConvertPoint(OsmSharp.IO.Xml.Kml.v2_0_response.Point point)
    {
      Feature feature = new Feature((Geometry) new OsmSharp.Geo.Geometries.Point(KmlFeatureStreamSource.ConvertCoordinates(point.coordinates)[0]));
      if (point.altitudeModeSpecified)
        feature.Attributes.Add("altitude", (object) point.altitudeMode);
      if (point.extrudeSpecified)
        feature.Attributes.Add("extrude", (object) point.extrude);
      if (point.id != null)
        feature.Attributes.Add("id", (object) point.id);
      return feature;
    }

    private static Feature ConvertMultiPolygon(OsmSharp.IO.Xml.Kml.v2_0_response.MultiPolygon multiPolygon)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.MultiPolygon(new OsmSharp.Geo.Geometries.Polygon[1]
      {
        KmlFeatureStreamSource.ConvertPolygon(multiPolygon.Polygon).Geometry as OsmSharp.Geo.Geometries.Polygon
      }));
    }

    private static Feature ConvertMultiPoint(OsmSharp.IO.Xml.Kml.v2_0_response.MultiPoint multiPoint)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.MultiPoint(new OsmSharp.Geo.Geometries.Point[1]
      {
        KmlFeatureStreamSource.ConvertPoint(multiPoint.Point).Geometry as OsmSharp.Geo.Geometries.Point
      }));
    }

    private static Feature ConvertMultiLineString(OsmSharp.IO.Xml.Kml.v2_0_response.MultiLineString multiLineString)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.MultiLineString(new OsmSharp.Geo.Geometries.LineString[1]
      {
        KmlFeatureStreamSource.ConvertLineString(multiLineString.LineString).Geometry as OsmSharp.Geo.Geometries.LineString
      }));
    }

    private void ConvertMultiGeometry(OsmSharp.IO.Xml.Kml.v2_0_response.MultiGeometry multiGeometry)
    {
      for (int index = 0; index < multiGeometry.Items.Length; ++index)
      {
        switch (multiGeometry.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.LineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertLineString(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.LineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiGeometry:
            this.ConvertMultiGeometry(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiGeometry);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiLineString:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiLineString(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiLineString));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPoint:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPoint(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPoint));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPolygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertMultiPolygon(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPolygon));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.Point:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPoint(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Point));
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.Polygon:
            this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPolygon(multiGeometry.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Polygon));
            break;
        }
      }
    }

    private static Feature ConvertLineString(OsmSharp.IO.Xml.Kml.v2_0_response.LineString lineString)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.LineString((IEnumerable<GeoCoordinate>) KmlFeatureStreamSource.ConvertCoordinates(lineString.coordinates)))
      {
        Attributes = {
          {
            "id",
            (object) lineString.id
          }
        }
      };
    }

    private void ConvertFolder(OsmSharp.IO.Xml.Kml.v2_0_response.Folder folder)
    {
      for (int index = 0; index < folder.Items.Length; ++index)
      {
        switch (folder.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType2.Document:
            this.ConvertDocument(folder.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType2.Folder:
            this.ConvertFolder(folder.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType2.Placemark:
            this.ConvertPlacemark(folder.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
            break;
        }
      }
    }

    private void ConvertDocument(OsmSharp.IO.Xml.Kml.v2_0_response.Document document)
    {
      for (int index = 0; index < document.Items.Length; ++index)
      {
        switch (document.ItemsElementName[index])
        {
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType3.Document:
            this.ConvertDocument(document.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType3.Folder:
            this.ConvertFolder(document.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
            break;
          case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType3.Placemark:
            this.ConvertPlacemark(document.Items[index] as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
            break;
        }
      }
    }

    private void ConvertFeatures(FeatureType[] featureType)
    {
      foreach (FeatureType feature in featureType)
        this.ConvertFeature(feature);
    }

    private void ConvertFeature(FeatureType feature)
    {
      if (feature is ContainerType)
      {
        this.ConvertContainer(feature as ContainerType);
      }
      else
      {
        if (!(feature is PlacemarkType))
          return;
        this.ConvertPlacemark(feature as PlacemarkType);
      }
    }

    private void ConvertPlacemark(PlacemarkType placemark)
    {
      this.ConvertGeometry(placemark.Item1);
    }

    private void ConvertGeometry(GeometryType geometry)
    {
      if (geometry is PointType)
        this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPoint(geometry as PointType));
      else if (geometry is LineStringType)
        this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertLineString(geometry as LineStringType));
      else if (geometry is LinearRingType)
        this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertLinearRing(geometry as LinearRingType));
      else if (geometry is PolygonType)
      {
        this.FeatureCollection.Add(KmlFeatureStreamSource.ConvertPolygon(geometry as PolygonType));
      }
      else
      {
        if (!(geometry is MultiGeometryType))
          return;
        this.ConvertMultiGeometry(geometry as MultiGeometryType);
      }
    }

    private void ConvertMultiGeometry(MultiGeometryType multiGeometry)
    {
      foreach (GeometryType geometry in multiGeometry.Items)
        this.ConvertGeometry(geometry);
    }

    private static Feature ConvertPolygon(PolygonType polygon)
    {
      IEnumerable<LineairRing> holes = KmlFeatureStreamSource.ConvertBoundary(polygon.innerBoundaryIs);
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.Polygon(KmlFeatureStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing).Geometry as LineairRing, holes));
    }

    private static IEnumerable<LineairRing> ConvertBoundary(boundaryType[] boundary)
    {
      List<LineairRing> lineairRingList = new List<LineairRing>();
      foreach (boundaryType boundaryType in boundary)
        lineairRingList.Add(KmlFeatureStreamSource.ConvertLinearRing(boundaryType.LinearRing).Geometry as LineairRing);
      return (IEnumerable<LineairRing>) lineairRingList;
    }

    private static Feature ConvertLinearRing(LinearRingType linearRing)
    {
      return new Feature((Geometry) new LineairRing((IEnumerable<GeoCoordinate>) KmlFeatureStreamSource.ConvertCoordinates(linearRing.coordinates)))
      {
        Attributes = {
          {
            "id",
            (object) linearRing.id
          }
        }
      };
    }

    private static Feature ConvertLineString(LineStringType lineString)
    {
      return new Feature((Geometry) new OsmSharp.Geo.Geometries.LineString((IEnumerable<GeoCoordinate>) KmlFeatureStreamSource.ConvertCoordinates(lineString.coordinates)))
      {
        Attributes = {
          {
            "id",
            (object) lineString.id
          }
        }
      };
    }

    private static Feature ConvertPoint(PointType point)
    {
      Feature feature = new Feature((Geometry) new OsmSharp.Geo.Geometries.Point(KmlFeatureStreamSource.ConvertCoordinates(point.coordinates)[0]));
      if (point.targetId != null)
        feature.Attributes.Add("targetId", (object) point.targetId);
      feature.Attributes.Add("altitude", (object) point.altitudeMode);
      if (point.extrude)
        feature.Attributes.Add("extrude", (object) point.extrude);
      if (point.id != null)
        feature.Attributes.Add("id", (object) point.id);
      return feature;
    }

    private void ConvertContainer(ContainerType container)
    {
      if (container is FolderType)
      {
        this.ConvertFeatures((container as FolderType).Items1);
      }
      else
      {
        if (!(container is DocumentType))
          return;
        this.ConvertFeatures((container as DocumentType).Items1);
      }
    }

    private static IList<GeoCoordinate> ConvertCoordinates(string coordinates)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      string str1 = coordinates;
      char[] chArray = new char[1]{ '\n' };
      foreach (string str2 in str1.Split(chArray))
      {
        if (str2 != null && str2.Length > 0 && str2.Trim().Length > 0)
        {
          string[] strArray = str2.Split(',');
          double result1 = 0.0;
          int index1 = 0;
          double.TryParse(strArray[index1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1);
          double result2 = 0.0;
          int index2 = 1;
          double.TryParse(strArray[index2], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result2);
          geoCoordinateList.Add(new GeoCoordinate(result2, result1));
        }
      }
      return (IList<GeoCoordinate>) geoCoordinateList;
    }
  }
}
