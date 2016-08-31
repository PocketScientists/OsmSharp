using OsmSharp.Collections.Sorting;
using OsmSharp.Geo;
using OsmSharp.Math.Algorithms;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graphs.Geometric;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using OsmSharp.Routing.Network;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Search
{
  public static class Hilbert
  {
    public static int DefaultHilbertSteps = (int) System.Math.Pow(2.0, 15.0);

    public static void Sort(this GeometricGraph graph)
    {
      graph.Sort(Hilbert.DefaultHilbertSteps);
    }

    public static void Sort(this RoutingNetwork graph)
    {
      graph.Sort(Hilbert.DefaultHilbertSteps);
    }

    public static void Sort(this GeometricGraph graph, int n)
    {
      if (graph.VertexCount <= 0U)
        return;
      QuickSort.Sort((Func<long, long>) (vertex => graph.Distance(n, (uint) vertex)), (Action<long, long>) ((vertex1, vertex2) =>
      {
        if (vertex1 == vertex2)
          return;
        graph.Switch((uint) vertex1, (uint) vertex2);
      }), 0L, (long) (graph.VertexCount - 1U));
    }

    public static void Sort(this RoutingNetwork graph, int n)
    {
      if (graph.VertexCount <= 0U)
        return;
      QuickSort.Sort((Func<long, long>) (vertex => graph.Distance(n, (uint) vertex)), (Action<long, long>) ((vertex1, vertex2) =>
      {
        if (vertex1 == vertex2)
          return;
        graph.Switch((uint) vertex1, (uint) vertex2);
      }), 0L, (long) (graph.VertexCount - 1U));
    }

    public static long Distance(this GeometricGraph graph, int n, uint vertex)
    {
      float latitude;
      float longitude;
      if (!graph.GetVertex(vertex, out latitude, out longitude))
        throw new Exception(string.Format("Cannot calculate hilbert distance, vertex {0} does not exist.", (object) vertex));
      return HilbertCurve.HilbertDistance(latitude, longitude, (long) n);
    }

    public static long Distance(this RoutingNetwork graph, int n, uint vertex)
    {
      float latitude;
      float longitude;
      if (!graph.GetVertex(vertex, out latitude, out longitude))
        throw new Exception(string.Format("Cannot calculate hilbert distance, vertex {0} does not exist.", (object) vertex));
      return HilbertCurve.HilbertDistance(latitude, longitude, (long) n);
    }

    public static HashSet<uint> Search(this GeometricGraph graph, float latitude, float longitude, float offset)
    {
      return graph.Search(Hilbert.DefaultHilbertSteps, latitude - offset, longitude - offset, latitude + offset, longitude + offset);
    }

    public static HashSet<uint> Search(this GeometricGraph graph, float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
    {
      return graph.Search(Hilbert.DefaultHilbertSteps, minLatitude, minLongitude, maxLatitude, maxLongitude);
    }

    public static HashSet<uint> Search(this GeometricGraph graph, int n, float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
    {
      List<long> longList = HilbertCurve.HilbertDistances(System.Math.Max(minLatitude, -90f), System.Math.Max(minLongitude, -180f), System.Math.Min(maxLatitude, 90f), System.Math.Min(maxLongitude, 180f), (long) n);
      longList.Sort();
      HashSet<uint> uintSet = new HashSet<uint>();
      int index = 0;
      uint vertex1 = 0;
      uint vertex2 = graph.VertexCount - 1U;
      for (; index < longList.Count && vertex1 < graph.VertexCount; ++index)
      {
        long num1 = longList[index];
        long maxHilbert;
        for (maxHilbert = num1; index < longList.Count - 1 && longList[index + 1] <= maxHilbert + 1L; ++index)
          maxHilbert = longList[index + 1];
        uint vertex;
        int count;
        float latitude;
        float longitude;
        if (num1 == maxHilbert)
        {
          if (graph.Search(num1, n, vertex1, vertex2, out vertex, out count))
          {
            int num2 = count;
            for (; count > 0; --count)
            {
              if (graph.GetVertex(vertex + (uint) (count - 1), out latitude, out longitude) && (double) minLatitude < (double) latitude && ((double) minLongitude < (double) longitude && (double) maxLatitude > (double) latitude) && (double) maxLongitude > (double) longitude)
                uintSet.Add(vertex + (uint) (count - 1));
            }
            vertex1 = vertex + (uint) num2;
          }
        }
        else if (graph.SearchRange(num1, maxHilbert, n, vertex1, vertex2, out vertex, out count))
        {
          int num2 = count;
          for (; count > 0; --count)
          {
            if (graph.GetVertex(vertex + (uint) (count - 1), out latitude, out longitude) && (double) minLatitude < (double) latitude && ((double) minLongitude < (double) longitude && (double) maxLatitude > (double) latitude) && (double) maxLongitude > (double) longitude)
              uintSet.Add(vertex + (uint) (count - 1));
          }
          vertex1 = vertex + (uint) num2;
        }
      }
      return uintSet;
    }

    public static bool Search(this GeometricGraph graph, long hilbert, int n, uint vertex1, uint vertex2, out uint vertex, out int count)
    {
      long num1 = graph.Distance(n, vertex1);
      long num2 = graph.Distance(n, vertex2);
      while (vertex1 <= vertex2)
      {
        if (num1 > num2)
          throw new Exception("Graph not sorted: Binary search using hilbert distance not possible.");
        if (num1 == hilbert)
        {
          uint vertex3;
          for (vertex3 = vertex1; num1 == hilbert && (int) vertex3 != 0; num1 = graph.Distance(n, vertex3))
            --vertex3;
          uint vertex4 = vertex1;
          for (long index = graph.Distance(n, vertex4); index == hilbert && vertex4 < graph.VertexCount - 1U; index = graph.Distance(n, vertex4))
            ++vertex4;
          vertex = vertex3;
          count = (int) vertex4 - (int) vertex3 + 1;
          return true;
        }
        if (num2 == hilbert)
        {
          uint vertex3;
          for (vertex3 = vertex2; num2 == hilbert && (int) vertex3 != 0; num2 = graph.Distance(n, vertex3))
            --vertex3;
          uint vertex4 = vertex2;
          for (long index = graph.Distance(n, vertex4); index == hilbert && vertex4 < graph.VertexCount - 1U; index = graph.Distance(n, vertex4))
            ++vertex4;
          vertex = vertex3;
          count = (int) vertex4 - (int) vertex3 + 1;
          return true;
        }
        if (num1 == num2 || (int) vertex1 == (int) vertex2 || (int) vertex1 == (int) vertex2 - 1)
        {
          vertex = vertex1;
          count = 0;
          return true;
        }
        uint vertex5 = vertex1 + (vertex2 - vertex1) / 2U;
        long num3 = graph.Distance(n, vertex5);
        if (hilbert <= num3)
        {
          vertex2 = vertex5;
          num2 = num3;
        }
        else
        {
          vertex1 = vertex5;
          num1 = num3;
        }
      }
      vertex = vertex1;
      count = 0;
      return false;
    }

    public static bool SearchRange(this GeometricGraph graph, long minHilbert, long maxHilbert, int n, uint vertex1, uint vertex2, out uint vertex, out int count)
    {
      long num1 = graph.Distance(n, vertex1);
      long num2 = graph.Distance(n, vertex2);
      while (vertex1 <= vertex2)
      {
        uint vertex3 = vertex1 + (vertex2 - vertex1) / 2U;
        long num3 = graph.Distance(n, vertex3);
        if (maxHilbert <= num3)
        {
          vertex2 = vertex3;
          num2 = num3;
        }
        else if (minHilbert >= num3)
        {
          vertex1 = vertex3;
          num1 = num3;
        }
        else
        {
          uint vertex4 = vertex1;
          uint vertex5 = vertex3;
          uint num4 = uint.MaxValue;
          while ((int) num4 == -1)
          {
            if (graph.Distance(n, vertex4) == minHilbert)
              num4 = vertex4;
            else if (graph.Distance(n, vertex5) == minHilbert)
            {
              num4 = vertex5;
            }
            else
            {
              while (vertex4 <= vertex5)
              {
                uint vertex6 = vertex4 + (vertex5 - vertex4) / 2U;
                long num5 = graph.Distance(n, vertex6);
                if (num5 > minHilbert)
                  vertex5 = vertex6;
                else if (num5 < minHilbert)
                {
                  vertex4 = vertex6;
                }
                else
                {
                  num4 = vertex6;
                  break;
                }
                if ((int) vertex4 == (int) vertex5 || (int) vertex4 == (int) vertex5 - 1)
                  break;
              }
              if ((int) num4 == -1)
              {
                ++minHilbert;
                if (minHilbert == maxHilbert)
                {
                  vertex = vertex1;
                  count = 0;
                  return true;
                }
              }
            }
          }
          uint vertex7 = vertex3;
          uint vertex8 = vertex2;
          uint num6 = uint.MaxValue;
          while ((int) num6 == -1)
          {
            if (graph.Distance(n, vertex7) == maxHilbert)
              num6 = vertex7;
            else if (graph.Distance(n, vertex8) == maxHilbert)
            {
              num6 = vertex8;
            }
            else
            {
              while (vertex7 <= vertex8)
              {
                uint vertex6 = vertex7 + (vertex8 - vertex7) / 2U;
                long num5 = graph.Distance(n, vertex6);
                if (num5 > maxHilbert)
                  vertex8 = vertex6;
                else if (num5 < maxHilbert)
                {
                  vertex7 = vertex6;
                }
                else
                {
                  num6 = vertex6;
                  break;
                }
                if ((int) vertex7 == (int) vertex8 || (int) vertex7 == (int) vertex8 - 1)
                  break;
              }
              if ((int) num6 == -1)
              {
                --maxHilbert;
                if (minHilbert == maxHilbert)
                {
                  num6 = num4;
                  break;
                }
              }
            }
          }
          uint num7 = num4;
          while ((int) num7 != 0 && graph.Distance(n, num7 - 1U) == minHilbert)
            --num7;
          uint num8 = num6;
          while ((int) num8 != (int) graph.VertexCount - 1 && graph.Distance(n, num8 + 1U) == maxHilbert)
            ++num8;
          vertex = num7;
          count = (int) num8 - (int) num7 + 1;
          return true;
        }
        if (num1 > num2)
          throw new Exception("Graph not sorted: Binary search using hilbert distance not possible.");
        if (num1 == num2 || (int) vertex1 == (int) vertex2 || (int) vertex1 == (int) vertex2 - 1)
        {
          vertex = vertex1;
          count = 0;
          return true;
        }
      }
      vertex = vertex1;
      count = 0;
      return false;
    }

    public static uint SearchClosest(this GeometricGraph graph, float latitude, float longitude, float latitudeOffset, float longitudeOffset)
    {
      HashSet<uint> uintSet = graph.Search(latitude - latitudeOffset, longitude - longitudeOffset, latitude + latitudeOffset, longitude + longitudeOffset);
      double num1 = double.MaxValue;
      uint num2 = 4294967294;
      foreach (uint vertex in uintSet)
      {
        float latitude1;
        float longitude1;
        if (graph.GetVertex(vertex, out latitude1, out longitude1))
        {
          double num3 = GeoCoordinate.DistanceEstimateInMeter((double) latitude, (double) longitude, (double) latitude1, (double) longitude1);
          if (num3 < num1)
          {
            num1 = num3;
            num2 = vertex;
          }
        }
      }
      return num2;
    }

    public static uint SearchClosestEdge(this GeometricGraph graph, float latitude, float longitude, float latitudeOffset, float longitudeOffset, float maxDistanceMeter, Func<GeometricEdge, bool> isOk)
    {
      GeoCoordinate geoCoordinate = new GeoCoordinate((double) latitude, (double) longitude);
      HashSet<uint> uintSet1 = graph.Search(latitude - latitudeOffset, longitude - longitudeOffset, latitude + latitudeOffset, longitude + longitudeOffset);
      uint num1 = uint.MaxValue;
      double num2 = (double) maxDistanceMeter;
      GeometricGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator();
      foreach (uint vertex1 in uintSet1)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
        double num3 = GeoCoordinate.DistanceEstimateInMeter((double) latitude, (double) longitude, (double) vertex2.Latitude, (double) vertex2.Longitude);
        if (num3 < num2)
        {
          edgeEnumerator.MoveTo(vertex1);
          while (edgeEnumerator.MoveNext())
          {
            if (isOk(edgeEnumerator.Current))
            {
              num2 = num3;
              num1 = edgeEnumerator.Id;
              break;
            }
          }
        }
      }
      GeoCoordinateBox geoCoordinateBox = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) ((double) maxDistanceMeter), DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) ((double) maxDistanceMeter), DirectionEnum.SouthEast));
      HashSet<uint> uintSet2 = new HashSet<uint>();
      foreach (uint vertex1 in uintSet1)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
        if (edgeEnumerator.MoveTo(vertex1) && edgeEnumerator.HasData)
        {
          while (edgeEnumerator.MoveNext())
          {
            if (!uintSet2.Contains(edgeEnumerator.Id))
            {
              uintSet2.Add(edgeEnumerator.Id);
              bool flag = isOk == null;
              ICoordinate coordinate = (ICoordinate) vertex2;
              ShapeBase shapeBase = edgeEnumerator.Shape;
              if (shapeBase != null)
              {
                if (edgeEnumerator.DataInverted)
                  shapeBase = shapeBase.Reverse();
                IEnumerator<ICoordinate> enumerator = shapeBase.GetEnumerator();
                enumerator.Reset();
                while (enumerator.MoveNext())
                {
                  ICoordinate current = enumerator.Current;
                  double num3 = GeoCoordinate.DistanceEstimateInMeter((double) current.Latitude, (double) current.Longitude, (double) latitude, (double) longitude);
                  if (num3 < num2)
                  {
                    if (!flag && isOk(edgeEnumerator.Current))
                      flag = true;
                    if (flag)
                    {
                      num2 = num3;
                      num1 = edgeEnumerator.Id;
                      geoCoordinateBox = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) num2, DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) num2, DirectionEnum.SouthEast));
                    }
                  }
                  if (geoCoordinateBox.IntersectsPotentially((double) coordinate.Longitude, (double) coordinate.Latitude, (double) current.Longitude, (double) current.Latitude))
                  {
                    PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude), new GeoCoordinate((double) current.Latitude, (double) current.Longitude), true, true).ProjectOn((PointF2D) geoCoordinate);
                    if (pointF2D != (PointF2D) null)
                    {
                      double num4 = GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude);
                      if (num4 < num2)
                      {
                        if (!flag && isOk(edgeEnumerator.Current))
                          flag = true;
                        if (flag)
                        {
                          num2 = num4;
                          num1 = edgeEnumerator.Id;
                          geoCoordinateBox = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) num2, DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) num2, DirectionEnum.SouthEast));
                        }
                      }
                    }
                  }
                  coordinate = current;
                }
              }
              ICoordinate vertex3 = (ICoordinate) graph.GetVertex(edgeEnumerator.To);
              if (geoCoordinateBox.IntersectsPotentially((double) coordinate.Longitude, (double) coordinate.Latitude, (double) vertex3.Longitude, (double) vertex3.Latitude))
              {
                PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude), new GeoCoordinate((double) vertex3.Latitude, (double) vertex3.Longitude), true, true).ProjectOn((PointF2D) geoCoordinate);
                if (pointF2D != (PointF2D) null)
                {
                  double num3 = GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude);
                  if (num3 < num2)
                  {
                    if (!flag && isOk(edgeEnumerator.Current))
                      flag = true;
                    if (flag)
                    {
                      num2 = num3;
                      num1 = edgeEnumerator.Id;
                      geoCoordinateBox = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) num2, DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) num2, DirectionEnum.SouthEast));
                    }
                  }
                }
              }
            }
          }
        }
      }
      return num1;
    }

    public static uint[] SearchClosestEdges(this GeometricGraph graph, float latitude, float longitude, float latitudeOffset, float longitudeOffset, float maxDistanceMeter, Func<GeometricEdge, bool>[] isOks)
    {
      GeoCoordinate geoCoordinate = new GeoCoordinate((double) latitude, (double) longitude);
      HashSet<uint> uintSet1 = graph.Search(latitude - latitudeOffset, longitude - longitudeOffset, latitude + latitudeOffset, longitude + longitudeOffset);
      uint[] numArray1 = new uint[isOks.Length];
      double[] numArray2 = new double[isOks.Length];
      for (int index = 0; index < numArray1.Length; ++index)
      {
        numArray1[index] = uint.MaxValue;
        numArray2[index] = (double) maxDistanceMeter;
      }
      GeometricGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator();
      foreach (uint vertex1 in uintSet1)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
        double num = GeoCoordinate.DistanceEstimateInMeter((double) latitude, (double) longitude, (double) vertex2.Latitude, (double) vertex2.Longitude);
        for (int index = 0; index < isOks.Length; ++index)
        {
          if (num < numArray2[index])
          {
            edgeEnumerator.MoveTo(vertex1);
            while (edgeEnumerator.MoveNext())
            {
              if (isOks[index](edgeEnumerator.Current))
              {
                numArray2[index] = num;
                numArray1[index] = edgeEnumerator.Id;
                break;
              }
            }
          }
        }
      }
      GeoCoordinateBox[] boxes = new GeoCoordinateBox[isOks.Length];
      for (int index = 0; index < boxes.Length; ++index)
        boxes[index] = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) ((double) maxDistanceMeter), DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) ((double) maxDistanceMeter), DirectionEnum.SouthEast));
      HashSet<uint> uintSet2 = new HashSet<uint>();
      foreach (uint vertex1 in uintSet1)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
        if (edgeEnumerator.MoveTo(vertex1) && edgeEnumerator.HasData)
        {
          while (edgeEnumerator.MoveNext())
          {
            if (!uintSet2.Contains(edgeEnumerator.Id))
            {
              uintSet2.Add(edgeEnumerator.Id);
              bool[] flagArray = new bool[isOks.Length];
              for (int index = 0; index < isOks.Length; ++index)
                flagArray[index] = isOks[index] == null;
              ICoordinate coordinate = (ICoordinate) vertex2;
              ShapeBase shapeBase = edgeEnumerator.Shape;
              if (shapeBase != null)
              {
                if (edgeEnumerator.DataInverted)
                  shapeBase = shapeBase.Reverse();
                IEnumerator<ICoordinate> enumerator = shapeBase.GetEnumerator();
                enumerator.Reset();
                while (enumerator.MoveNext())
                {
                  ICoordinate current = enumerator.Current;
                  double num1 = GeoCoordinate.DistanceEstimateInMeter((double) current.Latitude, (double) current.Longitude, (double) latitude, (double) longitude);
                  for (int index = 0; index < numArray1.Length; ++index)
                  {
                    if (num1 < numArray2[index])
                    {
                      if (!flagArray[index] && isOks[index](edgeEnumerator.Current))
                        flagArray[index] = true;
                      if (flagArray[index])
                      {
                        numArray2[index] = num1;
                        numArray1[index] = edgeEnumerator.Id;
                        boxes[index] = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) numArray2[index], DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) numArray2[index], DirectionEnum.SouthEast));
                      }
                    }
                  }
                  if (boxes.AnyIntersectsPotentially((double) coordinate.Longitude, (double) coordinate.Latitude, (double) current.Longitude, (double) current.Latitude))
                  {
                    PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude), new GeoCoordinate((double) current.Latitude, (double) current.Longitude), true, true).ProjectOn((PointF2D) geoCoordinate);
                    if (pointF2D != (PointF2D) null)
                    {
                      double num2 = GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude);
                      for (int index = 0; index < numArray1.Length; ++index)
                      {
                        if (num2 < numArray2[index])
                        {
                          if (!flagArray[index] && isOks[index](edgeEnumerator.Current))
                            flagArray[index] = true;
                          if (flagArray[index])
                          {
                            numArray2[index] = num2;
                            numArray1[index] = edgeEnumerator.Id;
                            boxes[index] = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) numArray2[index], DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) numArray2[index], DirectionEnum.SouthEast));
                          }
                        }
                      }
                    }
                  }
                  coordinate = current;
                }
              }
              ICoordinate vertex3 = (ICoordinate) graph.GetVertex(edgeEnumerator.To);
              if (boxes.AnyIntersectsPotentially((double) coordinate.Longitude, (double) coordinate.Latitude, (double) vertex3.Longitude, (double) vertex3.Latitude))
              {
                PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude), new GeoCoordinate((double) vertex3.Latitude, (double) vertex3.Longitude), true, true).ProjectOn((PointF2D) geoCoordinate);
                if (pointF2D != (PointF2D) null)
                {
                  double num = GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude);
                  for (int index = 0; index < isOks.Length; ++index)
                  {
                    if (num < numArray2[index])
                    {
                      if (!flagArray[index] && isOks[index](edgeEnumerator.Current))
                        flagArray[index] = true;
                      if (flagArray[index])
                      {
                        numArray2[index] = num;
                        numArray1[index] = edgeEnumerator.Id;
                        boxes[index] = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) numArray2[index], DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) numArray2[index], DirectionEnum.SouthEast));
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      return numArray1;
    }

    public static List<uint> SearchCloserThan(this GeometricGraph graph, float latitude, float longitude, float offset, float maxDistanceMeter, Func<GeometricEdge, bool> isOk)
    {
      HashSet<uint> uintSet1 = new HashSet<uint>();
      GeoCoordinate geoCoordinate = new GeoCoordinate((double) latitude, (double) longitude);
      HashSet<uint> uintSet2 = graph.Search(latitude, longitude, offset);
      GeometricGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator();
      foreach (uint vertex1 in uintSet2)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
        if (GeoCoordinate.DistanceEstimateInMeter((double) latitude, (double) longitude, (double) vertex2.Latitude, (double) vertex2.Longitude) < (double) maxDistanceMeter)
        {
          edgeEnumerator.MoveTo(vertex1);
          while (edgeEnumerator.MoveNext())
          {
            if (isOk(edgeEnumerator.Current))
            {
              uintSet1.Add(edgeEnumerator.Id);
              break;
            }
          }
        }
      }
      GeoCoordinateBox geoCoordinateBox = new GeoCoordinateBox(new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) ((double) maxDistanceMeter), DirectionEnum.NorthWest), new GeoCoordinate((double) latitude, (double) longitude).OffsetWithDirection((Meter) ((double) maxDistanceMeter), DirectionEnum.SouthEast));
      HashSet<uint> uintSet3 = new HashSet<uint>();
      foreach (uint vertex1 in uintSet2)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
        if (edgeEnumerator.MoveTo(vertex1) && edgeEnumerator.HasData)
        {
          while (edgeEnumerator.MoveNext())
          {
            if (!uintSet3.Contains(edgeEnumerator.Id))
            {
              uintSet3.Add(edgeEnumerator.Id);
              bool flag = isOk == null;
              ICoordinate coordinate = (ICoordinate) vertex2;
              ShapeBase shapeBase = edgeEnumerator.Shape;
              if (shapeBase != null)
              {
                if (edgeEnumerator.DataInverted)
                  shapeBase = shapeBase.Reverse();
                IEnumerator<ICoordinate> enumerator = shapeBase.GetEnumerator();
                enumerator.Reset();
                while (enumerator.MoveNext())
                {
                  ICoordinate current = enumerator.Current;
                  if (GeoCoordinate.DistanceEstimateInMeter((double) current.Latitude, (double) current.Longitude, (double) latitude, (double) longitude) < (double) maxDistanceMeter)
                  {
                    if (!flag && isOk(edgeEnumerator.Current))
                      flag = true;
                    if (flag)
                      uintSet1.Add(edgeEnumerator.Id);
                  }
                  if (geoCoordinateBox.IntersectsPotentially((double) coordinate.Longitude, (double) coordinate.Latitude, (double) current.Longitude, (double) current.Latitude))
                  {
                    PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude), new GeoCoordinate((double) current.Latitude, (double) current.Longitude), true, true).ProjectOn((PointF2D) geoCoordinate);
                    if (pointF2D != (PointF2D) null && GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude) < (double) maxDistanceMeter)
                    {
                      if (!flag && isOk(edgeEnumerator.Current))
                        flag = true;
                      if (flag)
                        uintSet1.Add(edgeEnumerator.Id);
                    }
                  }
                  coordinate = current;
                }
              }
              ICoordinate vertex3 = (ICoordinate) graph.GetVertex(edgeEnumerator.To);
              if (geoCoordinateBox.IntersectsPotentially((double) coordinate.Longitude, (double) coordinate.Latitude, (double) vertex3.Longitude, (double) vertex3.Latitude))
              {
                PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude), new GeoCoordinate((double) vertex3.Latitude, (double) vertex3.Longitude), true, true).ProjectOn((PointF2D) geoCoordinate);
                if (pointF2D != (PointF2D) null && GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude) < (double) maxDistanceMeter)
                {
                  if (!flag && isOk(edgeEnumerator.Current))
                    flag = true;
                  if (flag)
                    uintSet1.Add(edgeEnumerator.Id);
                }
              }
            }
          }
        }
      }
      return new List<uint>((IEnumerable<uint>) uintSet1);
    }

    private static bool AnyIntersectsPotentially(this GeoCoordinateBox[] boxes, double x1, double y1, double x2, double y2)
    {
      for (int index = 0; index < boxes.Length; ++index)
      {
        if (boxes[index].IntersectsPotentially(x1, y1, x2, y2))
          return true;
      }
      return false;
    }
  }
}
