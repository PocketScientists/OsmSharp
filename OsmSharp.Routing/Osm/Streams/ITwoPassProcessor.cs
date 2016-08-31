using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using System;

namespace OsmSharp.Routing.Osm.Streams
{
  public interface ITwoPassProcessor
  {
    Action<TagsCollectionBase, TagsCollectionBase> OnAfterWayTagsNormalize { get; }

    void FirstPass(Node node);

    void FirstPass(Way way);

    void FirstPass(Relation relation);

    void SecondPass(Node node);

    void SecondPass(Way way);

    void SecondPass(Relation relation);
  }
}
