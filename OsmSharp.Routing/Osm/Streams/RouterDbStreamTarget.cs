using OsmSharp.Collections.LongIndex;
using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Network.Data;
using OsmSharp.Routing.Osm.Relations;
using OsmSharp.Routing.Osm.Vehicles;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Osm.Streams
{
    public class RouterDbStreamTarget : OsmStreamTarget
    {
        private readonly RouterDb _db;
        private readonly Vehicle[] _vehicles;
        private readonly bool _allNodesAreCore;
        private readonly int _minimumStages;
        private readonly Func<NodeCoordinatesDictionary> _createNodeCoordinatesDictionary;
        private readonly bool _normalizeTags;
        private bool _firstPass;
        private ILongIndex _allRoutingNodes;
        private ILongIndex _anyStageNodes;
        private ILongIndex _processedWays;
        private NodeCoordinatesDictionary _stageCoordinates;
        private ILongIndex _coreNodes;
        private CoreNodeIdMap _coreNodeIdMap;
        private long _nodeCount;
        private double _minLatitude;
        private double _minLongitude;
        private double _maxLatitude;
        private double _maxLongitude;
        private List<GeoCoordinateBox> _stages;
        private int _stage;

        public List<ITwoPassProcessor> Processors { get; set; }

        public CoreNodeIdMap CoreNodeIdMap
        {
            get
            {
                return this._coreNodeIdMap;
            }
        }

        public RouterDbStreamTarget(RouterDb db, Vehicle[] vehicles, bool allCore = false, int minimumStages = 1, bool normalizeTags = true, IEnumerable<ITwoPassProcessor> processors = null)
        {
            this._db = db;
            this._vehicles = vehicles;
            this._allNodesAreCore = allCore;
            this._normalizeTags = normalizeTags;
            this._createNodeCoordinatesDictionary = (Func<NodeCoordinatesDictionary>)(() => new NodeCoordinatesDictionary());
            this._stageCoordinates = this._createNodeCoordinatesDictionary();
            this._allRoutingNodes = (ILongIndex)new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
            this._anyStageNodes = (ILongIndex)new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
            this._coreNodes = (ILongIndex)new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
            this._coreNodeIdMap = new CoreNodeIdMap();
            this._processedWays = (ILongIndex)new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
            this._minimumStages = minimumStages;
            foreach (Vehicle vehicle in vehicles)
            {
                foreach (Profile profile in vehicle.GetProfiles())
                    db.AddSupportedProfile(profile);
            }
            if (processors == null)
                processors = (IEnumerable<ITwoPassProcessor>)new List<ITwoPassProcessor>();
            this.Processors = new List<ITwoPassProcessor>(processors);
            this.InitializeDefaultProcessors();
        }

        private void InitializeDefaultProcessors()
        {
            Vehicle[] vehicles = this._vehicles;
            Func<Vehicle, bool> func1 = (Func<Vehicle, bool>)(x => x.UniqueName == "Bicycle");
            if (((IEnumerable<Vehicle>)vehicles).FirstOrDefault<Vehicle>(func1) == null)
                return;
            List<ITwoPassProcessor> processors = this.Processors;
            Func<ITwoPassProcessor, bool> func2 = (Func<ITwoPassProcessor, bool>)(x => x.GetType().Equals(typeof(CycleNetworkProcessor)));
            if (processors.FirstOrDefault<ITwoPassProcessor>(func2) != null)
                return;
            this.Processors.Add((ITwoPassProcessor)new CycleNetworkProcessor());
        }

        public override void Initialize()
        {
            this._firstPass = true;
        }

        public override bool OnBeforePull()
        {
            this.DoPull(false, false, false);
            this._stage = 0;
            this._firstPass = false;
            while (this._stage < this._stages.Count)
            {
                this.Source.Reset();
                this.DoPull(false, false, false);
                this._stage = this._stage + 1;
                this._stageCoordinates = this._createNodeCoordinatesDictionary();
            }
            return false;
        }

        public virtual void RegisterSource(OsmStreamSource source, bool filterNonRoutingTags)
        {
            if (filterNonRoutingTags)
            {
                OsmStreamFilterWithEvents filterWithEvents = new OsmStreamFilterWithEvents();
                // ISSUE: method pointer
                filterWithEvents.MovedToNextEvent += (OsmGeo osmGeo, object param) =>
                {
                    if (osmGeo.Type == OsmGeoType.Way)
                    {
                        foreach (Tag tag in new TagsCollection(osmGeo.Tags))
                        {
                            bool flag = false;
                            int num = 0;
                            while (num < (int)this._vehicles.Length)
                            {
                                if (!this._vehicles[num].IsRelevant(tag.Key, tag.Value))
                                {
                                    num++;
                                }
                                else
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                continue;
                            }
                            osmGeo.Tags.RemoveKeyValue(tag);
                        }
                    }
                    return osmGeo;
                };
                ((OsmStreamFilter)filterWithEvents).RegisterSource(source);
                base.RegisterSource((OsmStreamSource)filterWithEvents);
            }
            else
                base.RegisterSource(source);
        }

        public override void RegisterSource(OsmStreamSource source)
        {
            this.RegisterSource(source, true);
        }

        public override void AddNode(Node node)
        {
            if (this._firstPass)
            {
                this._nodeCount = this._nodeCount + 1L;
                double num1 = node.Latitude.Value;
                if (num1 < this._minLatitude)
                    this._minLatitude = num1;
                if (num1 > this._maxLatitude)
                    this._maxLatitude = num1;
                double num2 = node.Longitude.Value;
                if (num2 < this._minLongitude)
                    this._minLongitude = num2;
                if (num2 > this._maxLongitude)
                    this._maxLongitude = num2;
                if (this.Processors == null)
                    return;
                foreach (ITwoPassProcessor processor in this.Processors)
                    processor.FirstPass(node);
            }
            else
            {
                if (this.Processors != null)
                {
                    foreach (ITwoPassProcessor processor in this.Processors)
                        processor.SecondPass(node);
                }
                long? id1;
                if (!this._stages[this._stage].Contains(node.Longitude.Value, node.Latitude.Value))
                {
                    ILongIndex anyStageNodes = this._anyStageNodes;
                    id1 = ((OsmGeo)node).Id;
                    long number = id1.Value;
                    if (!anyStageNodes.Contains(number))
                        return;
                }
                ILongIndex allRoutingNodes = this._allRoutingNodes;
                id1 = ((OsmGeo)node).Id;
                long number1 = id1.Value;
                if (!allRoutingNodes.Contains(number1))
                    return;
                NodeCoordinatesDictionary stageCoordinates = this._stageCoordinates;
                id1 = ((OsmGeo)node).Id;
                long id2 = id1.Value;
                // ISSUE: variable of a boxed type
                GeoCoordinateSimple local = new GeoCoordinateSimple()
                {
                    Latitude = (float)node.Latitude.Value,
                    Longitude = (float)node.Longitude.Value
                };
                stageCoordinates.Add(id2, (ICoordinate)local);
            }
        }

        public override void AddWay(Way way)
        {
            ICoordinate coordinate;
            EdgeData data;

            if (way == null)
            {
                return;
            }

            if (way.Nodes == null)
            {
                return;
            }

            if (way.Nodes.Count == 0)
            {
                return;
            }

            if (!this._firstPass)
            {
                if (this.Processors != null)
                {
                    foreach (ITwoPassProcessor processor in this.Processors)
                    {
                        processor.SecondPass(way);
                    }
                }
                if (this._vehicles.AnyCanTraverse(way.Tags))
                {
                    if (this._processedWays.Contains(way.Id.Value))
                    {
                        return;
                    }
                    TagsCollection tagsCollection = new TagsCollection(way.Tags.Count);
                    TagsCollection tagsCollection1 = new TagsCollection(way.Tags.Count);
                    foreach (Tag tag in way.Tags)
                    {
                        if (!this._vehicles.IsRelevantForProfile(tag.Key))
                        {
                            tagsCollection1.Add(tag);
                        }
                        else
                        {
                            tagsCollection.Add(tag);
                        }
                    }
                    if (this._normalizeTags)
                    {
                        TagsCollection tagsCollection2 = new TagsCollection(tagsCollection.Count);
                        if (!tagsCollection.Normalize(tagsCollection2, tagsCollection1, this._vehicles))
                        {
                            return;
                        }
                        if (this.Processors != null)
                        {
                            foreach (ITwoPassProcessor twoPassProcessor in this.Processors)
                            {
                                Action<TagsCollectionBase, TagsCollectionBase> onAfterWayTagsNormalize = twoPassProcessor.OnAfterWayTagsNormalize;
                                if (onAfterWayTagsNormalize == null)
                                {
                                    continue;
                                }
                                onAfterWayTagsNormalize.Invoke(tagsCollection2, tagsCollection);
                            }
                        }
                        tagsCollection = tagsCollection2;
                    }
                    uint num = this._db.EdgeProfiles.Add(tagsCollection);
                    if (num > 16384)
                    {
                        throw new Exception("Maximum supported profiles exeeded, make sure only routing tags are included in the profiles.");
                    }
                    uint num1 = this._db.EdgeMeta.Add(tagsCollection1);
                    int num2 = 0;
                    Label0:
                    while (num2 < way.Nodes.Count - 1)
                    {
                        List<ICoordinate> list = new List<ICoordinate>();
                        float single = 0f;
                        if (!this._stageCoordinates.TryGetValue(way.Nodes[num2], out coordinate))
                        {
                            for (int i = 0; i < way.Nodes.Count; i++)
                            {
                                this._anyStageNodes.Add(way.Nodes[i]);
                            }
                            return;
                        }
                        uint from = this.AddCoreNode(way.Nodes[num2], coordinate.Latitude, coordinate.Longitude);
                        long item = way.Nodes[num2];
                        ICoordinate coordinate1 = coordinate;
                        num2++;
                        long to = -1;
                        long item1 = 9223372036854775807L;
                        while (this._stageCoordinates.TryGetValue(way.Nodes[num2], out coordinate))
                        {
                            single = single + (float)GeoCoordinate.DistanceEstimateInMeter(coordinate1, coordinate);
                            if (!this._coreNodes.Contains(way.Nodes[num2]))
                            {
                                list.Add(coordinate);
                                coordinate1 = coordinate;
                                num2++;
                            }
                            else
                            {
                                to = this.AddCoreNode(way.Nodes[num2], coordinate.Latitude, coordinate.Longitude);
                                item1 = way.Nodes[num2];
                                if (from != to)
                                {
                                    RoutingEdge routingEdge = Enumerable.FirstOrDefault<RoutingEdge>(this._db.Network.GetEdgeEnumerator(from), (RoutingEdge x) => x.To == to);
                                    if (routingEdge != null || from == to)
                                    {
                                        data = routingEdge.Data;
                                        if (data.Distance == single)
                                        {
                                            data = routingEdge.Data;
                                            if (data.Profile == num)
                                            {
                                                data = routingEdge.Data;
                                                if (data.MetaId == num1)
                                                {
                                                    goto Label0;
                                                }
                                            }
                                        }
                                        uint metaId = num1;
                                        uint profile = num;
                                        float distance = single;
                                        if (list.Count == 0 && routingEdge != null && routingEdge.Shape != null)
                                        {
                                            list = new List<ICoordinate>(routingEdge.Shape);
                                            from = routingEdge.From;
                                            to = routingEdge.To;
                                            data = routingEdge.Data;
                                            metaId = data.MetaId;
                                            data = routingEdge.Data;
                                            profile = data.Profile;
                                            data = routingEdge.Data;
                                            distance = data.Distance;
                                            long num3 = to;
                                            data = new EdgeData()
                                            {
                                                MetaId = num1,
                                                Distance = System.Math.Max(single, 0f),
                                                Profile = (ushort)num
                                            };
                                            this.AddCoreEdge(from, (uint)num3, data, null);
                                        }
                                        if (list.Count <= 0)
                                        {
                                            GeoCoordinateSimple vertex = this._db.Network.GetVertex(from);
                                            uint num4 = this.AddNewCoreNode(item, vertex.Latitude, vertex.Longitude);
                                            data = new EdgeData()
                                            {
                                                Distance = 0f,
                                                MetaId = metaId,
                                                Profile = (ushort)profile
                                            };
                                            this.AddCoreEdge(from, num4, data, null);
                                            GeoCoordinateSimple geoCoordinateSimple = this._db.Network.GetVertex((uint)to);
                                            uint num5 = this.AddNewCoreNode(item1, geoCoordinateSimple.Latitude, geoCoordinateSimple.Longitude);
                                            long num6 = to;
                                            data = new EdgeData()
                                            {
                                                Distance = 0f,
                                                MetaId = metaId,
                                                Profile = (ushort)profile
                                            };
                                            this.AddCoreEdge(num5, (uint)num6, data, null);
                                            data = new EdgeData()
                                            {
                                                Distance = distance,
                                                MetaId = metaId,
                                                Profile = (ushort)profile
                                            };
                                            this.AddCoreEdge(num4, num5, data, null);
                                            goto Label0;
                                        }
                                        else
                                        {
                                            uint vertexCount = this._db.Network.VertexCount;
                                            this._db.Network.AddVertex(vertexCount, list[0].Latitude, list[0].Longitude);
                                            float single1 = (float)GeoCoordinate.DistanceEstimateInMeter(this._db.Network.GetVertex(from), list[0]);
                                            distance = distance - single1;
                                            data = new EdgeData()
                                            {
                                                MetaId = metaId,
                                                Distance = System.Math.Max(single1, 0f),
                                                Profile = (ushort)profile
                                            };
                                            this.AddCoreEdge(from, vertexCount, data, null);
                                            list.RemoveAt(0);
                                            long num7 = to;
                                            data = new EdgeData()
                                            {
                                                MetaId = metaId,
                                                Distance = System.Math.Max(distance, 0f),
                                                Profile = (ushort)profile
                                            };
                                            this.AddCoreEdge(vertexCount, (uint)num7, data, list);
                                            goto Label0;
                                        }
                                    }
                                    else
                                    {
                                        long num8 = to;
                                        data = new EdgeData()
                                        {
                                            MetaId = num1,
                                            Distance = single,
                                            Profile = (ushort)num
                                        };
                                        this.AddCoreEdge(from, (uint)num8, data, list);
                                        goto Label0;
                                    }
                                }
                                else if (list.Count != 1)
                                {
                                    if (list.Count < 2)
                                    {
                                        goto Label0;
                                    }
                                    uint vertexCount1 = this._db.Network.VertexCount;
                                    this._db.Network.AddVertex(vertexCount1, list[0].Latitude, list[0].Longitude);
                                    uint vertexCount2 = this._db.Network.VertexCount;
                                    this._db.Network.AddVertex(vertexCount2, list[list.Count() - 1].Latitude, list[list.Count - 1].Longitude);
                                    float single2 = (float)GeoCoordinate.DistanceEstimateInMeter(this._db.Network.GetVertex(from), list[0]);
                                    float single3 = (float)GeoCoordinate.DistanceEstimateInMeter(this._db.Network.GetVertex((uint)to), list[list.Count - 1]);
                                    list.RemoveAt(0);
                                    list.RemoveAt(list.Count - 1);
                                    data = new EdgeData()
                                    {
                                        MetaId = num1,
                                        Distance = single2,
                                        Profile = (ushort)num
                                    };
                                    this.AddCoreEdge(from, vertexCount1, data, null);
                                    data = new EdgeData()
                                    {
                                        MetaId = num1,
                                        Distance = single - single3 - single2,
                                        Profile = (ushort)num
                                    };
                                    this.AddCoreEdge(vertexCount1, vertexCount2, data, list);
                                    uint num9 = (uint)to;
                                    data = new EdgeData()
                                    {
                                        MetaId = num1,
                                        Distance = single3,
                                        Profile = (ushort)num
                                    };
                                    this.AddCoreEdge(vertexCount2, num9, data, null);
                                    goto Label0;
                                }
                                else
                                {
                                    uint vertexCount3 = this._db.Network.VertexCount;
                                    this._db.Network.AddVertex(vertexCount3, list[0].Latitude, list[0].Longitude);
                                    data = new EdgeData()
                                    {
                                        MetaId = num1,
                                        Distance = (float)GeoCoordinate.DistanceEstimateInMeter(this._db.Network.GetVertex(from), list[0]),
                                        Profile = (ushort)num
                                    };
                                    this.AddCoreEdge(from, vertexCount3, data, null);
                                    goto Label0;
                                }
                            }
                        }
                        for (int j = 0; j < way.Nodes.Count; j++)
                        {
                            this._anyStageNodes.Add(way.Nodes[j]);
                        }
                        return;
                    }
                    this._processedWays.Add(way.Id.Value);
                }
            }
            else
            {
                if (this.Processors != null)
                {
                    foreach (ITwoPassProcessor processor1 in this.Processors)
                    {
                        processor1.FirstPass(way);
                    }
                }
                GeoCoordinateBox geoCoordinateBox = new GeoCoordinateBox(new GeoCoordinate(this._minLatitude, this._minLongitude), new GeoCoordinate(this._maxLatitude, this._maxLongitude));
                double num10 = 1E-05;
                if (this._stages.Count == 0)
                {
                    if (this._nodeCount > (long)500000000 || this._minimumStages > 1)
                    {
                        double num11 = System.Math.Max(System.Math.Ceiling((double)(this._nodeCount / (long)500000000)), (double)this._minimumStages);
                        if (num11 >= 4)
                        {
                            num11 = 4;
                            this._stages.Add(new GeoCoordinateBox(new GeoCoordinate(this._minLatitude, this._minLongitude), new GeoCoordinate(geoCoordinateBox.Center.Latitude, geoCoordinateBox.Center.Longitude)));
                            this._stages[0] = this._stages[0].Resize(num10);
                            this._stages.Add(new GeoCoordinateBox(new GeoCoordinate(this._minLatitude, geoCoordinateBox.Center.Longitude), new GeoCoordinate(geoCoordinateBox.Center.Latitude, this._maxLongitude)));
                            this._stages[1] = this._stages[1].Resize(num10);
                            this._stages.Add(new GeoCoordinateBox(new GeoCoordinate(geoCoordinateBox.Center.Latitude, this._minLongitude), new GeoCoordinate(this._maxLatitude, geoCoordinateBox.Center.Longitude)));
                            this._stages[2] = this._stages[2].Resize(num10);
                            this._stages.Add(new GeoCoordinateBox(new GeoCoordinate(geoCoordinateBox.Center.Latitude, geoCoordinateBox.Center.Longitude), new GeoCoordinate(this._maxLatitude, this._maxLongitude)));
                            this._stages[3] = this._stages[3].Resize(num10);
                        }
                        else if (num11 < 2)
                        {
                            num11 = 1;
                            this._stages.Add(geoCoordinateBox);
                            this._stages[0] = this._stages[0].Resize(num10);
                        }
                        else
                        {
                            num11 = 2;
                            this._stages.Add(new GeoCoordinateBox(new GeoCoordinate(this._minLatitude, this._minLongitude), new GeoCoordinate(this._maxLatitude, geoCoordinateBox.Center.Longitude)));
                            this._stages[0] = this._stages[0].Resize(num10);
                            this._stages.Add(new GeoCoordinateBox(new GeoCoordinate(this._minLatitude, geoCoordinateBox.Center.Longitude), new GeoCoordinate(this._maxLatitude, this._maxLongitude)));
                            this._stages[1] = this._stages[1].Resize(num10);
                        }
                    }
                    else
                    {
                        this._stages.Add(geoCoordinateBox);
                        this._stages[0] = this._stages[0].Resize(num10);
                    }
                }
                if (this._vehicles.AnyCanTraverse(way.Tags))
                {
                    for (int k = 0; k < way.Nodes.Count; k++)
                    {
                        long item2 = way.Nodes[k];
                        if (this._allRoutingNodes.Contains(item2) || this._allNodesAreCore)
                        {
                            this._coreNodes.Add(item2);
                        }
                        this._allRoutingNodes.Add(item2);
                    }
                    this._coreNodes.Add(way.Nodes[0]);
                    this._coreNodes.Add(way.Nodes[way.Nodes.Count - 1]);
                    return;
                }
            }
        }

        private uint AddCoreNode(long node, float latitude, float longitude)
        {
            uint vertex = uint.MaxValue;
            if (this._coreNodeIdMap.TryGetFirst(node, out vertex))
                return vertex;
            return this.AddNewCoreNode(node, latitude, longitude);
        }

        private uint AddNewCoreNode(long node, float latitude, float longitude)
        {
            uint vertexCount = this._db.Network.VertexCount;
            this._db.Network.AddVertex(vertexCount, latitude, longitude);
            this._coreNodeIdMap.Add(node, vertexCount);
            return vertexCount;
        }

        public void AddCoreEdge(uint vertex1, uint vertex2, EdgeData data, List<ICoordinate> shape)
        {
            if ((double)data.Distance < (double)this._db.Network.MaxEdgeDistance)
            {
                int num1 = (int)this._db.Network.AddEdge(vertex1, vertex2, data, (IEnumerable<ICoordinate>)shape);
            }
            else
            {
                if (shape == null)
                    shape = new List<ICoordinate>();
                shape = new List<ICoordinate>((IEnumerable<ICoordinate>)shape);
                shape.Insert(0, (ICoordinate)this._db.Network.GetVertex(vertex1));
                shape.Add((ICoordinate)this._db.Network.GetVertex(vertex2));
                for (int index = 1; index < shape.Count; ++index)
                {
                    if (GeoCoordinate.DistanceEstimateInMeter(shape[index - 1], shape[index]) >= (double)this._db.Network.MaxEdgeDistance)
                    {
                        shape.Insert(index, (ICoordinate)new GeoCoordinateSimple()
                        {
                            Latitude = (float)(((double)shape[index - 1].Latitude + (double)shape[index].Latitude) / 2.0),
                            Longitude = (float)(((double)shape[index - 1].Longitude + (double)shape[index].Longitude) / 2.0)
                        });
                        --index;
                    }
                }
                int num2 = 0;
                List<ICoordinate> coordinateList1 = new List<ICoordinate>();
                float num3 = 0.0f;
                int index1 = num2 + 1;
                EdgeData edgeData;
                while (index1 < shape.Count)
                {
                    float num4 = (float)GeoCoordinate.DistanceEstimateInMeter(shape[index1 - 1], shape[index1]);
                    if ((double)num4 + (double)num3 > (double)this._db.Network.MaxEdgeDistance)
                    {
                        ICoordinate coordinate = coordinateList1[coordinateList1.Count - 1];
                        coordinateList1.RemoveAt(coordinateList1.Count - 1);
                        uint vertexCount = this._db.Network.VertexCount;
                        this._db.Network.AddVertex(vertexCount, coordinate.Latitude, coordinate.Longitude);
                        RoutingNetwork network = this._db.Network;
                        int num5 = (int)vertex1;
                        int num6 = (int)vertexCount;
                        edgeData = new EdgeData();
                        edgeData.Distance = num3;
                        edgeData.MetaId = data.MetaId;
                        edgeData.Profile = data.Profile;
                        EdgeData data1 = edgeData;
                        List<ICoordinate> coordinateList2 = coordinateList1;
                        int num7 = (int)network.AddEdge((uint)num5, (uint)num6, data1, (IEnumerable<ICoordinate>)coordinateList2);
                        vertex1 = vertexCount;
                        coordinateList1.Clear();
                        coordinateList1.Add(shape[index1]);
                        num3 = num4;
                        ++index1;
                    }
                    else
                    {
                        coordinateList1.Add(shape[index1]);
                        num3 += num4;
                        ++index1;
                    }
                }
                if (coordinateList1.Count > 0)
                    coordinateList1.RemoveAt(coordinateList1.Count - 1);
                RoutingNetwork network1 = this._db.Network;
                int num8 = (int)vertex1;
                int num9 = (int)vertex2;
                edgeData = new EdgeData();
                edgeData.Distance = num3;
                edgeData.MetaId = data.MetaId;
                edgeData.Profile = data.Profile;
                EdgeData data2 = edgeData;
                List<ICoordinate> coordinateList3 = coordinateList1;
                int num10 = (int)network1.AddEdge((uint)num8, (uint)num9, data2, (IEnumerable<ICoordinate>)coordinateList3);
            }
        }

        public override void AddRelation(Relation relation)
        {
            if (this._firstPass)
            {
                if (this.Processors == null)
                    return;
                foreach (ITwoPassProcessor processor in this.Processors)
                    processor.FirstPass(relation);
            }
            else
            {
                if (this.Processors == null)
                    return;
                foreach (ITwoPassProcessor processor in this.Processors)
                    processor.SecondPass(relation);
            }
        }
    }
}
