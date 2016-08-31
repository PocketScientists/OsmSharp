using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Routing.Osm.Streams;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Relations
{
  public class RelationTagProcessor : ITwoPassProcessor
  {
    private readonly Action<Way, TagsCollectionBase> _addTags;
    private readonly Func<Relation, bool> _isRelevant;
    private readonly Dictionary<long, RelationTagProcessor.LinkedRelation> _linkedRelations;
    private readonly Dictionary<long, TagsCollectionBase> _relationTags;

    public virtual Action<TagsCollectionBase, TagsCollectionBase> OnAfterWayTagsNormalize
    {
      get
      {
        return (Action<TagsCollectionBase, TagsCollectionBase>) null;
      }
    }

    public RelationTagProcessor(Func<Relation, bool> isRelevant, Action<Way, TagsCollectionBase> addTags)
    {
      this._addTags = addTags;
      this._isRelevant = isRelevant;
      this._linkedRelations = new Dictionary<long, RelationTagProcessor.LinkedRelation>();
      this._relationTags = new Dictionary<long, TagsCollectionBase>();
    }

    public void FirstPass(Relation relation)
    {
      if (!this._isRelevant(relation))
        return;
      Dictionary<long, TagsCollectionBase> relationTags = this._relationTags;
      long? nullable = ((OsmGeo) relation).Id;
      long index1 = nullable.Value;
      TagsCollectionBase tags = ((OsmGeo) relation).Tags;
      relationTags[index1] = tags;
      if (relation.Members == null)
        return;
      using (List<RelationMember>.Enumerator enumerator = relation.Members.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          RelationMember current = enumerator.Current;
          OsmGeoType? memberType = current.MemberType;
          OsmGeoType osmGeoType = (OsmGeoType) 1;
          if ((memberType.GetValueOrDefault() == osmGeoType ? (memberType.HasValue ? 1 : 0) : 0) != 0)
          {
            RelationTagProcessor.LinkedRelation linkedRelation1 = (RelationTagProcessor.LinkedRelation) null;
            Dictionary<long, RelationTagProcessor.LinkedRelation> linkedRelations1 = this._linkedRelations;
            nullable = current.MemberId;
            long key = nullable.Value;
            linkedRelations1.TryGetValue(key, out linkedRelation1);
            Dictionary<long, RelationTagProcessor.LinkedRelation> linkedRelations2 = this._linkedRelations;
            nullable = current.MemberId;
            long index2 = nullable.Value;
            RelationTagProcessor.LinkedRelation linkedRelation2 = new RelationTagProcessor.LinkedRelation();
            linkedRelation2.Next = linkedRelation1;
            nullable = ((OsmGeo) relation).Id;
            long num = nullable.Value;
            linkedRelation2.RelationId = num;
            linkedRelations2[index2] = linkedRelation2;
          }
        }
      }
    }

    public void FirstPass(Way way)
    {
    }

    public void FirstPass(Node node)
    {
    }

    public void SecondPass(Relation relation)
    {
    }

    public void SecondPass(Way way)
    {
      RelationTagProcessor.LinkedRelation next;
      if (!this._linkedRelations.TryGetValue(((OsmGeo) way).Id.Value, out next))
        return;
      for (; next != null; next = next.Next)
      {
        TagsCollectionBase relationTag = this._relationTags[next.RelationId];
        this._addTags(way, relationTag);
      }
    }

    public void SecondPass(Node node)
    {
    }

    private class LinkedRelation
    {
      public long RelationId { get; set; }

      public RelationTagProcessor.LinkedRelation Next { get; set; }
    }
  }
}
