using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Collections
{
  internal class OsmCollectionStreamWriter : OsmStreamTarget
  {
    private readonly ICollection<OsmGeo> _baseObjects;

    public OsmCollectionStreamWriter(ICollection<OsmGeo> baseObjects)
    {
      this._baseObjects = baseObjects;
    }

    public override void Initialize()
    {
    }

    public override void AddNode(Node node)
    {
      if (this._baseObjects == null)
        throw new InvalidOperationException("No target collection set!");
      this._baseObjects.Add((OsmGeo) node);
    }

    public override void AddWay(Way way)
    {
      if (this._baseObjects == null)
        throw new InvalidOperationException("No target collection set!");
      this._baseObjects.Add((OsmGeo) way);
    }

    public override void AddRelation(Relation relation)
    {
      if (this._baseObjects == null)
        throw new InvalidOperationException("No target collection set!");
      this._baseObjects.Add((OsmGeo) relation);
    }
  }
}
