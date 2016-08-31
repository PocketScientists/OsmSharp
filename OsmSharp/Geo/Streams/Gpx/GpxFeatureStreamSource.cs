using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.IO.Xml;
using OsmSharp.IO.Xml.Gpx;
using OsmSharp.IO.Xml.Gpx.v1_0;
using OsmSharp.IO.Xml.Gpx.v1_1;
using OsmSharp.IO.Xml.Sources;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Geo.Streams.Gpx
{
  public class GpxFeatureStreamSource : FeatureCollectionStreamSource
  {
    private readonly bool _createTrackPoints = true;
    private readonly Stream _stream;
    private bool _read;

    public GpxFeatureStreamSource(Stream stream)
      : base(new FeatureCollection())
    {
      this._stream = stream;
    }

    public GpxFeatureStreamSource(Stream stream, bool createTrackPoints)
      : base(new FeatureCollection())
    {
      this._stream = stream;
      this._createTrackPoints = createTrackPoints;
    }

    public override void Initialize()
    {
      if (!this._read)
        this.DoReadGpx();
      base.Initialize();
    }

    private void DoReadGpx()
    {
      if (this._stream.CanSeek)
        this._stream.Seek(0L, SeekOrigin.Begin);
      GpxDocument gpxDocument = new GpxDocument((IXmlSource) new XmlStreamSource(this._stream));
      object gpx = gpxDocument.Gpx;
      switch (gpxDocument.Version)
      {
        case GpxVersion.Gpxv1_0:
          this.ReadGpxv1_0(gpx as gpx);
          break;
        case GpxVersion.Gpxv1_1:
          this.ReadGpxv1_1(gpx as gpxType);
          break;
      }
    }

    private void ReadGpxv1_1(gpxType gpx)
    {
      this.FeatureCollection.Clear();
      if (gpx.wpt != null)
      {
        foreach (wptType wptType in gpx.wpt)
        {
          Feature feature = new Feature((Geometry) new Point(new GeoCoordinate((double) wptType.lat, (double) wptType.lon)));
          if (wptType.ageofdgpsdataSpecified)
            feature.Attributes.Add("ageofdgpsdata", (object) wptType.ageofdgpsdata);
          if (wptType.eleSpecified)
            feature.Attributes.Add("ele", (object) wptType.ele);
          if (wptType.fixSpecified)
            feature.Attributes.Add("fix", (object) wptType.fix);
          if (wptType.geoidheightSpecified)
            feature.Attributes.Add("geoidheight", (object) wptType.geoidheight);
          if (wptType.hdopSpecified)
            feature.Attributes.Add("hdop", (object) wptType.hdop);
          if (wptType.magvarSpecified)
            feature.Attributes.Add("magvar", (object) wptType.magvar);
          if (wptType.pdopSpecified)
            feature.Attributes.Add("pdop", (object) wptType.pdop);
          if (wptType.timeSpecified)
            feature.Attributes.Add("time", (object) wptType.time);
          if (wptType.vdopSpecified)
            feature.Attributes.Add("vdop", (object) wptType.vdop);
          if (wptType.cmt != null)
            feature.Attributes.Add("cmt", (object) wptType.cmt);
          if (wptType.desc != null)
            feature.Attributes.Add("desc", (object) wptType.desc);
          if (wptType.dgpsid != null)
            feature.Attributes.Add("dgpsid", (object) wptType.dgpsid);
          if (wptType.name != null)
            feature.Attributes.Add("name", (object) wptType.name);
          if (wptType.sat != null)
            feature.Attributes.Add("sat", (object) wptType.sat);
          if (wptType.src != null)
            feature.Attributes.Add("src", (object) wptType.src);
          if (wptType.sym != null)
            feature.Attributes.Add("sym", (object) wptType.sym);
          if (wptType.type != null)
            feature.Attributes.Add("type", (object) wptType.type);
          this.FeatureCollection.Add(feature);
        }
      }
      if (gpx.rte != null)
      {
        foreach (rteType rteType in gpx.rte)
        {
          List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
          foreach (wptType wptType in rteType.rtept)
          {
            GeoCoordinate coordinate = new GeoCoordinate((double) wptType.lat, (double) wptType.lon);
            geoCoordinateList.Add(coordinate);
            if (this._createTrackPoints)
            {
              Feature feature = new Feature((Geometry) new Point(coordinate));
              if (wptType.ageofdgpsdataSpecified)
                feature.Attributes.Add("ageofdgpsdata", (object) wptType.ageofdgpsdata);
              if (wptType.eleSpecified)
                feature.Attributes.Add("ele", (object) wptType.ele);
              if (wptType.fixSpecified)
                feature.Attributes.Add("fix", (object) wptType.fix);
              if (wptType.geoidheightSpecified)
                feature.Attributes.Add("geoidheight", (object) wptType.geoidheight);
              if (wptType.hdopSpecified)
                feature.Attributes.Add("hdop", (object) wptType.hdop);
              if (wptType.magvarSpecified)
                feature.Attributes.Add("magvar", (object) wptType.magvar);
              if (wptType.pdopSpecified)
                feature.Attributes.Add("pdop", (object) wptType.pdop);
              if (wptType.timeSpecified)
                feature.Attributes.Add("time", (object) wptType.time);
              if (wptType.vdopSpecified)
                feature.Attributes.Add("vdop", (object) wptType.vdop);
              if (wptType.cmt != null)
                feature.Attributes.Add("cmt", (object) wptType.cmt);
              if (wptType.desc != null)
                feature.Attributes.Add("desc", (object) wptType.desc);
              if (wptType.dgpsid != null)
                feature.Attributes.Add("dgpsid", (object) wptType.dgpsid);
              if (wptType.name != null)
                feature.Attributes.Add("name", (object) wptType.name);
              if (wptType.sat != null)
                feature.Attributes.Add("sat", (object) wptType.sat);
              if (wptType.src != null)
                feature.Attributes.Add("src", (object) wptType.src);
              if (wptType.sym != null)
                feature.Attributes.Add("sym", (object) wptType.sym);
              if (wptType.type != null)
                feature.Attributes.Add("type", (object) wptType.type);
              this.FeatureCollection.Add(feature);
            }
          }
          Feature feature1 = new Feature((Geometry) new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList));
          if (rteType.cmt != null)
            feature1.Attributes.Add("cmt", (object) rteType.cmt);
          if (rteType.desc != null)
            feature1.Attributes.Add("desc", (object) rteType.desc);
          if (rteType.name != null)
            feature1.Attributes.Add("name", (object) rteType.name);
          if (rteType.number != null)
            feature1.Attributes.Add("number", (object) rteType.number);
          if (rteType.src != null)
            feature1.Attributes.Add("src", (object) rteType.src);
          this.FeatureCollection.Add(feature1);
        }
      }
      foreach (trkType trkType in gpx.trk)
      {
        List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
        foreach (trksegType trksegType in trkType.trkseg)
        {
          foreach (wptType wptType in trksegType.trkpt)
          {
            GeoCoordinate coordinate = new GeoCoordinate((double) wptType.lat, (double) wptType.lon);
            if (geoCoordinateList.Count == 0 || (PointF2D) geoCoordinateList[geoCoordinateList.Count - 1] != (PointF2D) coordinate)
              geoCoordinateList.Add(coordinate);
            if (this._createTrackPoints)
            {
              Feature feature = new Feature((Geometry) new Point(coordinate));
              if (wptType.ageofdgpsdataSpecified)
                feature.Attributes.Add("ageofdgpsdata", (object) wptType.ageofdgpsdata);
              if (wptType.eleSpecified)
                feature.Attributes.Add("ele", (object) wptType.ele);
              if (wptType.fixSpecified)
                feature.Attributes.Add("fix", (object) wptType.fix);
              if (wptType.geoidheightSpecified)
                feature.Attributes.Add("geoidheight", (object) wptType.geoidheight);
              if (wptType.hdopSpecified)
                feature.Attributes.Add("hdop", (object) wptType.hdop);
              if (wptType.magvarSpecified)
                feature.Attributes.Add("magvar", (object) wptType.magvar);
              if (wptType.pdopSpecified)
                feature.Attributes.Add("pdop", (object) wptType.pdop);
              if (wptType.timeSpecified)
                feature.Attributes.Add("time", (object) wptType.time);
              if (wptType.vdopSpecified)
                feature.Attributes.Add("vdop", (object) wptType.vdop);
              if (wptType.cmt != null)
                feature.Attributes.Add("cmt", (object) wptType.cmt);
              if (wptType.desc != null)
                feature.Attributes.Add("desc", (object) wptType.desc);
              if (wptType.dgpsid != null)
                feature.Attributes.Add("dgpsid", (object) wptType.dgpsid);
              if (wptType.name != null)
                feature.Attributes.Add("name", (object) wptType.name);
              if (wptType.sat != null)
                feature.Attributes.Add("sat", (object) wptType.sat);
              if (wptType.src != null)
                feature.Attributes.Add("src", (object) wptType.src);
              if (wptType.sym != null)
                feature.Attributes.Add("sym", (object) wptType.sym);
              if (wptType.type != null)
                feature.Attributes.Add("type", (object) wptType.type);
              this.FeatureCollection.Add(feature);
            }
          }
        }
        Feature feature1 = new Feature((Geometry) new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList));
        if (trkType.cmt != null)
          feature1.Attributes.Add("cmt", (object) trkType.cmt);
        if (trkType.desc != null)
          feature1.Attributes.Add("desc", (object) trkType.desc);
        if (trkType.name != null)
          feature1.Attributes.Add("name", (object) trkType.name);
        if (trkType.number != null)
          feature1.Attributes.Add("number", (object) trkType.number);
        if (trkType.src != null)
          feature1.Attributes.Add("src", (object) trkType.src);
        this.FeatureCollection.Add(feature1);
      }
    }

    private void ReadGpxv1_0(gpx gpx)
    {
      this.FeatureCollection.Clear();
      if (gpx.wpt != null)
      {
        foreach (gpxWpt gpxWpt in gpx.wpt)
        {
          Feature feature = new Feature((Geometry) new Point(new GeoCoordinate((double) gpxWpt.lat, (double) gpxWpt.lon)));
          if (gpxWpt.ageofdgpsdataSpecified)
            feature.Attributes.Add("ageofdgpsdata", (object) gpxWpt.ageofdgpsdata);
          if (gpxWpt.eleSpecified)
            feature.Attributes.Add("ele", (object) gpxWpt.ele);
          if (gpxWpt.fixSpecified)
            feature.Attributes.Add("fix", (object) gpxWpt.fix);
          if (gpxWpt.geoidheightSpecified)
            feature.Attributes.Add("geoidheight", (object) gpxWpt.geoidheight);
          if (gpxWpt.hdopSpecified)
            feature.Attributes.Add("hdop", (object) gpxWpt.hdop);
          if (gpxWpt.magvarSpecified)
            feature.Attributes.Add("magvar", (object) gpxWpt.magvar);
          if (gpxWpt.pdopSpecified)
            feature.Attributes.Add("pdop", (object) gpxWpt.pdop);
          if (gpxWpt.timeSpecified)
            feature.Attributes.Add("time", (object) gpxWpt.time);
          if (gpxWpt.vdopSpecified)
            feature.Attributes.Add("vdop", (object) gpxWpt.vdop);
          if (gpxWpt.Any != null)
            feature.Attributes.Add("Any", (object) gpxWpt.Any);
          if (gpxWpt.cmt != null)
            feature.Attributes.Add("cmt", (object) gpxWpt.cmt);
          if (gpxWpt.desc != null)
            feature.Attributes.Add("desc", (object) gpxWpt.desc);
          if (gpxWpt.dgpsid != null)
            feature.Attributes.Add("dgpsid", (object) gpxWpt.dgpsid);
          if (gpxWpt.name != null)
            feature.Attributes.Add("name", (object) gpxWpt.name);
          if (gpxWpt.sat != null)
            feature.Attributes.Add("sat", (object) gpxWpt.sat);
          if (gpxWpt.src != null)
            feature.Attributes.Add("src", (object) gpxWpt.src);
          if (gpxWpt.sym != null)
            feature.Attributes.Add("sym", (object) gpxWpt.sym);
          if (gpxWpt.url != null)
            feature.Attributes.Add("url", (object) gpxWpt.url);
          if (gpxWpt.urlname != null)
            feature.Attributes.Add("urlname", (object) gpxWpt.urlname);
          if (gpxWpt.type != null)
            feature.Attributes.Add("type", (object) gpxWpt.type);
          this.FeatureCollection.Add(feature);
        }
      }
      if (gpx.rte != null)
      {
        foreach (gpxRte gpxRte in gpx.rte)
        {
          List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
          foreach (gpxRteRtept gpxRteRtept in gpxRte.rtept)
          {
            GeoCoordinate coordinate = new GeoCoordinate((double) gpxRteRtept.lat, (double) gpxRteRtept.lon);
            geoCoordinateList.Add(coordinate);
            if (this._createTrackPoints)
            {
              Feature feature = new Feature((Geometry) new Point(coordinate));
              if (gpxRteRtept.ageofdgpsdataSpecified)
                feature.Attributes.Add("ageofdgpsdata", (object) gpxRteRtept.ageofdgpsdata);
              if (gpxRteRtept.eleSpecified)
                feature.Attributes.Add("ele", (object) gpxRteRtept.ele);
              if (gpxRteRtept.fixSpecified)
                feature.Attributes.Add("fix", (object) gpxRteRtept.fix);
              if (gpxRteRtept.geoidheightSpecified)
                feature.Attributes.Add("geoidheight", (object) gpxRteRtept.geoidheight);
              if (gpxRteRtept.hdopSpecified)
                feature.Attributes.Add("hdop", (object) gpxRteRtept.hdop);
              if (gpxRteRtept.magvarSpecified)
                feature.Attributes.Add("magvar", (object) gpxRteRtept.magvar);
              if (gpxRteRtept.pdopSpecified)
                feature.Attributes.Add("pdop", (object) gpxRteRtept.pdop);
              if (gpxRteRtept.timeSpecified)
                feature.Attributes.Add("time", (object) gpxRteRtept.time);
              if (gpxRteRtept.vdopSpecified)
                feature.Attributes.Add("vdop", (object) gpxRteRtept.vdop);
              if (gpxRteRtept.Any != null)
                feature.Attributes.Add("Any", (object) gpxRteRtept.Any);
              if (gpxRteRtept.cmt != null)
                feature.Attributes.Add("cmt", (object) gpxRteRtept.cmt);
              if (gpxRteRtept.desc != null)
                feature.Attributes.Add("desc", (object) gpxRteRtept.desc);
              if (gpxRteRtept.dgpsid != null)
                feature.Attributes.Add("dgpsid", (object) gpxRteRtept.dgpsid);
              if (gpxRteRtept.name != null)
                feature.Attributes.Add("name", (object) gpxRteRtept.name);
              if (gpxRteRtept.sat != null)
                feature.Attributes.Add("sat", (object) gpxRteRtept.sat);
              if (gpxRteRtept.src != null)
                feature.Attributes.Add("src", (object) gpxRteRtept.src);
              if (gpxRteRtept.sym != null)
                feature.Attributes.Add("sym", (object) gpxRteRtept.sym);
              if (gpxRteRtept.url != null)
                feature.Attributes.Add("url", (object) gpxRteRtept.url);
              if (gpxRteRtept.urlname != null)
                feature.Attributes.Add("urlname", (object) gpxRteRtept.urlname);
              if (gpxRteRtept.type != null)
                feature.Attributes.Add("type", (object) gpxRteRtept.type);
              this.FeatureCollection.Add(feature);
            }
          }
          Feature feature1 = new Feature((Geometry) new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList));
          if (gpxRte.Any != null)
            feature1.Attributes.Add("Any", (object) gpxRte.Any);
          if (gpxRte.cmt != null)
            feature1.Attributes.Add("cmt", (object) gpxRte.cmt);
          if (gpxRte.desc != null)
            feature1.Attributes.Add("desc", (object) gpxRte.desc);
          if (gpxRte.name != null)
            feature1.Attributes.Add("name", (object) gpxRte.name);
          if (gpxRte.number != null)
            feature1.Attributes.Add("number", (object) gpxRte.number);
          if (gpxRte.src != null)
            feature1.Attributes.Add("src", (object) gpxRte.src);
          if (gpxRte.url != null)
            feature1.Attributes.Add("url", (object) gpxRte.url);
          if (gpxRte.urlname != null)
            feature1.Attributes.Add("urlname", (object) gpxRte.urlname);
          this.FeatureCollection.Add(feature1);
        }
      }
      foreach (gpxTrk gpxTrk in gpx.trk)
      {
        List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
        foreach (gpxTrkTrksegTrkpt gpxTrkTrksegTrkpt in gpxTrk.trkseg)
        {
          GeoCoordinate coordinate = new GeoCoordinate((double) gpxTrkTrksegTrkpt.lat, (double) gpxTrkTrksegTrkpt.lon);
          geoCoordinateList.Add(coordinate);
          if (this._createTrackPoints)
          {
            Feature feature = new Feature((Geometry) new Point(coordinate));
            if (gpxTrkTrksegTrkpt.ageofdgpsdataSpecified)
              feature.Attributes.Add("ageofdgpsdata", (object) gpxTrkTrksegTrkpt.ageofdgpsdata);
            if (gpxTrkTrksegTrkpt.eleSpecified)
              feature.Attributes.Add("ele", (object) gpxTrkTrksegTrkpt.ele);
            if (gpxTrkTrksegTrkpt.fixSpecified)
              feature.Attributes.Add("fix", (object) gpxTrkTrksegTrkpt.fix);
            if (gpxTrkTrksegTrkpt.geoidheightSpecified)
              feature.Attributes.Add("geoidheight", (object) gpxTrkTrksegTrkpt.geoidheight);
            if (gpxTrkTrksegTrkpt.hdopSpecified)
              feature.Attributes.Add("hdop", (object) gpxTrkTrksegTrkpt.hdop);
            if (gpxTrkTrksegTrkpt.magvarSpecified)
              feature.Attributes.Add("magvar", (object) gpxTrkTrksegTrkpt.magvar);
            if (gpxTrkTrksegTrkpt.pdopSpecified)
              feature.Attributes.Add("pdop", (object) gpxTrkTrksegTrkpt.pdop);
            if (gpxTrkTrksegTrkpt.timeSpecified)
              feature.Attributes.Add("time", (object) gpxTrkTrksegTrkpt.time);
            if (gpxTrkTrksegTrkpt.vdopSpecified)
              feature.Attributes.Add("vdop", (object) gpxTrkTrksegTrkpt.vdop);
            if (gpxTrkTrksegTrkpt.Any != null)
              feature.Attributes.Add("Any", (object) gpxTrkTrksegTrkpt.Any);
            if (gpxTrkTrksegTrkpt.cmt != null)
              feature.Attributes.Add("cmt", (object) gpxTrkTrksegTrkpt.cmt);
            if (gpxTrkTrksegTrkpt.desc != null)
              feature.Attributes.Add("desc", (object) gpxTrkTrksegTrkpt.desc);
            if (gpxTrkTrksegTrkpt.dgpsid != null)
              feature.Attributes.Add("dgpsid", (object) gpxTrkTrksegTrkpt.dgpsid);
            if (gpxTrkTrksegTrkpt.name != null)
              feature.Attributes.Add("name", (object) gpxTrkTrksegTrkpt.name);
            if (gpxTrkTrksegTrkpt.sat != null)
              feature.Attributes.Add("sat", (object) gpxTrkTrksegTrkpt.sat);
            if (gpxTrkTrksegTrkpt.src != null)
              feature.Attributes.Add("src", (object) gpxTrkTrksegTrkpt.src);
            if (gpxTrkTrksegTrkpt.sym != null)
              feature.Attributes.Add("sym", (object) gpxTrkTrksegTrkpt.sym);
            if (gpxTrkTrksegTrkpt.url != null)
              feature.Attributes.Add("url", (object) gpxTrkTrksegTrkpt.url);
            if (gpxTrkTrksegTrkpt.urlname != null)
              feature.Attributes.Add("urlname", (object) gpxTrkTrksegTrkpt.urlname);
            if (gpxTrkTrksegTrkpt.type != null)
              feature.Attributes.Add("type", (object) gpxTrkTrksegTrkpt.type);
            this.FeatureCollection.Add(feature);
          }
        }
        Feature feature1 = new Feature((Geometry) new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList));
        if (gpxTrk.Any != null)
          feature1.Attributes.Add("Any", (object) gpxTrk.Any);
        if (gpxTrk.cmt != null)
          feature1.Attributes.Add("cmt", (object) gpxTrk.cmt);
        if (gpxTrk.desc != null)
          feature1.Attributes.Add("desc", (object) gpxTrk.desc);
        if (gpxTrk.name != null)
          feature1.Attributes.Add("name", (object) gpxTrk.name);
        if (gpxTrk.number != null)
          feature1.Attributes.Add("number", (object) gpxTrk.number);
        if (gpxTrk.src != null)
          feature1.Attributes.Add("src", (object) gpxTrk.src);
        if (gpxTrk.url != null)
          feature1.Attributes.Add("url", (object) gpxTrk.url);
        if (gpxTrk.urlname != null)
          feature1.Attributes.Add("urlname", (object) gpxTrk.urlname);
        this.FeatureCollection.Add(feature1);
      }
    }
  }
}
