using OsmSharp.Geo;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graphs.Geometric;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Search
{
  public class ResolveMultipleAlgorithm : AlgorithmBase
  {
    private readonly GeometricGraph _graph;
    private readonly float _latitude;
    private readonly float _longitude;
    private readonly float _maxOffset;
    private readonly float _maxDistance;
    private readonly Func<GeometricEdge, bool> _isAcceptable;
    private List<RouterPoint> _results;

    public List<RouterPoint> Results
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._results;
      }
    }

    public ResolveMultipleAlgorithm(GeometricGraph graph, float latitude, float longitude, float maxOffset, float maxDistance, Func<GeometricEdge, bool> isAcceptable)
    {
      this._graph = graph;
      this._latitude = latitude;
      this._longitude = longitude;
      this._maxDistance = maxDistance;
      this._maxOffset = maxOffset;
      this._isAcceptable = isAcceptable;
    }

    protected override void DoRun()
    {
      this._results = new List<RouterPoint>();
      List<uint> uintList = this._graph.SearchCloserThan(this._latitude, this._longitude, this._maxOffset, this._maxDistance, this._isAcceptable);
      if (uintList.Count == 0)
      {
        this.ErrorMessage = string.Format("Could not resolve point at [{0}, {1}]. Probably too far from closest road or outside of the loaded network.", new object[2]
        {
          (object) this._latitude.ToInvariantString(),
          (object) this._longitude.ToInvariantString()
        });
      }
      else
      {
        for (int index1 = 0; index1 < uintList.Count; ++index1)
        {
          uint edgeId = uintList[index1];
          GeometricEdge edge = this._graph.GetEdge(edgeId);
          float projectedLatitude;
          float projectedLongitude;
          float projectedDistanceFromFirst;
          int projectedShapeIndex;
          float distanceToProjected;
          float totalLength;
          if (!this._graph.ProjectOn(edge, this._latitude, this._longitude, out projectedLatitude, out projectedLongitude, out projectedDistanceFromFirst, out projectedShapeIndex, out distanceToProjected, out totalLength))
          {
            List<ICoordinate> shape = this._graph.GetShape(edge);
            ICoordinate coordinate1 = shape[0];
            float num1 = 0.0f;
            float num2 = 0.0f;
            float num3 = (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate1.Latitude, (double) coordinate1.Longitude, (double) this._latitude, (double) this._longitude);
            projectedLatitude = coordinate1.Latitude;
            projectedLongitude = coordinate1.Longitude;
            for (int index2 = 1; index2 < shape.Count; ++index2)
            {
              ICoordinate coordinate2 = shape[index2];
              num2 += (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate2.Latitude, (double) coordinate2.Longitude, (double) coordinate1.Latitude, (double) coordinate1.Longitude);
              distanceToProjected = (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate2.Latitude, (double) coordinate2.Longitude, (double) this._latitude, (double) this._longitude);
              if ((double) distanceToProjected < (double) num3)
              {
                num3 = distanceToProjected;
                num1 = num2;
                projectedLatitude = coordinate2.Latitude;
                projectedLongitude = coordinate2.Longitude;
              }
              coordinate1 = coordinate2;
            }
            projectedDistanceFromFirst = num1;
          }
          ushort offset = (ushort) ((double) projectedDistanceFromFirst / (double) totalLength * (double) ushort.MaxValue);
          this._results.Add(new RouterPoint(projectedLatitude, projectedLongitude, edgeId, offset));
        }
        this.HasSucceeded = true;
      }
    }
  }
}
