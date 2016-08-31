using OsmSharp.IO.Xml;
using OsmSharp.IO.Xml.Gpx;
using OsmSharp.IO.Xml.Gpx.v1_1;
using OsmSharp.IO.Xml.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OsmSharp.Routing.IO.Gpx
{
  internal static class RouteGpx
  {
    internal static void Save(Stream stream, Route route)
    {
      GpxDocument gpxDocument = new GpxDocument((IXmlSource) new XmlStreamSource(stream));
      gpxType gpxType = new gpxType();
      gpxType.trk = new trkType[1];
      List<wptType> wptTypeList1 = new List<wptType>();
      trkType trkType = new trkType();
      List<wptType> wptTypeList2 = new List<wptType>();
      trkType.trkseg = new trksegType[1];
      trksegType trksegType = new trksegType();
      for (int index1 = 0; index1 < route.Segments.Count; ++index1)
      {
        RouteSegment segment = route.Segments[index1];
        if (segment.Points != null)
        {
          for (int index2 = 0; index2 < segment.Points.Length; ++index2)
          {
            RouteStop point = segment.Points[index2];
            RouteTags routeTags = point.Tags == null ? (RouteTags) null : ((IEnumerable<RouteTags>) point.Tags).FirstOrDefault<RouteTags>((Func<RouteTags, bool>) (x => x.Value == "name"));
            wptTypeList2.Add(new wptType()
            {
              lat = (Decimal) point.Latitude,
              lon = (Decimal) point.Longitude,
              name = routeTags == null ? string.Empty : routeTags.Value
            });
          }
        }
        wptTypeList1.Add(new wptType()
        {
          lat = (Decimal) segment.Latitude,
          lon = (Decimal) segment.Longitude
        });
      }
      trksegType.trkpt = wptTypeList1.ToArray();
      trkType.trkseg[0] = trksegType;
      gpxType.trk[0] = trkType;
      gpxType.wpt = wptTypeList2.ToArray();
      gpxDocument.Gpx = (object) gpxType;
      gpxDocument.Save();
    }
  }
}
