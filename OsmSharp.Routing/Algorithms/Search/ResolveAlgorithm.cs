using OsmSharp.Geo;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graphs.Geometric;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Search
{
  public class ResolveAlgorithm : AlgorithmBase, IResolver, IAlgorithm
  {
    public static int BetterEdgeThreshold = 50;
    public static float BetterEdgeFactor = 2f;
    private readonly GeometricGraph _graph;
    private readonly float _latitude;
    private readonly float _longitude;
    private readonly float _maxOffsetInMeter;
    private readonly float _maxDistance;
    private readonly Func<GeometricEdge, bool> _isAcceptable;
    private readonly Func<GeometricEdge, bool> _isBetter;
    private RouterPoint _result;

    public RouterPoint Result
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._result;
      }
    }

    public ResolveAlgorithm(GeometricGraph graph, float latitude, float longitude, float maxOffset, float maxDistance, Func<GeometricEdge, bool> isAcceptable)
      : this(graph, latitude, longitude, maxOffset, maxDistance, isAcceptable, (Func<GeometricEdge, bool>) null)
    {
    }

    public ResolveAlgorithm(GeometricGraph graph, float latitude, float longitude, float maxOffsetInMeter, float maxDistance, Func<GeometricEdge, bool> isAcceptable, Func<GeometricEdge, bool> isBetter)
    {
      this._graph = graph;
      this._latitude = latitude;
      this._longitude = longitude;
      this._maxDistance = maxDistance;
      this._maxOffsetInMeter = maxOffsetInMeter;
      this._isAcceptable = isAcceptable;
      this._isBetter = isBetter;
    }

    protected override void DoRun()
    {
      GeoCoordinate geoCoordinate = new GeoCoordinate((double) this._latitude, (double) this._longitude).OffsetWithDistances((Meter) ((double) this._maxOffsetInMeter));
      float latitudeOffset = (float) System.Math.Abs((double) this._latitude - geoCoordinate[1]);
      float longitudeOffset = (float) System.Math.Abs((double) this._longitude - geoCoordinate[0]);
      uint[] numArray;
      if (this._isBetter == null)
        numArray = new uint[2]
        {
          this._graph.SearchClosestEdge(this._latitude, this._longitude, latitudeOffset, longitudeOffset, this._maxDistance, this._isAcceptable),
          0U
        };
      else
        numArray = this._graph.SearchClosestEdges(this._latitude, this._longitude, latitudeOffset, longitudeOffset, this._maxDistance, new Func<GeometricEdge, bool>[2]
        {
          this._isAcceptable,
          (Func<GeometricEdge, bool>) (potentialEdge =>
          {
            if (this._isAcceptable(potentialEdge))
              return this._isBetter(potentialEdge);
            return false;
          })
        });
      if ((int) numArray[0] == -1)
      {
        this.ErrorMessage = string.Format("Could not resolve point at [{0}, {1}]. Probably too far from closest road or outside of the loaded network.", new object[2]
        {
          (object) this._latitude.ToInvariantString(),
          (object) this._longitude.ToInvariantString()
        });
      }
      else
      {
        GeometricEdge edge1 = this._graph.GetEdge(numArray[0]);
        uint edgeId = numArray[0];
        float projectedLatitude1;
        float projectedLongitude1;
        float projectedDistanceFromFirst1;
        int projectedShapeIndex1;
        float distanceToProjected1;
        float totalLength1;
        if (!this._graph.ProjectOn(edge1, this._latitude, this._longitude, out projectedLatitude1, out projectedLongitude1, out projectedDistanceFromFirst1, out projectedShapeIndex1, out distanceToProjected1, out totalLength1))
        {
          List<ICoordinate> shape = this._graph.GetShape(edge1);
          ICoordinate coordinate1 = shape[0];
          float num1 = 0.0f;
          float num2 = 0.0f;
          float num3 = (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate1.Latitude, (double) coordinate1.Longitude, (double) this._latitude, (double) this._longitude);
          projectedLatitude1 = coordinate1.Latitude;
          projectedLongitude1 = coordinate1.Longitude;
          for (int index = 1; index < shape.Count; ++index)
          {
            ICoordinate coordinate2 = shape[index];
            num2 += (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate2.Latitude, (double) coordinate2.Longitude, (double) coordinate1.Latitude, (double) coordinate1.Longitude);
            distanceToProjected1 = (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate2.Latitude, (double) coordinate2.Longitude, (double) this._latitude, (double) this._longitude);
            if ((double) distanceToProjected1 < (double) num3)
            {
              num3 = distanceToProjected1;
              num1 = num2;
              projectedLatitude1 = coordinate2.Latitude;
              projectedLongitude1 = coordinate2.Longitude;
            }
            coordinate1 = coordinate2;
          }
          projectedDistanceFromFirst1 = num1;
        }
        if (this._isBetter != null && (int) numArray[0] != (int) numArray[1] && (int) numArray[1] != -1)
        {
          GeometricEdge edge2 = this._graph.GetEdge(numArray[1]);
          float projectedLatitude2;
          float projectedLongitude2;
          float projectedDistanceFromFirst2;
          int projectedShapeIndex2;
          float distanceToProjected2;
          float totalLength2;
          if (!this._graph.ProjectOn(edge2, this._latitude, this._longitude, out projectedLatitude2, out projectedLongitude2, out projectedDistanceFromFirst2, out projectedShapeIndex2, out distanceToProjected2, out totalLength2))
          {
            List<ICoordinate> shape = this._graph.GetShape(edge2);
            ICoordinate coordinate1 = shape[0];
            float num1 = 0.0f;
            float num2 = 0.0f;
            float num3 = (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate1.Latitude, (double) coordinate1.Longitude, (double) this._latitude, (double) this._longitude);
            projectedLatitude2 = coordinate1.Latitude;
            projectedLongitude2 = coordinate1.Longitude;
            for (int index = 1; index < shape.Count; ++index)
            {
              ICoordinate coordinate2 = shape[index];
              num2 += (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate2.Latitude, (double) coordinate2.Longitude, (double) coordinate1.Latitude, (double) coordinate1.Longitude);
              distanceToProjected2 = (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate2.Latitude, (double) coordinate2.Longitude, (double) this._latitude, (double) this._longitude);
              if ((double) distanceToProjected2 < (double) num3)
              {
                num3 = distanceToProjected2;
                num1 = num2;
                projectedLatitude2 = coordinate2.Latitude;
                projectedLongitude2 = coordinate2.Longitude;
              }
              coordinate1 = coordinate2;
            }
            projectedDistanceFromFirst2 = num1;
          }
          if ((double) distanceToProjected2 <= (double) ResolveAlgorithm.BetterEdgeThreshold || (double) distanceToProjected2 <= (double) distanceToProjected1 * (double) ResolveAlgorithm.BetterEdgeFactor)
          {
            totalLength1 = totalLength2;
            edgeId = numArray[1];
            projectedLatitude1 = projectedLatitude2;
            projectedLongitude1 = projectedLongitude2;
            projectedDistanceFromFirst1 = projectedDistanceFromFirst2;
          }
        }
        ushort offset = (ushort) ((double) projectedDistanceFromFirst1 / (double) totalLength1 * (double) ushort.MaxValue);
        this._result = new RouterPoint(projectedLatitude1, projectedLongitude1, edgeId, offset);
        this.HasSucceeded = true;
      }
    }
  }
}
