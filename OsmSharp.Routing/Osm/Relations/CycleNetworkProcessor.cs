using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using System;

namespace OsmSharp.Routing.Osm.Relations
{
  public class CycleNetworkProcessor : RelationTagProcessor
  {
    private static Func<Relation, bool> IsRelevant = new Func<Relation, bool>((Relation r) => {
        if (!r.Tags.ContainsKeyValue("type", "route"))
        {
            return false;
        }
        return r.Tags.ContainsKeyValue("route", "bicycle");
    });

    private static Action<Way, TagsCollectionBase> AddTags = new Action<Way, TagsCollectionBase>((Way w, TagsCollectionBase t) => w.Tags.AddOrReplace("cyclenetwork", "yes"));

        public override Action<TagsCollectionBase, TagsCollectionBase> OnAfterWayTagsNormalize
    {
      get
      {
        return (Action<TagsCollectionBase, TagsCollectionBase>) ((after, before) =>
        {
          if (!before.ContainsKeyValue("cyclenetwork", "yes"))
            return;
          after.AddOrReplace("cyclenetwork", "yes");
        });
      }
    }

    public CycleNetworkProcessor()
      : base(CycleNetworkProcessor.IsRelevant, CycleNetworkProcessor.AddTags)
    {
    }
  }
}
